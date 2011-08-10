using System.Runtime.InteropServices;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;

namespace MufflonoSoft.CodingDojoHelperVsExtension
{
    [Guid("486687b3-3286-49a5-a11c-74d829df845d")]
    public class MyToolWindow : ToolWindowPane
    {
        public MyToolWindow() :
            base(null)
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            var bootstrapper = new CodingDojoHelper.Bootstrapper();
            bootstrapper.Run();

            var shell = bootstrapper.Shell;
            shell.Background = new SolidColorBrush(Colors.Black);

            base.Content = shell;
        }
    }
}