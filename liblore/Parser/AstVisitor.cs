using System;

namespace Lore {

    /// <summary>
    /// Ast visitor.
    /// </summary>
    public abstract class AstVisitor {

        public virtual void Accept (CodeBlock root) { }
    }
}