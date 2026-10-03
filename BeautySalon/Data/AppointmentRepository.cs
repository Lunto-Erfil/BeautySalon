using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Data
{
    public class AppointmentRepository
    {
        public DataTable GetUpcoming()
        {
            var table = new DataTable();

            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
SELECT s.Title AS ServiceTitle,
       c.LastName + N' ' + c.FirstName + ISNULL(N' ' + c.Patronymic, N'') AS ClientFullName,
       c.Email, c.Phone, cs.StartTime
FROM ClientService cs
JOIN Service s ON s.ID = cs.ServiceID
JOIN Client  c ON c.ID = cs.ClientID
WHERE cs.StartTime >= CAST(GETDATE() AS date)
  AND cs.StartTime <  DATEADD(day, 2, CAST(GETDATE() AS date))
ORDER BY cs.StartTime ASC", conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(table);
            }

            // Добавляем вычисляемые столбцы
            table.Columns.Add("RemainingTime", typeof(string));
            table.Columns.Add("IsUrgent", typeof(bool));

            foreach (DataRow row in table.Rows)
            {
                var start = (DateTime)row["StartTime"];
                var left = start - DateTime.Now;

                row["RemainingTime"] = left.TotalMinutes < 0
                    ? "просрочено"
                    : $"{(int)left.TotalHours} ч {left.Minutes} мин";

                row["IsUrgent"] = left.TotalMinutes >= 0 && left.TotalHours < 1;
            }

            return table;
        }

        public void Create(int clientId, int serviceId, DateTime startTime, string comment = null)
        {
            using (var conn = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
INSERT INTO ClientService (ClientID, ServiceID, StartTime, Comment)
VALUES (@clientId, @serviceId, @startTime, @comment)", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@clientId", clientId);
                cmd.Parameters.AddWithValue("@serviceId", serviceId);
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@comment", (object)comment ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
    }
}