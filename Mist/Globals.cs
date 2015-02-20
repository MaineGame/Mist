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
        public static string DOMAIN = "https://s3-us-west-2.amazonaws.com/mainegamesteam";
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

    }

    public enum Tab
    {
        STORE,
        LIBRARY,
        NOT_SET

    }

    public class GameContract
    {
        public string id { get; set; }

        public string name { get; set; }

        public string executableName { get; set; }

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

        public Game(GameContract contract)
        {
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
