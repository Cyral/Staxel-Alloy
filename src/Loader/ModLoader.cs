using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Alloy.API;
using Plukit.Base;
using Staxel;
using Staxel.Data;
using Staxel.Logic;
using Staxel.Server;
using Staxel.Tiles;

namespace Alloy.Loader
{
    public class ModLoader
    {
        public List<Mod> Mods { get; }

        public ModHost Host { get; }

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

        public static void PlaceTile(Entity entity, Vector3I location, Tile tile)
        {
            Console.WriteLine($"Tile placed! {location.X},{location.Y},{location.Z} ({tile.Configuration.Code})");
        }

        public static void SendChat(ClientServerConnection connection, string message)
        {
            var blob = BlobAllocator.Blob(true);
            blob.SetString("response", message);
            connection.SendPacket(new DataPacket(ServerClockNow(), DataPacketKind.ConsoleResponse, blob));
            blob.Deallocate();
        }

        private static Timestep ServerClockNow()
        {
            return ServerMainLoop._clock.Now();
        }

        public static bool ServerNetwork(ClientServerConnection connection, DataPacket packet)
        {
            //Console.WriteLine($"Server Packet: {packet?.Kind} from {connection?.Credentials?.Username}");
            if (packet != null)
            {
                var blob = packet.Blob;
                var user = connection.Credentials;
                switch (packet.Kind)
                {
                    case DataPacketKind.ConsoleMessage:
                    {
                        var message = blob.GetString("message");
                        Console.WriteLine($"{user.Username}: {message}");
                        if (message.Equals("ping", StringComparison.OrdinalIgnoreCase))
                        {
                            SendChat(connection, "PONG");
                            return false;
                        }
                        break;
                    }
                }
            }
            return true;
        }

        public static void Test()
        {
            Console.WriteLine("Mod loader test.");
        }

        private void LoadMods()
        {
            // Get all directories in mods folder.
            if (!Directory.Exists("mods"))
                Directory.CreateDirectory("mods");
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
                var pluginType = typeof (Mod);
                var mainType = types.FirstOrDefault(type => pluginType.IsAssignableFrom(type));

                // Create instance.
                var instance = (Mod) Activator.CreateInstance(mainType,
                    BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null,
                    new object[] {Host}, CultureInfo.CurrentCulture);
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