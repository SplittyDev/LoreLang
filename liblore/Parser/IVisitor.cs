using System;

namespace Lore {

    /// <summary>
    /// Visitor.
    /// </summary>
    public interface IVisitor {

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        void Visit (AstVisitor visitor);

        /// <summary>
        /// Visit the children of the specified visitor.
        /// </summary>
        /// <returns>The children.</returns>
        /// <param name="visitor">Visitor.</param>
        void VisitChildren (AstVisitor visitor);
    }
}

