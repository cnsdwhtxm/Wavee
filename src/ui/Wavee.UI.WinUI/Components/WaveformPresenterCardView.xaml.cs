using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.Foundation;
using Windows.System.Threading;
using AudioEffectsLib;
using CommunityToolkit.Labs.WinUI;
using LanguageExt.UnsafeValueAccess;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using NAudio.Wave;
using Microsoft.UI.Xaml.Hosting;
using UIElement = Microsoft.UI.Xaml.UIElement;

namespace Wavee.UI.WinUI.Components;

public sealed partial class WaveformPresenterCardView : UserControl, INotifyPropertyChanged
{
    List<Rectangle> lstBands;
    public static readonly DependencyProperty CardViewProperty = DependencyProperty.Register(nameof(CardView), typeof(CardView), typeof(WaveformPresenterCardView), new PropertyMetadata(default(CardView)));
    private readonly DispatcherQueue _dispatcherQueue;

    private static readonly NAudio.Wave.IWavePlayer PreviewPlayer = new WaveOutEvent
    {
        Volume = 0.5f
    };
    public WaveformPresenterCardView()
    {
        this.InitializeComponent();
        _dispatcherQueue = this.DispatcherQueue;
    }
    private InterveneSampleProvider? _interveneSampleProvider;

    public CardView CardView
    {
        get => (CardView)GetValue(CardViewProperty);
        set => SetValue(CardViewProperty, value);
    }

    public bool PopupLoad
    {
        get => _popupLoad;
        set => SetField(ref _popupLoad, value);
    }

    public static readonly ConcurrentDictionary<UIElement, bool> _inRectangle = new ConcurrentDictionary<UIElement, bool>();
    private static CancellationTokenSource cts;
    private bool _popupLoad;

