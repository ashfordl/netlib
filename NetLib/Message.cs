using System;
using System.Text;

namespace NetLib
{
    public class Message
    {
        public int Code
        {
            get;
            set;
        }

        public byte[] Data
        {
            get;
            set;
        }

        public Message(int code)
        {
            this.Code = code;
            this.Data = new byte[0];
        }

        public Message(byte[] data)
        {
            this.Code = 0;
            this.Data = data;
        }

        public Message(string data)
        {
            this.Code = 0;
            this.Data = Encoding.UTF8.GetBytes(data);
        }

        public Message(int code, byte[] data)
        {
            this.Code = code;
            this.Data = data;
        }

        public Message(int code, string data)
        {
            this.Code = code;
            this.Data = Encoding.UTF8.GetBytes(data);
        }

        public virtual byte[] ToByteArray()
        {
            byte[] lenBuffer = BitConverter.GetBytes(this.Data.Length); // 4 bytes
            byte[] codeBuffer = BitConverter.GetBytes(this.Code); // 4 bytes

            byte[] bytes = new byte[8 + this.Data.Length];

            lenBuffer.CopyTo(bytes, 0);
            codeBuffer.CopyTo(bytes, 4);
            this.Data.CopyTo(bytes, 8);

            return bytes;
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.Data);
        }
    }
}
