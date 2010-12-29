using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
namespace DSO_Economic
{
    public partial class FixForm : Form
    {
        public FixForm()
        {
            InitializeComponent();
        }

        private void FixForm_Load(object sender, EventArgs e)
        {
            items.DataSource = DSO_Economic.DSOEForm.itemEntries;
            items.DisplayMember = "Amount";
            items.ValueMember = "ID";

            items.BindingContext[DSO_Economic.DSOEForm.itemEntries].SuspendBinding();
            items.BindingContext[DSO_Economic.DSOEForm.itemEntries].ResumeBinding();
        }

        private void items_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = items.SelectedIndex;
            if (idx == -1) return;
            if (idx == 0) return;
            items.BindingContext[DSO_Economic.DSOEForm.itemEntries].SuspendBinding();
            for (int i = 1; i < idx; i++)
                DSO_Economic.DSOEForm.itemEntries.Remove(DSO_Economic.DSOEForm.itemEntries[0]);

            for (int i = 0; i < DSO_Economic.DSOEForm.itemEntries.Count; i++)
                DSO_Economic.DSOEForm.itemEntries[i].setID((uint)i);
                
                
            this.Close();
        }


    }
}