    private async void FirstControl_OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {

        //If the pointer hovered over the card for 4 seconds, then show the tooltip
        //Otherwise, do nothing
        PopupLoad = true;
        cts ??= new CancellationTokenSource();
        var tokenInstance = cts.Token;
        var item = sender as UIElement;
        _inRectangle[item] = false;
        try
        {
            await Task.Delay(2000, tokenInstance);
        }
        catch (Exception)
        {
            _inRectangle.TryRemove(item, out _);
        }

        try
        {
            if (tokenInstance.IsCancellationRequested) return;
            if (_inRectangle.TryGetValue(item, out _))
            {
                var transition = (TransitionHelper)this.Resources["MyTransitionHelper"];
                transition.Source = (FrameworkElement)item;
                transition.Target = SecondControl;

                _inRectangle[item] = true;
                SecondControlPopup.IsOpen = true;
                await transition.StartAsync();

                lstBands = GenerateBands(40, 6, 5);
                var crvd = CardView;
                MediaFoundationReader? waveProvider = null;
                await Task.Run(async () =>
                {
                    try
                    {
                        if (PreviewPlayer.PlaybackState == PlaybackState.Playing)
                        {
                            PreviewPlayer.Stop();
                            _interveneSampleProvider?.Dispose();
                            _interveneSampleProvider = null;
                        }

                        // var bs = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "4dba53850d6bfdb9800d53d65fe2e5f1369b9040.mp3");
                        //var waveProvider = new Mp3FileReader(bs);
                        var previewStreams = await crvd.GetPreviewStreamsAsync(tokenInstance);
                        var previewStreamUrl = previewStreams.ValueUnsafe();
                        var firstStream = await Task.Run(() => previewStreamUrl, tokenInstance);
                        waveProvider = new MediaFoundationReader(firstStream);
                        _interveneSampleProvider = new InterveneSampleProvider(waveProvider, 2, 65, 40, 20, 500, 46);
                        InterveneSampleProvider.SpectrumDataReady += Capture_AudioDataAvailable;
                        PreviewPlayer.Init(_interveneSampleProvider);
                        PreviewPlayer.Play();
                        while (PreviewPlayer.PlaybackState == PlaybackState.Playing)
                        {
                            await Task.Delay(100, tokenInstance);
                        }
                    }
                    catch (Exception x)
                    {
                        //"   at System.Text.Json.JsonElement.GetProperty(String propertyName)\r\n   at Wavee.Infrastructure.Public.Live.LiveSpotifyPublicClient.<DeserializePagedResponse>d__10`1.MoveNext() in C:\\Users\\chris-pc\\Desktop\\Wavee\\Wavee\\src\\lib\\Wavee\\Infrastructure\\Public\\Live\\LiveSpotifyPublicClient.cs:line 138\r\n   at LanguageExt.TaskExtensions.<MapAsync>d__23`2.MoveNext()\r\n   at LanguageExt.TaskExtensions.<Map>d__22`2.MoveNext()\r\n   at Wavee.UI.Client.Previews.SpotifyUIPreviewClient.<GetPreviewStreamsForContext>d__2.MoveNext() in C:\\Users\\chris-pc\\Desktop\\Wavee\\Wavee\\src\\ui\\Wavee.UI\\Client\\Previews\\SpotifyUIPreviewClient.cs:line 31\r\n   at Wavee.UI.WinUI.Components.CardView.<GetPreviewStreamsAsync>d__48.MoveNext() in C:\\Users\\chris-pc\\Desktop\\Wavee\\Wavee\\src\\ui\\Wavee.UI.WinUI\\Components\\CardView.xaml.cs:line 219\r\n   at Wavee.UI.WinUI.Components.WaveformPresenterCardView.<>c__DisplayClass11_1.<<FirstControl_OnPointerEntered>b__0>d.MoveNext() in C:\\Users\\chris-pc\\Desktop\\Wavee\\Wavee\\src\\ui\\Wavee.UI.WinUI\\Components\\WaveformPresenterCardView.xaml.cs:line 94"
                    }
                    finally
                    {
                        if (waveProvider is not null)
                            await waveProvider.DisposeAsync();
                        _interveneSampleProvider?.Dispose();
                        InterveneSampleProvider.SpectrumDataReady -= Capture_AudioDataAvailable;
                        PreviewPlayer.Stop();
                    }
                });

                if (lstBands != null)
                {
                    foreach (Rectangle rect in lstBands)
                    {
                        rootp.Children.Remove(rect);
                    }

                    lstBands.Clear();
                    lstBands = null;
                    GC.Collect();
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            cts?.Dispose();
            cts = null;

            FirstControl_OnPointerExited(sender, e);
        }
    }

    private async void FirstControl_OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        try
        {
            if (cts is not null)
                cts.Cancel();

            var uiElement = sender as UIElement;
            _inRectangle.Remove(uiElement, out var transitioned);
            if (transitioned)
            {
                var transition = (TransitionHelper)this.Resources["MyTransitionHelper"];
                _interveneSampleProvider?.Dispose();
                PreviewPlayer.Stop();
                try
                {
                    await transition.ReverseAsync(true);
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    SecondControlPopup.IsOpen = false;
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            PopupLoad = false;
        }
    }

    private void SecondControl_OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        FirstControl_OnPointerExited(FirstControl, null);
    }

    public List<Rectangle> GenerateBands(int amount, double width, double gapwidth)
    {
        if (lstBands != null)
        {
            foreach (Rectangle rect in lstBands)
            {
                rootp.Children.Remove(rect);
            }
            lstBands.Clear();
            GC.Collect();
        }

        List<Rectangle> lstRects = new List<Rectangle>();
        for (int i = 0; i < amount; i++)
        {
            Rectangle rect = new Rectangle();
            rect.Height = 0;
            rect.Width = width;

            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.VerticalAlignment = VerticalAlignment.Bottom;

            double twidth = width + gapwidth;

            rect.Margin = new Thickness((twidth * i), 0, 0, 0);

            AcrylicBrush awb = (AcrylicBrush)Application.Current.Resources["SystemControlAccentAcrylicWindowAccentMediumHighBrush"];
            rect.Fill = awb;
            //rect.Fill = new SolidColorBrush(Colors.Green);
            rootp.Children.Add(rect);

            lstRects.Add(rect);
        }

        return lstRects;
    }

    private void Capture_AudioDataAvailable(object sender, object e)
    {
        try
        {

            _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, delegate ()
            {
                double[] audioData = new double[1];
                double elapsedTime = 0;
                try
                {
                    audioData = (double[])e;
                }
                catch
                {
                    return;
                }
                for (int i = 0; i < audioData.Length; i += 1)
                {
                    if (lstBands is null) break;
                    //double logamp = 100 + (100 * Math.Log10(e.AudioData[i] / 100));
                    double height = audioData[i] * 200;// *10000;//(e.AudioData[i] * polyheight) / (capture.WaveFormat.SampleRate *100);
                                                       //Debug.WriteLine(height);
                                                       //Debug.WriteLine(height);
                                                       //height = 30* Math.Log(height);

                    if (height < 0)
                    {
                        height = 0;
                        //height = Math.Abs(height);
                    }
                    else if (height > 200)
                    {
                        height = 200;
                        //height = Math.Abs(height);
                    }

                    try
                    {

                        lstBands[i].Height = height;

                    }
                    catch { }
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

        }
    }

    private async void SecondControlNavigatedionrequested(object sender, TappedRoutedEventArgs e)
    {
        try
        {
            if (cts is not null)
                cts.Cancel();
        }
        catch (Exception)
        {
            // ignored
        }

        _inRectangle.Clear();
        var transition = (TransitionHelper)this.Resources["MyTransitionHelper"];
        _interveneSampleProvider?.Dispose();
        PreviewPlayer.Stop();
        try
        {
            transition.ReverseDuration = TimeSpan.Zero;
            await transition.ReverseAsync();
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            SecondControlPopup.IsOpen = false;
        }

        CardView.Navigate();

    }

    private void WaveformPresenterCardView_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (cts is not null)
        {
            try
            {
                cts.Cancel();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                cts.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }

            cts = null;
        }
        _interveneSampleProvider?.Dispose();
        PreviewPlayer.Stop();
        _inRectangle.Clear();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

internal class InterveneSampleProvider : IWaveProvider, IDisposable
{
    private readonly MediaFoundationReader _toSampleProvider;
    AudioSpectrum spectrum;
    public InterveneSampleProvider(MediaFoundationReader toSampleProvider, double attack, double decay, int bands, double freqMin, double freqMac, double sensitivy)
    {
        _toSampleProvider = toSampleProvider;
        spectrum = new AudioSpectrum(WaveFormat.SampleRate, WaveFormat.BitsPerSample, WaveFormat.Channels);
        spectrum.Attack = attack;
        spectrum.Decay = decay;
        spectrum.Bands = bands;
        spectrum.FreqMax = freqMin;
        spectrum.FreqMax = freqMac;
        spectrum.Sensitivity = sensitivy;
        spectrum.UseFFT = true;
        spectrum.Window = AudioEffectsLib.FastFourierTransform.WFWindow.HammingWindow;
        spectrum.FFTSize = 8192;
        spectrum.FFTBufferSize = 32768;
        spectrum.Channel = 0;
        spectrum.SpectrumDataAvailable += Spectrum_SpectrumDataAvailable;


        spectrum.Start();
    }
    public static event EventHandler<object> SpectrumDataReady;

    private void Spectrum_SpectrumDataAvailable(object sender, AudioVisEventArgs e)
    {
        SpectrumDataReady?.Invoke(sender, e.AudioData);
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        var read = _toSampleProvider.Read(buffer, offset, count);
        //cast to bytes
        if (read is 0) return 0;
        //copy to new array, make sure offset is respected
        var audioData = new byte[read];
        for (int i = 0; i < read; i++)
        {
            audioData[i] = buffer[i + offset];
        }


        var audioBufferSize = audioData.Length;

        int realbuffersize;
        for (realbuffersize = audioBufferSize - 1; audioData[realbuffersize] == 0; realbuffersize--)
        {

        }

        Debug.WriteLine("Removed " + (audioBufferSize - realbuffersize) + " bytes of zero padding");

        byte[] data = new byte[realbuffersize];

        for (int i = 0; i < realbuffersize; i++)
        {
            data[i] = audioData[i];
        }


        //calculate how many milliseconds of data we have
        double milliseconds = (double)realbuffersize / (double)_toSampleProvider.WaveFormat.AverageBytesPerSecond * 1000.0;
        var ts = TimeSpan.FromMilliseconds(milliseconds);
        ThreadPoolTimer tppt = ThreadPoolTimer.CreateTimer((source) =>
        {
            try
            {
                (spectrum.AudioClient as AudioStreamWaveSource).Write(data, 0, realbuffersize);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }, ts);

        return read;
    }

    public WaveFormat WaveFormat => _toSampleProvider.WaveFormat;

    public void Dispose()
    {
        spectrum.SpectrumDataAvailable -= Spectrum_SpectrumDataAvailable;
        spectrum?.Dispose();
        _toSampleProvider.Dispose();
    }
}
