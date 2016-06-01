using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        FloatExpression ParseFloat () {
            var expr = FloatExpression.Create (unit.Location);
            var literal = unit.Expect (LoreToken.FloatLiteral).Value;

            // Try parsing the string
            double floatval;
            try {
                floatval = double.Parse (literal, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
            } catch (Exception e) {
                throw LoreException.Create (unit.Location).Describe ($"Failed to parse float literal: ${e.Message}");
            }

            // Set the value and return
            expr.SetValue (floatval);
            return expr;
        }
    }
}

