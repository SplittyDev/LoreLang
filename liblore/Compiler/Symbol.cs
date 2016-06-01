using System;
using LexDotNet;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Symbol.
    /// </summary>
    public class Symbol {

        /// <summary>
        /// The name.
        /// </summary>
        string name;

        /// <summary>
        /// The value.
        /// </summary>
        LLVMValueRef value;

        /// <summary>
        /// The location.
        /// </summary>
        public readonly SourceLocation Location;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => name;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public LLVMValueRef Value => value;

        /// <summary>
        /// The isptr.
        /// </summary>
        bool isptr;

        /// <summary>
        /// Gets the is pointer.
        /// </summary>
        /// <value>The is pointer.</value>
        public bool IsPointer => isptr;

        /// <summary>
        /// The isproto.
        /// </summary>
        bool isproto;

        /// <summary>
        /// Gets the is prototype.
        /// </summary>
        /// <value>The is prototype.</value>
        public bool IsPrototype => isproto;

        /// <summary>
        /// The ismutable.
        /// </summary>
        bool ismutable;

        /// <summary>
        /// Gets the is mutable.
        /// </summary>
        /// <value>The is mutable.</value>
        public bool IsMutable => ismutable;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lore.Symbol"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="location">Location.</param>
        Symbol (string name, LLVMValueRef value, SourceLocation location = null) {
            this.name = name;
            this.value = value;
            Location = location ?? SourceLocation.Zero;
        }

        /// <summary>
        /// Creates a new symbol.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="location">Location.</param>
        public static Symbol Create (string name, LLVMValueRef value, SourceLocation location, bool mutable = false)
        => new Symbol (name, value, location) { ismutable = mutable };

        /// <summary>
        /// Creates a new symbol.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="location">Location.</param>
        public static Symbol CreateMutable (string name, LLVMValueRef value, SourceLocation location, bool mutable = false)
        => Create (name, value, location, true);

        /// <summary>
        /// Creates a new symbol.
        /// </summary>
        /// <returns>The ptr.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="location">Location.</param>
        public static Symbol CreatePtr (string name, LLVMValueRef value, SourceLocation location, bool mutable = false)
        => new Symbol (name, value, location) { isptr = true, ismutable = mutable };

        /// <summary>
        /// Creates a new symbol.
        /// </summary>
        /// <returns>The ptr.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="location">Location.</param>
        public static Symbol CreateMutablePtr (string name, LLVMValueRef value, SourceLocation location)
        => CreatePtr (name, value, location, true);

        /// <summary>
        /// Creates a new symbol.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="location">Location.</param>
        public static Symbol CreatePrototype (string name, LLVMValueRef value, SourceLocation location)
        => new Symbol (name, value, location) { isproto = true };
    }
}

