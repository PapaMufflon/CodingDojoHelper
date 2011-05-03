using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using CodingDojoHelper.Helper.Interfaces;

namespace CodingDojoHelper.Helper
{
    internal class DojoStopwatch : IStopwatch
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Timer _timer = new Timer();
        private List<StopwatchItem> _alarms;
        private readonly List<StopwatchItem> _activeAlarms = new List<StopwatchItem>();

        public DojoStopwatch()
        {
            _timer.AutoReset = false;
            _timer.Elapsed += OnAlarmElapsed;
        }

        #region IStopwatch Members

        public List<StopwatchItem> Alarms
        {
            get { return _alarms; }
            set
            {
                if (!AreAlarmsAscending(value) || AlarmsHasDuplicateKeys(value))
                {
                    _alarms = null;
                    throw new ArgumentException();
                }

                _alarms = value;
            }
        }

        private static bool AreAlarmsAscending(IEnumerable<StopwatchItem> alarms)
        {
            var previousAlarm = TimeSpan.Zero;

            foreach (var timeSpan in alarms)
            {
                if (timeSpan.Alarm.Subtract(previousAlarm).TotalSeconds < 0)
                    return false;

                previousAlarm = timeSpan.Alarm;
            }

            return true;
        }

        private static bool AlarmsHasDuplicateKeys(IList<StopwatchItem> value)
        {
            for (int index = 0; index < value.Count - 1; index++)
            {
                var stopwatchItem = value[index];

                for (int indexB = index + 1; indexB < value.Count; indexB++)
                {
                    if (stopwatchItem.Key == value[indexB].Key)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnAlarmElapsed(object sender, ElapsedEventArgs e)
        {
            _activeAlarms[0].Callback.Invoke();
            _activeAlarms.RemoveAt(0);

            if (_activeAlarms.Count > 0)
                StartNextAlarm();
        }

        private void StartNextAlarm()
        {
            _timer.Interval = _activeAlarms[0].Alarm.Subtract(_stopwatch.Elapsed).TotalMilliseconds;
        }

        public TimeSpan Elapsed
        {
            get { return _stopwatch.Elapsed; }
        }

        public void Start()
        {
            _activeAlarms.InsertRange(0, _alarms);

            if (_activeAlarms != null)
            {
                _timer.Interval = _activeAlarms.First().Alarm.TotalMilliseconds;
                _timer.Start();
            }

            _stopwatch.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _stopwatch.Reset();
        }

        public void StopAlarm()
        {
            _timer.Stop();
        }

        public void Restart()
        {
            _timer.Stop();
            _stopwatch.Reset();

            Start();
        }

        public void RestartAlarm(string key)
        {
            var stopwatchItem = GetAlarm(key);
            RemoveActiveAlarm(key);
            var newAlarm = stopwatchItem.Alarm.Add(_stopwatch.Elapsed);
            var index = 0;

            if (_activeAlarms.Count > 0)
                index = FindPositionOfNewAlarm(newAlarm);

            _activeAlarms.Insert(index, new StopwatchItem(stopwatchItem.Key, newAlarm, stopwatchItem.Callback));

            if (_activeAlarms.Count > 1 && index == 0)
                ReplaceActiveAlarm();

            StartNextAlarm();
        }

        private void RemoveActiveAlarm(string key)
        {
            for (int index = 0; index < _activeAlarms.Count; index++)
            {
                var stopwatchItem = _activeAlarms[index];
                if (stopwatchItem.Key == key)
                {
                    _activeAlarms.Remove(stopwatchItem);
                    return;
                }
            }
        }

        private int FindPositionOfNewAlarm(TimeSpan newAlarm)
        {
            if (_activeAlarms[0].Alarm > newAlarm)
                return 0;

            int index = 1;

            while (_activeAlarms.Count > index && _activeAlarms[index].Alarm < newAlarm)
                index++;

            return index;
        }

        private void ReplaceActiveAlarm()
        {
            _timer.Interval = (_activeAlarms[1].Alarm.TotalMilliseconds - _activeAlarms[0].Alarm.TotalMilliseconds);
        }

        #endregion

        public StopwatchItem GetAlarm(string key)
        {
            var item = Alarms.Where(i => i.Key == key).FirstOrDefault();

            if (item == null)
                throw new ArgumentException("No alarm found with key " + key, "key");

            return item;
        }
    }
}