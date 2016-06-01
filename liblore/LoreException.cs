using System;
using System.Diagnostics;
using System.Text;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Parser exception.
    /// </summary>
    public class LoreException : Exception {

        /// <summary>
        /// The message.
        /// </summary>
        public new string Message;

        string Header;
        string Description;
        string Solution;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreException"/> class.
        /// </summary>
        /// <param name="loc">Location.</param>
        public LoreException (SourceLocation loc) {
            Header = $"At Line {loc.Line} Pos {loc.Position}:";
            Description = string.Empty;
            Solution = string.Empty;
            BuildMessage ();
        }

        /// <summary>
        /// Creates a new Lore exception.
        /// </summary>
        /// <param name="loc">Location.</param>
        public static LoreException Create (SourceLocation loc = null)
        => new LoreException (loc ?? SourceLocation.Zero);

        public LoreException Describe (string line) {
            Description = $"{Description}\n| D | {line}";
            BuildMessage ();
            return this;
        }

        public LoreException Resolve (string line) {
            Solution = $"{Solution}\n| S | {line}";
            BuildMessage ();
            return this;
        }

        public void BuildMessage () {
            var accum = new StringBuilder ();
            accum.Append (Header);
            if (!string.IsNullOrEmpty (Description)) {
                accum.Append (Description);
            }
            if (!string.IsNullOrEmpty (Solution)) {
                accum.Append (Solution);
            }
            Message = accum.ToString ();
        }
    }
}

