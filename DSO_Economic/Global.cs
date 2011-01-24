﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.Odbc;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
namespace DSO_Economic
{

    static class Global
    {
        public static bool isLinux;
        public static bool connected
        {
            get
            {
                uint br = 0;
                uint[] mem = new uint[1];
                if (!ReadProcessMemory(Main.Handle, MainClass, mem, 4, ref br)) return false;
                if ((mem[0] < (uint)npswf.BaseAddress) || (mem[0] > (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    return false;
                if (!ReadProcessMemory(Main.Handle, MainClass + 0x5c, mem, 4, ref br)) return false;
                if (mem[0] == 0)
                    return false;
                if (!ReadProcessMemory(Main.Handle,mem[0], mem, 4, ref br)) return false;

                return true;
            }
        }
        public static uint BuildingsPointer = 0;
        public static Dictionary<String, uint> Buildings;

        public static MyProcessModule npswf = null;
        public static uint MainClass = 0;
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
                ModuleInfo modi=new ModuleInfo();
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
        private static void findMainClass(IntPtr handle, uint[] mem, uint start, uint size)
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
                v = mem[(i + 0x38) / 4];
                w = mem[(i + 0x40) / 4];
                if (w < v) continue;
                if (v > 1000) continue;
                if (w > 1000) continue;
                if (w == 0) continue;

                w = mem[(i + 0x5c) / 4];
                w += 0x10;

                uint br = 0;
                uint[] mem2 = new uint[4];
                if (!ReadProcessMemory(handle, w, mem2, 4 * 4, ref br)) continue;

                if (mem2[0] != 44) continue;
                if (mem2[1] != 46) continue;

                uint starttbl = mem2[3];
                if ((starttbl > (uint)npswf.BaseAddress) && (starttbl < (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    continue;

                uint sz = (uint)(4 * itemnames.Count);
                mem2 = new uint[itemnames.Count + 1];
                if (!ReadProcessMemory(handle, starttbl + 8, mem2, sz, ref br)) continue;

                MainClass = start + i;
                Debug.Print("Main class at: {0:x}", start + i);


                Debug.Print("Table at: {0:x}", starttbl);
                itemEntries = new List<CItemEntry>();
                CItemEntry.reset();
                for (int x = 0; x < itemnames.Count; x++)
                {
                    CItemEntry ie = new CItemEntry(mem2[x] & 0xFFFFFFF8);
                    Debug.Print(ie.internName);
                    itemEntries.Add(ie);
                }

                mem2 = new uint[1];
                if (!ReadProcessMemory(handle, MainClass + 0x88, mem2, 4, ref br)) continue;

                if (!ReadProcessMemory(handle, mem2[0] + 0x1a8, mem2, 4, ref br)) continue;

                if (!ReadProcessMemory(handle, mem2[0] + 0x68, mem2, 4, ref br)) continue;

                if (!ReadProcessMemory(handle, mem2[0] + 0x54, mem2, 4, ref br)) continue;

                uint[] mem3 = new uint[4];

                if (!ReadProcessMemory(handle, mem2[0] + 0x10, mem3, 0x10, ref br)) continue;

                uint cnt = mem3[0];
                BuildingsPointer = mem3[3];

                mem2 = new uint[cnt];
                if (!ReadProcessMemory(handle, BuildingsPointer, mem2, cnt * 4, ref br)) continue;

                buildingEntries = new List<CBuildingEntry>();
                Buildings = new Dictionary<String, uint>();

                DbConnection3.Open();
                for (int x = 0; x < cnt; x++)
                {
                    CBuildingEntry BE = new CBuildingEntry(mem2[x] & 0xFFFFFFF8);
                    Debug.Print("Building at: {0:x}", mem2[x] & 0xFFFFFFF8);
                    Debug.Print("{0} {1} {2}", BE.Name, BE.X, BE.Y);
                    if (!Buildings.ContainsKey(BE.Name))
                        Buildings.Add(BE.Name, 1);
                    else
                        Buildings[BE.Name]++;
                    buildingEntries.Add(BE);
                }
                DbConnection3.Close();
            }
            return;
        }
        private static void findResources(uint[] mem, uint start, uint size)
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
                if (((v == 5) || (v == 25)) && (!Params.trees))
                    continue;

                if ((v != 160) && (v != 1000)) //Wasser und Getreide werden immer angezeigt ... (1000 und 160 sind dabei die maximale Anzahl an Einheiten, bisher habe ich keinen besseren Weg gefunden)
                    if (v == w)
                        continue;


                resourceEntries.Add(new CResourceEntry(start + i + 0x40));
            }
            return;
        }
        #endregion
        public static bool connect()
        {
            uint MaxAddress = 0x7fffffff;
            long address = 0;
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
                List<MyProcess> pList=new List<MyProcess>();

                if (!isLinux)
                    foreach(Process p in Process.GetProcessesByName(pname))
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
                                break;
                            }
                        }
                    }

                    Debug.Print("End module list loop");

                    if (npswf == null) continue; //nix gefunden ... versuche es mit nächstem Prozess
                    
                    Debug.Print("npswf found ...");

                    uint size;
                    uint br = 0;
                    address = 0;
                    MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();
                    do
                    {
                        result = VirtualQueryEx(p.Handle, (IntPtr)address, out m, (uint)Marshal.SizeOf(m));
                        if (!result) break; //am ende angekommen ... wir können aufhören
                        /*if (m.AllocationBase == null)
                        {
                            address = (long)m.BaseAddress + (long)m.RegionSize;
                            continue;
                        }*/

                        Debug.Print("Searching in:{0:x} - {1:x} Size: {2:x}", (long)m.BaseAddress, (long)m.BaseAddress + (long)m.RegionSize, m.RegionSize);
                        size = (uint)m.RegionSize;
                        if (size > Params.maxmemsize)
                        {
                            address = (long)m.BaseAddress + (long)m.RegionSize;
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
                                address = (long)m.BaseAddress + (long)m.RegionSize;
                                continue;
                            }
                        }
                        if (size == 0)
                        {
                            address = (long)m.BaseAddress + (long)m.RegionSize;
                            continue;
                        }

                        findMainClass(p.Handle, mem, (uint)m.BaseAddress, (uint)m.RegionSize);

                        //ToDo: optimieren ... aus der Hauptklasse rausfischen
                        findResources(mem, (uint)m.BaseAddress, (uint)m.RegionSize); //Rohstoffe sind in mehreren Segmenten enthalten ... also suchen wir alles komplett durch

                        address = (long)m.BaseAddress + (long)m.RegionSize;

                    } while (address <= MaxAddress);

                    if ((Main != null) && (itemEntries.Count > 0)) break;
                }
                if ((Main != null) && (itemEntries.Count > 0)) break;
            }

            if ((Main == null) || (itemEntries.Count == 0))
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
}
