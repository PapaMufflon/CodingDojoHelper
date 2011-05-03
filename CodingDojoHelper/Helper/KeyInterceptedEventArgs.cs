using System;
using System.Windows.Forms;

namespace CodingDojoHelper.Helper
{
    internal class KeyInterceptedEventArgs : EventArgs
    {
        public Keys Key { get; set; }

        public KeyInterceptedEventArgs(Keys key)
        {
            Key = key;
        }
    }
}