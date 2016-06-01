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
        List<NamedParameter> ParseDeclarationArgumentList () {
            var parameters = new List<NamedParameter> ();
            unit.Expect (LoreToken.OpenParen);
            while (!unit.Match (LoreToken.CloseParen)) {

                // Parse the name of the parameter
                var parameter = NamedParameter.Create (ParseName ());

                // Parse the type of the parameter
                unit.Expect (LoreToken.Colon);
                parameter.SetType (ParseName ());
                parameters.Add (parameter);
            }
            unit.Expect (LoreToken.CloseParen);
            return parameters;
        }
    }
}

