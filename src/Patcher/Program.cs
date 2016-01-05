using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Alloy.Injector;

namespace Alloy.Patcher
{
    internal class Program
    {
        private static AssemblyInjector injector;
        private static void Main(string[] args)
        {
            Console.Title = "Alloy Patcher";

            var config = JObject.Parse(File.ReadAllText("config.json"));
            var path = config["StaxelAssembly"].ToString();
            injector = new AssemblyInjector(path);

            Console.WriteLine($"Patching {path}");

            Console.ReadLine();
        }
    }
}