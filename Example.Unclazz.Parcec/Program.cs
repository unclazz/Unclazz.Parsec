using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Example.Unclazz.Parcec
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new ExpressionParser();
            var input = args.Aggregate(new StringBuilder(),
                (b, a) => b.Append(a).Append(' ')).ToString();

            parser.Parse(input)
                .IfSuccessful(c => Console.WriteLine("result = {0}", c.Value), 
                m => Console.WriteLine("error = {0}", m));
        }
    }

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

    sealed class NumberParser : Parser<double>
    {
        readonly Parser sign;
        readonly Parser digits;
        readonly Parser integral;
        readonly Parser fractional;
        readonly Parser exponent;
        readonly Parser<double> number;

        public NumberParser()
        {
            sign = CharIn("+-").OrNot();
            digits = CharsWhileIn("0123456789", min: 0);
            integral = Char('0') | (CharBetween('1', '9') & digits);
            fractional = Char('.') & digits;
            exponent = CharIn("eE") & (sign) & (digits);
            number = ((integral & fractional.OrNot() &
                exponent.OrNot()).Capture()).Map(double.Parse);
        }

        protected override ParseResult<double> DoParse(Reader input)
        {
            return number.Parse(input);
        }
    }
}
