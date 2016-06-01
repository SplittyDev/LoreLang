using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        /*
         *  { code... }
         * or
         *  [captures...] { code... }
         */
        CodeBlock ParseBlock () {
            if (unit.Match (LoreToken.OpenBracket)) {
                return ParseBlockWithCaptures ();
            }
            return ParseBlockWithoutCaptures ();
        }

        /*  
         *  { code... }
         */
        CodeBlock ParseBlockWithoutCaptures () {
            var code = CodeBlock.Create (unit.Location);
            unit.Expect (LoreToken.OpenBrace);
            while (!unit.Match (LoreToken.CloseBrace)) {
                code.AddChild (ParseStatement ());
            }
            unit.Expect (LoreToken.CloseBrace);
            return code;
        }

        /*
         *  [captures...] { code... }
         */
        CodeBlock ParseBlockWithCaptures () {
            var capturedBlock = CodeBlock.Create (unit.Location);

            // Parse the captures
            unit.Expect (LoreToken.OpenBracket);
            while (!unit.Match (LoreToken.CloseBracket)) {
                var lex = unit.Read ();
                var capture = Capture.Create (unit, lex);
                capturedBlock.AddCapture (capture);
            }
            unit.Expect (LoreToken.CloseBracket);

            // Parse the actual block
            var pureBlock = ParseBlockWithoutCaptures ();

            // Merge the block with the captures
            capturedBlock.Merge (pureBlock);
            return capturedBlock;
        }
    }
}

