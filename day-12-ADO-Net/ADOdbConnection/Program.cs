using Npgsql;
using System.Data;
using System.Net.NetworkInformation;

namespace UnderstandingADOApp
{
    
    internal class Program
    {
        string connectionString =
            "Host=localhost;Port=5432;Database=Dummydb;Username=nandhiraja;Password=";
        private NpgsqlConnection connection;

        public Program()
        {
          connection = new NpgsqlConnection(connectionString);
           
        }
        void GetProductDataFromDatabase()
        {
            string selectQuery = "Select * from products";
            NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection);
            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("Product Id : " + reader[0].ToString());
                    Console.WriteLine("Product Name : " + reader[1].ToString());
                }
                Console.WriteLine("Done reading");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
            }

        }

        void InsertUserDate()
        {
            User? user = GetUserFromConsole();
            if(user == null)
            {
                return;
            }
            string InsertString = $"INSERT INTO Users VALUES ('{user.UserName}','{user.Password}','{user.Role}')";
            NpgsqlCommand command = new NpgsqlCommand(InsertString,connection);
            try
            {
                 connection.Open();
                 int value = command.ExecuteNonQuery();
                 Console.Write("Inser Successfully");

            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
            finally
            {
                connection.Close();
            }


        }

        List<String>? GetNewUserPassword()
        {
            List<string> data = new List<string>();
            Console.WriteLine("Enter the UserId/UserName : ");
            string user_id= Console.ReadLine()??"";
            Console.WriteLine("Enter new password: ");
            string newPassword = Console.ReadLine()??"";

            if (newPassword != "")
            {
                data.Add(user_id);
                data.Add(newPassword);
                return data;
            }
            return null;

        }   
        void UpateUserPassword()
        {
            List<string>? data = GetNewUserPassword();
            if(data== null) return;

            string updateString = $"Update  Users  set password='{data[1]}' where userId = '{data[0]}'";
            NpgsqlCommand updateCommand= new NpgsqlCommand(updateString,connection);
            try
            {
                connection.Open();
                int val = updateCommand.ExecuteNonQuery();
                Console.WriteLine("Updated "+data[1]);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error "+e);
            }
            finally
            {
                connection.Close();
            }
        }
        User? GetUserFromConsole()
        {
            User newUser= new User();

            Console.WriteLine("Enter name: ");
            string name = Console.ReadLine()??"";
            Console.WriteLine("Enter Password: ");
            string password = Console.ReadLine()??"";  
            Console.WriteLine("Enter role: ");
            string role = Console.ReadLine()??"";
            if(name!="" && password!="" && role != "")
            {
                newUser.UserName = name;
                newUser.Password = password;
                newUser.Role = role;
            
            return newUser;
            }
            return null;
        }

        void UpdateUserToDatabase(User user, string field)
        {
            string updateCmd = "";
            if (field == "password")
                updateCmd = $"update  Users set user_password = '{user.Password}' where user_id='{user.UserName}'";
            else if (field == "role")
                updateCmd = $"update  Users set user_role = '{user.Role}' where user_id='{user.UserName}'";
            else
                throw new Exception("Invalid column to update");

            NpgsqlCommand command = new NpgsqlCommand(updateCmd, connection);
            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();
                if (result > 0)
                    Console.WriteLine("User details updated");
                else
                    Console.WriteLine("No such user");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection?.Close();
            }
        }
        void InitiateUserDataUpdate()
        {
            Console.WriteLine("Please enter the user id you want to update");
            User user = new User();
            user.UserName = Console.ReadLine() ?? "";
            Console.WriteLine("Please enter the field you want to update Password/Role");
            
            string option = Console.ReadLine()??"".ToLower();
            if(option =="role")
            {
                Console.WriteLine("Please enter the new value for role");
                user.Role = Console.ReadLine() ?? "";
            }
            else if(option == "password")
            {
                Console.WriteLine("Please enter the new value for password");
                user.Password = Console.ReadLine() ?? "";
            }
            try
            {
                UpdateUserToDatabase(user, option);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            new Program().InsertUserDate();

        }
    }
    class User
    {
        public string UserName {get; set;} = string.Empty;
        public string Password {get;set;} = string.Empty;
        public string Role {get;set;} = string.Empty;

        
    }
}