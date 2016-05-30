using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        string GetPadding (int depth) => string.Empty.PadLeft (depth * 2);

        void Visualize (AstNode root, int depth = 0, bool suppressNewline = false) {
            if (depth > 0 && !suppressNewline) {
                Console.WriteLine ();
            }
            Console.Write ($"{GetPadding (depth)}");
            if (root is AstRoot) {
                Console.Write ("* Root");
                foreach (var child in ((AstRoot)root).Children)
                    Visualize (child, depth + 1);
            } else if (root is CodeBlock) {
                foreach (var child in ((CodeBlock)root).Children)
                    Visualize (child, depth);
            } else if (root is FunctionDeclaration) {
                Console.Write ($"* {root}");
                Visualize (((FunctionDeclaration)root).Body, depth, true);
            } else if (root is BinaryExpression) {
                Console.Write ($"* Binary expression");
                Visualize (((BinaryExpression)root).Left, depth + 1);
                Visualize (((BinaryExpression)root).Operation, depth + 1);
                Visualize (((BinaryExpression)root).Right, depth + 1);
            } else if (root is UnaryExpression) {
                Console.Write ($"* Unary expression");
                Visualize (((UnaryExpression)root).Operation, depth + 1);
                Visualize (((UnaryExpression)root).Child, depth + 1);
            } else if (root is NameExpression) {
                Console.Write ($"* Variable: {((NameExpression)root).Name}");
            } else if (root is CallExpression) {
                Console.Write ($"* Call");
                Visualize (((CallExpression)root).Arguments, depth + 1);
                Visualize (((CallExpression)root).Target, depth + 1);
            } else if (root is LambdaExpression) {
                Console.Write ($"* {root}");
                Visualize (((LambdaExpression)root).Body, depth + 1, true);
            } else if (root is IntegerExpression) {
                Console.Write ($"* {root}");
            } else {
                Console.Write ($"* {root.GetType ().Name}");
            }
            if (depth == 0 && !suppressNewline) {
                Console.WriteLine ();
            }
        }

        void Visualize (BinaryOperation expr, int depth = 0) {
            if (depth > 0) {
                Console.WriteLine ();
            }
            Console.Write ($"{GetPadding (depth)}");
            Console.Write ($"* Binary operation: {expr}");
        }

        void Visualize (UnaryOperation expr, int depth = 0) {
            if (depth > 0) {
                Console.WriteLine ();
            }
            Console.Write ($"{GetPadding (depth)}");
            Console.Write ($"* Unary operation: {expr}");
        }
    }
}

