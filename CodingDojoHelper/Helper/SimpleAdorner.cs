using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace MCC.PresentationUtilities.Controls
{
    internal class SimpleAdorner : Adorner
    {
        protected ContentPresenter ContentPresenter;

        public SimpleAdorner(UIElement adornedElement, object elementOnAdorner) :
            base(adornedElement)
        {
            ContentPresenter = new ContentPresenter();
            ContentPresenter.Content = elementOnAdorner;

            SetMargin(adornedElement);

            if (adornedElement is ItemsControl && !(adornedElement is ComboBox))
            {
                ContentPresenter.VerticalAlignment = VerticalAlignment.Center;
                ContentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
            }

            // Hide the control adorner when the adorned element is hidden
            Binding binding = new Binding("IsVisible");
            binding.Source = adornedElement;
            binding.Converter = new BooleanToVisibilityConverter();
            SetBinding(VisibilityProperty, binding);
        }

        private void SetMargin(UIElement adornedElement)
        {
            if (adornedElement is Control)
                ContentPresenter.Margin = new Thickness(((Control)adornedElement).Margin.Left + ((Control)adornedElement).Padding.Left,
                                                         ((Control)adornedElement).Margin.Top + ((Control)adornedElement).Padding.Top, 0, 0);
            else if (adornedElement is FrameworkElement)
                ContentPresenter.Margin = new Thickness(((FrameworkElement)adornedElement).Margin.Left, ((FrameworkElement)adornedElement).Margin.Top, 0, 0);
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return ContentPresenter;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            // Here's the secret to getting the adorner to cover the whole control
            ContentPresenter.Measure(AdornedElement.RenderSize);
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ContentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public virtual bool ShowAdorner(AdornerVisibilityInState adornerVisibilityInState)
        {
            return true;
        }
    }
}