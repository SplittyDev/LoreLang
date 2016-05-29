using System;

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
                // TODO: Implement this
            }

            // Try parsing a capturing block
            // Example: [captures] { code }
            if (current.Is (LoreToken.OpenBracket)) {
                // return ParseBlockWithCaptures ();
            }

            // Try parsing a pure block
            // Example: { code }
            if (current.Is (LoreToken.OpenBrace)) {
                return ParseBlockWithoutCaptures ();
            }

            // TODO: Finish this
            throw new ParserException (unit, $"Unexpected token: '{current.Value}' ({current.Token})");
        }

        AstNode ParseBlockWithoutCaptures () {
            var code = new CodeBlock (unit.Location);
            // TODO: Implement this
            return null;
        }
   }
}

