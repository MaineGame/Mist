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
using System.Data.SqlClient;

namespace Mist
{
    public partial class Mist : MaterialForm
    {
        public Mist()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

        }

        private void Mist_Load(object sender, EventArgs e)
        {

            try
            {
                Console.WriteLine("Connecting...");
                SqlConnection connection = new SqlConnection(
                    "Server=mainegamesteam.cd7espbfum11.us-west-2.rds.amazonaws.com;" +
                    "Database=mainegamesteam;" +
                    "Uid=mainegamesteam;" +
                    "Pwd=mainegamesteam1!;"
                    );
                connection.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("wuddup!");
        }
    }
}
