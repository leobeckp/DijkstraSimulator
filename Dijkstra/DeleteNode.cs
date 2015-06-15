using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dijkstra
{
    public partial class DeleteNode : Form
    {
        public class GraphRowInfo
        {
            public bool Adj { get; set; }
            public string NName { get; set; }
            public string Id { get; set; }

            public GraphRowInfo(bool adj, string name, string id)
            {
                this.Adj = adj;
                this.NName = name;
                this.Id = id;
            }
        }

        public DeleteNode(Graph graph)
        {
            InitializeComponent();
            this.CurrentGraph = graph;
        }

        public Graph CurrentGraph { get; set; }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    this.CurrentGraph.Nodes.Remove(int.Parse(row.Cells[2].Value.ToString()));
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private void DeleteNode_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            var dl = new BindingList<GraphRowInfo>();
            foreach (var value in CurrentGraph.Nodes)
            {
                dl.Add(new GraphRowInfo(false, value.Value.Name, value.Key.ToString()));

            }
            this.dataGridView1.DataSource = dl;
        }
    }
}
