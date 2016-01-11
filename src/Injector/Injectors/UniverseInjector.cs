using Alloy.Loader;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Alloy.Injector.Injectors
{
    internal class UniverseInjector : Injector
    {
        public UniverseInjector(AssemblyDefinition assembly) : base(assembly)
        {
        }

        public override void Inject()
        {
            // Place tile
            var tileMethod = GetAssemblyMethod("Logic.Universe.PlaceTile");
            var modLoaderTile = assembly.MainModule.Import(typeof (Core).GetMethod("OnPlaceTile"));
            AddInstruction(tileMethod, Instruction.Create(OpCodes.Ldarg_1));
            AddInstruction(tileMethod, Instruction.Create(OpCodes.Ldarg_2));
            AddInstruction(tileMethod, Instruction.Create(OpCodes.Ldarg_3));
            AddInstruction(tileMethod, Instruction.Create(OpCodes.Call, modLoaderTile));
            var returnInstruction = tileMethod.Body.Instructions[tileMethod.Body.Instructions.Count - 1];
            AddInstruction(tileMethod, Instruction.Create(OpCodes.Brfalse, returnInstruction));
        }
    }
}