using CodingDojoHelper.ViewModels;

namespace CodingDojoHelper.Views
{
    internal partial class AdvancedConfigView
    {
        public AdvancedConfigView(AdvancedConfigViewModel vm)
        {
            InitializeComponent();

            Loaded += (s, e) => DataContext = vm;
        }
    }
}
