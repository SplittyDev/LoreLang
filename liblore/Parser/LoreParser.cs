using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public class LoreParser {

        /// <summary>
        /// The parsing unit.
        /// </summary>
        readonly ParsingUnit unit;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreParser"/> class.
        /// </summary>
        /// <param name="unit">Unit.</param>
        LoreParser (ParsingUnit unit) {
            this.unit = unit;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="LoreParser"/> class.
        /// </summary>
        /// <param name="unit">The parsing unit.</param>
        public static LoreParser Create (ParsingUnit unit) => new LoreParser (unit);

        /// <summary>
        /// Parse the parsing unit.
        /// </summary>
        public CodeBlock Parse () {
            var root = new CodeBlock (unit.Location);
            while (unit.See ()) {
                root.AddChild (ParseStatement ());
            }
            return root;
        }

        AstNode ParseStatement () {
            var current = unit.Current;

            // Try parsing a keyword
            if (current.Is (LoreToken.Keyword)) {
                switch (current.Value) {
                case "fn":
                    return ParseFunction ();
                }
            }

            // Try parsing a capturing block
            if (current.Is (LoreToken.OpenBracket)) {
                return ParseBlockWithCaptures ();
            }

            // Try parsing a pure block
            if (current.Is (LoreToken.OpenBrace)) {
                return ParsePureBlock ();
            }

            throw new ParserException (unit, $"Unexpected token: '{current.Value}' ({current.Token})");
        }

        /*
         *  fn name (parameters...) [captures...] {
         *      code...
         *  }
         */
        Function ParseFunction () {
            var function = new Function (unit.Location);
            unit.Expect (LoreToken.Keyword, "fn");

            // Read the name of the function
            var name = unit.Expect (LoreToken.Identifier);
            function.SetName (name.Value);

            // Read the parameter list
            if (unit.Match (LoreToken.OpenParen)) {
                function.SetParameters (ParseParameterList ());
            }

            // Read the function body
            var body = ParseBlock ();
            function.SetBody (body);

            // Return the function
            Console.WriteLine (function);
            return function;
        }

        /*
         * (parameters...)
         */
        List<FunctionParameter> ParseParameterList () {
            var parameters = new List<FunctionParameter> ();
            unit.Expect (LoreToken.OpenParen);
            while (!unit.Match (LoreToken.CloseParen)) {
                var lex = unit.Expect (LoreToken.Identifier);
                var parameter = FunctionParameter.Create (lex.Value);
                parameters.Add (parameter);
            }
            unit.Expect (LoreToken.CloseParen);
            return parameters;
        }

        /*
         *  { code... }
         * or
         *  [captures...] { code... }
         */
        CodeBlock ParseBlock () {
            if (unit.Match (LoreToken.OpenBracket)) {
                return ParseBlockWithCaptures ();
            }
            return ParsePureBlock ();
        }

        /*  
         *  { code... }
         */
        CodeBlock ParsePureBlock () {
            var code = new CodeBlock (unit.Location);
            unit.Expect (LoreToken.OpenBrace);
            while (!unit.Match (LoreToken.CloseBrace)) {
                code.AddChild (ParseStatement ());
            }
            unit.Expect (LoreToken.CloseBrace);
            return code;
        }

        /*
         *  [captures...] { code... }
         */
        CodeBlock ParseBlockWithCaptures () {
            var capturedBlock = new CodeBlock (unit.Location);

            // Parse the captures
            unit.Expect (LoreToken.OpenBracket);
            while (!unit.Match (LoreToken.CloseBracket)) {
                var lex = unit.Read ();
                var capture = Capture.Create (unit, lex);
                capturedBlock.AddCapture (capture);
            }
            unit.Expect (LoreToken.CloseBracket);

            // Parse the actual block
            var pureBlock = ParsePureBlock ();

            // Merge the block with the captures
            capturedBlock.Merge (pureBlock);
            return capturedBlock;
        }
   }
}

