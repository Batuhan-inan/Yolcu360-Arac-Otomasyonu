using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arac_Kiralama
{
    public static class DbHelper
    {
        private static string connectionString = "Server=localhost;Database=yolcu360;Uid=root;Pwd=root;";
        public static IDbConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        internal static object QueryFirstOrDefault<T>(string sql, object value)
        {
            // 1. GetConnection() ile MySQL bağlantısını alıyoruz 
            using (var conn = GetConnection())
            {
                // 2. Dapper'ın QueryFirstOrDefault metodunu çağırıp sonucu döndürüyoruz 
                return conn.QueryFirstOrDefault<T>(sql, value);
            }
        }
    }
}
