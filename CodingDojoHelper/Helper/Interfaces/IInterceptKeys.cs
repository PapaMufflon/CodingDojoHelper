using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodingDojoHelper.Helper.Interfaces
{
    internal interface IInterceptKeys
    {
        event EventHandler<KeyInterceptedEventArgs> KeyIntercepted;

        List<Keys> AllowedKeys { get; set; }

        void WaitForNextKeyAsync(TimeSpan maxWait, Action<Keys> callback);
    }
}