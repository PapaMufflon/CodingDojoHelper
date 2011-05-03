using System.Windows;
using System.Windows.Media;

namespace MCC.PresentationUtilities.Controls
{
    internal class GreyDisabledBackgroundAdorner : SimpleAdorner
    {
        public GreyDisabledBackgroundAdorner(UIElement adornedElement, object elementOnAdorner) : base(adornedElement, elementOnAdorner) {}

        public override bool ShowAdorner(AdornerVisibilityInState adornerVisibilityInState)
        {
            return true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var b = new SolidColorBrush(Colors.Black);
            b.Opacity = 0.25;

            drawingContext.DrawRectangle(b, new Pen(Brushes.Black, 0), new Rect(new Point(0,0), DesiredSize));

            base.OnRender(drawingContext);
        }
    }
}