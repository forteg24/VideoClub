using System;
using System.Configuration;
using System.Data.SqlClient;
using Videoclub.helpers;

namespace Videoclub
{
    class User
    {
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public DateTime UserBirthdate { get; set; }

        public User() { }

        public User(string userName, string userSurname, DateTime userBirthdate, string userEmail, string userPassword)
        {
            UserName = userName;
            UserSurname = userSurname;
            UserBirthdate = userBirthdate;
            UserEmail = userEmail;
            UserPassword = userPassword;
        }

        static string connectionString = ConfigurationManager.ConnectionStrings["VIDEOCLUB"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);

        public void Login()
        {
            bool stayLogin = true;
            do
            {
                Console.WriteLine("\n\tLOGIN");
                Console.Write("\n\tEnter your email address: (Empty to register)\n\t");
                UserEmail = Console.ReadLine();
                if (String.IsNullOrEmpty(UserEmail))
                {
                    Register();
                }
                else
                {
                    if (CheckEmailExist(UserEmail))
                    {
                        Console.Write("\n\tEnter password: \n\t");
                        string password = ReadPassword();
                        UserPassword = PasswordHelper.EncodePasswordMd5(password);

                        if (CheckPassword(UserEmail, UserPassword))
                        {
                            FillUserData(UserEmail);
                            stayLogin = false;
                            Program.ShowCorrectMessage($"\n\tWelcome {UserName} {UserSurname}, you are logged in.", 1500);
                        }
                        else
                        {
                            Program.ShowError("\n\tThe password is not correct.");
                        }
                    }
                    else
                    {
                        Program.ShowError("\n\tThere are no matches with any registered email.");
                    }
                }
            } while (stayLogin);
        }

        public bool CheckPassword(string userEmail, string userPassword)
        {
            connection.Open();
            string query = $"SELECT * FROM Users WHERE Email='{userEmail}' and Password='{userPassword}'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader user = command.ExecuteReader();
            if (user.Read())
            {
                connection.Close();
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }

        public void FillUserData(string userEmail)
        {
            connection.Open();
            string query = $"SELECT * FROM Users WHERE Email='{userEmail}'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader user = command.ExecuteReader();
            if (user.Read())
            {
                UserName = user["Name"].ToString();
                UserSurname = user["Surname"].ToString();
                UserBirthdate = Convert.ToDateTime(user["Birthdate"]);
            }
            else
            {
                Program.ShowError("\n\tThere has been a problem with the request to the database. Contact the administrator.");
            }
            connection.Close();
        }

        //Register

        public void Register()
        {
            Console.Clear();
            Console.WriteLine("\n\tREGISTRO");

            //Name
            bool validName = false;
            do
            {
                Console.Write("\n\tIntroduce name: ");
                UserName = Console.ReadLine();

                if (!String.IsNullOrEmpty(UserName))
                {
                    validName = true;
                }
                else
                {
                    Program.ShowError("\n\tThe name cannot be empty.");
                }
            } while (validName == false);

            //Surname
            bool validSurname = false;
            do
            {
                Console.Write("\n\tIntroduce surname: ");
                UserSurname = Console.ReadLine();

                if (!String.IsNullOrEmpty(UserSurname))
                {
                    validSurname = true;
                }
                else
                {
                    Program.ShowError("\n\tThe surname cannot be empty.");
                }
            } while (validSurname == false);

            //Birthdate
            bool validBirthdate = false;
            UserBirthdate = Convert.ToDateTime("01/01/0001");
            do
            {
                Console.Write("\n\tIntroduce birthdate: (DD-MM-YYYY) \n\t");
                string birthdate = Console.ReadLine();

                try
                {
                    UserBirthdate = Convert.ToDateTime(birthdate);
                    validBirthdate = true;
                }
                catch
                {
                    Program.ShowError("\n\tThe birthdate is not valid.");
                }
            } while (validBirthdate == false);

            //Email
            bool validEmail = false;
            do
            {
                Console.Write("\n\tIntroduce email address: ");
                UserEmail = Console.ReadLine();

                if (IsValidEmail(UserEmail))
                {
                    validEmail = true;
                }
                else
                {
                    Program.ShowError("\n\tThe email format is not valid.");
                }
            } while (validEmail == false);

            //Password
            bool validPassword = false;
            UserPassword = "";
            do
            {
                Console.Write("\n\tIntroduce password: (At least 8 characters)\n\t");
                string password = ReadPassword();

                if (password.Length >= 8)
                {
                    validPassword = true;
                    UserPassword = PasswordHelper.EncodePasswordMd5(password);
                }
                else
                {
                    Program.ShowError("\n\tThe password must have 8 characters at least.");
                }
            } while (validPassword == false);

            //Confirm password
            //Console.Write("\n\tConfirm password:");

            if (CheckEmailExist(UserEmail))
            {
                Program.ShowError("\n\tRegistration not completed. There is already a registered user with that email.");
            }
            else
            {
                InsertUser(UserName, UserSurname, UserBirthdate, UserEmail, UserPassword);

                Program.ShowCorrectMessage("\n\tRegistration is complete.");
            }

        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckEmailExist(string email)
        {
            connection.Open();
            string query = $"SELECT * FROM Users WHERE Email='{email}'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader user = command.ExecuteReader();
            if (user.Read())
            {
                connection.Close();
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }

        public void InsertUser(string userName, string userSurname, DateTime userBirthdate, string userEmail, string userPassword)
        {
            connection.Open();

            string query = $"INSERT INTO Users (Name, Surname, Birthdate, Email, Password) values ('{userName}', '{userSurname}', '{userBirthdate}', '{userEmail}', '{userPassword}')";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
    }
}
