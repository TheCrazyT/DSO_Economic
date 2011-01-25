using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DSO_Economic
{
    public static class Linux
    {
        [DllImport("libc.so")]
        static extern long ptrace(int request, IntPtr pid, IntPtr addr, out byte[] data);

        [DllImport("libc.so")]
        static extern long waitpid(IntPtr pid, int a, int b);

        public const int PTRACE_ATTACH = 0x10;
        public const int PTRACE_DETACH = 0x11;
        static public bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength)
        {
            Debug.Print("Linux.VirtualQueryEx {0} {1}", hProcess, lpAddress);
            lpBuffer = new MEMORY_BASIC_INFORMATION();
            string txt = File.ReadAllText("/proc/" + hProcess + "/maps");
            string[] lines = txt.Split('\n');
            foreach (string l in lines)
            {
                //Debug.Print("{0}", l);
                Regex r = new Regex("^([a-f0-9]+)-([a-f0-9]+) +.{4} +[a-f0-9]+ +[0-9:]+ +[0-9]+ +(.+)$");
                if (r.IsMatch(l))
                {
                    Match m = r.Match(l);
                    //Debug.Print("-->{0} {1} {2}<--", m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
                    int start = (int)Convert.ToUInt32(m.Groups[1].Value, 16);
                    int size = (int)(Convert.ToUInt32(m.Groups[2].Value, 16) - (long)start);
                    if ((lpAddress == null) || ((uint)lpAddress <= (uint)start))
                    {
                        lpBuffer.BaseAddress = (IntPtr)start;
                        lpBuffer.RegionSize = (IntPtr)size;

                        return true;
                    }
                }
            }
            return false;
        }
        static public bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, ref int lpNumberOfBytesRead)
        {
            Debug.Print("Linux.ReadProcessMemory {0} {1} {2}", hProcess, (uint)lpBaseAddress, nSize);
            byte[] data = new byte[4];
            ptrace(PTRACE_ATTACH, hProcess, (IntPtr)0, out data);
            waitpid(hProcess, 0, 0);

            try
            {
                BinaryReader br = new BinaryReader(new FileStream("/proc/" + hProcess + "/mem", FileMode.Open));
                br.BaseStream.Seek((uint)lpBaseAddress, SeekOrigin.Begin);
                lpNumberOfBytesRead = br.Read(lpBuffer, 0, nSize);
                br.Close();

                ptrace(PTRACE_DETACH, hProcess, (IntPtr)0, out data);

                if (lpNumberOfBytesRead > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.Print("{0}",e);
                return false;
            }
        }
        static public bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, uint[] lpBuffer, int nSize, ref int lpNumberOfBytesRead)
        {
            Debug.Print("Linux.ReadProcessMemory {0} {1} {2}", hProcess, (uint)lpBaseAddress, nSize);
            byte[] data = new byte[4];
            ptrace(PTRACE_ATTACH, hProcess, (IntPtr)0, out data);
            waitpid(hProcess, 0, 0);

            try
            {
                BinaryReader br = new BinaryReader(new FileStream("/proc/" + hProcess + "/mem", FileMode.Open));
                byte[] buf = new byte[nSize * sizeof(uint)];
                br.BaseStream.Seek((uint)lpBaseAddress, SeekOrigin.Begin);
                lpNumberOfBytesRead = br.Read(buf, 0, nSize);
                br.Close();

                ptrace(PTRACE_DETACH, hProcess, (IntPtr)0, out data);

                System.Buffer.BlockCopy(buf, 0, lpBuffer, 0, nSize);
                if (lpNumberOfBytesRead > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.Print("{0}", e);
                return false;
            }
        }
        static public bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, ulong[] lpBuffer, int nSize, ref int lpNumberOfBytesRead)
        {
            Debug.Print("Linux.ReadProcessMemory {0} {1} {2}", hProcess, (uint)lpBaseAddress, nSize);
            byte[] data = new byte[4];
            ptrace(PTRACE_ATTACH, hProcess, (IntPtr)0, out data);
            waitpid(hProcess, 0, 0);

            try
            {
                BinaryReader br = new BinaryReader(new FileStream("/proc/" + hProcess + "/mem", FileMode.Open));
                byte[] buf = new byte[nSize * sizeof(ulong)];
                br.BaseStream.Seek((uint)lpBaseAddress, SeekOrigin.Begin);
                lpNumberOfBytesRead = br.Read(buf, 0, nSize);
                System.Buffer.BlockCopy(buf, 0, lpBuffer, 0, nSize);
                br.Close();

                ptrace(PTRACE_DETACH, hProcess, (IntPtr)0, out data);

                if (lpNumberOfBytesRead > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.Print("{0}", e);
                return false;
            }
        }
        static public bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, double[] lpBuffer, int nSize, ref int lpNumberOfBytesRead)
        {
            Debug.Print("Linux.ReadProcessMemory {0} {1} {2}", hProcess, (uint)lpBaseAddress, nSize);
            byte[] data = new byte[4];
            ptrace(PTRACE_ATTACH, hProcess, (IntPtr)0, out data);
            waitpid(hProcess, 0, 0);

            try
            {
                BinaryReader br = new BinaryReader(new FileStream("/proc/" + hProcess + "/mem", FileMode.Open));
                byte[] buf = new byte[nSize * sizeof(double)];
                br.BaseStream.Seek((uint)lpBaseAddress, SeekOrigin.Begin);
                lpNumberOfBytesRead = br.Read(buf, 0, nSize);
                System.Buffer.BlockCopy(buf, 0, lpBuffer, 0, nSize);
                br.Close();

                ptrace(PTRACE_DETACH, hProcess, (IntPtr)0, out data);

                if (lpNumberOfBytesRead > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.Print("{0}", e);
                return false;
            }
        }
        static public bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            int lpNumberOfBytesReadInt = 0;

            bool result = Linux.ReadProcessMemory(hProcess, (int)lpBaseAddress, (byte[])lpBuffer, (int)nSize, ref lpNumberOfBytesReadInt);
            lpNumberOfBytesRead = (uint)lpNumberOfBytesReadInt;
            return result;
        }
        static public bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            int lpNumberOfBytesReadInt = 0;

            bool result = Linux.ReadProcessMemory(hProcess, (int)lpBaseAddress, (byte[])lpBuffer, (int)nSize, ref lpNumberOfBytesReadInt);
            lpNumberOfBytesRead = (uint)lpNumberOfBytesReadInt;
            return result;
        }

        static public bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            int lpNumberOfBytesReadInt = 0;

            bool result = Linux.ReadProcessMemory(hProcess, (int)lpBaseAddress, lpBuffer, (int)nSize, ref lpNumberOfBytesReadInt);
            lpNumberOfBytesRead = (uint)lpNumberOfBytesReadInt;
            return result;
        }

        static public bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, uint[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            int lpNumberOfBytesReadInt = 0;

            bool result = Linux.ReadProcessMemory(hProcess, (int)lpBaseAddress, lpBuffer, (int)nSize, ref lpNumberOfBytesReadInt);
            lpNumberOfBytesRead = (uint)lpNumberOfBytesReadInt;
            return result;
        }

        static public bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, ulong[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            int lpNumberOfBytesReadInt = 0;

            bool result = Linux.ReadProcessMemory(hProcess, (int)lpBaseAddress, lpBuffer, (int)nSize, ref lpNumberOfBytesReadInt);
            lpNumberOfBytesRead = (uint)lpNumberOfBytesReadInt;
            return result;
        }

        static public bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, double[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead)
        {
            int lpNumberOfBytesReadInt = 0;

            bool result = Linux.ReadProcessMemory(hProcess, (int)lpBaseAddress, lpBuffer, (int)nSize, ref lpNumberOfBytesReadInt);
            lpNumberOfBytesRead = (uint)lpNumberOfBytesReadInt;
            return result;
        }
        public static string GetNameByStatus(string txt)
        {
            string[] lines = txt.Split('\n');
            foreach (string l in lines)
            {
                string[] x = l.Split('\t');
                if (x.Length > 0)
                    if (x[0].Trim() == "Name:")
                        return x[1];
            }
            return "";
        }
        public static List<MyProcessModule> GetProcessModules(LinuxProcess process)
        {
            Debug.Print("Linux.GetProcessModules");
            List<MyProcessModule> mpm = new List<MyProcessModule>();
            string txt = File.ReadAllText(process.path + "/maps");
            string[] lines = txt.Split('\n');
            foreach (string l in lines)
            {
                MyProcessModule mo = new MyProcessModule();
                Debug.Print("{0}", l);
                Regex r = new Regex("^([a-f0-9]+)-([a-f0-9]+) +.{4} +[a-f0-9]+ +[0-9:]+ +[0-9]+ +(.+)$");
                if (r.IsMatch(l))
                {
                    Match m = r.Match(l);

                    Debug.Print("-->{0} {1} {2}<--", m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);

                    mo.BaseAddress = (IntPtr)Convert.ToInt32(m.Groups[1].Value, 16);
                    mo.ModuleMemorySize = (int)(Convert.ToUInt32(m.Groups[2].Value, 16) - (long)mo.BaseAddress);
                    mo.ModuleName = Path.GetFileName(m.Groups[3].Value);
                }
                else
                {
                    mo.ModuleName = "empty";
                }
                mpm.Add(mo);
            }
            Debug.Print("{0} Modules found.", mpm.Count);
            return mpm;
        }
        public static List<LinuxProcess> GetProcessesByName(string name)
        {
            List<LinuxProcess> lplist = new List<LinuxProcess>();
            string[] directorys = Directory.GetDirectories("/proc/");
            foreach (string d in directorys)
            {
                try
                {
                    if (GetNameByStatus(File.ReadAllText(d + "/status")) == name)
                        lplist.Add(new LinuxProcess(System.IO.Path.GetFileName(d)));
                }
                catch (UnauthorizedAccessException e)
                {
                }
                catch (FileNotFoundException e)
                {
                }
            }
            return lplist;
        }

    }
    public class LinuxProcess
    {
        public IntPtr Handle;
        public string path;
        public LinuxProcess(string id)
        {
            path = "/proc/" + id;
            Handle = (IntPtr)Convert.ToInt32(id);
        }
    }
}