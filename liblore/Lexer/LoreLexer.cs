using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Globalization;
using LexDotNet;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore lexer.
    /// </summary>
    public class LoreLexer : LexerBase<LoreToken> {

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreLexer"/> class.
        /// </summary>
        /// <param name="unit">Unit.</param>
        LoreLexer (SourceUnit unit) : base (unit) {
        }

        /// <summary>
        /// Create a new instance of the <see cref="LoreLexer"/> class.
        /// </summary>
        /// <param name="unit">Unit.</param>
        public static LoreLexer Create (SourceUnit unit) => new LoreLexer (unit);

        /// <summary>
        /// Configures the scanners.
        /// </summary>
        /// <returns>The scanners.</returns>
        protected override void ConfigureScanners () {
            Scanners.Discard (ReadMultilineComment).If (IsMultilineComment);
            Scanners.Discard (ReadComment).If (IsComment);
            Scanners.Scan (ReadIdentifier).If (IsIdentifier);
            Scanners.Scan (ReadString).If (IsString);
            Scanners.Scan (ReadNumber).If (IsNumber);
            Scanners.Scan (ReadOperator).If (IsOperator);
            Scanners.Scan ('(').As (LoreToken.OpenParen);
            Scanners.Scan (')').As (LoreToken.CloseParen);
            Scanners.Scan ('{').As (LoreToken.OpenBrace);
            Scanners.Scan ('}').As (LoreToken.CloseBrace);
            Scanners.Scan ('[').As (LoreToken.OpenBracket);
            Scanners.Scan (']').As (LoreToken.CloseBracket);
            Scanners.Scan (',').As (LoreToken.Comma);
            Scanners.Scan (':').As (LoreToken.Colon);
            Scanners.Scan (';').As (LoreToken.Semicolon);
            Scanners.Scan ('.').As (LoreToken.MemberAccess);
        }

        #region Predicates
        bool IsComment (SourceUnit source) => source.See (1) && source.Peeks (2) == "//";
        bool IsMultilineComment (SourceUnit source) => source.See (1) && source.Peeks (2) == "/*";
        bool IsIdentifier (char chr) => char.IsLetter (chr) || chr == '_';
        bool IsOperator (char chr) => LoreLexerConstants.OperatorChars.Contains (chr);
        bool IsString (char chr) => chr == '\'' || chr == '"';
        bool IsNumber (SourceUnit unit) {
            var chr1 = unit.Peek ();
            var chr2 = unit.See () ? unit.Peek (1) : '\0';
            // Decimal
            if (char.IsDigit (chr1))
                return true;
            // Hexadecimal
            if (chr1 == '0' && chr2 == 'x')
                return true;
            // Float
            if (chr1 == '.' && char.IsDigit (chr2))
                return true;
            // Invalid
            return false;
        }
        #endregion

        #region Scanners
        void ReadComment (SourceUnit source) => source.SkipLine ();
        void ReadMultilineComment (SourceUnit source) {
            source.Skip (2);
            string nn = string.Empty;
            while (source.See (1) && (nn = source.Peeks (2)) != "*/") {
                source.Skip ();
            }
            if (nn != "*/") {
                Throw ("Unexpected end of file.");
            }
            source.Skip (2);
        }
        void ReadOperator (SourceUnit unit, ScanResult<LoreToken> result) {
            result.Token = LoreToken.Operator;
            var op1 = unit.Peek ();
            var op2 = unit.See (1) ? unit.Peeks (2) : string.Empty;
            var op3 = unit.See (2) ? unit.Peeks (3) : string.Empty;
            switch (op3) {
            case "==>":
                result.Value = op3;
                unit.Skip (3);
                return;
            }
            switch (op2) {
            case "->":
            case "=>":
            case "==":
            case "!=":
            case "+=":
            case "-=":
            case "*=":
            case "/=":
            case "%=":
            case "&=":
            case "|=":
            case "^=":
            case "<=":
            case ">=":
            case "&&":
            case "||":
                result.Value = op2;
                unit.Skip (2);
                return;
            }
            result.Value = op1.ToString ();
            unit.Skip ();
        }
        void ReadIdentifier (SourceUnit unit, ScanResult<LoreToken> result) {
            var accum = new StringBuilder ();
            var chr = unit.Peek ();
            while (unit.See (0) && (char.IsLetterOrDigit (chr) || chr == '_')) {
                accum.Append (unit.Read ());
                if (!unit.See (0)) {
                    break;
                }
                chr = unit.Peek ();
            }
            var str = accum.ToString ();
            result.Value = str;
            result.Token = LoreToken.Identifier;
            if (LoreLexerConstants.KeywordStrings.Contains (str)) {
                result.Token = LoreToken.Keyword;
            } else if (LoreLexerConstants.OperatorStrings.Contains (str)) {
                result.Token = LoreToken.Operator;
            }
        }
        void ReadString (SourceUnit source, ScanResult<LoreToken> result) {
            char delimiter;
            var str = JustReadString (source, out delimiter);
            result.Token = LoreToken.StringLiteral;
            result.Value = str;
        }
        void ReadNumber (SourceUnit unit, ScanResult<LoreToken> result) {
            const string HexChars = "abcdefABCDEF";
            var accum = new StringBuilder ();
            var isfloat = false;
            var nofloatstart = false;
            var nofloatend = false;
            var ishex = false;
            var chr = unit.Peek ();
            if (chr == '.') {
                nofloatstart = true;
                accum.Append ("0");
            } else if (chr == '0' && unit.See (2) && unit.Peek (1) == 'x') {
                ishex = true;
                unit.Skip (2);
            }
            if (!unit.See (0)) {
                Throw ("Unexpected end of file.");
            }
            while (unit.See (0) && (chr = unit.Peek ()) > 0) {
                if (chr == '.') {
                    if (!unit.See (1) || !char.IsDigit (unit.Peek (1))) {
                        nofloatend = true;
                        if (nofloatstart) {
                            Throw ("Floating-point literal must have a digit before or after the decimal point.");
                        }
                    }
                    if (isfloat) {
                        break;
                        // Throw ("There can only be one decimal point in a floating-point literal.");
                    }
                    isfloat = true;
                    accum.Append (unit.Read ());
                    if (nofloatend) {
                        accum.Append ('0');
                        break;
                    }
                    continue;
                }
                if (ishex) {
                    if (char.IsDigit (chr) || HexChars.Contains (chr)) {
                        accum.Append (unit.Read ());
                        continue;
                    }
                    break;
                }
                if (char.IsDigit (chr)) {
                    accum.Append (unit.Read ());
                    continue;
                }
                break;
            }
            var str = accum.ToString ();
            ulong hexvalue = 0;
            if (ishex) {
                if (!ulong.TryParse (str, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out hexvalue)) {
                    try {
                        ulong.Parse (str, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                    } catch (Exception e) {
                        Throw ($"Unable to parse base-16 integer literal: {e.Message}.");
                    }
                }
            } else if (isfloat) {
                try {
                    float.Parse (str, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
                } catch (Exception e) {
                    Throw ($"Unable to parse floating-point literal: {e.Message}.");
                }
            } else {
                try {
                    ulong.Parse (str, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                } catch (Exception e) {
                    Throw ($"Unable to parse integer literal: {e.Message}.");
                }
            }
            result.Token = isfloat ? LoreToken.FloatLiteral : LoreToken.IntLiteral;
            result.Value = ishex ? hexvalue.ToString () : str;
        }
        #endregion

        #region Supporting functions
        string JustReadString (SourceUnit source, out char delimiter) {
            var accum = new StringBuilder ();
            delimiter = source.Read ();
            var c = source.Peek ();
            while (source.See () && c != delimiter) {
                c = source.Read ();
                if (c == '\\') {
                    var next = source.Peek ();
                    var dict = new Dictionary<char, char> {
                        ['"'] = '\"',
                        ['0'] = '\0',
                        ['a'] = '\a',
                        ['b'] = '\b',
                        ['f'] = '\f',
                        ['n'] = '\n',
                        ['r'] = '\r',
                        ['t'] = '\t',
                        ['\''] = '\''
                    };
                    if (!dict.ContainsKey (next))
                        throw new Exception ($"Unrecognized escape sequence: '\\{next}'");
                    c = dict [next];
                    source.Skip ();
                }
                accum.Append (c);
                c = source.Peek ();
            }
            if (c != delimiter)
                throw new Exception ($"Unterminated string literal at {source.Location}");
            if (source.See ())
                source.Skip ();
            return accum.ToString ();
        }
        #endregion
    }
}

