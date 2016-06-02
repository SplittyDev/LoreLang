using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        StringExpression ParseString () {
            var token = unit.Expect (LoreToken.StringLiteral);
            return StringExpression.Create (unit.Location, token.Value);
        }
    }
}

