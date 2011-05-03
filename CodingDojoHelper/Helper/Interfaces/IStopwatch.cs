using System;
using System.Collections.Generic;

namespace CodingDojoHelper.Helper.Interfaces
{
    internal interface IStopwatch
    {
        List<StopwatchItem> Alarms { get; set; }
        TimeSpan Elapsed { get; }

        void Start();
        void Stop();
        void StopAlarm();
        void Restart();
        void RestartAlarm(string key);
        StopwatchItem GetAlarm(string key);
    }
}