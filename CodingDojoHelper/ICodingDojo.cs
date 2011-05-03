using System;
using System.Collections.Generic;

namespace CodingDojoHelper
{
    internal interface ICodingDojo
    {
        event EventHandler CycleTimeElapsed;
        event EventHandler FinishHimTimeElapsed;
        event EventHandler DojoTimeElapsed;

        TimeSpan AverageCycleTime { get; }
        List<TimeSpan> CycleTimes { get; }
        DateTime StartTime { get; }

        void Start();
        void ChangeDeveloper();
        void Stop();
    }
}