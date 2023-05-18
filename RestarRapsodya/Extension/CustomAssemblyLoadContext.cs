using System.Reflection;
using System.Runtime.Loader;

namespace RestarRapsodya.Extension
{
    public class CustomAssemblyLoadContext:AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absoultePath)
        {
            return LoadUnmanagedDll(absoultePath);
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}
