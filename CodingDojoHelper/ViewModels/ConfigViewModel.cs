using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CodingDojoHelper.Events;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using Microsoft.Practices.Prism.Events;

namespace CodingDojoHelper.ViewModels
{
    enum ConfigValue
    {
        CombatantsCount,
        CycleTime,
        DojoTime
    }

    class ConfigViewModel : INotifyPropertyChanged
    {
        public const int MaxCombatants = 7;

        private const int MaxCycleTime = 91;
        private const int CycleTimeOffset = 25;
        private const int MaxDojoTime = 43;
        private const int DojoTimeOffset = 0;//25;

        public int Minimum { get; set; }
        public int Maximum { get; set; }

        private int _value;
        private ConfigValue _activeValue;
        private int _combatantsCount;
        private bool _internallyChangingValue;
        private readonly ISession _session;
        private readonly IEventAggregator _eventAggregator;
        private readonly List<Uri> _allCombatants;

        public ConfigViewModel(ISession session, IEventAggregator eventAggregator)
        {
            _session = session;
            _eventAggregator = eventAggregator;

            CombatantsCount = 3;
            ActiveValue = ConfigValue.CombatantsCount;
            SetupNewMode();

            _allCombatants = _session.Get<List<Uri>>(Session.CombatantImages);
        }

        public ConfigValue ActiveValue
        {
            get { return _activeValue; }
            set
            {
                _activeValue = value;
                SetupNewMode();
            }
        }

        private void SetupNewMode()
        {
            _internallyChangingValue = true;

            switch (ActiveValue)
            {
                case ConfigValue.CombatantsCount:
                    Maximum = MaxCombatants;
                    Value = CombatantsCount;
                    break;

                case ConfigValue.CycleTime:
                    Maximum = MaxCycleTime;
                    Value = (int)(CycleTime.TotalSeconds - CycleTimeOffset) / 5;
                    break;

                case ConfigValue.DojoTime:
                    Maximum = MaxDojoTime;
                    Value = (int) (DojoTime.TotalMinutes - DojoTimeOffset)/5;
                    break;

                default:
                    break;
            }

            _internallyChangingValue = false;

            OnPropertyChanged("Minimum");
            OnPropertyChanged("Maximum");
            OnPropertyChanged("Value");
        }

        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;

                if (_internallyChangingValue)
                    return;

                switch (ActiveValue)
                {
                    case ConfigValue.CombatantsCount:
                        CombatantsCount = value;
                        OnPropertyChanged("Combatants");
                        break;

                    case ConfigValue.CycleTime:
                        CycleTime = TimeSpan.FromSeconds(CycleTimeOffset + value * 5);
                        _session.Set(Session.CycleTime, CycleTime);
                        OnPropertyChanged("CycleTime");
                        break;

                    case ConfigValue.DojoTime:
                        DojoTime = TimeSpan.FromMinutes(DojoTimeOffset + value*5);
                        _session.Set(Session.DojoTime, DojoTime);
                        OnPropertyChanged("DojoTime");
                        break;

                    default:
                        break;
                }
            }
        }

        public TimeSpan CycleTime
        {
            get { return _session.Get<TimeSpan>(Session.CycleTime); }
            set
            {
                _session.Set(Session.CycleTime, value);
            }
        }
        
        public TimeSpan DojoTime
        {
            get { return _session.Get<TimeSpan>(Session.DojoTime); }
            set
            {
                _session.Set(Session.DojoTime, value);
            }
        }

        public int CombatantsCount
        {
            get { return _combatantsCount; }

            set
            {
                if (value < 1 || value > MaxCombatants)
                    throw new ArgumentException();

                _combatantsCount = value;
            }
        }

        public List<Uri> Combatants
        {
            get
            {
                return _allCombatants.Take(CombatantsCount).ToList();
            }
        }

        public ICommand StartKataCommand
        {
            get
            {
                return new ParameterlessDelegateCommand(() =>
                    _eventAggregator.GetEvent<StartKataEvent>().Publish(null));
            }
        }

        public ICommand ConfigCommand
        {
            get
            {
                return new ParameterlessDelegateCommand(() =>
                    _eventAggregator.GetEvent<ShowAdvancedConfigEvent>().Publish(null));
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