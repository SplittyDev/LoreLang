using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        /*
         * (parameter [, parameter...])
         */
        List<FunctionParameter> ParseDeclarationArgumentList () {
            var parameters = new List<FunctionParameter> ();
            unit.Expect (LoreToken.OpenParen);
            while (!unit.Match (LoreToken.CloseParen)) {
                var lex = unit.Expect (LoreToken.Identifier);
                var parameter = FunctionParameter.Create (lex.Value);
                parameters.Add (parameter);
            }
            unit.Expect (LoreToken.CloseParen);
            return parameters;
        }
    }
}

