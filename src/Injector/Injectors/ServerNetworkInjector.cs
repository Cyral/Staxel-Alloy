using Alloy.API;
using Alloy.Loader;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Alloy.Injector.Injectors
{
    /// <summary>
    /// Adds packet interception.
    /// </summary>
    internal class ServerNetworkInjector : Injector
    {
        public ServerNetworkInjector(AssemblyDefinition assembly) : base(assembly)
        {
        }

        public override void Inject()
        {
            var networkMethod = GetAssemblyMethod("Server.ServerMainLoop.ProcessPacket");

            // Make the clock field accessable.
            var clockField = GetAssemblyField("Server.ServerMainLoop._clock");
            clockField.IsStatic = true;
            clockField.IsPublic = true;

            // Call the network method whenever a packet arrives.
            var modLoaderNetwork = assembly.MainModule.Import(typeof (Core).GetMethod("OnServerPacket"));
            AddInstruction(networkMethod, Instruction.Create(OpCodes.Ldarg_1));
            AddInstruction(networkMethod, Instruction.Create(OpCodes.Ldarg_2));
            AddInstruction(networkMethod, Instruction.Create(OpCodes.Call, modLoaderNetwork));

            // If the network method returns false, return from the method.
            var returnInstruction = networkMethod.Body.Instructions[networkMethod.Body.Instructions.Count - 1];
            AddInstruction(networkMethod, Instruction.Create(OpCodes.Brfalse, returnInstruction));
        }
    }
}