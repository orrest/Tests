using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using SkiaSharp;
using SkiaSharp.Skottie;
using SkiaSharp.Views.Desktop;

namespace Tests.Wpf.Skottie;

public partial class SkottieView : UserControl
{
    private readonly SkottieViewModel viewModel;
    private readonly Stopwatch clock = Stopwatch.StartNew();
    private Animation? animation;
    private TimeSpan lastFrame;
    private double playbackTime;
    private bool isSeeking;

    public SkottieView(SkottieViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = viewModel;

        this.viewModel.PropertyChanged += OnViewModelPropertyChanged;
        this.viewModel.Stopped += OnViewModelStopped;

        // Thumb drag events bubble from the Slider template, so hook them on the Slider itself.
        // Thumb 拖拽事件会从 Slider 模板冒泡，因此直接在 Slider 上挂接。
        this.SeekSlider.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnSeekDragStarted));
        this.SeekSlider.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnSeekDragCompleted));
        this.SeekSlider.ValueChanged += OnSeekValueChanged;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadSelectedClip();
        this.lastFrame = this.clock.Elapsed;
        CompositionTarget.Rendering += OnRendering;
    }

    // Detach the render loop when the page is switched away, otherwise it keeps burning CPU.
    // 切走页面时注销渲染循环，否则会持续占用 CPU。
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SkottieViewModel.SelectedClip))
        {
            LoadSelectedClip();
        }
    }

    private void OnViewModelStopped(object? sender, EventArgs e)
    {
        this.playbackTime = 0;
        this.SeekSlider.Value = 0;
        UpdateTimeText();
        AnimationHost.InvalidateVisual();
    }

    private void LoadSelectedClip()
    {
        var clip = this.viewModel.SelectedClip;
        if (clip is null)
        {
            return;
        }

        this.animation?.Dispose();
        this.animation = null;
        this.playbackTime = 0;

        if (!File.Exists(clip.FilePath))
        {
            this.viewModel.InfoText = $"File not found: {clip.FilePath}";
            UpdateTimeText();
            AnimationHost.InvalidateVisual();
            return;
        }

        this.animation = Animation.Create(clip.FilePath);
        if (this.animation is null)
        {
            this.viewModel.InfoText = $"Failed to parse Lottie JSON: {clip.FileName}";
            UpdateTimeText();
            AnimationHost.InvalidateVisual();
            return;
        }

        var frames = this.animation.OutPoint - this.animation.InPoint;
        var size = this.animation.Size;
        this.viewModel.InfoText =
            $"{clip.Title}  |  {size.Width:0}x{size.Height:0}  |  {this.animation.Fps:0.#} fps  |  {frames:0} frames  |  {this.animation.Duration.TotalSeconds:0.00}s  |  lottie {this.animation.Version}";
        UpdateTimeText();
        AnimationHost.InvalidateVisual();
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        if (this.animation is null)
        {
            return;
        }

        var now = this.clock.Elapsed;
        var delta = (now - this.lastFrame).TotalSeconds * this.viewModel.Speed;
        this.lastFrame = now;

        if (this.viewModel.IsPlaying)
        {
            var duration = this.animation.Duration.TotalSeconds;
            this.playbackTime += delta;
            if (this.playbackTime >= duration)
            {
                if (this.viewModel.IsLooping)
                {
                    this.playbackTime %= duration;
                }
                else
                {
                    this.playbackTime = duration;
                    this.viewModel.IsPlaying = false;
                }
            }

            if (!this.isSeeking)
            {
                this.SeekSlider.Value = duration <= 0 ? 0 : this.playbackTime / duration * 100d;
            }

            UpdateTimeText();
        }

        AnimationHost.InvalidateVisual();
    }

    private void OnSeekDragStarted(object sender, DragStartedEventArgs e)
    {
        this.isSeeking = true;
    }

    private void OnSeekValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!this.isSeeking || this.animation is null)
        {
            return;
        }

        this.playbackTime = this.SeekSlider.Value / 100d * this.animation.Duration.TotalSeconds;
        UpdateTimeText();
        AnimationHost.InvalidateVisual();
    }

    private void OnSeekDragCompleted(object sender, DragCompletedEventArgs e)
    {
        this.isSeeking = false;
    }

    private void UpdateTimeText()
    {
        var duration = this.animation?.Duration.TotalSeconds ?? 0;
        TimeText.Text = $"{this.playbackTime:0.00}s / {duration:0.00}s";
    }

    private void OnAnimationHostPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);
        if (this.animation is null)
        {
            return;
        }

        this.animation.SeekFrameTime(this.playbackTime);

        // Letterbox: keep the Lottie canvas aspect ratio inside the element.
        // 等比缩放：在元素内保持 Lottie 画布比例。
        var info = e.Info;
        var animSize = this.animation.Size;
        var scale = Math.Min(info.Width / animSize.Width, info.Height / animSize.Height);
        var width = animSize.Width * scale;
        var height = animSize.Height * scale;
        var dst = new SKRect(
            (info.Width - width) / 2f,
            (info.Height - height) / 2f,
            (info.Width + width) / 2f,
            (info.Height + height) / 2f);
        this.animation.Render(canvas, dst);
    }
}
