using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingDojoHelper.Helper.Interfaces;

namespace CodingDojoHelper.Helper
{
    class InterceptKeys : IInterceptKeys, IDisposable
    {
        public List<Keys> AllowedKeys { get; set; }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private LowLevelKeyboardProc _proc;
        private readonly IntPtr _hookID = IntPtr.Zero;
        private Keys _lastKey = Keys.None;

        public InterceptKeys()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                var vkCode = (Keys)Marshal.ReadInt32(lParam);
                _lastKey = vkCode;
                OnKeyIntercepted(vkCode);

                if (AllowedKeys != null && !AllowedKeys.Contains(vkCode))
                {
                    // do not propagate the pressed key to other apps
                    return (IntPtr) 1;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #region IInterceptKeys Members

        public event EventHandler<KeyInterceptedEventArgs> KeyIntercepted;

        private void OnKeyIntercepted(Keys key)
        {
            if (KeyIntercepted != null)
            {
                KeyIntercepted(this, new KeyInterceptedEventArgs(key));
            }
        }

        public void WaitForNextKeyAsync(TimeSpan maxWait, Action<Keys> callback)
        {
            _lastKey = Keys.None;

            Task.Factory.StartNew(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (stopwatch.Elapsed.CompareTo(maxWait) < 0 && _lastKey == Keys.None)
                    System.Threading.Thread.Sleep(100);

                callback.Invoke(_lastKey);
            });
        }

        #endregion

        #region DllImport

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

        #endregion
    }
}
