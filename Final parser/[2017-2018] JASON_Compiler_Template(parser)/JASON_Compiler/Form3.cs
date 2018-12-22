using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public partial class Form3 : Form
    {
        Node MainRoot;
        public Form3()
        {
            InitializeComponent();
            Node root = SyntaxAnalyser.Parse(Scanner.Tokens);
            MainRoot = root;
            treeView1.Nodes.Add(SyntaxAnalyser.PrintParseTree(root));
            PrintErrors();
        }

        public TreeNodeCollection ReturnTree()
        {
            return treeView1.Nodes;
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        void PrintErrors()
        {
            for (int i = 0; i < Errors.Error_List.Count; i++)
            {
                textBox2.Text += Errors.Error_List[i];
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SemanticAnalyser.treeroot = MainRoot;
            SemanticAnalyser.TreeName(SemanticAnalyser.treeroot);
            SemanticAnalyser.TraverseTree(SemanticAnalyser.treeroot);
            treeView1.Nodes.Add(SemanticAnalyser.PrintSemanticTree(SemanticAnalyser.treeroot));
            foreach (var item in SemanticAnalyser.SymbolTable)
            {
                listBox1.Items.Add(item.Scope + " " + item.DataType + " " + item.Name + "=" + item.Value);
            }
        }
    }
}
