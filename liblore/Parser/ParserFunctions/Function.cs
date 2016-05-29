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
                function.SetReturnType (ParseName ());
            }

            // Read the function body
            var body = ParseBlock ();
            function.SetBody (body);

            // Return the function
            return function;
        }
    }
}

