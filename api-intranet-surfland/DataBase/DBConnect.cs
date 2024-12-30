using api_intranet_surfland.Models;
using MySql.Data.MySqlClient;

namespace api_intranet_surfland.DataBaseConnection;

public class DBConnect {
    public static MySqlConnection Connection() {
        try {
            DotNetEnv.Env.Load();
            string connString = Environment.GetEnvironmentVariable("MYSQL_STRING_CONNECTION");

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            return conn;
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
}
