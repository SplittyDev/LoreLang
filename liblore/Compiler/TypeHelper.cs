using System;
using LexDotNet;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Type helper.
    /// </summary>
    public class TypeHelper {

        /// <summary>
        /// The visitor.
        /// </summary>
        readonly AstVisitor Visitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lore.TypeRegistry"/> class.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        TypeHelper (AstVisitor visitor) {
            Visitor = visitor;
        }

        /// <summary>
        /// Creates a new type registry.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public static TypeHelper Create (AstVisitor visitor) => new TypeHelper (visitor);

        public bool IsInteger (LLVMValueRef elem)
        => elem.TypeOf ().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind;

        public bool IsFloat (LLVMValueRef elem)
        => elem.TypeOf ().TypeKind == LLVMTypeKind.LLVMFloatTypeKind;

        public bool IsStruct (LLVMValueRef elem)
        => elem.TypeOf ().TypeKind == LLVMTypeKind.LLVMStructTypeKind;

        public bool IsBoolean (LLVMValueRef elem)
        => CompareType (elem, LLVM.Int1Type ());

        public bool IsDouble (LLVMValueRef elem)
        => elem.TypeOf ().TypeKind == LLVMTypeKind.LLVMDoubleTypeKind;

        public bool IsFloatOrDouble (LLVMValueRef elem)
        => IsFloat (elem) || IsDouble (elem);

        public bool CompareType (LLVMTypeRef type1, LLVMTypeRef type2)
        => type1.Pointer == type2.Pointer;

        public bool CompareType (LLVMValueRef elem, LLVMTypeRef type)
        => CompareType (elem.TypeOf (), type);

        public bool CompareType (LLVMValueRef elem1, LLVMValueRef elem2)
        => CompareType (elem1.TypeOf (), elem2.TypeOf ());

        public readonly static LLVMTypeRef VoidType = LLVM.VoidType ();
        public readonly static LLVMTypeRef BoolType = LLVM.IntType (1);
        public readonly static LLVMTypeRef Int8Type = LLVM.IntType (8);
        public readonly static LLVMTypeRef UInt8Type = LLVM.IntType (10);
        public readonly static LLVMTypeRef Int16Type = LLVM.IntType (16);
        public readonly static LLVMTypeRef UInt16Type = LLVM.IntType (18);
        public readonly static LLVMTypeRef Int32Type = LLVM.IntType (32);
        public readonly static LLVMTypeRef UInt32Type = LLVM.IntType (34);
        public readonly static LLVMTypeRef Int64Type = LLVM.IntType (64);
        public readonly static LLVMTypeRef UInt64Type = LLVM.IntType (66);
        public readonly static LLVMTypeRef FloatType = LLVM.FloatType ();
        public readonly static LLVMTypeRef DoubleType = LLVM.DoubleType ();
        public readonly static LLVMTypeRef StringType = LLVM.PointerType (Int8Type, 0);

        public LLVMTypeRef GetBuiltinTypeFromString (string type) {
            switch (type) {
            case "void":
                return VoidType;
            case "bool":
                return BoolType;
            case "char":
            case "int8":
                return Int8Type;
            case "uint8":
                return UInt8Type;
            case "int16":
                return Int16Type;
            case "uint16":
                return UInt16Type;
            case "int":
            case "int32":
                return Int32Type;
            case "uint":
            case "uint32":
                return UInt32Type;
            case "int64":
                return Int64Type;
            case "uint64":
                return UInt64Type;
            case "float":
                return FloatType;
            case "double":
                return DoubleType;
            case "string":
                return StringType;
            }
            throw LoreException.Create (Visitor.Location).Describe ($"Unable to resolve type: '{type}'");
        }

        public void BuildOptimalReturn (LLVMBuilderRef builder, LLVMValueRef elem, LLVMTypeRef returnType) {

            // Check if the return type is void
            if (CompareType (returnType, LLVM.VoidType ())) {

                // The return type is void
                // Emit an unreachable instruction
                LLVM.BuildRetVoid (builder);
                return;
            }

            // Try to perform a cast to the target type
            elem = BuildCast (builder, elem, returnType);

            // Build the return statement
            LLVM.BuildRet (builder, elem);
        }

        public LLVMValueRef BuildCast (LLVMBuilderRef builder, LLVMValueRef elem, LLVMTypeRef targetType) {

            // Check if the type of the element equals the targe type
            if (CompareType (elem, targetType)) {

                // There is no need to cast
                // Just return the element
                return elem;
            }

            // Strings need special handling
            if (CompareType (targetType, StringType)) {
                LLVMValueRef indices;
                var gep = LLVM.BuildGEP (builder, elem, out indices, 0u, "tmpgep");
                var ptr = LLVM.BuildPointerCast (builder, gep, StringType, "tmpptrcast");
                return ptr;
            }

            string strtype1, strtype2;
            switch (targetType.TypeKind) {
            case LLVMTypeKind.LLVMIntegerTypeKind:

                // Check if the integer fits
                if (IsInteger (elem)) {
                    var lwidth = elem.TypeOf ().GetIntTypeWidth ();
                    var rwidth = targetType.GetIntTypeWidth ();
                    if (lwidth > rwidth) {

                        // The width of the result type is higher
                        // than the width of the element type
                        // Throw an exception
                        strtype1 = elem.TypeOf ().PrintTypeToString ();
                        strtype2 = targetType.PrintTypeToString ();
                        throw LoreException.Create (Visitor.Location)
                                           .Describe ($"Unable to cast element of type '{strtype1}' to '{strtype2}':")
                                           .Describe ($"The element is too big for the target integer type.");
                    }
                }

                // Perform a float to signed integer cast if needed
                else if (IsFloatOrDouble (elem)) {
                    elem = LLVM.BuildFPToSI (builder, elem, targetType, "tmpfptosicast");
                }

                // Unsupported element type
                else {
                    
                    // Unable to perform a meaningful cast
                    strtype1 = elem.TypeOf ().PrintTypeToString ();
                    strtype2 = targetType.PrintTypeToString ();
                    throw LoreException.Create (Visitor.Location).Describe ($"Unable to cast element of type '{strtype1}' to '{strtype2}':");
                }

                // Perform an integer cast
                return LLVM.BuildIntCast (builder, elem, targetType, "tmpcast");
            case LLVMTypeKind.LLVMFloatTypeKind:
            case LLVMTypeKind.LLVMDoubleTypeKind:

                // Perform a signed integer to float cast if needed
                if (IsInteger (elem)) {
                    elem = LLVM.BuildSIToFP (builder, elem, targetType, "tmpsitofpcast");
                }
                return LLVM.BuildFPCast (builder, elem, targetType, "tmpfcast");
            }

            // Unable to perform a meaningful cast
            strtype1 = elem.TypeOf ().PrintTypeToString ();
            strtype2 = targetType.PrintTypeToString ();
            throw LoreException.Create (Visitor.Location).Describe ($"Unable to cast element of type '{strtype1}' to '{strtype2}':");
        }

        public bool TryBuildCast (LLVMBuilderRef builder, LLVMValueRef elem, LLVMTypeRef targetType, out LLVMValueRef result) {
            result = new LLVMValueRef (IntPtr.Zero);
            try {
                result = BuildCast (builder, elem, targetType);
            } catch (LoreException) {
                return false;
            }
            return true;
        }
    }
}

