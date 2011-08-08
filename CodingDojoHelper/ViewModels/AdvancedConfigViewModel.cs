using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CodingDojoHelper.Events;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using Microsoft.Practices.Prism.Events;

namespace CodingDojoHelper.ViewModels
{
    class AdvancedConfigViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Visibility AdornerVisibility { get; set; }

        private const int FinishHimTimeOffset = 25;

        private readonly IEventAggregator _eventAggregator;
        private readonly ISession _session;
        private readonly IInterceptKeys _interceptKeys;

        public AdvancedConfigViewModel(IEventAggregator eventAggregator, ISession session, IInterceptKeys interceptKeys)
        {
            _eventAggregator = eventAggregator;
            _session = session;
            _interceptKeys = interceptKeys;

            _session.ValueChanged += OnSessionValueChanged;
            AdornerVisibility = Visibility.Hidden;
        }

        private void OnSessionValueChanged(object s, ValueChangedEventArgs e)
        {
            switch (e.Key)
            {
                case Session.CycleTime:
                    OnCycleTimeChanged(e);
                    break;

                case Session.FinishHimTime:
                    OnFinishHimTimeChanged();
                    break;
            }
        }

        private void OnCycleTimeChanged(ValueChangedEventArgs e)
        {
            OnPropertyChanged("ValueForCycleTime");

            var cycleTime = (TimeSpan)e.NewValue;
            var finishHimTime = _session.Get<TimeSpan>(Session.FinishHimTime);

            if (finishHimTime.Subtract(cycleTime).TotalSeconds < 0)
                _session.Set(Session.FinishHimTime, cycleTime);
        }

        private void OnFinishHimTimeChanged()
        {
            OnPropertyChanged("ValueForFinishHimTime");
            OnPropertyChanged("FinishHimTime");
        }

        public ICommand BackToPreviousScreenCommand
        {
            get
            {
                return new ParameterlessDelegateCommand(() =>
                    _eventAggregator.GetEvent<ResetKataEvent>().Publish(null));
            }
        }

        public ICommand SelectChangeDeveloperKeyCommand
        {
            get
            {
                return new ParameterlessDelegateCommand(OnSelectChangeDeveloperKeyCommand);

            }
        }

        private void OnSelectChangeDeveloperKeyCommand()
        {
            ChangeKey(Session.ChangeDeveloperKey);
        }

        public ICommand SelectEndKataKeyCommand
        {
            get
            {
                return new ParameterlessDelegateCommand(OnSelectEndKataKeyCommand);

            }
        }

        private void OnSelectEndKataKeyCommand()
        {
            ChangeKey(Session.EndKataKey);
        }

        private void ChangeKey(string key)
        {
            AdornerVisibility = Visibility.Visible;
            OnPropertyChanged("AdornerVisibility");

            var maxWaitForChangeDeveloperKey = TimeSpan.FromMinutes(1);
            _interceptKeys.WaitForNextKeyAsync(maxWaitForChangeDeveloperKey, k =>
            {
                if (k != Keys.None)
                {
                    _session.Set(key, k);
                    OnPropertyChanged(key);
                }

                AdornerVisibility = Visibility.Hidden;
                OnPropertyChanged("AdornerVisibility");
            });
        }

        public Keys ChangeDeveloperKey
        {
            get
            {
                return _session.Get<Keys>(Session.ChangeDeveloperKey);
            }
        }

        public Keys EndKataKey
        {
            get
            {
                return _session.Get<Keys>(Session.EndKataKey);
            }
        }

        public int ValueForFinishHimTime
        {
            get
            {
                return (int)(_session.Get<TimeSpan>(Session.FinishHimTime).TotalSeconds - FinishHimTimeOffset) / 5;
            }

            set
            {
                _session.Set(Session.FinishHimTime, TimeSpan.FromSeconds(FinishHimTimeOffset + value * 5));
                OnPropertyChanged("FinishHimTime");
            }
        }

        public int ValueForCycleTime
        {
            get
            {
                return (int)(_session.Get<TimeSpan>(Session.CycleTime).TotalSeconds - FinishHimTimeOffset) / 5;
            }
        }

        public TimeSpan FinishHimTime
        {
            get
            {
                return _session.Get<TimeSpan>(Session.FinishHimTime);
            }
        }

        public bool FinishHimTimeActive
        {
            get
            {
                return _session.Get<bool>(Session.FinishHimTimeActive);
            }

            set
            {
                _session.Set(Session.FinishHimTimeActive, value);
                OnPropertyChanged("FinishHimTimeActive");
            }
        }

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}