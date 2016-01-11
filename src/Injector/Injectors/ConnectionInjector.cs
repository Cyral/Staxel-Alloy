using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace Alloy.Injector.Injectors
{
    class ConnectionInjector : Injector
    {
        public ConnectionInjector(AssemblyDefinition assembly) : base(assembly)
        {
        }

        public override void Inject()
        {
            var closedField = GetAssemblyField("Server.ClientServerConnection._closed");
            closedField.IsPublic = true;

            var conField = GetAssemblyField("Server.ClientServerConnection._connection");
            conField.IsPublic = true;
        }
    }
}
