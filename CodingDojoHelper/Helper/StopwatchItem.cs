using System;

namespace CodingDojoHelper.Helper
{
    class StopwatchItem
    {
        public StopwatchItem(string key, TimeSpan alarm, Action callback)
        {
            Key = key;
            Alarm = alarm;
            Callback = callback;
        }

        public string Key { get; set; }
        public TimeSpan Alarm { get; set; }
        public Action Callback { get; set; }
    }
}
