using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Assign statement node.
    /// </summary>
    public class AssignStatement: AstNode {

        /// <summary>
        /// Whether the assignment is global.
        /// </summary>
        bool globalscope;

        /// <summary>
        /// Whether this assignment is mutable.
        /// </summary>
        bool mutable;

        /// <summary>
        /// The identifiers.
        /// </summary>
        public readonly List<NamedParameter> Identifiers;

        /// <summary>
        /// The expressions.
        /// </summary>
        public readonly List<AstNode> Expressions;

        /// <summary>
        /// Gets whether the assignment is global.
        /// </summary>
        /// <value>Whether the assignment is global.</value>
        public bool Global => globalscope;

        /// <summary>
        /// Gets whether this assignment is mutable.
        /// </summary>
        /// <value>Whether this assignment is mutable.</value>
        public bool Mutable => mutable;

        /// <summary>
        /// Whether the value is packed as a tuple.
        /// </summary>
        public bool Packed => Identifiers.Count > 1 && Expressions.Count == 1;

        /// <summary>
        /// Gets the identifier count.
        /// </summary>
        /// <value>The identifier count.</value>
        public int IdentifierCount => Identifiers.Count;

        /// <summary>
        /// Gets the expression count.
        /// </summary>
        /// <value>The expression count.</value>
        public int ExpressionCount => Expressions.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignStatement"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        AssignStatement (SourceLocation location) : base (location) {
            Identifiers = new List<NamedParameter> ();
            Expressions = new List<AstNode> ();
        }

        /// <summary>
        /// Create a new instance of the <see cref="AssignStatement"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public static AssignStatement Create (SourceLocation location) => new AssignStatement (location);

        /// <summary>
        /// Adds an identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        /// <param name="identifier">Identifier.</param>
        public void AddIdentifier (NameExpression identifier) {
            Identifiers.Add (NamedParameter.Create (identifier));
        }

        /// <summary>
        /// Adds an identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        /// <param name="identifier">Identifier.</param>
        public void AddIdentifier (NamedParameter identifier) {
            Identifiers.Add (identifier);
        }

        /// <summary>
        /// Adds an expression.
        /// </summary>
        /// <returns>The expression.</returns>
        /// <param name="expression">Expression.</param>
        public void AddExpression (AstNode expression) {
            Expressions.Add (expression);
        }

        /// <summary>
        /// Makes the assignment global.
        /// </summary>
        public void MakeGlobal () {
            globalscope = true;
        }

        /// <summary>
        /// Makes the assignment mutable.
        /// </summary>
        /// <returns>The mutable.</returns>
        public void MakeMutable () {
            mutable = true;
        }

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Visit (AstVisitor visitor) {
            visitor.Accept (this);
        }

        /// <summary>
        /// Visits the children.
        /// </summary>
        /// <returns>The children.</returns>
        /// <param name="visitor">Visitor.</param>
        public override void VisitChildren (AstVisitor visitor) {
            Expressions.ForEach (expr => expr.Visit (visitor));
        }

        public override string ToString () => $"[Assign: " +
        $"To=[{string.Join (", ", Identifiers)}] Global={Global} Mutable={Mutable}]]";
    }
}

