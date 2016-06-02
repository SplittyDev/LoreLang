using System;
using System.IO;

namespace Lore {

    /// <summary>
    /// Lore module.
    /// </summary>
    public class LoreModule {

        /// <summary>
        /// The path.
        /// </summary>
        readonly string path;

        /// <summary>
        /// The name.
        /// </summary>
        string name;

        /// <summary>
        /// The is anonymous.
        /// </summary>
        bool isAnonymous;

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path => path;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => name;

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <value>The directory.</value>
        public string Directory => System.IO.Path.GetDirectoryName (path);

        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename => System.IO.Path.GetFileNameWithoutExtension (path);

        /// <summary>
        /// Gets the has path.
        /// </summary>
        /// <value>The has path.</value>
        public bool HasPath => path != null;

        /// <summary>
        /// Gets the is anonymous.
        /// </summary>
        /// <value>The is anonymous.</value>
        public bool IsAnonymous => isAnonymous;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lore.LoreModule"/> class.
        /// </summary>
        /// <param name="path">Path.</param>
        LoreModule (string path = null) {
            this.path = path;
            name = Filename;
        }

        /// <summary>
        /// Creates a new module.
        /// </summary>
        /// <param name="path">Path.</param>
        public static LoreModule Create (string path)
        => new LoreModule (path);

        /// <summary>
        /// Creates a new module.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="name">Name.</param>
        public static LoreModule Create (string path, string name)
        => new LoreModule (path) { name = name };

        /// <summary>
        /// Creates a new anonymous module.
        /// </summary>
        /// <returns>The anonymous.</returns>
        public static LoreModule CreateAnonymous ()
        => new LoreModule { isAnonymous = true, name = "__anonymous__" };
    }
}

