using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace ShredderGUI
{
    public partial class Form1 : Form
    {
        //string filePath = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            for(int i = 0; i < textBox2.Text.Length; i++)
            {
                if (textBox2.Text[i] < '0' || textBox2.Text[i] > '9')
                {
                    textBox2.Text = textBox2.Text.Remove(i, 1);
                }
            }
        }

        private void WriteData(int times)
        {
            foreach(string file in openFileDialog1.FileNames)
            {
                string filePath = file;
                if (filePath == string.Empty)
                {
                    MessageBox.Show("File was not selected");
                    return;
                }
                int BUFFER_SIZE = File.ReadAllBytes(filePath).Length;
                using (var rng = new RNGCryptoServiceProvider())
                {
                    for (int i = 1; i <= times; i++)
                    {
                        byte[] randomData = new byte[BUFFER_SIZE];
                        rng.GetBytes(randomData);
                        Invoke((MethodInvoker)(() => listBox1.Items.Insert(0, $"Shredding {i}/{times} (" + Convert.ToBase64String(randomData).Substring(0, 10) + ")")));
                        File.WriteAllBytes(filePath, randomData);
                    }
                }

                if (checkBox1.Checked)
                {
                    string dirPath = Path.GetDirectoryName(filePath) + "\\";
                    for (int i = filePath.Length; i > 0; i--)
                    {
                        string final = dirPath;
                        for (int j = 0; j < i; j++) final += "0";
                        File.Move(filePath, final);
                        filePath = final;
                        Invoke((MethodInvoker)(() => listBox1.Items.Insert(0, "File renamed to " + final)));
                    }
                    File.Delete(filePath);
                    Invoke((MethodInvoker)(() => listBox1.Items.Insert(0, filePath + " The file was removed successfully")));
                }
                Invoke((MethodInvoker)(() => listBox1.Items.Insert(0, "Finished!!!")));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(() => WriteData(int.Parse(textBox2.Text)));
            th.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach(string filePath in openFileDialog1.FileNames)
                    listBox1.Items.Insert(0, "File selected => " + filePath);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) listBox1.Items.Insert(0, "The file will be deleted");
            else listBox1.Items.Insert(0, "The file won't be deleted");
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0) listBox1.Items.Insert(0, "Number of iterations => " + textBox2.Text);
        }
    }
}