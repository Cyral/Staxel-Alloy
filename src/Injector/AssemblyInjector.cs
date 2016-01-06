using System;
using System.Linq;
using System.Reflection;
using Alloy.Loader;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = Mono.Cecil.FieldAttributes;

namespace Alloy.Injector
{
    public class AssemblyInjector
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public AssemblyDefinition Assembly { get; set; }
        private int instrIndex;

        public AssemblyInjector(string source, string target)
        {
            SourcePath = source;
            TargetPath = target;
            Assembly = AssemblyDefinition.ReadAssembly(source);
        }

        public void Export()
        {
            Assembly.Write(TargetPath);
        }

        public void Inject()
        {
            InjectTest();
        }

        public void InjectTest()
        {
            //foreach (var m in Assembly.MainModule.Types)
            //  Console.WriteLine(m.FullName);
            //var writeLine = GetMethod(typeof(Console), "WriteLine", typeof(string));
            //var refMethod = GetMethod(typeof(ModLoader), "Test");

            var initializeMethod = GetAssemblyMethod("GameContext.Initialize");
            // Just a method that is called later (to test accessing the mod loader)
            var laterMethod = GetAssemblyMethod("GameContext.ResourceInitializations");
            var gameClass = GetAssemblyClass("GameContext");

            // Import and create instance of the mod loader.
            var modLoader = Assembly.MainModule.Import(typeof (ModLoader));
            var modLoaderCtor = Assembly.MainModule.Import(typeof(ModLoader).GetConstructors()[0]);
            var modLoaderTest = Assembly.MainModule.Import(typeof(ModLoader).GetMethod("Test"));

            // Create a public static field in GameContext to hold the mod loader/host.
            var fieldDef = new FieldDefinition("ModLoader", FieldAttributes.Public | FieldAttributes.Static, modLoader);
            gameClass.Fields.Add(fieldDef);

            // Create the mod loader instance and store it as a field.
            AddInstruction(initializeMethod, Instruction.Create(OpCodes.Newobj, modLoaderCtor));
            AddInstruction(initializeMethod, Instruction.Create(OpCodes.Stsfld, fieldDef));

            instrIndex = 0;

            // Just a test to see how to call the mod loader from another function (which will be used for event hooks)
            AddInstruction(laterMethod, Instruction.Create(OpCodes.Call, modLoaderTest));

            //AddInstruction(initializeMethod, Instruction.Create(OpCodes.Ldstr, "Testing Code Injection"));
            //AddInstruction(initializeMethod, Instruction.Create(OpCodes.Call, writeLine));
            //AddInstruction(initializeMethod, Instruction.Create(OpCodes.Call, refMethod));
        }

        private void AddInstruction(MethodDefinition method, Instruction instruction)
        {
            method.Body.Instructions.Insert(instrIndex, instruction);
            instrIndex++;
        }

        internal MethodInfo DefineMethod(Type rootType, string methodName, params Type[] argTypes)
        {
            return rootType.GetMethod(methodName, argTypes);
        }

        internal MethodDefinition GetAssemblyMethod(string name)
        {
            return
                Assembly.MainModule.Types.SelectMany(typeDef => typeDef.Methods)
                    .FirstOrDefault(x => CleanMethodName(x).Equals(name));
        }

        internal TypeDefinition GetAssemblyClass(string name)
        {
            return
                Assembly.MainModule.Types.FirstOrDefault(x => CleanClassName(x).Equals(name));
        }

        internal MethodReference GetMethod(Type rootType, string methodName, params Type[] argTypes)
        {
            return ImportMethod(DefineMethod(rootType, methodName, argTypes));
        }

        internal MethodReference ImportMethod(MethodBase method)
        {
            return Assembly.MainModule.Import(method);
        }

        /// <summary>
        /// Returns the method name without the first namespace (Staxel.) and the arguments. :: is replaced with . as well.
        /// </summary>
        private string CleanMethodName(MethodDefinition methodDefinition)
        {
            var start = methodDefinition.FullName.IndexOf('.', methodDefinition.FullName.IndexOf(' '));
            var end = methodDefinition.FullName.IndexOf('(');
            return methodDefinition.FullName.Substring(start + 1, end - start - 1).Replace("::", ".");
        }

        private string CleanClassName(TypeDefinition typeDefinition)
        {
            var start = typeDefinition.FullName.IndexOf('.');
            if (start < 0) // For stuff like <Module>
                return typeDefinition.FullName;
            return typeDefinition.FullName.Substring(start + 1);
        }
    }
}