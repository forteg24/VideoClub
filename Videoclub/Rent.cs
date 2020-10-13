using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Videoclub
{
    class Rent
    {
        public int Id { get; set; }
        public string CodTitle { get; set; }
        public string CodEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public Rent() { }

        public Rent(int id, string codTitle, string codEmail, DateTime startDate, DateTime endDate)
        {
            Id = id;
            CodTitle = codTitle;
            CodEmail = codEmail;
            StartDate = startDate;
            EndDate = endDate;
        }

        static string connectionString = ConfigurationManager.ConnectionStrings["VIDEOCLUB"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);

        List<Rent> rents = new List<Rent>();

        static public bool NewRent(string title, string userEmail)
        {

            Console.Clear();
            Console.Write($"\n\tDays you want to rent {title}:\n\t");
            string days = Console.ReadLine();
            int day = -1;
            try
            {
                day = Convert.ToInt32(days);
            }
            catch
            {
                Program.ShowError("\n\tThe answer must by a positive number, more than 0.");
            }
            if (day > 0)
            {
                DateTime endDate = DateTime.Now.AddDays(day);
                DateTime startDate = DateTime.Now;
                connection.Open();

                string query = $"INSERT INTO Rent (CodTitle, CodEmail, StartDate, EndDate) values ('{title}', '{userEmail}', '{startDate}', '{endDate}')";
                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                connection.Close();
                return true;
            }
            else
            {
                Program.ShowError("\n\tThe answer must by a positive number, that is more than 0.");
                return false;
            }
        }

        public void MyRents(string userEmail)
        {

            bool stayRent;
            do
            {
                connection.Open();
                string query = $"SELECT * FROM Rent WHERE CodEmail = '{userEmail}' and DeliveryDate is null order by Id asc";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader register = command.ExecuteReader();

                while (register.Read())
                {
                    rents.Add(new Rent()
                    {
                        Id = Convert.ToInt32(register["Id"]),
                        CodTitle = Convert.ToString(register["CodTitle"]),
                        CodEmail = Convert.ToString(register["CodEmail"]),
                        StartDate = Convert.ToDateTime(register["StartDate"]),
                        EndDate = Convert.ToDateTime(register["EndDate"])
                    });
                }
                connection.Close();

                Console.Clear();
                Console.WriteLine("\n\tRented films\n");

                foreach (var r in rents)
                {
                    if (r.EndDate < DateTime.Now)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\t{r.Id}. {r.CodTitle}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.WriteLine($"\t{r.Id}. {r.CodTitle}");
                    }
                }

                stayRent = true;
                Console.Write("\n\tSelect the movie to return it. Empty to exit.\n\t");

                string rentAnswer = Console.ReadLine();

                if (rentAnswer == "")
                {
                    stayRent = false;
                }
                else
                {
                    int? numberRentAnswer = null;
                    try
                    {
                        numberRentAnswer = Convert.ToInt32(rentAnswer);
                    }
                    catch
                    {
                        Program.ShowError("\n\tThe answer must by empty or a number.");
                    }

                    if (numberRentAnswer != null)
                    {
                        Rent rent = new Rent();
                        try
                        {
                            rent = rents.Where(x => x.Id == numberRentAnswer).FirstOrDefault();
                            Film.UpdateFilm(rent.CodTitle, "available");
                            UpdateRent(rent.CodTitle);
                            Program.ShowCorrectMessage($"\n\tYou succcessfully return {rent.CodTitle}");
                        }
                        catch
                        {
                            Program.ShowError("\n\tThe number is not valid.");
                        }

                    }
                }

                rents.Clear();

            } while (stayRent);

            Console.Clear();
        }

        public void UpdateRent(string title)
        {
            DateTime now = DateTime.Now;
            connection.Open();

            string query = $"UPDATE Rent Set DeliveryDate = '{now}' WHERE CodTitle = '{title}'";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
