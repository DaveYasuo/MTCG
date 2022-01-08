using System;
using System.Globalization;
using System.Text.Json;
using System.Threading;

namespace MTCG.Data.Cards
{
    public class Package
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }

        public static void PrintPackageData(string data)
        {
            Package[] packages = JsonSerializer.Deserialize<Package[]>(data);
            try
            {
                if (packages != null)
                    foreach (Package package in packages)
                    {
                        //DB.InsertPackage(package);
                        Console.WriteLine($"ID:{package?.Id}");
                        Console.WriteLine($"Name:{package?.Name}");
                        Console.WriteLine($"Damage:{package?.Damage}");
                    }
            }
            catch (Exception e)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
