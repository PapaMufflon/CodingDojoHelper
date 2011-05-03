using System;
using System.Collections.Generic;
using System.Linq;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;

namespace CodingDojoHelper
{
    internal class CodingDojo : ICodingDojo
    {
        public event EventHandler CycleTimeElapsed;
        public event EventHandler FinishHimTimeElapsed;
        public event EventHandler DojoTimeElapsed;

        public TimeSpan AverageCycleTime { get; private set; }
        public List<TimeSpan> CycleTimes { get; private set; }
        public DateTime StartTime { get; private set; }

        private readonly IStopwatch _stopwatch;
        private readonly IKombatSoundPlayer _soundPlayer;
        private readonly ISession _session;
        private bool _started;
        private TimeSpan _lastStop = TimeSpan.Zero;

        public CodingDojo(IStopwatch stopwatch, IKombatSoundPlayer soundPlayer, ISession session)
        {
            _stopwatch = stopwatch;
            _soundPlayer = soundPlayer;
            _session = session;

            CycleTimes = new List<TimeSpan>();
        }

        private void OnCycleTimeElapsed()
        {
            if (CycleTimeElapsed != null)
                CycleTimeElapsed(this, EventArgs.Empty);

            _soundPlayer.BeginPlayFinishHimSound();
        }

        private void OnDojoTimeElapsed()
        {
            if (DojoTimeElapsed != null)
                DojoTimeElapsed(this, EventArgs.Empty);
        }

        private void OnFinishHimTimeElapsed()
        {
            if (FinishHimTimeElapsed != null)
                FinishHimTimeElapsed(this, EventArgs.Empty);

            _soundPlayer.BeginPlayFinishHimSound();
        }

        #region ICodingDojo Members

        public void Start()
        {
            _stopwatch.Stop();

            ResetValues();

            _soundPlayer.BeginPlayStartSound();
            _stopwatch.Start();
            _started = true;
        }

        private void ResetValues()
        {
            var alarms = new List<StopwatchItem>
            {
                new StopwatchItem(Session.CycleTime, _session.Get<TimeSpan>(Session.CycleTime), OnCycleTimeElapsed),
                new StopwatchItem(Session.DojoTime, _session.Get<TimeSpan>(Session.DojoTime), OnDojoTimeElapsed)
            };

            if (_session.Get<bool>(Session.FinishHimTimeActive))
            {
                var finishHimTime = _session.Get<TimeSpan>(Session.FinishHimTime);

                if (finishHimTime != TimeSpan.Zero)
                    alarms.Add(new StopwatchItem(Session.FinishHimTime, finishHimTime, OnFinishHimTimeElapsed));
            }

            alarms.Sort((a, b) => a.Alarm.CompareTo(b.Alarm));
            _stopwatch.Alarms = alarms;

            _lastStop = TimeSpan.Zero;
            AverageCycleTime = TimeSpan.Zero;
            CycleTimes = new List<TimeSpan>();
            StartTime = DateTime.Now;
        }

        public void ChangeDeveloper()
        {
            if (!_started)
                throw new InvalidOperationException();

            AddCycleTime();

            _soundPlayer.BeginPlayCycleSound(CycleTimes.Last());
            _stopwatch.RestartAlarm(Session.CycleTime);

            if (_session.Get<bool>(Session.FinishHimTimeActive))
                _stopwatch.RestartAlarm(Session.FinishHimTime);
        }

        private void AddCycleTime()
        {
            var elapsed = _stopwatch.Elapsed;

            if (_lastStop == TimeSpan.Zero)
                CycleTimes.Add(elapsed);
            else
                CycleTimes.Add(elapsed.Subtract(_lastStop));

            _lastStop = elapsed;

            AverageCycleTime = TimeSpan.FromMilliseconds(CycleTimes.Average(t => t.TotalMilliseconds));
        }

        public void Stop()
        {
            AddCycleTime();

            _stopwatch.Stop();
            _soundPlayer.BeginPlayStopSound();
            _started = false;
        }

        #endregion
    }
}