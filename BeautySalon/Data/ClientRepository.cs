using BeautySalon.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Data
{
    public class ClientRepository
    {
        public List<Client> GetAll()
        {
            var list = new List<Client>();

            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT ID, FirstName, LastName, Patronymic, Email, Phone FROM Client ORDER BY LastName", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(new Client
                        {
                            ID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Patronymic = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Phone = reader.GetString(5)
                        });
            }

            return list;
        }
    }
}
