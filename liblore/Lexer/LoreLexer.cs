using System;
using System.Runtime.CompilerServices;
using System.Text;
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
        }

        #region Predicates
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        bool IsIdentifier (char chr) => char.IsLetter (chr) || chr == '_';
        #endregion

        #region Scanners
        void ReadIdentifier (SourceUnit unit, ScanResult<LoreToken> result) {
            var accum = new StringBuilder ();
            var chr = unit.Peek ();
            while (unit.See () && char.IsLetterOrDigit (chr) || chr == '_') {
                accum.Append (chr);
            }
            result.Value = accum.ToString ();
            result.Token = LoreToken.Identifier;
        }
        #endregion
    }
}

