using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using JsonInheritanceApp;

namespace JsonInheritanceApp
{
    class Program
    {
        static string basePath = "users.json";
        static string extendedPath = "user_types.json";

        static void Main(string[] args)
        {
            // 1: XML skaitymas
            Console.WriteLine("=== XML duomenų skaitymas ===");
            if (File.Exists("duomenys.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("duomenys.xml");
                XmlNodeList nodeList = doc.DocumentElement.SelectNodes("/Users/User");
                foreach (XmlNode node in nodeList)
                {
                    string name = node["Name"]?.InnerText;
                    Console.WriteLine("Vartotojas iš XML: " + name);
                }
            }

            // 2: JSON įkėlimas
            var users = LoadUsers(basePath);
            Console.WriteLine("\n=== Pradiniai Vartotojai ===");
            foreach (var user in users)
                PrintUser(user);

            // 2: naujo vartotojo pridėjimas
            users.Add(new User { Name = "Tomas", Age = 28, City = "Vilnius" });
            SaveUsers(basePath, users);

            // 5: user_types.json nuskaitymas
            var typedUsers = LoadTypedUsers(extendedPath);
            Console.WriteLine("\n=== Tipiniai Vartotojai ===");
            foreach (var user in typedUsers)
                PrintUser(user);
        }

        static List<User> LoadUsers(string path)
        {
            if (!File.Exists(path)) return new List<User>();
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<User>>(json);
        }

        static void SaveUsers(string path, List<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path, json);
        }

        static List<User> LoadTypedUsers(string path)
        {
            var result = new List<User>();
            if (!File.Exists(path)) return result;

            var raw = File.ReadAllText(path);
            var tempUsers = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(raw);

            foreach (var entry in tempUsers)
            {
                string role = entry.ContainsKey("Role") ? entry["Role"].ToString() : "";

                if (role == "Administratorius")
                {
                    result.Add(new Admin
                    {
                        Name = entry["Name"]?.ToString() ?? string.Empty,
                        Age = Convert.ToInt32(entry["Age"]),
                        City = entry["City"]?.ToString() ?? string.Empty
                    });
                }
                else
                {
                    result.Add(new StandardUser
                    {
                        Name = entry["Name"]?.ToString() ?? string.Empty,
                        Age = Convert.ToInt32(entry["Age"]),
                        City = entry["City"]?.ToString() ?? string.Empty
                    });
                }
            }

            return result;
        }

        static void PrintUser(User user)
        {
            string role = user is Admin ? ((Admin)user).Role :
                          user is StandardUser ? ((StandardUser)user).Role : "Naudotojas";
            Console.WriteLine($"Vardas: {user.Name}, Amžius: {user.Age}, Miestas: {user.City}, Tipas: {role}");
        }
    }
}
