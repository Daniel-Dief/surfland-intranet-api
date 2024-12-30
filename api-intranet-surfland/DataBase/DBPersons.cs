using api_intranet_surfland.Controllers;
using api_intranet_surfland.DataBaseConnection;
using api_intranet_surfland.Models;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace api_intranet_surfland.DataBase;
public class DBPersons {
    public static DTOPerson GetPerson(int PersonId) {
        try {
            MySqlConnection conn = DBConnect.Connection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Persons p WHERE p.PersonId = @param_PersonId", conn);
            cmd.Parameters.AddWithValue("@param_PersonId", PersonId);
            MySqlDataReader reader = cmd.ExecuteReader();

            DTOPerson person = new DTOPerson();

            while (reader.Read()) {
                person.PersonId = reader.GetInt32("PersonId");
                person.Name = reader.GetString("Name");
                person.Document = reader.GetString("Document");
                person.BirthDate = reader.GetDateTime("BirthDate");
                person.Email = reader.GetString("Email");
                person.Phone = reader.GetString("Phone");
                person.Foreigner = reader.GetBoolean("Foreigner");
            }

            conn.Close();
            return person;
        } catch (Exception ex) {
            Console.WriteLine(DateTime.Now.ToString() + " * ERROR DBPersons.GetPerson * " + ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public static List<DTOPerson> FindPersons(
            string? name = null,
            string? document = null,
            string? birthDate = null,
            string? email = null,
            string? phone = null,
            string? foreigner = null
    ) {
        try {
            List<string> conditions = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(name)) {
                conditions.Add("p.Name like @param_Name");
                parameters.Add("@param_Name", "%" + name + "%");
            }
            if (!string.IsNullOrEmpty(document)) {
                conditions.Add("p.Document like @param_Document");
                parameters.Add("@param_Document", "%" + document + "%");
            }
            if (!string.IsNullOrEmpty(birthDate)) {
                conditions.Add("p.BirthDate = @param_BirthDate");
                parameters.Add("@param_BirthDate", birthDate);
            }
            if (!string.IsNullOrEmpty(email)) {
                conditions.Add("p.Email like @param_Email");
                parameters.Add("@param_Email", "%" + email + "%");
            }
            if (!string.IsNullOrEmpty(phone)) {
                conditions.Add("p.Phone like @param_Phone");
                parameters.Add("@param_Phone", "%" + phone + "%");
            }
            if (!string.IsNullOrEmpty(foreigner)) {
                conditions.Add("p.Foreigner = @param_Foreigner");
                parameters.Add("@param_Foreigner", foreigner);
            }

            string sqlCommand = "SELECT * FROM Persons p";

            if (conditions.Any()) {
                sqlCommand += " WHERE " + string.Join(" AND ", conditions);
            }

            MySqlConnection conn = DBConnect.Connection();
            MySqlCommand cmd = new MySqlCommand(sqlCommand, conn);

            foreach (var param in parameters) {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            MySqlDataReader reader = cmd.ExecuteReader();

            List<DTOPerson> persons = new List<DTOPerson>();

            while (reader.Read()) {
                persons.Add(new DTOPerson {
                    PersonId = reader.GetInt32("PersonId"),
                    Name = reader.GetString("Name"),
                    Document = reader.GetString("Document"),
                    BirthDate = reader.GetDateTime("BirthDate"),
                    Email = reader.GetString("Email"),
                    Phone = reader.GetString("Phone"),
                    Foreigner = reader.GetBoolean("Foreigner")
                });
            }

            conn.Close();
            return persons;
        } catch (Exception ex) {
            Console.WriteLine(DateTime.Now.ToString() + " * ERROR DBPersons.FindPersons * " + ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public static DTOPerson CreatePerson(DTOPerson person) {
        try {
            MySqlConnection conn = DBConnect.Connection();
            MySqlCommand cmd = new MySqlCommand("INSERT INTO Persons(Name, Document, BirthDate, Email, Phone, Foreigner) VALUES(@param_Name, @param_Document, @param_BirthDate, @param_Email, @param_Phone, @param_Foreigner)", conn);
            cmd.Parameters.AddWithValue("@param_Name", person.Name);
            cmd.Parameters.AddWithValue("@param_Document", person.Document);
            cmd.Parameters.AddWithValue("@param_BirthDate", person.BirthDate);
            cmd.Parameters.AddWithValue("@param_Email", person.Email);
            cmd.Parameters.AddWithValue("@param_Phone", person.Phone);
            cmd.Parameters.AddWithValue("@param_Foreigner", person.Foreigner);
            cmd.ExecuteNonQuery();
            conn.Close();

            int createdPersonId = (int)cmd.LastInsertedId;

            return GetPerson(createdPersonId);

        } catch (Exception ex) {
            Console.WriteLine(DateTime.Now.ToString() + " * ERROR DBPersons.CreatePerson* " + ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public static DTOPerson UpdatePerson(DTOPerson person) {
        try {
            List<string> fields = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (person.Name is not null) {
                fields.Add("Name = @param_Name");
                parameters.Add("@param_Name", person.Name);
            }
            if (person.Document is not null) {
                fields.Add("Document = @param_Document");
                parameters.Add("@param_Document", person.Document);
            }
            if (person.BirthDate is not null) {
                fields.Add("BirthDate = @param_BirthDate");
                parameters.Add("@param_BirthDate", person.BirthDate);
            }
            if (person.Email is not null) {
                fields.Add("Email = @param_Email");
                parameters.Add("@param_Email", person.Email);
            }
            if (person.Phone is not null) {
                fields.Add("Phone = @param_Phone");
                parameters.Add("@param_Phone", person.Phone);
            }
            if (person.Foreigner is not null) {
                fields.Add("Foreigner = @param_Foreigner");
                parameters.Add("@param_Foreigner", person.Foreigner);
            }

            string sqlCommand;

            if (fields.Any()) {
                sqlCommand = "UPDATE Persons SET " + string.Join(", ", fields) + " WHERE PersonId = @param_PersonId";
            } else {
                Console.WriteLine(DateTime.Now.ToString() + " * ERROR DBPersons.UpdatePerson * " + "All fields are null");
                throw new Exception("Nenhum campo para atualizar");
            }

            MySqlConnection conn = DBConnect.Connection();
            MySqlCommand cmd = new MySqlCommand(sqlCommand, conn);

            foreach (var param in parameters) {
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@param_PersonId", person.PersonId);


            cmd.ExecuteNonQuery();
            conn.Close();
            return GetPerson(person.PersonId ?? 0);

        } catch (MySqlException ex) {
            Console.WriteLine("MySQL Error Code: " + ex.Number);
            Console.WriteLine("MySQL Error Message: " + ex.Message);
            throw;
        } catch (Exception ex) {
            Console.WriteLine(DateTime.Now.ToString() + " * ERROR DBPersons.UpdatePerson * " + ex.Message);
            throw new Exception(ex.Message);
        }
    }
}