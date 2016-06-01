using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        AstNode ParseTerm () {

            // Try parsing a name
            if (unit.Match (LoreToken.Identifier)) {
                return ParseName ();
            }

            // Try parsing an integer
            if (unit.Match (LoreToken.IntLiteral)) {
                return ParseInteger ();
            }

            // Try parsing a float
            if (unit.Match (LoreToken.FloatLiteral)) {
                return ParseFloat ();
            }

            // Try parsing a list
            if (unit.Match (LoreToken.OpenBracket)) {
                return ParseList ();
            }

            // Try parsing a tuple, a lambda or an expression
            if (unit.Match (LoreToken.OpenParen)) {
                unit.Skip ();
                var istuple = false;

                // Parse the first expression
                var expr = ParseExpression ();

                // Create a temporary tuple expression
                // that holds the arguments of the lambda expression
                TupleExpression tmp = TupleExpression.Create (unit.Location);
                tmp.Add (expr);

                // Try parsing a tuple
                TupleExpression tpl = null;
                if (unit.Accept (LoreToken.Comma)) {
                    tpl = ParseTuple (expr);
                    istuple = true;
                }

                // Read the remaning parenthesis if
                // we did not successfully parse a tuple
                else {
                    unit.Expect (LoreToken.CloseParen);
                }

                // Get the correct tuple
                tpl = tpl ?? tmp;

                // Try parsing a lambda expression
                if (unit.Match (LoreToken.Operator, "=>")) {
                    return ParseLambda (tpl);
                }

                // Return the tuple if needed
                if (istuple) {
                    return tpl;
                }

                // This is neither a tuple nor a lambda
                // Just return the expression
                return expr;
            }

            // Try parsing a keyword
            if (unit.Match (LoreToken.Keyword)) {
                
                // TODO: Implement true, false, null, etc
                switch (unit.Current.Value) {
                case "true":
                    unit.Skip ();
                    // TODO: Return TrueExpression
                    return null;
                case "false":
                    unit.Skip ();
                    // TODO: Return FalseExpression
                    return null;
                case "null":
                    unit.Skip ();
                    // TODO: Return NullExpression
                    return null;
                }
            }

            throw LoreException.Create (unit.Location).Describe ("Unexpected end of term.");
        }
    }
}

