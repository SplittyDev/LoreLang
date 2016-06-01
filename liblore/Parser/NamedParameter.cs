using System;

namespace Lore {

    /// <summary>
    /// Named parameter.
    /// </summary>
    public class NamedParameter {

        /// <summary>
        /// The name.
        /// </summary>
        public readonly NameExpression Name;

        /// <summary>
        /// The type.
        /// </summary>
        NameExpression type;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public NameExpression Type => type;

        /// <summary>
        /// Gets whether the parameter has a type.
        /// </summary>
        /// <value>Whether the parameter has a type.</value>
        public bool HasType => type != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedParameter"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        NamedParameter (NameExpression name) {
            Name = name;
        }

        /// <summary>
        /// Create a new instance of the <see cref="NamedParameter"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        public static NamedParameter Create (NameExpression name) => new NamedParameter (name);

        /// <summary>
        /// Sets the type.
        /// </summary>
        /// <returns>The type.</returns>
        /// <param name="typeName">Type name.</param>
        public void SetType (NameExpression typeName) {
            type = typeName;
        }

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString () => $"{Name}";
    }
}

