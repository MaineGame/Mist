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

/*

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniSteam
{
    public partial class Form1 : Form
    {

        private string STORE = "Store";
        private string LIBRARY = "Library";

        private string selected;
        private string selecting;

        private List<Label> tabs;

        private void switchTabs(string tab)
        {
            if (!backgroundWorker1.IsBusy)
            {
                selecting = tab;
                updateButtons();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void updateButtons()
        {
            foreach (Label label in tabs)
            {
                if (selecting == label.Text)
                {
                    label.Font = new Font(label.Font, FontStyle.Underline);
                    label.ForeColor = Color.Black;
                }
                else
                {
                    label.Font = new Font(label.Font, FontStyle.Regular);
                    label.ForeColor = Color.DimGray;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            tabs = new List<Label>();
            tabs.Add(label3);
            tabs.Add(label4);

            switchTabs(STORE);

            /*  host = "mainegamesteam.cd7espbfum11.us-west-2.rds.amazonaws.com",
  user = "mainegamesteam"
  passwd = "mainegamesteam1!"
  db = "mainegamesteam"
*/
            /*
            try
            {
                string username = "mainegamesteam";
                string password = "mainegamesteam1!";
                string url = "mainegamesteam.cd7espbfum11.us-west-2.rds.amazonaws.com";
                string database = "mainegamesteam";
                string connectionString = 
                    
                    "user id=" + username + ";" +
                    "Pwd=" + password + ";" +
                    "Server=" + url + ";" +
                    "Database=" + database + "" + 
                    "";

                
                SqlConnection connection = new SqlConnection(connectionString);
                

                Console.WriteLine("connecting...");
                connection.Open();

                Console.WriteLine("Success!");
                Console.WriteLine("Sending command...");
                SqlCommand command = new SqlCommand("SELECT * FROM store");
                command.Connection = connection;

                Console.WriteLine("Recieved response!");
                SqlDataReader reader = command.ExecuteReader();

                //getting lines and shoving them into the transaction, then returning 
                //true if it had another or false if that was the end...
                Console.WriteLine("Reading...");
                while (reader.Read())
                {
                    Console.WriteLine(reader.ToString());
                }
                Console.WriteLine("Done!");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Close();
            }
             * */
        }

        //File Transfer Successful

        private void Button1_Click(object sender, EventArgs e)
        {
            new Upload().ShowDialog();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        /*
         * Your Server Name:	mikepreble.powwebmysql.com
         * Database Name:       mainegamesteam
         * Database Username:	mainegamesteam
         * password:            mainegamesteam1!
         * 
         */



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (selecting == STORE)
                if (selected == STORE) refreshStore();
                else loadStore();
            else if (selecting == LIBRARY)
                if (selected == LIBRARY) refreshLibrary();
                else loadLibrary();
        }

        private void refreshStore()
        {
            // well actually, we only need this to load new data so....
            //for now nothing goes here but you know, the placeholding
            //is real...
        }

        private void refreshLibrary()
        {
            //just dont even
        }

        private void loadLibrary()
        {
            //yeah noooo
        }


        private Game[] games = new Game[0];
        private const int CONNECTING = -1;
        private const int FAILED = -2;

        private void loadStore()
        {
            try
            {
                //get a list of games we have.
                games = getGamesFromServer();
            }
            catch (Exception e)
            {
                backgroundWorker1.ReportProgress(FAILED);

            }
        }

        private Game[] getGamesFromServer()
        {
            backgroundWorker1.ReportProgress(CONNECTING);
            //create an empty list of game objects.
            List<Game> games = new List<Game>();

            string pythonresponse = getPythonResponse("search", "");

            //L L L L L L L L L L L L L LLOOOOP DA LINES
            string[] lines = pythonresponse.Split('\n');
            int i = 0;
            foreach (string id in lines)
            {
                i++;
                Game game = new Game(getGameContract(id));

                games.Add(game);
                backgroundWorker1.ReportProgress((int)((i / (double)lines.Length) * 100d));

            }

            return games.ToArray<Game>();
        }

        public static string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;

        public static string exeFolder = exePath.Substring(0, exePath.LastIndexOf("\\"));

        private static string getPythonResponse(string script, string args)
        {
            string _return = null;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = script + ".exe";
            startInfo.WorkingDirectory = exeFolder;
            startInfo.Arguments = " " + args;
            startInfo.CreateNoWindow = true;

            int pid = new Random().Next(100, 999);

            StringBuilder builder = new StringBuilder();

            startInfo.RedirectStandardOutput = true;
            process.ErrorDataReceived += delegate(object o, DataReceivedEventArgs e)
            {
                Console.WriteLine("[" + pid + " Python err] " + e.Data);
            };


            startInfo.RedirectStandardError = true;
            process.OutputDataReceived += delegate(object o, DataReceivedEventArgs e)
            {

                Console.WriteLine("[" + pid + " Python out] " + e.Data);
                builder.Append(e.Data + "\n");
                if (e.Data == "end")
                {
                    _return = builder.ToString();

                }

            };

            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();
            return _return.Substring(0, _return.Length - 5);
        }

        private static GameContract getGameContract(string id)
        {
            return JsonConvert.DeserializeObject<GameContract>(getPythonResponse("getGame", id));
        }

        public static string root;

        private void addGameListing(Game game)
        {
            bool downloaded = File.Exists(root + "\\games\\" + game.id + "\\" + game.executableName);

            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.LeftToRight;
            panel.Size = new Size(383, 102);
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.Transparent;

            FlowLayoutPanel textPanel = new FlowLayoutPanel();
            var textPanelMargin = textPanel.Margin;
            textPanelMargin.All = 0;
            textPanel.Margin = textPanelMargin;
            textPanel.Size = new Size(300, 100);
            //textPanel.BorderStyle = BorderStyle.FixedSingle;
            textPanel.FlowDirection = FlowDirection.TopDown;
            textPanel.BackgroundImage = Image.FromFile("blerp.png");

            Label textPanelTopSpacer = new Label();
            textPanelTopSpacer.Text = "";
            textPanelTopSpacer.Size = new Size(50, 5);

            textPanel.Controls.Add(textPanelTopSpacer);

            Label nameLabel = new Label();
            nameLabel.Font = new Font("Arial", 19);
            nameLabel.AutoSize = true;
            nameLabel.Text = game.displayName;
            textPanel.Controls.Add(nameLabel);

            Label versionLabel = new Label();

            versionLabel.Text = "  v" + game.version;

            textPanel.Controls.Add(versionLabel);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.FlowDirection = FlowDirection.BottomUp;
            //buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            buttonPanel.Size = new Size(81, 100);
            var buttonPanelMargin = buttonPanel.Margin;
            buttonPanelMargin.All = 0;
            buttonPanel.Margin = buttonPanelMargin;


            Button playButton = new Button();
            playButton.Text = "Play";
            playButton.Click += delegate(object sender, EventArgs e)
            {
                openGame(game);
            };
            playButton.Dock = DockStyle.Right;
            buttonPanel.Controls.Add(playButton);
            if (!downloaded) playButton.Enabled = false;

            if (!downloaded)
            {
                Button installButton = new Button();

                installButton.Text = "Install";
                installButton.Click += delegate(object sender, EventArgs e)
                {
                    downloadGame(game);
                };

                installButton.Dock = DockStyle.Right;
                buttonPanel.Controls.Add(installButton);
            }
            else
            {
                Button installButton = new Button();

                installButton.Text = "Uninstall";
                installButton.Click += delegate(object sender, EventArgs e)
                {
                    downloadGame(game);
                };

                installButton.Dock = DockStyle.Right;
                buttonPanel.Controls.Add(installButton);
            }



            panel.Controls.Add(textPanel);
            panel.Controls.Add(buttonPanel);

            flowLayoutPanel1.Controls.Add(panel);


            /*
            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.TopDown;
            panel.Size = new Size(400, 100);
            Button download = new Button();
            Console.WriteLine(root);
            download.BackColor = Color.Transparent;



            download.Text = downloaded ? "Play Now!" : "Click here to download.";

            if (!downloaded)
            {
                download.Click += delegate(object sender, EventArgs e)
                {
                    Download downloadWindow = new Download(game);

                    //very sync call
                    downloadWindow.ShowDialog();

                    switchTabs(selected);

                };
            }
            else
            {
                download.Click += delegate(object sender, EventArgs e)
                {
                    Process process = new Process();
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = root + "\\games\\" + game.id + "\\" + game.executableName;
                    process.Start();

                };
            }
            download.AutoSize = true;

            Label title = new Label();
            title.Font = new Font("Arial", 20);
            title.Text = game.name.Replace("&", "&&");
            title.AutoSize = true;
            panel.Controls.Add(title);

            Label versionLabel = new Label();
            */
            /*
            int major = game.version / 1000000;
            int minor = (game.version / 10000) - (major * 10000);
            int build = (game.version / 100) - (minor * 100) - (major * 10000);
            int revision = (game.version / 1) - (build * 100) - (minor * 10000) - (major * 1000000);
            
            versionLabel.Text = "v" + major + "." + minor + "." + build + "." + revision;
            
            /*
            versionLabel.Text = game.versionString;

            versionLabel.AutoSize = true;
            versionLabel.Dock = DockStyle.Right;

            panel.Controls.Add(versionLabel);
            panel.Controls.Add(download);

            var padding = panel.Padding;
            //padding.All = 10;
            panel.Padding = padding;
            panel.BorderStyle = BorderStyle.FixedSingle;
            



        }

        private void downloadGame(Game game)
        {
            Download downloadWindow = new Download(game);

            //very sync call
            downloadWindow.ShowDialog();

            switchTabs(selected);
        }

        private void openGame(Game game)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = root + "\\games\\" + game.id + "\\" + game.executableName;
            process.Start();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case CONNECTING:
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    progressBar1.MarqueeAnimationSpeed = 20;
                    label5.Text = "Connecting...";
                    break;
                default:
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = e.ProgressPercentage;
                    label5.Text = "Downloading data...";
                    break;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO
            if (games.Length != 0)
            {
                flowLayoutPanel1.Controls.Clear();

                foreach (Game game in games) addGameListing(game);

                label5.Text = "Done!";
            }
            else
            {
                //this means we goofed...


                //make sure that when we try again, it wont just refresh. basically,
                //make sure the system know that we did not succeed in selecting a page
                selecting = "";
                label5.Text = "Connection Failed!";
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 0;
            }

            // we will always get to the place we wanted to go. unless we reroute that place
            //to an empty string, then well... whoops...
            selected = selecting;

        }

        private void label4_MouseClick(object sender, MouseEventArgs e)
        {
            switchTabs(((Label)sender).Text);
        }

    }

    public class GameContract
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "executableName")]
        public string executableName { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string versionString { get; set; }
    }

    public class Game
    {

        public string id;
        public string versionString;
        public string version;
        public string name;
        public string executableName;
        public string displayName;

        public Game(GameContract contract) {
            id = contract.id;
            versionString = contract.versionString;
            name = contract.name;
            executableName = contract.executableName;


            displayName = name.Replace("&", "&&");
            string major = "" + Int32.Parse(versionString.Substring(0, 2));
            string minor = "" + Int32.Parse(versionString.Substring(2, 2));
            string build = "" + Int32.Parse(versionString.Substring(4, 2));
            string revision = "" + Int32.Parse(versionString.Substring(6, 2));
            version = major + "." + minor + "." + build + "." + revision;
        } 

    }

}

*/