using CodingDojoHelper;

namespace CodingDojoHelperDesktop
{
    public partial class MainWindow
    {
        private Bootstrapper _bootstrapper;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _bootstrapper = new Bootstrapper();
            _bootstrapper.Run();

            MouseLeftButtonDown += (s, e) => DragMove();
        }

        public object CodingDojo
        {
            get
            {
                return _bootstrapper.Shell;
            }
        }

        private void endButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
