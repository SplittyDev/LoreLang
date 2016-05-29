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
        public static LoreLexer GrabLexer (string source) => new LoreLexer (SourceUnit.FromSource (source));
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
            const string source = "1337 0x1337";
            LexemeCollection<LoreToken> actual = null;
            Assert.DoesNotThrow (() => actual = Support.GrabLexer (source).Tokenize ());
            Assert.That (actual?.All (tk => tk.Token == LoreToken.IntLiteral) ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "1337") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "4919") ?? false);
        }

        [Test]
        public void TestInvalidIntegers () {
            var source = "0x1GG";
            LexemeCollection<LoreToken> actual = null;
            Assert.Throws<LexerException> (() => actual = Support.GrabLexer (source).Tokenize ());
        }

        [Test]
        public void TestFloats () {
            const string source = ".10 13.37 73.31 1.";
            LexemeCollection<LoreToken> actual = null;
            Assert.DoesNotThrow (() => actual = Support.GrabLexer (source).Tokenize ());
            Assert.That (actual?.All (tk => tk.Token == LoreToken.FloatLiteral) ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "13.37") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "73.31") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "0.10") ?? false);
            Assert.That (actual?.Any (tk => tk.Value == "1.0") ?? false);
        }

        [Test]
        public void TestInvalidFloats () {
            const string source = "0. 1. . .1";
            LexemeCollection<LoreToken> actual = null;
            Assert.Throws<LexerException> (() => actual = Support.GrabLexer (source).Tokenize ());
        }
    }
}

