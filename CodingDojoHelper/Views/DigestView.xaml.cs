using System.ComponentModel;
using System.Windows;
using CodingDojoHelper.Events;
using CodingDojoHelper.ViewModels;
using Microsoft.Practices.Composite.Events;

namespace CodingDojoHelper.Views
{
    internal partial class DigestView
    {
        private DigestViewModel _vm;
        private readonly IEventAggregator _eventAggregator;

        public DigestView(DigestViewModel vm, IEventAggregator eventAggregator)
        {
            _vm = vm;
            _eventAggregator = eventAggregator;

            InitializeComponent();

            _vm.PropertyChanged += CreateChart;
            Loaded += (s, e) => DataContext = _vm;
        }

        private void CreateChart(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CycleTimes"))
            {
                _lineChart.SeriesSource = ((DigestViewModel)sender).CycleTimes;
                _lineChart.IDMemberPath = "Key";
                _lineChart.Refresh();

                _cycleTimeLineChartGraph.DataItemsSource = ((DigestViewModel)sender).CycleTimes;
                _cycleTimeLineChartGraph.SeriesIDMemberPath = "Key";
                _cycleTimeLineChartGraph.ValueMemberPath = "Value";

                _averageTimeLineChartGraph.DataItemsSource = ((DigestViewModel)sender).Average;
                _averageTimeLineChartGraph.SeriesIDMemberPath = "Key";
                _averageTimeLineChartGraph.ValueMemberPath = "Value";

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ResetKataEvent>().Publish(null);
        }
    }
}
