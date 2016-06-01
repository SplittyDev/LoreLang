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
            try {
                while (unit.See ()) {
                    root.AddChild (ParseStatement ());
                }
                Console.WriteLine ("Abstract Syntax Tree:");
                Visualize (root);
                Console.WriteLine ();
            } catch (LoreException e) {
                Console.WriteLine (e.Message);
            } catch (Exception e) {
                Console.WriteLine ("*** Severe error");
                Console.WriteLine (e.Message);
            }
            return root;
        }

        internal void Synchronize () {
            while (unit.Current != null) {
                var tk = unit.Read ();
                if (tk.Token == LoreToken.CloseBracket
                    || tk.Token == LoreToken.Semicolon)
                    return;
            }
        }

        AstNode ParseStatement () {
            var current = unit.Current;

            // Try parsing a keyword
            if (current.Is (LoreToken.Keyword)) {
                switch (current.Value) {
                case "fn":
                    return ParseFunction ();
                case "val":
                    return ParseValueDeclaration ();
                case "var":
                    return ParseVariableDeclaration ();
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

            /*
            LoreException.Create (unit.Location)
                         .Describe ($"Unexpected token: '{current.Value}' ({current.Token})")
                         .Throw ();
            */
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

