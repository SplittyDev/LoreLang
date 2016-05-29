using System;

namespace Lore {

    /// <summary>
    /// Function parameter.
    /// </summary>
    public class FunctionParameter {

        /// <summary>
        /// The name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionParameter"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        FunctionParameter (string name) {
            Name = name;
        }

        /// <summary>
        /// Create a new instance of the <see cref="FunctionParameter"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        public static FunctionParameter Create (string name) => new FunctionParameter (name);

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString () => $"{Name}";
    }
}

