using System.Configuration;
using System.Data.Odbc;
using System.Data.ProviderBase;
using System;
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
        private static Process Main = null;
        private static long max;
        private static uint vartype;
        private static uint lastresourceEntriesID = 0;

        private static List<String> itemnames;
        public static List<ItemEntry> itemEntries;
        private static List<ResourceEntry> resourceEntries;
        private static OdbcConnection DbConnection;
        private static OdbcConnection DbConnection2;
        private static OdbcConnection DbConnection3;


        private static bool usecustomdb = false;
        private static bool usetxt = false;
        private static bool trees = true;
        private static uint maxmemsize = 0x300000;
        private static uint maxsearchoffset = 0x1E0000;
        private static uint maxstorage = 6000;
        private static uint maxmatch1rounds = 10;


        private static string tblext = "";


        [DllImport("Kernel32.dll")]
        static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("Kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static extern uint GetLastError();


        public DSOEForm()
        {
            InitializeComponent();
        }

        #region MemorySearchFunctions
        private void findResources(IntPtr handle, uint start, uint size)
        {
            Application.DoEvents();

            if (size > maxmemsize)
                return;

            UInt32 br = 0;
            uint i = 0;
            uint v;
            uint w;

            if (size > maxsearchoffset)
                size = maxsearchoffset;

            uint[] mem = new uint[size / 4];
            if (!ReadProcessMemory(handle, (IntPtr)start, mem, size, ref br))
            {
                if (GetLastError() == 0x12b) //nur einen Teil ausgelesen
                    size = br;
                else
                {
                    Debug.Print("Last error:{0:x}", GetLastError());
                    return;
                }
            }
            if (size == 0) return;


            for (i = 0; i < size - 0x54; i += 4)
            {
                do
                {
                    v = mem[(i) / 4];
                    if ((v < (uint)npswf.BaseAddress) || (v > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                        break;

                    uint y = mem[(i + 0x40) / 4];
                    if (y != 2) break;

                    v = mem[(i + 0x44) / 4];
                    if (((int)v != -1) && ((int)v != 0x17) && ((int)v != 0x14) && ((int)v != 5)) break;


                    w = mem[(i + 0x48) / 4];
                    if (w > 5000) break;

                    v = mem[(i + 0x54) / 4];
                    if (((v == 5) || (v == 25)) && (!trees)) break;

                    if ((v != 160) && (v != 1000)) //Wasser und Getreide werden immer angezeigt ... (1000 und 160 sind dabei die maximale Anzahl an Einheiten, bisher habe ich keinen besseren Weg gefunden)
                        if (v == w) break;

                    resourceEntries.Add(new ResourceEntry(start + i + 0x40));
                    Debug.Print("Step2 ...");
                }
                while (false);
            }
            return;
        }



        private void findItems(IntPtr handle, uint start, uint size)
        {
            Application.DoEvents();
            if (size > maxmemsize) return;

            UInt32 br = 0;
            uint i = 0;
            uint starti = 0;
            uint v;
            uint w;

            if (size > maxsearchoffset)
                size = maxsearchoffset;

            uint[] mem = new uint[size / 4];
            uint[] mem2 = new uint[0x14 / 4];
            if (!ReadProcessMemory(handle, (IntPtr)start, mem, size, ref br))
                if (GetLastError() == 0x12b) //nur einen Teil ausgelesen
                    size = br;
                else
                {
                    Debug.Print("Last error:{0:x}", GetLastError());
                    return;
                }

            if (size == 0) return;
            for (i = 0; i < size - 0x20; i += 4)
            {

                starti = i;

                w = mem[i / 4];

                if ((w < (uint)npswf.BaseAddress) || (w > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize))) continue;

                w = mem[(i + 0x04) / 4];
                if ((w & 0xFF) != 3) continue;

                vartype = mem[(i + 0x0C) / 4];
                if (vartype == 0) continue;

                v = mem[(i + 0x10) / 4];

                w = mem[(i + 0x14) / 5];

                if (v == 0) continue;
                if (v > maxstorage) continue;
                if (v % 100 != 0) continue;
                if (w > v) continue;
                max = v;

                if (max == 100) continue;

                Debug.Print("{0:x} {1:x} {2} {3}", start + i, vartype, v, w);

                itemEntries.Add(new ItemEntry(start + i));
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
        public void AutoFix(IntPtr handle)
        {
            Debug.Print("AutoFix ...");
            MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();
            long address = 0;
            uint MaxAddress = 0x7fffffff;
            uint[] mem = new uint[1];


            uint br = 0;
            do
            {
                bool result = VirtualQueryEx(handle, (IntPtr)address, out m, (uint)Marshal.SizeOf(m));
                if (!result) break;
                if (m.AllocationBase == 0)
                {
                    address = (long)(m.BaseAddress + m.RegionSize);
                    continue;
                }

                Debug.Print("Searching in:{0:x} - {1:x} Size: {2:x}", m.BaseAddress, m.BaseAddress + (uint)m.RegionSize, m.RegionSize);
                uint[] mem2 = new uint[m.RegionSize / 4];
                ReadProcessMemory(handle, (IntPtr)m.BaseAddress, mem2, (uint)m.RegionSize, ref br);

                for (int y = 0; y < itemEntries.Count - 2; y++)
                {
                    long off1 = itemEntries[y].memoffset;

                    for (uint i = 0; i < m.RegionSize - 8; i += 4)
                    {
                        if ((mem2[i / 4] & 0xFFFFFFF8) == (off1 & 0xFFFFFFF8))
                        {
                            while (mem2[i / 4] != 0)
                                i -= 4;
                            i += 12;

                            if (!ReadProcessMemory(handle, (IntPtr)(mem2[i / 4] & 0xFFFFFFF8), mem, 4, ref br)) continue;

                            if ((mem[0] < (uint)npswf.BaseAddress) || (mem[0] > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                                continue;

                            Debug.Print("Table at: {0:x}", m.BaseAddress + i);
                            itemEntries = new List<ItemEntry>();
                            ItemEntry.reset();
                            for (int x = 0; x < itemnames.Count; x++)
                            {
                                ItemEntry ie = new ItemEntry(mem2[(uint)(i + x * 4) / 4] & 0xFFFFFFF8);
                                if (x == 0) max = ie.max;
                                itemEntries.Add(ie);
                            }
                            Debug.Print("Autofixed ...");
                            return;
                        }
                    }
                }
                address = (long)(m.BaseAddress + m.RegionSize);
            } while (address <= MaxAddress);
            Debug.Print("Autofix not possible ...");
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
                if ((arg == "/usetxt") || (usetxt))
                {
                    usetxt = true;
                    tblext = ".txt";
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

            if (usecustomdb)
            {
                DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
            }
            if (!usetxt)
            {
                DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
                DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
                DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
            }
            else
            {
                DbConnection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
                DbConnection2 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
                DbConnection3 = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
            }


            DbConnection.Open();
            DbConnection2.Open();
            DbConnection3.Open();

            OdbcCommand DbCommand = DbConnection.CreateCommand();
            OdbcDataReader DbReader;
            DbCommand.CommandText = "SELECT Name FROM items" + tblext + " ORDER BY ID ASC";
            DbReader = DbCommand.ExecuteReader();

            itemnames = new List<string>();
            while (DbReader.Read())
            {
                itemnames.Add(DbReader.GetString(0));
            }
            DbReader.Close();

            itemEntries = new List<ItemEntry>();
            resourceEntries = new List<ResourceEntry>();
            string[] processes = new string[] { "plugin-container", "iexplore" }; //plugin-container für Chrome und Firefox ... IE macht wieder sein eigenes Ding
            foreach (string pname in processes)
            {
                Process[] pList = Process.GetProcessesByName(pname);
                foreach (Process p in pList)
                {
                    Main = p;

                    ProcessModuleCollection moc = p.Modules;
                    foreach (ProcessModule mo in moc)
                    {
                        if (mo.ModuleName.ToUpper() == "NPSWF32.DLL") //wird vom Firefox geladen
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

                    MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();
                    do
                    {
                        result = VirtualQueryEx(p.Handle, (IntPtr)address, out m, (uint)Marshal.SizeOf(m));
                        if (!result) break; //am ende angekommen ... wir können aufhören
                        if (m.AllocationBase == 0)
                        {
                            address = (long)(m.BaseAddress + m.RegionSize);
                            continue;
                        }

                        Debug.Print("Searching in:{0:x} - {1:x} Size: {2:x}", m.BaseAddress, m.BaseAddress + (uint)m.RegionSize, m.RegionSize);
                        findItems(p.Handle, (uint)m.BaseAddress, (uint)m.RegionSize);
                        findResources(p.Handle, (uint)m.BaseAddress, (uint)m.RegionSize); //Rohstoffe sind in mehreren Segmenten enthalten ... also suchen wir alles komplett durch

                        address = (long)(m.BaseAddress + m.RegionSize);

                    } while (address <= MaxAddress);

                    if ((Main != null) && (itemEntries.Count > 0)) break;
                }
                if ((Main != null) && (itemEntries.Count > 0)) break;
            }
            #endregion

            if ((Main != null) && (itemEntries.Count > 0))
            {
                //                if (itemEntries.Count > itemnames.Count)
                AutoFix(Main.Handle);

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
                if (Main == null)
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

        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int Type;
        }


        public class ItemEntry
        {
            private uint _ID;
            private static int last_ID = -1;
            public string Amount
            {
                get
                {
                    return amount.ToString();
                }
            }
            public uint amount
            {
                get
                {
                    if (memoffset == 0) return 0;
                    uint[] mem = new uint[0x20 / 4];
                    UInt32 br = 0;
                    ReadProcessMemory(Main.Handle, (IntPtr)(memoffset), mem, 0x20, ref br);
                    return mem[0x14 / 4];
                }
            }
            public uint max
            {
                get
                {
                    if (memoffset == 0) return 0;
                    uint[] mem = new uint[0x20 / 4];
                    UInt32 br = 0;
                    ReadProcessMemory(Main.Handle, (IntPtr)(memoffset), mem, 0x20, ref br);
                    return mem[0x10 / 4];
                }
            }
            public uint ID
            {
                get
                {
                    return _ID;
                }
            }
            private string _Name;
            public long memoffset;
            public ItemEntry(long offset)
            {
                _ID = (uint)(last_ID + 1);
                last_ID = (int)_ID;
                if (_ID < itemnames.Count)
                    _Name = itemnames[(int)_ID];
                else
                    _Name = "";
                memoffset = offset;
            }
            public static void reset()
            {
                ItemEntry.last_ID = -1;
            }
            public void setID(uint ID)
            {
                this._ID = ID;
                if (_ID < itemnames.Count)
                    this._Name = itemnames[(int)_ID];
                else
                    this._Name = "";
            }
            public void save()
            {
                try
                {
                    OdbcCommand DbCommand = DbConnection3.CreateCommand();
                    DbCommand.CommandText = "INSERT INTO History" + tblext + " (ID,[DateTime],Amount) VALUES (" + ID + ",CDate('" + DateTime.Now + "')," + amount + ")";
                    DbCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.Print(e.ToString());
                }

            }
            public string Text
            {
                get
                {
                    UInt32 br = 0;
                    byte[] mem = new byte[0x20];
                    if ((Main != null) && (memoffset != 0))
                    {
                        ReadProcessMemory(Main.Handle, (IntPtr)(memoffset), mem, 0x20, ref br);
                        return _Name + ": " + amount;
                    }
                    Debug.Print("ID:{0} memoffset:{1:x}", ID, memoffset);
                    return "";
                }
            }
        }

        public class ResourceEntry
        {
            private long memoffset;
            public long amount;
            private uint _ID;
            public uint ID
            {
                get
                {
                    return _ID;
                }
            }
            public ResourceEntry(long offset)
            {
                this._ID = lastresourceEntriesID;
                lastresourceEntriesID++;
                this.amount = 0;
                this.memoffset = offset;
            }
            public string Text
            {
                get
                {
                    UInt32 br = 0;
                    uint[] mem = new uint[0x18 / 4];
                    if ((Main != null) && (memoffset != 0))
                    {
                        ReadProcessMemory(Main.Handle, (IntPtr)(memoffset), mem, 0x18, ref br);

                        if ((int)mem[0] == 2)
                        {
                            amount = mem[8 / 4];

                            uint max = mem[0x14 / 4];
                            string name = "";
                            switch (max)
                            {
                                case 5:
                                case 25:
                                    name = "Baum";
                                    break;

                                case 1000:
                                    name = "Wasser";
                                    break;
                                case 610:
                                    name = "Stein";
                                    break;
                                case 700:
                                case 680:
                                    name = "Fisch";
                                    break;
                                case 400:
                                    name = "Eisen";
                                    break;
                                case 710:
                                    name = "Kupfer";
                                    break;
                                case 300:
                                    name = "Marmor";
                                    break;
                                case 160:
                                    name = "Getreide";
                                    break;
                                default:
                                    name = "?" + max;
                                    break;

                            }
                            return name + ": " + amount;
                        }
                        else
                            Debug.Print("Invalid ID:{0} memoffset:{1:x}", ID, memoffset);
                    }
                    Debug.Print("ID:{0} memoffset:{1:x}", ID, memoffset);
                    return "";
                }
            }
        }


        private void ItemRefresh_Tick(object sender, EventArgs e)
        {
            refreshItemList();
            foreach (ItemEntry i in itemEntries)
                i.save();
        }

        private void items_SelectedValueChanged(object sender, EventArgs e)
        {
            if (items.SelectedIndex == -1) return;
            try
            {
                OdbcCommand DbCommand = DbConnection2.CreateCommand();
                //DbCommand.CommandText = "SELECT [DateTime],Amount FROM History" + tblext + " WHERE ID=" + items.SelectedIndex + " AND [DateTime]>DateAdd('d',-1,NOW()) ORDER BY [DateTime] ASC";
                DbCommand.CommandText = "SELECT [DateTime],Amount FROM History" + tblext + " WHERE ID=" + items.SelectedIndex + " AND [DateTime]>CDate('" + DateTime.Now.AddDays(-1) + "') ORDER BY [DateTime] ASC";
                OdbcDataReader DbReader = DbCommand.ExecuteReader();

                PointPairList list = new PointPairList();
                while (DbReader.Read())
                {

                    double diff = new XDate(DbReader.GetDateTime(0));
                    list.Add(diff, DbReader.GetInt32(1));
                }
                DbReader.Close();
                CreateGraph(graph, itemnames[items.SelectedIndex], list);
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
                OdbcCommand DbCommand = DbConnection2.CreateCommand();
                //            DbCommand.CommandText = "SELECT TOP 1 [DateTime],Amount FROM History" + tblext + " WHERE ID=" + ID + " AND [DateTime]>DateAdd('m',-10,NOW()) ORDER BY [DateTime] ASC";
                DbCommand.CommandText = "SELECT TOP 1 [DateTime],Amount FROM History" + tblext + " WHERE ID=" + ID + " AND [DateTime]>CDate('" + DateTime.Now.AddMinutes(-10) + "') ORDER BY [DateTime] ASC";
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

                        double ms = (amt2 / ((amt2 - amt) / t.TotalMilliseconds));
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
                OdbcCommand DbCommand = DbConnection2.CreateCommand();
                //DbCommand.CommandText = "SELECT TOP 1 [DateTime],Amount FROM History" + tblext + " WHERE ID=" + ID + " AND [DateTime]>DateAdd('m',-10,NOW()) ORDER BY [DateTime] ASC";
                DbCommand.CommandText = "SELECT TOP 1 [DateTime],Amount FROM History" + tblext + " WHERE ID=" + ID + " AND [DateTime]>CDate('" + DateTime.Now.AddMinutes(-10) + "') ORDER BY [DateTime] ASC";
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

                        double ms = ((max - amt2) / ((amt - amt2) / t.TotalMilliseconds));
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
            uint i = 0;
            foreach (ListViewItem liv in itemsOverview.Items)
            {
                liv.SubItems[1].Text = getTimeLeftEmpty(i);
                liv.SubItems[2].Text = getTimeLeftFull(i++);
            }
        }

        private void tabCtrl_TabIndexChanged(object sender, EventArgs e)
        {

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
    }
}
