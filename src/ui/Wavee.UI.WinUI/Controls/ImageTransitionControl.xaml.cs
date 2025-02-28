using System;
using System.IO;
using System.Numerics;
using System.Runtime;
using System.Threading.Tasks;
using Windows.Foundation;
using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Wavee.UI.WinUI.Controls;

public sealed partial class ImageTransitionControl : UserControl
{
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        "Source", typeof(string), typeof(ImageTransitionControl),
        new PropertyMetadata(default(string), OnSourcePropertyChanged));

    //public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
    //    "Stretch", typeof(Stretch), typeof(ImageTransitionControl),
    //    new PropertyMetadata(default(Stretch), OnStretchPropertyChanged));

    // Using a DependencyProperty as the backing store for TransitionDuration.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TransitionDurationProperty =
        DependencyProperty.Register("TransitionDuration", typeof(TimeSpan), typeof(ImageTransitionControl),
            new PropertyMetadata(TimeSpan.Parse("00:00:01"),
                (d, e) =>
                {
                    var control = (ImageTransitionControl)d;
                    var newValue = (TimeSpan)e.NewValue;

                    // var fadeInAnim = (Storyboard)control.Resources["FadeInAnim"];
                    // fadeInAnim.Children[0].Duration = newValue;
                    //
                    // var fadeOutAnim = (Storyboard)control.Resources["FadeOutAnim"];
                    // fadeOutAnim.Children[0].Duration = newValue;
                }));

    public static readonly DependencyProperty BlurValueProperty =
        DependencyProperty.Register("BlurValue", typeof(double),
            typeof(ImageTransitionControl), new PropertyMetadata(default(double), OnBlurValueChanged));

    public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(ImageTransitionControl), new PropertyMetadata(default(double)));
    public static readonly DependencyProperty WithScaleProperty = DependencyProperty.Register(nameof(WithScale), typeof(bool), typeof(ImageTransitionControl), new PropertyMetadata(true, OnScaleProeprtyChanged));

    private static void OnScaleProeprtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as ImageTransitionControl;
        control.Br.WithScale = (bool)e.NewValue;
    }

    public ImageTransitionControl()
    {
        InitializeComponent();
    }

    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /*
            public Stretch Stretch
            {
                get => (Stretch)GetValue(StretchProperty);
                set => SetValue(StretchProperty, value);
            }
    */

    public TimeSpan TransitionDuration
    {
        get => (TimeSpan)GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, value);
    }

    public double BlurValue
    {
        get => (double)GetValue(BlurValueProperty);
        set => SetValue(BlurValueProperty, value);
    }

    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public bool WithScale
    {
        get => (bool)GetValue(WithScaleProperty);
        set => SetValue(WithScaleProperty, value);
    }

    private static void OnBlurValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ImageTransitionControl)d;

        var newValue = (double)e.NewValue;
        control.Br.Blur = newValue;
    }

    private static async void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var newImage = e.NewValue as string;
        var oldImage = e.OldValue as string;

        if (newImage == oldImage)
            return;
        var control = (ImageTransitionControl)d;
        if (string.IsNullOrEmpty(newImage))
        {
            await control.FadeOut();
            return;
        }

        if (control.Br.Source != null)
        {
            await control.FadeOut();
            await control.FadeIn();
        }
        else
        {
            control.Br.Source = new BitmapImage(new Uri(newImage));
            //control.FadeIn();
        }
    }

    private static void OnStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ImageTransitionControl)d;
        // control.Image.Stretch = (Stretch)e.NewValue;
    }

    private void Image_OnImageOpened(object sender, RoutedEventArgs e)
    {
        FadeIn();
    }

    private Task FadeOut()
    {
        // var s = (Storyboard)Resources["FadeOutAnim"];
        // s.Begin();
        return Task.CompletedTask;
    }

    public Task FadeIn()
    {
        // var s = (Storyboard)Resources["FadeInAnim"];
        // s.Begin();
        var baseAnimation = AnimationBuilder
            .Create()
            .Opacity(
                from: 0,
                to: 1,
                duration: TimeSpan.FromMilliseconds(400));

        if (WithScale)
        {
            baseAnimation = baseAnimation.Scale(to: new Vector2(1.15f, 1.15f),
                from: new Vector2(1f, 1f),
                duration: TimeSpan.FromSeconds(0.8),
                easingMode: EasingMode.EaseInOut,
                easingType: EasingType.Quintic,
                delay: TimeSpan.FromMilliseconds(20)
            );
        }

        baseAnimation.Start(Image);
        return Task.CompletedTask;
    }

    private void ImageFadeOutAnim_OnCompleted(object sender, object e)
    {
        if (string.IsNullOrEmpty(Source)) return;
        Br.Source = new BitmapImage(new Uri(Source));

        //if it's a local image, fade in immediately
        FadeIn();
    }

    private void Br_OnOpened(ImageOpacityBrush sender, EventArgs args)
    {
        FadeIn();
    }

    private void Image_OnLoaded(object sender, RoutedEventArgs e)
    {
        AnimationBuilder
                .Create()
                .Opacity(
                    from: 1,
                    to: 0,
                    duration: TimeSpan.FromMilliseconds(1))
                .Start(Image);
    }
}


