using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Globalization;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Lore lexer.
    /// </summary>
    public class LoreLexer : LexerBase<LoreToken> {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lore.LoreLexer"/> class.
        /// </summary>
        /// <param name="unit">Unit.</param>
        public LoreLexer (SourceUnit unit) : base (unit) {
        }

        /// <summary>
        /// Configures the scanners.
        /// </summary>
        /// <returns>The scanners.</returns>
        protected override void ConfigureScanners () {
            Scanners.Scan (ReadIdentifier).If (IsIdentifier);
            Scanners.Scan (ReadNumber).If (IsNumber);
        }

        #region Predicates
        bool IsIdentifier (char chr) => char.IsLetter (chr) || chr == '_';
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
            result.Value = accum.ToString ();
            result.Token = LoreToken.Identifier;
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
            if (!unit.See ()) {
                Throw ("Unexpected end of file.");
            }
            while (unit.See (0) && (chr = unit.Peek ()) > 0) {
                if (chr == '.') {
                    if (!unit.See (1) || !char.IsDigit (unit.Peek (1))) {
                        nofloatend = true;
                        if (nofloatstart) {
                            Throw ("Floating-point literal must have a digit before or after the decimal point.");
                        }
                    } else if (isfloat) {
                        Throw ("There can only be one decimal point in a floating-point literal.");
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
                    Throw ("Unexpected character in base-16 integer literal.");
                } else if (char.IsDigit (chr)) {
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
    }
}

