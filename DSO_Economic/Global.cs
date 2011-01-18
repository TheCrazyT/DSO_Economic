using System;
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
        public static bool connected
        {
            get
            {
                uint br = 0;
                uint[] mem = new uint[1];
                if (!Global.ReadProcessMemory(Main.Handle, MainClass, mem, 4, ref br)) return false;
                if ((mem[0] > (uint)npswf.BaseAddress) && (mem[0] < (uint)((uint)npswf.BaseAddress + npswf.ModuleMemorySize)))
                    return true;
                return false;
            }
        }
        public static MyProcessModule npswf = null;
        public static uint MainClass = 0;
        public const uint LIST_MODULES_ALL = 0x03; 
        public static Process Main = null;
        public static List<String> itemnames;
        public static OdbcConnection DbConnection;
        public static OdbcConnection DbConnection2;
        public static OdbcConnection DbConnection3;

        public static List<CItemEntry> itemEntries;
        public static List<CBuildingEntry> buildingEntries;
        public static List<CResourceEntry> resourceEntries;

        public static string tblext = "";
        public static CProduction Production;

        [DllImport("Kernel32.dll")]
        static public extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("Kernel32.dll")]
        static public extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static public extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static public extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static public extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static public extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, ulong[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static public extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, double[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("Kernel32.dll")]
        static public extern uint GetLastError();


        [DllImport("psapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetModuleInformation(
            [In] IntPtr ProcessHandle,
            [In] IntPtr ModuleHandle,
            [Out] out ModuleInfo ModInfo,
            [In] int Size
            );

        
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcessModulesEx(
            [In] IntPtr ProcessHandle,
            [Out] IntPtr[] ModuleHandles,
            [In] int Size,
            [Out] out int RequiredSize,
            [In] uint dwFilterFlag
            );
        
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcessModules(
            [In] IntPtr ProcessHandle,
            [Out] IntPtr[] ModuleHandles,
            [In] int Size,
            [Out] out int RequiredSize
            );

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName,
            [In] [MarshalAs(UnmanagedType.U4)] int nSize);

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
        public int BaseAddress;
        public int AllocationBase;
        public int AllocationProtect;
        public int RegionSize;
        public int State;
        public int Protect;
        public int Type;
    }
}
