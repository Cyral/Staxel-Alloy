using Alloy.Loader;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Alloy.Injector.Injectors
{
    /// <summary>
    /// Adds the mod loader to Staxel.
    /// The mod loader will be called when the game initializes.
    /// </summary>
    internal class CreateModLoaderInjector : Injector
    {
        public CreateModLoaderInjector(AssemblyDefinition assembly) : base(assembly)
        {
        }

        public override void Inject()
        {
            var initializeMethod = GetAssemblyMethod("GameContext.Initialize");
            var gameClass = GetAssemblyClass("GameContext");

            var modLoader = Import(typeof (ModLoader));
            var modLoaderCtor = Import(typeof (ModLoader).GetConstructors()[0]);

            // Create a public static field in GameContext to hold the mod loader/host.
            var fieldDef = new FieldDefinition("ModLoader", FieldAttributes.Public | FieldAttributes.Static, modLoader);
            gameClass.Fields.Add(fieldDef);

            // Create the mod loader instance and store it as a field.
            AddInstruction(initializeMethod, Instruction.Create(OpCodes.Newobj, modLoaderCtor));
            AddInstruction(initializeMethod, Instruction.Create(OpCodes.Stsfld, fieldDef));
        }
    }
}