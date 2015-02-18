using MaterialSkin;
using MaterialSkin.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mist
{
    public partial class Mist : MaterialForm
    {
        private Tab selecting = Tab.NOT_SET;
        private Tab selected = Tab.NOT_SET;

        private Game[] games = null;

        private MaterialSkinManager manager = MaterialSkinManager.Instance;

        public Mist()
        {

            Globals.root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            //new Download(new Game(contract)).ShowDialog();

            InitializeComponent();
            manager.AddFormToManage(this);
            manager.Theme = MaterialSkinManager.Themes.DARK;

        }

        private void Mist_Load(object sender, EventArgs e)
        {
            switchTabs(Tab.STORE);
        }


        //called upon selecting the store. called from the worker completed thing.
        private void updateListings()
        {
            FlowLayoutPanel mainPanel = new FlowLayoutPanel();
            mainPanel.FlowDirection = FlowDirection.LeftToRight;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Size = materialTabControl1.TabPages[materialTabControl1.TabIndex].Size;
            mainPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            foreach (Game game in games)
            {
                //Console.Beep();

                bool downloaded = File.Exists(Globals.root + "\\games\\" + game.id + "\\" + game.executableName);

                FlowLayoutPanel panel = new FlowLayoutPanel();
                panel.FlowDirection = FlowDirection.LeftToRight;
                panel.Size = new Size(402, 102);
                panel.BorderStyle = BorderStyle.FixedSingle;
                //panel.BackColor = Color.Transparent;

                FlowLayoutPanel textPanel = new FlowLayoutPanel();
                var textPanelMargin = textPanel.Margin;
                textPanelMargin.All = 0;
                textPanel.Margin = textPanelMargin;
                textPanel.Size = new Size(300, 100);
                //textPanel.BorderStyle = BorderStyle.FixedSingle;
                textPanel.FlowDirection = FlowDirection.TopDown;
                //textPanel.BackgroundImage = Image.FromFile("blerp.png");

                MaterialLabel textPanelTopSpacer = new MaterialLabel();
                textPanelTopSpacer.Text = "";
                textPanelTopSpacer.Size = new Size(50, 5);

                textPanel.Controls.Add(textPanelTopSpacer);

                MaterialLabel nameLabel = new MaterialLabel();
                
                nameLabel.Font = new Font(nameLabel.Font.FontFamily, 20, FontStyle.Regular);
                nameLabel.AutoSize = true;
                nameLabel.Text = game.displayName;
                textPanel.Controls.Add(nameLabel);

                MaterialLabel versionLabel = new MaterialLabel();

                versionLabel.Text = "  v" + game.version;

                textPanel.Controls.Add(versionLabel);

                FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
                buttonPanel.FlowDirection = FlowDirection.BottomUp;
                //buttonPanel.BorderStyle = BorderStyle.FixedSingle;
                buttonPanel.Size = new Size(100, 100);
                var buttonPanelMargin = buttonPanel.Margin;
                buttonPanelMargin.All = 0;
                buttonPanel.Margin = buttonPanelMargin;

                buttonPanel.BorderStyle = BorderStyle.FixedSingle;

                MaterialRaisedButton playButton = new MaterialRaisedButton();
                playButton.BackColor = Color.Gray;
                playButton.Text = "Play";
                playButton.Click += delegate(object sender, EventArgs e)
                {
                    openGame(game);
                };
                playButton.Size = new Size(94, 20);
                //playButton.Dock = DockStyle.Right;
                buttonPanel.Controls.Add(playButton);
                if (!downloaded) playButton.Enabled = false;

                if (!downloaded)
                {
                    MaterialRaisedButton installButton = new MaterialRaisedButton();

                    installButton.Text = "Install";
                    installButton.Click += delegate(object sender, EventArgs e)
                    {
                        downloadGame(game);
                    };

                    installButton.Size = new Size(94, 20);
                    //installButton.Dock = DockStyle.Right;
                    buttonPanel.Controls.Add(installButton);
                }
                else
                {
                    MaterialRaisedButton installButton = new MaterialRaisedButton();

                    installButton.Text = "Uninstall";
                    installButton.Click += delegate(object sender, EventArgs e)
                    {
                        downloadGame(game);
                    };

                    installButton.Size = new Size(94, 20);
                    //installButton.Dock = DockStyle.Right;
                    buttonPanel.Controls.Add(installButton);
                }



                panel.Controls.Add(textPanel);
                panel.Controls.Add(buttonPanel);

                mainPanel.Controls.Add(panel);



            }

            materialTabControl1.TabPages[materialTabControl1.TabIndex].Controls.Clear();
            materialTabControl1.TabPages[materialTabControl1.TabIndex].Controls.Add(mainPanel);

            manager.Theme = MaterialSkinManager.Themes.DARK;

        }

        private void downloadGame(Game game)
        {
            new Download(game).ShowDialog();
            switchTabs(selected);
        }

        private void openGame(Game game)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = Globals.root + "\\games\\" + game.id + "\\" + game.executableName;
            process.Start();
        }

        private void switchTabs(Tab tab)
        {
            if (!backgroundWorker1.IsBusy)
            {
                selecting = tab;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //this happens directly after click, before any animation...
        private void materialTabControl1_Selected(object sender, TabControlEventArgs e)
        {
            switchTabs(Tab.STORE);
        }

        //pretty simple method, thought there would be more to this. there wasn't
        //oh well, just in case there is later...
        private void loadStore()
        {
            games = getGamesFromStore();
        }

        //quick thing to call before accessing database commands just to be sure.
        //also, should always call this from UI thread as it opens up a dialog.
        private void maintainDatabaseConnection()
        {
            //if for some reason that connection goes bad, reconnect it.
            while (Globals.connection == null
                || Globals.connection.State != ConnectionState.Open
                || Globals.connection.IsPasswordExpired)

                new Connect().ShowDialog();
        }
        
        private Game[] getGamesFromStore()
        {

            //before we do anything, make sure that we totally have an active connection...
            maintainDatabaseConnection();

            List<Game> games = new List<Game>();

            MySqlCommand command = new MySqlCommand();
            command.CommandText = "Select * from store;";
            command.Connection = Globals.connection;
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                GameContract contract = new GameContract();
                contract.executableName = reader["executableName"].ToString();
                contract.versionString = reader["version"].ToString();
                contract.name = reader["gameName"].ToString();
                contract.id = reader["gameID"].ToString();
                Game game = new Game(contract);
                games.Add(game);
            }

            reader.Close();

            return games.ToArray<Game>();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //primary objective of this method is to load the right games into the games array.

            if (selecting == selected)
            {
                //dont need to load anything new into our games array.
                //this is just a refreshing call.
                return;
            }
            
            //if we want the store, i.e. parameter passed
            if (selecting == Tab.STORE)
            {
                loadStore();
            }
        }

        

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updateListings();
            selected = selecting;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
    }
}
