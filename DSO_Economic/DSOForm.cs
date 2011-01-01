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
        private static uint MainClass = 0;
        private static long max;
        private static uint vartype;
        private static uint lastresourceEntriesID = 0;
        private static uint BuildingsPointer = 0;

        private static List<String> itemnames;
        public static List<ItemEntry> itemEntries;
        private static List<ResourceEntry> resourceEntries;
        private static List<BuildingEntry> buildingEntries;
        private static Dictionary<String,uint> Buildings;
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
        static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static extern uint GetLastError();


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
                if (!ReadProcessMemory(handle, w, mem2, 4*4, ref br)) continue;

                if (mem2[0] != 44) continue;
                if (mem2[1] != 46) continue;

                uint starttbl = mem2[3];
                if ((starttbl > (uint)npswf.BaseAddress) && (starttbl < (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    continue;

                uint sz=(uint)(4 * itemnames.Count);
                mem2 = new uint[itemnames.Count+1];
                if (!ReadProcessMemory(handle, starttbl+8, mem2,sz, ref br)) continue;

                MainClass = start + i;
                Debug.Print("Main class at: {0:x}", start + i);

                
                Debug.Print("Table at: {0:x}", starttbl);
                itemEntries = new List<ItemEntry>();
                ItemEntry.reset();
                for (int x = 0; x < itemnames.Count; x++)
                {
                    ItemEntry ie = new ItemEntry(mem2[x] & 0xFFFFFFF8);
                    if (x == 0) max = ie.max;
                    itemEntries.Add(ie);
                }

                mem2 = new uint[1];
                if (!ReadProcessMemory(handle, MainClass+0x88, mem2, 4, ref br)) continue;
                
                if (!ReadProcessMemory(handle, mem2[0] + 0x1b0, mem2, 4, ref br)) continue;

                if (!ReadProcessMemory(handle, mem2[0] + 0x68, mem2, 4, ref br)) continue;
                
                if (!ReadProcessMemory(handle, mem2[0] + 0x54, mem2, 4, ref br)) continue;

                uint[] mem3 = new uint[4];

                if (!ReadProcessMemory(handle, mem2[0] + 0x10, mem3, 0x10, ref br)) continue;

                uint cnt = mem3[0];
                BuildingsPointer = mem3[3];

                mem2 = new uint[cnt];
                if (!ReadProcessMemory(handle, BuildingsPointer, mem2, cnt*4, ref br)) continue;

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

                foreach (String name in Buildings.Keys)
                    lst_buildings.Items.Add(name+" : "+Buildings[name]);
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
            DbConnection.Close();


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

                    uint size;
                    uint br = 0;
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
                        size = (uint)m.RegionSize;
                        if (size > maxmemsize)
                        {
                            address = (long)(m.BaseAddress + m.RegionSize);
                            continue;
                        }
                        uint[] mem = new uint[size / 4];
                        if (!ReadProcessMemory(p.Handle, (IntPtr)m.BaseAddress, mem, size, ref br))
                        {
                            if (GetLastError() == 0x12b) //nur einen Teil ausgelesen
                                size = br;
                            else
                            {
                                Debug.Print("Last error:{0:x}", GetLastError());
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

                    if ((Main != null) && (itemEntries.Count > 0)) break;
                }
                if ((Main != null) && (itemEntries.Count > 0)) break;
            }
            #endregion

            if ((Main != null) && (itemEntries.Count > 0))
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

        public class BuildingEntry
        {
            private long memoffset;
            public string Name;
            public BuildingEntry(uint offset)
            {
                this.memoffset = offset;

                uint br = 0;
                uint[] mem2 = new uint[4];
                
                if (!ReadProcessMemory(Main.Handle, offset+0x9C, mem2, 4, ref br)) return;
                
                if (!ReadProcessMemory(Main.Handle, mem2[0] + 0x08, mem2, 4*3, ref br)) return;
                
                byte[] mem = new byte[mem2[2]];
                if (!ReadProcessMemory(Main.Handle, mem2[0], mem, mem2[2], ref br)) return;
                Name = Encoding.UTF8.GetString(mem);
            }
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
            DbConnection3.Open();
            refreshItemList();
            foreach (ItemEntry i in itemEntries)
                i.save();
            DbConnection3.Close();
        }

        private void items_SelectedValueChanged(object sender, EventArgs e)
        {
            if (items.SelectedIndex == -1) return;
            try
            {
                DbConnection2.Open();
                OdbcCommand DbCommand = DbConnection2.CreateCommand();
                DbCommand.CommandText = "SELECT [DateTime],Amount FROM History" + tblext + " WHERE ID=" + items.SelectedIndex + " AND [DateTime]>CDate('" + DateTime.Now.AddDays(-1) + "') ORDER BY [DateTime] ASC";
                OdbcDataReader DbReader = DbCommand.ExecuteReader();

                PointPairList list = new PointPairList();
                while (DbReader.Read())
                {

                    double diff = new XDate(DbReader.GetDateTime(0));
                    list.Add(diff, DbReader.GetInt32(1));
                }
                DbReader.Close();
                DbConnection2.Close();
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
                OdbcCommand DbCommand = DbConnection2.CreateCommand();
                //DbCommand.CommandText = "SELECT TOP 1 [DateTime],Amount FROM History" + tblext + " WHERE ID=" + ID + " AND [DateTime]>DateAdd('m',-10,NOW()) ORDER BY [DateTime] ASC";
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
            DbConnection2.Open();
            uint i = 0;
            foreach (ListViewItem liv in itemsOverview.Items)
            {
                liv.SubItems[1].Text = getTimeLeftEmpty(i);
                liv.SubItems[2].Text = getTimeLeftFull(i++);
            }
            DbConnection2.Close();
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
