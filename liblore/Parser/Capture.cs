using System;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Capture.
    /// </summary>
    public class Capture {

        /// <summary>
        /// The identifier that is captured by this capture.
        /// </summary>
        public readonly string Identifier;

        /// <summary>
        /// Whether this capture captures all data.
        /// </summary>
        public readonly bool CapturesAll;

        /// <summary>
        /// Initializes a new instance of the <see cref="Capture"/> class.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="capturesAll">Captures all.</param>
        Capture (string identifier, bool capturesAll) {
            Identifier = identifier;
            CapturesAll = capturesAll;
        }

        /// <summary>
        /// Create a new instance of the <see cref="Capture"/> class.
        /// </summary>
        /// <param name="unit">Unit.</param>
        /// <param name="lex">Lex.</param>
        public static Capture Create (ParsingUnit unit, Lexeme<LoreToken> lex) {
            if (lex.Is (LoreToken.Identifier)) {
                return new Capture (lex.Value, capturesAll: false);
            }
            if (lex.Is (LoreToken.Operator) && lex.Is ("=")) {
                return new Capture (lex.Value, capturesAll: true);
            }
            throw new ParserException (unit, $"Not a capture: '{lex.Value}' ({lex.Token})");
        }

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString () => CapturesAll ? "All" : Identifier;
    }
}

