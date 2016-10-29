
using System.Drawing;
using System.Windows.Forms;

namespace KryX2.UI
{
    internal static class Chat
    {
        private static RichTextBox Chatter;
        internal static void SetRichTextBox(RichTextBox chatObject)
        {
            Chatter = chatObject;
        }
        private static void PrepareWrite(int SelStart, Color ChatColor)
        {
            Chatter.SelectionStart = SelStart;
            Chatter.SelectionColor = ChatColor;
        } //chat>> sets selection start and color
        internal static void Add(Color ChatColor, string Text)
        {
            SetText(ChatColor, Text);
        } //chat>> adds chat using a formatted system color
        internal static void Add(string ChatColor, string Text)
        {
            SetText(System.Drawing.ColorTranslator.FromHtml(ChatColor), Text);
        } //chat>> adds chat using a string system color

        private delegate void ChatDelegate(Color ChatColor, string Text);
        private static void SetText(Color ChatColor, string Text)
        {
            if (Chatter.InvokeRequired)
            {
                Chatter.Invoke(new ChatDelegate(SetText), new object[] { ChatColor, Text });  // invoking itself
            }
            else
            {
                try
                {
                    PrepareWrite(Chatter.TextLength, ChatColor);
                    Chatter.AppendText(Text);
                    Chatter.ScrollToCaret();

                }
                catch { }
            }
        }


    }

}
