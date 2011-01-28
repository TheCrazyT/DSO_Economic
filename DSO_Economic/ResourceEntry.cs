using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DSO_Economic
{
    public class CResourceEntry : IComparable
    {
        private long memoffset;
        public long amount
        {
            get
            {
                UInt32 br = 0;
                uint[] mem = new uint[(0x40 + 0x18) / 4];
                Global.ReadProcessMemory(Global.Main.Handle, (IntPtr)(memoffset), mem, 0x18 + 0x40, ref br);
                if ((int)mem[0x40 / 4] == 2)
                    return mem[0x48 / 4];
                return 0;
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
        public CResourceEntry(uint offset)
        {
            this._ID = lastresourceEntriesID;
            lastresourceEntriesID++;
            this.memoffset = offset;

            Name=Flash.getString(offset + 0x5C);
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
                if ((Global.Main != null) && (memoffset != 0))
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
                Debug.Print("ID:{0} memoffset:{1:x}", ID, memoffset);
                return "";
            }
        }
    }
}
