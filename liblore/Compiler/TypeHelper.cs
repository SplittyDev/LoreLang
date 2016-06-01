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

        public LLVMTypeRef GetBuiltinTypeFromString (string type) {
            switch (type) {
            case "void":
                return LLVM.VoidType ();
            case "bool":
                return LLVM.IntType (1);
            case "int":
                return LLVM.IntType (32);
            case "uint":
                return LLVM.IntType (34);
            case "int8":
                return LLVM.IntType (8);
            case "uint8":
                return LLVM.IntType (10);
            case "int16":
                return LLVM.IntType (16);
            case "uint16":
                return LLVM.IntType (18);
            case "int32":
                return LLVM.IntType (32);
            case "uint32":
                return LLVM.IntType (34);
            case "int64":
                return LLVM.IntType (64);
            case "uint64":
                return LLVM.IntType (66);
            case "float":
                return LLVM.FloatType ();
            case "double":
                return LLVM.DoubleType ();
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

