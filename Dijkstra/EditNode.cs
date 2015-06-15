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
    public partial class EditNode : Form
    {
        public class GraphRowInfo
        {
            public bool Adj { get; set; }
            public string NName { get; set; }
            public string Weight { get; set; }
            public string Id { get; set; }

            public GraphRowInfo(bool adj, string name, string weight, string id)
            {
                this.Adj = adj;
                this.NName = name;
                this.Weight = weight;
                this.Id = id;
            }
        }

        public EditNode(Graph graph)
        {
            InitializeComponent();
            this.CurrentGraph = graph.Copy();
        }

        public Graph CurrentGraph { get; set; }
        
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                groupBox1.Enabled = comboBox1.SelectedIndex < 0;
            }
            else
            {
                groupBox1.Enabled = true;
                var node = CurrentGraph.Nodes[CurrentGraph.Nodes.Keys.ElementAt(comboBox1.SelectedIndex)];
                textBox1.Text = node.Name;

                var dl = new BindingList<GraphRowInfo>();
                foreach (var value in CurrentGraph.Nodes)
                {
                    dl.Add(new GraphRowInfo(node.AdjacencyList.ContainsKey(value.Value), value.Value.Name, node.AdjacencyList.ContainsKey(value.Value) ? node.AdjacencyList[value.Value].Cost.ToString() : "", value.Key.ToString()));
                }
                this.dataGridView1.DataSource = dl;
            }
        }

        private void EditNode_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            foreach (var node in CurrentGraph.Nodes)
            {
                comboBox1.Items.Add(node.Value.Name);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                MessageBox.Show("O nome do nó está em branco!", "Erro ao editar nó", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    int weight;
                    if (!int.TryParse(row.Cells[2].Value.ToString(), out weight))
                    {
                        MessageBox.Show("O peso da aresta do nó \"" + row.Cells[1].Value.ToString() + "\" não é um valor numérico válido!",
                            "Erro ao editar nó", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    if (weight < 0)
                    {
                        MessageBox.Show("O peso da aresta do nó \"" + row.Cells[1].Value.ToString() + "\" é negativo e não suportado pelo Algoritmo de Dijkstra!",
                            "Erro ao editar nó", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            var node = CurrentGraph.Nodes[CurrentGraph.Nodes.Keys.ElementAt(comboBox1.SelectedIndex)];

            node.Name = textBox1.Text;

            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                var target = this.CurrentGraph.GetNodeById(int.Parse(row.Cells[3].Value.ToString()));

                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    int weight = int.Parse(row.Cells[2].Value.ToString());

                    if (node.AdjacencyList.ContainsKey(target))
                    {
                        node.AdjacencyList[target].Cost = weight;
                    }
                    else
                    {
                        this.CurrentGraph.AddAdjacency(node, target, weight);
                    }
                }
                else
                {
                    this.CurrentGraph.RemoveAdjacency(node,target);
                }
            }

            comboBox1.SelectedIndex = -1;
            groupBox1.Enabled = false;
            dataGridView1.DataSource = null;
            textBox1.Text = "";
        }
    }
}
