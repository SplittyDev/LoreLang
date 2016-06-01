using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        AssignStatement ParseAssignStatement () {
            var stmt = AssignStatement.Create (unit.Location);

            // Parse identifiers
            while (!unit.Match (LoreToken.Operator, "=")) {

                // Parse name
                var ident = ParseName ();
                var name = NamedParameter.Create (ident);

                // Parse type
                if (unit.Accept (LoreToken.Colon)) {
                    name.SetType (ParseName ());
                }

                // Add the named parameter to the identifiers
                stmt.AddIdentifier (name);

                // Try parsing another identifier
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

            // Verify that all identifiers are satisfied
            var pluralizeExpression = stmt.ExpressionCount > 1 ? "s" : string.Empty;
            var pluralizeIdentifier = stmt.IdentifierCount > 1 ? "s" : string.Empty;
            if (stmt.IdentifierCount > stmt.ExpressionCount) {
                var count = stmt.IdentifierCount - stmt.ExpressionCount;
                var pluralizeDifference = count > 1 ? "s" : string.Empty;
                throw LoreException.Create (stmt.Location)
                                   .Describe ($"Attempt to assign {stmt.ExpressionCount} expression{pluralizeExpression} to {stmt.IdentifierCount} identifier{pluralizeIdentifier}.")
                                   .Resolve ($"Add more expressions or remove {count} identifier{pluralizeDifference}.");
            }
            if (stmt.IdentifierCount < stmt.ExpressionCount) {
                var count = stmt.ExpressionCount - stmt.IdentifierCount;
                var pluralizeDifference = count > 1 ? "s" : string.Empty;
                throw LoreException.Create (stmt.Location)
                                   .Describe ($"Attempt to assign {stmt.ExpressionCount} expression{pluralizeExpression} to {stmt.IdentifierCount} identifier{pluralizeIdentifier}.")
                                   .Resolve ($"Discard {count} expression{pluralizeDifference} by using the _ operator.");
            }

            return stmt;
        }
    }
}

