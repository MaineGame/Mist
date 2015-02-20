using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mist
{
    public partial class Upload : MaterialForm
    {
        public Upload()
        {
            MaterialSkinManager.Instance.AddFormToManage(this);
            InitializeComponent();
        }
        
        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void Upload_Load(object sender, EventArgs e)
        {

        }
    }
}
