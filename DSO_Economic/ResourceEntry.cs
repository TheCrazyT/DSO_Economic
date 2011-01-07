using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DSO_Economic
{
    public class ResourceEntry
    {
        private long memoffset;
        public long amount;
        private uint _ID;
        private static uint lastresourceEntriesID=0;
        public uint ID
        {
            get
            {
                return _ID;
            }
        }
        public ResourceEntry(long offset)
        {
            this._ID = lastresourceEntriesID;
            lastresourceEntriesID++;
            this.amount = 0;
            this.memoffset = offset;
        }
        public string Text
        {
            get
            {
                UInt32 br = 0;
                uint[] mem = new uint[0x18 / 4];
                if ((Global.Main != null) && (memoffset != 0))
                {
                    Global.ReadProcessMemory(Global.Main.Handle, (IntPtr)(memoffset), mem, 0x18, ref br);

                    if ((int)mem[0] == 2)
                    {
                        amount = mem[8 / 4];

                        uint max = mem[0x14 / 4];
                        string name = "";
                        switch (max)
                        {
                            case 5:
                            case 25:
                                name = "Baum";
                                break;

                            case 1000:
                                name = "Wasser";
                                break;
                            case 610:
                                name = "Stein";
                                break;
                            case 700:
                            case 680:
                                name = "Fisch";
                                break;
                            case 400:
                                name = "Eisen";
                                break;
                            case 710:
                                name = "Kupfer";
                                break;
                            case 300:
                                name = "Marmor";
                                break;
                            case 160:
                                name = "Getreide";
                                break;
                            default:
                                name = "?" + max;
                                break;

                        }
                        return name + ": " + amount;
                    }
                    else
                        Debug.Print("Invalid ID:{0} memoffset:{1:x}", ID, memoffset);
                }
                Debug.Print("ID:{0} memoffset:{1:x}", ID, memoffset);
                return "";
            }
        }
    }
}
