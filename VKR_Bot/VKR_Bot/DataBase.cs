using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR_Bot
{
    internal class DataBase
    {
        public SqlConnection sqlConnection;
        public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ПК\source\repos\VKR_Bot\VKR_Bot\Database1.mdf;Integrated Security=True";
        public void ConnectSql()
        {
            sqlConnection = new SqlConnection(connectionString);
        }
    }
}
