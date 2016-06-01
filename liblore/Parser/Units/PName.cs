using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        NameExpression ParseName () {
            var ident = unit.Expect (LoreToken.Identifier).Value;
            return NameExpression.Create (unit.Location, ident);
        }
    }
}

