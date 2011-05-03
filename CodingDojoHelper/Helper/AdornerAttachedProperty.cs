using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MCC.PresentationUtilities.Controls
{
    public enum AdornerVisibilityInState
    {
        ControlLoaded,
        GotKeyboardFocus,
        LostKeyboardFocus,
        ItemsChanged,
        ItemsSourceChanged
    }

    public static class AdornerAttachedProperty
    {
        private static readonly Dictionary<object, ItemsControl> ItemsControls = new Dictionary<object, ItemsControl>();

        public enum AdornerType
        {
            Simple,
            GreyDisabledBackground,
            Watermark
        }

        public static readonly DependencyProperty AdornerTypeProperty = DependencyProperty.RegisterAttached(
            "AdornerType",
            typeof(AdornerType),
            typeof(AdornerAttachedProperty),
            new FrameworkPropertyMetadata(AdornerType.Simple, OnAdornerTypeChanged));

        public static AdornerType GetAdornerType(DependencyObject d)
        {
            return (AdornerType)d.GetValue(AdornerTypeProperty);
        }

        public static void SetAdornerType(DependencyObject d, AdornerType value)
        {
            d.SetValue(AdornerTypeProperty, value);
        }

        private static void OnAdornerTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecideOnAdornerVisibility(d, AdornerVisibilityInState.ControlLoaded);
        }

        public static readonly DependencyProperty AdornerVisibilityProperty = DependencyProperty.RegisterAttached(
            "AdornerVisibility",
            typeof(Visibility),
            typeof(AdornerAttachedProperty),
            new FrameworkPropertyMetadata(Visibility.Visible, OnAdornerVisibilityChanged));

        public static Visibility GetAdornerVisibility(DependencyObject d)
        {
            return (Visibility)d.GetValue(AdornerVisibilityProperty);
        }

        public static void SetAdornerVisibility(DependencyObject d, Visibility value)
        {
            d.SetValue(AdornerVisibilityProperty, value);
        }

        private static void OnAdornerVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecideOnAdornerVisibility(d, AdornerVisibilityInState.ControlLoaded);
        }

        public static readonly DependencyProperty AdornerProperty = DependencyProperty.RegisterAttached(
           "Adorner",
           typeof(object),
           typeof(AdornerAttachedProperty),
           new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAdornerChanged)));

        public static object GetAdorner(DependencyObject d)
        {
            return d.GetValue(AdornerProperty);
        }

        public static void SetAdorner(DependencyObject d, object value)
        {
            d.SetValue(AdornerProperty, value);
        }

        private static void OnAdornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;
            frameworkElement.Loaded += (s, arg) => DecideOnAdornerVisibility(d, AdornerVisibilityInState.ControlLoaded);

            if (d is ComboBox || d is TextBox)
            {
                frameworkElement.GotKeyboardFocus += (s, arg) => DecideOnAdornerVisibility(d, AdornerVisibilityInState.GotKeyboardFocus);
                frameworkElement.LostKeyboardFocus += (s, arg) => DecideOnAdornerVisibility(d, AdornerVisibilityInState.LostKeyboardFocus);
            }

            if (d is ItemsControl && !(d is ComboBox))
            {
                var i = (ItemsControl)d;

                // for Items property  
                i.ItemContainerGenerator.ItemsChanged += (s, arg) =>
                {
                    ItemsControl itemsControl;
                    if (ItemsControls.TryGetValue(s, out itemsControl))
                        DecideOnAdornerVisibility(itemsControl, AdornerVisibilityInState.ItemsChanged);
                };

                ItemsControls.Add(i.ItemContainerGenerator, i);

                // for ItemsSource property  
                var prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, i.GetType());
                prop.AddValueChanged(i, (s, arg) => DecideOnAdornerVisibility(d, AdornerVisibilityInState.ItemsSourceChanged));
            }
        }

        #region Helper Methods

        private static void DecideOnAdornerVisibility(DependencyObject d, AdornerVisibilityInState adornerVisibilityInState)
        {
            if (GetAdornerVisibility(d) == Visibility.Visible)
            {
                var adorner = BuildAdorner(d);

                if (adorner.ShowAdorner(adornerVisibilityInState))
                {
                    ShowAdorner((FrameworkElement)d, adorner);
                    return;
                }
            }

            RemoveAdorner((FrameworkElement)d);
        }

        private static SimpleAdorner BuildAdorner(DependencyObject d)
        {
            switch (GetAdornerType(d))
            {
                case AdornerType.Simple:
                    return new SimpleAdorner((FrameworkElement) d, GetAdorner(d));

                case AdornerType.GreyDisabledBackground:
                    return new GreyDisabledBackgroundAdorner((FrameworkElement)d, GetAdorner(d));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void RemoveAdorner(FrameworkElement frameworkElement)
        {
            var layer = AdornerLayer.GetAdornerLayer(frameworkElement);

            // layer could be null if control is no longer in the visual tree
            if (layer != null)
            {
                var adorners = layer.GetAdorners(frameworkElement);

                if (adorners == null)
                    return;

                foreach (var adorner in adorners)
                {
                    adorner.Visibility = Visibility.Hidden;
                    layer.Remove(adorner);
                }
            }
        }

        private static void ShowAdorner(FrameworkElement frameworkElement, SimpleAdorner adorner)
        {
            var layer = AdornerLayer.GetAdornerLayer(frameworkElement);

            // layer could be null if control is no longer in the visual tree
            if (layer != null)
                layer.Add(adorner);
        }

        #endregion
    }
}