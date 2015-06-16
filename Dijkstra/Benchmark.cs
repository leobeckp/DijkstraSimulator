using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;


namespace Dijkstra
{
    public partial class Benchmark : Form
    {
        public Benchmark(Graph graph)
        {
            InitializeComponent();
            this.CurrentGraph = graph;
        }
        private Thread ExecTimeCalcThread { get; set; }
        private Graph CurrentGraph { get; set; }
        private void Benchmark_Load(object sender, EventArgs e)
        {
            this.label5.Text = "--";
            this.label6.Text = "--";
            this.label7.Text = "--";
            foreach (var node in CurrentGraph.Nodes)
            {
                comboBox1.Items.Add(node.Value.Name);
            }
        }
        private void CalculateExecTime(object args)
        {
            try
            {
                var data = (KeyValuePair<int, Node>)args;
                int rep = data.Key;

                Stopwatch sw = new Stopwatch();

                sw.Start();
                for (int i = 0; i < rep; i++)
                {
                    this.CurrentGraph.DijkstraDistanceVector(data.Value);
                }
                sw.Stop();
                label5.Invoke((MethodInvoker)(() => label5.Text = (sw.ElapsedMilliseconds/(double)rep)+" ms"));

                sw.Reset();
                sw.Start();
                for (int i = 0; i < rep; i++)
                {
                    this.CurrentGraph.DijkstraFibonacciHeap(data.Value);
                }
                sw.Stop();
                label6.Invoke((MethodInvoker)(() => label6.Text = (sw.ElapsedMilliseconds / (double)rep) + " ms"));

                sw.Reset();
                sw.Start();
                for (int i = 0; i < rep; i++)
                {
                    this.CurrentGraph.DijkstraBinary(data.Value);
                }
                sw.Stop();
                label7.Invoke((MethodInvoker)(() => label7.Text = (sw.ElapsedMilliseconds / (double)rep) + " ms"));

                button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
            }
            catch(Exception e)
            {
                MessageBox.Show(
                   "Não foi possível calcular os tempos de execução. MOTIVO: " + e.Message + "\r\n\r\nDetalhes do erro: " +
                   e.StackTrace, "Erro ao calcular tempos de execução.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int rep;

            if(!int.TryParse(textBox1.Text, out rep) || rep <= 0)
            {
                MessageBox.Show("O número de repetições não é um valor numérico válido.", "Erro ao calcular tempo de execução", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Você deve selecionar o nó de início.", "Erro ao calcular tempo de execução",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var start = this.CurrentGraph.Nodes[this.CurrentGraph.Nodes.Keys.ToArray()[comboBox1.SelectedIndex]];

            button1.Enabled = false;
            this.label5.Text = "Calculando...";
            this.label6.Text = "Calculando...";
            this.label7.Text = "Calculando...";
            ExecTimeCalcThread = new Thread(CalculateExecTime);
            ExecTimeCalcThread.Start(new KeyValuePair<int, Node>(rep, start));
        }


    }
}
