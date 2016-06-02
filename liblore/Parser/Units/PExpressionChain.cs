using System;
using System.Collections.Generic;

namespace Lore {

    /// <summary>
    /// Lore parser.
    /// </summary>
    public partial class LoreParser {

        /*
         * Expression call chain:
         * ParseExpression
         * - ParseAssignment
         * -- ParseLogicalOr
         * --- ParseLogicalAnd
         * ---- ParseBitwiseOr
         * ----- ParseBitwiseXor
         * ------ ParseBitwiseAnd
         * ------- ParseRelationalOp
         * -------- ParseBitshift
         * --------- ParseAdditive
         * ---------- ParseMultiplicative
         * ----------- ParseUnary
         * ------------ ParseCall
         * ------------- ParseTerm
         */
        AstNode ParseExpression () {
            var ass = ParseAssignment ();
            while (unit.Match (LoreToken.Semicolon)) {
                unit.Skip ();
            }
            return ass;
        }

        AstNode ParseAssignment () {
            AstNode expr = ParseLogicalOr ();
            while (unit.Match (LoreToken.Operator)) {
                switch (unit.Current.Value) {
                case "=>":
                    expr = ParseLambda ();
                    continue;
                case "=":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.Assign,
                        left: expr,
                        right: ParseLogicalOr ()
                    );
                    continue;
                }
                break;
            }
            return expr;
        }

        AstNode ParseLogicalOr () {
            AstNode expr = ParseLogicalAnd ();
            while (unit.Accept (LoreToken.Operator, "||")) {
                expr = BinaryExpression.Create (
                    location: unit.Location,
                    op: BinaryOperation.LogicalOr,
                    left: expr,
                    right: ParseLogicalAnd ()
                );
            }
            return expr;
        }

        AstNode ParseLogicalAnd () {
            AstNode expr = ParseBitwiseOr ();
            while (unit.Accept (LoreToken.Operator, "&&")) {
                expr = BinaryExpression.Create (
                    location: unit.Location,
                    op: BinaryOperation.LogicalAnd,
                    left: expr,
                    right: ParseBitwiseOr ()
                );
            }
            return expr;
        }

        AstNode ParseBitwiseOr () {
            AstNode expr = ParseBitwiseXor ();
            while (unit.Accept (LoreToken.Operator, "|")) {
                expr = BinaryExpression.Create (
                    location: unit.Location,
                    op: BinaryOperation.BitwiseOr,
                    left: expr,
                    right: ParseBitwiseXor ()
                );
            }
            return expr;
        }

        AstNode ParseBitwiseXor () {
            AstNode expr = ParseBitwiseAnd ();
            while (unit.Accept (LoreToken.Operator, "^")) {
                expr = BinaryExpression.Create (
                    location: unit.Location,
                    op: BinaryOperation.BitwiseXor,
                    left: expr,
                    right: ParseBitwiseAnd ()
                );
            }
            return expr;
        }

        AstNode ParseBitwiseAnd () {
            AstNode expr = ParseEquals ();
            while (unit.Accept (LoreToken.Operator, "&")) {
                expr = BinaryExpression.Create (
                    location: unit.Location,
                    op: BinaryOperation.BitwiseAnd,
                    left: expr,
                    right: ParseEquals ()
                );
            }
            return expr;
        }

        AstNode ParseEquals () {
            AstNode expr = ParseRelationalOp ();
            while (unit.Match (LoreToken.Operator)) {
                switch (unit.Current.Value) {
                case "==":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.Equals,
                        left: expr,
                        right: ParseRelationalOp ()
                    );
                    continue;
                case "!=":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.NotEquals,
                        left: expr,
                        right: ParseRelationalOp ()
                    );
                    continue;
                }
                break;
            }
            return expr;
        }

        AstNode ParseRelationalOp () {
            AstNode expr = ParseBitshift ();
            while (unit.Match (LoreToken.Operator)) {
                switch (unit.Current.Value) {
                case ">":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.GreaterThan,
                        left: expr,
                        right: ParseBitshift ()
                    );
                    continue;
                case ">=":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.GreaterThanOrEqual,
                        left: expr,
                        right: ParseBitshift ()
                    );
                    continue;
                case "<":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.LessThan,
                        left: expr,
                        right: ParseBitshift ()
                    );
                    continue;
                case "<=":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.LessThanOrEqual,
                        left: expr,
                        right: ParseBitshift ()
                    );
                    continue;
                }
                break;
            }
            return expr;
        }

        AstNode ParseBitshift () {
            AstNode expr = ParseAdditive ();
            while (unit.Match (LoreToken.Operator)) {
                switch (unit.Current.Value) {
                case "<<":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.ShiftLeft,
                        left: expr,
                        right: ParseAdditive ()
                    );
                    continue;
                case ">>":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.ShiftRight,
                        left: expr,
                        right: ParseAdditive ()
                    );
                    continue;
                }
                break;
            }
            return expr;
        }

        AstNode ParseAdditive () {
            AstNode expr = ParseMultiplicative ();
            while (unit.Match (LoreToken.Operator)) {
                switch (unit.Current.Value) {
                case "+":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.Add,
                        left: expr,
                        right: ParseMultiplicative ()
                    );
                    continue;
                case "-":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.Subtract,
                        left: expr,
                        right: ParseMultiplicative ()
                    );
                    continue;
                }
                break;
            }
            return expr;
        }

        AstNode ParseMultiplicative () {
            AstNode expr = ParseUnary ();
            while (unit.Match (LoreToken.Operator)) {
                switch (unit.Current.Value) {
                case "*":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.Multiply,
                        left: expr,
                        right: ParseUnary ()
                    );
                    continue;
                case "/":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.Divide,
                        left: expr,
                        right: ParseUnary ()
                    );
                    continue;
                case "%":
                    unit.Skip ();
                    expr = BinaryExpression.Create (
                        location: unit.Location,
                        op: BinaryOperation.Modulo,
                        left: expr,
                        right: ParseUnary ()
                    );
                    continue;
                }
                break;
            }
            return expr;
        }

        AstNode ParseUnary () {
            while (unit.Match (LoreToken.Operator)) {
                switch (unit.Current.Value) {
                case "-":
                    unit.Skip ();
                    return UnaryExpression.Create (
                        location: unit.Location,
                        op: UnaryOperation.Negate,
                        child: ParseUnary ()
                    );
                case "~":
                    unit.Skip ();
                    return UnaryExpression.Create (
                        location: unit.Location,
                        op: UnaryOperation.BitwiseNot,
                        child: ParseUnary ()
                    );
                case "!":
                    unit.Skip ();
                    return UnaryExpression.Create (
                        location: unit.Location,
                        op: UnaryOperation.LogicalNot,
                        child: ParseUnary ()
                    );
                }
            }
            return ParseCallOrAccess ();
        }
    }
}

