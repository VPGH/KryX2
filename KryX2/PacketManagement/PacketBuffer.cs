using System;
using System.Diagnostics;

namespace KryX2.PacketManagement
{

    internal class PacketBuffer
    {

        private byte[] _buffer = new byte[512];
        private int _writeLocation = 0;
        private int _readLocation = 0;

        internal void Append(byte[] data)
        {
            try
            {
                //if buffer isnt long enough to hold new data then increase it before copying in new data.
                int neededBufferSpace = (data.Length + _writeLocation);
                if (_buffer.Length < neededBufferSpace)
                    Array.Resize(ref _buffer, neededBufferSpace + 512);

                //copy data to buffer
                Buffer.BlockCopy(data, 0, _buffer, _writeLocation, data.Length);
                //adjust where the next byte[]s should be written to.
                _writeLocation += data.Length;
            }
            catch
            {
                ResetBuffer();
            }
        }


        //returns a byte array of specified length starting from read location
        //then manipulates array by cleaning already read data and or resizing if
        //needed.
        private byte[] ReturnSection(int length)
        {
            try
            {
                int startingReadPoint = _readLocation;
                //passed later to trunctuate the buffer

                byte[] result = new byte[length];
                Buffer.BlockCopy(_buffer, _readLocation, result, 0, length);
                _readLocation += length;

                //if at end of buffer then reset it
                if (_readLocation >= _writeLocation)
                    ResetBuffer();

                return result;
            }
            catch
            {
                ResetBuffer();
                return null;
            }
        }

        //resets buffer
        internal void ResetBuffer()
        {
            if (_buffer.Length == 512)
                Array.Clear(_buffer, 0, 512);
            else
                _buffer = new byte[512];

            _writeLocation = 0;
            _readLocation = 0;
        }


        //called to return a full packet from longbuffer.
        //this assumes that the third byte of data received contains the packets
        //length. If you expect your length indicator to be at a different position
        //alter (_readLocation + 2) to your liking.
        //example:
        //BYTE   HEADER
        //BYTE   ID
        //BYTE   LENGTH
        //BYTE[] DATA
        //where as HEADER indicates a new packet, ID is the packet type, and LENGTH contains the
        //amount of data needed to complete the packet.
        internal byte[] ReturnFullPacket()
        {

            //grabs how long the packet is supposed to be. in my case the length of the packet
            //is held in byte 3 and 4 as a WORD. Some buffers use a single bytes, and others DWORDs.
            //adjust the packetLength type accordingly.
            //EG: WORD would be Int16 while DWORD would be Int32.
            Int16 packetLength = BitConverter.ToInt16(_buffer, (_readLocation + 2));
            //if unprocessed data is at or above packet length then return a section of specified length.
            //otherwise return null, which suggest a packet is not full yet
            if ((_writeLocation - _readLocation) >= packetLength)
            {
                return ReturnSection(packetLength);
            }
            return null;

        }

        //returns a value of how much data has not been processed/returned as a packet
        internal int ReturnDataLength()
        {
            return (_writeLocation - _readLocation);
        }

        //resets the buffer to a starting state.
        //if buffer is already at normal size just clear existing data.
        //if not at normal size then resize without preserving data.


    }
}
