using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CodingDojoHelper.Events;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using Microsoft.Practices.Prism.Events;

namespace CodingDojoHelper.ViewModels
{
    class DigestViewModel : INotifyPropertyChanged
    {
        public TimeSpan AverageCycleTime { get { return _codingDojo.AverageCycleTime; } }
        public ObservableCollection<KeyValuePair<string, double>> CycleTimes { get; private set; }
        public ObservableCollection<KeyValuePair<string, double>> Average { get; set; }

        private ICodingDojo _codingDojo;
        private readonly ISession _session;

        public DigestViewModel(IEventAggregator eventAggregator, ISession session)
        {
            _session = session;

            eventAggregator.GetEvent<KataFinishedEvent>().Subscribe(OnKataFinished);

            CycleTimes = new ObservableCollection<KeyValuePair<string, double>>();
            Average = new ObservableCollection<KeyValuePair<string, double>>();
        }

        public string TotalRounds
        {
            get
            {
                return _codingDojo.CycleTimes.Count + " " + Resources.Rounds;
            }
        }

        private void OnKataFinished(ICodingDojo codingDojo)
        {
            if (codingDojo == null)
                throw new ArgumentNullException("codingDojo");

            _codingDojo = codingDojo;
            OnPropertyChanged("AverageCycleTime");

            CompileCycleTimes();
            OnPropertyChanged("CycleTimes");
        }

        private void CompileCycleTimes()
        {
            var currentCycleTime = _codingDojo.StartTime;
            var average = _session.Get<TimeSpan>(Session.CycleTime);

            CycleTimes.Clear();
            Average.Clear();
            
            foreach (var cycleTime in _codingDojo.CycleTimes)
            {
                var cycleTimeAsString = string.Format("{0}:{1:00}:{2:00}", currentCycleTime.Hour, currentCycleTime.Minute, currentCycleTime.Second);
                CycleTimes.Add(new KeyValuePair<string, double>(cycleTimeAsString, cycleTime.TotalMinutes));
                currentCycleTime = currentCycleTime.Add(cycleTime);

                Average.Add(new KeyValuePair<string, double>(cycleTimeAsString, average.TotalMinutes));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
