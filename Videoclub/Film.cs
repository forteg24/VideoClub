using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Videoclub
{
    class Film
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public int RecommendedAge { get; set; }
        public string State { get; set; }

        public Film() { }

        static string connectionString = ConfigurationManager.ConnectionStrings["VIDEOCLUB"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);

        List<Film> films = new List<Film>();
        
        public void Catalogue(DateTime userBirthdate)
        {
            int edad = DateTime.Today.AddTicks(-userBirthdate.Ticks).Year - 1;

            LoadFilms();

            bool stayCatalogue;
            do
            {
                ManageCataloque(ref stayCatalogue);
            } 
            while (stayCatalogue);
            
            films.Clear();
            Console.Clear();
        }
        
        private void LoadFilms()
        {
            connection.Open();
            
            // *? Think wheather you are selecting more than you need
            string query = $"SELECT * FROM Films WHERE RecommendedAge <= '{edad}' order by Id asc";
            
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader register = command.ExecuteReader();

            while (register.Read())
            {
                films.Add(new Film()
                {
                    Id = Convert.ToInt32(register["Id"]),
                    Title = Convert.ToString(register["Title"]),
                    Synopsis = Convert.ToString(register["Synopsis"]),
                    RecommendedAge = Convert.ToInt32(register["RecommendedAge"]),
                    State = Convert.ToString(register["State"])
                });
            }
            
            connection.Close();    
        }
        
        private void ManageCatalogue(ref bool stayCatalogue)
        {
            Console.Clear();
            Console.WriteLine("\n\tCatalogue\n");

            foreach (var f in films)
            {
                Console.WriteLine($"\t{f.Id}. {f.Title}");
            }

            stayCatalogue = true;
            Console.Write("\n\tSelect the movie for which you want to see the data. Empty to exit.\n\t");

            string catalogueAnswer = Console.ReadLine();

            if (catalogueAnswer == "")
            {
                stayCatalogue = false;
            }
            else
            {
                ShowFilmInformation(Convert.ToInt32(catalogueAnswer));
            }   
        }
        
        private ShowFilmInformation(int filmNumber)
        {
            int? numberCatalogueAnswer = null;
            try
            {
                numberCatalogueAnswer = filmNumber;
            }
            catch
            {
                Program.ShowError("\n\tThe answer must by empty or a number.");
            }

            if (numberCatalogueAnswer != null)
            {
                Film film = new Film();
                try
                {
                    film = films.Where(x => x.Id == numberCatalogueAnswer).FirstOrDefault();

                    Console.Clear();
                    Console.WriteLine($"\n\tTitle: {film.Title}\n\n\tSynopsys: {film.Synopsis}\n\n\tRecommendedage: {film.RecommendedAge}\n\n\tState: {film.State}\n\n\n\tPress Enter to exit.");
                    Console.ReadLine();
                }
                catch
                {
                    Program.ShowError("\n\tThe number is not valid.");
                }
            }
        }

        public void RentAFilm(DateTime userBirthdate, string userEmail)
        {

            int edad = DateTime.Today.AddTicks(-userBirthdate.Ticks).Year - 1;


            bool stayRent;
            do
            {
                connection.Open();
                string query = $"SELECT * FROM Films WHERE RecommendedAge <= '{edad}' and State = 'available' order by Id asc";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader register = command.ExecuteReader();

                while (register.Read())
                {
                    films.Add(new Film()
                    {
                        Id = Convert.ToInt32(register["Id"]),
                        Title = Convert.ToString(register["Title"]),
                        Synopsis = Convert.ToString(register["Synopsis"]),
                        RecommendedAge = Convert.ToInt32(register["RecommendedAge"]),
                        State = Convert.ToString(register["State"])
                    });
                }
                connection.Close();

                Console.Clear();
                Console.WriteLine("\n\tCatalogue\n");

                foreach (var f in films)
                {
                    Console.WriteLine($"\t{f.Id}. {f.Title}");
                }

                stayRent = true;
                Console.Write("\n\tSelect the movie to rent it. Empty to exit.\n\t");

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
                        Film film = new Film();
                        try
                        {
                            film = films.Where(x => x.Id == numberRentAnswer).FirstOrDefault();
                            if (Rent.NewRent(film.Title, userEmail))
                            {
                                UpdateFilm(film.Title, "rented");
                                Program.ShowCorrectMessage($"\n\tYou succcessfully rent {film.Title}");
                            }
                        }
                        catch
                        {
                            Program.ShowError("\n\tThe number is not valid.");
                        }

                    }
                }

                films.Clear();

            } while (stayRent);
            Console.Clear();
        }

        public static void UpdateFilm(string title, string state)
        {
            connection.Open();

            string query = $"UPDATE Films Set State = '{state}' WHERE Title = '{title}'";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
