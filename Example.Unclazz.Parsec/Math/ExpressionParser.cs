using System;
using System.Collections.Generic;
using System.Linq;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Example.Unclazz.Parsec.Math
{
    sealed class ExpressionParser : Parser<double>
    {
        readonly Parser<string> addSub;
        readonly Parser<string> mulDiv;
        readonly Parser parenLeft;
        readonly Parser parenRight;
        readonly Parser<double> number;

        public ExpressionParser()
        {
            Configure(a =>
            {
                a.SetNonSignificant(WhileSpaceAndControls);
            });
            addSub = CharIn("+-").Capture();
            mulDiv = CharIn("*/").Capture();
            parenLeft = Char('(');
            parenRight = Char(')');
            number = new NumberParser();
        }

        protected override ParseResult<double> DoParse(Reader input)
        {
            return (Expr() & EndOfFile).Parse(input);
        }
        Parser<double> Expr()
        {
            var term = Lazy<double>(Term);
            var opRights = (addSub.Cut().Then(term)).Repeat();
            return (term.Then(opRights)).Map(Eval);
        }
        Parser<double> Term()
        {
            var factor = Lazy<double>(Factor);
            var opRights = (mulDiv.Cut().Then(factor)).Repeat();
            return (factor.Then(opRights)).Map(Eval);
        }
        Parser<double> Factor()
        {
            var expr = Lazy<double>(Expr);
            return number | (parenLeft.Cut() & expr & parenRight);
        }

        double Eval(Tuple<double, IList<Tuple<string, double>>> tree)
        {
            var leftSeed = tree.Item1;
            var opRights = tree.Item2;
            return opRights.Aggregate(leftSeed, Eval_Accumulator);
        }

        double Eval_Accumulator(double left, Tuple<string, double> opRight)
        {
            if (opRight.Item1 == "+")
            {
                return left + opRight.Item2;
            }
            else if (opRight.Item1 == "-")
            {
                return left - opRight.Item2;
            }
            else if (opRight.Item1 == "*")
            {
                return left * opRight.Item2;
            }
            else if (opRight.Item1 == "/")
            {
                return left / opRight.Item2;
            }
            throw new Exception("unknown operator.");
        }
    }
}
