using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using CodingDojoHelper.Events;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using Microsoft.Practices.Composite.Events;

namespace CodingDojoHelper.ViewModels
{
    class DojoViewModel : INotifyPropertyChanged
    {
        public TimeSpan AverageCycleTime { get { return _codingDojo.AverageCycleTime; } }

        internal Keys LastKey { get; set; }

        private readonly IInterceptKeys _interceptKeys;
        private readonly ICodingDojo _codingDojo;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISession _session;
        private Keys _changeDeveloperKey;
        private Keys _endKataKey;
        private Dispatcher _dispatcher;

        public DojoViewModel(IInterceptKeys interceptKeys, ICodingDojo codingDojo, IEventAggregator eventAggregator, ISession session)
        {
            if (interceptKeys == null)
                throw new ArgumentNullException("interceptKeys");

            if (codingDojo == null)
                throw new ArgumentNullException("codingDojo");

            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");

            if (session == null)
                throw new ArgumentNullException("session");

            _interceptKeys = interceptKeys;
            _codingDojo = codingDojo;
            _eventAggregator = eventAggregator;
            _session = session;
            _dispatcher = Dispatcher.CurrentDispatcher;

            _interceptKeys.KeyIntercepted += OnKeyIntercepted;
            _eventAggregator.GetEvent<StartKataEvent>().Subscribe(OnStartKata);
            _codingDojo.FinishHimTimeElapsed += OnFinishHimTimeElapsed;
            _codingDojo.DojoTimeElapsed += OnDojoTimeElapsed;
        }

        public string CurrentRound
        {
            get
            {
                return Resources.Round + " " + (_codingDojo.CycleTimes.Count + 1);
            }
        }

        private void OnStartKata(object o)
        {
            _changeDeveloperKey = _session.Get<Keys>(Session.ChangeDeveloperKey);
            _endKataKey = _session.Get<Keys>(Session.EndKataKey);

            _codingDojo.Start();

            OnPropertyChanged("AverageCycleTime");
            OnPropertyChanged("CurrentRound");
        }

        private void OnKeyIntercepted(object s, KeyInterceptedEventArgs e)
        {
            LastKey = e.Key;
            OnPropertyChanged("LastKey");

            if (e.Key == _changeDeveloperKey)
                Cycle();
            else if (e.Key == _endKataKey)
                OnStopCommand();
        }

        private void Cycle()
        {
            _interceptKeys.AllowedKeys = null;
            _codingDojo.ChangeDeveloper();
            OnPropertyChanged("AverageCycleTime");
            OnPropertyChanged("CurrentRound");
        }

        private void OnFinishHimTimeElapsed(object sender, EventArgs e)
        {
            if (_session.Get<bool>(Session.FinishHimTimeActive))
                _interceptKeys.AllowedKeys = new List<Keys>
                {
                    _session.Get<Keys>(Session.ChangeDeveloperKey),
                    _session.Get<Keys>(Session.EndKataKey)
                };
        }

        public ICommand StopCommand
        {
            get { return new ParameterlessDelegateCommand(OnStopCommand); }
        }

        private void OnDojoTimeElapsed(object sender, EventArgs eventArgs)
        {
            OnStopCommand();
        }

        private void OnStopCommand()
        {
            _interceptKeys.AllowedKeys = null;
            _codingDojo.Stop();
            OnPropertyChanged("StopCommand");
            _dispatcher.Invoke((Action)(() => _eventAggregator.GetEvent<KataFinishedEvent>().Publish(_codingDojo)));
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