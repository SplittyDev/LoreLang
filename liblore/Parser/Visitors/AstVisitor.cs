using System;

namespace Lore {

    /// <summary>
    /// Ast visitor.
    /// </summary>
    public abstract class AstVisitor {

        public virtual void Accept (Dummy dummy) { }

        public virtual void Accept (AstRoot root) { }
        public virtual void Accept (CodeBlock block) { }
        public virtual void Accept (FunctionDeclaration func) { }
        public virtual void Accept (ArgumentList args) { }
        public virtual void Accept (NameExpression name) { }
        public virtual void Accept (BinaryExpression expr) { }
        public virtual void Accept (UnaryExpression expr) { }
        public virtual void Accept (CallExpression call) { }
        public virtual void Accept (ListExpression args) { }
        public virtual void Accept (TupleExpression tuple) { }
        public virtual void Accept (AssignStatement stmt) { }
        public virtual void Accept (LambdaExpression lambda) { }
        public virtual void Accept (ReturnStatement stmt) { }
    }
}