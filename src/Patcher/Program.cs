using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
            var source = Environment.ExpandEnvironmentVariables(config["SourceAssembly"].ToString());
            var target = Environment.ExpandEnvironmentVariables(config["TargetAssembly"].ToString());
            var targetDir = Path.GetDirectoryName(target);
            var refs = config["References"];

            if (File.Exists(source))
            {
                injector = new AssemblyInjector(source, target);
                Console.WriteLine($"Patching assembly {source}.");
                injector.Inject();
                Console.WriteLine($"Exporting assembly to {target}.");
                injector.Export();
                Console.WriteLine("Copying references.");
                foreach (var reference in refs)
                    File.Copy(reference.ToString(), Path.Combine(targetDir, reference.ToString()), true);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Done.");
                Thread.Sleep(1000);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Assembly {source} not found.");

                Console.ReadLine();
            }
        }
    }
}