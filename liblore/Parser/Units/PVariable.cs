using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        AssignStatement ParseValueDeclaration () {
            unit.Expect (LoreToken.Keyword, "val");
            return ParseAssignStatement ();
        }

        AssignStatement ParseVariableDeclaration () {
            unit.Expect (LoreToken.Keyword, "var");
            var stmt = ParseAssignStatement ();
            stmt.MakeMutable ();
            return stmt;
        }
    }
}

