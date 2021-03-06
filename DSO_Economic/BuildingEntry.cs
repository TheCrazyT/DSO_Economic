﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.Diagnostics;
using FlashABCRead;
using System.IO;

namespace DSO_Economic
{
    public class CBuildingEntry
    {
        private fClass cls;
        public string Name;

        public double _loadedePTime=0;
        public double _loadedsPTime = 0;
        public double _loadedX = 0;
        public double _loadedY = 0;
        public uint level
        {
            get
            {
                return cls.gUINT("mUpgradeLevel");
            }
        }
        public bool isActive
        {
            get
            {
                if (cls.gUINT("mProductionActive") == 1)
                    return true;
                else
                    return false;
            }
        }
        public double wayTime1
        {
            get
            {
                if(cls.gUINT("mResourceCreation")==0)return 0;
                if (cls.gUINT("mResourceCreation.path") == 0) return 0;
                return ((double)cls.gUINT("mResourceCreation.path.pathLenX10000"))/2500.0;
            }
        }
        public double wayTime2
        {
            get
            {
                if (cls.gUINT("mResourceCreation") == 0) return 0;
                if (cls.gUINT("mResourceCreation.depositPath") == 0) return wayTime1;
                return ((double)cls.gUINT("mResourceCreation.depositPath.pathLenX10000")) / 2500.0;
            }
        }
        public double wayTime
        {
            get
            {
                double wt1 = wayTime1;
                double wt2 = wayTime2;
                if (wt1 == 0) return 0;
                if (wt2 == 0) return 0;
                return wt1 + wt2;
            }
        }
        public double PTime
        {
            get
            {
                if (cls.gUINT("mResourceCreation") == 0) return 0;
                if (cls.gUINT("mResourceCreation.resourceCreationDefinition") == 0) return 0;

                return cls.gUINT("mResourceCreation.resourceCreationDefinition.workTime")*1;
            }
        }
        public double sPTime { get{return -1;}}
        public double ePTime { get{return -1;}}
        /*public double sPTime
        {
            get
            {
                //double result = cls.gDBL("mResourceCreation.startProductionTime");
                double result = 0;
                if ((result == -1) || (result == 0)) return _loadedsPTime == 0 ? -1 : _loadedsPTime;
                return result;
            }
        }
        public double ePTime
        {
            get
            {
                //double result = cls.gDBL("mResourceCreation.endProductionTime");
                double result = 0;
                if ((result == -1) || (result == 0)) return _loadedePTime == 0 ? -1 : _loadedePTime;
                return result;
            }
        }*/
        public uint X
        {
            get
            {
                return cls.gUINT("mBuildingGrid");
            }
        }
        public uint Y
        {
            get
            {
                return 0;
            }
        }
        public CBuildingEntry(fClass c)
        {
            this.cls = c;
            Name = cls.gSTR("mBuildingName_string");
        }
        public long getBuffs()
        {
            fClass buffs = cls.gC("mBuffs_vector");
            if (buffs == null) return 0;
            return buffs.getClassList("cBuff").Count;
        }
        public void load()
        {
            /*OdbcCommand DbCommand = Global.DbConnection3.CreateCommand();
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
            odr.Close();*/
        }
        public void save()
        {
            /*try
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
            catch (EndOfStreamException e)
            {
                Debug.Print("{0}", e);
            }*/
        }
    }
}
