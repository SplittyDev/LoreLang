using System;

namespace Lore {
    
    public static class LoreLexerConstants {
        
        public static readonly char [] OperatorChars = {
            '!', '%', '&', '*',
            '+', '-', '.', '/',
            '<', '=', '>', '?',
            '^', '|', '~'
        };

        public static readonly string [] OperatorStrings = {
            "export"
        };

        public static readonly string [] KeywordStrings = {
            "fn",
            "let"
        };
    }
}

