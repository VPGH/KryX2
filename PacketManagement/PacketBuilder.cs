using System;
using System.IO;
using System.Text;
using KryX2.Client;
using KryX2.Sockets;
using System.Diagnostics;

namespace KryX2.PacketManagement
{
    internal class Builder
    {
        private int _Length;
        private readonly MemoryStream _memoryBuilder = new MemoryStream();

        public Int32 GetLength()
        {
            return _Length;
        }

        private byte[] FinalizePacket(byte packetId)
        {
            byte[] result = new byte[_Length + 4];

            result[0] = 0xFF;
            result[1] = packetId;

            MemoryStream msPacketLength = new MemoryStream();
            msPacketLength.Write(BitConverter.GetBytes((_Length + 4)), 0, 2);
            Array.Copy(msPacketLength.ToArray(), 0, result, 2, 2);

            Array.Copy(
                _memoryBuilder.ToArray(),
                0, result, 4, _Length);

            ClearBuilder();
            return result;
        }

        internal void SendPacket(ClientSocket cs, byte packetId)
        {
            //Debug.Print("sending packet " + packetId.ToString("X2"));
            SocketActions.SendData(cs, FinalizePacket(packetId));
        }


        private void ClearBuilder()
        {
            _Length = 0;
            _memoryBuilder.SetLength(0);
        }


        internal void InsertByte(byte data)
        {
            _memoryBuilder.WriteByte(data);
            _Length++;
        }

        internal void InsertInt32(int data)
        {
            _memoryBuilder.Write(BitConverter.GetBytes(data), 0, 4);
            _Length += 4;
        }



        internal void InsertNTString(string data)
        {
            InsertByteArray(Encoding.ASCII.GetBytes(data));
            InsertByte(0);
        }


        internal void InsertNonNTString(string data)
        {
            InsertByteArray(Encoding.ASCII.GetBytes(data));
        }

        internal void InsertString(string data)
        {
            InsertByteArray(Encoding.ASCII.GetBytes(data));
        }

        internal void InsertByteArray(byte[] data)
        {
            _memoryBuilder.Write(data, 0, data.Length);
            _Length += data.Length;
        }
    } //end builder class
}