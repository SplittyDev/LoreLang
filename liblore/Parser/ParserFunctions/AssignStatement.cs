using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        AstNode ParseAssignStatement () {
            var stmt = AssignStatement.Create (unit.Location);

            // Parse identifiers
            while (!unit.Match (LoreToken.Operator, "=")) {
                var name = unit.Expect (LoreToken.Identifier).Value;
                stmt.AddIdentifier (name);
                if (!unit.Match (LoreToken.Operator, "=")) {
                    unit.Expect (LoreToken.Comma);
                }
            }
            unit.Expect (LoreToken.Operator, "=");

            // Parse expressions
            do {
                var expr = ParseExpression ();
                stmt.AddExpression (expr);
            } while (unit.Accept (LoreToken.Comma));
            return stmt;
        }
    }
}

