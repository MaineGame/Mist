using MaterialSkin;
using MaterialSkin.Controls;
using MySql.Data.MySqlClient;
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
    public partial class Connect : MaterialForm
    {
        public Connect()
        {
            InitializeComponent();
            MaterialSkinManager manager = MaterialSkinManager.Instance;
            manager.AddFormToManage(this);
            manager.Theme = MaterialSkinManager.Themes.DARK;
        }

        private void Connect_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 20;
            backgroundWorker1.RunWorkerAsync();
        }

        private void connect() {
            try
            {
                Console.WriteLine("Connecting...");
                Globals.connection = new MySqlConnection(
                    "Server=mainegamesteam.cd7espbfum11.us-west-2.rds.amazonaws.com;" +
                    "Database=mainegamesteam;" +
                    "Uid=mainegamesteam;" +
                    "Pwd=mainegamesteam1!;"
                    );
                Globals.connection.Open();
                Console.WriteLine("wuddup!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Globals.connection = null;
            }
        }

        private static int DONE = -1;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Globals.connection == null) connect();
            backgroundWorker1.ReportProgress(DONE);

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Close();
        }
    }
}
