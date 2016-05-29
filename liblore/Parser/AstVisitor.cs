using System;

namespace Lore {

    /// <summary>
    /// Ast visitor.
    /// </summary>
    public abstract class AstVisitor {

        public virtual void Accept (AstRoot root) { }
        public virtual void Accept (CodeBlock block) { }
        public virtual void Accept (Function func) { }
    }
}