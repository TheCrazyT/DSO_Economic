using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DSO_Economic
{
    class RemoteMemoryStream : MemoryStream
    {
        private IntPtr phandle;
        private long pos = 0;
        private uint cache_start;
        private uint cache_end;
        private MemoryStream cache;
        public void InitCache(uint start, uint end)
        {
            uint br=0;
            this.cache_start = start;
            this.cache_end = end;
            byte[] mem = new byte[end - start];
            Global.ReadProcessMemory(phandle, start, mem, end - start, ref br);
            this.cache = new MemoryStream(mem, 0, (int)br);
        }
        public void RemoveCache()
        {
            this.cache_start = 0;
            this.cache_end = 0;
            this.cache = null;
        }
        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override int Capacity { get; set; }
        public override long Length
        {
            get
            {
                return 0xFFFFFFFF;
            }
        }
        public override long Position { get { return pos; } set { throw new Exception("Not implemented"); } }
        public RemoteMemoryStream(IntPtr phandle)
        {
            this.phandle = phandle;
        }
        public override void Flush()
        {
            throw new Exception("Not implemented");
        }

        public override byte[] GetBuffer()
        {
            throw new Exception("Not implemented");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if(offset!=0)
                throw new Exception("Not implemented");
            uint br = 0;
            if ((cache != null)&&(pos>=cache_start)&&(pos<=cache_end))
            {
                cache.Seek(pos-cache_start,SeekOrigin.Begin);
                br=(uint)cache.Read(buffer, offset, count);
            }
            else
            {
                Global.ReadProcessMemory(phandle, (uint)pos, buffer, (uint)count, ref br);
            }
            pos += br;
            return (int)br;
        }
        public override int ReadByte()
        {
            throw new Exception("Not implemented");
        }
        public override long Seek(long offset, SeekOrigin loc)
        {
            switch (loc)
            {
                case SeekOrigin.Begin:
                    this.pos = offset;
                    break;
                case SeekOrigin.Current:
                    this.pos += offset;
                    break;
                case SeekOrigin.End:
                    throw new Exception("Not implemented");
                    break;
            }
            return pos;
        }

        public override void SetLength(long value)
        {
            throw new Exception("Not implemented");
        }
        public override byte[] ToArray()
        {
            throw new Exception("Not implemented");
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new Exception("Not implemented");
        }
        public override void WriteByte(byte value)
        {
            throw new Exception("Not implemented");
        }
        public override void WriteTo(Stream stream)
        {
            throw new Exception("Not implemented");
        }

    }
}
