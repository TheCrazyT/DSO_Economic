using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.Diagnostics;

namespace DSO_Economic
{
    public class BuildingEntry
    {
        public uint memoffset;
        private uint RCoffset;
        public string Name;

        public double _loadedePTime=0;
        public double _loadedsPTime = 0;
        public double _loadedX = 0;
        public double _loadedY = 0;
        public uint level
        {
            get
            {
                uint br = 0;
                uint[] mem = new uint[1];

                if (!Global.ReadProcessMemory(Global.Main.Handle, memoffset + 0x78, mem, 4, ref br)) return 0;

                return mem[0];
            }
        }
        public bool isActive
        {
            get
            {
                uint br = 0;
                uint[] mem = new uint[1];

                if (!Global.ReadProcessMemory(Global.Main.Handle, memoffset + 0x70, mem, 4, ref br)) return false;

                if (mem[0] == 1)
                    return true;
                else
                    return false;
            }
        }
        public double sPTime
        {
            get
            {
                uint br = 0;
                double[] mem = new double[1];

                uint[] mem2 = new uint[4];
                if (!Global.ReadProcessMemory(Global.Main.Handle, memoffset + 0xB4, mem2, 4, ref br)) return 0;
                RCoffset = mem2[0];

                if (RCoffset == 0) return 0;
                if (!Global.ReadProcessMemory(Global.Main.Handle, RCoffset + 0x68, mem, 8, ref br)) return 0;

                double result = mem[0];
                if ((result == -1) || (result == 0)) return _loadedsPTime;
                return result;
            }
        }
        public double ePTime
        {
            get
            {
                uint br = 0;
                double[] mem = new double[1];

                uint[] mem2 = new uint[4];
                if (!Global.ReadProcessMemory(Global.Main.Handle, memoffset + 0xB4, mem2, 4, ref br)) return 0;
                RCoffset = mem2[0];

                
                if (RCoffset == 0) return 0;
                if (!Global.ReadProcessMemory(Global.Main.Handle, RCoffset + 0x70, mem, 8, ref br)) return 0;

                double result = mem[0];
                if ((result == -1) || (result == 0)) return _loadedePTime;
                return result;
            }
        }
        public double X
        {
            get
            {
                uint br = 0;
                double[] mem = new double[1];

                if (!Global.ReadProcessMemory(Global.Main.Handle, memoffset + 0x20, mem, 8, ref br)) return 0;

                return mem[0];
            }
        }
        public double Y
        {
            get
            {
                uint br = 0;
                double[] mem = new double[1];

                if (!Global.ReadProcessMemory(Global.Main.Handle, memoffset + 0x28, mem, 8, ref br)) return 0;

                return mem[0];
            }
        }
        public BuildingEntry(uint offset)
        {
            Name = "";
            this.memoffset = offset;

            uint br = 0;
            uint[] mem2 = new uint[4];

            if (!Global.ReadProcessMemory(Global.Main.Handle, offset + 0x9C, mem2, 4, ref br)) return;

            if (!Global.ReadProcessMemory(Global.Main.Handle, mem2[0] + 0x08, mem2, 4 * 3, ref br)) return;

            byte[] mem = new byte[mem2[2]];
            if (!Global.ReadProcessMemory(Global.Main.Handle, mem2[0], mem, mem2[2], ref br)) return;

            Name = Encoding.UTF8.GetString(mem);
            if (Name == null) Name = "";

            if (!Global.ReadProcessMemory(Global.Main.Handle, offset + 0xB4, mem2,4, ref br)) return;
            RCoffset = mem2[0];
            load();
        }
        public void load()
        {
            OdbcCommand DbCommand = Global.DbConnection3.CreateCommand();
            DbCommand.CommandText = "SELECT sPTime,ePTime FROM BuildingProductionTimes WHERE X=? AND Y=?";
            DbCommand.Parameters.Add("X", OdbcType.Double).Value = X;
            DbCommand.Parameters.Add("Y", OdbcType.Double).Value = Y;
            OdbcDataReader odr=DbCommand.ExecuteReader();
            if (odr.Read())
            {
                this._loadedsPTime = odr.GetDouble(0);
                this._loadedePTime = odr.GetDouble(1);
            }
            else
            {
                this._loadedsPTime = -1;
                this._loadedePTime = -1;
            }
        }
        public void save()
        {
            if ((ePTime == 0) || (sPTime == 0)) return;
            if ((ePTime == -1) || (sPTime == -1)) return;
            OdbcCommand DbCommand = Global.DbConnection3.CreateCommand();
            DbCommand.CommandText = "INSERT INTO BuildingProductionTimes (X,Y,sPTime,ePTime) VALUES (?,?,?,?)";
            DbCommand.Parameters.Add("X", OdbcType.Double).Value = X;
            DbCommand.Parameters.Add("Y", OdbcType.Double).Value = Y;
            DbCommand.Parameters.Add("sPTime", OdbcType.Double).Value = sPTime;
            DbCommand.Parameters.Add("ePTime", OdbcType.Double).Value = ePTime;
            try
            {
                DbCommand.ExecuteNonQuery();
            }
            catch (OdbcException error)
            {
                Debug.Write(error);
            }

            DbCommand = Global.DbConnection3.CreateCommand();
            DbCommand.CommandText = "UPDATE BuildingProductionTimes SET sPTime=?,ePTime=? WHERE X=? AND Y=?";
            DbCommand.Parameters.Add("sPTime", OdbcType.Double).Value = sPTime;
            DbCommand.Parameters.Add("ePTime", OdbcType.Double).Value = ePTime;
            DbCommand.Parameters.Add("X", OdbcType.Double).Value = X;
            DbCommand.Parameters.Add("Y", OdbcType.Double).Value = Y;
            DbCommand.ExecuteNonQuery();
        }
    }
}
