using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Cards;
using Data.Users;


namespace BusinessLogic
{
    public static class RequestHandler
    {
        /**
         * Das ist die Logik.
         * Hier kommt eine switch-case.
         * Kommt drauf an, welche RawUrl, welche HTTP-Methode und welche Daten übergeben wurden.
         * Vllt eine Instanz der DataHandler erstellen und die Queries als Resultat wieder zurückgeben.
         **/
        public static void PrintData(string data, string url)
        {
            switch (url)
            {
                case "/users": User.PrintUserData(data); break;
                case "/sessions": User.PrintUserData(data); break;
                case "/packages": Package.PrintPackageData(data); break;
                case "/transactions/packages": Console.WriteLine("leer"); break;
                case "/cards": Console.WriteLine("leer"); break;
                case "/deck": Console.WriteLine("leer"); break;
                case "/stats": Console.WriteLine("leer"); break;
                case "/score": Console.WriteLine("leer"); break;
                case "/tradings": Console.WriteLine("leer"); break;
            }
        }
    }
}
