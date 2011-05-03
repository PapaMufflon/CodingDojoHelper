using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CodingDojoHelper.Helper
{
    class GifImage : Image
    {
        public int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));

        static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GifImage ob = obj as GifImage;
            ob.Source = ob.gf.Frames[(int)ev.NewValue];
            ob.InvalidateVisual();
        }

        public Uri Uri
        {
            get { return (Uri)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(Uri), typeof(GifImage), new UIPropertyMetadata(null, OnUriChanged));

        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue as Uri == null)
                return;

            var gifImage = d as GifImage;

            try
            {
                var stream = Application.GetResourceStream((Uri)e.NewValue).Stream;
                gifImage.gf = new GifBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            }
            catch (ArgumentException) { return; }

            var framesCount = gifImage.gf.Frames.Count;
            gifImage.anim = new Int32Animation(0, framesCount - 1, new Duration(new TimeSpan(0, 0, 0, framesCount / 10, (int)((framesCount / 10.0 - framesCount / 10) * 1000))));
            gifImage.anim.RepeatBehavior = RepeatBehavior.Forever;
            gifImage.Source = gifImage.gf.Frames[0];
        }

        public GifBitmapDecoder gf;
        public Int32Animation anim;
        bool animationIsWorking = false;
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (!animationIsWorking)
            {
                BeginAnimation(FrameIndexProperty, anim);
                animationIsWorking = true;
            }
        }
    }
}
