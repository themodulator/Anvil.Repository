using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Reflection.Emit;

using System.Text.RegularExpressions;

namespace System.Collections.Generic
{
    public static class DynamicEnum
    {
        public static Type ToEnum<T>(
            this IEnumerable<T> items,
            Func<T, string> nameSelector,
            Func<T, long> valueSelector
            )
        {
            // Get the current application domain for the current thread.
            AppDomain currentDomain = AppDomain.CurrentDomain;

            // Create a dynamic assembly in the current application domain,
            // and allow it to be executed and saved to disk.
            AssemblyName aName = new AssemblyName(typeof(T).Name + "Enum");
            AssemblyBuilder ab = currentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

            // Define a dynamic module in "TempAssembly" assembly. For a single-
            // module assembly, the module has the same name as the assembly.
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

            // Define a public enumeration with the name "Elevation" and an 
            // underlying type of Integer.
            EnumBuilder eb = mb.DefineEnum(aName.Name, TypeAttributes.Public, typeof(long));

            eb.DefineLiteral("None", 0L);

            foreach(T item in items)
            {
                string name = nameSelector(item);
                long value = valueSelector(item);

                eb.DefineLiteral(name, value);
            }

            // Create the type and save the assembly.
            Type finished = eb.CreateType();
            return finished;
        }
    }
}
