using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        /*
         *  => [captures...] expression
         * or
         *  => [captures...] { code... }
         * or
         *  (parameters...) => [captures...] expression
         * or
         *  (parameters...) => [captures...] { code... }
         */
        LambdaExpression ParseLambda (TupleExpression argumentList = null) {
            var lambda = LambdaExpression.Create (unit.Location);

            // Parse parameter list
            if (argumentList == null || argumentList.Count == 0) {
                if (unit.Match (LoreToken.OpenParen)) {
                    var parameters = ParseDeclarationArgumentList ();
                    lambda.SetParameters (parameters);
                }
            } else {
                var lst = new List<FunctionParameter> ();
                foreach (var argument in argumentList.Items) {
                    var name = argument as NameExpression;
                    if (name == null) {
                        throw new ParserException (unit, "Invalid parameter list in lambda declaration.");
                    }
                    lst.Add (FunctionParameter.Create (name.Name));
                }
                lambda.SetParameters (lst);
            }

            // Expect arrow operator
            unit.Expect (LoreToken.Operator, "=>");

            // Create master code block
            CodeBlock master = CodeBlock.Create (unit.Location);

            // Parse captures
            if (unit.Match (LoreToken.OpenBracket)) {

                // Parse captures
                unit.Expect (LoreToken.OpenBracket);
                while (!unit.Match (LoreToken.CloseBracket)) {
                    var lex = unit.Read ();
                    var capture = Capture.Create (unit, lex);
                    master.AddCapture (capture);
                }
                unit.Expect (LoreToken.CloseBracket);
            }

            // Parse block
            if (unit.Match (LoreToken.OpenBrace)) {
                var block = ParseBlockWithoutCaptures ();

                // Merge captures
                master.Merge (block);
            }

            // Or parse expression
            else {

                // Create return statement for the result of the expression
                var retstmt = ReturnStatement.Create (unit.Location, ParseExpression ());

                // Create a new block with the return statement
                var block = CodeBlock.Create (unit.Location);
                block.AddChild (retstmt);

                // Merge captures
                master.Merge (block);
            }

            // Set the lambda body
            lambda.SetBody (master);
            Console.WriteLine (lambda);
            return lambda;
        }
    }
}

