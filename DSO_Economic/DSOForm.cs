using System.Configuration;
using System.Data.Odbc;
using System.Data.ProviderBase;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ZedGraph;
using System.Xml.Serialization;

namespace DSO_Economic
{

    public partial class DSOEForm : Form
    {


        public DSOEForm()
        {
            InitializeComponent();
        }
        private void disconnected()
        {
            BuildingRefresh.Enabled = false;
            TimeLeft.Enabled = false;
            ItemRefresh.Enabled = false;
            status.Text = "Verbindung unterbrochen!";
            btn_reconnect.Visible = true;
        }
        


        private void refreshItemList()
        {

            int idx = items.SelectedIndex;
            items.BindingContext[Global.itemEntries].SuspendBinding();
            items.BindingContext[Global.itemEntries].ResumeBinding();
            items.SelectedIndex = idx;


            Global.resourceEntries.Sort(CResourceEntry.SortByAmount);
            object idx2 = resources.SelectedItem;
            resources.BindingContext[Global.resourceEntries].SuspendBinding();
            resources.BindingContext[Global.resourceEntries].ResumeBinding();
            resources.SelectedItem = idx2;
        }

        private void CreateGraph(ZedGraphControl zgc, string title, PointPairList ppl1, PointPairList ppl2)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.Title.Text = title;
            myPane.XAxis.Title.Text = "Datum/Uhrzeit";
            myPane.XAxis.Type = AxisType.Date;
            myPane.XAxis.Scale.MajorUnit = DateUnit.Hour;
            myPane.XAxis.Scale.MinorUnit = DateUnit.Minute;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 1;

            myPane.YAxis.Title.Text = "Anzahl";

            LineItem myCurve1 = myPane.AddCurve(title, ppl1, Color.Blue,
                              SymbolType.Circle);
            myCurve1.Line.Fill = new Fill(Color.White, Color.Red, 45F);
            myCurve1.Symbol.Fill = new Fill(Color.White);

