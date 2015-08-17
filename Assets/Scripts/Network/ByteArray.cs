using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FiveVsFive
{
    class ByteArray : MemoryStream
    {
        public void write(int data)
        {
            byte[] bits=BitConverter.GetBytes(data);
            this.Write(bits, 0, 4);
        }
        public void write(byte data)
        {
            this.WriteByte(data);
        }

        public void write(string data)//因为字符串长度不定，因此将第一个4字节用作字符串长度
        {
            byte[] bits = Encoding.UTF8.GetBytes(data);
            byte[] len = BitConverter.GetBytes(bits.Length);
            this.Write(len, 0, 4);
            this.Write(bits, 0, bits.Length);
        }

        public byte[] encode()
        {
            byte[] data = this.ToArray();
            this.Close();
            //加密
            return data;
        }


        public void decode(byte[] data)
        {
            this.Write(data, 0, data.Length);
            //解密

            this.Position = 0;
        }
        public int readInt()
        {
            byte[] bits=new byte[4];
            this.Read(bits,0,4);
            return BitConverter.ToInt32(bits, 0);
        }
        public byte readByte()
        {
            return (byte)this.ReadByte();
        }
        public string readString()
        {
            byte[] lenBits = new byte[4];
            this.Read(lenBits, 0, 4);
            int len = BitConverter.ToInt32(lenBits, 0);

            byte[] bits = new byte[len];
            this.Read(bits, 0, len);
            return Encoding.UTF8.GetString(bits);
        }
    }
}
