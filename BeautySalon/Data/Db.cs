using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Data
{
    public static class Db
    {
        public static string ConnectionString =
            @"Data Source=.;Initial Catalog=BeautySalon;Integrated Security=True;";

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);
    }
}
