using System;
using System.Collections.Generic;
using System.Linq;
using LexDotNet;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Scope.
    /// </summary>
    public class Scope {

        /// <summary>
        /// The symbols.
        /// </summary>
        readonly Stack<Symbol> Symbols;

        /// <summary>
        /// The captures.
        /// </summary>
        readonly List<string> Captures;

        /// <summary>
        /// The declarelocalfunctions.
        /// </summary>
        bool isfunction;

        /// <summary>
        /// Gets the declare local functions.
        /// </summary>
        /// <value>The declare local functions.</value>
        public bool IsFunction => isfunction;

        /// <summary>
        /// The function.
        /// </summary>
        public LLVMValueRef Function;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope"/> class.
        /// </summary>
        Scope () {
            Symbols = new Stack<Symbol> ();
            Captures = new List<string> ();
            Function = new LLVMValueRef (IntPtr.Zero);
        }

        /// <summary>
        /// Creates a new scope.
        /// </summary>
        /// <returns>The scope.</returns>
        public static Scope Create () => new Scope ();

        /// <summary>
        /// Creates a new scope.
        /// </summary>
        /// <returns>The function scope.</returns>
        public static Scope CreateFunction (LLVMValueRef func)
        => new Scope { isfunction = true, Function = func };

        public void Capture (string name) {
            if (Captures.Contains (name)) {
                throw LoreException.Create ()
                                   .Describe ($"Symbol '{name}' was captured multiple times.")
                                   .Resolve ($"Capture the symbol just once.");
            }
            Captures.Add (name);
        }

        /// <summary>
        /// Adds a symbol.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public void AddSymbol (string name, LLVMValueRef value, SourceLocation location) {
            Symbols.Push (Symbol.Create (name, value, location));
        }

        /// <summary>
        /// Adds a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="symbol">Symbol.</param>
        public void AddSymbol (Symbol symbol) {
            Symbols.Push (symbol);
        }

        /// <summary>
        /// Finds a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="name">Name.</param>
        public bool FindSymbol (string name) {
            return Symbols.Any (s => s.Name == name);
        }

        /// <summary>
        /// Finds a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="val">Value.</param>
        public bool FindSymbol (LLVMValueRef val) {
            return Symbols.Any (s => s.Value.Pointer == val.Pointer);
        }

        /// <summary>
        /// Finds a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="name">Name.</param>
        /// <param name="symbol">Symbol.</param>
        public bool FindSymbol (string name, out Symbol symbol) {
            symbol = null;
            if (FindSymbol (name)) {
                symbol = GetSymbol (name);
                return true;
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
            if (FindSymbol (value)) {
                symbol = GetSymbol (value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="name">Name.</param>
        public Symbol GetSymbol (string name) {
            return Symbols.First (s => s.Name == name);
        }

        /// <summary>
        /// Gets a symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="val">Value.</param>
        public Symbol GetSymbol (LLVMValueRef val) {
            return Symbols.First (s => s.Value.Pointer == val.Pointer);
        }
    }
}