public class ImageOpacityBrush : XamlCompositionBrushBase, IDisposable
{
    /// <summary>
    ///     Identifies the <see cref="Source" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(ImageSource), // We use ImageSource type so XAML engine will automatically construct proper object from String.
        typeof(ImageOpacityBrush),
        new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    ///     Identifies the <see cref="Stretch" /> dependency property.
    ///     Requires 16299 or higher for modes other than None.
    /// </summary>
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
        nameof(Stretch),
        typeof(Stretch),
        typeof(ImageOpacityBrush),
        new PropertyMetadata(Stretch.None, OnPropertyChanged));

    public static readonly DependencyProperty BlurProperty = DependencyProperty.Register(
        nameof(Blur),
        typeof(double),
        typeof(ImageOpacityBrush),
        new PropertyMetadata(default(double), OnBlurPropertyChanged));


    private LoadedImageSurface _surface;
    private CompositionSurfaceBrush _surfaceBrush;
    public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(ImageOpacityBrush), new PropertyMetadata(default(double), OffsetChanged));
    public static readonly DependencyProperty WithScaleProperty = DependencyProperty.Register(nameof(WithScale), typeof(bool), typeof(ImageOpacityBrush), new PropertyMetadata(default(bool), OffsetChanged));

    /// <summary>
    ///     Gets or sets the <see cref="BitmapImage" /> source of the image to composite.
    /// </summary>
    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }


    /// <summary>
    ///     Gets or sets how to stretch the image within the brush.
    /// </summary>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public double Blur
    {
        get => (double)GetValue(BlurProperty);
        set => SetValue(BlurProperty, value);
    }

    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public bool WithScale
    {
        get => (bool)GetValue(WithScaleProperty);
        set => SetValue(WithScaleProperty, value);
    }


    public void Dispose()
    {
        // newSurface?.Dispose();
    }

    public event TypedEventHandler<ImageOpacityBrush, EventArgs> Opened;

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var brush = (ImageOpacityBrush)d;
        brush.OnDisconnected();
        brush.OnConnected();
    }
    private static void OffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var brush = (ImageOpacityBrush)d;
        brush.OnDisconnected();
        brush.OnConnected();
    }

    private static void OnBlurPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var brush = (ImageOpacityBrush)d;

        brush.CompositionBrush?.Properties
            .InsertScalar("blur.BlurAmount", (float)(double)e.NewValue);
    }

    /// <inheritdoc />
    protected override void OnConnected()
    {
        // Delay creating composition resources until they're required.
        if (CompositionBrush == null && Source is BitmapImage bitmap)
        {
            if (bitmap.UriSource == null) return;
            // Use LoadedImageSurface API to get ICompositionSurface from image uri provided
            // If UriSource is invalid, StartLoadFromUri will return a blank texture.
            _surface = LoadedImageSurface.StartLoadFromUri(bitmap.UriSource);

            // Load Surface onto SurfaceBrush
            _surfaceBrush = App.MainWindow.Compositor.CreateSurfaceBrush(_surface);
            _surfaceBrush.Stretch = CompositionStretch.UniformToFill;
            //offset a bit
            _surfaceBrush.Offset = new Vector2(0, (float)OffsetY);


            var compositor = App.MainWindow.Compositor;
            var blurEffect = new GaussianBlurEffect
            {
                Name = "blur",
                BlurAmount = (float)Blur,
                BorderMode = EffectBorderMode.Hard,
                Optimization = EffectOptimization.Balanced,
                Source = new CompositionEffectSourceParameter("Source")
            };


            var asset = Path.Combine(AppContext.BaseDirectory, "Assets", "amb.png");
            var opacityMaskSurface = LoadedImageSurface.StartLoadFromUri(new Uri(asset));
            var opacityBrush = compositor.CreateSurfaceBrush(opacityMaskSurface);
            opacityBrush.Stretch = CompositionStretch.Fill;

            var effect = new AlphaMaskEffect
            {
                Source = blurEffect,
                AlphaMask = new CompositionEffectSourceParameter("Mask")
            };
            var brush = compositor.CreateEffectFactory(
                    effect,
                    new[] { "blur.BlurAmount" })
                .CreateBrush();
            brush.SetSourceParameter("Source", compositor.CreateBackdropBrush());

            brush.SetSourceParameter("Source", _surfaceBrush);
            brush.SetSourceParameter("Mask", opacityBrush);
            CompositionBrush = brush;
            _surface.LoadCompleted += SurfaceOnLoadCompleted;
        }
    }

    private void SurfaceOnLoadCompleted(LoadedImageSurface sender, LoadedImageSourceLoadCompletedEventArgs args)
    {
        Opened?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    protected override void OnDisconnected()
    {
        // Dispose of composition resources when no longer in use.
        if (CompositionBrush != null)
        {
            CompositionBrush.Dispose();
            CompositionBrush = null;
        }

        if (_surfaceBrush != null)
        {
            _surfaceBrush.Dispose();
            _surfaceBrush = null;
        }

        if (_surface != null)
        {
            _surface.LoadCompleted -= SurfaceOnLoadCompleted;
            try
            {
                _surface.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }

            _surface = null;
        }
    }

    //// Helper to allow XAML developer to use XAML stretch property rather than another enum.
    private static CompositionStretch CompositionStretchFromStretch(Stretch value)
    {
        switch (value)
        {
            case Stretch.None:
                return CompositionStretch.None;
            case Stretch.Fill:
                return CompositionStretch.Fill;
            case Stretch.Uniform:
                return CompositionStretch.Uniform;
            case Stretch.UniformToFill:
                return CompositionStretch.UniformToFill;
        }

        return CompositionStretch.None;
    }
}
