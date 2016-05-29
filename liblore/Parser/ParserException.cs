using System;

namespace Lore {

    /// <summary>
    /// Parser exception.
    /// </summary>
    public class ParserException : Exception {

        /// <summary>
        /// The message.
        /// </summary>
        public new string Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserException"/> class.
        /// </summary>
        /// <param name="unit">Unit.</param>
        /// <param name="message">Message.</param>
        public ParserException (ParsingUnit unit, string message) {
            Message = $"At {unit.Location}: {message}";
        }
    }
}

