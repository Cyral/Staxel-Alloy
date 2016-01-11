using System;
using System.Collections.Generic;
using System.Linq;
using Alloy.Injector.Injectors;
using Mono.Cecil;

namespace Alloy.Injector
{
    public class AssemblyInjector
    {
        private readonly AssemblyDefinition assembly;
        private readonly List<Injectors.Injector> injectors;
        private readonly string sourcePath;
        private readonly string targetPath;

        public AssemblyInjector(string source, string target)
        {
            sourcePath = source;
            targetPath = target;
            assembly = AssemblyDefinition.ReadAssembly(source);
            injectors = new List<Injectors.Injector>
            {
                new CreateModLoaderInjector(assembly),
                new ConnectionInjector(assembly),
                new ServerNetworkInjector(assembly),
                new UniverseInjector(assembly),
            };
        }

        public void Export()
        {
            assembly.Write(targetPath);
        }

        public void Inject()
        {
            MakePublic();

            foreach (var injector in injectors)
            {
                Console.WriteLine("Running " + injector.GetType().Name + ".");
                injector.Inject();
            }
        }

        private void MakePublic()
        {
            // Make all methods public so we have easier access to them in the API.
            foreach (var method in
                    assembly.MainModule.Types.Where(type => type.HasMethods)
                        .SelectMany(type => type.Methods.Where(m => !m.IsConstructor)))
            {
                method.IsPublic = true;
                method.IsPrivate = false;
            }

        }
    }
}