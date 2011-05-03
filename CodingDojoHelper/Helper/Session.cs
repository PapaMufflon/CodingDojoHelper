using System;
using System.Collections.Generic;
using CodingDojoHelper.Helper.Interfaces;

namespace CodingDojoHelper.Helper
{
    class Session : ISession
    {
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        internal const string CycleTime = "CycleTime";
        internal const string DojoTime = "DojoTime";
        internal const string CombatantImages = "CombatantImages";
        internal const string FinishHimTime = "FinishHimTime";
        internal const string FinishHimTimeActive = "FinishHimTimeActive";
        internal const string ChangeDeveloperKey = "ChangeDeveloperKey";
        internal const string EndKataKey = "EndKataKey";

        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        #region ISession Members

        public void Set(string key, object value)
        {
            object oldValue = null;

            if (_dictionary.ContainsKey(key))
            {
                oldValue = _dictionary[key];
                _dictionary.Remove(key);
            }

            _dictionary.Add(key, value);

            OnValueChanged(key, oldValue, value);
        }

        public T Get<T>(string key)
        {
            if (!_dictionary.ContainsKey(key))
                throw new ArgumentException("key");

            try
            {
                var value = (T) _dictionary[key];
                return value;
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion

        private void OnValueChanged(string key, object oldValue, object newValue)
        {
            if (ValueChanged != null)
                ValueChanged(this, new ValueChangedEventArgs(key, oldValue, newValue));
        }
    }
}
