using System;
using Alloy.API;

namespace TestMod
{
    public class TestMod : Mod
    {
        public TestMod(ModHost host) : base(host)
        {
        }

        public override void Load()
        {
            Events.Chat.AddHandler(args =>
            {
                Console.WriteLine($"{args.Sender.Name}: {args.Message}");

                if (args.Message.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
                {
                    args.Sender.SendMessage("Pong!");
                    args.Cancel();
                }
            });

            Events.PlayerJoined.AddHandler(args =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{args.Player.Name} has joined.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }, EventPriority.Final);

            Events.PlayerQuit.AddHandler(args =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{args.Player.Name} has quit.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }, EventPriority.Final);
        }

        public override void Unload()
        {
        }
    }
}