            LineItem myCurve2 = myPane.AddCurve("Prognose", ppl2, Color.Green,
                              SymbolType.None);
            myCurve2.Line.Fill = new Fill(Color.White, Color.Gold, 45F);
            myCurve2.Symbol.Fill = new Fill(Color.White);

            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            zgc.AxisChange();
        }
        private void DSOEForm_Load(object sender, EventArgs e)
        {
            #region init
            Global.isLinux=System.Environment.OSVersion.Platform.Equals(System.PlatformID.Unix);
            status.Text = "";
            btn_reconnect.Visible = false;
            this.Visible = false;
            this.Text = "DSO Economic Version " + System.Configuration.ConfigurationManager.AppSettings["Version"];
            string[] args = Environment.GetCommandLineArgs();
            uint h = 0;

            #region ParamChecks
            foreach (string arg in args)
            {
                if (arg == "/buildingsonly")
                {
                    Params.buildingsonly = true;
                }
                if (arg == "/usecustomdb")
                {
                    Params.usecustomdb = true;
                }
                if (arg == "/usesqlitedb")
                {
                    Params.usesqlite = true;
                }
                if ((arg == "/usetxt") || (Params.usetxt))
                {
                    Params.usetxt = true;
                    Global.tblext = ".txt";
                }
                if (arg == "/timer")
                {
                    if (args.Length > h + 1)
                    {
                        try
                        {
                            ItemRefresh.Interval = int.Parse(args[h + 1]);
                        }
                        catch (Exception e2)
                        {
                            MessageBox.Show("Unbekannte Eingabe:" + args[h + 1], "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            this.Dispose();
                        }
                    }
                }

                if (arg == "/mms")
                {
                    if (args.Length > h + 1)
                    {
                        try
                        {
                            Params.maxmemsize = uint.Parse(args[h + 1]);
                        }
                        catch (Exception e2)
                        {
                            MessageBox.Show("Unbekannte Eingabe:" + args[h + 1], "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            this.Dispose();
                        }
                    }
                }
                if (arg == "/mso")
                {
                    if (args.Length > h + 1)
                    {
                        try
                        {
                            Params.maxsearchoffset = uint.Parse(args[h + 1]);
                        }
                        catch (Exception e2)
                        {
                            MessageBox.Show("Unbekannte Eingabe:" + args[h + 1], "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            this.Dispose();
                        }
                    }
                }
                if (arg == "/notrees") Params.trees = false;
                h++;
            }
            #endregion




            Loading LDForm = new Loading();
            LDForm.Show();

            if (Params.usesqlite)
            {
                Global.DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
                Global.DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
                Global.DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
            }
            else
                if (Params.usecustomdb)
                {
                    Global.DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                    Global.DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                    Global.DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                }
                else
                    if (!Params.usetxt)
                    {
                        Global.DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
                        Global.DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
                        Global.DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
                    }
                    else
                    {
                        Global.DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
                        Global.DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
                        Global.DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
                    }

            Global.DbConnection.Open();
            OdbcCommand DbCommand = Global.DbConnection.CreateCommand();
            OdbcDataReader DbReader;
            DbCommand.CommandText = "SELECT Name FROM items" + Global.tblext + " ORDER BY ID ASC";
            DbReader = DbCommand.ExecuteReader();

            Global.itemnames = new List<string>();
            while (DbReader.Read())
            {
                Global.itemnames.Add(DbReader.GetString(0));
            }
            DbReader.Close();
            Global.DbConnection.Close();


            if(!Global.connect())
            {
                Application.Exit();
                return;
            }


            lst_buildings.DisplayMember = "Name";
            lst_buildings.ValueMember = "Value";

            foreach (String name in Global.Buildings.Keys)
            {
                CNameValue NV = new CNameValue(name + " : " + Global.Buildings[name], name);
                lst_buildings.Items.Add(NV);
            }
            Debug.Print("Building array at: {0:x}", Global.BuildingsPointer);


            XmlSerializer xs = new XmlSerializer(typeof(CProduction));
            FileStream fs = new FileStream("ResourceProduction.xml", FileMode.Open);
            Global.Production = (CProduction)xs.Deserialize(fs);
            Global.Production.init();
            /*Global.Production = new CProduction();
            CProductionBuilding pb = new CProductionBuilding("Test");
            pb.ResourceProduced = new CProductionResource("Wood",1);
            pb.ResourcesNeeded.Add(new CProductionResource("Wood", 2));
            Global.Production.Building.Add(pb);
            FileStream fs = new FileStream("ResourceProductionSave.xml", FileMode.Create);
            xs.Serialize(fs, Global.Production);
            fs.Close();*/

            if (!Params.buildingsonly)
            {
                resources.DataSource = Global.resourceEntries;
                resources.DisplayMember = "Text";
                resources.ValueMember = "ID";

                items.DataSource = Global.itemEntries;
                items.DisplayMember = "Text";
                items.ValueMember = "ID";

                refreshItemList();
                ItemRefresh.Enabled = true;
            }
            else
            {
                tabCtrl.TabPages.Remove(tabPage_Items);
                tabCtrl.TabPages.Remove(tabPage_Time);
                tabPage_Buildings.Select();
            }
            BuildingRefresh.Enabled = true;
            #endregion

            this.Visible = true;
            LDForm.Hide();
            LDForm.Dispose();
        }

        private void ItemRefresh_Tick(object sender, EventArgs e)
        {
            if (!Global.connected)
            {
                disconnected();
                return;
            }
            ItemRefresh.Enabled = false;
            refreshItemList();
            Global.DbConnection3.Open();
            foreach (CItemEntry i in Global.itemEntries)
                i.save();
            Global.DbConnection3.Close();
            ItemRefresh.Enabled = true;
        }

        private void items_SelectedValueChanged(object sender, EventArgs e)
        {
            if (items.SelectedIndex == -1) return;
            PointPairList list = new PointPairList();
            try
            {
                Global.DbConnection2.Open();
                OdbcCommand DbCommand = Global.DbConnection2.CreateCommand();
                //DbCommand.CommandText = "SELECT [DateTime],Amount FROM History" + Global.tblext + " WHERE ID=" + items.SelectedIndex + " AND [DateTime]>CDate('" + DateTime.Now.AddDays(-1) + "') ORDER BY [DateTime] ASC";
                DbCommand.CommandText = "SELECT [DateTime],Amount FROM History" + Global.tblext + " WHERE ID=? AND [DateTime]>? ORDER BY [DateTime] ASC";
                DbCommand.Parameters.Add("ID", OdbcType.Int).Value = items.SelectedIndex;
                DbCommand.Parameters.Add("Date", OdbcType.DateTime).Value = DateTime.Now.AddDays(-1);

                OdbcDataReader DbReader = DbCommand.ExecuteReader();

                while (DbReader.Read())
                {

                    double diff = new XDate(DbReader.GetDateTime(0));
                    list.Add(diff, DbReader.GetInt32(1));
                }
                DbReader.Close();
                Global.DbConnection2.Close();
            }
            catch (Exception er)
            {
                Debug.Print(er.StackTrace);
            }
            PointPairList list2=new PointPairList();
            foreach(CItemEntry ie in Global.itemEntries)
                if(ie.ID==items.SelectedIndex)
                    list2 = Global.Production.simulate(ie.internName);
            CreateGraph(graph, Global.itemnames[items.SelectedIndex], list, list2);
            graph.Refresh();
        }
        private string formatedTimeFromSeconds(double seconds)
        {
            if (seconds == -1) return "-";
            int h = (int)(seconds / 60 / 60);
            int min = (int)((seconds - h * 60 * 60 ) / 60);
            int s = (int)(seconds - h * 60 * 60  + min * 60);

            TimeSpan t2 = new TimeSpan(h, min, s);
            return t2.ToString();
        }
        private void TimeLeft_Tick(object sender, EventArgs e)
        {
            if (!Global.connected)
            {
                disconnected();
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            TimeLeft.Enabled = false;
            
            int i = 0;
            DateTime dt = DateTime.Now;
            foreach (ListViewItem liv in itemsOverview.Items)
            {
                string resname = Global.itemEntries[i++].internName;
                liv.SubItems[1].Text = "*";
                liv.SubItems[2].Text = "*";
                Application.DoEvents();
                liv.SubItems[1].Text = formatedTimeFromSeconds(Global.Production.findLimitEmpty(resname));
                Application.DoEvents();
                liv.SubItems[2].Text = formatedTimeFromSeconds(Global.Production.findLimitFull(resname));
                Debug.Print("Fetch Limit for 1 item {1} took {0} seconds.", (DateTime.Now - dt).TotalSeconds,resname); 
                Application.DoEvents();
            }
            
            this.Cursor = Cursors.Default;
            TimeLeft.Enabled = true;
        }

        private void tabCtrl_Selected(object sender, TabControlEventArgs e)
        {
            if (tabCtrl.SelectedTab == tabPage_Time)
            {
                foreach (CItemEntry ie in Global.itemEntries)
                {
                    string[] cols = new string[3];
                    cols[0] = ie.Text;
                    cols[1] = "-";
                    cols[2] = "-";
                    ListViewItem liv = new ListViewItem(cols);
                    itemsOverview.Items.Add(liv);
                }
                if (Global.connected)
                {
                    Application.DoEvents();
                    TimeLeft_Tick(null, null);
                    TimeLeft.Enabled = true;
                }
            }
            else
            {
                itemsOverview.Items.Clear();
                TimeLeft.Enabled = false;
            }
        }

        private void lst_buildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_buildings.SelectedIndex == -1) return;
            if (!Global.connected) return;

            lst_production.Items.Clear();
            uint i = 0;
            foreach (CBuildingEntry b in Global.buildingEntries)
                if (b.Name == ((CNameValue)lst_buildings.SelectedItem).Value)
                {
                    ListViewItem lve = new ListViewItem();
                    lve.Text = b.Name + i.ToString();
                    i++;
                    lst_production.Items.Add(lve);
                    if (b == null) continue;
                    if ((b.ePTime != -1) && (b.sPTime != -1))
                    {
                        double ticks = b.ePTime - b.sPTime;
                        Debug.Print(b.Name);
                        Debug.Print("{0:x}", b.memoffset);
                        Debug.Print("{0}", DateTime.Now.Ticks);
                        Debug.Print("{0}", ticks);
                        Debug.Print("{0}", b.ePTime);
                        Debug.Print("{0}", b.sPTime);
                        Debug.Print("{0}", DateTime.Now.Ticks / b.ePTime);
                        Debug.Print("{0}", DateTime.Now.Ticks / b.sPTime);
                        Debug.Print("{0} Min {1} Sec", (long)(ticks / 1000 / 60), (ticks / 1000) % 60);

                        lve.SubItems.Add(String.Format("{0} Min {1} Sec", (long)(ticks / 1000 / 60), (ticks / 1000) % 60));
                    }
                    else
                        lve.SubItems.Add("");
                    lve.SubItems.Add(b.level.ToString());
                    if (b.isActive)
                        lve.SubItems.Add("ja");
                    else
                        lve.SubItems.Add("nein");
                }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            StreamWriter s;

            sfd.DefaultExt = ".txt";
            sfd.Filter = "Textdatei (*.txt)|";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if ((s = new StreamWriter(sfd.OpenFile())) != null)
                {
                    s.WriteLine("Name,PTime,Level,Active");
                    foreach (CBuildingEntry b in Global.buildingEntries)
                    {
                        double ticks = b.ePTime - b.sPTime;
                        if ((b.ePTime == -1) || (b.sPTime == -1))
                            ticks = 0;
                        string a;
                        if (b.isActive)
                            a = "1";
                        else
                            a = "0";
                        s.WriteLine(b.Name + "," + (ticks / 1000) + "," + b.level + "," + a);
                    }
                    s.Close();
                }
            }

        }

        private void BuildingRefresh_Tick(object sender, EventArgs e)
        {
            if (!Global.connected)
            {
                disconnected();
                return;
            }
            BuildingRefresh.Enabled = false;
            Global.DbConnection3.Open();
            foreach (CBuildingEntry b in Global.buildingEntries)
                b.save();
            Global.DbConnection3.Close();
            BuildingRefresh.Enabled = true;
        }

        private void btn_reconnect_Click(object sender, EventArgs e)
        {
            if (Global.connect())
            {
                this.status.Text = "";
                btn_reconnect.Visible = false;
                if (!Params.buildingsonly)
                {
                    resources.DataSource = Global.resourceEntries;
                    resources.DisplayMember = "Text";
                    resources.ValueMember = "ID";

                    items.DataSource = Global.itemEntries;
                    items.DisplayMember = "Text";
                    items.ValueMember = "ID";

                    refreshItemList();
                    ItemRefresh.Enabled = true;
                }
                else
                {
                    tabCtrl.TabPages.Remove(tabPage_Items);
                    tabCtrl.TabPages.Remove(tabPage_Time);
                    tabPage_Buildings.Select();
                }
                BuildingRefresh.Enabled = true;
                return;
            }
            this.status.Text = "keine Verbindung!";
        }

    }
    public class CNameValue
    {
        public string Name;
        public string Value;
        public CNameValue(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
