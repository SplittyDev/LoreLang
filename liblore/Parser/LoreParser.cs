using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

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
        public AstRoot Parse () {
            var root = AstRoot.Create (unit.Location);
            while (unit.See ()) {
                root.AddChild (ParseStatement ());
            }
            Visualize (root);
            return root;
        }

        AstNode ParseStatement () {
            var current = unit.Current;

            // Try parsing a keyword
            if (current.Is (LoreToken.Keyword)) {
                switch (current.Value) {
                case "fn":
                    return ParseFunction ();
                case "let":
                    unit.Skip ();
                    return ParseAssignment ();
                }
            }

            // Try parsing a capturing block
            if (unit.Match (LoreToken.OpenBracket)) {
                return ParseBlockWithCaptures ();
            }

            // Try parsing a pure block
            if (unit.Match (LoreToken.OpenBrace)) {
                return ParseBlockWithoutCaptures ();
            }

            // Try parsing an assign statement
            //if (unit.See () && unit.Peek (1).Token == LoreToken.Comma) {
            //    return ParseAssignStatement ();
            //}

            return ParseExpression ();

            throw new ParserException (unit, $"Unexpected token: '{current.Value}' ({current.Token})");
        }

        ArgumentList ParseArgumentList () {
            var list = ArgumentList.Create (unit.Location);
            unit.Expect (LoreToken.OpenParen);
            while (!unit.Match (LoreToken.CloseParen)) {
                var expr = ParseExpression ();
                list.Add (expr);
                if (!unit.Accept (LoreToken.Comma)) {
                    break;
                }
            }
            unit.Expect (LoreToken.CloseParen);
            return list;
        }
   }
}

