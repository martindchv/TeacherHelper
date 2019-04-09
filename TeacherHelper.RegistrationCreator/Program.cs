using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeacherHelper.Services.Services;

namespace TeacherHelper.RegistrationCreator
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Welcome to the Registration for TeacherHelper!");

            if (!DatabaseConnection.Try())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to connect to the database!");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.WriteLine("Please type the following information:");

            Console.Write("Real name: ");
            var name = Console.ReadLine();

            Console.Write("Username: ");
            var username = Console.ReadLine();

            Console.Write("Password: ");
            var password = Console.ReadLine();

            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.Write("Should the user be admin (true/false): ");
            bool.TryParse(Console.ReadLine(), out bool is_admin);

            var service = new AuthenticationService();

            service.Register(username, password, name, email, is_admin);
        }
    }
}
