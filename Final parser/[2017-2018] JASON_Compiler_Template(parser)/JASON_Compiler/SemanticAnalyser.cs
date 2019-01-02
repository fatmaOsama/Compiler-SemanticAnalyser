using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    class SymbolValue
    {
        public string Name;
        public string DataType;
        public string Scope;
        public object Value;
    }
    class FunctionValue
    {
        public string ID;
        public Token_Class ReturnType;
        public List<string> ParamterDataType = new List<string>();
        public int ParameterNumber = 0;
    }

    class SemanticAnalyser
    {
        public static Node treeroot;
        public static Node tem = new Node();
        public static Node Newtem = new Node();
        public static List<SymbolValue> SymbolTable = new List<SymbolValue>();
        public static List<FunctionValue> FunctionTable = new List<FunctionValue>();

        public static string CurrentScope="main",CalledFunction="",InnerScope="",CurrentDeclaredFn="";
        public static bool GoInIf = false,GoInElse=false,GoInELseIf=false;

        public SemanticAnalyser()
        {
            CurrentScope = "main";
        }
        static bool DeclareFunc(FunctionValue NewFunction)
        {
            FunctionValue Result = FunctionTable.Find(fv => fv.ID == NewFunction.ID);
            if(Result != null)
            {
                MessageBox.Show("Function " + NewFunction.ID + " already Declared");
                return false;
            }
            FunctionTable.Add(NewFunction);
            return true;
        }
        static bool AddVariable(SymbolValue NewSymbolVal)
        {
            SymbolValue Result = SymbolTable.Find(sv => sv.Name == NewSymbolVal.Name && sv.Scope==NewSymbolVal.Scope);
            if (Result != null)
            {

                if (Result.Scope == NewSymbolVal.Scope)
                {
                    MessageBox.Show("Variable " +Result.Name+" already declared");
                    return false;
                }
                else
                {
                    SymbolTable.Add(NewSymbolVal);
                }
            }
            else
            {
                SymbolTable.Add(NewSymbolVal);

            }
            return true;
        }
        public static void HandleMulOp(Node root, Node root1)
        {

            if (root1.children[0].children[0].Name == "*" && root.datatype == "Integer")
            {

                root1.value = Convert.ToInt16(root1.value) * Convert.ToInt16(root.value);

            }

            if (root1.children[0].children[0].Name == "/" && root.datatype == "Integer")
            {
                root1.value = Convert.ToInt16(root.value) / Convert.ToInt16(root1.value);
            }

            if (root1.children[0].children[0].Name == "*" && root.datatype == "float")
            {
                root1.value = Convert.ToDouble(root1.value) * Convert.ToDouble(root.value);

            }

            if (root1.children[0].children[0].Name == "/" && root.datatype == "float")
            {

                root1.value = Convert.ToDouble(root.value) / Convert.ToDouble(root1.value);
            }




        }

        public static void HandleAddOp(Node root, Node root1)
        {
            if (root.datatype == Token_Class.Float.ToString() && root1.datatype == Token_Class.Integer.ToString())
            {
                
                root1.datatype = Token_Class.Float.ToString();
            }
            else if (root.datatype == Token_Class.Integer.ToString() && root1.datatype == Token_Class.Float.ToString())
            {
                
                root.datatype = Token_Class.Float.ToString();
            }

            if (root1.children[0].children[0].Name == "+" && root.datatype == Token_Class.Integer.ToString())
            {

                root1.value = Convert.ToInt32(root1.value) + Convert.ToInt32(root.value);

            }

            if (root1.children[0].children[0].Name == "-" && root.datatype == Token_Class.Integer.ToString())
            {
                root1.value = Convert.ToInt32(root.value) - Convert.ToInt32(root1.value);
            }

            if (root1.children[0].children[0].Name == "+" && root.datatype == Token_Class.Float.ToString())
            {
                root1.value = Convert.ToDouble(root1.value) + Convert.ToDouble(root.value);

            }

            if (root1.children[0].children[0].Name == "-" && root.datatype == Token_Class.Float.ToString())
            {

                root1.value = Convert.ToDouble(root.value) - Convert.ToDouble(root1.value);
            }

        }

        static bool AssignValue(string VariableName,Node Expression)
        {
            SymbolValue Result = SymbolTable.Find(sv => sv.Name == VariableName && CurrentScope.Contains(sv.Scope));
            if (Result == null)
            {
                MessageBox.Show("Variable "+ VariableName+" Doesn't Exist");
                return false;
            }
            else
            {
                if (Result.DataType == Expression.datatype)                    
                {
                    Result.Value = Expression.value;

                }
                else if (Result.DataType == Token_Class.Float.ToString() && Expression.datatype == Token_Class.Integer.ToString())
                {
                    Result.Value = Expression.value;
                    Expression.datatype= Token_Class.Float.ToString(); 
                }
                else if(Result.DataType == Token_Class.Integer.ToString() && Expression.datatype == Token_Class.Float.ToString())
                {
                    Result.Value = Expression.value;
                    Result.DataType = Token_Class.Float.ToString();
                }
                else
                {
                    //Don't forget..int goes in float !!!!
                    MessageBox.Show("DataTypes"+Result.DataType+"and"+ Expression.datatype + "Missmatch");
                    return false;
                }
            }
            return true;
        }

        static bool CheckIfDeclared(string VariableName,string ParameterType)
        {
            SymbolValue Result = SymbolTable.Find(sv => sv.Name == VariableName && CurrentScope.Contains(sv.Scope));
            if (Result==null|| Result.DataType!=ParameterType)
            {
                MessageBox.Show("Variable" + VariableName + " is not declared");
                return false;
            }
            return true;
        }
        public static void TreeName(Node root)
        {
            foreach(Node child in root.children)
            {
                child.Name = child.token.lex;
                TreeName(child);
            }
        }
        public static void TraverseTree(Node root)
        {
            for (int i = 0; i < root.children.Count; i++)
            {
                TraverseTree(root.children[i]);
            }
            if (root.Name == "FuncStatment1" || root.Name == "FuncStatement")
            {
                HandleFunctionStatement(root);
            }
            if (root.Name == "MainFunction")
            {
                FunctionValue fv = new FunctionValue();
                HandleDatatype(root.children[0]);
                fv.ReturnType = root.children[0].token.token_type;
                fv.ParameterNumber = 0;
                fv.ID = root.children[1].Name;
                DeclareFunc(fv);
                HandleMainStatments(root.children[4].children[1]);
                //HandleReturnStatment(root.children[4].children[2]);
            }
        }
        public static void HandleMainStatments(Node root)
        {
            if (root.Name == "DeclerationStatement")
            {
                HandleDeclerationStatment(root);
            }
            else if (root.Name == "AssignmentStatement")
            {
                HandleAssignmentStatement(root);
            }
           else if (root.Name == "IfStatement")
            {
                HandleIfStatment(root);
            }
            else if (root.Name == "ReadStatement")
            {
                HandleReadStatement(root);
            }
            else if (root.Name == "WriteStatement")
            {
                HandleWriteStatement(root);
            }
            else if (root.Name == "RepeatStatement")
            {
                HandleRepeatStatement(root);
            }

            else
            {
                for(int i = 0; i < root.children.Count; i++)
                {
                    HandleMainStatments(root.children[i]);
                }
            }
        }
        public static void HandleDeclerationStatment(Node root)
        {
            SymbolValue sv = new SymbolValue();
            HandleDatatype(root.children[0]);
            sv.DataType=root.children[0].datatype;
            root.children[1].datatype = root.children[0].datatype;
            HandleListIdentifier(root.children[1]);
        }
        public static void HandleDatatype(Node root)
        {
            root.datatype = root.children[0].token.token_type.ToString() ;
            root.token.token_type=root.children[0].token.token_type;
        }
        public static void HandleListIdentifier(Node root)
        {
            if (root.children.Count== 0)
            {
                return;
            }
            if (root.children[0].Name == "Hazmbola" || root.Name == "Hazmbola")
            {
                root.children[0].datatype = root.datatype;
                HandleZ(root.children[0]);
                if (root.children.Count == 2)
                {
                    root.children[1].datatype = root.datatype;
                    HandleListIdentifier(root.children[1]);
                }
            }
            else
            {
                int start = 0;
                if (root.children[0].Name == ",")
                {
                    start = 1;
                }
                for (int i = start; i < root.children.Count; i++)
                {
                    root.children[i].datatype = root.datatype;
                    HandleListIdentifier(root.children[i]);
                }
            }
         }
        public static void HandleZ(Node root)
        {
            
            root.children[0].datatype = root.datatype;
            if (root.children[0].Name== "AssignmentStatement")
            {
                HandleExpression(root.children[0].children[2]);
                SymbolValue sv = new SymbolValue();
                sv.Name = root.children[0].children[0].Name;
                sv.DataType = root.children[0].datatype;
                sv.Scope = CurrentScope;
                sv.Value = "0";//mo2ktan
                AddVariable(sv);
                HandleAssignmentStatement(root.children[0]);
            }
            else
            {
                
                HandleParameters(root.children[0]);
             }
        }
        public static void HandleAssignmentStatement(Node root)
        {
            HandleExpression(root.children[2]);
            if(AssignValue(root.children[0].Name, root.children[2]))
            {
                root.children[0].value = root.children[2].value;
                root.children[0].datatype = root.children[2].datatype;
                root.value = root.children[2].value;
                root.datatype = root.children[2].datatype;
            }
            
        }
        public static void HandleExpression(Node root)
        {
            if (root.children[0].Name == "Term")
            {
                HandleTerm(root.children[0]);
            }
            else if (root.children[0].Name == "Equation")
            {
                HandleEquation(root.children[0]);
            }
            else
            {
                root.children[0].value = root.children[0].Name;
                root.children[0].datatype = Token_Class.String.ToString();
            }
            root.datatype = root.children[0].datatype;
            root.value = root.children[0].value;

        }
        public static void HandleEquation(Node root)
        {
            if (root.children.Count == 0)
            {
                return;

            }
            Handle(root);
            if (root.children[1].children.Count == 0)
            {
                return;

            }

            else
            {
                if (root.children.Count == 2)
                {

                    HandleEquation1(root.children[0]);
                    root.value = root.children[0].value;
                    root.datatype = root.children[0].datatype;
                    tem.value = root.value;
                    tem.datatype = root.datatype;
                    HandleEquation(root.children[1]);
                    root.value = root.children[1].value;
                    root.datatype = root.children[1].datatype;
                    //HandleAddOp(root, root.children[1]);

                }

                if (root.children.Count == 3)
                {

                    HandleEquation1(root.children[1]);
                    root.value = root.children[1].value;
                    root.datatype = root.children[1].datatype;
                    HandleAddOp(tem, root);
                    tem.value = root.value;
                    tem.datatype = root.datatype;
                    HandleEquation(root.children[2]);

                    root.value = tem.value;
                    root.datatype = tem.datatype;
                }
            }
        }
        public static void Handle(Node root)
        {
            if (root.children.Count == 0)
            {
                return;
            }

            if (root.children[0].Name== "MulOp")
            {


                HandleEquation1(root.children[0]);
                root.value = root.children[0].value;
                root.datatype = root.children[0].datatype;
            }

            else
            {

                //  HandleEquation(root);

            }
        }
        public static void HandleEquation1(Node root)
        {
            if (root.children.Count == 0)
            {
                return;
            }
            if (root.children[0].Name == "Eq2")
            {
                HandleEquation2(root.children[0]);
                root.datatype = root.children[0].datatype;
                root.value = root.children[0].value;
                Newtem.value = root.value;
                Newtem.datatype = root.datatype;
            }

            if (root.children[1].children.Count == 3)
            {

                HandleEquation1(root.children[1]);
                root.value = root.children[1].value;
                root.datatype = root.children[1].datatype;

            }

            if (root.children[1].Name == "Eq2")
            {

                HandleEquation2(root.children[1]);
                root.datatype = root.children[1].datatype;
                root.value = root.children[1].value;
                HandleMulOp(Newtem, root);
                Newtem.value = root.value;
                Newtem.datatype = root.datatype;
                HandleEquation1(root.children[2]);
                root.value = Newtem.value;
                root.datatype = Newtem.datatype;
            }
        }
        public static void HandleEquation2(Node root)
        {
            if (root.children[0].Name == "Term")
            {
                HandleTerm(root.children[0]);
                root.value = root.children[0].value;
                root.datatype = root.children[0].datatype;
            }
            else
            {
                //(
                HandleEquation(root.children[1]);
                root.datatype = root.children[1].datatype;
                root.value = root.children[1].value;
                //)
            }
        }
        public static void HandleTerm(Node root)
        {
            if (root.children[0].Name == "Constant")
            {
                root.children[0].value=root.children[0].children[0].Name;
                if (root.children[0].children[0].Name.Contains("."))
                {
                    root.children[0].datatype = root.children[0].datatype.ToString();
                    root.children[0].datatype = Token_Class.Float.ToString();
                    root.children[0].children[0].datatype = Token_Class.Float.ToString();
                }
                else
                {
                    root.children[0].datatype = root.children[0].datatype.ToString();
                    root.children[0].datatype = Token_Class.Integer.ToString();
                    root.children[0].children[0].datatype = Token_Class.Integer.ToString();
                }
               
            }
            else if (root.children[0].Name == "FunctionCall")
            {
                HandleFuncCall(root.children[0]);
                root.children[0].value =float.MinValue; //default value! Needs If conditions
            }
            else
            {
                
                SymbolValue Result = SymbolTable.Find(sv => sv.Name == root.children[0].Name && CurrentScope.Contains(sv.Scope));
                if (Result == null)
                {
                    MessageBox.Show("Variable " + root.children[0].Name + " doesn't exist in " + CurrentScope);
                }
                else
                {

                    root.children[0].datatype = Result.DataType;
                    root.children[0].value = Result.Value;
                }
            }
            root.datatype = root.children[0].datatype;
            root.value = root.children[0].value;
        }
        public static void HandleParameters(Node root)
        {
            if (root.children.Count == 0)
            {
                return;
            }
            if (root.children.Count == 1)
            {
                SymbolValue sv = new SymbolValue();
                if (root.datatype == Token_Class.Integer.ToString())
                {
                    root.children[0].value = 0;
                }
                else if (root.datatype == Token_Class.Float.ToString())
                {
                    root.children[0].value = 0.0;
                }
                else
                {
                    root.children[0].value = "empty";
                }
                sv.Name = root.children[0].Name;
                sv.Value = root.children[0].value;
                sv.DataType = root.datatype;
                sv.Scope = CurrentScope;
                AddVariable(sv);
            }
            else 
            {
                int start = 0;
                if (root.children[0].Name == ",")
                {
                    start = 1;
                }
                for(int i = start; i < root.children.Count; i++)
                {
                    root.children[i].datatype = root.datatype;
                    HandleParameters(root.children[i]);
                }
            }
            
        }
        public static void HandleFuncCall(Node root)
        {
            string FunctionName = root.children[0].Name;
            CalledFunction = FunctionName;
            FunctionValue temp = FunctionTable.Find(fv => fv.ID == CalledFunction);
            if (temp == null)
            {
                MessageBox.Show("Function " + CalledFunction + " is not declared");
            }
            else
            {
               root.datatype = temp.ReturnType.ToString();

            }
            //root.children[1] (
            int count = 0;
            HandleCallList(root.children[2].children[0],ref count);
            //MessageBox.Show(count.ToString());
            if (temp.ParameterNumber != count)
            {
                MessageBox.Show("Exceeded Parameters Number!");
            }
        }
        public static void HandleCallList(Node root,ref int count)
        {
            if (root.children.Count() == 0)
            {
                return;
            }
            if(root.Name== "FuncCallListDash" || root.Name== "FuncCallList")
            {
                HandleCallList(root.children[0],ref count);
            }
            else
            {
                int start = 0;
                if (root.children[0].Name == ",")
                {
                    start = 1;
                }
                for (int i = start; i < root.children.Count; i++)
                {
                    HandleCallList(root.children[i], ref count);
                }
            }
            if (root.Name=="terminals") //yeb2a ana fi terminal 
            {
                FunctionValue temp = FunctionTable.Find(fv => fv.ID == CalledFunction);
                if (temp == null)
                {
                    MessageBox.Show("Function is not declared");
                }
                else if(count < temp.ParameterNumber)
                {
                    string ParameterType= temp.ParamterDataType[count];
                    string VariableName = root.children[0].Name;
                    if(!CheckIfDeclared(VariableName, ParameterType))
                    {
                        MessageBox.Show("Wrong parameter");
                    }
                }
                count++; //kda da el number of parameters

            }
           
        }
        public static void HandleFunctionStatement(Node root)
        {
            if (root.children.Count == 0)
            {
                return;
            }
            if (root.Name == "FuncStatment1" || root.Name== "FuncStatement")
            {
                HandleFunctionStatement(root.children[0]);
            }
            else
            {
                HandleFuncDecl(root.children[0]);
                HandleFuncBody(root.children[1]);
                HandleFunctionStatement(root.children[2]);
            }

        }
        public static void HandleFuncDecl(Node root)
        {
            FunctionValue fv = new FunctionValue();
            HandleDatatype(root.children[0]);
            fv.ReturnType = root.children[0].token.token_type;
            //HandleFuncName
            fv.ID = root.children[1].children[0].Name;
            CurrentScope = fv.ID;
            CurrentDeclaredFn = fv.ID;
            //root.children[2]=(
            fv.ParamterDataType = new List<string>();
            HandleListParameters(root.children[3], fv.ParamterDataType);
            fv.ParameterNumber = fv.ParamterDataType.Count();
            //MessageBox.Show(fv.ParameterNumber.ToString());
            //still need to add parameters to Symbol table
            DeclareFunc(fv);
        }
        public static void HandleListParameters(Node root, List<string> list)
        {
            if (root.children.Count == 0)
            {
                return;
            }
            if (root.children.Count == 3)
            {
                HandleDatatype(root.children[0]);
                list.Add(root.children[0].datatype);
                //root.children[1] ParameterName!
                root.children[1].datatype = root.children[0].datatype;
                SymbolValue sv = new SymbolValue();
                sv.Name = root.children[1].Name;
                sv.Scope = CurrentScope;
                sv.DataType = root.children[0].datatype;
                sv.Value = 0;//mo2ktan
                AddVariable(sv);
                //ha3ml 7aga b namae el parameter ??? 
                HandleListParameters(root.children[2],list);
            }
            else
            {
                //root.children[0] ->"."
                HandleDatatype(root.children[1]);
                list.Add(root.children[1].datatype);
                //root.children[2] ParameterName!
                root.children[2].datatype = root.children[1].datatype;
                SymbolValue sv = new SymbolValue();
                sv.Name = root.children[2].Name;
                sv.Scope = CurrentScope;
                sv.DataType = root.children[1].datatype;
                sv.Value = 0;//mo2ktan
                AddVariable(sv);
                //ha3ml 7aga b namae el parameter ??? 
                HandleListParameters(root.children[3], list);
            }
        }
        public static void HandleFuncBody(Node root)
        {
            //root.children[0] } 
            //root.children[1] statments
            HandleMainStatments(root.children[1]);
            //root.children[2] return statment
            HandleReturnStatment(root.children[2]);
            //root.children[3] ;
            //root.children[4] }
        }
        public static void HandleReturnStatment(Node root)
        {
            ////root.children[0] return
            HandleExpression(root.children[1]);
            if (!CompareReturnType(CurrentScope, root.children[1].token.token_type))
            {
                MessageBox.Show("Return Type incompatable");
            }
            CurrentScope = "main";
        }
        static bool CompareReturnType(string FunctionName, Token_Class Datatype)
        {
            FunctionValue Result = FunctionTable.Find(fv => fv.ID == FunctionName);
            if (Result == null)
            {
                MessageBox.Show("Function does't exist, something went wrong");
                return false;
            }
            else
            {
                if (Result.ReturnType != Datatype)
                {
                    return false;
                }
            }
            return true;
        }
        public static void HandleReadStatement(Node root)
        {
            //root.children[0] read;
            string currentVariable = root.children[1].Name;
            SymbolValue Result = SymbolTable.Find(sv => sv.Name == currentVariable && CurrentScope.Contains(sv.Scope));
            if (Result == null)
            {
                MessageBox.Show("Variable " + currentVariable + " doesn't exist");
            }
            else
            {
                root.value = Result.Value;
                root.datatype = Result.DataType;
                root.children[1].value = Result.Value;
                root.children[1].datatype = Result.DataType;
            }
        }
        public static void HandleWriteStatement(Node root)
        {
            //root.children[0] write;
            if (root.children[1].children[0].Name == "Expression")
            {
                HandleExpression(root.children[1].children[0]);
            }
            else if(root.children[1].children[0].Name == "endl")
            {
                root.children[1].children[0].datatype = Token_Class.String.ToString();
                root.children[1].children[0].value = "endl";
            }
            root.children[1].value = root.children[1].children[0].value;
            root.children[1].datatype = root.children[1].children[0].datatype;
            root.value = root.children[1].value;
            root.datatype = root.children[1].datatype;
        }
        public static void HandleRepeatStatement(Node root)
        {
            HandleConditionStatment(root.children[3]);
            CurrentScope += ".repeat";
            while (Convert.ToBoolean(root.children[3].value) == false)
            {
                HandleMainStatments(root.children[1]);
                HandleConditionStatment(root.children[3]);
            }
            string Reversed = ReverseString(CurrentScope);
            string[] Splited = Reversed.Split(new char[] { '.' }, 2);
            CurrentScope = ReverseString(Splited[1]);
        }
        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        public static void HandleIfStatment(Node root)
        {
            //root.children[0] -> if
            //root.children[1] -> ConditionStatment
            //root.children[2] -> then
            HandleConditionStatment(root.children[1]);
            if (Convert.ToBoolean(root.children[1].value) == true)
            {
                CurrentScope += ".If";
                GoInIf = true;
                //root.children[3] -> Statments
                HandleMainStatments(root.children[3]);
                string Reversed = ReverseString(CurrentScope);
                string[] Splited = Reversed.Split(new char[] { '.' }, 2);
                CurrentScope = ReverseString(Splited[1]);
                GoInIf = false;
            }
            else
            {
                //root.children[4] -> ElseChoice
                if (root.children[4].children.Count != 0)
                {
                HandleElseChoiceStatment(root.children[4].children[0]);

                }
            }
        }
        public static void HandleElseChoiceStatment(Node root)
        {
            if (root.children.Count == 0)
            {
                return;
            }
            HandleConditionStatment(root.children[1]);
            if (root.Name== "ElseIfStatement")
            {
                if (Convert.ToBoolean(root.children[1].value) == true)
                {
                    InnerScope += ".ElseIf";
                    GoInELseIf = true;
                    //root.children[3] -> Statments
                    HandleMainStatments(root.children[3]);
                    string Reversed = ReverseString(CurrentScope);
                    string[] Splited = Reversed.Split(new char[] { '.' }, 2);
                    CurrentScope = ReverseString(Splited[1]);
                    GoInELseIf = false;
                }
                else
                {
                    //root.children[4] -> ElseChoice
                    HandleElseChoiceStatment(root.children[4]);
                }
            }
            else if (root.Name == "ElseStatement")
            {
                CurrentScope += ".ElseIf";
                GoInElse = true;
                HandleMainStatments(root.children[1]);
                string Reversed = ReverseString(CurrentScope);
                string[] Splited = Reversed.Split(new char[] { '.' }, 2);
                CurrentScope = ReverseString(Splited[1]);
                GoInElse = false;
            }
        }
        public static void HandleConditionStatment(Node root)
        {
            bool final;
            if (root.children.Count == 0)
            {
                return;
            }
            if (root.Name== "ConditionStatementOR")
            {
                //ConditionOperator root.children[0];
                HandleConditionStatment(root.children[1]);
                HandleConditionStatment(root.children[2]);
                root.value = root.children[1].value;

            }
            else if (root.Name == "ConditionStatementAnd")
            {
                //opp
                //condtion
                string VariableName = root.children[1].children[0].Name;
                SymbolValue Result = SymbolTable.Find(sv => sv.Name == VariableName && CurrentScope.Contains(sv.Scope));
                if (Result == null)
                {
                    MessageBox.Show("Variable " + VariableName + " doesn't exist in " + CurrentScope);
                }
                else
                {
                    root.children[1].children[0].value = Result.Value;
                    root.children[1].children[0].datatype = Result.DataType;
                    string Opp = root.children[1].children[1].children[0].Name;
                    HandleTerm(root.children[1].children[2]);
                    root.children[1].value = EvaluateCondition(Result, Opp, root.children[1].children[2]);

                }
                //CondStatments
                HandleConditionStatment(root.children[2]);
                if (root.children[2].children.Count == 0)
                {
                    root.value = root.children[1].value;
                }
                else
                {
                    root.value = (Convert.ToBoolean(root.children[1].value) && Convert.ToBoolean(root.children[2].value));
                }

            }
            else
            {
                if (root.children[0].Name == "Condition")
                {
                    //Need to check every find conditions
                    string VariableName = root.children[0].children[0].Name;
                    SymbolValue Result = SymbolTable.Find(sv => sv.Name == VariableName && CurrentScope.Contains(sv.Scope));
                    if (Result == null)
                    {
                        MessageBox.Show("Variable " + VariableName + " doesn't exist in " + CurrentScope);
                    }
                    else
                    {

                        root.children[0].children[0].datatype = Result.DataType;
                        string Opp = root.children[0].children[1].children[0].Name;
                        HandleTerm(root.children[0].children[2]);
                        root.children[0].value = EvaluateCondition(Result, Opp, root.children[0].children[2]);
                        root.children[0].children[0].value = Result.Value;
                    }

                    HandleConditionStatment(root.children[1]);
                    if (root.children[1].children.Count == 0)
                    {
                        root.value = root.children[0].value;
                    }
                    else
                    {
                        root.value = (Convert.ToBoolean(root.children[0].value) && Convert.ToBoolean(root.children[1].value));
                    }
                }
                else
                {
                    HandleConditionStatment(root.children[0]);
                }
                HandleConditionStatment(root.children[1]);
            }
            final = Convert.ToBoolean(root.children[0].value) || Convert.ToBoolean(root.children[1].value);
            root.value = final;
        }
        public static bool EvaluateCondition(SymbolValue Right,string Opp ,Node Left)
        {
            bool Result = false;
            if (Right.DataType != Left.datatype)
            {
                MessageBox.Show("Cannot Compare different datatypes");
            }
            else
            {
                if (Opp == "=")
                {
                    if (Left.datatype == Token_Class.Integer.ToString())
                    {
                        Result = (Convert.ToInt32(Right.Value) == Convert.ToInt32(Left.value));
                    }
                    else if (Left.datatype == Token_Class.Float.ToString())
                    {
                        Result = (Convert.ToDecimal(Right.Value) == Convert.ToDecimal(Left.value));
                    }
                }
                else if (Opp == ">")
                {
                    if (Left.datatype == Token_Class.Integer.ToString())
                    {
                        Result = (Convert.ToInt32(Right.Value) > Convert.ToInt32(Left.value));
                    }
                    else if (Left.datatype == Token_Class.Float.ToString())
                    {
                        Result = (Convert.ToDecimal(Right.Value) > Convert.ToDecimal(Left.value));
                    }
                }
                else if (Opp == "<")
                {
                    if (Left.datatype == Token_Class.Integer.ToString())
                    {
                        Result = (Convert.ToInt32(Right.Value) < Convert.ToInt32(Left.value));
                    }
                    else if (Left.datatype == Token_Class.Float.ToString())
                    {
                        Result = (Convert.ToDecimal(Right.Value) < Convert.ToDecimal(Left.value));
                    }
                }
                else if (Opp == "<>")
                {
                    if (Left.datatype == Token_Class.Integer.ToString())
                    {
                        Result = (Convert.ToInt32(Right.Value) != Convert.ToInt32(Left.value));
                    }
                    else if (Left.datatype == Token_Class.Float.ToString())
                    {
                        Result = (Convert.ToDecimal(Right.Value) != Convert.ToDecimal(Left.value));
                    }
                }
            }
            return Result;
        }
        public static TreeNode PrintSemanticTree(Node root)
        {
            TreeNode tree = new TreeNode("Annotated Tree");
            TreeNode treeRoot = PrintAnnotatedTree(root);
            tree.Expand();
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintAnnotatedTree(Node root)
        {
            if (root == null)
                return null;
            TreeNode tree;
            if (root.value == null && root.datatype == "")
                tree = new TreeNode(root.Name);
            else if (root.value != null && root.datatype == "")
                tree = new TreeNode(root.Name + " & its value is: " + root.value);
            else if (root.value == null && root.datatype != "")
                tree = new TreeNode(root.Name + " & its datatype is: " + root.datatype);
            else
                tree = new TreeNode(root.Name + " & its value is: " + root.value + " & datatype is: " + root.datatype);
            tree.Expand();
            if (root.children.Count == 0)
                return tree;
            foreach (Node child in root.children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintAnnotatedTree(child));
            }
            return tree;
        }

    }
}
