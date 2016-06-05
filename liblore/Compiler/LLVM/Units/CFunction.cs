using System;
using System.IO;
using System.Text;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        internal void DeclareFunctionPrototype (FunctionDeclaration funcdecl) {

            // Prepare argument type array
            var argumentCount = funcdecl.Parameters.Count;
            var arguments = new LLVMTypeRef [Math.Max (argumentCount, 0)];

            // Check if the function is already defined
            Symbol sym;
            if ((Table.TopScope.IsFunction && Table.TopScope.FindSymbol (funcdecl.Name, out sym))
                || (!Table.TopScope.IsFunction && Table.FindSymbol (funcdecl.Name, out sym))) {
                var e = LoreException.Create (funcdecl.Location).Describe ($"Redefinition of function '{funcdecl.Name}'!");
                e.Describe ($"Previous declaration was at {sym.Location}.");
                e.Resolve ($"Rename the function to avoid a collision.");
                throw e;
            }

            // Add function parameter types to array
            for (var i = 0; i < funcdecl.Parameters.Count; i++) {
                arguments [i] = Helper.GetBuiltinTypeFromString (funcdecl.Parameters [i].Type.Name);
            }

            // Check if the function returns multiple values
            var returnType = LLVM.VoidType ();
            var tupleReturnTypes = new LLVMTypeRef [0];
            if (funcdecl.ReturnsTuple) {
                var count = funcdecl.TupleReturnTypes.Count;
                tupleReturnTypes = new LLVMTypeRef [count];
                for (var i = 0; i < count; i++) {
                    var current = funcdecl.TupleReturnTypes [i];
                    tupleReturnTypes [i] = Helper.GetBuiltinTypeFromString (current.Name);
                }
                returnType = LLVM.StructType (tupleReturnTypes, true);
            }

            // Create the return type
            else if (funcdecl.HasReturnType) {
                returnType = Helper.GetBuiltinTypeFromString (funcdecl.ReturnType.Name);
            }

            // Create the function type
            var functionType = LLVM.FunctionType (
                ReturnType: returnType,
                ParamTypes: arguments,
                IsVarArg: LLVMFalse
            );

            // Create the actual function
            var functionRef = LLVM.AddFunction (LLVMModule, funcdecl.Name, functionType);
            LLVM.SetLinkage (functionRef, LLVMLinkage.LLVMExternalLinkage);

            // Add the function prototype as a symbol
            Table.AddSymbol (Symbol.CreatePrototype (funcdecl.Name, functionRef, funcdecl.Location));
        }

        void CompileFunction (FunctionDeclaration funcdecl) {

            // Check if a symbol with the name of the function is already defined
            Symbol sym = null;

            // Check if only local functions should be generated
            if (Table.TopScope.IsFunction) {
                
                // Declare the function
                DeclareFunctionPrototype (funcdecl);
            }

            if (!Table.FindSymbol (funcdecl.Name, out sym)) {
                throw LoreException.Create (funcdecl.Location).Describe ($"Undefined function: '{funcdecl.Name}'.");
            }

            // Check if the symbol is a prototype
            if (!sym.IsPrototype) {
                throw new LoreException (funcdecl.Location).Describe ($"Function without prototype: '{funcdecl.Name}'.");
            }

            // Get the function prototype
            LLVMValueRef functionRef = sym.Value;

            // Enter a new scope for the function
            Table.PushScope (Scope.CreateFunction (functionRef));

            // Check if the function returns multiple values
            var returnType = LLVM.VoidType ();
            var tupleReturnTypes = new LLVMTypeRef [0];
            if (funcdecl.ReturnsTuple) {
                var count = funcdecl.TupleReturnTypes.Count;
                tupleReturnTypes = new LLVMTypeRef [count];
                for (var i = 0; i < count; i++) {
                    var current = funcdecl.TupleReturnTypes [i];
                    tupleReturnTypes [i] = Helper.GetBuiltinTypeFromString (current.Name);
                }
                returnType = LLVM.StructType (tupleReturnTypes, true);
            }

            // Create the return type
            else if (funcdecl.HasReturnType) {
                returnType = Helper.GetBuiltinTypeFromString (funcdecl.ReturnType.Name);
            }

            // Add function parameters
            for (var i = 0; i < funcdecl.Parameters.Count; i++) {
                var argname = funcdecl.Parameters [i].Name.Name;

                // Get the i-th function parameter
                var param = LLVM.GetParam (functionRef, (uint)i);

                // Set the correct name
                LLVM.SetValueName (param, argname);

                // Add the named parameter to the scope
                Table.AddSymbol (Symbol.Create (argname, param, Location));
            }

            // Create the entry block for the function
            var block = LLVM.AppendBasicBlock (functionRef, "entry");
            LLVM.PositionBuilderAtEnd (Builder, block);

            // Visit the function body
            funcdecl.VisitChildren (this);

            // Return void and clear the stack if the
            // function return type is void
            if (Helper.CompareType (returnType, LLVM.VoidType ())) {

                // Build return statement
                LLVM.BuildRetVoid (Builder);
            } else if (Stack.Count == 0) {

                // The function has a return type,
                // but nothing was returned!
                if (funcdecl.HasReturnType) {
                    throw LoreException.Create (Location)
                                       .Describe ($"Function '{funcdecl.Name}' has return type '{funcdecl.ReturnType.Name}' but does not return anything!")
                                       .Resolve ($"Return a value of type '{funcdecl.ReturnType.Name}' or change the return type to 'void'.");
                }

                // There are no elements on the stack
                // Just return void
                LLVM.BuildRetVoid (Builder);
            } else if (funcdecl.ReturnsTuple && Stack.Count >= funcdecl.TupleReturnTypes.Count) {

                // Prepare element array
                var elems = new LLVMValueRef [funcdecl.TupleReturnTypes.Count];

                // Fill the element array
                for (var i = 0; i < elems.Length; i++) {
                    
                    // Pop the element from the stack
                    var current = Stack.Pop ().Value;

                    // Check if the current element is a defined symbol
                    if (Table.FindSymbolByRef (current, out sym)) {

                        // Check if the current element is a pointer
                        if (sym.IsPointer) {

                            // Load the element
                            current = LLVM.BuildLoad (Builder, current, "tmpload");
                        }
                    }

                    // Cast the element to the right type
                    current = Helper.BuildCast (Builder, current, tupleReturnTypes [i]);

                    // Add the element to the array
                    elems [elems.Length - i - 1] = current;
                }

                // Build the return statement
                LLVM.BuildAggregateRet (Builder, elems);
                // What the hellt
            } else if (funcdecl.HasReturnType && Stack.Count > 0) {

                // There are elements on the stack
                // Take the top element from the stack
                var elem = Stack.Pop ().Value;

                // Check if the return element is a defined symbol
                if (Table.FindSymbolByRef (elem, out sym)) {

                    // Check if the return element is a pointer
                    if (sym.IsPointer) {

                        // Load the element
                        elem = LLVM.BuildLoad (Builder, elem, "tmpload");
                    }
                }

                Helper.BuildOptimalReturn (Builder, elem, returnType);
            } else {
                throw LoreException.Create (Location)
                                   .Describe ($"Unable to construct a return value for function '{funcdecl.Name}'.")
                                   .Describe ($"This is a compiler bug.");
            }

            // Validate the function
            if (LLVM.VerifyFunction (functionRef, LLVMVerifierFailureAction.LLVMPrintMessageAction).Value != 0) {
                throw LoreException.Create (funcdecl.Location).Describe ($"Function failed to compile: '{funcdecl.Name}'");
            }

            // Exit the scope of the function
            Table.PopScope ();

            if (Table.TopScope.IsFunction) {
                var func = Table.TopScope.Function;
                LLVM.PositionBuilderAtEnd (Builder, func.GetEntryBasicBlock ());
            }

            // Add the function symbol
            Table.AddSymbol (Symbol.Create (funcdecl.Name, functionRef, funcdecl.Location));
            Stack.Clear ();
        }

        /* Original code
        void CompileGlobalFunction (FunctionDeclaration funcdecl) {

            // Clear the stack
            Stack.Clear ();

            // Enter a new scope for the function
            Table.PushScope (Scope.CreateFunction ());

            // Prepare argument type array
            var argumentCount = funcdecl.Parameters.Count;
            var arguments = new LLVMTypeRef [Math.Max (argumentCount, 0)];

            // Check if the function is already defined
            var functionRef = LLVM.GetNamedFunction (Module, funcdecl.Name);
            if (functionRef.Pointer != IntPtr.Zero) {
                Symbol prevdecl = null;
                var e = LoreException.Create (funcdecl.Location).Describe ($"Redefinition of function '{funcdecl.Name}'!");
                if (Table.FindSymbol (funcdecl.Name, out prevdecl)) {
                    e.Describe ($"Previous declaration was at {prevdecl.Location}.");
                }
                e.Resolve ($"Rename the function to avoid a collision.");
                throw e;
            }

            // Add function parameter types to array
            for (var i = 0; i < funcdecl.Parameters.Count; i++) {
                arguments [i] = Helper.GetBuiltinTypeFromString (funcdecl.Parameters [i].Type.Name);
            }

            // Check if the function returns multiple values
            var returnType = LLVM.VoidType ();
            var tupleReturnTypes = new LLVMTypeRef [0];
            if (funcdecl.ReturnsTuple) {
                var count = funcdecl.TupleReturnTypes.Count;
                tupleReturnTypes = new LLVMTypeRef [count];
                for (var i = 0; i < count; i++) {
                    var current = funcdecl.TupleReturnTypes [i];
                    tupleReturnTypes [i] = Helper.GetBuiltinTypeFromString (current.Name);
                }
                returnType = LLVM.StructType (tupleReturnTypes, true);
            }

            // Create the return type
            else if (funcdecl.HasReturnType) {
                returnType = Helper.GetBuiltinTypeFromString (funcdecl.ReturnType.Name);
            }

            // Create the function type
            var functionType = LLVM.FunctionType (
                ReturnType: returnType,
                ParamTypes: arguments,
                IsVarArg: LLVMFalse
            );

            // Create the actual function
            functionRef = LLVM.AddFunction (Module, funcdecl.Name, functionType);

            // Add function parameters
            for (var i = 0; i < funcdecl.Parameters.Count; i++) {
                var argname = funcdecl.Parameters [i].Name.Name;

                // Get the i-th function parameter
                var param = LLVM.GetParam (functionRef, (uint)i);

                // Set the correct name
                LLVM.SetValueName (param, argname);

                // Add the named parameter to the scope
                Table.AddSymbol (Symbol.Create (argname, param, Location));
            }

            // Create the entry block for the function
            var block = LLVM.AppendBasicBlock (functionRef, "entry");
            LLVM.PositionBuilderAtEnd (Builder, block);

            // Visit the function body
            funcdecl.VisitChildren (this);

            // Check if the stack contains elements
            if (Stack.Count == 0) {

                // The function has a return type,
                // but nothing was returned!
                if (funcdecl.HasReturnType) {
                    throw LoreException.Create (Location)
                                       .Describe ($"Function '{funcdecl.Name}' has return type '{funcdecl.ReturnType.Name}' but does not return anything!")
                                       .Resolve ($"Return a value of type '{funcdecl.ReturnType.Name}' or change the return type to 'void'.");
                }

                // There are no elements on the stack
                // Just return void
                LLVM.BuildRetVoid (Builder);
            } else if (funcdecl.ReturnsTuple && Stack.Count >= funcdecl.TupleReturnTypes.Count) {

                // Prepare element array
                var elems = new LLVMValueRef [funcdecl.TupleReturnTypes.Count];

                // Fill the element array
                for (var i = 0; i < elems.Length; i++) {
                    
                    // Pop the element from the stack
                    var current = Stack.Pop ();

                    // Check if the current element is a defined symbol
                    Symbol sym;
                    if (Table.FindSymbolByRef (current, out sym)) {

                        // Check if the current element is a pointer
                        if (sym.IsPointer) {

                            // Load the element
                            current = LLVM.BuildLoad (Builder, current, "tmpload");
                        }
                    }

                    // Cast the element to the right type
                    current = Helper.BuildCast (Builder, current, tupleReturnTypes [i]);

                    // Add the element to the array
                    elems [elems.Length - i - 1] = current;
                }

                // Build the return statement
                LLVM.BuildAggregateRet (Builder, elems);
                // What the hellt
            } else if (funcdecl.HasReturnType && Stack.Count > 0) {

                // There are elements on the stack
                // Take the top element from the stack
                var elem = Stack.Pop ();

                // Check if the return element is a defined symbol
                Symbol sym;
                if (Table.FindSymbolByRef (elem, out sym)) {

                    // Check if the return element is a pointer
                    if (sym.IsPointer) {

                        // Load the element
                        elem = LLVM.BuildLoad (Builder, elem, "tmpload");
                    }
                }

                Helper.BuildOptimalReturn (Builder, elem, returnType);
            } else {
                throw LoreException.Create (Location)
                                   .Describe ($"Unable to construct a return value for function '{funcdecl.Name}'.")
                                   .Describe ($"This is a compiler bug.");
            }

            // Validate the function
            if (LLVM.VerifyFunction (functionRef, LLVMVerifierFailureAction.LLVMPrintMessageAction).Value != 0) {
                throw LoreException.Create (funcdecl.Location).Describe ($"Function failed to compile: '{funcdecl.Name}'");
            }

            // Exit the scope of the function
            Table.PopScope ();

            // Add the function symbol
            Table.AddSymbol (Symbol.Create (funcdecl.Name, functionRef, funcdecl.Location));
        }
        */
    }
}

