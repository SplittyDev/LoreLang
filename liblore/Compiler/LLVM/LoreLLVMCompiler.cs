using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LexDotNet;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore LLVM compiler backend.
    /// </summary>
    public partial class LoreLLVMCompiler : AstVisitor {

        /// <summary>
        /// The AST root.
        /// </summary>
        readonly AstRoot Root;

        /// <summary>
        /// The helpers.
        /// </summary>
        internal readonly TypeHelper Helper;

        /// <summary>
        /// A false LLVM boolean.
        /// </summary>
        internal readonly LLVMBool LLVMFalse;

        /// <summary>
        /// A true LLVM boolean.
        /// </summary>
        internal readonly LLVMBool LLVMTrue;

        /// <summary>
        /// A LLVM NULL void pointer.
        /// </summary>
        internal readonly LLVMValueRef LLVMNull;

        /// <summary>
        /// The symbol table.
        /// </summary>
        internal readonly SymbolTable Table;

        /// <summary>
        /// The stack.
        /// </summary>
        internal readonly Stack<LLVMValueRef> Stack;

        /// <summary>
        /// The LLVM module reference.
        /// </summary>
        internal LLVMModuleRef Module;

        /// <summary>
        /// The LLVM builder reference.
        /// </summary>
        internal LLVMBuilderRef Builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreLLVMCompiler"/> class.
        /// </summary>
        /// <param name="root">The AST root.</param>
        LoreLLVMCompiler (AstRoot root, string module = null) {
            LLVM.LinkInMCJIT ();
            LLVM.InitializeNativeTarget ();
            LLVM.InitializeNativeAsmParser ();
            LLVM.InitializeNativeAsmPrinter ();
            LLVM.InitializeNativeDisassembler ();
            Root = root;
            Table = SymbolTable.Create ();
            Helper = TypeHelper.Create (this);
            Stack = new Stack<LLVMValueRef> ();
            LLVMFalse = new LLVMBool (0);
            LLVMTrue = new LLVMBool (1);
            LLVMNull = new LLVMValueRef (IntPtr.Zero);
            Builder = LLVM.CreateBuilder ();
            Module = LLVM.ModuleCreateWithName (module ?? "__anonymous__");
            PrototypeCompiler.Analyze (this, root);
        }

        public static LoreLLVMCompiler Create (AstRoot root, string moduleName = null) {
            return new LoreLLVMCompiler (root, moduleName);
        }

        public static LoreLLVMCompiler CreateFromFile (string fileName) {
            try {
                var sunit = SourceUnit.FromFile (fileName);
                var lexer = LoreLexer.Create (sunit);
                var lexemes = lexer.Tokenize ();
                var punit = ParsingUnit.Create (lexemes);
                var parser = LoreParser.Create (punit);
                var ast = parser.Parse ();
                return Create (ast, Path.GetFileNameWithoutExtension (fileName));
            } catch (LoreException e) {
                Console.WriteLine (e.Message);
                throw;
            } catch (Exception e) {
                Console.WriteLine ($"Fatal exception: ${e.Message}");
                throw;
            }
        }

        delegate int MainFunctionDelegate (int args);

        public void Compile () {
            try {
                Root.Visit (this);
                IntPtr lastError;
                LLVM.VerifyModule (Module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out lastError);

                var pm = new PassManager ();
                pm.AddVerifierPass ();
                pm.AddBasicAliasAnalysisPass ();
                pm.AddPromoteMemoryToRegisterPass ();
                pm.AddInstructionCombiningPass ();
                pm.AddReassociatePass ();
                pm.AddGVNPass ();
                pm.AddCFGSimplificationPass ();
                pm.AddScalarizerPass ();
                pm.AddIPConstantPropagationPass ();
                pm.InitializeFunctionPassManager ();
                pm.RunPassManager (Module);
                Console.WriteLine ("\nLLVM Module Dump:");
                LLVM.PrintModuleToFile (Module, "test.s", out lastError);
            } catch (LoreException e) {
                Console.WriteLine (e.Message);
                //Console.WriteLine ("\nStack trace:");
                //Console.WriteLine (e.StackTrace);
            } catch (Exception e) {
                Console.WriteLine ($"Fatal exception: ${e.Message}");
            }
            LLVM.DumpModule (Module);
        }

        public override void Accept (AstRoot root) {
            base.Accept (root);
            root.VisitChildren (this);
        }

        public override void Accept (FunctionDeclaration func) {
            base.Accept (func);
            CompileFunction (func);
        }

        public override void Accept (AssignStatement stmt) {
            base.Accept (stmt);
            CompileAssignment (stmt);
        }

        public override void Accept (UnaryExpression expr) {
            base.Accept (expr);
            CompileUnaryOperation (expr);
        }

        public override void Accept (BinaryExpression expr) {
            base.Accept (expr);
            CompileBinaryOperation (expr);
        }

        public override void Accept (IntegerExpression expr) {
            base.Accept (expr);
            CompileIntegerExpression (expr);
        }

        public override void Accept (FloatExpression expr) {
            base.Accept (expr);
            CompileFloatExpression (expr);
        }

        public override void Accept (CallExpression call) {
            base.Accept (call);
            CompileCall (call);
        }

        public override void Accept (NameExpression name) {
            base.Accept (name);
            CompileName (name);
        }

        public override void Accept (TupleExpression tuple) {
            base.Accept (tuple);
            CompileTuple (tuple);
        }
    }
}

