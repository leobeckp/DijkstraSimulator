using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace Dijkstra
{
    public partial class Form1 : Form, IMessageFilter
    {
        public Form1()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            this.panel1.MouseWheel += pictureBox1_MouseWheel;
            Steps = new List<Graph>();
        }
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        public Graph CurrentGraph { get; set; }
        public Graph BaseGraph { get; set; }
        public GraphGeneration Generator { get; set; }
        public Image OriginalBitmap { get; set; }
        public Image CurrentBitmap { get; set; }
        private int CurrentStep = 0;
        private List<Graph> Steps { get; set; } 

        int _picWidth, _picHeight, _zoomInt = 0;
        private double _picRatio;
        private bool _isPanning = false;
        private Point _startPt;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BaseGraph = new Graph(GraphType.Undirected);
            this.CurrentGraph = this.BaseGraph.Copy();

            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            // GraphGeneration can be injected via the IGraphGeneration interface

            this.Generator = new GraphGeneration(getStartProcessQuery,
                                              getProcessStartInfoQuery,
                                              registerLayoutPluginCommand);
        }
        public void GetRatio()
        {
            _picRatio = ((double)_picHeight / (double)_picWidth);// needed for aspect ratio

        }
        public void ZoomPictureBox()
        {
            pictureBox1.Width = _picWidth;
            pictureBox1.Height = _picHeight;

            switch (_zoomInt)
            {
                case -2:
                    pictureBox1.Width = Convert.ToInt32(((double)pictureBox1.Width) * 0.5);
                    pictureBox1.Height = Convert.ToInt32(((double)pictureBox1.Width) * _picRatio);
                    break;
                case -1:
                    pictureBox1.Width = Convert.ToInt32(((double)pictureBox1.Width) * 0.75);
                    pictureBox1.Height = Convert.ToInt32(((double)pictureBox1.Width) * _picRatio);
                    break;
                case 0:
                    pictureBox1.Width = _picWidth;
                    pictureBox1.Height = _picHeight;
                    break;
                case 1:
                    pictureBox1.Width = Convert.ToInt32(((double)pictureBox1.Width) * 1.1);
                    pictureBox1.Height = Convert.ToInt32(((double)pictureBox1.Width) * _picRatio);
                    break;
                case 2:
                    pictureBox1.Width = Convert.ToInt32(((double)pictureBox1.Width) * 1.5);
                    pictureBox1.Height = Convert.ToInt32(((double)pictureBox1.Width) * _picRatio);
                    break;
                case 3:
                    pictureBox1.Width = Convert.ToInt32(((double)pictureBox1.Width) * 1.75);
                    pictureBox1.Height = Convert.ToInt32(((double)pictureBox1.Width) * _picRatio);
                    break;
                case 4:
                    pictureBox1.Width = Convert.ToInt32(((double)pictureBox1.Width) * 2);
                    pictureBox1.Height = Convert.ToInt32(((double)pictureBox1.Width) * _picRatio);
                    break;
            }


            pictureBox1.Update();
        }

        private void CenterImage()
        {
            int x = (panel1.Width - pictureBox1.Width)/2;
            int y = (panel1.Height - pictureBox1.Height)/2;

            pictureBox1.Location = new Point(x,y);
        }
        private void RefreshGraphDraw()
        {
            using (MemoryStream ms = new MemoryStream(Generator.GenerateGraph(this.CurrentGraph.ToDotFormat(), Enums.GraphReturnType.Png).ToArray()))
            {
                var img = Image.FromStream(ms);
                this.pictureBox1.Image = img;
                this.OriginalBitmap = this.pictureBox1.Image;
                pictureBox1.Image = img;
                pictureBox1.Width = img.Width;
                pictureBox1.Height = img.Height;
                _picWidth = pictureBox1.Width;
                _picHeight = pictureBox1.Height;
                GetRatio();
                CenterImage();
                this.CurrentBitmap = this.pictureBox1.Image;
            }
        }
        private void Update()
        {
            this.listView1.Items.Clear();
            this.comboBox1.Items.Clear();
            this.comboBox2.Items.Clear();

            this.CurrentGraph = this.BaseGraph.Copy();
            
            Steps.Clear();
            CurrentStep = -1;


            foreach (var node in CurrentGraph.Nodes)
            {
                this.listView1.Items.Add(
                    new ListViewItem(new string[]
                    {node.Value.Name, string.Join(", ", node.Value.AdjacencyList.Select(e => e.Key.Name))}));
                comboBox1.Items.Add(node.Value.Name);
                comboBox2.Items.Add(node.Value.Name);
            }

            RefreshGraphDraw();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (new AddNode(this.BaseGraph).ShowDialog() == DialogResult.OK)
                Update();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Você deve selecionar o nó de início.", "Erro ao executar Algoritmo de Dijkstra",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBox2.SelectedIndex < 0)
            {
                MessageBox.Show("Você deve selecionar o nó de término.", "Erro ao executar Algoritmo de Dijkstra",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.CurrentGraph = this.BaseGraph.Copy();
            var start = this.CurrentGraph.Nodes[comboBox1.SelectedIndex];
            var end = this.CurrentGraph.Nodes[comboBox2.SelectedIndex];

            Steps = this.CurrentGraph.Dijkstra(start, end);

            CurrentStep = this.Steps.Count - 1;
            button5.Enabled = true;
            button6.Enabled = false;
            this.CurrentGraph = Steps[CurrentStep];
            RefreshGraphDraw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (new DeleteNode(this.BaseGraph).ShowDialog() == DialogResult.OK)
                Update();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var form = new EditNode(this.BaseGraph);

            if (form.ShowDialog() == DialogResult.OK)
            {
                this.BaseGraph = form.CurrentGraph;
                Update();
            }
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                _zoomInt++;
                if (_zoomInt > 4)
                {
                    _zoomInt = 4;
                }
                ZoomPictureBox();
            }
            else if (e.Delta < 0)
            {
                _zoomInt--;
                if (_zoomInt == -3)
                {
                    _zoomInt = -2;
                }
                ZoomPictureBox();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _isPanning = true;
            _startPt = e.Location;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                //Cursor = Cursors.Hand;
                Cursor = Cursors.SizeAll;
                Control c = (Control)sender;
                c.Left = (c.Left + e.X) - _startPt.X;
                c.Top = (c.Top + e.Y) - _startPt.Y;
                c.BringToFront();

            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _isPanning = false;
            Cursor = Cursors.Default;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (CurrentStep == -1)
            {
                if (comboBox1.SelectedIndex < 0)
                {
                    MessageBox.Show("Você deve selecionar o nó de início.", "Erro ao executar Algoritmo de Dijkstra",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (comboBox2.SelectedIndex < 0)
                {
                    MessageBox.Show("Você deve selecionar o nó de término.", "Erro ao executar Algoritmo de Dijkstra",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                this.CurrentGraph = this.BaseGraph.Copy();
                var start = this.CurrentGraph.Nodes[comboBox1.SelectedIndex];
                var end = this.CurrentGraph.Nodes[comboBox2.SelectedIndex];

                Steps = this.CurrentGraph.Dijkstra(start, end);
                CurrentStep = 0;
            }
            else
            {
                if (CurrentStep >= Steps.Count - 1)
                {
                    button6.Enabled = false;
                    button5.Enabled = true;
                    return;
                }
                CurrentStep++;
                button5.Enabled = true;
                if (CurrentStep >= Steps.Count - 1)
                {
                    button6.Enabled = false;
                    button5.Enabled = true;
                }
                this.CurrentGraph = Steps[CurrentStep];
                RefreshGraphDraw();
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (CurrentStep == 0)
            {
                button5.Enabled = false;
                button6.Enabled = true;
            }
            else
            {
                CurrentStep--;
                if (CurrentStep == 0)
                {
                    button5.Enabled = false;
                    button6.Enabled = true;
                }
                button6.Enabled = true;
                this.CurrentGraph = Steps[CurrentStep];
                RefreshGraphDraw();
            }
        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this.BaseGraph);
            stream.Close();
        }

        private void carregarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Graph obj = (Graph)formatter.Deserialize(stream);
            stream.Close();
            this.BaseGraph = obj;
            this.Update();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CurrentGraph = this.BaseGraph.Copy();

            Steps.Clear();
            CurrentStep = -1;
            button6.Enabled = true;
            button5.Enabled = false;
            
            RefreshGraphDraw();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CurrentGraph = this.BaseGraph.Copy();

            Steps.Clear();
            CurrentStep = -1;
            button6.Enabled = true;
            button5.Enabled = false;

            RefreshGraphDraw();
        }
    }
}
