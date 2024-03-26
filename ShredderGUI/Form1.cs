using System;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Threading;
using System.IO.MemoryMappedFiles;

namespace ShredderGUI
{
    public partial class Form1 : Form
    {
        string filePath = string.Empty;
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

        private void button1_Click(object sender, EventArgs e)
        {
            using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                int BUFFER_SIZE = (int)stream.Length;
                using(var rng = new RNGCryptoServiceProvider())
                {
                    for(int i = 1; i <= int.Parse(textBox2.Text); i++)
                    {
                        byte[] randomData = new byte[BUFFER_SIZE];
                        rng.GetBytes(randomData);
                        listBox1.Items.Add($"Shredding {i}/{textBox2.Text} (" + Convert.ToBase64String(randomData).Substring(0, 10) + ")");
                        stream.Write(randomData, 0, randomData.Length);
                    }
                }
            }
            /*//https://stackoverflow.com/questions/57017270/unable-to-read-video-file-greater-than-2gb
            int BUFFER_SIZE = File.ReadAllBytes(filePath).Length;
            using(var rng = new RNGCryptoServiceProvider())
            {
                for(int i = 1; i <= int.Parse(textBox2.Text); i++)
                {
                    byte[] randomData = new byte[BUFFER_SIZE];
                    rng.GetBytes(randomData);
                    listBox1.Items.Add($"Shredding {i}/{textBox2.Text} (" + Convert.ToBase64String(randomData).Substring(0, 10) + ")");
                    File.WriteAllBytes(filePath, randomData);
                }
            }*/
            if(checkBox1.Checked)
            {
                string dirPath = Path.GetDirectoryName(filePath) + "\\";
                for (int i = filePath.Length; i > 0; i--)
                {
                    string final = dirPath;
                    for (int j = 0; j < i; j++) final += "0";
                    File.Move(filePath, final);
                    filePath = final;
                    listBox1.Items.Add("File renamed to " + final);
                }
                File.Delete(filePath);
                listBox1.Items.Add(filePath + " The file was removed successfully");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                listBox1.Items.Add("File selected => " + filePath);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) listBox1.Items.Add("The file will be deleted");
            else listBox1.Items.Add("The file won't be deleted");
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0) listBox1.Items.Add("Number of iterations => " + textBox2.Text);
        }
    }
}
