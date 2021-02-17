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
                toolStripStatusLabel2.Text = "Идет конвертация...";
                folderBrowserDialog1.ShowDialog();
                string path = folderBrowserDialog1.SelectedPath;

                File.WriteAllText(path + @"//External.xsd", Resource1.External);
                File.WriteAllText(path + @"//balance.xsd", Resource1.balance);
                File.WriteAllText(path + @"//financialActivityPlan.xsd", Resource1.financialActivityPlan);
                File.WriteAllText(path + @"//Types.xsd", Resource1.Types);
                string fileShema = path + @"//External.xsd";

                string[] files = Directory.GetFiles(path, "*.txt");
                toolStripProgressBar1.Minimum = toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = files.Length;
                foreach(string file in files)
                {
                    string[] lines = File.ReadAllLines(file, Encoding.GetEncoding(1251));
                    string nameXmlFile = file.Replace(".txt", ".xml");
                    listBox1.Items.Add(Convert.Converter(lines, nameXmlFile, fileShema));
                    toolStripProgressBar1.PerformStep();
                }
                toolStripStatusLabel2.Text = "Конвертация выполнена!";
                button1.Enabled = false;

                File.Delete(path + @"//External.xsd");
                File.Delete(path + @"//balance.xsd");
                File.Delete(path + @"//financialActivityPlan.xsd");
                File.Delete(path + @"//Types.xsd");
            }
            catch { MessageBox.Show("Ошибка конвертации!", "Ошибка"); }
        }
    }
}
