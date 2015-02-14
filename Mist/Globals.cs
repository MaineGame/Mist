using System;
using System.Collections.Generic;
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
        public static string DOMAIN = "themainegame.com/games/games";
        public static string root = "";
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
