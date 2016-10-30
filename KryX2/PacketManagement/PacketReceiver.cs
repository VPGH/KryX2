using System;
using System.Text;

namespace KryX2.PacketManagement
{

    internal class Receiver
    {

        private int _ReadLocation;
        private byte[] _Input;

        internal Receiver(byte[] data)
        {
            _Input = data;
            _ReadLocation = 0;
        }

        //internal void Set(byte[] data)
        //{
        //    _Input = data;
        //    _ReadLocation = 0;
        //}

        internal void SkipLength(int Data)
        {
            _ReadLocation += Data;
        } //just skips read location ahead to avoid any data that's not neccesary


        internal Int32 GetInt32()
        {
            try
            {
                Int32 result = BitConverter.ToInt32(_Input, _ReadLocation);
                _ReadLocation += 4;
                return result;
            }
            catch
            {
                return 0;
            }
        }


        internal string GetNTString()
        {

            byte[] byteResult = GetNTBytes();
            return Encoding.ASCII.GetString(byteResult);

        }

        internal byte[] GetNTBytes()
        {


            try
            {
                int index = _ReadLocation;
                while (index < _Input.Length)
                {
                    if (_Input[index] == 0)
                    {
                        break;
                    } //break if read index is 0x0
                    index++;
                }

                byte[] result = new byte[(index - _ReadLocation)];
                Buffer.BlockCopy(_Input, _ReadLocation, result, 0, result.Length);
                //sets to just before null char
                _ReadLocation = (index + 1);
                //modifies to just past null char
                return result;

            }
            catch
            {
                byte[] nullBytes = new byte[1];
                return nullBytes;
            }
        }

        internal byte GetByte()
        {
            try
            {
                byte result = _Input[_ReadLocation];
                _ReadLocation++;
                return result;
            }
            catch
            {
                return 0;
            }
        }

        #region unused conversion
        //public Int16 GetInt16()
        //{
        //    Int16 result = BitConverter.ToInt16(_Input, _ReadLocation);
        //    _ReadLocation += 2;
        //    return result;
        //}

        //public UInt16 GetUInt16()
        //{
        //    UInt16 result = BitConverter.ToUInt16(_Input, _ReadLocation);
        //    _ReadLocation += 2;
        //    return result;
        //}

        //public UInt32 GetUInt32()
        //{
        //    UInt32 result = BitConverter.ToUInt32(_Input, _ReadLocation);
        //    _ReadLocation += 4;
        //    return result;
        //}

        //public ulong GetUInt64()
        //{

        //    ulong result = BitConverter.ToUInt64(_Input, _ReadLocation);
        //    _ReadLocation += 8;
        //    return result;
        //}

        //public long GetInt64()
        //{
        //    long result = BitConverter.ToInt64(_Input, _ReadLocation);
        //    _ReadLocation += 8;
        //    return result;
        //}
        #endregion


    } //end receiver class

}
