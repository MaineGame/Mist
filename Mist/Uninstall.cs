using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mist
{
    public partial class Uninstall : MaterialForm
    {
        private Game game;

        public Uninstall(Game game)
        {
            this.game = game;
            MaterialSkinManager.Instance.AddFormToManage(this);
            InitializeComponent();
        }

        private void Uninstall_Load(object sender, EventArgs e)
        {
            materialLabel1.Text = "Uninstall " + game.name + "?";
        }

        private State state = State.INSTALLED;

        private enum State
        {
            INSTALLED,
            UNINSTALLING,
            UNINSTALLED
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            if (state == State.INSTALLED)
                backgroundWorker1.RunWorkerAsync();
            else if (state == State.UNINSTALLED)
                Close();
        }

        private static int DONE = -1;
        private static int UNINSTALLING = -1;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(UNINSTALLING);
            Directory.Delete(Globals.root + "\\games\\" + game.id + "\\", true);
            backgroundWorker1.ReportProgress(DONE);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == DONE)
            {
                materialLabel1.Text = "" + game.name + " Successfully uninstalled!";
                materialRaisedButton1.Enabled = true;
                materialRaisedButton1.Text = "Close";
                state = State.UNINSTALLED;
            }
            else if (e.ProgressPercentage == UNINSTALLING)
            {
                materialRaisedButton1.Enabled = false;
                materialLabel1.Text = "Uninstalling " + game.name + "...";
                state = State.UNINSTALLING;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
