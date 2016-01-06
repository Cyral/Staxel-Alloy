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
            Console.WriteLine("Test Mod: Hello, World!");
        }

        public override void Unload()
        {
        }
    }
}