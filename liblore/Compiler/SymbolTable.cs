using System;
using System.Collections.Generic;
using LexDotNet;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Symbol table.
    /// </summary>
    public class SymbolTable {

        /// <summary>
        /// The global scope.
        /// </summary>
        readonly Scope GlobalScope;

        /// <summary>
        /// The scopes.
        /// </summary>
        readonly Stack<Scope> Scopes;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lore.SymbolTable"/> class.
        /// </summary>
        SymbolTable () {
            GlobalScope = Scope.Create ();
            Scopes = new Stack<Scope> ();
            Scopes.Push (GlobalScope);
        }

        /// <summary>
        /// Creates a new symbol table.
        /// </summary>
        public static SymbolTable Create () => new SymbolTable ();

        /// <summary>
        /// Gets the top scope.
        /// </summary>
        /// <value>The top scope.</value>
        public Scope TopScope => Scopes.Peek ();

        /// <summary>
        /// Pushs a scope.
        /// </summary>
        /// <returns>The scope.</returns>
        /// <param name="scope">Scope.</param>
        public void PushScope (Scope scope) {
            Scopes.Push (scope);
        }

        /// <summary>
        /// Pops a scope.
        /// </summary>
        /// <returns>The scope.</returns>
        public void PopScope () {
            if (Scopes.Count <= 1) {
                throw LoreException.Create ().Describe ("Attempt to leave global scope.").Describe ("This is a compiler bug.");
            }
            Scopes.Pop ();
        }

        /// <summary>
        /// Finds a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="name">Name.</param>
        /// <param name="symbol">Symbol.</param>
        public bool FindSymbol (string name, out Symbol symbol) {
            symbol = null;
            foreach (var scope in Scopes) {
                if (scope.FindSymbol (name)) {
                    symbol = scope.GetSymbol (name);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds a symbol by reference.
        /// </summary>
        /// <returns>The symbol by reference.</returns>
        /// <param name="value">Value.</param>
        /// <param name="symbol">Symbol.</param>
        public bool FindSymbolByRef (LLVMValueRef value, out Symbol symbol) {
            symbol = null;
            foreach (var scope in Scopes) {
                if (scope.FindSymbol (value)) {
                    symbol = scope.GetSymbol (value);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="name">Name.</param>
        public bool FindSymbol (string name) {
            Symbol _;
            return FindSymbol (name, out _);
        }

        /// <summary>
        /// Adds a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        public void AddSymbol (Symbol symbol) {
            Scopes.Peek ().AddSymbol (symbol);
        }

        /// <summary>
        /// Adds a global symbol.
        /// </summary>
        /// <returns>The global symbol.</returns>
        /// <param name="symbol">Symbol.</param>
        public void AddGlobalSymbol (Symbol symbol) {
            GlobalScope.AddSymbol (symbol);
        }
    }
}

