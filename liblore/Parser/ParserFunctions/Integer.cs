using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        IntegerExpression ParseInteger () {
            var expr = IntegerExpression.Create (unit.Location);
            var literal = unit.Expect (LoreToken.IntLiteral).Value;

            // Try parsing the string
            ulong intval;
            try {
                intval = ulong.Parse (literal, NumberStyles.Integer);
            } catch (Exception e) {
                throw new ParserException (unit, $"Failed to parse integer literal: ${e.Message}");
            }

            // Determine the size of the integer
            expr.SetSize (IntegerSize.ULong);
            if (intval <= (byte)sbyte.MaxValue) {
                expr.SetSize (IntegerSize.SByte);
            } else if (intval <= byte.MaxValue) {
                expr.SetSize (IntegerSize.UByte);
            } else if (intval <= (ushort)short.MaxValue) {
                expr.SetSize (IntegerSize.SShort);
            } else if (intval <= ushort.MaxValue) {
                expr.SetSize (IntegerSize.UShort);
            } else if (intval <= int.MaxValue) {
                expr.SetSize (IntegerSize.SWord);
            } else if (intval <= uint.MaxValue) {
                expr.SetSize (IntegerSize.UWord);
            } else if (intval <= long.MaxValue) {
                expr.SetSize (IntegerSize.SLong);
            }

            // Set the value and return
            expr.SetValue (intval);
            return expr;
        }
    }
}

