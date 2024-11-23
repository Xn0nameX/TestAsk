using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TestAsk.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TestAsk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Dictionary<string, string> nodeValues = new Dictionary<string, string>();

        private string GetNodeValue(TreeNode node, string filePath)
        {
            string value = string.Empty;

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == node.Text.Split('[')[0])
                    {
                        if (reader.Read() && reader.NodeType == XmlNodeType.Text)
                        {
                            value = reader.Value;
                        }
                        break;
                    }
                }
            }

            return value;
        }

        private async void LoadXmlDoc(string filePath)
        {
            treeView1.Nodes.Clear();
            nodeValues.Clear();

            XmlReaderSettings settings = new XmlReaderSettings
            {
                Async = true
            };

            using (XmlReader reader = XmlReader.Create(filePath, settings))
            {
                while (await reader.ReadAsync())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        TreeNode node = new TreeNode(reader.Name);
                        await Task.Run(() => AddChildNodes(reader, node));
                        treeView1.Invoke((MethodInvoker)delegate {
                            treeView1.Nodes.Add(node);
                        });
                    }
                }
            }
        }



        private void AddChildNodes(XmlReader reader, TreeNode treeNode)
        {
            Stack<TreeNode> stack = new Stack<TreeNode>();
            stack.Push(treeNode);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {

                    TreeNode childNode = new TreeNode(reader.Name);
                    stack.Peek().Nodes.Add(childNode);
                    stack.Push(childNode);

                    
                    if (!reader.IsEmptyElement && reader.Read() && reader.NodeType == XmlNodeType.Text)
                    {
                        nodeValues[childNode.Text] = reader.Value;
                        childNode.Text += $": {reader.Value}";
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    stack.Pop();
                    if (stack.Count == 0) break;
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = openFileDialog1.FileName;
            LoadXmlDoc(filePath);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            openFileDialog1.Filter = "Xml files (*.xml)|*.xml";
            openFileDialog1.Title = "Выберите xml файл";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
     
        }
    
        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("\nАвтор: Метелев Даниил Андреевич\nДата выполнения: 24.11.2024", "Помощь");
        }
    }
}
