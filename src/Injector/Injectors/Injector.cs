using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Alloy.Injector.Injectors
{
    /// <summary>
    /// Base injector class.
    /// </summary>
    public abstract class Injector
    {
        protected AssemblyDefinition assembly;
        private int instrIndex;

        public Injector(AssemblyDefinition assembly)
        {
            this.assembly = assembly;
        }

        public abstract void Inject();

        /// <summary>
        /// Adds an instruction to the end of the method.
        /// </summary>
        protected void AddInstruction(MethodDefinition method, Instruction instruction)
        {
            method.Body.Instructions.Insert(instrIndex, instruction);
            instrIndex++;
        }

        /// <summary>
        /// </summary>
        internal MethodInfo FindMethod(Type rootType, string methodName, params Type[] argTypes)
        {
            return rootType.GetMethod(methodName, argTypes);
        }

        /// <summary>
        /// Get a class from Staxel.
        /// </summary>
        internal TypeDefinition GetAssemblyClass(string name)
        {
            return
                assembly.MainModule.Types.FirstOrDefault(x => CleanClassName(x).Equals(name));
        }

        /// <summary>
        /// Get a field from Staxel.
        /// </summary>
        internal FieldDefinition GetAssemblyField(string name)
        {
            return
                assembly.MainModule.Types.SelectMany(typeDef => typeDef.Fields)
                    .FirstOrDefault(x => CleanFieldName(x).Equals(name));
        }

        /// <summary>
        /// Get a method from the Staxel assembly.
        /// </summary>
        internal MethodDefinition GetAssemblyMethod(string name)
        {
            return
                assembly.MainModule.Types.SelectMany(typeDef => typeDef.Methods)
                    .FirstOrDefault(x => CleanMethodName(x).Equals(name));
        }

        /// <summary>
        /// Gets a reference to a method based on a type, name, and arguments.
        /// </summary>
        internal MethodReference GetMethod(Type rootType, string methodName, params Type[] argTypes)
        {
            return Import(FindMethod(rootType, methodName, argTypes));
        }

        /// <summary>
        /// Import a type into Staxel.
        /// </summary>
        internal TypeReference Import(Type type)
        {
            return assembly.MainModule.Import(type);
        }

        /// <summary>
        /// Import a method into Staxel.
        /// </summary>
        internal MethodReference Import(MethodBase method)
        {
            return assembly.MainModule.Import(method);
        }

        private static string CleanClassName(TypeDefinition typeDefinition)
        {
            var start = typeDefinition.FullName.IndexOf('.');
            if (start < 0) // For stuff like <Module>
                return typeDefinition.FullName;
            return typeDefinition.FullName.Substring(start + 1);
        }

        private static string CleanFieldName(FieldDefinition fieldDefinition)
        {
            var start = fieldDefinition.FullName.IndexOf('.', fieldDefinition.FullName.IndexOf(' '));
            return fieldDefinition.FullName.Substring(start + 1).Replace("::", ".");
        }

        /// <summary>
        /// Returns the method name without the first namespace (Staxel.) and the arguments. :: is replaced with . as well.
        /// </summary>
        private static string CleanMethodName(MethodDefinition methodDefinition)
        {
            var start = methodDefinition.FullName.IndexOf('.', methodDefinition.FullName.IndexOf(' '));
            var end = methodDefinition.FullName.IndexOf('(');
            return methodDefinition.FullName.Substring(start + 1, end - start - 1).Replace("::", ".");
        }
    }
}