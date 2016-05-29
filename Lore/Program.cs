using System;
using Codeaddicts.libArgument;

namespace LoreCompiler {

    /// <summary>
    /// Program.
    /// </summary>
    public static class Program {

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main (string [] args) {

            // Parse command-line arguments
            var options = ArgumentParser.Parse<Options> (args);
        }
    }
}

