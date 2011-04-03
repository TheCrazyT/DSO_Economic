using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FlashABCRead;

namespace DSO_Economic
{
    public class CResourceEntry : IComparable
    {
        private fClass cls;
        public long amount
        {
            get
            {
                return cls.gUINT("mAmount");
            }
        }
        private uint _ID;
        private static uint lastresourceEntriesID = 0;
        public string Name="";
        public static IComparer<CResourceEntry> SortByAmount
        {
            get
            {
                return ((IComparer<CResourceEntry>)new SortByAmountClass());
            }
        }
        class SortByAmountClass : IComparer<CResourceEntry>
        {
            public int Compare(CResourceEntry a, CResourceEntry b)
            {
                return a.CompareTo(b);
            }
        }
        public uint ID
        {
            get
            {
                return _ID;
            }
        }
        public CResourceEntry(fClass c)
        {
            this._ID = lastresourceEntriesID;
            lastresourceEntriesID++;
            this.cls = c;
            Name = cls.gSTR("mName_string");
        }
        public int CompareTo(object obj)
        {
            return ((CResourceEntry)obj).amount.CompareTo(amount);
        }
        public string Text
        {
            get
            {
                UInt32 br = 0;
                uint[] mem = new uint[(0x40+0x18) / 4];
                if ((Global.Main != null))
                {
                        string n = "";
                        switch (Name)
                        {
                            case "Wood":
                            case "RealWood":
                                n = "Baum";
                                break;
                            case "Coal":
                                n = "Kohle";
                                break;
                            case "Water":
                                n = "Wasser";
                                break;
                            case "Stone":
                                n = "Stein";
                                break;
                            case "Fish":
                                n = "Fisch";
                                break;
                            case "Wild":
                                n = "Wild";
                                break;
                            case "IronOre":
                                n = "Eisen";
                                break;
                            case "BronzeOre":
                                n = "Kupfer";
                                break;
                            case "Marble":
                                n = "Marmor";
                                break;
                            case "Corn":
                                n = "Getreide";
                                break;
                            default:
                                n = "?" + Name;
                                break;

                        }
                        return n + ": " + amount;
                }
                return "";
            }
        }
    }
}
