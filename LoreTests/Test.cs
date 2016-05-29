using System;
using System.Linq;
using LexDotNet;
using Lore;
using NUnit.Framework;

namespace LoreTests {

    /// <summary>
    /// Support.
    /// </summary>
    static class Support {

        /// <summary>
        /// Grabs a new lexer for the specified source.
        /// </summary>
        /// <returns>The lexer.</returns>
        /// <param name="source">Source.</param>
        public static LoreLexer GrabLexer (string source) => LoreLexer.Create (SourceUnit.FromSource (source));

        /// <summary>
        /// Grabs a new parser for the specified source.
        /// </summary>
        /// <returns>The parser.</returns>
        /// <param name="source">Source.</param>
        public static LoreParser GrabParser (string source) {
            var lexer = GrabLexer (source);
            var lexemes = lexer.Tokenize ();
            var unit = ParsingUnit.Create (lexemes);
            var parser = LoreParser.Create (unit);
            return parser;
        }
    }
    
    [TestFixture]
    public class LexerTest {
        
        [Test]
        public void TestIdentifiers () {
            const string source = "these are four identifiers";
            LexemeCollection<LoreToken> actual = null;
            Assert.DoesNotThrow (() => actual = Support.GrabLexer (source).Tokenize ());
            Assert.That (actual?.All (tk => tk.Token == LoreToken.Identifier) ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "these") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "are") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "four") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "identifiers") ?? false);
        }

        [Test]
        public void TestIntegers () {
            const string source = "1337 0x1337 25";
            LexemeCollection<LoreToken> actual = null;
            Assert.DoesNotThrow (() => actual = Support.GrabLexer (source).Tokenize ());
            Assert.That (actual?.All (tk => tk.Token == LoreToken.IntLiteral) ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "1337") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "4919") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "25") ?? false);
        }

        [Test]
        public void TestInvalidIntegers () {
            var source = "0x1GG";
            LexemeCollection<LoreToken> actual = null;
            Assert.DoesNotThrow (() => actual = Support.GrabLexer (source).Tokenize ());
        }

        [Test]
        public void TestFloats () {
            var source = ".10 13.37 73.31 1.";
            LexemeCollection<LoreToken> actual = null;
            Assert.DoesNotThrow (() => actual = Support.GrabLexer (source).Tokenize ());
            Assert.That (actual?.All (tk => tk.Token == LoreToken.FloatLiteral) ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "13.37") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "73.31") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "0.10") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "1.0") ?? false);
            source = "0. 1..something";
            Assert.DoesNotThrow (() => actual = Support.GrabLexer (source).Tokenize ());
        }

        [Test]
        public void TestOperators () {
            var operators = new [] { ".", "=", "==", "!=", "<=", ">=", "^=" };
            LexemeCollection<LoreToken> actual = null;
            foreach (var op in operators) {
                var source = $"a {op} b";
                try {
                    actual = Support.GrabLexer (source).Tokenize ();
                } catch (LexerException e) {
                    Assert.Fail (e.Message);
                }
                Assert.That (actual?.ElementAt (0).Token == LoreToken.Identifier);
                Assert.That (actual?.ElementAt (1).Token == LoreToken.Operator);
                Assert.That (actual?.ElementAt (1).Value == op);
                Assert.That (actual?.ElementAt (2).Token == LoreToken.Identifier);
            }
        }

        [Test]
        public void TestRealProgram () {
            const string source = @"
            // Lore master race
            fn print (msg) {
                stdout.writeln (msg);
            }
            fn main {
                a = 2;
                let _print ==> [a] print (a)
                _print ()
            }";
            LexemeCollection<LoreToken> actual = null;
            try {
                actual = Support.GrabLexer (source).Tokenize ();
            } catch (LexerException e) {
                Assert.Fail (e.Message);
            }
            Assert.DoesNotThrow (() => actual = Support.GrabLexer (source).Tokenize ());
        }
    }

    [TestFixture]
    public class ParserTest {
        
        [Test]
        public void TestParser () {
            const string source = @"
            fn hello (arg1) {
            }
            ";
            var parser = Support.GrabParser (source);
            var ast = parser.Parse ();
        }
    }
}

