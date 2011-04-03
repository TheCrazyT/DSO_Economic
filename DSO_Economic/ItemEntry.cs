using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.Diagnostics;
using FlashABCRead;

namespace DSO_Economic
{

    public class CItemEntry
    {
        private uint _ID;
        private static int last_ID = -1;
        private fClass cls;
        public string amountStr
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
                return cls.gUINT("amount");
            }
        }
        public uint max
        {
            get
            {
                return cls.gUINT("maxLimit");
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
        public string internName
        {
            get
            {
                return cls.gSTR("name_string");
            }
        }

        public CItemEntry(fClass c)
        {
            _ID = (uint)(last_ID + 1);
            last_ID = (int)_ID;
            this.cls = c;
            if (_ID < Global.itemnames.Count)
                _Name = Global.itemnames[(int)_ID];
            else
                _Name = "";
        }
        public static void reset()
        {
            CItemEntry.last_ID = -1;
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
                return _Name + ": " + amount;
            }
        }
    }
}
