using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using KryX2.UI;
using KryX2.Sockets;
using KryX2.Settings;
using System.Diagnostics;
using System.IO;

namespace KryX2.FileManagement
{
    internal class ProxyDetails
    {
        internal IPAddress Address { get; set; }
        internal int Port { get; set; }
        internal ProxyType Type { get; set; }
        internal byte UsedCount { get; set; }
    }
    internal enum ProxyType
    {
        Socks4,
        Socks5,
        Http,
        Default
    }


    internal static class Proxies
    {

        private static List<ProxyDetails> _proxyList = new List<ProxyDetails>();
        private static int _proxyIndex = 0;
        private const Char CharSpace = (char)32;
        private const Char CharTab = (char)9;

        internal static bool CycleProxy(ClientSocket cs)
        {
            //reduces the count of how many times this specific proxy ip has been used
            LowerUseCount(cs.ProxyAddress);

            ProxyDetails proxy = NextAddress();
            if (proxy == null)
            {
                return false;
            } //no more keys
            else
            {
                cs.ProxyPort = proxy.Port;
                cs.ProxyAddress = proxy.Address;
                cs.ProxyFormat = proxy.Type;
                return true;
            }
        }


        internal static int ReturnListCount()
        {
            return _proxyList.Count;
        }


        private static ProxyDetails NextAddress()
        {
            int proxiesChecked = 0;
            int listCount = _proxyList.Count;

            while (proxiesChecked < listCount) //prevents an endless loop
            {
                try
                {
                    _proxyIndex++;
                    if (_proxyIndex >= _proxyList.Count)
                        _proxyIndex = 0;

                    //Debug.Print("proxy index " + _proxyIndex.ToString() + ", used count " + _proxyList[_proxyIndex].UsedCount + ", max per " + UserSettings.ClientsPerProxy);
                    if (_proxyList[_proxyIndex].UsedCount < UserSettings.ClientsPerProxy)
                    {
                        _proxyList[_proxyIndex].UsedCount++;
                        return _proxyList[_proxyIndex];
                    }
                    //found a result thats not 

                    proxiesChecked += 1;
                }
                catch { }
            }

            return null;

        } //returns proxy. if none available return null returns null

        private static void LowerUseCount(IPAddress address)
        {
            if (address != null)
            {
                int index = ReturnProxyIndex(address);
                if (index != -1)
                {
                    byte usedCount = _proxyList[index].UsedCount;

                    if (usedCount > 0)
                        usedCount--;

                    _proxyList[index].UsedCount = usedCount;
                }
            }
        }

        private static int ReturnProxyIndex(IPAddress address)
        {
            return _proxyList.FindIndex(proxydetails => proxydetails.Address.Equals(address));
        }

        //loads all proxy list
        internal static void ImportProxylist()
        {
            //clear proxy list, reset take from index
            _proxyList.Clear();
            _proxyIndex = 0;

            string[] paths;
            paths = new string[]
            {
                    GeneratedSettings.AppDirectory + @"\Text\Proxies.txt",
                    GeneratedSettings.AppDirectory + @"\Text\ProxiesHttp.txt",
                    GeneratedSettings.AppDirectory + @"\Text\ProxiesS4.txt",
                    GeneratedSettings.AppDirectory + @"\Text\ProxiesS5.txt"
            };

            ProxyType[] types;
            types = new ProxyType[]
            {
                ProxyType.Default,
                ProxyType.Http,
                ProxyType.Socks4,
                ProxyType.Socks5
            };


            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                string fileText = null; //default
                ProxyType type = types[i];

                if (File.Exists(path))
                {
                    StreamReader FileReader = new StreamReader(path);
                    while ((fileText = FileReader.ReadLine()) != null)
                    {
                        try
                        {
                            fileText = TidyText(fileText);

                            if (!string.IsNullOrEmpty(fileText))
                            { //filetext still contains data, lets proceed

                                //set proxy type from the beginning if reading from a specified type file
                                switch (type)
                                {
                                    case ProxyType.Default:
                                        break;
                                    case ProxyType.Http:
                                        fileText = fileText + "@http";
                                        break;
                                    case ProxyType.Socks4:
                                        fileText = fileText + "@s4";
                                        break;
                                    case ProxyType.Socks5:
                                        fileText = fileText + "@s5";
                                        break;
                                }

                                //pass string to proxy parser
                                ProxyDetails proxyInfo = new ProxyDetails(); //initialize
                                ProxyParser parser = new ProxyParser();
                                parser.Engage(fileText); //presets some valus on our sorter                    

                                proxyInfo.Address = parser.ReturnAddress();

                                //if valid ip proceed
                                if (proxyInfo.Address != null)
                                {
                                    //if already in list continue past
                                    if (ReturnProxyIndex(proxyInfo.Address) != -1)
                                        continue;
                                    proxyInfo.Port = parser.ReturnPort();
                                    proxyInfo.Type = parser.ReturnType();
                                    proxyInfo.UsedCount = 0;
                                    //bool proxyFound = _proxyList.Exists(IPAddress => IPAddress.Address == proxyInfo.Address);
                                    //if (!proxyFound)
                                    //{ 
                                    _proxyList.Add(proxyInfo);
                                }
                            }
                        } //try end
                        finally
                        {
                            //do nothing
                        } //try set scanlist entry
                    }
                    FileReader.Close();
                }

            }

            Chat.Add(Color.Silver, "Loaded " + _proxyList.Count + " proxies." + Environment.NewLine);

        }

        private static string TidyText(string text)
        {
            text = text.TrimStart(); //remove front blanks

            if (text.Length > 6)
            { //only if string could potentially be of proxy length

                if (text.Substring(0, 2) == "//")
                    return string.Empty;

                int locationTab = 0;
                int locationSpace = 0;
                int locationResult = 0;

                locationSpace = text.IndexOf(CharSpace, 1);
                locationTab = text.IndexOf(CharTab, 1);
                //check locations
                if ((locationTab > 0) && (locationSpace > 0))
                {
                    locationResult = Math.Min(locationTab, locationSpace);
                }
                else
                {
                    locationResult = Math.Max(locationTab, locationSpace);
                }
                //if theres a space or tab determine which of the two are lesser so as long as they are greater than 0
                //and set result
                if (locationResult > 1) { text = text.Substring(0, (locationResult)); }
                //if locationresult greater than 1 then remove excess
                return text;
            }
            else
            {//proxy string not long enough
                return string.Empty; //cant possibly be a proxy so empty string
            }

        } //remove spaces in front or end of entries as well removes extra text

    }  //proxylist class end

}