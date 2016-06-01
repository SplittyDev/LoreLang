using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        /*
         *  fn name (parameters...) -> type [captures...] {
         *      code...
         *  }
         */
        FunctionDeclaration ParseFunction () {
            var function = FunctionDeclaration.Create (unit.Location);
            unit.Expect (LoreToken.Keyword, "fn");

            // Read the name of the function
            var name = unit.Expect (LoreToken.Identifier);
            function.SetName (name.Value);

            // Read the parameter list
            if (unit.Match (LoreToken.OpenParen)) {
                function.SetParameters (ParseDeclarationArgumentList ());
            }

            // Read the return type
            if (unit.Accept (LoreToken.Operator, "->")) {
                var names = new List<NameExpression> ();
                while (unit.Match (LoreToken.Identifier)) {
                    names.Add (ParseName ());
                    if (!unit.Accept (LoreToken.Comma)) {
                        break;
                    }
                }
                if (names.Count == 1) {
                    function.SetReturnType (names [0]);
                } else {
                    function.SetReturnTuple (names);
                }
            }

            // Create the function body
            CodeBlock body = CodeBlock.Create (unit.Location);

            // Parse captures
            if (unit.Accept (LoreToken.OpenBracket)) {
                while (!unit.Match (LoreToken.CloseBracket)) {
                    var lex = unit.Read ();
                    var capture = Capture.Create (unit, lex);
                    body.AddCapture (capture);
                }
                unit.Expect (LoreToken.CloseBracket);
            }

            // Parse body
            if (unit.Match (LoreToken.OpenBrace)) {
                body = ParseBlock ();
            } else if (unit.Accept (LoreToken.Operator, "=>")) {
                body.AddChild (ParseExpression ());
            }

            // Return the function
            function.SetBody (body);
            return function;
        }
    }
}

