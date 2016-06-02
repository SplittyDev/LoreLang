using System;

namespace Lore {

    /// <summary>
    /// Lore token.
    /// </summary>
    public enum LoreToken {
        None,
        Identifier,
        IntLiteral,
        FloatLiteral,
        StringLiteral,
        Operator,
        Keyword,
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        Comma,
        Colon,
        Semicolon,
        MemberAccess,
    }
}

