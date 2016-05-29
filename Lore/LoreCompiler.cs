using System;
using System.IO;
using LexDotNet;
using Lore;

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

        public void Run () {
            Compile (SourceUnit.FromFile (options.Input));
        }

        void Compile (SourceUnit sunit) {
            var lexer = LoreLexer.Create (sunit);
            var lexemes = lexer.Tokenize ();
            var punit = ParsingUnit.Create (lexemes);
            var parser = LoreParser.Create (punit);
            AstRoot ast;
            try {
                ast = parser.Parse ();
            } catch (ParserException e) {
                Console.WriteLine (e.Message);
            }
        }
    }
}

