using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mist
{

    public class Globals
    {
        public const string RDSDOMAIN = "mainegamesteam.cbzhynv0adrl.us-east-1.rds.amazonaws.com";
        public const string FTPIP = "169.244.195.143";

        //cant be const because has to be set a runtime.
        //but please don't change it?
        public static string root = "";

        public static MySqlConnection connection = null;
        public static Process process = null;

        //quick thing to call before accessing database commands just to be sure.
        //also, should always call this from UI thread as it opens up a dialog.
        public static void maintainDatabaseConnection()
        {
            //if for some reason that connection goes bad, reconnect it.
            while (Globals.connection == null
                || Globals.connection.State != ConnectionState.Open
                || Globals.connection.IsPasswordExpired)

                new Connect().ShowDialog();
        }

        public static Tab convert(string tab) {
            switch (tab.ToLower())
            {
                case "strore":
                    return Tab.STORE;
                case "library":
                    return Tab.LIBRARY;
                default:
                    return Tab.NOT_SET;
            }
        }


        public static string[] args;

        public static bool hasArg(string arg)
        {
            return args.Contains(arg);
        }

        public static int hash(String str)
        {
            int prime = 164973157;
            int sum = prime;
            foreach (char _char in str.ToCharArray())
                sum = sum * _char + prime;
            return sum;
        }

        /**
         * path starts with /
         */
        public static Stream getFile(string path)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + FTPIP + path);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("mainegamesteam", "mainegamesteam1!");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            return responseStream;
        }

    }

    public enum Tab
    {
        STORE,
        LIBRARY,
        NOT_SET

    }

    //because everything pumps out of here a as a string.
    public class GameContract
    {
        public string id { get; set; }

        public string name { get; set; }

        public string executableName { get; set; }

        public string versionString { get; set; }

        public string zipLength { get; set; }
    }

    public class Game
    {
        public string id;
        public int versionInteger;
        public string version;
        public string name;
        public string executableName;
        public string displayName;
        public int zipLength;

        //then later on in here we do the converting.
        private Game(GameContract contract)
        {
            id = contract.id;
            versionInteger = Int32.Parse(contract.versionString);
            name = contract.name;
            executableName = contract.executableName;
            zipLength = Int32.Parse(contract.zipLength);

            displayName = name.Replace("&", "&&");
            int major = versionInteger / 1000000;
            int minor = (versionInteger - (major * 1000000)) / 10000;
            int build = (versionInteger - (major * 1000000) - (minor * 10000)) / 100;
            int revision = versionInteger - (major * 1000000) - (minor * 10000) - (build * 100);
            version = major + "." + minor + "." + build + "." + revision;
        }

        public static Game getGame(GameContract contract )
        {
            try{
                Game game = new Game(contract);
                return game;
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
