﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Glee.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace HideAndSeek
{
    public partial class Form1 : Form
    {
    //Constructor:
        public Form1()
        {
            InitializeComponent();
            pictureBox2.BackColor = System.Drawing.Color.Transparent;
            //uthis.Refresh();
        }

    //Various Windows Form Event Handler:
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.fileURLTextBox.Text = "";
            // Show the dialog that allows user to select a file, the 
            // call will result a value from the DialogResult enum
            // when the dialog is dismissed.
            DialogResult result = this.openFileDialog.ShowDialog();
            // if a file is selected
            if (result == DialogResult.OK)
            {
                // Set the selected file URL to the textbox
                this.fileURLTextBox.Text = this.openFileDialog.FileName;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void QueryButton_Click(object sender, EventArgs e)
        {
            // Create form 2 if somehow closed by user
            if (Process.f2.IsDisposed)
                Process.f2 = new Form2();
            Process.runQuery(this,Process.f2);
        }

        private void fileURLTextBox_TextChanged(object sender, EventArgs e)
        {
            if(fileURLTextBox.Text != "")
            {
                this.changePictureIDLE();
                this.label10.Visible = false;
                Process.runDFS(this);
            }

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click_1(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.fileURLTextBox2.Text = "";
            DialogResult result = this.openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.fileURLTextBox2.Text = this.openFileDialog.FileName;
            }
        }

        private void fileURLTextBox2_TextChanged(object sender, EventArgs e)
        {
            // Create form 2 if somehow closed by user
            if (Process.f2.IsDisposed)
                Process.f2 = new Form2();
            this.changePictureIDLE();
            this.label10.Visible = false;
            if (this.fileURLTextBox2.Text != "")
            {
                Process.fileQuery(this, Process.f2);
                Process.f2.Show();
                //Console.WriteLine("f2 entered!");
            }
            else if (this.fileURLTextBox.Text == "")
            {
                Process.f2.resetBox();
                Process.f2.writeToBox("INPUT ERROR! Invalid format or null input!");
                Process.f2.Show();
                //this.fileURLTextBox2.Text = "PROCESS FAIL: Build graph first please!";
            }

        }

        //Setter & Getter from Form1 Class for Process Class:
        //Get External File URL:
        public String getFileURL()
        {
            return this.fileURLTextBox.Text;
        }
        public String getFileURL2()
        {
            return this.fileURLTextBox2.Text;
        }
        //Get 0/1 from textbox:
        public String get01()
        {
            return this.comboBox1.Text;
        }
        //Get "rumah" to be checked from textbox:
        public String geta()
        {
            return this.textBox1.Text;
        }
        //Get starting point from textbox:
        public String getb()
        {
            return this.textBox2.Text;
        }
    //Miscellaneous Functions:
        //Display Query Result:
        public void displayQans(int truthval)
        {
            this.label10.Visible = true;
            if (truthval==1)
                this.Qans.Text = "YES";
            else
                this.Qans.Text = "NO";
        }
        //Change Rilakuma to YES!:
        public void changePictureYES()
        {
            pictureBox1.Image = Properties.Resources.yay;
            pictureBox1.Refresh();
            pictureBox1.Visible = true;
        }
        //Change Rilakuma to NO!:
        public void changePictureNO()
        {
            pictureBox1.Image = Properties.Resources.nocowell;
            pictureBox1.Refresh();
            pictureBox1.Visible = true;
        }
        //Change Rilakuma to IDLE FACE!:
        public void changePictureIDLE()
        {
            pictureBox1.Image = Properties.Resources.hide;
            pictureBox1.Refresh();
            pictureBox1.Visible = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
       
        }
    }

    //MAIN FUNCTIONS:
    static class Process
    {
        static int jmlRumah;
        static List<List<int>> tree;
        static bool[] isBody;
        static int[] pointsTo;
        static bool[] visited;
        static long[] arrive;
        static long[] leave;
        static long globaleZeit = 1;
        static List<int> Path = new List<int>();
        //Create graph object
        static Microsoft.Glee.Drawing.Graph defaultG = new Microsoft.Glee.Drawing.Graph("graph");
        // Creating form for graph visualization
        static System.Windows.Forms.Form graphForm = new System.Windows.Forms.Form();
        // Create object GViewer to view the constructed graph
        static Microsoft.Glee.GraphViewerGdi.GViewer viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();
        static public Form2 f2 = new Form2();
        //[STAThread]
        static void buildTree(ref StreamReader sr, int jmlRumah)
        {
            tree = new List<List<int>>(jmlRumah + 1);
            pointsTo = new int[jmlRumah + 1];
            isBody = new bool[jmlRumah + 1];
            isBody[1] = true;

            for (int i = 0; i <= jmlRumah; i++)
            {
                tree.Add(new List<int>());
            }

            List<(int, int)> pending = new List<(int a, int b)>();

            for (int i = 0; i < jmlRumah - 1; i++)
            {
                string[] line = sr.ReadLine().Split();

                int a = Int32.Parse(line[0]);
                int b = Int32.Parse(line[1]);


                if (isBody[a])
                {
                    pointsTo[b] = a;
                    isBody[b] = true;
                }
                else if (isBody[b])
                {
                    pointsTo[a] = b;
                    isBody[a] = true;
                }
                else
                {
                    pending.Add((a, b));
                }
                tree[a].Add(b);
                tree[b].Add(a);
                // Add edges between 2 vertices to graph object:
                Microsoft.Glee.Drawing.Edge e = defaultG.AddEdge(line[0], line[1]);
                // Set edges without arrowhead => UNDIRECTED GRAPH!
                e.Attr.ArrowHeadAtTarget = ArrowStyle.None;
            }

            int idx = 0;
            while (pending.Any())
            {
                if (idx >= pending.Count())
                {
                    idx = 0;
                }
                int a = pending[idx].Item1;
                int b = pending[idx].Item2;
                if (isBody[a])
                {
                    pointsTo[b] = a;
                    isBody[b] = true;
                    pending.RemoveAt(idx);
                }
                else if (isBody[b])
                {
                    pointsTo[a] = b;
                    isBody[a] = true;
                    pending.RemoveAt(idx);
                }
                else
                {
                    idx++;
                }
            }
        }

        static void DFS(int startNode, ref List<List<int>> tree)
        {
            visited[startNode] = true;
            arrive[startNode] = globaleZeit++;
            foreach (var childNode in tree[startNode])
            {
                if (!visited[childNode])
                {
                    DFS(childNode, ref tree);
                }
            }
            leave[startNode] = globaleZeit++;
        }

        static bool isChildOf(int nodeChild, int nodeParent)
        {
            if(nodeChild == nodeParent)
            {
                return true;
            }
            else
                return arrive[nodeChild] > arrive[nodeParent] && leave[nodeChild] < leave[nodeParent];
        }
        // Generate Path:
        static public List<int> generatePath(int bottom, int upper, bool reverse)
        {
            Path.Clear();
            int current = bottom;
            if(upper == bottom)
            {
                Path.Add(current);
            }
            else {
                while (current != pointsTo[upper])
                {
                    //Console.WriteLine(Path);
                    Path.Add(current);
                    current = pointsTo[current];
                }
                if (reverse)
                {
                    Path.Reverse();
                }
            }


            return Path;
        }
        // Uncolor Graph:
        public static void resetGraph()
        {
            for (int i = 0; i < Path.Count; i++)
            {
                // COLOR THE NODES TRAVERSED IN RED
                if(Path[i].ToString() == "1")
                {
                    Microsoft.Glee.Drawing.Node n = defaultG.FindNode(Path[i].ToString());
                    n.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.SeaGreen;
                }
                else
                {
                    Microsoft.Glee.Drawing.Node n = defaultG.FindNode(Path[i].ToString());
                    n.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.White;
                }
                
            }
        }
        // Deepcopy Object Function:
        public static T DeepClone<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
        // Colorize edges and vertices to mark path:
        public static void updateGraph()
        {
            Console.WriteLine(Path.Count);
            for (int i = 0; i < Path.Count; i++)
            {
                // COLOR THE NODES TRAVERSED IN RED
                Node n = defaultG.FindNode(Path[i].ToString());
                n.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.DarkRed;
                // Bind the graph to the viewer
                viewer.Graph = defaultG;
                viewer.Dock = DockStyle.Fill;
                graphForm.Refresh();
                System.Threading.Thread.Sleep(500);
                // COLOR THE EDGES TOO:
                // Edge e = graph.fi
                // e.Attr.ArrowHeadAtTarget = ArrowStyle.None;
            }
        }

        // FUNCTION TO DO LOAD GRAPH AND RUN DFS:
        public static void runDFS(Form1 f1)
        {
            defaultG = new Graph("graph");
            if(graphForm.IsDisposed)
            {
                Console.WriteLine(graphForm == null);
                graphForm = new Form();
                viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();
            }
            StreamReader sr = new StreamReader(@f1.getFileURL());
            string Rumah = sr.ReadLine();
            jmlRumah = Int32.Parse(Rumah);

            buildTree(ref sr, jmlRumah);

            // DFS
            arrive = new long[jmlRumah + 1];
            leave = new long[jmlRumah + 1];
            visited = new bool[jmlRumah + 1];

            DFS(1, ref tree);

            // COLOR THE ROOT (1) WITH SEAGREEN COLOR
            Microsoft.Glee.Drawing.Node n1 = defaultG.FindNode("1");
            n1.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.SeaGreen;
            // Make Root a double-circled vertex just for emphasis, hehe:
            n1.Attr.Shape = Microsoft.Glee.Drawing.Shape.DoubleCircle;

            // SHOWING THE GRAPH:
            // Bind the graph to the viewer
            viewer.Graph = defaultG;

            // Associate the viewer with the created form
                graphForm.SuspendLayout();
                viewer.Dock = System.Windows.Forms.DockStyle.Fill;
                graphForm.Controls.Add(viewer);
                graphForm.ResumeLayout();
                graphForm.Refresh();

            // Show the form as a new window, BUT not locked there!
                graphForm.Show();
        }

    // FUNCTION TO DO QUERIES:
        //External file queries:
        public static void fileQuery(Form1 f1,Form2 f2)
        {
            f2.resetBox();
            StreamReader sr = new StreamReader(@f1.getFileURL2());
            string Query = sr.ReadLine();
            int jmlQuery = Int32.Parse(Query);

            for (int i = 0; i < jmlQuery; i++)
            {
                string[] line = sr.ReadLine().Split();
                int Q = Int32.Parse(line[0]);
                int dest = Int32.Parse(line[1]);
                int source = Int32.Parse(line[2]);

                if (dest > jmlRumah || dest < 1 || source > jmlRumah || source < 1)
                {
                    f2.resetBox();
                    f2.writeToBox("QUERY ERROR: INPUT OUT OF BOUNDS!");
                    f2.Show();
                }
                else
                {
                    if (Q == 1)
                    {
                        if (isChildOf(dest, source))
                        {
                            f2.writeToBox("YES");
                            f2.writeToBox(String.Join(" -> ", generatePath(dest, source, true)));
                        }
                        else
                        {
                            f2.writeToBox("NO");
                        }
                    }
                    else
                    {
                        if (isChildOf(source, dest))
                        {
                            f2.writeToBox("YES");
                            f2.writeToBox(String.Join(" -> ", generatePath(source, dest, false)));
                        }
                        else
                        {
                            f2.writeToBox("NO");
                        }
                    }
                }
            }
            f2.Show();

        }

        // Run individual query:
        public static void runQuery(Form1 f1, Form2 f2)
        {
            if (f1.get01() != "" && f1.geta() != "" && f1.getb() != "" && f1.getFileURL() != "")
            {
                // Parse input (string) into integer from textboxes.
                int Q = Int32.Parse(f1.get01());
                int _a = Int32.Parse(f1.geta());
                int _b = Int32.Parse(f1.getb());

                if (_a > jmlRumah || _a < 1 || _b > jmlRumah || _b < 1)
                {
                    f2.resetBox();
                    f2.writeToBox("QUERY ERROR: INPUT OUT OF BOUNDS!");
                    f2.Show();
                }
                else
                {
                    // ResetGraph First:
                    if (Path != null)
                    {
                        resetGraph();
                        viewer.Graph = defaultG;
                        viewer.Dock = System.Windows.Forms.DockStyle.Fill;
                        graphForm.Refresh();

                    }

                    // Check for answer:
                    if (Q == 1)
                    {
                        if (isChildOf(_a, _b))
                        {
                            f1.displayQans(1);
                            f1.changePictureYES();
                            // Generate Path:
                            generatePath(_a, _b, true);
                            // Update Graph:
                            updateGraph();
                        }
                        else
                        {
                            f1.displayQans(0);
                            f1.changePictureNO();
                        }
                    }
                    else
                    {
                        if (isChildOf(_b, _a))
                        {
                            f1.displayQans(1);
                            f1.changePictureYES();
                            // Generate Path:
                            generatePath(_b, _a, false);
                            // Update Graph:
                            updateGraph();
                        }
                        else
                        {
                            f1.displayQans(0);
                            f1.changePictureNO();
                        }
                    }
                }
                
            }
            
        }

    }


}
