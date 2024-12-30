using api_intranet_surfland.Controllers;
using api_intranet_surfland.DataBaseConnection;
using api_intranet_surfland.Models;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace api_intranet_surfland.DataBase;
public class DBUsers {
    public static DTOUser GetUser(int userId) {
        try {
            MySqlConnection conn = DBConnect.Connection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Users u WHERE u.UserID = @param_UserId", conn);
            cmd.Parameters.AddWithValue("@param_UserId", userId);
            MySqlDataReader reader = cmd.ExecuteReader();

            DTOUser user = new DTOUser();

            while (reader.Read()) {
                user.UserId = reader.GetInt32("UserId");
                user.Login = reader.GetString("Login");
                user.Password = reader.GetString("Password");
                user.AccessProfileId = reader.GetInt32("AccessProfileId");
                user.StatusId = reader.GetInt32("StatusId");
                user.TemporaryPassword = reader.IsDBNull(reader.GetOrdinal("TemporaryPassword")) ? null : reader.GetString("TemporaryPassword");
                user.PersonId = reader.GetInt32("PersonId");
                user.UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime("UpdatedAt");
                user.UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32("UpdatedBy");
            }

            conn.Close();
            return user;
        } catch (MySqlException ex) {
            Console.WriteLine("MySQL Error Code: " + ex.Number);
            Console.WriteLine("MySQL Error Message: " + ex.Message);
            throw;
        } catch (Exception ex) {
            Console.WriteLine(DateTime.Now.ToString() + " * ERROR DBUsers.GetUser * " + ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public static List<DTOUser> FindUsers(
        string? login = null,
        string? accessProfileId = null,
        int? statusId = null,
        int? personId = null,
        DateTime? updatedAt = null,
        int? updatedBy = null
    ) {
        try {
            List<string> conditions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (login is not null) {
                conditions.Add("u.Login like @param_Login");
                parameters.Add("@param_Login", "%" + login + "%");
            }
            if (accessProfileId is not null) {
                conditions.Add("u.AccessProfileId like @param_AccessProfileId");
                parameters.Add("@param_AccessProfileId", "%" + accessProfileId + "%");
            }
            if (statusId is not null) {
                conditions.Add("u.StatusId = @param_StatusId");
                parameters.Add("@param_StatusId", statusId);
            }
            if (personId is not null) {
                conditions.Add("u.PersonId = @param_PersonId");
                parameters.Add("@param_PersonId", personId);
            }
            if (updatedAt is not null) {
                conditions.Add("u.UpdatedAt = @param_UpdatedAt");
                parameters.Add("@param_UpdatedAt", updatedAt);
            }
            if (updatedBy is not null) {
                conditions.Add("u.UpdatedBy = @param_UpdatedBy");
                parameters.Add("@param_UpdatedBy", updatedBy);
            }

            string sqlCommand = "SELECT * FROM Users u";

            if (conditions.Any()) {
                sqlCommand += " WHERE " + string.Join(" AND ", conditions);
            }

            MySqlConnection conn = DBConnect.Connection();
            MySqlCommand cmd = new MySqlCommand(sqlCommand, conn);

            foreach (var param in parameters) {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            MySqlDataReader reader = cmd.ExecuteReader();

            List<DTOUser> users = new List<DTOUser>();

            while (reader.Read()) {
                users.Add(new DTOUser {
                    UserId = reader.GetInt32("UserId"),
                    Login = reader.GetString("Login"),
                    Password = reader.GetString("Password"),
                    AccessProfileId = reader.GetInt32("AccessProfileId"),
                    StatusId = reader.GetInt32("StatusId"),
                    TemporaryPassword = reader.IsDBNull(reader.GetOrdinal("TemporaryPassword")) ? null : reader.GetString("TemporaryPassword"),
                    PersonId = reader.GetInt32("PersonId"),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime("UpdatedAt"),
                    UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32("UpdatedBy")
                });
            }

            conn.Close();
            return users;
        } catch (Exception ex) {
            Console.WriteLine(DateTime.Now.ToString() + " * ERROR DBUsers.FindPersons * " + ex.Message);
            throw new Exception(ex.Message);
        }
    }
}