using System;
using System.Collections.Generic;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Prototype compiler.
    /// </summary>
    public class PrototypeCompiler : AstVisitor {

        /// <summary>
        /// The compiler.
        /// </summary>
        readonly LoreLLVMCompiler Compiler;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrototypeCompiler"/> class.
        /// </summary>
        /// <param name="compiler">Compiler.</param>
        PrototypeCompiler (LoreLLVMCompiler compiler) {
            Compiler = compiler;
        }

        /// <summary>
        /// Analyze the specified compiler and root.
        /// </summary>
        /// <param name="compiler">Compiler.</param>
        /// <param name="root">Root.</param>
        public static void Analyze (LoreLLVMCompiler compiler, AstRoot root) {
            var visitor = new PrototypeCompiler (compiler);
            visitor.Analyze (root);
        }

        /// <summary>
        /// Analyze the specified root.
        /// </summary>
        /// <param name="root">Root.</param>
        public void Analyze (AstRoot root) {
            Accept (root);
        }

        public override void Accept (AstRoot root) {
            base.Accept (root);
            root.VisitChildren (this);
        }

        public override void Accept (FunctionDeclaration func) {
            base.Accept (func);
            Compiler.DeclareFunctionPrototype (func);
        }
    }
}

