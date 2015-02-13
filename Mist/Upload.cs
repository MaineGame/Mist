using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniSteam
{
    public partial class Upload : Form
    {
        public Upload()
        {
            InitializeComponent();
        }
        
        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            if (openFileDialog1.CheckFileExists)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();

            if (openFileDialog2.CheckFileExists)
            {
                textBox2.Text = openFileDialog2.FileName;
            }
        }

        private void Upload_Load(object sender, EventArgs e)
        {
            label6.Text = "";
        }
    }
}
