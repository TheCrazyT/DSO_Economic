﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.Odbc;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using FlashABCRead;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
namespace DSO_Economic
{
    class Global
    {
        public static bool isLinux;
        private static uint SwfClassToken;
        public static Loading LDForm;
        public static bool connected
        {
            get
            {
                uint br = 0;
                uint[] mem = new uint[1];
                if (!ReadProcessMemory(Main.Handle, (IntPtr)MainClass, mem, 4, ref br)) return false;
                if ((mem[0] < (uint)npswf.BaseAddress) || (mem[0] > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    return false;
                /*if (!ReadProcessMemory(Main.Handle, MainClass + 0x5c, mem, 4, ref br)) return false;
                if (mem[0] == 0)
                    return false;
                if (!ReadProcessMemory(Main.Handle, mem[0], mem, 4, ref br)) return false;*/

                return true;
            }
        }
        public static Dictionary<String, uint> Buildings;

        public static MyProcessModule npswf = null;
        public static Int64 MainClass = 0;
        public const uint LIST_MODULES_ALL = 0x03;
        public static MyProcess Main = null;
        public static List<String> itemnames;
        public static OdbcConnection DbConnection;
        public static OdbcConnection DbConnection2;
        public static OdbcConnection DbConnection3;

        public static List<CItemEntry> itemEntries;
        public static List<CBuildingEntry> buildingEntries;
        public static List<CResourceEntry> resourceEntries;

        public static string tblext = "";
        public static CProduction Production;

        static public void init()
        {

            if (!File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\fla.dat"))
            {
                if (MessageBox.Show("Datei fla.dat existiert nicht und muss heruntergeladen werden!", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                {
                    Global.LDForm.Close();
                    Global.LDForm.Dispose();
                    //Application.Exit();
                    
                    Environment.Exit(0);
                    return;
                }
                WebRequest wr = WebRequest.Create("https://gist.github.com/raw/19cb07aa1f7949016d3d/gistfile1.txt");
                WebResponse wrp = wr.GetResponse();
                Stream s = wrp.GetResponseStream();
                string url = Encoding.Default.GetString((new BinaryReader(s)).ReadBytes((int)wrp.ContentLength));
                wr = WebRequest.Create(url);
                wrp = wr.GetResponse();
                s = wrp.GetResponseStream();
                File.WriteAllBytes(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\fla.dat", new BinaryReader(s).ReadBytes((int)wrp.ContentLength));
            }

            List<string> classes = new List<string>();
            classes.Add("cGO");
            classes.Add("cPlayerData");
            classes.Add("cGeneralInterface");
            classes.Add("cPlayerZoneScreen");
            classes.Add("cStreetDataMap");
            classes.Add("cBuilding");
            classes.Add("cBuff");
            classes.Add("dResource");
            classes.Add("cResourceCreation");
            classes.Add("dResourceCreationDefinition");
            classes.Add("cDeposit");
            classes.Add("cResources");
            classes.Add("cPathObject");
            //classes.Add("gEconomics");
            FlashRead.ReadCompressed(new StreamReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\fla.dat").BaseStream, classes);
            if (fClass.Count == 0)
            {
                MessageBox.Show("Datei fla.dat vermutlich beschädigt lösche die Datei und versuche es erneut!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            fClass.InitClasses();


            Configuration conf = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
            foreach (ConnectionStringSettings cs in conf.ConnectionStrings.ConnectionStrings)
            {
                cs.ConnectionString = cs.ConnectionString.Replace("|DataDirectory|", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            }
            if (conf.ConnectionStrings == null)
                throw (new System.Exception("Konfigurationsdatei nicht geladen!"));

            if (Params.usesqlite)
            {
                DbConnection = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
                DbConnection2 = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
                DbConnection3 = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.SQLiteDB"].ConnectionString);
            }
            else
                if (Params.usecustomdb)
                {
                    DbConnection = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                    DbConnection2 = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                    DbConnection3 = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.CustomDB"].ConnectionString);
                }
                else
                    if (!Params.usetxt)
                    {
                        if (conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"] == null)
                            throw (new System.Exception("Konfigurationseintrag DSO_Economic.Properties.Settings.DataDB nicht geladen!\nPfad1:" + Application.ExecutablePath + "\nPfad2:" + Application.StartupPath + "\nPfad3:" + System.Reflection.Assembly.GetExecutingAssembly().Location));

                        DbConnection = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
                        DbConnection2 = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
                        DbConnection3 = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.DataDB"].ConnectionString);
                    }
                    else
                    {
                        DbConnection = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
                        DbConnection2 = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
                        DbConnection3 = new OdbcConnection(conf.ConnectionStrings.ConnectionStrings["DSO_Economic.Properties.Settings.CsvDB"].ConnectionString);
                    }

            if (DbConnection == null)
                throw (new System.Exception("Datenbank nicht initialisiert!"));

            DbConnection.Open();
            OdbcCommand DbCommand = DbConnection.CreateCommand();
            OdbcDataReader DbReader;
            DbCommand.CommandText = "SELECT Name FROM items" + tblext + " ORDER BY ID ASC";
            DbReader = DbCommand.ExecuteReader();

            if (DbConnection == null)
                throw (new System.Exception("DbReader nicht initialisiert!"));

            itemnames = new List<string>();
            while (DbReader.Read())
            {
                itemnames.Add(DbReader.GetString(0));
            }
            DbReader.Close();
            DbConnection.Close();
        }
        static public string export_buildings()
        {
            string s = "Name,PTime,Level,Active,Buff\r\n";
            foreach (CBuildingEntry b in buildingEntries)
            {
                double ticks = 0;
                try
                {
                    /*ticks = b.ePTime - b.sPTime;
                    if ((b.ePTime == -1) || (b.sPTime == -1))
                        ticks = 0;*/
                    ticks = b.PTime * 1000;
                }
                catch (EndOfStreamException err)
                {
                }
                string a;
                if (b.isActive)
                    a = "1";
                else
                    a = "0";
                
                s += (b.Name + "," + Math.Round(ticks / 1000) + "," + b.level + "," + a +","+ b.getBuffs()+ "\r\n");
            }
            return s;
        }
        static public string export_resources()
        {
            string s = "Name,Amount\r\n";
            foreach (CResourceEntry r in resourceEntries)
                s += (r.Name + "," + r.amount + "\r\n");
            return s;
        }
        static public string export_items()
        {
            string s = "Name,Amount\r\n";
            foreach (CItemEntry i in itemEntries)
                s += (i.internName + "," + i.amountStr + "\r\n");
            return s;
        }
        static public bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength)
        {
            if (!isLinux)
                return Windows.VirtualQueryEx(hProcess, lpAddress, out lpBuffer, dwLength);
            else
                return Linux.VirtualQueryEx(hProcess, lpAddress, out lpBuffer, dwLength);
        }

        static public bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            if (!isLinux)
                return Windows.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
            else
                return Linux.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
        }

        static public bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            if (!isLinux)
                return Windows.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
            else
                return Linux.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
        }

        static public bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            if (!isLinux)
                return Windows.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
            else
                return Linux.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
        }

        static public bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            if (!isLinux)
                return Windows.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
            else
                return Linux.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
        }

        static public bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, ulong[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            if (!isLinux)
                return Windows.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
            else
                return Linux.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
        }

        static public bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, double[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            if (!isLinux)
                return Windows.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
            else
                return Linux.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, ref lpNumberOfBytesRead);
        }

        static public uint GetLastError()
        {
            if (!isLinux)
                return Windows.GetLastError();
            return 0;
        }

        public static bool GetModuleInformation(
            [In] IntPtr ProcessHandle,
            [In] IntPtr ModuleHandle,
            [Out] out ModuleInfo ModInfo,
            [In] int Size
            )
        {
            if (!isLinux)
                return Windows.GetModuleInformation(ProcessHandle, ModuleHandle, out ModInfo, Size);
            else
            {
                ModInfo = new ModuleInfo();
                return false;
            }
        }

        public static bool EnumProcessModulesEx(
            [In] IntPtr ProcessHandle,
            [Out] IntPtr[] ModuleHandles,
            [In] int Size,
            [Out] out int RequiredSize,
            [In] uint dwFilterFlag
            )
        {
            if (!isLinux)
                return Windows.EnumProcessModulesEx(ProcessHandle, ModuleHandles, Size, out RequiredSize, dwFilterFlag);
            else
            {
                RequiredSize = 0;
                return false;
            }
        }

        public static bool EnumProcessModules(
            [In] IntPtr ProcessHandle,
            [Out] IntPtr[] ModuleHandles,
            [In] int Size,
            [Out] out int RequiredSize
            )
        {
            if (!isLinux)
            {
                return Windows.EnumProcessModules(ProcessHandle, ModuleHandles, Size, out RequiredSize);
            }
            else
            {
                RequiredSize = 0;
                return false;
            }
        }

        public static uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName,
            [In] [MarshalAs(UnmanagedType.U4)] int nSize)
        {
            if (!isLinux)
            {
                return Windows.GetModuleFileNameEx(hProcess, hModule, lpBaseName, nSize);
            }
            else
            {
                return 0;
            }
        }

        //Umstandsweg dank Microsoft
        public static List<MyProcessModule> GetProcessModules(Process process)
        {
            IntPtr processHandle = process.Handle;
            List<MyProcessModule> modules = new List<MyProcessModule>();

            IntPtr[] modhHandles = new IntPtr[0];
            int lpcbNeeded = 0;

            try
            {
                EnumProcessModulesEx(processHandle, modhHandles, 0, out lpcbNeeded, LIST_MODULES_ALL);

                modhHandles = new IntPtr[lpcbNeeded / IntPtr.Size];
                EnumProcessModulesEx(processHandle, modhHandles, modhHandles.Length * IntPtr.Size, out lpcbNeeded, LIST_MODULES_ALL);

                //Zum Test auf 32 Bit System ...
                //EnumProcessModules(processHandle, modhHandles, 0, out lpcbNeeded);
                //EnumProcessModules(processHandle, modhHandles, modhHandles.Length * IntPtr.Size, out lpcbNeeded);
            }
            catch (EntryPointNotFoundException)
            {
                foreach (ProcessModule m in process.Modules)
                {
                    MyProcessModule pm = new MyProcessModule();
                    pm.ModuleName = m.ModuleName;
                    pm.BaseAddress = m.BaseAddress;
                    pm.ModuleMemorySize = m.ModuleMemorySize;
                    modules.Add(pm);
                }
                return modules;
            }

            for (int i = 0; i < modhHandles.Length; i++)
            {
                ModuleInfo modi = new ModuleInfo();
                StringBuilder modName = new StringBuilder(256);
                if (GetModuleFileNameEx(processHandle, modhHandles[i], modName, modName.Capacity) != 0)
                    if (GetModuleInformation(processHandle, modhHandles[i], out modi, System.Runtime.InteropServices.Marshal.SizeOf(modi)))
                    {
                        MyProcessModule pm = new MyProcessModule();
                        pm.ModuleMemorySize = modi.SizeOfImage;
                        pm.BaseAddress = modi.BaseOfDll;
                        string modFileName = Path.GetFileName(modName.ToString());
                        Debug.Print(modFileName.ToString());
                        pm.ModuleName = modFileName.ToString();
                        modules.Add(pm);
                    }
            }
            return modules;
        }
        #region MemorySearchFunctions
        /*private static void findItemsClass(IntPtr handle, RemoteMemoryStream rms, uint start, uint size)
        {
            rms.InitCache(start, start + size);
            Application.DoEvents();
            try
            {
                uint i = 0;
                uint v;
                uint w;
                BinaryReader binr = new BinaryReader(rms);
                for (i = 0; i < size - 0x5C; i += 4)
                {
                    rms.Seek(start + i, SeekOrigin.Begin);
                    //v = mem[(i) / 4];
                    v = binr.ReadUInt32();
                    if (v != SwfClassToken)
                        continue;

                    rms.Seek(start + i, SeekOrigin.Begin);
                    fClass economic = new fClass(rms, "mResources_vector");

                    fClass items = economic.gC("mResources_vector");
                    itemEntries = new List<CItemEntry>();
                    CItemEntry.reset();
                    foreach (fClass it in items.getClassList("dResourceDefaultDefinition"))
                    {
                        string n = it.gSTR("resourceName_string");
                        Debug.Print("{0} {1}", it.gINT("amount"), n);
                        if ((n != "Population") && (n != "HardCurrency"))
                            itemEntries.Add(new CItemEntry(it));
                    }
                }
            }
            catch (EndOfStreamException e)
            {
                return;
            }
        }*/
        private static void findMainClass(IntPtr handle, RemoteMemoryStream rms, Int64 start, Int64 size)
        {
            rms.InitCache(start, start + size);
            Application.DoEvents();
            try
            {
                uint i = 0;
                uint v;
                uint w;
                uint swftoken;
                BinaryReader binr = new BinaryReader(rms);
                for (i = 0; i < size - 0x5C; i += 4)
                {
                    rms.Seek(start + i, SeekOrigin.Begin);
                    //v = mem[(i) / 4];
                    v = binr.ReadUInt32();
                    if ((v < (uint)npswf.BaseAddress) || (v > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                        continue;
                    swftoken = v;

                    rms.Seek(start+i, SeekOrigin.Begin);
                    fClass player = new fClass(rms, "cPlayerData");

                    /*if (start + i > 0xBEF0000)
                    {
                        v = binr.ReadUInt32();
                        v = binr.ReadUInt32();
                        if ((v & 0xFFFF) == 4)
                        {
                            string s = player.getClassName();
                            if (s != "")
                                Debug.Print("{0:x} Classname {1}",start+i, s);
                        }
                    }*/

                    if (player.gUINT("mGeologistsAmount") > 25) continue;
                    if (player.gUINT("mExplorersAmount") > 25) continue;
                    if (player.gUINT("mPlayerId") == 0) continue;
                    if (player.gUINT("mPlayerLevel") > 50) continue;
                    if (player.gUINT("mPlayerLevel") == 0) continue;
                    //Debug.Print("P level {0}", player.gUINT("mPlayerLevel"));
                    if (player.gUINT("mGeneralsAmount") > 25) continue;
                    v = player.gUINT("mCurrentBuildingsCountAll");
                    w = player.gUINT("mCurrentMaximumBuildingsCountAll");
                    if (w < v) continue;
                    if (v == 0) continue;
                    if (v > 1000) continue;
                    if (w > 1000) continue;
                    if (w == 0) continue;
                    Debug.Print("Main class at?: {0:x}", start + i);
                    Debug.Print("Buildings {0}/{1}", w, v);

                    //if (w == 135) Debugger.Break();
                    fClass resources = player.gO("mGeneralInterface.mCurrentPlayerZone.map_PlayerID_Resources", player.gUINT("mPlayerId"), "cResources");
                    if (resources == null) continue;
                    fClass items = resources.gC("mResources_vector");
                    Debug.Print("mResources_vector: {0:x}", resources.gUINT("mResources_vector"));
                    itemEntries = new List<CItemEntry>();
                    CItemEntry.reset();
                    foreach (fClass it in items.getClassList("dResource"))
                    {
                        string n=it.gSTR("name_string");
                        Debug.Print("{0} {1}", it.gINT("amount"), n);
                        if((n!="Population")&&(n!="HardCurrency"))
                            itemEntries.Add(new CItemEntry(it));
                    }

                    MainClass = start + i;

                    fClass fdeposits = player.gC("mGeneralInterface.mCurrentPlayerZone.mStreetDataMap.mDeposits_vector");
                    Debug.Print("Deposits at?: {0:x}", fdeposits.getOffset());

                    foreach (fClass r in fdeposits.getClassList("cDeposit"))
                    {
                        CResourceEntry RE = new CResourceEntry(r);
                        Debug.Print("{0}", RE.Name);
                        if (!(((RE.Name == "Wood") || (RE.Name == "RealWood")) && !Params.trees))
                            resourceEntries.Add(RE);
                    }

                    DbConnection3.Open();
                    fClass fbuildings = player.gC("mGeneralInterface.mCurrentPlayerZone.mStreetDataMap.mBuildings_vector");
                    Buildings = new Dictionary<String, uint>();
                    buildingEntries = new List<CBuildingEntry>();
                    Debug.Print("Buildings at?: {0:x}", fbuildings.getOffset());
                    foreach (fClass b in fbuildings.getClassList("cBuilding"))
                    {
                        CBuildingEntry BE = new CBuildingEntry(b);
                        Debug.Print("{0} {1} {2}", BE.Name, BE.X, BE.Y);
                        Debug.Print("{0}", BE.wayTime1);
                        Debug.Print("{0}", BE.wayTime2);
                        if (!Buildings.ContainsKey(BE.Name))
                            Buildings.Add(BE.Name, 1);
                        else
                            Buildings[BE.Name]++;
                        buildingEntries.Add(BE);
                    }
                    DbConnection3.Close();

                    MainClass = start + i;
                    SwfClassToken = swftoken;
                    Debug.Print("Main class at: {0:x}", start + i);

                    Debug.Print("{0:x}", player.gC("mGeneralInterface.mCurrentPlayerZone").getOffset());
                    rms.RemoveCache();
                }
            }
            catch (EndOfStreamException e)
            {
                return;
            }
            return;
        }
        #endregion
        public static bool connect()
        {
            uint MaxAddress = 0x7fffffff;
            Int64 address = 0;
            bool result;

            itemEntries = new List<CItemEntry>();
            resourceEntries = new List<CResourceEntry>();
            string[] processes;
            if (!isLinux)
                processes = new string[] { "plugin-container", "iexplore", "chrome" }; //plugin-container für Chrome und Firefox ... IE macht wieder sein eigenes Ding
            else
                processes = new string[] { "plugin-containe", "plugin-container" };
            foreach (string pname in processes)
            {
                List<MyProcess> pList = new List<MyProcess>();

                if (!isLinux)
                    foreach (Process p in Process.GetProcessesByName(pname))
                        pList.Add(new MyProcess(p));
                else
                    foreach (LinuxProcess p in Linux.GetProcessesByName(pname))
                        pList.Add(new MyProcess(p));


                foreach (MyProcess p in pList)
                {
                    Main = p;
                    npswf = null;
                    Debug.Print("Process: {0}", pname);

                    if (!isLinux)
                    {
                        foreach (MyProcessModule mo in GetProcessModules(p.Process))
                        {
                            Debug.Print(mo.ModuleName.ToUpper());
                            if (mo.ModuleName.ToUpper() == "NPSWF32.DLL") //wird von Firefox geladen
                            {
                                npswf = mo;
                                break;
                            }
                            if (mo.ModuleName.ToUpper() == "GCSWF32.DLL") //wird von Chrome geladen
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
                    }
                    else
                    {
                        foreach (MyProcessModule mo in Linux.GetProcessModules(p.LinuxProcess))
                        {
                            Debug.Print("Module ...");
                            Debug.Print(mo.ModuleName.ToUpper());
                            if (mo.ModuleName.ToUpper() == "LIBFLASHPLAYER.SO")
                            {
                                npswf = mo;
                                int i = 0;
                                foreach (MyProcessModule mo2 in Linux.GetProcessModules(p.LinuxProcess))
                                    if (mo2.ModuleName.ToUpper() == "LIBFLASHPLAYER.SO")
                                        if (i++ >= 1)
                                            npswf.ModuleMemorySize += mo2.ModuleMemorySize;
                                break;
                            }
                        }
                    }

                    Debug.Print("End module list loop");

                    if (npswf == null) continue; //nix gefunden ... versuche es mit nächstem Prozess

                    Debug.Print("npswf found ...");

                    RemoteMemoryStream rms = new RemoteMemoryStream(p.Handle);
                    Int64 size;
                    uint br = 0;
                    address = 0;
                    MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();
                    do
                    {
                        result = VirtualQueryEx(p.Handle, (IntPtr)address, out m, (uint)Marshal.SizeOf(m));
                        if (!result) break; //am ende angekommen ... wir können aufhören

                        Debug.Print("Searching in:{0:x} - {1:x} Size: {2:x}", (long)m.BaseAddress, (long)m.BaseAddress + (long)m.RegionSize, m.RegionSize);
                        size = m.RegionSize.ToInt64();
                        if (size > Params.maxmemsize)
                        {
                            address = m.BaseAddress.ToInt64() + m.RegionSize.ToInt64();
                            continue;
                        }
                        if (size == 0)
                        {
                            address = m.BaseAddress.ToInt64() + m.RegionSize.ToInt64();
                            continue;
                        }
                        rms.Seek(m.BaseAddress, SeekOrigin.Begin);

                        findMainClass(p.Handle, rms, m.BaseAddress.ToInt64(), m.RegionSize.ToInt64());
                        if (buildingEntries!=null)
                        {
                            break;
                        }

                        //if ((fClass.Count != 0) && (Main != null) && ((itemEntries.Count > 0) && (!Params.buildingsonly))) break;

                        address = m.BaseAddress.ToInt64() + m.RegionSize.ToInt64();

                    } while (address <= MaxAddress);

                    if ((Main != null) && (itemEntries.Count > 0 && (!Params.buildingsonly))) break;
                }
                if ((Main != null) && ((itemEntries.Count > 0) && (!Params.buildingsonly))) break;
            }

            if ((Main == null) || ((itemEntries.Count == 0) && (!Params.buildingsonly)))
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
                return false;
            }
            return true;
        }
    }
    public class MyProcess
    {
        public IntPtr Handle;
        public Process Process;
        public LinuxProcess LinuxProcess;
        public MyProcess(LinuxProcess p)
        {
            this.Handle = p.Handle;
            this.LinuxProcess = p;
        }
        public MyProcess(Process p)
        {
            this.Process = p;
            this.Handle = p.Handle;
        }
    }
    public class MyProcessModule
    {
        public string ModuleName;
        public IntPtr BaseAddress;
        public int ModuleMemorySize;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ModuleInfo
    {
        public IntPtr BaseOfDll;
        public int SizeOfImage;
        public IntPtr EntryPoint;
    }
    public struct MEMORY_BASIC_INFORMATION
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public uint AllocationProtect;
        public IntPtr RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
    }
    public static class Params
    {
        public static bool usecustomdb = false;
        public static bool usetxt = false;
        public static bool usesqlite = false;
        public static bool trees = true;
        public static uint maxmemsize = 0x300000;
        public static uint maxsearchoffset = 0x1E0000;
        public static bool buildingsonly = false;
    }



    [Guid("9B105525-AE1C-4ea8-8777-8AAB5AB026EF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDSOE_VBAInterface
    {
        [DispId(1)]
        string getBuildingsCSV();
        [DispId(2)]
        string getResourcesCSV();
        [DispId(3)]
        string getItemsCSV();
    }

    [Guid("B73E5033-0042-4b59-B9FF-1AD5267660C4")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("DSO_Economic")]
    public class DSOE_VBAInterface : IDSOE_VBAInterface
    {
        public DSOE_VBAInterface()
        {
            Global.init();
            Global.connect();
        }
        public string getResourcesCSV()
        {
            try
            {
                if (Global.connected)
                    return Global.export_resources();
                return "";
            }
            catch (Exception e)
            {

                return e.ToString() + '\n' + e.StackTrace + '\n' + e.InnerException + '\n';
            }
        }
        public string getBuildingsCSV()
        {
            try
            {
                if (Global.connected)
                    return Global.export_buildings();
                return "";
            }
            catch (Exception e)
            {

                return e.ToString() + '\n' + e.StackTrace + '\n' + e.InnerException + '\n';
            }
        }
        public string getItemsCSV()
        {
            try
            {
                if (Global.connected)
                    return Global.export_items();
                return "";
            }
            catch (Exception e)
            {

                return e.ToString() + '\n' + e.StackTrace + '\n' + e.InnerException + '\n';
            }
        }
    }
}
