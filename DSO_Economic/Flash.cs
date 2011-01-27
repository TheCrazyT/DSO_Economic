using System;
using System.Collections.Generic;
using System.Text;

namespace DSO_Economic
{
    class Flash
    {
        static public string getString(uint offset)
        {
            string txt = "";
            uint br = 0;
            uint[] mem2 = new uint[4];

            if (!Global.ReadProcessMemory(Global.Main.Handle, offset, mem2, 4, ref br)) return txt;

            if (!Global.ReadProcessMemory(Global.Main.Handle, mem2[0] + 0x08, mem2, 4 * 3, ref br)) return txt;
            byte[] mem = new byte[mem2[2]];
            if (!Global.ReadProcessMemory(Global.Main.Handle, mem2[0], mem, mem2[2], ref br)) return txt;

            txt = Encoding.UTF8.GetString(mem);
            if (txt == null) txt = "";

            return txt;
        }
        static public string getString2(uint offset)
        {
            uint br = 0;
            uint[] mem2 = new uint[4];

            if (!Global.ReadProcessMemory(Global.Main.Handle, offset, mem2, 4, ref br)) return "";

            if (!Global.ReadProcessMemory(Global.Main.Handle, mem2[0] + 0x08, mem2, 4 * 3, ref br)) return "";
            uint reloffset = mem2[0];
            offset = mem2[1];
            uint size = mem2[2];
            byte[] mem = new byte[size];
            if (!Global.ReadProcessMemory(Global.Main.Handle, offset + 0x08, mem2, 4, ref br)) return "";
            offset = mem2[0];
            if (!Global.ReadProcessMemory(Global.Main.Handle, offset + reloffset, mem, size, ref br)) return "";


            string Name = Encoding.UTF8.GetString(mem);
            if (Name == null) Name = "";
            return Name;
        }
        static public List<uint> getOffsetList(uint offset)
        {
            List<uint> list=new List<uint>();
            uint br = 0;
            uint[] mem = new uint[5];

            if (!Global.ReadProcessMemory(Global.Main.Handle, offset, mem, 4, ref br)) return list;

            if (!Global.ReadProcessMemory(Global.Main.Handle, mem[0] + 0x10, mem, 0x10, ref br)) return list;

            offset = mem[3];
            uint cnt = mem[0];

            mem = new uint[cnt];
            if (!Global.ReadProcessMemory(Global.Main.Handle, offset, mem, cnt * 4, ref br)) return list;

            for (int x = 0; x < cnt; x++)
                list.Add(mem[x] & 0xFFFFFFF8);
            return list;

        }
    }
}
