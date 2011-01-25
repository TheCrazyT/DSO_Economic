using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
namespace DSO_Economic
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if VBAInterfaceTest
            DSOE_VBAInterface i=new DSOE_VBAInterface();
            MessageBox.Show(i.getcsv());
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DSOEForm());
#endif
        }
    }
}
