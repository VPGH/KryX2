using System;
using System.Net;


namespace KryX2.FileManagement
{
    internal class ProxyParser
    {

        private const Char CharAt = (char)64;
        private string _proxy = string.Empty;
        private int _port = 0;
        private string _typeRaw = string.Empty;
        private ProxyType _proxyType = ProxyType.Default;
        private void EngageVariables(string text)
        {
            _proxy = text.Substring(0, _locationColon);
            //gets ip address only
            int locationType = text.IndexOf(CharAt);
            if (locationType == -1)
            { //no @ symbol found
                _port = Convert.ToInt32(
                    text.Substring(_locationColon + 1)
                    );
            }
            else
            {
                _port = Convert.ToInt32(
                    text.Substring(
                    _locationColon + 1, (
                        (locationType - 1) - _locationColon)
                        )
                    );

                _typeRaw = text.Substring(locationType + 1);
                _proxyType = ParseTypeRaw(_typeRaw); //returns type, if default then type was not specified properly
            }

            if (_proxyType == ProxyType.Default)
            {// could not be determined if at default, try to find using port
                if (WithinNumberRange(_port, 80, 89)
                    || WithinNumberRange(_port, 8000, 8999)
                    || WithinNumberRange(_port, 3100, 3130))
                {
                    _proxyType = ProxyType.Http;
                }
                else
                {
                    _proxyType = ProxyType.Default;
                }
                //if within most common http ranges then assume as http
            }

        } //end engage variables

        private ProxyType ParseTypeRaw(string Text)
        {
            switch (Text.Substring((
                 Text.Length - 2)).ToLower())
            {
                case "tp":
                    return ProxyType.Http;
                case "s4":
                    return ProxyType.Socks4;
                case "s5":
                    return ProxyType.Socks5;
                default:
                    return ProxyType.Default;
            }
        }


        private static bool WithinNumberRange(int number, int low, int high)
        {
            if (number >= low && number <= high)
            {
                return true;
            }
            else
            {
                return false;
            }
        } //checks if given number falls within a range

        private const Char CharColon = (char)58;
        private int _locationColon;
        private int ReturnColon(string tProxy) { return tProxy.IndexOf(CharColon); }
        internal IPAddress ReturnAddress()
        {
            string address = _proxy.Substring(0, _locationColon);
            IPAddress ParseResult = null;
            TryIPAddress(address, out ParseResult);
            if (ParseResult != null)
            {
                return ParseResult;
            }
            else
            {
                return null;
            }
        }

        internal int ReturnPort() { return _port; }
        internal ProxyType ReturnType() { return _proxyType; }
        internal void Engage(string tProxy)
        {
            _locationColon = ReturnColon(tProxy);
            EngageVariables(tProxy);
        }

        private static void TryIPAddress(string address, out IPAddress result)
        {
            try
            {
                result = IPAddress.Parse(address);
            }
            catch
            {
                result = null;
            }
        } //tries to parse an ip. if an error is thrown then set to null

    } //Parsing class END
    //reads a proxy and returns various values such as IP, Port, Type...
}
