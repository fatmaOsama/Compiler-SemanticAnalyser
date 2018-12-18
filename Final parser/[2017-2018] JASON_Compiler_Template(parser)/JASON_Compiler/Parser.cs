using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> children = new List<Node>();
        public Token token;
        public int value = Int32.MinValue;
        //public object value;
        public string datatype = "";
        public string Name;
      
        public void SetNodeName(string N)
        {
            this.Name = N;
        }
        public Node()
        {
            token = new Token();
        }
        public Node(string emptyToken)
        {
            token = new Token();
            token.lex = emptyToken;
            Name = emptyToken;
        }
    }
    class SyntaxAnalyser
    {
        public static List<Token> myTokens = new List<Token>();
        public static int i = 0;
        public static Node ParseTree;
        public static Node Parse(List<Token> Tokens)
        {
            myTokens = Tokens;
            Node root = new Node();

            //write your parser code
            root.children.Add(Program());
            return root;
        }
        static Node Program()
        {
            Node n = new Node("Program");

                n.children.Add(FuncStatement());
                n.children.Add(MainFunction());

            


            return n;
        }
        static Node FuncStatement()
        {
            Node n = new Node("FuncStatement");
            n.children.Add(FuncStatement1());
            return n;
        }
        static Node FuncStatement1()
        {
            Node n = new Node("FuncStatement1");

            if (i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.Integer || myTokens[i].token_type == Token_Class.String || myTokens[i].token_type == Token_Class.Float)
                {
                    Node s = FuncDecl();
                    if (s == null)
                        return n;
                    n.children.Add(s);
                    //MessageBox.Show(i.ToString());
                    
                    n.children.Add(FuncBody());
                    n.children.Add(FuncStatement1());
                }
               /* else
                {
                    Errors.Error_List.Add("it should start with DataType");
                }*/
            }

            return n;
        }
        static Node FuncDecl()
        {
            Node n = new Node("FunDecl");
            if ((myTokens[i].token_type == Token_Class.Integer || myTokens[i].token_type == Token_Class.Float || myTokens[i].token_type == Token_Class.String) && myTokens[i + 1].token_type == Token_Class.Main) { return null; }
            n.children.Add(DataType());
            n.children.Add(FuncName());
            n.children.Add(match(Token_Class.LeftParanthesis));
            n.children.Add(ListParameters());
            n.children.Add(match(Token_Class.RightParanthesis));
            return n;
        }
        static Node FuncBody()
        {
            Node n = new Node("FuncBody");
            n.children.Add(match(Token_Class.LeftBrace));
            //MessageBox.Show("left" + i.ToString());
            n.children.Add(Statements());
            n.children.Add(ReturnStatement());
            n.children.Add(match(Token_Class.SemiColon));
           // MessageBox.Show("Return" + i.ToString());
            n.children.Add(match(Token_Class.RightBrace));
           // MessageBox.Show("R" + i.ToString());
            return n;
        }
        static Node Statements()
        {
            Node n = new Node("Statemments");
            if (i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.Idenifier && (myTokens[i + 1].token_type == Token_Class.LessThanOp || myTokens[i + 1].token_type == Token_Class.GreaterThanOp || myTokens[i + 1].token_type == Token_Class.EqualOp || myTokens[i + 1].token_type == Token_Class.NotEqualOp) ||
                (myTokens[i].token_type == Token_Class.Idenifier || myTokens[i].token_type == Token_Class.Integer || myTokens[i].token_type == Token_Class.Float) || myTokens[i].token_type == Token_Class.String ||
                myTokens[i].token_type == Token_Class.Read || myTokens[i].token_type == Token_Class.Write ||
                myTokens[i].token_type == Token_Class.Repeat || myTokens[i].token_type == Token_Class.If)
                {
                    n.children.Add(Statement());
                    n.children.Add(Statements());
                }
               
            }

            return n;
        }
        static Node Statement()
        {
            Node n = new Node("Statemment");
            if (i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.Idenifier && (myTokens[i + 1].token_type == Token_Class.LessThanOp || myTokens[i + 1].token_type == Token_Class.GreaterThanOp || myTokens[i + 1].token_type == Token_Class.EqualOp || myTokens[i + 1].token_type == Token_Class.NotEqualOp))
                {
                    n.children.Add(ConditionStatement());

                }
                else if (myTokens[i].token_type == Token_Class.Idenifier)
                {
                    n.children.Add(AssignmentStatement());
                    n.children.Add(match(Token_Class.SemiColon));
                }
                else if (myTokens[i].token_type == Token_Class.Integer || myTokens[i].token_type == Token_Class.Float || myTokens[i].token_type == Token_Class.String)
                {
                    n.children.Add(DeclerationStatement());
                    n.children.Add(match(Token_Class.SemiColon));
                }
                else if (myTokens[i].token_type == Token_Class.Read)
                {
                    n.children.Add(ReadStatement());
                    n.children.Add(match(Token_Class.SemiColon));
                }
                else if (myTokens[i].token_type == Token_Class.Write)
                {
                    n.children.Add(WriteStatement());
                    n.children.Add(match(Token_Class.SemiColon));
                }
                
                else if (myTokens[i].token_type == Token_Class.Repeat)
                {
                    n.children.Add(RepeatStatement());
                }
                else if (myTokens[i].token_type == Token_Class.If)
                {
                    n.children.Add(IfStatement());
                }
                else
                {
                   // Errors.Error_List.Add("missing identifier");
                }

            }

            return n;
        }

        static Node ListParameters()
        {
            Node n = new Node("ListParameters");

            if (i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.Integer || myTokens[i].token_type == Token_Class.Float || myTokens[i].token_type == Token_Class.String)
                {
                    n.children.Add(DataType());
                    n.children.Add(match(Token_Class.Idenifier));
                    n.children.Add(ListParameters1());
                }
            }

            return n;
        }
        static Node ListParameters1()
        {
            Node n = new Node("ListParameters1");
            if (i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.Comma)
                {
                    n.children.Add(match(Token_Class.Comma));
                    n.children.Add(DataType());
                    n.children.Add(match(Token_Class.Idenifier));
                    n.children.Add(ListParameters1());
                }

            }


            return n;
        }
        static Node FuncName()
        {
            Node n = new Node("FuncName");
            if (myTokens[i].token_type == Token_Class.Idenifier) { n.children.Add(match(Token_Class.Idenifier)); }
            
            return n;
        }
        static Node MainFunction()
        {
            Node n = new Node("MainFunction");
            MessageBox.Show("i=" + i.ToString());
            n.children.Add(DataType());
            MessageBox.Show("i=" + i.ToString());
            n.children.Add(match(Token_Class.Main));
            MessageBox.Show("i=" + i.ToString());
            n.children.Add(match(Token_Class.LeftParanthesis));
            n.children.Add(match(Token_Class.RightParanthesis));
            n.children.Add(FuncBody());

            return n;
        }

        static Node DataType()
        {
            Node n = new Node("DataType");

            if(i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.Integer)
            {
                n.children.Add(match(Token_Class.Integer));
            }
            else if(myTokens[i].token_type == Token_Class.Float)
            {
                 n.children.Add(match(Token_Class.Float));
            }
           else if(myTokens[i].token_type ==Token_Class.String)

            {
                n.children.Add(match(Token_Class.String));
           }
               
                
            }
            
           
            return n;
        }



        static Node AssignmentStatement()
        {
            Node n = new Node("AssignmentStatement");

            if (i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.Idenifier)
                {
                    n.children.Add(match(Token_Class.Idenifier));
                    n.children.Add(match(Token_Class.Assign));
                    n.children.Add(Expression());
                    
                }
            }

            return n;
        }

        static Node DeclerationStatement()
        {
            Node n = new Node("DeclerationStatement");
            n.children.Add(DataType());
            n.children.Add(List_Identifiers());
            
            return n;
        }
        static Node List_Identifiers()
        {
            Node n = new Node("List_Identifiers");
            n.children.Add(Hazmbola());
            n.children.Add(List_Identifiers1());
            return n;
        }
        static Node Hazmbola()
        {
            Node n = new Node("Hazmbola");
            if(myTokens[i].token_type == Token_Class.Idenifier && myTokens[i+1].token_type == Token_Class.Assign)
            {
                n.children.Add(AssignmentStatement());
                
            }
            else if(myTokens[i].token_type == Token_Class.Idenifier)
            {
                n.children.Add(Parameters());
            }
            
            return n;
        }
        static Node List_Identifiers1()
        {
            Node n = new Node("List_Identifiers1");
            if (i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.Comma)
                {
                    n.children.Add(match(Token_Class.Comma));
                    n.children.Add(Hazmbola());
                    n.children.Add(List_Identifiers1());
                }
            }

            return n;
        }
        static Node WriteStatement()
        {
            Node n = new Node("WriteStatement");
            n.children.Add(match(Token_Class.Write));
            n.children.Add(A());
           
            return n;
        }
        static Node A()
        {
            Node n = new Node("A");
            if (myTokens[i].token_type == Token_Class.Endl)
            {
                n.children.Add(match(Token_Class.Endl));
            }
            else if (myTokens[i].token_type == Token_Class.String || 
                myTokens[i].token_type == Token_Class.LeftParanthesis || myTokens[i].token_type == Token_Class.Idenifier ||
                myTokens[i].token_type == Token_Class.Constant || myTokens[i].token_type == Token_Class.Float||
                myTokens[i].token_type == Token_Class.LeftParanthesis && myTokens[i + 1].lex == "Equation")
            {
                n.children.Add(Expression());
            }
            
            
            return n;
        }
        static Node ReadStatement()
        {
            Node n = new Node("ReadStatement");
            n.children.Add(match(Token_Class.Read));
            n.children.Add(match(Token_Class.Idenifier));
           
            return n;
        }

        static Node ReturnStatement()
        {
            Node n = new Node("ReturnStatement");
            n.children.Add(match(Token_Class.Return));
            //MessageBox.Show("r" + i.ToString());
             if ((myTokens[i].token_type == Token_Class.String)||( myTokens[i].token_type == Token_Class.LeftParanthesis || myTokens[i].token_type == Token_Class.Idenifier || myTokens[i].token_type == Token_Class.Constant || myTokens[i].token_type == Token_Class.Float) )
                {
                    n.children.Add(Expression());
                }
                
          
           

            return n;
        }

        static Node Expression()
        {
            Node n = new Node("Expression");
            if (i < myTokens.Count)
            {
                if (myTokens[i].token_type == Token_Class.String)
                {
                    n.children.Add(match(Token_Class.String));
                }
                else if (myTokens[i].token_type == Token_Class.LeftParanthesis || myTokens[i].token_type == Token_Class.Idenifier || myTokens[i].token_type == Token_Class.Constant || myTokens[i].token_type == Token_Class.Float)
                {
                    if (myTokens[i + 1].token_type == Token_Class.MultiplyOp || myTokens[i + 1].token_type == Token_Class.MinusOp ||
                        myTokens[i + 1].token_type == Token_Class.PlusOp || myTokens[i + 1].token_type == Token_Class.DivideOp)
                    {
                        n.children.Add(Equation());
                    }
                    else
                    {
                        n.children.Add(Term());
                    }

                }

            }
            return n;
        }
        /// <summary>
        /// ////////////////
        /// </summary>
        /// <returns></returns>
        static Node Term()
        {
            Node n = new Node("Term");
            if (myTokens[i].token_type == Token_Class.Constant )
            {
                n.children.Add(match(Token_Class.Constant));

            }
            else if (myTokens[i].token_type == Token_Class.Idenifier && myTokens[i + 1].token_type == Token_Class.LeftParanthesis) { n.children.Add(FunctionCall()); }
            else if (myTokens[i].token_type == Token_Class.Idenifier) { n.children.Add(match(Token_Class.Idenifier)); }

            return n;
        }
        static Node Parameters()
        {
            Node n = new Node("Parameters");
            n.children.Add(terminals());
            n.children.Add(ParametersDash());
            return n;
        }
        static Node ParametersDash()
        {
            Node n = new Node("ParametersDash");

            if (myTokens[i].token_type == Token_Class.Comma)
            {
                n.children.Add(match(Token_Class.Comma));
                n.children.Add(terminals());
                n.children.Add(ParametersDash());
            }
            return n;
        }

        static Node ConditionStatement()
        {
            Node n = new Node("ConditionStatement");
            n.children.Add(ConditionStatementDash());
            n.children.Add(ConditionStatementOR());
            return n;
        }
        static Node ConditionStatementOR()
        {
            Node n = new Node("ConditionStatementOR");
            if (myTokens[i].token_type == Token_Class.Or)
            {
                n.children.Add(match(Token_Class.Or));
                n.children.Add(ConditionStatementDash());
                n.children.Add(ConditionStatementOR());

            }


            return n;
        }
        static Node ConditionStatementDash()
        {
            Node n = new Node("ConditionStatementDash");

            n.children.Add(Condition());
            n.children.Add(ConditionStatementAnd());



            return n;

        }
        static Node ConditionStatementAnd()
        {
            Node n = new Node("ConditionStatementAnd");
            if (myTokens[i].token_type == Token_Class.And)
            {
                n.children.Add(match(Token_Class.And));
                n.children.Add(Condition());
                n.children.Add(ConditionStatementAnd());

            }
           


            return n;
        }
        static Node Condition()
        {
            Node n = new Node("Condition");
            n.children.Add(match(Token_Class.Idenifier));
            n.children.Add(ConditionOperator());
            n.children.Add(Term());


            return n;
        }


        static Node RepeatStatement()
        {
            Node n = new Node("RepeatStatement");
            n.children.Add(match(Token_Class.Repeat));
            n.children.Add(Statements());
            n.children.Add(match(Token_Class.Until));
            n.children.Add(ConditionStatement());
           
            return n;
        }
        static Node ConditionOperator()
        {
            Node n = new Node("ConditionOperator");
            if (myTokens[i].token_type == Token_Class.GreaterThanOp)
            {
                n.children.Add(match(Token_Class.GreaterThanOp));
            }
            else if(myTokens[i].token_type == Token_Class.LessThanOp)
            {
                n.children.Add(match(Token_Class.LessThanOp));
            }
            else if(myTokens[i].token_type == Token_Class.EqualOp)
            {
                n.children.Add(match(Token_Class.EqualOp));
            }
            else if(myTokens[i].token_type == Token_Class.NotEqualOp)
            {
                n.children.Add(match(Token_Class.NotEqualOp));
            }
            
            return n;
        }
        static Node IfStatement()
        {
            Node n = new Node("IfStatement");
            n.children.Add(match(Token_Class.If));
            n.children.Add(ConditionStatement());
            n.children.Add(match(Token_Class.Then));
            n.children.Add(Statements());
            n.children.Add(ElseChoice());


            return n;
        }
        static Node BooleanOperator()
        {
            Node n = new Node("BooleanOperator");
            n.children.Add(match(Token_Class.And));
            n.children.Add(match(Token_Class.Or));


            return n;
        }
        static Node ElseChoice()
        {
            Node n = new Node("ElseChoice");

            if (myTokens[i].token_type == Token_Class.ElseIf) { n.children.Add(ElseIfStatement()); }
            else if (myTokens[i].token_type == Token_Class.Else) { n.children.Add(ElseStatement()); }
            else if (myTokens[i].token_type == Token_Class.End) { n.children.Add(match(Token_Class.End)); }



            return n;
        }
        static Node ElseIfStatement()
        {
            Node n = new Node("ElseIfStatement");
            n.children.Add(match(Token_Class.ElseIf));
            n.children.Add(ConditionStatement());
            n.children.Add(match(Token_Class.Then));
            n.children.Add(Statements());
            n.children.Add(ElseChoice());
            return n;

        }
        static Node ElseStatement()
        {
            Node n = new Node("ElseStatement");
            n.children.Add(match(Token_Class.Else));
            n.children.Add(Statements());
            n.children.Add(match(Token_Class.End));

            return n;

        }


        static Node FunctionCall()
        {
            Node n = new Node("FunctionCall");
            n.children.Add(match(Token_Class.Idenifier));
            n.children.Add(match(Token_Class.LeftParanthesis));

            n.children.Add(FuncCallList());
            n.children.Add(match(Token_Class.RightParanthesis));


            return n;

        }
        static Node FuncCallList()
        {
            Node n = new Node("FuncCallList");
            if (myTokens[i].token_type == Token_Class.Idenifier || myTokens[i].token_type == Token_Class.Constant) { n.children.Add(FuncCallListDash()); }
            return n;

        }
        static Node FuncCallListDash()
        {
            Node n = new Node("FuncCallListDash");
           
             if(  myTokens[i].token_type == Token_Class.Idenifier || myTokens[i].token_type == Token_Class.Constant)
            { 
                n.children.Add(Parameters());
                n.children.Add(FuncCallListDash());
            }




            return n;
        }

        static Node Equation()
        {
            Node n = new Node("Equation");
            n.children.Add(Eq1());
            n.children.Add(EquationDash());
            return n;

        }
        static Node EquationDash()
        {
            Node n = new Node("EquationDash");
            if (myTokens[i].token_type == Token_Class.PlusOp || myTokens[i].token_type == Token_Class.MinusOp)
            {
                n.children.Add(AddOp());
                n.children.Add(Eq1());
                n.children.Add(EquationDash());
            }
            return n;

        }
        static Node Eq1()
        {
            Node n = new Node("Eq1");
            n.children.Add(Eq2());
            n.children.Add(Eq1Dash());

            return n;

        }
        static Node Eq1Dash()
        {
            Node n = new Node("Eq1Dash");
            if (myTokens[i].token_type == Token_Class.MultiplyOp || myTokens[i].token_type == Token_Class.DivideOp)
            {
                n.children.Add(MulOp());
                n.children.Add(Eq2());
                n.children.Add(Eq1Dash());
            }
            return n;

        }
        static Node Eq2()
        {
            Node n = new Node("Eq2");
            if (myTokens[i].token_type == Token_Class.LeftParanthesis)
            {

                n.children.Add(match(Token_Class.LeftParanthesis));
                n.children.Add(Equation());
                n.children.Add(match(Token_Class.RightParanthesis));

            }
            else
            {
                n.children.Add(Term());
            }
            return n;

        }
        static Node AddOp()
        {
            Node n = new Node("AddOp");
            if (myTokens[i].token_type == Token_Class.PlusOp) { n.children.Add(match(Token_Class.PlusOp)); }
            else if (myTokens[i].token_type == Token_Class.MinusOp) { n.children.Add(match(Token_Class.MinusOp)); }
            return n;

        }
        static Node MulOp()
        {
            Node n = new Node("MulOp");
            if (myTokens[i].token_type == Token_Class.MultiplyOp) { n.children.Add(match(Token_Class.MultiplyOp)); }
            else if (myTokens[i].token_type == Token_Class.DivideOp) { n.children.Add(match(Token_Class.DivideOp)); }
            return n;

        }




        public static Node match(Token_Class ExpectedToken)
        {
            string temp = Enum.GetName(typeof(Token_Class), ExpectedToken);
            Node node = new Node(temp);
            //Node node = new Node();
            //  if (i < myTokens.Count)
            //{
            if (myTokens[i].token_type == ExpectedToken)
                {
                    node.token = myTokens[i];
                    i++;
                }
                else if (myTokens[i].token_type == Token_Class.Comment) 
                {
                    i++;
                }
                else
                {
                    Errors.Error_List.Add(" " + "missing" + " ");
                    Errors.Error_List.Add(ExpectedToken.ToString());
                }
            //}
            
            return node;
        }


        static Node terminals()
        {
            Node n = new Node("terminals");
            if (myTokens[i].token_type == Token_Class.Constant )
            { n.children.Add(match(Token_Class.Constant)); }
            else if (myTokens[i].token_type == Token_Class.Idenifier)
            {
                Node temp = match(Token_Class.Idenifier);
                temp.Name = myTokens[i].lex;
                n.children.Add(temp);
            }
            return n;

        }
        //use this function to print the parse tree in TreeView Toolbox
        public static TreeNode PrintParseTree(Node root)
        {
            ParseTree = root;
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.token == null)
                return null;
            TreeNode tree = new TreeNode(root.token.lex);
            if (root.children.Count == 0)
                return tree;
            foreach (Node child in root.children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}