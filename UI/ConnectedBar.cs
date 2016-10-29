using System;
using System.Drawing;
using System.Windows.Forms;
using KryX2.Sockets;

namespace KryX2.UI
{
    internal static class ConnectedBar
    {
        private static int _totalSockets = 0;
        private static int _connected = 0;
        private static Label _progressBar;

        internal static void Reference(Label labelObject)
        {
            _progressBar = labelObject;
        }

        internal static int ReturnConnected()
        {
            return _connected;
        }
        internal static int ReturnConfigured()
        {
            return _totalSockets;
        }

        internal static void SetTotalSockets(int total)
        {
            _totalSockets = total;
            SetDisplay();
        }

        private delegate void ProgressDelegate();
        private static void SetDisplay()
        {
            try
            {
                if (_progressBar.InvokeRequired)
                {
                    _progressBar.Invoke(new ProgressDelegate(SetDisplay));  // invoking itself
                } //new object[] { ChatColor, Text }
                else
                {

                    //if none are connected
                    if (_connected == 0)
                    {
                        _progressBar.Width = 45;
                        _progressBar.Text = "0%";
                        return;
                    }
                    //if all are connected
                    if (_connected >= _totalSockets)
                    {
                        _progressBar.Width = 225;
                        _progressBar.Text = "100%";
                        return;
                    }
                    //creates a percentage decimal. eg result could be 0.5, which is 50%
                    //while 1.0 is 100%
                    double percentConnected = (double)_connected / _totalSockets;
                    //force min/max
                    percentConnected = ForceDblRange(percentConnected, 0.01, 1);
                    //adjust alignment based on percentage. Just for looks really
                    _progressBar.Text = percentConnected.ToString("#%");
                    //some reason the alignment changes each time we update the label. set back to default
                    _progressBar.TextAlign = ContentAlignment.MiddleCenter;
                    //sets width of percent bar
                    int widthValue = (int)Math.Round(225 * percentConnected);
                    //minimum width of x is needed to ensure everything displays properly
                    if (widthValue < 45)
                        widthValue = 45;
                    _progressBar.Width = widthValue;

                }
            }
            catch { }
        }

        private static double ForceDblRange(double number, double min, double max)
        {
            if (number < min)
                return min;
            if (number > max)
                return max;
            return number;
        }
        internal static void AddOne()
        {
            _connected++;
            SetDisplay();
        }

        internal static void RemoveOne(ClientSocket cs)
        {
            if (cs.LoggedOn)
            {
                _connected--;
                SetDisplay();
            }
        }

        internal static void Reset()
        {

        }
    }
}
