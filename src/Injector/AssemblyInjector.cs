using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Alloy.Injector
{
    public class AssemblyInjector
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public AssemblyDefinition Assembly { get; set; }

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
            //foreach (var m in Assembly.MainModule.Types.SelectMany(typeDef => typeDef.Methods).Where(x => x.FullName.Contains("GameContext")))
            //   Console.WriteLine(CleanMethodName(m));

            var writeLine = GetMethod(typeof (Console), "WriteLine", typeof (string));
            var initializeMethod = GetAssemblyMethod("GameContext.Initialize");

            initializeMethod.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, "Testing Code Injection"));
            initializeMethod.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, writeLine));
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
    }
}