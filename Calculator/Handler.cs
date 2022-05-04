using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    /// <summary>
    /// Evaluates an equation
    /// </summary>
    class Handler
    {
        public Handler()
        {}

        public double Evaluate(Token[] seq) //evaluates a tokenized array
        {
            seq = BracketHandler(seq);
            seq = FunctionHandler(seq);
            Token result = ArithmeticHandler(seq)[0];
            return result.nValue;
        }

        #region Handlers

        Token[] BracketHandler(Token[] seq) //deals with parentheses
        {
            int from = -1;
            int count = 0;
            for (int i = 0; i < seq.Length; i++)
            {
                if (seq[i].oValue == "(")
                {
                    if (from == -1) from = i;
                    count++;
                }
                else if (seq[i].oValue == ")")
                {
                    count--;
                }
            }
            if (count != 0) throw new Exception("ERROR Brackets- not paired");
            if (from == -1) return seq;

            int index = from + 1;
            count = 1;
            while (count != 0 && index < seq.Length)
            {
                if (seq[index].oValue == "(")
                {
                    count++;
                }
                else if (seq[index].oValue == ")")
                {
                    count--;
                }
                index++;
            }
            int to = index - 1;

            Token[] slice = Slice(seq, from + 1, to - 1);
            seq = Remove(seq, from, to, Evaluate(slice));
            seq = BracketHandler(seq);
            return seq;
        }

        Token[] ArithmeticHandler(Token[] seq) //deals with simple expressions
        {
            Exception e = new Exception("ERROR: Arithmetic- not computable");
            //order 1
            for (int i = 0; i < seq.Length; i++)
            {
                Token o = seq[i];
                switch (o.oValue)
                {
                    case "^":
                        if (seq[i - 1].oValue != null || seq[i + 1].oValue != null) throw e;
                        double pow = Math.Pow(seq[i - 1].nValue, seq[i + 1].nValue);
                        seq = Remove(seq, i - 1, i + 1, pow);
                        i = -1;
                        break;
                    case "!":
                        if (seq[i - 1].oValue != null) throw e;
                        double n = seq[i - 1].nValue;
                        if (n < 1) throw new Exception("ERROR: Arithmetic- factorial less than 1");
                        if (n != Math.Floor(n)) throw new Exception("ERROR: Arithmetic- factorial decimal");
                        double fac = 1;
                        for (int j = 1; j < (int)n + 1; j++)
                        {
                            fac = j * fac;
                        }
                        seq = Remove(seq, i - 1, i, fac);
                        i = -1;
                        break;
                    default:
                        break;
                }
            }

            //order 2
            for (int i = 0; i < seq.Length; i++)
            {
                Token o = seq[i];
                switch (o.oValue)
                {
                    case "*":
                        if (seq[i - 1].oValue != null || seq[i + 1].oValue != null) throw e;
                        double mult = seq[i - 1].nValue * seq[i + 1].nValue;
                        seq = Remove(seq, i - 1, i + 1, mult);
                        i = -1;
                        break;
                    case "/":
                        if (seq[i - 1].oValue != null || seq[i + 1].oValue != null) throw e;
                        if (seq[i + 1].nValue == 0) throw new Exception("ERROR: Arithmetic- division by 0");
                        double div = seq[i - 1].nValue / seq[i + 1].nValue;
                        seq = Remove(seq, i - 1, i + 1, div);
                        i = -1;
                        break;
                    default:
                        break;
                }
            }

            //order 3
            for (int i = 0; i < seq.Length; i++)
            {
                Token o = seq[i];
                switch (o.oValue)
                {
                    case "+":
                        if (seq[i - 1].oValue != null || seq[i + 1].oValue != null) throw e;
                        double add = seq[i - 1].nValue + seq[i + 1].nValue;
                        seq = Remove(seq, i - 1, i + 1, add);
                        i = -1;
                        break;
                    case "-":
                        if (seq[i - 1].oValue != null || seq[i + 1].oValue != null) throw e;
                        double sub = seq[i - 1].nValue - seq[i + 1].nValue;
                        seq = Remove(seq, i - 1, i + 1, sub);
                        i = -1;
                        break;
                    default:
                        break;
                }
            }

            return seq;
        }

        Token[] FunctionHandler(Token[] seq) //deals with functions
        {
            for (int i = 0; i < seq.Length; i++)
            {
                Token f = seq[i];
                if (f.isFunc)
                {
                    if (seq[i + 1].oValue != null) throw new Exception("ERROR: Function- not computable");
                    switch (f.oValue) 
                    {
                        case "abs":
                            double abs = Math.Abs(seq[i + 1].nValue);
                            seq = Remove(seq, i, i + 1, abs);
                            break;
                        case "cos":
                            double cos = Math.Cos(seq[i + 1].nValue);
                            seq = Remove(seq, i, i + 1, cos);
                            break;
                        case "sin":
                            double sin = Math.Sin(seq[i + 1].nValue);
                            seq = Remove(seq, i, i + 1, sin);
                            break;
                        case "tan":
                            double tan = Math.Tan(seq[i + 1].nValue);
                            seq = Remove(seq, i, i + 1, tan);
                            break;
                        case "log":
                            double log = Math.Log(seq[i + 1].nValue);
                            seq = Remove(seq, i, i + 1, log);
                            break;
                        case "sqr":
                            double sqr = Math.Sqrt(seq[i + 1].nValue);
                            seq = Remove(seq, i, i + 1, sqr);
                            break;
                        case "clg":
                            double clg = Math.Ceiling(seq[i + 1].nValue);
                            seq = Remove(seq, i, i + 1, clg);
                            break;
                        case "flr":
                            double flr = Math.Floor(seq[i + 1].nValue);
                            seq = Remove(seq, i, i + 1, flr);
                            break;
                        default:
                            break;
                    }
                }
            }
            return seq;
        }

        #endregion

        #region Seq changers
        public Token[] Slice(Token[] seq, int from, int to) //returns a given section of a tokenized array
        {
            List<Token> seqList = new List<Token>();

            for (int i = from; i < to + 1; i++)
            {
                seqList.Add(seq[i]);
            }

            return seqList.ToArray();
        }

        public Token[] Remove(Token[] seq, int from, int to, double instead) //changes a section of a tokenized array into a value
        {
            List<Token> seqList = new List<Token>();

            for (int i = 0; i < from; i++)
            {
                seqList.Add(seq[i]);
            }
            seqList.Add(new Token(instead));
            for (int i = to + 1; i < seq.Length; i++)
            {
                seqList.Add(seq[i]);
            }

            return seqList.ToArray();
        }

        #endregion 
    }
}
