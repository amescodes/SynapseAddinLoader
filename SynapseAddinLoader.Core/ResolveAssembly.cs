using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SynapseAddinLoader.Core
{
    public class ResolveAssembly
    {
        private string assemblyDirectory;

        public ResolveAssembly(string assemblyDirectory)
        {
            this.assemblyDirectory = assemblyDirectory;
        }

        public void AddResolveAssemblyHandler()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        public void RemoveResolveAssemblyHandler()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
        }

        public Assembly LoadFromPath(string filePath)
        {
            Assembly result = null;
            try
            {
                Monitor.Enter(this);
                result = Assembly.LoadFile(filePath);
            }
            finally
            {
                Monitor.Exit(this);
            }

            return result;
        }

        private Assembly Resolve(object sender, ResolveEventArgs args)
        {
            string assembly = args.Name;
            string assemblyName = assembly.Substring(0, assembly.IndexOf(','));
            
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (loadedAssemblies.FirstOrDefault(a => a.GetName().Name.Equals(assemblyName)) is Assembly
                alreadyLoadedAssembly)
            {
                return alreadyLoadedAssembly;
            }

            string filePath = Path.Combine(assemblyDirectory, $"{assemblyName}.dll");
            if (File.Exists(filePath))
            {
                return LoadFromPath(filePath);
            }

            return null;
        }
    }
}
