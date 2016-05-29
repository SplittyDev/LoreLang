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
        /// The identifiers.
        /// </summary>
        public readonly List<string> Identifiers;

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
        /// Whether the value is packed as a tuple.
        /// </summary>
        public bool Packed => Identifiers.Count > 1 && Expressions.Count == 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignStatement"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        AssignStatement (SourceLocation location) : base (location) {
            Identifiers = new List<string> ();
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
        public void AddIdentifier (string identifier) {
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
    }
}

