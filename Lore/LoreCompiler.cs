using System;

namespace LoreCompiler {

    /// <summary>
    /// Lore compiler.
    /// </summary>
    public class LoreCompiler {

        /// <summary>
        /// The command-line options.
        /// </summary>
        readonly Options options;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreCompiler"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        public LoreCompiler (Options options) {
            this.options = options;
        }
    }
}

