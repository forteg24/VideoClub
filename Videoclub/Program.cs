using System;
using System.Threading;

namespace Videoclub
{
    class Program
    {
        static void Main(string[] args)
        {
            User user = new User();
            Film film = new Film();
            Rent rent = new Rent();

            bool stayRunning;
            do
            {
                stayRunning = true;

                user.Login();

                bool stayMenu;
                do
                {
                    stayMenu = true;
                    switch (Menu())
                    {
                        case "1":
                            film.Catalogue(user.UserBirthdate);
                            break;
                        case "2":
                            film.RentAFilm(user.UserBirthdate, user.UserEmail);
                            break;
                        case "3":
                            rent.MyRents(user.UserEmail);
                            break;
                        case "4":
                            stayMenu = false;
                            ShowCorrectMessage($"\n\tBye {user.UserName}, you log out", 1000);
                            break;
                        default:
                            ShowError("\n\tThe entered value is invalid. You have to insert a value between 1 and 4.", 3000);
                            break;
                    }
                } while (stayMenu);

            } while (stayRunning);
        }

        private static string Menu()
        {
            Console.Write("\n\t1.-Ver películas disponibles");
            Console.Write("\n\t2.-Alquilar película");
            Console.Write("\n\t3.-Mis alquileres");
            Console.Write("\n\t4.-Logout\n\t");

            return Console.ReadLine();

        }

        public static void ShowError(string errorMessage)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(errorMessage);
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(2000);
            Console.Clear();
        }

        public static void ShowError(string errorMessage, int time)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(errorMessage);
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(time);
            Console.Clear();
        }

        public static void ShowCorrectMessage(string message)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(2000);
            Console.Clear();
        }

        public static void ShowCorrectMessage(string message, int time)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(time);
            Console.Clear();
        }

    }
}
