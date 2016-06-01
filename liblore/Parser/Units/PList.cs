using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        ListExpression ParseList () {
            unit.Expect (LoreToken.OpenBracket);
            var list = ListExpression.Create (unit.Location);
            while (!unit.Match (LoreToken.CloseBracket)) {
                var expr = ParseAssignment ();
                // TODO: Implement for, in, if, etc
                // ...
                list.Add (expr);
                if (!unit.Accept (LoreToken.Comma)) {
                    break;
                }
            }
            unit.Expect (LoreToken.CloseBracket);
            return list;
        }
    }
}

