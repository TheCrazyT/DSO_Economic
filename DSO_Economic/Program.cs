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
            MessageBox.Show(i.getBuildingsCSV());
            MessageBox.Show(i.getItemsCSV());
            MessageBox.Show(i.getResourcesCSV());
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DSOEForm mainForm = new DSOEForm();
            mainForm.LoadEnv();
            Application.Run(mainForm);
#endif
        }
    }
}
