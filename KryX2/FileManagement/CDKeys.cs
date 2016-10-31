using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using KryX2.UI;
using KryX2.Sockets;
using KryX2.Settings;

namespace KryX2.FileManagement
{

    internal static class CDKeys
    {

        private static readonly string NewLine = Environment.NewLine;
        private static List<string> _cdKeyList = new List<string>();

        internal static bool CycleCDKey(ClientSocket cs)
        {
            string scKey = ReturnCdKey();
            if (scKey == null)
            {
                return false;
            } //no more keys
            else
            {
                cs.CdKey = scKey;
                return true;
            } //if getting new key
        } //sets a new key on a specific socket

        private static int ReturnCdKeyIndex(string cdkey)
        {
            return _cdKeyList.IndexOf(cdkey);
        }

        internal static int ReturnListCount()
        {
            return _cdKeyList.Count;
        }

        private static void ImportGoodCdKeys(List<string> badkeys)
        {

            string path = (KryX2.Settings.GeneratedSettings.AppDirectory + @"\Text\CdKeys.txt");
            if (!File.Exists(path))
            {
                Chat.Add(Color.NavajoWhite, "Missing CdKeys.txt file!" + NewLine);
            }

            string fileText = null; //default
            _cdKeyList.Clear();

            StreamReader fileReader = new StreamReader(path);
            while ((fileText = fileReader.ReadLine()) != null)
            {

                try
                {
                    fileText = TidyText(fileText);
                    if (fileText.Length == 13)
                    { //if length is 13 proceed
                        if (!badkeys.Contains(fileText) && (ReturnCdKeyIndex(fileText) == -1))
                        {
                            _cdKeyList.Add(fileText);
                        }
                    }
                } //try end
                finally { } //do nothing

            }
            fileReader.Close();

            Chat.Add(Color.Silver, "Loaded " + _cdKeyList.Count + " cdkeys." + Environment.NewLine);

        } //loads scanlist into an array

        internal static void ImportCdKeys()
        {

            List<string> badkeys = ImportBadCdKeys();
            ImportGoodCdKeys(badkeys);

        }

        private static List<string> ImportBadCdKeys()
        {

            List<string> badCDKeys = new List<string>();
            string path = GeneratedSettings.BadCDKeysText;

            //no bad keys found, return empty list
            if (!File.Exists(path))
            {
                return badCDKeys;
            }

            Debug.Print("Loaded bad CDKeys");
            //each line is read to this variable
            string fileText = null;
            //location of space if found
            int spaceIndex = 0;
            StreamReader fileReader = new StreamReader(path);
            while ((fileText = fileReader.ReadLine()) != null)
            {

                try
                {

                    fileText = TidyText(fileText);
                    //bad keys should already be in tidy format
                    if (fileText.Length == 13)
                    { //if length is 13 proceed
                        badCDKeys.Add(fileText);
                    }
                    else
                    {
                        spaceIndex = fileText.IndexOf(" ");
                        if (spaceIndex == 13)
                        {
                            fileText = fileText.Substring(0, 13);
                            badCDKeys.Add(fileText);
                        }
                    }
                } //try end
                finally { }

            }
            fileReader.Close();

            return badCDKeys;

        } //loads scanlist into an array


        //remove spaces in front or end of entries as well removes extra text
        private static string TidyText(string text)
        {
            text = text.Trim(); //remove spaces from front and back
            text = text.Replace("-", ""); //remove dashes

            if (text.Length > 12)
            {
                if (text.Substring(0, 2) == "//")
                    return string.Empty;

                return text;
            }
            else
            {
                return string.Empty;
            }    
        }

        private static string ReturnCdKey()
        {
            if (_cdKeyList.Count == 0) { return null; }

            string cdkey = _cdKeyList[0];
            _cdKeyList.RemoveAt(0);

            return cdkey;
        }

        internal static void WriteBadCDKey(string cdkey, string reason)
        {
            try
            {

                string path = (GeneratedSettings.BadCDKeysText);
                if (File.Exists(path))
                {
                    using (StreamWriter sw = File.AppendText(path))
                        sw.WriteLine(cdkey + reason);
                }
                else
                {
                    TextWriter tw = new StreamWriter(path, true);
                    tw.WriteLine(cdkey + reason);
                    tw.Close();
                }
            }
            catch { }
            finally
            {
                //Debug.Print("bad key because" + reason);
            }
        }
    }  //end cdkeylist class

}