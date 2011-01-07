using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.Diagnostics;

namespace DSO_Economic
{

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
                Global.ReadProcessMemory(Global.Main.Handle, (IntPtr)(memoffset), mem, 0x20, ref br);
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
                Global.ReadProcessMemory(Global.Main.Handle, (IntPtr)(memoffset), mem, 0x20, ref br);
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
            if (_ID < Global.itemnames.Count)
                _Name = Global.itemnames[(int)_ID];
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
            if (_ID < Global.itemnames.Count)
                this._Name = Global.itemnames[(int)_ID];
            else
                this._Name = "";
        }
        public void save()
        {
            try
            {
                OdbcCommand DbCommand = Global.DbConnection3.CreateCommand();
                //DbCommand.CommandText = "INSERT INTO History" + Global.tblext + " (ID,[DateTime],Amount) VALUES (" + ID + ",CDate('" + DateTime.Now + "')," + amount + ")";
                DbCommand.CommandText = "INSERT INTO History" + Global.tblext + " (ID,[DateTime],Amount) VALUES (?,?,?)";
                DbCommand.Parameters.Add("ID", OdbcType.Int).Value = ID;
                DbCommand.Parameters.Add("CurrentDate", OdbcType.DateTime).Value = DateTime.Now;
                DbCommand.Parameters.Add("Amount", OdbcType.Int).Value = amount;
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
                if ((Global.Main != null) && (memoffset != 0))
                {
                    Global.ReadProcessMemory(Global.Main.Handle, (IntPtr)(memoffset), mem, 0x20, ref br);
                    return _Name + ": " + amount;
                }
                Debug.Print("ID:{0} memoffset:{1:x}", ID, memoffset);
                return "";
            }
        }
    }
}
