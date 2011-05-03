using System;

namespace CodingDojoHelper.Helper.Interfaces
{
    interface ISession
    {
        event EventHandler<ValueChangedEventArgs> ValueChanged;

        void Set(string key, object value);
        T Get<T>(string key);
    }
}
