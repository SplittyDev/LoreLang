using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        TupleExpression ParseTuple (AstNode expr) {
            var tuple = TupleExpression.Create (unit.Location);
            tuple.Add (expr);
            while (!unit.Match (LoreToken.CloseParen)) {
                tuple.Add (ParseExpression ());
                if (!unit.Accept (LoreToken.Comma)) {
                    break;
                }
            }
            unit.Expect (LoreToken.CloseParen);
            Console.WriteLine (tuple);
            return tuple;
        }
    }
}

