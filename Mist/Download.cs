using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using MaterialSkin.Controls;
using MaterialSkin;

namespace Mist
{
    public partial class Download : MaterialForm
    {
        //the game to be downloaded. we can construct the url and everything 
        //because all information about a game can be found in its json,
        //which is deserialized at loadStore time and stored in an array of
        //these puppy dogs.
        private Game game;

        //just to track how many bytes we're downloading.
        private long totalBytes = TOTALBYTES_NOT_SET;

        //constants because im not memorizing all the hacks im using
        //to update the ui illegitamently.
        private static int CLOSE_WINDOW = -4;
        private static int STATE_EXTRACTING = -1;
        private static int TOTALBYTES_NOT_SET = -1;
        private static int STATE_SUCCESS = -2;
        private static int STATE_CONNECTING = -3;

        public Download(Game game)
        {
            this.game = game;
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
        }

        private void Download_Load(object sender, EventArgs e)
        {
            materialLabel1.Text = "Click Start to begin downloading " + game.name + ".";
            materialLabel1.AutoSize = true;
            materialLabel2.Text = "";
            materialLabel3.Text = "";
        }

        //this is assumed to be synchronous, when this returns we cant report progress.
        //noting that because at first, i tried to await and had to make this async, but it 
        //always returned before work was done, but yet continued to do its work, just unable
        //to report progress as it thought it was done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (materialRaisedButton1.Text == "Close")
            {
                // this will only be set like this if we already reported success from the 
                // else bit of this conditional.
                // so if so, then we just report a clsoe request and then the ui thread
                // will see it and close the window.
                backgroundWorker1.ReportProgress(CLOSE_WINDOW);
            }
            else
            {
                //this place is a mess and will cause everything to break presently because we just switched to
                //from http to ftp and i haven't transferred this code bit yet.
                #region download and extracting the game data files
                
                //tell the ui we're trying to connect...
                backgroundWorker1.ReportProgress(STATE_CONNECTING);

                //establish a new webclient because downloading is a thing.
                WebClient client = new WebClient();
                
                totalBytes = game.zipLength;
                
                Stream responseFile = Globals.getFile("/games/" + game.id + "/current.zip");
                
                //give the downloader a progress listener. doesn't yell at the ui as much as you think, but nonetheless,
                //it doesn't bother the ui thread, only raises flags for it. so the ui thread only actally recives the update
                //as much as it refreshes itself.
                client.DownloadProgressChanged += delegate(object Object, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
                {
                    //we send it the number of bytes we done got so far
                    if (backgroundWorker1.IsBusy) backgroundWorker1.ReportProgress((int)(downloadProgressChangedEventArgs.BytesReceived));

                };

                Directory.CreateDirectory(Globals.root + "\\games");
                Directory.CreateDirectory(Globals.root + "\\games\\" + game.id);
                
                if (Directory.Exists(Globals.root + "\\games\\" + game.id))
                    //delete the old if you have one
                    Directory.Delete(Globals.root + "\\games\\" + game.id, true);

                //download it
                try
                {
                    client.Credentials = new NetworkCredential(Globals.userName, Globals.password);
                    client.DownloadFileTaskAsync(new Uri("ftp://" + Globals.FTPIP + "/games/" + game.id + "/current.zip"), "" + Globals.root + "\\games\\temp.zip").Wait();
                }
                catch (Exception ex)
                {
                    
                }
                //okay, we good downloading, tell the ui we're extracting now
                backgroundWorker1.ReportProgress(STATE_EXTRACTING);

                
                //then you know, actually start that bit...
                ZipFile.ExtractToDirectory(Globals.root + "\\games\\temp.zip", Globals.root + "\\games\\" + game.id);
                //File.Delete(Globals.root + "\\games\\temp.zip");

                //houston, we're done here.
                backgroundWorker1.ReportProgress(STATE_SUCCESS);
                #endregion

            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progresspercentage is actually the bytes completed

            #region stuff that should apply to every update
            progressBar1.Minimum = 0;
            progressBar1.Maximum = (int)(totalBytes / 1024);
            progressBar1.Value = (int)(e.ProgressPercentage / 1024);

            double percent = (double)(e.ProgressPercentage / 1024) / (double)(totalBytes / 1024);
            //round it to 2 places
            percent *= 10000;
            percent = (int)percent;
            percent /= 100;

            materialLabel2.Text = "" + percent + "%";

            materialLabel3.Text = "" + ((double)(e.ProgressPercentage / (1024 * 1024))) + " MiBs / " + ((double)(totalBytes / (1024 * 1024))) + " MiBs";

            #endregion

            if (e.ProgressPercentage == STATE_EXTRACTING)
            {
                #region extracting update stuff
                materialLabel2.Text = "100%";

                materialLabel3.Text = "" + ((double)(totalBytes / (1024 * 1024))) + " MiBs / " + ((double)(totalBytes / (1024 * 1024))) + " MiBs";
                materialLabel3.AutoSize = true;

                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 20;

                materialLabel1.Text = "Extracting " + game.name + "...";
                #endregion
            }
            else if (e.ProgressPercentage == STATE_SUCCESS)
            {
                #region success stuff
                //when the zip completes
                materialLabel1.Text = "" + game.name + " successfully installed!";
                materialRaisedButton1.Enabled = true;
                materialLabel2.Text = "100%";
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 1;
                progressBar1.Value = 1;
                progressBar1.Style = ProgressBarStyle.Continuous;
                materialRaisedButton1.Text = "Close";
                materialLabel3.Text = "" + ((double)(totalBytes / (1024 * 1024))) + " MiBs / " + ((double)(totalBytes / (1024 * 1024))) + " MiBs";
                #endregion
            }
            else if (e.ProgressPercentage == STATE_CONNECTING)
            {
                #region connecting to server...
                //when the zip completes
                materialLabel1.Text = "Connecting to server...";
                materialRaisedButton1.Enabled = false;
                materialLabel3.Text = "" + ((double)(0 / (1024 * 1024))) + " MiBs / " + ((double)(totalBytes / (1024 * 1024))) + " MiBs";
                materialLabel2.Text = "0%";
                #endregion
            }
            else if (e.ProgressPercentage == CLOSE_WINDOW)
            {
                #region literally just close the window... nothing special
                Close();
                #endregion
            }
            else
            {
                #region during actual progress
                //when the download is donwloading
                materialRaisedButton1.Enabled = false;
                progressBar1.Style = ProgressBarStyle.Continuous;
                materialLabel1.Text = "Downloading " + game.name + "...";
                #endregion
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            //just run the work thread, it'll know what you want to do
            //by reading the text on the only button on the form.
            materialFlatButton1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}