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

namespace DSO_Economic
{
    public partial class DSOEForm : Form
    {
        private static ProcessModule npswf = null;
        private static uint MainClass = 0;
        private static long max;
        private static uint vartype;
        private static uint BuildingsPointer = 0;

        public static List<ItemEntry> itemEntries;
        private static List<ResourceEntry> resourceEntries;
        private static List<BuildingEntry> buildingEntries;
        private static Dictionary<String,uint> Buildings;

        private static bool usecustomdb = false;
        private static bool usetxt = false;
        private static bool usesqlite = false;
        private static bool trees = true;
        private static uint maxmemsize = 0x300000;
        private static uint maxsearchoffset = 0x1E0000;
        private static uint maxstorage = 6000;
        private static uint maxmatch1rounds = 10;


        public DSOEForm()
        {
            InitializeComponent();
        }

        #region MemorySearchFunctions
        private void findMainClass(IntPtr handle,uint[] mem, uint start, uint size)
        {
            Application.DoEvents();
            uint i = 0;
            uint v;
            uint w;

            for (i = 0; i < size - 0x5C; i += 4)
            {
                v = mem[(i) / 4];
                if ((v < (uint)npswf.BaseAddress) || (v > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    continue;
                if (mem[(i + 0x20) / 4] > 40)
                    continue;
                if (mem[(i + 0x28) / 4] > 40)
                    continue;
                if (mem[(i + 0x2C) / 4] == 0)
                    continue;
                if (mem[(i + 0x30) / 4] > 40)
                    continue;
                if (mem[(i + 0x3C) / 4] > 40)
                    continue;
                v = mem[(i+0x38) / 4];
                w = mem[(i + 0x40) / 4];
                if (w < v) continue;
                if (v > 1000) continue;
                if (w > 1000) continue;
                if (w == 0) continue;

                w = mem[(i + 0x5c) / 4];
                w += 0x10;

                uint br = 0;
                uint[] mem2 = new uint[4];
                if (!Global.ReadProcessMemory(handle, w, mem2, 4*4, ref br)) continue;

                if (mem2[0] != 44) continue;
                if (mem2[1] != 46) continue;

                uint starttbl = mem2[3];
                if ((starttbl > (uint)npswf.BaseAddress) && (starttbl < (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    continue;

                uint sz=(uint)(4 * Global.itemnames.Count);
                mem2 = new uint[Global.itemnames.Count + 1];
                if (!Global.ReadProcessMemory(handle, starttbl + 8, mem2, sz, ref br)) continue;

                MainClass = start + i;
                Debug.Print("Main class at: {0:x}", start + i);

                
                Debug.Print("Table at: {0:x}", starttbl);
                itemEntries = new List<ItemEntry>();
                ItemEntry.reset();
                for (int x = 0; x < Global.itemnames.Count; x++)
                {
                    ItemEntry ie = new ItemEntry(mem2[x] & 0xFFFFFFF8);
                    if (x == 0) max = ie.max;
                    itemEntries.Add(ie);
                }

                mem2 = new uint[1];
                if (!Global.ReadProcessMemory(handle, MainClass + 0x88, mem2, 4, ref br)) continue;

                if (!Global.ReadProcessMemory(handle, mem2[0] + 0x1b0, mem2, 4, ref br)) continue;

                if (!Global.ReadProcessMemory(handle, mem2[0] + 0x68, mem2, 4, ref br)) continue;

                if (!Global.ReadProcessMemory(handle, mem2[0] + 0x54, mem2, 4, ref br)) continue;

                uint[] mem3 = new uint[4];

                if (!Global.ReadProcessMemory(handle, mem2[0] + 0x10, mem3, 0x10, ref br)) continue;

                uint cnt = mem3[0];
                BuildingsPointer = mem3[3];

                mem2 = new uint[cnt];
                if (!Global.ReadProcessMemory(handle, BuildingsPointer, mem2, cnt * 4, ref br)) continue;

                buildingEntries = new List<BuildingEntry>();
                Buildings = new Dictionary<String,uint>();

                for (int x = 0; x < cnt; x++)
                {
                    Debug.Print("Building at: {0:x}", mem2[x] & 0xFFFFFFF8);
                    BuildingEntry BE = new BuildingEntry(mem2[x] & 0xFFFFFFF8);
                    Debug.Print(BE.Name);
                    if (!Buildings.ContainsKey(BE.Name))
                        Buildings.Add(BE.Name, 1);
                    else
                        Buildings[BE.Name]++;
                    buildingEntries.Add(BE);
                }

                lst_buildings.DisplayMember = "Name";
                lst_buildings.ValueMember = "Value";

                foreach (String name in Buildings.Keys)
                {
                    NameValue NV = new NameValue(name + " : " + Buildings[name],name);
                    lst_buildings.Items.Add(NV);
                }
                Debug.Print("Building array at: {0:x}", BuildingsPointer);
            }
            return;
        }
        private void findResources(uint[] mem, uint start, uint size)
        {
            Application.DoEvents();

            uint i = 0;
            uint v;
            uint w;

            for (i = 0; i < size - 0x54; i += 4)
            {
                v = mem[(i) / 4];
                if ((v < (uint)npswf.BaseAddress) || (v > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    continue;

                uint y = mem[(i + 0x40) / 4];
                if (y != 2)
                    continue;

                v = mem[(i + 0x44) / 4];
                if (((int)v != -1) && ((int)v != 0x17) && ((int)v != 0x14) && ((int)v != 5))
                    continue;


                w = mem[(i + 0x48) / 4];
                if (w > 5000)
                    continue;

                v = mem[(i + 0x54) / 4];
                if (((v == 5) || (v == 25)) && (!trees))
                    continue;

                if ((v != 160) && (v != 1000)) //Wasser und Getreide werden immer angezeigt ... (1000 und 160 sind dabei die maximale Anzahl an Einheiten, bisher habe ich keinen besseren Weg gefunden)
                    if (v == w)
                        continue;


                resourceEntries.Add(new ResourceEntry(start + i + 0x40));
            }
            return;
        }

        #endregion


        private void refreshItemList()
        {

            int idx = items.SelectedIndex;
            items.BindingContext[itemEntries].SuspendBinding();
            items.BindingContext[itemEntries].ResumeBinding();
            items.SelectedIndex = idx;

            idx = resources.SelectedIndex;
            resources.BindingContext[resourceEntries].SuspendBinding();
            resources.BindingContext[resourceEntries].ResumeBinding();
            resources.SelectedIndex = idx;

        }

        private void CreateGraph(ZedGraphControl zgc, string title, PointPairList ppl)
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

            LineItem myCurve = myPane.AddCurve(title, ppl, Color.Blue,
                              SymbolType.Circle);
            myCurve.Line.Fill = new Fill(Color.White, Color.Red, 45F);
            myCurve.Symbol.Fill = new Fill(Color.White);
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            zgc.AxisChange();
        }
        private void DSOEForm_Load(object sender, EventArgs e)
        {
            #region init
            string[] args = Environment.GetCommandLineArgs();
            uint h = 0;

            #region ParamChecks
            foreach (string arg in args)
            {
                if (arg == "/usecustomdb")
                {
                    usecustomdb = true;
                }
                if (arg == "/usesqlite")
                {
                    usesqlite = true;
                }
                if ((arg == "/usetxt") || (usetxt))
                {
                    usetxt = true;
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

                if (arg == "/mm1r")
                {
                    if (args.Length > h + 1)
                    {
                        try
                        {
                            maxmatch1rounds = uint.Parse(args[h + 1]);
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
                            maxmemsize = uint.Parse(args[h + 1]);
                        }
                        catch (Exception e2)
                        {
                            MessageBox.Show("Unbekannte Eingabe:" + args[h + 1], "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            this.Dispose();
                        }
                    }
                }
                if (arg == "/ms")
                {
                    if (args.Length > h + 1)
                    {
                        try
                        {
                            maxstorage = uint.Parse(args[h + 1]);
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
                            maxsearchoffset = uint.Parse(args[h + 1]);
                        }
                        catch (Exception e2)
                        {
                            MessageBox.Show("Unbekannte Eingabe:" + args[h + 1], "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            this.Dispose();
                        }
                    }
                }
                if (arg == "/notrees") trees = false;
                h++;
            }
            #endregion

            uint MaxAddress = 0x7fffffff;
            long address = 0;
            bool result;


            Loading LDForm = new Loading();
            LDForm.Show();

            if (usesqlite)
            {
                Global.DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
                Global.DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
                Global.DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
            }
            else
            if (usecustomdb)
            {
                Global.DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                Global.DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                Global.DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
            }
            else
            if (!usetxt)
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


            itemEntries = new List<ItemEntry>();
            resourceEntries = new List<ResourceEntry>();
            string[] processes = new string[] { "plugin-container", "iexplore" }; //plugin-container für Chrome und Firefox ... IE macht wieder sein eigenes Ding
            foreach (string pname in processes)
            {
                Process[] pList = Process.GetProcessesByName(pname);
                foreach (Process p in pList)
                {
                    Global.Main = p;

                    ProcessModuleCollection moc = p.Modules;
                    foreach (ProcessModule mo in moc)
                    {
                        if (mo.ModuleName.ToUpper() == "NPSWF32.DLL") //wird vom Firefox geladen
                        {
                            npswf = mo;
                            break;
                        }
                        if (mo.ModuleName.ToUpper() == "NPSWF64.DLL") //wird vom Firefox geladen
                        {
                            npswf = mo;
                            break;
                        }
                        if ((mo.ModuleName.ToUpper().Substring(0, 5) == "FLASH") && (mo.ModuleName.ToUpper().Substring(mo.ModuleName.Length - 4, 4) == ".OCX")) //Flash*.ocx ... Internet Explorer ...
                        {
                            npswf = mo;
                            break;
                        }
                    }

                    if (npswf == null) continue; //nix gefunden ... versuche es mit nächstem Prozess

                    uint size;
                    uint br = 0;
                    MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();
                    do
                    {
                        result = Global.VirtualQueryEx(p.Handle, (IntPtr)address, out m, (uint)Marshal.SizeOf(m));
                        if (!result) break; //am ende angekommen ... wir können aufhören
                        if (m.AllocationBase == 0)
                        {
                            address = (long)(m.BaseAddress + m.RegionSize);
                            continue;
                        }

                        Debug.Print("Searching in:{0:x} - {1:x} Size: {2:x}", m.BaseAddress, m.BaseAddress + (uint)m.RegionSize, m.RegionSize);
                        size = (uint)m.RegionSize;
                        if (size > maxmemsize)
                        {
                            address = (long)(m.BaseAddress + m.RegionSize);
                            continue;
                        }
                        uint[] mem = new uint[size / 4];
                        if (!Global.ReadProcessMemory(p.Handle, (IntPtr)m.BaseAddress, mem, size, ref br))
                        {
                            if (Global.GetLastError() == 0x12b) //nur einen Teil ausgelesen
                                size = br;
                            else
                            {
                                Debug.Print("Last error:{0:x}", Global.GetLastError());
                                address = (long)(m.BaseAddress + m.RegionSize);
                                continue;
                            }
                        }
                        if (size == 0)
                        {
                            address = (long)(m.BaseAddress + m.RegionSize);
                            continue;
                        }

                        findMainClass(p.Handle,mem, (uint)m.BaseAddress, (uint)m.RegionSize);

                        //findItems(mem,(uint)m.BaseAddress, (uint)m.RegionSize);
                        findResources(mem, (uint)m.BaseAddress, (uint)m.RegionSize); //Rohstoffe sind in mehreren Segmenten enthalten ... also suchen wir alles komplett durch

                        address = (long)(m.BaseAddress + m.RegionSize);

                    } while (address <= MaxAddress);

                    if ((Global.Main != null) && (itemEntries.Count > 0)) break;
                }
                if ((Global.Main != null) && (itemEntries.Count > 0)) break;
            }
            #endregion

            if ((Global.Main != null) && (itemEntries.Count > 0))
            {
                resources.DataSource = resourceEntries;
                resources.DisplayMember = "Text";
                resources.ValueMember = "ID";

                items.DataSource = itemEntries;
                items.DisplayMember = "Text";
                items.ValueMember = "ID";

                refreshItemList();
                ItemRefresh.Enabled = true;
            }
            else
            {
                string errorcode = "";
                if (Global.Main == null)
                    errorcode += "1";
                else
                    errorcode += "0";

                if (itemEntries.Count == 0)
                    errorcode += "1";
                else
                    errorcode += "0";

                if (resourceEntries.Count == 0) //kein KO-Kriterium, aber dennoch hilfreich bei der Fehlersuche
                    errorcode += "1";
                else
                    errorcode += "0";

                if (npswf == null)
                    errorcode += "1";
                else
                    errorcode += "0";

                MessageBox.Show("Fehlercode: " + errorcode + "\nDaten konnten nicht abgefangen werden.\nEntweder ist das Spiel noch nicht gestartet, oder die Version dieses Programms ist veraltet!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LDForm.Hide();
            LDForm.Dispose();
        }

        private void ItemRefresh_Tick(object sender, EventArgs e)
        {
            Global.DbConnection3.Open();
            refreshItemList();
            foreach (ItemEntry i in itemEntries)
                i.save();
            Global.DbConnection3.Close();
        }

        private void items_SelectedValueChanged(object sender, EventArgs e)
        {
            if (items.SelectedIndex == -1) return;
            try
            {
                Global.DbConnection2.Open();
                OdbcCommand DbCommand = Global.DbConnection2.CreateCommand();
                //DbCommand.CommandText = "SELECT [DateTime],Amount FROM History" + Global.tblext + " WHERE ID=" + items.SelectedIndex + " AND [DateTime]>CDate('" + DateTime.Now.AddDays(-1) + "') ORDER BY [DateTime] ASC";
                DbCommand.CommandText = "SELECT [DateTime],Amount FROM History" + Global.tblext + " WHERE ID=? AND [DateTime]>? ORDER BY [DateTime] ASC";
                DbCommand.Parameters.Add("ID",OdbcType.Int).Value = items.SelectedIndex;
                DbCommand.Parameters.Add("Date", OdbcType.DateTime).Value = DateTime.Now.AddDays(-1);

                OdbcDataReader DbReader = DbCommand.ExecuteReader();

                PointPairList list = new PointPairList();
                while (DbReader.Read())
                {

                    double diff = new XDate(DbReader.GetDateTime(0));
                    list.Add(diff, DbReader.GetInt32(1));
                }
                DbReader.Close();
                Global.DbConnection2.Close();
                CreateGraph(graph, Global.itemnames[items.SelectedIndex], list);
                graph.Refresh();
            }
            catch (Exception er)
            {
                Debug.Print(er.StackTrace);
            }
        }
        private string getTimeLeftEmpty(uint ID)
        {
            //TODO: genauere Berechnung mittels Korrelationsgerade
            try
            {
                string limit1 = "";
                string limit2 = "";
                if (usesqlite)
                    limit2 = " LIMIT 1 ";
                else
                    limit1 = " TOP 1 ";

                OdbcCommand DbCommand = Global.DbConnection2.CreateCommand();
                //DbCommand.CommandText = "SELECT TOP 1 [DateTime],Amount FROM History" + Global.tblext + " WHERE ID=" + ID + " AND [DateTime]>CDate('" + DateTime.Now.AddMinutes(-10) + "') ORDER BY [DateTime] ASC";
                DbCommand.CommandText = "SELECT "+limit1+" [DateTime],Amount FROM History" + Global.tblext + " WHERE ID=? AND [DateTime]>? ORDER BY [DateTime] ASC"+limit2;
                DbCommand.Parameters.Add("ID",OdbcType.Int).Value=ID;
                DbCommand.Parameters.Add("Date", OdbcType.DateTime).Value = DateTime.Now.AddMinutes(-10);

                OdbcDataReader DbReader = DbCommand.ExecuteReader();

                if (DbReader.Read())
                {
                    long amt = DbReader.GetInt32(1);
                    long amt2 = itemEntries[(int)ID].amount;
                    if (amt <= amt2)
                    {
                        DbReader.Close();
                        return "-";
                    }
                    else
                    {
                        TimeSpan t = DateTime.Now - DbReader.GetDateTime(0);
                        DbReader.Close();

                        double ms = (amt2 / ((amt - amt2) / t.TotalMilliseconds));
                        int h = (int)(ms / 1000 / 60 / 60);
                        int min = (int)((ms - h * 60 * 60 * 1000) / 1000 / 60);
                        int s = (int)((ms - h * 60 * 60 * 1000 + min * 60 * 1000) / 1000);

                        TimeSpan t2 = new TimeSpan(h, min, s);
                        return t2.ToString();
                    }
                }
                DbReader.Close();
            }
            catch (Exception e)
            {
                Debug.Print(e.StackTrace);
            }

            return "-";
        }
        private string getTimeLeftFull(uint ID)
        {
            //TODO: genauere Berechnung mittels Korrelationsgerade
            try
            {
                OdbcCommand DbCommand = Global.DbConnection2.CreateCommand();
                //DbCommand.CommandText = "SELECT TOP 1 [DateTime],Amount FROM History" + tblext + " WHERE ID=" + ID + " AND [DateTime]>DateAdd('m',-10,NOW()) ORDER BY [DateTime] ASC";
                //DbCommand.CommandText = "SELECT TOP 1 [DateTime],Amount FROM History" + Global.tblext + " WHERE ID=" + ID + " AND [DateTime]>CDate('" + DateTime.Now.AddMinutes(-10) + "') ORDER BY [DateTime] ASC";
                string limit1 = "";
                string limit2 = "";
                if (usesqlite)
                    limit2 = " LIMIT 1 ";
                else
                    limit1 = " TOP 1 ";
                DbCommand.CommandText = "SELECT "+limit1+" [DateTime],Amount FROM History" + Global.tblext + " WHERE ID=? AND [DateTime]>? ORDER BY [DateTime] ASC"+limit2;
                DbCommand.Parameters.Add("ID", OdbcType.Int).Value = ID;
                DbCommand.Parameters.Add("Date", OdbcType.DateTime).Value = DateTime.Now.AddMinutes(-10);
                
                OdbcDataReader DbReader = DbCommand.ExecuteReader();

                if (DbReader.Read())
                {
                    long amt = DbReader.GetInt32(1);
                    long amt2 = itemEntries[(int)ID].amount;
                    if (amt >= amt2)
                    {
                        DbReader.Close();
                        return "-";
                    }
                    else
                    {
                        TimeSpan t = DateTime.Now - DbReader.GetDateTime(0);
                        DbReader.Close();

                        double ms = ((max - amt2) / ((amt2 - amt) / t.TotalMilliseconds));
                        int h = (int)(ms / 1000 / 60 / 60);
                        int min = (int)((ms - h * 60 * 60 * 1000) / 1000 / 60);
                        int s = (int)((ms - h * 60 * 60 * 1000 + min * 60 * 1000) / 1000);

                        TimeSpan t2 = new TimeSpan(h, min, s);
                        return t2.ToString();
                    }
                }
                DbReader.Close();
            }
            catch (Exception e)
            {
                Debug.Print(e.StackTrace);
            }

            return "-";
        }
        private void TimeLeft_Tick(object sender, EventArgs e)
        {
            Global.DbConnection2.Open();
            uint i = 0;
            foreach (ListViewItem liv in itemsOverview.Items)
            {
                liv.SubItems[1].Text = getTimeLeftEmpty(i);
                liv.SubItems[2].Text = getTimeLeftFull(i++);
            }
            Global.DbConnection2.Close();
        }

        private void tabCtrl_Selected(object sender, TabControlEventArgs e)
        {
            if (tabCtrl.SelectedTab == tabCtrl.TabPages[1])
            {
                foreach (ItemEntry ie in itemEntries)
                {
                    string[] cols = new string[3];
                    cols[0] = ie.Text;
                    cols[1] = "-";
                    cols[2] = "-";
                    ListViewItem liv = new ListViewItem(cols);
                    itemsOverview.Items.Add(liv);
                }
                TimeLeft.Enabled = true;
            }
            else
            {
                itemsOverview.Items.Clear();
                TimeLeft.Enabled = false;
            }
        }

        private void lst_buildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lst_buildings.SelectedIndex==-1)return;
            lst_production.Items.Clear();
            uint i=0;
            foreach(BuildingEntry b in buildingEntries)
                if(b.Name==((NameValue)lst_buildings.SelectedItem).Value)
                {
                    ListViewItem lve = new ListViewItem();
                    lve.Text = b.Name+i.ToString();
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
                    foreach (BuildingEntry b in buildingEntries)
                    {
                        double ticks = b.ePTime - b.sPTime;
                        if ((b.ePTime == -1) || (b.sPTime == -1))
                            ticks = 0;
                        string a;
                        if (b.isActive)
                            a = "1";
                        else
                            a = "0";
                        s.WriteLine(b.Name+","+(ticks/1000)+","+b.level+","+a);
                    }
                    s.Close();
                }
            }

        }

    }
    public class NameValue
    {
        public string Name;
        public string Value;
        public NameValue(string Name, string Value)
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
