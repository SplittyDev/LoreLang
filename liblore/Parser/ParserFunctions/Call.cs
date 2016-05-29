﻿using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        AstNode ParseCallOrAccess () {
            var term = ParseTerm ();
            return ParseCallOrAccess (term);
        }

        AstNode ParseCallOrAccess (AstNode left) {

            // Function call
            if (unit.Match (LoreToken.OpenParen)) {

                // Parse argument list of call
                var args = ParseArgumentList ();
                var call = CallExpression.Create (
                    location: unit.Location,
                    target: left,
                    arguments: args
                );
                return ParseCallOrAccess (call);
            }

            // Array indexer
            /*
            if (unit.Match (LoreToken.OpenBracket)) {
                AstNode indexer = null; // TODO: Implement ParseIndexer
                return ParseCall (indexer);
            }
            */

            // Member access
            /*
            if (unit.Match (LoreToken.MemberAccess)) {
                AstNode accessor = null; // TODO: Implement ParseMemberAccess
                return ParseCall (accessor);
            }
            */

            return left;
        }
    }
}

