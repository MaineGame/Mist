using MaterialSkin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Mist
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        [STAThread]
        static void Main(String[] args)
        {
            Globals.args = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MaterialSkinManager.Instance.Theme = MaterialSkinManager.Themes.LIGHT;
            Globals.maintainDatabaseConnection();
            Application.Run(new Mist());
            
        }
    }
}