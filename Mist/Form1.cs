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
using System.Net;
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

            InitializeComponent();
            manager.AddFormToManage(this);
        }

        private void Mist_Load(object sender, EventArgs e)
        {
            materialLabel1.Text = "";
            WindowState = FormWindowState.Normal;
            if (Globals.hasArg("-K"))
                WindowState = FormWindowState.Maximized;

            switchTabs(Tab.STORE);
        }


        //called upon selecting the store. called from the worker completed thing.
        private void updateListings()
        {
            FlowLayoutPanel mainPanel = new FlowLayoutPanel();
            mainPanel.FlowDirection = FlowDirection.LeftToRight;
            //mainPanel.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Size = materialTabControl1.TabPages[materialTabControl1.TabIndex].Size;
            mainPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            foreach (Game game in games)
            {
                //Console.Beep();

                bool downloaded = File.Exists(Globals.root + "\\games\\" + game.id + "\\" + game.executableName);

                FlowLayoutPanel panel = new FlowLayoutPanel();
                panel.FlowDirection = FlowDirection.LeftToRight;
                panel.Size = new Size(400, 100);
                //panel.BorderStyle = BorderStyle.FixedSingle;
                //panel.BackColor = Color.Transparent;

                FlowLayoutPanel textPanel = new FlowLayoutPanel();
                
                var textPanelMargin = textPanel.Margin;
                textPanelMargin.All = 0;
                textPanel.Margin = textPanelMargin;
                textPanel.Size = new Size(300, 100);
                textPanel.FlowDirection = FlowDirection.TopDown;

                MaterialLabel nameLabel = new MaterialLabel();
                
                nameLabel.Font = new Font(nameLabel.Font.FontFamily, 20, FontStyle.Regular);
                nameLabel.AutoSize = true;
                nameLabel.Text = game.displayName;
                var nameMargin = nameLabel.Margin;
                nameMargin.Top = 10;
                nameLabel.Margin = nameMargin;

                MaterialLabel versionLabel = new MaterialLabel();

                versionLabel.Text = "v" + game.version;

                try
                {
                    textPanel.BackgroundImage = Image.FromStream(Globals.getFile("/games/" + game.id + "/default.jpg"));
                }
                catch (Exception e)
                {
                    textPanel.BackgroundImage = Image.FromStream(Globals.getFile("/games/default.jpg"));
                    textPanel.Controls.Add(nameLabel);
                    textPanel.Controls.Add(versionLabel);
                }

                FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
                buttonPanel.FlowDirection = FlowDirection.BottomUp;
                //buttonPanel.BorderStyle = BorderStyle.FixedSingle;
                buttonPanel.Size = new Size(100, 100);
                var buttonPanelMargin = buttonPanel.Margin;
                buttonPanelMargin.All = 0;
                buttonPanel.Margin = buttonPanelMargin;
                //buttonPanel.BorderStyle = BorderStyle.FixedSingle;
                Size buttonSize = new Size(93, 20);

                if (!downloaded)
                {
                    MaterialRaisedButton installButton = new MaterialRaisedButton();

                    installButton.Text = "Install";
                    installButton.Click += delegate(object sender, EventArgs e)
                    {
                        downloadGame(game);
                    };

                    installButton.Size = buttonSize;
                    //installButton.Dock = DockStyle.Right;
                    buttonPanel.Controls.Add(installButton);
                }
                else
                {
                    MaterialRaisedButton installButton = new MaterialRaisedButton();

                    installButton.Text = "Uninstall";
                    installButton.Click += delegate(object sender, EventArgs e)
                    {
                        uninstall(game);
                    };

                    installButton.Size = buttonSize;
                    //installButton.Dock = DockStyle.Right;
                    buttonPanel.Controls.Add(installButton);

                    MaterialRaisedButton playButton = new MaterialRaisedButton();
                    playButton.BackColor = Color.Gray;
                    playButton.Text = "Play";
                    playButton.Click += delegate(object sender, EventArgs e)
                    {
                        openGame(game);
                    };
                    playButton.Size = buttonSize;
                    buttonPanel.Controls.Add(playButton);
                }



                panel.Controls.Add(textPanel);
                panel.Controls.Add(buttonPanel);

                mainPanel.Controls.Add(panel);



            }

            materialTabControl1.TabPages[materialTabControl1.TabIndex].Controls.Clear();
            materialTabControl1.TabPages[materialTabControl1.TabIndex].Controls.Add(mainPanel);

        }

        private void uninstall(Game game)
        {
            new Uninstall(game).ShowDialog();
            switchTabs(selected);
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
            switchTabs(Globals.convert(e.TabPage.Text));
        }

        //pretty simple method, thought there would be more to this. there wasn't
        //oh well, just in case there is later...
        private void loadStore()
        {
            games = getGamesFromStore();
        }

        private Game[] getGamesFromStore()
        {
            //TODO if this ever ACTUALLY tries to open up a dialog, it will fail because
            //materialskin and cross threadin even nastier than winforms cross threading.
            Globals.maintainDatabaseConnection();

            List<Game> games = new List<Game>();

            MySqlCommand command = new MySqlCommand();
            command.CommandText = "Select * from store;";
            command.Connection = Globals.connection;
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                GameContract contract = new GameContract {
                    executableName = reader["executable"].ToString(), 
                    versionString = reader["gameVersion"].ToString(),
                    name = reader["gameName"].ToString(),
                    id = reader["gameID"].ToString(),
                    zipLength = reader["zipLength"].ToString()//TODO passcode
                };
                Game game = Game.getGame(contract);
                if(contract != null)
                    games.Add(game);
            }

            reader.Close();

            return games.ToArray<Game>();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //primary objective of this method is to load the right games into the games array.

            //leaving this here to alert future me that thisis a BAD IDEA.
            //i know it seems like a good idea, but picture this:
            //you open up the store and just moments after a new game title is added.
            //in order ti get that title, you immediately refresh the page. but that
            //doesn't work as you are never actually contacting the server.
            //this should only ever happen if the array is not set to change.
            //and the only time that would make any sense is when you install a game and simply
            //need to reload the screen to enble the play button.
            if (selecting == selected)
            {
                //dont need to load anything new into our games array.
                //this is just a refreshing call.
                //return;
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

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            new Upload().ShowDialog();
            switchTabs(selected);
        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
