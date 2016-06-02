using System;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Ast visitor.
    /// </summary>
    public abstract class AstVisitor {

        public SourceLocation Location;

        protected AstVisitor () {
            Location = SourceLocation.Zero;
        }

        public virtual void Accept (Dummy dummy) => Update (dummy);

        public virtual void Accept (AstRoot root) => Update (root);
        public virtual void Accept (CodeBlock block) => Update (block);
        public virtual void Accept (FunctionDeclaration func) => Update (func);
        public virtual void Accept (ArgumentList args) => Update (args);
        public virtual void Accept (NameExpression name) => Update (name);
        public virtual void Accept (BinaryExpression expr) => Update (expr);
        public virtual void Accept (UnaryExpression expr) => Update (expr);
        public virtual void Accept (CallExpression call) => Update (call);
        public virtual void Accept (ListExpression args) => Update (args);
        public virtual void Accept (TupleExpression tuple) => Update (tuple);
        public virtual void Accept (AssignStatement stmt) => Update (stmt);
        public virtual void Accept (LambdaExpression lambda) => Update (lambda);
        public virtual void Accept (ReturnStatement stmt) => Update (stmt);
        public virtual void Accept (IntegerExpression expr) => Update (expr);
        public virtual void Accept (FloatExpression expr) => Update (expr);
        public virtual void Accept (StringExpression expr) => Update (expr);

        void Update (AstNode node) => Location = node.Location;
    }
}