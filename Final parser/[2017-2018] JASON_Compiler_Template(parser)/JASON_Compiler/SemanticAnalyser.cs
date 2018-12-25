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
        //May add void to TokenClass
        public string ID;
        public Token_Class ReturnType;
        public List<string> ParamterDataType = new List<string>();
        public int ParameterNumber = 0;
    }

    class SemanticAnalyser
    {
        public static Node treeroot;

        public static List<SymbolValue> SymbolTable = new List<SymbolValue>();
        public static List<SymbolValue> FunctionTable = new List<SymbolValue>();

        public static string CurrentScope="main";

        public SemanticAnalyser()
        {
            CurrentScope = "Main";
        }
        static bool DeclareFunc(FunctionValue fv)
        {
            return true;
        }
        static bool AddVariable(SymbolValue NewSymbolVal)
        {
            SymbolValue Result = SymbolTable.Find(sv => sv.Name == NewSymbolVal.Name);
            if (Result != null)
            {

                if (Result.Scope == NewSymbolVal.Scope)
                {
                    MessageBox.Show("Variable already declared");
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
        static bool AssignValue(string VariableName,Node Expression)
        {
            SymbolValue Result = SymbolTable.Find(sv => sv.Name == VariableName);
            if (Result == null)
            {
                MessageBox.Show("Variable Doesn't Exist");
                return false;
            }
            else
            {
                if (Result.DataType == Expression.datatype)
                {
                    Result.Value = Expression.value;

                }
                else
                {
                    //Don't forget..int goes in float !!!!
                    MessageBox.Show("DataTypes Missmatch");
                    return false;
                }
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
            if (root.Name == "DeclerationStatement")
            {
                 HandleDeclerationStatment(root);
            }
            if (root.Name == "AssignmentStatement")
            {
                HandleAssignmentStatement(root);
            }
            if(root.Name == "FuncStatement")
            {
                HandleFunctionStatement(root);
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
            root.datatype = root.children[0].Name;
            root.token.token_type=root.children[0].token.token_type;
        }
        public static void HandleListIdentifier(Node root)
        {
            if (root.children.Count== 0)
            {
                return;
            }
            if(root.children[0].Name== "Hazmbola")
            {
                root.children[0].datatype = root.datatype;
                HandleZ(root.children[0]);
            }
            root.children[1].datatype = root.datatype;
            HandleListIdentifier(root.children[1]);
            
           
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
            AssignValue(root.children[0].Name, root.children[2]);
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
                //string!!
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
            if (root.children[0].Name == "Eq1")
            {
                HandleEquation1(root.children[0]);
                root.datatype = root.children[0].datatype;
                root.value = root.children[0].value;

            }
            else
            {

            }   
        }
        public static void HandleEquation1(Node root)
        {
            if(root.children[0].Name == "Eq2")
            {
                HandleEquation2(root.children[0]);
                root.datatype = root.children[0].datatype;
                root.value = root.children[0].value;
            }
            else
            {

            }
        }
        public static void HandleEquation2(Node root)
        {
            if (root.children[0].Name == "Term")
            {
                HandleTerm(root.children[0]);
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
                    root.children[0].datatype = "float";
                    root.children[0].children[0].datatype = root.children[0].datatype;
                }
                else
                {
                    root.children[0].datatype = "int";
                    root.children[0].children[0].datatype = root.children[0].datatype;
                }
               
            }
            else if (root.children[0].Name == "FunctionCall")
            {
                ///function call
            }
            else
            {
                SymbolValue Result = SymbolTable.Find(sv => sv.Name == root.children[0].Name);
                root.children[0].datatype = Result.DataType;
                root.children[0].value = Result.Value;
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
                if (root.datatype == "int")
                {
                    root.children[0].value = 0;
                }
                else if (root.datatype == "float")
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
            //root.children[2]=(
            fv.ParamterDataType = new List<string>();
            HandleListParameters(root.children[3], fv.ParamterDataType);
            fv.ParameterNumber = fv.ParamterDataType.Count();
            MessageBox.Show(fv.ParameterNumber.ToString());
            //still need to add parameters to Symbol table
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
                //ha3ml 7aga b namae el parameter ??? 
                HandleListParameters(root.children[2],list);
            }
            else
            {
                //root.children[0] -> ","
                HandleDatatype(root.children[1]);
                list.Add(root.children[1].datatype);
                //root.children[1] ParameterName!
                //ha3ml 7aga b namae el parameter ??? 
                HandleListParameters(root.children[3],list);
            }
        }
        public static void HandleFuncBody(Node root)
        {

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
