using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public enum Token_Class
{
    Else, If, ElseIf, Read, Then, Until, Write, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant, String, Float, Integer, And, Or, Endl,
         LeftParanthesis, RightParanthesis,Main,Repeat,End,
    Comment, LeftBrace, RightBrace, SemiColon, Assign, Comma, Return,Error
}

namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public static List<Token> Tokens = new List<Token>();
        public static List<Token> Tokens1 = new List<Token>();
        //public List<Token> Errors = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("int", Token_Class.Integer);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("main",Token_Class.Main);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("end", Token_Class.End);



            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);
            Operators.Add("(", Token_Class.LeftParanthesis);
            Operators.Add(")", Token_Class.RightParanthesis);
            Operators.Add("{", Token_Class.LeftBrace);
            Operators.Add("}", Token_Class.RightBrace);
            Operators.Add(";", Token_Class.SemiColon);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add(",", Token_Class.Comma);



        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = "";



                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;


                if ((CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z'))
                {

                    while ((CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9'))
                    {
                        CurrentLexeme += CurrentChar;
                        i++;
                        if (i >= SourceCode.Length)
                        {
                            break;
                        }
                        CurrentChar = SourceCode[i];


                    }
                    i--;
                    FindTokenClass(CurrentLexeme);
                    CurrentLexeme = "";



                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    CurrentLexeme = "";
                    bool flag = true;
                    int cntr=0;
                    while ( true  )  
                    {
                        if(   CurrentChar == ' ' || CurrentChar == '\r'|| CurrentChar == '\n'|| CurrentChar =='\t'
                           || CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' ||  CurrentChar == '/' 
                           || CurrentChar == '=' || CurrentChar == '<' || CurrentChar == '>' ||  CurrentChar == '(' 
                           || CurrentChar == ')' || CurrentChar == '{' || CurrentChar == '}' ||  CurrentChar == ':'
                           || CurrentChar == ';' || CurrentChar == ',')
                        {
                            i--;
                            break;
                        }
                        else if (CurrentChar == '.')
                        {
                            cntr++;
                        }
                        else if (CurrentChar < '0' || CurrentChar > '9')
                        {
                            flag = false;
                        }

                        i++;
                        CurrentLexeme+=CurrentChar;
                        if (i >= SourceCode.Length)
                        {
                            break;
                        }
                        CurrentChar = SourceCode[i];
                    }
                   // MessageBox.Show("ana tl3t");
                    if ( cntr < 2 &&  flag )
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                    else
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    CurrentLexeme = "";
                }


                //comments
                else if (CurrentChar == '/' && i + 1 != SourceCode.Length - 1 && SourceCode[i + 1] == '*')
                {
                    CurrentLexeme = "";
                    CurrentLexeme += CurrentChar;
 

                    i += 1;
                    while (true)
                    {
                        CurrentChar = SourceCode[i];
                        CurrentLexeme += CurrentChar;
                        i++;
                        if (i == SourceCode.Length)
                        {
                          ////  Token error = new Token();
                          //  error.lex = CurrentLexeme;
                          //  error.token_type = Token_Class.SyntaxError;
                          //  Errors.Add(error); 
                            Errors.Error_List.Add(CurrentLexeme);
                            break;
                        }
                        if (i + 1 < SourceCode.Length  && SourceCode[i] == '*' && SourceCode[i + 1] == '/') 
                        {
                            i++;
                            CurrentLexeme += '*';
                            CurrentLexeme += '/';
                            FindTokenClass(CurrentLexeme);
                            CurrentLexeme = "";
                          //  MessageBox.Show("bo2loz");
                            break;
                        }
                        
                    }

                }
                else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' ||
                         CurrentChar == '/' || CurrentChar == '=' || CurrentChar == '<' ||
                         CurrentChar == '>' || CurrentChar == '(' || CurrentChar == ')' ||
                         CurrentChar == '{' || CurrentChar == '}' || CurrentChar == ':' ||
                         CurrentChar == ';' || CurrentChar == ',')
                {
                    CurrentLexeme = "";
                    CurrentLexeme += CurrentChar;
                    if (CurrentChar == '<' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '>')
                    {
                        i++;
                        CurrentLexeme += SourceCode[i];
                    }
                    if (CurrentChar == ':' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '=')
                    {
                        i++;
                        CurrentLexeme += SourceCode[i];
                    }
                    FindTokenClass(CurrentLexeme);
                    CurrentLexeme = "";

                }
                //   && ||
                else if ((CurrentChar == '&' || CurrentChar == '|') &&
                         (i + 1 < SourceCode.Length && SourceCode[i + 1] == CurrentChar))
                {

                    i++;
                    CurrentLexeme = "";
                    CurrentLexeme += CurrentChar;
                    CurrentLexeme += CurrentChar;
                    FindTokenClass(CurrentLexeme);
                    CurrentLexeme = "";


                }
                else if (CurrentChar == '"')
                {
                    CurrentLexeme += CurrentChar;
                    i++;
                    CurrentChar = SourceCode[i];
                    bool flag = true;
                    while (CurrentChar != '"')
                    {
                        CurrentLexeme += CurrentChar;
                        i++;
                        if (i == SourceCode.Length)
                        {
                            Errors.Error_List.Add(CurrentLexeme);
                            flag = false;
                            break;
                        }
                        CurrentChar = SourceCode[i];
                    }
                    CurrentLexeme += CurrentChar;
                    if (flag)
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                    CurrentLexeme = "";
                }
                else
                {
                   // Console.WriteLine("ERROR!!!!!!!!!!!!!!!!!");
                    //Token error = new Token();
                    //error.lex = CurrentLexeme;
                    //error.token_type = Token_Class.Unrecognized_Token;
                    //Errors.Add(error);
                    CurrentLexeme += CurrentChar;
                    Errors.Error_List.Add(CurrentLexeme);
                    CurrentLexeme ="";

                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok.lex = Lex;


            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
            }

            else if ((Lex[0] >= 'A' && Lex[0] <= 'Z') || (Lex[0] >= 'a' && Lex[0] <= 'z'))
            {
                Tok.token_type = Token_Class.Idenifier;
            }

            else if (Lex[0] >= '0' && Lex[0] <= '9')
            {
                
                 Tok.token_type = Token_Class.Constant;
                
            }
            else if (Lex.Length > 1 && Lex[0] == '/' && Lex[1] == '*')
            {
                Tok.token_type = Token_Class.Comment;
            }
            else if (Lex[0] == '+' || Lex[0] == '-' || Lex[0] == '*' ||
                    Lex[0] == '/' || Lex[0] == '<' || Lex[0] == '>' ||
                    Lex[0] == '=' || Lex[0] == '(' || Lex[0] == ')' ||
                    Lex[0] == '{' || Lex[0] == '}' || Lex[0] == ':' ||
                    Lex[0] == ',' || Lex[0] == ';' || Lex[0]=='&' || Lex[0]=='|')
            {
                Tok.token_type = Operators[Lex];
            }
            else if (Lex[0] == '"')
            {
                Tok.token_type = Token_Class.String;
            }
            else if (Lex[0] == '/')
            {
                Tok.token_type = Token_Class.Comment;
            }

            Tokens.Add(Tok);
            if (Tok.token_type != Token_Class.Comment)
            {
                Tokens1.Add(Tok);
            }
        }

        bool isIdentifier(string lex)
        {
            bool isValid = true;


            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;

            return isValid;
        }
    }
}
