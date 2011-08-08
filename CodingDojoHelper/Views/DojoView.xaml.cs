using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using CodingDojoHelper.Events;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using CodingDojoHelper.ViewModels;
using Microsoft.Practices.Prism.Events;

namespace CodingDojoHelper.Views
{
    internal partial class DojoView
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISession _session;

        public DojoView(DojoViewModel vm, IEventAggregator eventAggregator, ISession session)
        {
            _eventAggregator = eventAggregator;
            _session = session;

            InitializeComponent();

            vm.PropertyChanged += OnLastKeyChanged;
            Loaded += (s, e) => DataContext = vm;
            _eventAggregator.GetEvent<KataFinishedEvent>().Subscribe(OnKataFinishedEvent);
        }

        private void OnLastKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (((DojoViewModel)sender).LastKey == _session.Get<Keys>(Session.ChangeDeveloperKey))
            {
                Dispatcher.Invoke(((Action)(() => ((Storyboard)Resources["ChangeDeveloper"]).Begin())));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ResetKataEvent>().Publish(null);
        }

        private void OnKataFinishedEvent(ICodingDojo obj)
        {
            ((Storyboard)Resources["EndScreen"]).Begin();

            while (((Storyboard)Resources["EndScreen"]).GetCurrentState() != ClockState.Stopped)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
