using System;
using System.IO;
using LexDotNet;
using Lore;

namespace LoreCompiler {

    /// <summary>
    /// Lore compiler.
    /// </summary>
    public class LoreCompilerWrapper {

        /// <summary>
        /// The command-line options.
        /// </summary>
        readonly Options options;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreCompilerWrapper"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        public LoreCompilerWrapper (Options options) {
            this.options = options;
        }

        public void Run () {
            var compiler = Lore.LoreLLVMCompiler.CreateFromFile (options.Input);
            compiler.Compile ();
        }
    }
}

