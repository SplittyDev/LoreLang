using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lore {

    /// <summary>
    /// Module resolver.
    /// </summary>
    public static class ModuleResolver {

        /// <summary>
        /// The search paths.
        /// </summary>
        static readonly List<string> SearchPaths;

        /// <summary>
        /// Initializes the <see cref="ModuleResolver"/> class.
        /// </summary>
        static ModuleResolver () {
            SearchPaths = new List<string> ();
            CollectSearchPaths ();
        }

        // TODO: Implement this
        public static bool ResolveModule (string id, out LoreModule module) {
            module = null;
            return false;
        }

        static void CollectSearchPaths () {
            SearchPaths.Add (Assembly.GetEntryAssembly ().GetName ().CodeBase);
            Console.WriteLine ("Collected search paths:");
            foreach (var path in SearchPaths) Console.WriteLine ($"* {path}");
        }
    }
}

