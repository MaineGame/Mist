using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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
            //MaterialSkinManager.Instance.Theme = MaterialSkinManager.Themes.DARK;
            InitializeComponent();
        }
        
        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void Upload_Load(object sender, EventArgs e)
        {
            MaterialSkinManager.Instance.AddFormToManage(this);

        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void browse1(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (File.Exists(openFileDialog1.FileName))
            {
                materialSingleLineTextField1.Text = openFileDialog1.FileName;
            }
        }

        private void browse2(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            if (File.Exists(openFileDialog2.FileName))
            {
                materialSingleLineTextField2.Text = openFileDialog2.FileName;
            }
        }

        private void browse3(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                materialSingleLineTextField4.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            materialRaisedButton4.Text = materialCheckBox1.Checked ? "Update" : "Upload";
        }

        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {

            //just do everything here first bc swag

            //variables for later methodization.
            string backgroundImagePath = openFileDialog1.FileName;
            string executablePath = openFileDialog2.FileName;
            string executableName = executablePath.Substring(executablePath.LastIndexOf("\\") + 1);
            string dataFolderPath = folderBrowserDialog1.SelectedPath;
            string dataFolderName = dataFolderPath.Substring(dataFolderPath.LastIndexOf("\\") + 1);
            string passcode = materialSingleLineTextField5.Text;

            //first copy everything in to a temp directory in root.
            Directory.Delete(Globals.root + "\\temp", true);
            Directory.CreateDirectory(Globals.root + "\\temp");
            File.Copy(backgroundImagePath, Globals.root + "\\temp\\default.jpg");
            File.Copy(executablePath, Globals.root + "\\temp\\" + executableName);
            DirectoryCopy(dataFolderPath, Globals.root + "\\temp\\" + dataFolderName, true);

            ZipFile.CreateFromDirectory(Globals.root + "\\temp\\", Globals.root + "\\current.zip");

            //IAmazonS3 as3;

        }

        //this method is pretty much copy pasta from msdn
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
