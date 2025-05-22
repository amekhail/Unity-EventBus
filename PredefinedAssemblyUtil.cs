using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Amekhail.EventBus
{
    /// <summary>
    /// Utility class for scanning predefined Unity assemblies and extracting types that implement a given interface.
    /// </summary>
    public static class PredefinedAssemblyUtil
    {
        /// <summary>
        /// Enum representing known Unity-generated assembly names.
        /// </summary>
        enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpFirstPass,
            AssemblyCSharpEditorFirstPass,
        }

        /// <summary>
        /// Maps a string-based assembly name to its corresponding <see cref="AssemblyType"/> enum, if recognized.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly (e.g., "Assembly-CSharp").</param>
        /// <returns>The corresponding <see cref="AssemblyType"/> if known; otherwise, <c>null</c>.</returns>
        static AssemblyType? GetAssemblyType(string assemblyName)
        {
            return assemblyName switch
            {
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                _ => null
            };
        }

        /// <summary>
        /// Retrieves all types from predefined Unity assemblies that implement a specific interface.
        /// </summary>
        /// <param name="interfaceType">The interface type to search for (e.g., <c>typeof(IEvent)</c>).</param>
        /// <returns>A list of types that implement the specified interface.</returns>
        public static List<Type> GetTypes(Type interfaceType)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Dictionary<AssemblyType, Type[]> assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            List<Type> types = new List<Type>();

            for (int i = 0; i < assemblies.Length; i++)
            {
                AssemblyType? assemblyType = GetAssemblyType(assemblies[i].GetName().Name);
                if (assemblyType != null)
                {
                    assemblyTypes.Add((AssemblyType)assemblyType, assemblies[i].GetTypes());
                }
            }

            // Extract types from targeted assemblies only
            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
            AddTypesFromAssembly(assemblyCSharpTypes, interfaceType, types);

            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
            AddTypesFromAssembly(assemblyCSharpFirstPassTypes, interfaceType, types);

            return types;
        }

        /// <summary>
        /// Adds all types from an assembly that implement a specific interface to the result list.
        /// </summary>
        /// <param name="assemblyTypes">Array of types from an assembly.</param>
        /// <param name="interfaceType">The interface to match.</param>
        /// <param name="results">The list where matching types will be added.</param>
        static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results)
        {
            if (assemblyTypes == null) return;

            for (int i = 0; i < assemblyTypes.Length; i++)
            {
                Type type = assemblyTypes[i];
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    results.Add(type);
                }
            }
        }
    }
}