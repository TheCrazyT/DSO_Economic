using System;
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
        public static Process Main = null;
        public static List<String> itemnames;
        public static OdbcConnection DbConnection;
        public static OdbcConnection DbConnection2;
        public static OdbcConnection DbConnection3;
        public static string tblext = "";


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
