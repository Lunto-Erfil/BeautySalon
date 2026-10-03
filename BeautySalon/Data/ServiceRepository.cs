using BeautySalon.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Data
{
    public class ServiceRepository
    {
        public List<Service> Search(string text, double? minDiscount, double? maxDiscount, bool sortAscending)
        {
            var result = new List<Service>();

            using (var conn = Db.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
SELECT ID, Title, Cost, DurationInSeconds, Description, Discount, MainImagePath
FROM Service
WHERE (@search = N'' OR Title LIKE @searchLike OR Description LIKE @searchLike)
  AND (@min IS NULL OR Discount >= @min)
  AND (@max IS NULL OR Discount < @max)";

                cmd.Parameters.AddWithValue("@search", text ?? string.Empty);
                cmd.Parameters.AddWithValue("@searchLike", "%" + (text ?? string.Empty) + "%");
                cmd.Parameters.AddWithValue("@min", (object)minDiscount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@max", (object)maxDiscount ?? DBNull.Value);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Service
                        {
                            ID = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Cost = reader.GetDecimal(2),
                            DurationInSeconds = reader.GetInt32(3),
                            Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Discount = reader.IsDBNull(5) ? (double?)null : reader.GetDouble(5),
                            MainImagePath = reader.IsDBNull(6) ? null : reader.GetString(6)
                        });
                    }
                }
            }
            result.Sort((a, b) => sortAscending
                ? a.Cost.CompareTo(b.Cost)
                : b.Cost.CompareTo(a.Cost));

            return result;
        }

        public int GetTotalCount()
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Service", conn))
            {
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public Service GetById(int id)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(
                @"SELECT ID, Title, Cost, DurationInSeconds, Description, Discount, MainImagePath
                  FROM Service WHERE ID = @id", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    return new Service
                    {
                        ID = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Cost = reader.GetDecimal(2),
                        DurationInSeconds = reader.GetInt32(3),
                        Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Discount = reader.IsDBNull(5) ? (double?)null : reader.GetDouble(5),
                        MainImagePath = reader.IsDBNull(6) ? null : reader.GetString(6)
                    };
                }
            }
        }

        public bool TitleExists(string title, int exceptId)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Service WHERE Title = @title AND ID <> @id", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@id", exceptId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public int Insert(Service s)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
INSERT INTO Service (Title, Cost, DurationInSeconds, Description, Discount, MainImagePath)
VALUES (@title, @cost, @duration, @description, @discount, @image);
SELECT SCOPE_IDENTITY();", conn))
            {
                conn.Open();
                AddParameters(cmd, s);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void Update(Service s)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
UPDATE Service
SET Title = @title, Cost = @cost, DurationInSeconds = @duration,
    Description = @description, Discount = @discount, MainImagePath = @image
WHERE ID = @id", conn))
            {
                conn.Open();
                AddParameters(cmd, s);
                cmd.Parameters.AddWithValue("@id", s.ID);
                cmd.ExecuteNonQuery();
            }
        }

        private static void AddParameters(SqlCommand cmd, Service s)
        {
            cmd.Parameters.AddWithValue("@title", s.Title);
            cmd.Parameters.AddWithValue("@cost", s.Cost);
            cmd.Parameters.AddWithValue("@duration", s.DurationInSeconds);
            cmd.Parameters.AddWithValue("@description", (object)s.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@discount", (object)s.Discount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@image", (object)s.MainImagePath ?? DBNull.Value);
        }

        public bool HasAppointments(int serviceId)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM ClientService WHERE ServiceID = @id", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@id", serviceId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public void Delete(int id)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Service WHERE ID = @id", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<ServicePhoto> GetPhotos(int serviceId)
        {
            var list = new List<ServicePhoto>();

            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT ID, ServiceID, PhotoPath FROM ServicePhoto WHERE ServiceID = @id", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@id", serviceId);

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(new ServicePhoto
                        {
                            ID = reader.GetInt32(0),
                            ServiceID = reader.GetInt32(1),
                            PhotoPath = reader.GetString(2)
                        });
            }

            return list;
        }

        public int AddPhoto(int serviceId, string path)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(
                "INSERT INTO ServicePhoto (ServiceID, PhotoPath) VALUES (@s, @p); SELECT SCOPE_IDENTITY();", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@s", serviceId);
                cmd.Parameters.AddWithValue("@p", path);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void DeletePhoto(int photoId)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM ServicePhoto WHERE ID = @id", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@id", photoId);
                cmd.ExecuteNonQuery();
            }
        }
        public static string CopyImageToProject(string sourcePath)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Services");
            Directory.CreateDirectory(dir);

            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(sourcePath);
            var destPath = Path.Combine(dir, fileName);
            File.Copy(sourcePath, destPath, true);

            return Path.Combine("Images", "Services", fileName);
        }
    }
}
