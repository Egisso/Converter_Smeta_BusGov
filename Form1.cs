using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Converter_Smeta_BusGov
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                toolStripStatusLabel2.Text = "Идет конвертация...";
                folderBrowserDialog1.ShowDialog();
                string path = folderBrowserDialog1.SelectedPath;
                stopWatch.Start();
                string[] files = Directory.GetFiles(path, "*.txt");
                toolStripProgressBar1.Minimum = toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = files.Length;

                foreach(string file in files)
                {
                    string[] lines = File.ReadAllLines(file, Encoding.GetEncoding(1251));
                    string nameXmlFile = file.Replace(".txt", ".xml");
                    Convert.Converter(lines, nameXmlFile);
                    toolStripProgressBar1.PerformStep();
                }
                toolStripStatusLabel2.Text = "Конвертация выполнена!";
                button1.Enabled = false;
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                label3.Text = $"Обработано файлов {files.Length} за {ts.TotalSeconds} секунд.";
                label3.Visible = true;
            }
            catch { MessageBox.Show("Ошибка конвертации!", "Ошибка"); }
        }
    }
}
