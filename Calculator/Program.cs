using System;
using System.Collections.Generic;

namespace Calculator
{
    /// <summary>
    /// Holds numbers, operations, brackets, and functions
    /// </summary>
    class Token
    {
        public double nValue { get; } //number value
        public string oValue { get; } //operation value
        public bool isFunc { get; }
        public Token(double nValue)
        {
            this.nValue = nValue;
            oValue = null;
            isFunc = false;
        }

        public Token(string oValue, bool isFunc = false)
        {
            nValue = 0;
            this.oValue = oValue;
            this.isFunc = isFunc;
        }
    }

    class Program
    {
        static Token[] Tokenize(string line) //converts a string into a tokenized array
        {
            List<Token> seq = new List<Token>();
            List<bool> isNum = new List<bool>();
            string num = "";
            line = RemoveSpaces(line);
            Exception e = new Exception("ERROR: Tokenizer- unknown character");

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (IsType(c, new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.' }))
                {
                    if (c == '.') c = ',';
                    num += c;
                }
                else if (IsType(c, new char[] { '+', '-', '*', '/', '^', '!', '(', ')' }))
                {
                    if (c == '-' && seq[seq.Count - 1].oValue == "(" && num == "")
                    {
                        seq.Add(new Token(-1));
                        seq.Add(new Token("*"));
                    }
                    else
                    {
                        if (num != "")
                        {
                            seq.Add(new Token(Convert.ToDouble(num)));
                        }
                        num = "";
                        seq.Add(new Token(c.ToString()));
                    }
                }
                else
                {
                    if (c == 'p')
                    {
                        seq.Add(new Token(Math.PI));
                    }
                    else if (c == 'e')
                    {
                        seq.Add(new Token(Math.E));
                    }
                    else
                    {
                        if (i > line.Length - 3) throw e;
                        string fun = c.ToString() + line[i + 1].ToString() + line[i + 2].ToString();

                        switch (fun)
                        {
                            case "abs":
                                seq.Add(new Token("abs", true));
                                i += 2;
                                break;
                            case "cos":
                                seq.Add(new Token("cos", true));
                                i += 2;
                                break;
                            case "sin":
                                seq.Add(new Token("sin", true));
                                i += 2;
                                break;
                            case "tan":
                                seq.Add(new Token("tan", true));
                                break;
                            case "log":
                                seq.Add(new Token("log", true));
                                break;
                            case "sqr":
                                seq.Add(new Token("sqr", true));
                                break;
                            case "clg":
                                seq.Add(new Token("clg", true));
                                i += 2;
                                break;
                            case "flr":
                                seq.Add(new Token("flr", true));
                                i += 2;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (num != "")
            {
                seq.Add(new Token(Convert.ToDouble(num)));
                isNum.Add(true);
            }

            return seq.ToArray();
        }

        static bool IsType(char symbol, char[] cArray) //determines if a character is of a given type
        {
            foreach (char c in cArray)
            {
                if (symbol == c) return true;
            }
            return false;
        }

        static string RemoveSpaces(string line)
        {
            string noSpace = "";
            foreach (char c in line)
            {
                if (c != ' ') noSpace += c.ToString();
            }
            return noSpace;
        }

        #region Test
        static void Test()
        {
            AssertEquals(2 + 3, "2 + 3");
            AssertEquals(2 * 3.2, "2 * 3.2");
            AssertEquals(4.5 * (-6) + 1, "4.5 * (-6)+ 1");
            AssertEquals((-13 + 13) * 2, "(- 13 + 13) * 2");
            AssertEquals((4 + (4.1 - 3) * 3) * (-2), "(4 + (4.1- 3)* 3) * (-2)");
            AssertEquals(120, "5!");
            AssertEquals(128, "4^3.5");
            AssertEquals(97.2, "(-6*2)^2-(15.6*3)");
        }

        static void AssertEquals(double compare, string line)
        {
            Handler handler = new Handler();
            try
            {
                double result = handler.Evaluate(Tokenize(line));
                if (compare == result) return;
                throw new Exception("wrong answer");
            }
            catch (Exception e)
            {
                string message = "ASSERT: " + e.Message + " in: " + line;
                throw new Exception(message);
            }
        }
        #endregion 

        static void Main(string[] args)
        {
            Test();
            Handler handler = new Handler();
            bool isEnd = true;
            string inp = Console.ReadLine();
            while (isEnd)
            {
                Token[] seq = Tokenize(inp);
                double result = handler.Evaluate(seq);
                Console.WriteLine("result:" + result);
                inp = Console.ReadLine();
                if (inp == "") isEnd = false;
            }
        }
    }
}
