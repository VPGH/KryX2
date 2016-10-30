
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using KryX2.Settings;

namespace KryX2.MBNCSUtil.CR
{

    //internal class ByteArray
    //{
    //    public byte[] Data;
    //}
    internal static class FilesBuffered
    {

        private static List<byte[]> _byteData = new List<byte[]>();

        internal static void LoadFiles()
        {

            //for (int i = 0; i < 24; i++)
            //{
            //    _fileData[i] = new ByteArray();
            //}

            byte[] inData;

            inData = File.ReadAllBytes(GeneratedSettings.StarcraftExe);
            _byteData.Add(inData);

            inData = File.ReadAllBytes(GeneratedSettings.StormDll);
            _byteData.Add(inData);

            inData = File.ReadAllBytes(GeneratedSettings.BattleSnp);
            _byteData.Add(inData);

            inData = File.ReadAllBytes(GeneratedSettings.StarBin);
            _byteData.Add(inData);

            for (int i = 0; i < 20; i++)
            {
                string filePath = GeneratedSettings.LockdownPath + "lockdown-IX86-" + i.ToString("D2") + ".dll";
                inData = File.ReadAllBytes(filePath);
                _byteData.Add(inData);
            }

        }

        internal static byte[] ReturnBytes(int dataIndex)
        {
            return _byteData[dataIndex];
        }

    }
}
