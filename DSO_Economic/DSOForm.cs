using System.Configuration;
using System.Data.Odbc;
using System.Data.OleDb;
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
        private static long offset = 0;
        private static long max;
        private static uint vartype;
        private static uint lastresourceEntriesID = 0;

        private static List<String> itemnames;
        private static List<ItemEntry> itemEntries;
        private static List<ResourceEntry> resourceEntries;
        private static OleDbConnection DbConnection;


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
        static extern uint GetLastError();


        public DSOEForm()
        {
            InitializeComponent();
        }
        private static uint getDword(byte[] b, uint i)
        {
            return (uint)(b[i] + b[i + 1] * 0x100 + b[i + 2] * 0x10000 + b[i + 3] * 0x1000000);
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

            byte[] mem = new byte[size];
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
                    v = getDword(mem, i);
                    if ((v < (uint)npswf.BaseAddress) || (v > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                        break;

                    uint y = getDword(mem, i + 0x40);
                    if (y != 2) break;

                    v = getDword(mem, i + 0x44);
                    if (((int)v != -1) && ((int)v != 0x17) && ((int)v != 0x14) && ((int)v != 5)) break;


                    w = getDword(mem, i + 0x48);
                    if (w > 5000) break;

                    v = getDword(mem, i + 0x54);
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

        private long findItems(IntPtr handle, uint start, uint size)
        {
            Application.DoEvents();
            if (size > maxmemsize) return 0;

            UInt32 br = 0;
            uint i = 0;
            uint starti = 0;
            uint v;
            uint w;

            if (size > maxsearchoffset)
                size = maxsearchoffset;

            byte[] mem = new byte[size];
            if (!ReadProcessMemory(handle, (IntPtr)start, mem, size, ref br))
                if (GetLastError() == 0x12b) //nur einen Teil ausgelesen
                    //size = br;
                    return 0;
                else
                {
                    Debug.Print("Last error:{0:x}", GetLastError());
                    return 0;
                }

            if (size == 0) return 0;

            for (i = 0; i < size - 0x20; i++)
            {
                //if (i + start > 0x12eaf000) Debugger.Break();

                starti = i;

                w = getDword(mem, i);
                if ((w < (uint)npswf.BaseAddress) || (w > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize))) continue;

                w = getDword(mem, i + 0x04);
                if (((w & 0xFF) != 2) && ((w & 0xFF) != 3)) continue;

                v = getDword(mem, i + 0x10);
                w = getDword(mem, i + 0x14);

                if (v == 0) continue;
                if (v > maxstorage) continue;
                if (v % 100 != 0) continue;
                if (w > v) continue;
                max = v;
                bool correct = true;

                for (uint rounds = 0; rounds < maxmatch1rounds; rounds++)
                {
                    if (i >= size - 4 - 0x20 - 16)
                    {
                        correct = false;
                        break;
                    }

                    w = getDword(mem, i + 0x20 );
                    if ((w < (uint)npswf.BaseAddress) || (w > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    {
                        correct = false;
                        break;
                    }

                    w = getDword(mem, i + 0x20 + 0x04);
                    if (((w&0xFF) != 2)&&((w&0xFF) != 3))
                    {
                        correct = false;
                        break;
                    }

                    w = getDword(mem, i + 0x20 + 0x10);
                    if (w != v)
                    {
                        correct = false;
                        break;
                    }

                    w = getDword(mem, i + 0x20 + 0x14);
                    if (w > v)
                    {
                        correct = false;
                        break;
                    }


                    i += 0x20;

                }
                if (correct)
                {
                    uint searchstart = starti + 0x0C;
                    uint vartype = getDword(mem, searchstart);
                    for (searchstart = starti + 0x0C; (searchstart > 0) && (searchstart < size - 8); searchstart -= 8 * 4)
                    {
                        if (getDword(mem, searchstart) == vartype)
                            if (getDword(mem, searchstart + 4) == max)
                                starti = searchstart - 0x0C;
                    }

                    long pos = start + starti;
                    for (int j = 0; j < 40; j++)
                    {
                        if (!ReadProcessMemory(Main.Handle, (IntPtr)(pos), mem, 0x20, ref br)) return 0;
                        uint x = 0;
                        while (getDword(mem, 0x0C) != vartype)
                        {
                            pos += 8 * 4;
                            if (!ReadProcessMemory(Main.Handle, (IntPtr)(pos), mem, 0x20, ref br)) return 0;
                            if (x++ > 100000) { return 0; }
                        }
                        pos += 8 * 4;
                    }

                    return start + starti;
                }
            }
            return 0;
        }
        #endregion


        private void refreshItemList()
        {
            UInt32 br = 0;
            byte[] mem = new byte[0x20];
            if (!ReadProcessMemory(Main.Handle, (IntPtr)(offset), mem, 0x20, ref br)) return;
            vartype = getDword(mem, 0x0C);

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
            long memstart = 0;

            Loading LDForm = new Loading();
            LDForm.Show();

            if (usecustomdb)
            {
                DbConnection = new OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
            }
            if (!usetxt)
                DbConnection = new OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
            else
                DbConnection = new OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);


            DbConnection.Open();
            OleDbCommand DbCommand = DbConnection.CreateCommand();
            OleDbDataReader DbReader;
            DbCommand.CommandText = "SELECT Name FROM items" + tblext + " ORDER BY ID ASC";
            DbReader = DbCommand.ExecuteReader();
            itemnames = new List<string>();
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

                    do
                    {
                        MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();
                        result = VirtualQueryEx(p.Handle, (IntPtr)address, out m, (uint)Marshal.SizeOf(m));
                        if (!result) break; //am ende angekommen ... wir können aufhören

                        address = (long)(m.BaseAddress + m.RegionSize);
                        if (m.AllocationBase == 0) continue;


                        long address2 = address;
                        MEMORY_BASIC_INFORMATION m2 = new MEMORY_BASIC_INFORMATION();
                        m2 = m;
                        uint totalsize = (uint)m.RegionSize;
                        while (m2.AllocationBase == m.AllocationBase)
                        {
                            address = address2;
                            result = VirtualQueryEx(p.Handle, (IntPtr)address2, out m2, (uint)Marshal.SizeOf(m));
                            totalsize += (uint)m2.RegionSize;
                            address2 = (long)(m2.BaseAddress + m2.RegionSize);
                            if (!result) break;
                        }
                        totalsize -= (uint)m2.RegionSize; //berechne totale Segmentgröße ... ist etwas schneller gleich viel Speicher zu laden und zu überprüfen anstatt einzeln

                        Debug.Print("Searching in:{0:x} - {1:x} Size: {2:x}", m.AllocationBase, (uint)m.AllocationBase + totalsize, totalsize);

                        long tempoff;
                        if (offset == 0) //Offset für items noch nicht gefunden?
                        {
                            tempoff = findItems(p.Handle, (uint)m.AllocationBase, (uint)totalsize);
                            if (tempoff != 0)
                            {
                                memstart = (uint)m.AllocationBase;
                                offset = tempoff;
                            }
                        }


                        findResources(p.Handle, (uint)m.AllocationBase, (uint)totalsize); //Rohstoffe sind in mehreren Segmenten enthalten ... also suchen wir alles komplett durch

                    } while (address <= MaxAddress);

                    if (offset != 0) break;
                }
                if ((Main != null) && (offset != 0)) break;
            }
            #endregion

            if ((Main != null) && (offset > 0))
            {


                Debug.Print("Start offset: {0:x}", offset);
                Debug.Print("Memory part offset: {0:x}", memstart);
                Debug.Print("Start offset relative to memory part offset: {0:x}", offset - memstart);
                UInt32 br = 0;
                byte[] mem = new byte[0x20];
                ReadProcessMemory(Main.Handle, (IntPtr)(offset ), mem, 0x20, ref br);
                vartype = getDword(mem, 0x0C);


                uint i = 0;
                while (DbReader.Read())
                {
                    itemnames.Add(DbReader.GetString(0));
                    itemEntries.Add(new ItemEntry(i++, DbReader.GetString(0), offset));
                }


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

                if (offset == 0)
                    errorcode += "1";
                else
                    errorcode += "0";

                errorcode += "0"; //wurde früher mal benutzt ... nun nichtmehr

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


        public struct ItemEntry
        {
            private uint _ID;
            public uint amount
            {
                get
                {
                    if (memoffset == 0) return 0;
                    byte[] mem = new byte[0x20];
                    UInt32 br = 0;
                    ReadProcessMemory(Main.Handle, (IntPtr)(memoffset), mem, 0x20, ref br);
                    return getDword(mem, 0x14);
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
            private long memoffset;
            public ItemEntry(uint ID, string Name, long offset)
            {
                this._ID = ID;
                _Name = Name;

                UInt32 br = 0;
                byte[] mem = new byte[0x20];
                if ((Main != null) && (offset != 0))
                {
                    long pos = offset;
                    for (int i = 0; i < 40; )
                    {
                        ReadProcessMemory(Main.Handle, (IntPtr)(pos), mem, 0x20, ref br);
                        uint x = 0;
                        while (getDword(mem, 0x0C) != vartype)
                        {
                            pos += 8 * 4;
                            ReadProcessMemory(Main.Handle, (IntPtr)(pos), mem, 0x20, ref br);
                            if (x++ > 100000) { Debug.Print("Limit reached, setting memoffset=0 for ID:{0}", ID); memoffset = 0; return; }
                        }

                        if (getDword(mem, 0x10) == max)
                            if (getDword(mem, 0xC) == vartype)
                                if (i == ID)
                                {
                                    memoffset = pos;
                                    return;
                                }
                                else
                                    i++;

                        pos += 8 * 4;
                    }
                }
                Debug.Print("Setting memoffset=0 for ID:{0}", ID);
                memoffset = 0;
            }
            public void save()
            {
                try
                {
                    OleDbCommand DbCommand = DbConnection.CreateCommand();
                    DbCommand.CommandText = "INSERT INTO History" + tblext + " (ID,[DateTime],Amount) VALUES (" + ID + ",'" + DateTime.Now + "'," + amount + ")";
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

                        if (getDword(mem, 0xc) == vartype)
                        {
                            return _Name + ": " + amount;
                        }

                    }
                    Debug.Print("ID:{0} memoffset:{1:x}", ID, memoffset);
                    return "";
                }
            }
        }

        public struct ResourceEntry
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
                    byte[] mem = new byte[0x18];
                    if ((Main != null) && (memoffset != 0))
                    {
                        ReadProcessMemory(Main.Handle, (IntPtr)(memoffset), mem, 0x18, ref br);

                        if ((int)getDword(mem, 0) == 2)
                        {
                            amount = getDword(mem, 8);

                            uint max = getDword(mem, 0x14);
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
            OleDbCommand DbCommand = DbConnection.CreateCommand();
            DbCommand.CommandText = "SELECT [DateTime],Amount FROM History" + tblext + " WHERE ID=" + items.SelectedIndex + " AND [DateTime]>DateAdd('d',-1,NOW()) ORDER BY [DateTime] ASC";
            OleDbDataReader DbReader = DbCommand.ExecuteReader();

            PointPairList list = new PointPairList();
            while (DbReader.Read())
            {

                double diff = new XDate(DbReader.GetDateTime(0));
                list.Add(diff, DbReader.GetInt32(1));
            }
            CreateGraph(graph, itemnames[items.SelectedIndex], list);
            graph.Refresh();
        }
    }
}
