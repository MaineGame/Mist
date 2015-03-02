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
            Console.WriteLine(Globals.hash("mainegamedefaultpassword"));
            
            Globals.args = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MaterialSkinManager.Instance.Theme = MaterialSkinManager.Themes.DARK;
            Globals.maintainDatabaseConnection();
            Application.Run(new Mist());
            
        }
    }
}