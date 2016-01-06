using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Alloy.API;

namespace Alloy.Loader
{
    public class ModLoader
    {
        public List<Mod> Mods { get; private set; }

        public ModHost Host { get; private set; }

        public ModLoader()
        {
            Mods = new List<Mod>();
            Host = new ModHost();

            WriteBreak();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Alloy - Staxel Modding API");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("https://github.com/Cyral/Alloy\n");
            Console.ForegroundColor = ConsoleColor.Gray;

            LoadMods();
        }

        public static void Test()
        {
            Console.WriteLine("Mod loader test.");
        }

        private void LoadMods()
        {
            // Get all directories in mods folder.
            var dirs = Directory.GetDirectories("mods");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Loading mods...\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var dir in dirs)
            {
                // TODO: Error handling, validation, metadata .json
                var filename = dir.Split('\\')[1] + ".dll";
                var file = Path.Combine(dir, filename);
                var raw = File.ReadAllBytes(file);
                var asm = Assembly.Load(raw);
                var types = asm.GetTypes();
                var pluginType = typeof(Mod);
                var mainType = types.FirstOrDefault(type => pluginType.IsAssignableFrom(type));

                // Create instance.
                var instance = (Mod)Activator.CreateInstance(mainType,
                    BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null,
                    new object[] { Host }, CultureInfo.CurrentCulture);
                Mods.Add(instance);

                Console.WriteLine($"Loaded mod: {filename}");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nInitializing mods...\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            // Call the load method on each mod so it can perform initialization logic.
            foreach (var mod in Mods)
            {
                mod.Load();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            var numMods = Mods.Count == 1 ? "mod" : "mods";
            Console.WriteLine($"\n{Mods.Count} {numMods} loaded.");
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteBreak();
        }

        private void WriteBreak()
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', Console.BufferWidth));
        }
    }
}