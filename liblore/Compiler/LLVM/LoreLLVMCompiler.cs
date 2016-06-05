using System;
using System.Collections.Generic;
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
        /// The module.
        /// </summary>
        readonly LoreModule LoreModule;

        /// <summary>
        /// The helpers.
        /// </summary>
        readonly TypeHelper Helper;

        /// <summary>
        /// A false LLVM boolean.
        /// </summary>
        readonly LLVMBool LLVMFalse;

        /// <summary>
        /// A true LLVM boolean.
        /// </summary>
        readonly LLVMBool LLVMTrue;

        /// <summary>
        /// A LLVM NULL void pointer.
        /// </summary>
        readonly LLVMValueRef LLVMNull;

        /// <summary>
        /// The symbol table.
        /// </summary>
        readonly SymbolTable Table;

        /// <summary>
        /// The stack.
        /// </summary>
        readonly Stack<Symbol> Stack;

        /// <summary>
        /// The LLVM module reference.
        /// </summary>
        LLVMModuleRef LLVMModule;

        /// <summary>
        /// The LLVM builder reference.
        /// </summary>
        LLVMBuilderRef Builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreLLVMCompiler"/> class.
        /// </summary>
        /// <param name="root">The AST root.</param>
        LoreLLVMCompiler (AstRoot root, LoreModule module, SymbolTable table = null) {

            // Initialize x86 target
            LLVM.InitializeX86Target ();
            LLVM.InitializeX86TargetInfo ();
            LLVM.InitializeX86AsmPrinter ();
            LLVM.InitializeX86TargetMC ();
            LLVM.LinkInMCJIT ();

            // Assign parameters
            Root = root;
            LoreModule = module;
            Table = table ?? SymbolTable.Create ();

            // Create the type helper
            Helper = TypeHelper.Create (this);

            // Create the stack
            Stack = new Stack<Symbol> (capacity: 512);

            // Create LLVM constants
            LLVMFalse = new LLVMBool (0);
            LLVMTrue = new LLVMBool (1);
            LLVMNull = new LLVMValueRef (IntPtr.Zero);

            // Create the LLVM IR builder
            Builder = LLVM.CreateBuilder ();

            // Create the LLVM module
            LLVMModule = LLVM.ModuleCreateWithName (module.Name);
        }

        public static LoreLLVMCompiler Create (AstRoot root, LoreModule module) {
            return new LoreLLVMCompiler (root, module);
        }

        public static LoreLLVMCompiler Create (SymbolTable symbols, AstRoot root, LoreModule module) {
            return new LoreLLVMCompiler (root, module, symbols);
        }

        public static LoreLLVMCompiler CreateFromFile (string fileName) {
            try {
                var sunit = SourceUnit.FromFile (fileName);
                var lexer = LoreLexer.Create (sunit);
                var lexemes = lexer.Tokenize ();
                var punit = ParsingUnit.Create (lexemes);
                var parser = LoreParser.Create (punit);
                var ast = parser.Parse ();
                return Create (ast, LoreModule.Create (fileName));
            } catch (LoreException e) {
                Console.WriteLine (e.Message);
                throw;
            } catch (Exception e) {
                Console.WriteLine ($"Fatal exception: ${e.Message}");
                throw;
            }
        }

        delegate int MainFunctionDelegate (int args);

        void BuildStub () {
            var entryType = LLVM.FunctionType (
                ReturnType: LLVM.Int32Type (),
                ParamTypes: new LLVMTypeRef [0],
                IsVarArg: false
            );
            var entryFunc = LLVM.AddFunction (LLVMModule, "__lore_entrypoint__", entryType);
            LLVM.SetLinkage (entryFunc, LLVMLinkage.LLVMExternalLinkage);
            PrototypeCompiler.Analyze (this, Root);
            var entryBlock = LLVM.AppendBasicBlock (entryFunc, "entry");
            LLVM.PositionBuilderAtEnd (Builder, entryBlock);
            var mainFunc = LLVM.GetNamedFunction (LLVMModule, "main");
            if (mainFunc.Pointer == IntPtr.Zero) {
                Console.WriteLine ("No main function found!");
                LLVM.BuildRet (Builder, LLVM.ConstInt (LLVM.Int32Type (), 0, LLVMFalse));
            } else {
                Console.WriteLine ("Main function found!");
                var callResult = LLVM.BuildCall (Builder, mainFunc, new LLVMValueRef [0], "callmain");
                if (Helper.CompareType (callResult, LLVM.Int32Type ())) {
                    LLVM.BuildRet (Builder, callResult);
                } else {
                    LLVM.BuildRet (Builder, LLVM.ConstInt (LLVM.Int32Type (), 0, LLVMFalse));
                }
            }
        }

        public void Compile () {
            try {
                BuildStub ();
                Root.Visit (this);
                IntPtr lastError;
                LLVM.VerifyModule (LLVMModule, LLVMVerifierFailureAction.LLVMPrintMessageAction, out lastError);

                /* Probably superfluous
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
                pm.RunPassManager (LLVMModule);
                */
                LLVM.PrintModuleToFile (LLVMModule, $"{LoreModule.Name}.s", out lastError);
            } catch (LoreException e) {
                Console.WriteLine (e.Message);
                //Console.WriteLine ("\nStack trace:");
                //Console.WriteLine (e.StackTrace);
            } catch (Exception e) {
                Console.WriteLine ("\nLLVM Module Dump:");
                Console.WriteLine ($"Fatal exception: ${e.Message}");
            }
            LLVM.DumpModule (LLVMModule);
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

        public override void Accept (StringExpression expr) {
            base.Accept (expr);
            CompileString (expr);
        }
    }
}

