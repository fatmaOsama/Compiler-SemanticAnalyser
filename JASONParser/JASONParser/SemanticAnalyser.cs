using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASONParser
{
    public class Node
    {
        public List<Node> children = new List<Node>();
        public int value = Int32.MinValue;
        public string datatype = "";
        public string Name;
        public Token token;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    class SymbolVal
    {
        public string Name;
        public string DataType;
        public string Scope;
        public object Value;
    }
    class funcVal
    {
        public List<string> dts = new List<string>();
        public int paramNo = 0;
    }
    class SemanticAnalyser
    {
        public static Dictionary<string, SymbolVal> SymbolTable = new Dictionary<string, SymbolVal>();
        public static Dictionary<string, funcVal> FuncTable = new Dictionary<string, funcVal>();

        //public static List<SymbolVal> SymbolTable = new List<SymbolVal>();
        //public static List<SymbolVal> FunctionTable = new List<SymbolVal>();

        //static bool AddVariable(SymbolVal NewSymbolVal)
        //{
        //    SymbolVal Result= SymbolTable.Find(sv => sv.Name == NewSymbolVal.Name);
        //    if (Result!= null )
        //    {
             
        //        if (Result.Scope == NewSymbolVal.Scope)
        //        {
        //            MessageBox.Show("Variable already declared");
        //            return false;
        //        }
        //        else
        //        {
        //            SymbolTable.Add(NewSymbolVal);
        //        }
        //    }
        //    else
        //    {
        //        SymbolTable.Add(NewSymbolVal);

        //    }
        //    return true;
        //}

        //static bool AssignValue()
        //{
        //    return true;
        //}
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////SEMANTIC CODE HERE///////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      
        static List<string> currentParam = new List<string>();
        public static Node treeroot;
        static string currentScope = "";
        static void ParamTraverse(Node root)
        {
            if (root.Name == "datatype")
            {
                currentParam.Add(root.children[0].Name);
            }
            for (int i = 0; i < root.children.Count; i++)
            {
                ParamTraverse(root.children[i]);
            }
        }
        public static void TraverseTree(Node root)
        {
            if (root.Name == "procdecl")
            {
                currentScope = root.children[0].children[1].Name;
                FuncTable.Add(currentScope, new funcVal());
                currentParam = new List<string>();
                ParamTraverse(root);
                FuncTable[currentScope].dts = currentParam;
                FuncTable[currentScope].paramNo = currentParam.Count;
            }
            for (int i = 0; i < root.children.Count; i++)
            {
                TraverseTree(root.children[i]);
            }
            if (root.Name == "vardecl")
            {
                handleVarDecl(root);
            }
            if (root.Name == "Param decl")
            {
                if (SymbolTable.ContainsKey(root.children[1].Name))
                {
                    MessageBox.Show("variable already declared");
                }
                else
                {
                    SymbolVal sv = new SymbolVal();
                    sv.DataType = root.children[0].children[0].Name;
                    sv.Value = 0;
                    sv.Scope = currentScope;
                    SymbolTable.Add(root.children[1].Name, sv);
                }
            }
        }
        public static void handleVarDecl(Node node)
        {
            node.children[0].datatype = node.children[0].children[0].Name;
            node.children[1].datatype = node.children[0].datatype;
            handleIDlist(node.children[1]);
        }
        public static void handleIDlist(Node node)
        {
            node.children[0].datatype = node.datatype;
            if (SymbolTable.ContainsKey(node.children[0].Name))
            {
                MessageBox.Show("variable already declared");
            }
            else
            {
                SymbolVal sv = new SymbolVal();
                sv.DataType = node.children[0].datatype;
                sv.Value = 0;
                sv.Scope = "Global";
                SymbolTable.Add(node.children[0].Name, sv);
            }
            if (node.children.Count > 1)
            {
                node.children[2].datatype = node.datatype;
                handleIDlist(node.children[2]);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
            if(root.value == Int32.MinValue && root.datatype == "")
                tree = new TreeNode(root.Name);
            else if(root.value != Int32.MinValue && root.datatype == "")
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
