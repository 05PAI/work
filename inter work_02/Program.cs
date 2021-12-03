using System;
using System.Text;

namespace ChatCoreTest
{
    internal class Program
    {
        private static byte[] m_PacketData;
        private static byte[] m_RePacketData;
        private static uint m_Pos;
        private static int m_length=0;
        public static void Main(string[] args)
        {
            m_PacketData = new byte[1024];
            m_RePacketData = new byte[1024];
            m_Pos = 4;

            Write(109);
            Write(109.99f);
            Write("Hello!");
            _WriteLength(m_Pos);
            for (int i = 0; i < m_Pos; i++)
            {
                m_RePacketData[m_Pos-1 - i] = m_PacketData[i];
            }
            Console.Write($"Output Byte array(length:{m_Pos}): ");

            for (var i = 0; i < m_Pos; i++)
            {
                Console.Write(m_PacketData[i] + ", ");

            }           
            Console.WriteLine("");       
            for (int i = (int)m_Pos-8; i >-1; i--)
            {
                var a=BitConverter.ToInt32(m_RePacketData, i);
                if(a==1)
                {
                    var aa = BitConverter.ToUInt32(m_RePacketData, i-4);
                    Console.WriteLine(aa);
                    i = i - 3;
                }
                if(a==2)
                {
                    var bb = BitConverter.ToSingle(m_RePacketData, i-4);
                    Console.WriteLine(bb);
                    i = i - 3;
                }
                if(a==3)
                {                    
                    var x = BitConverter.ToInt32(m_RePacketData, i-4);
                    i = i - 4 - x;
                    var ca = Encoding.Unicode.GetString(m_RePacketData).Substring(i, x/2);
                    Console.WriteLine(ca);                           
                }
                
            }                                           
        }

        // write an integer into a byte array
        private static bool Write(int i)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            
            var a=BitConverter.GetBytes(1);
            _Write(a);            
            _Write(bytes);
            return true;
        }

        // write a float into a byte array
        private static bool Write(float f)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(f);
            var a = BitConverter.GetBytes(2);
            _Write(a);           
            _Write(bytes);
            return true;
        }

        // write a string into a byte array
        private static bool Write(string s)
        {
            // convert string to byte array
            var bytes = Encoding.Unicode.GetBytes(s);
             var a = BitConverter.GetBytes(3);
            _Write(a);
            // write byte array length to packet's byte array
            //if (Write(bytes.Length) == false)
            //{
            //    return false;
            //}
            byte[] b = BitConverter.GetBytes(bytes.Length);
            _Write(b);
            _Write(bytes);           
            return true;
        }

        // write a byte array into packet's byte array
        private static void _Write(byte[] byteData)
        {
            // converter little-endian to network's big-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }
            byteData.CopyTo(m_PacketData, m_Pos);
            m_Pos += (uint)byteData.Length;
            m_length = m_length + byteData.Length;
            //var lengh=(byte)byteData.Length;
            //var ta=BitConverter.ToInt32(m_RePacketData, 0);
            //ta= ta + lengh;
        }
        private static void _WriteLength(uint m_Pos)
        {
            var bytes= BitConverter.GetBytes(m_Pos);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            bytes.CopyTo(m_PacketData,0);
        }       
    }
}
