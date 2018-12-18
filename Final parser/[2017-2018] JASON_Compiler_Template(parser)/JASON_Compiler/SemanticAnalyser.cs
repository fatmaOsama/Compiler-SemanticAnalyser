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

        public static string CurrentScope;

        public SemanticAnalyser()
        {
            CurrentScope = "Main";
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
        static bool AssignValue()
        {
            return true;
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
                HandleZ(root);
            }
            root.children[1].datatype = root.datatype;
            HandleListIdentifier(root.children[1]);
            
           
        }
        public static void HandleZ(Node root)
        {

            root.children[0].datatype = root.datatype;
            HandleParameters(root.children[0]);

        }
        public static void HandleParameters(Node root)
        {
            if (root.children.Count == 0)
            {
                return;
            }
            if (root.children.Count == 2)
            {
                root.children[1].datatype = root.datatype;
                HandleParameters(root.children[1]);
            }
            else
            {
                SymbolValue sv = new SymbolValue();
                root.children[0].value = -1;
                sv.Name = root.children[0].Name;
                sv.Value = root.children[0].value;
                sv.DataType = root.datatype;
                sv.Scope = CurrentScope;
                AddVariable(sv);

            }
            
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
            if (root.value == Int32.MinValue && root.datatype == "")
                tree = new TreeNode(root.Name);
            else if (root.value != Int32.MinValue && root.datatype == "")
                tree = new TreeNode(root.Name + " & its value is: " + root.value);
            else if (root.value == Int32.MinValue && root.datatype != "")
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
