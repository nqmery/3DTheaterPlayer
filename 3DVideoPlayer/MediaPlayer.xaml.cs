//参考(xamlも):
//https://qiita.com/jakucho0926/items/d2d71080bd1f5c39495a#%E5%AE%9F%E8%A3%85%E6%96%B9%E6%B3%95%E3%81%AB%E3%81%A4%E3%81%84%E3%81%A6


using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

using _3DVideoPlayer;

using UserControl = System.Windows.Controls.UserControl;//WPFとWinForms間の参照のバッティングを防ぐ
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Size = System.Windows.Size;

namespace WPF_MediaPlayer.Controls
{
    /// <summary>
    /// MediaPlayer.xaml の相互作用ロジック
    /// </summary>
    public partial class MediaPlayer : UserControl
    {

        #region Dependency Properties

        // SourceProperty に変更コールバックを追加
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(Uri),
            typeof(MediaPlayer),
            new PropertyMetadata(null, OnSourceChanged));

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mp = d as MediaPlayer;
            if (mp == null) return;

            var uri = e.NewValue as Uri;

            // Set MediaElement.Source
            if (mp.Media != null)
            {
                mp.Media.Source = uri;
            }

            // Also set MediaTimeline.Source if present
            try
            {
                var mediaTimeline = mp.TimelineStory?.Children?.OfType<MediaTimeline>()?.FirstOrDefault();
                if (mediaTimeline != null)
                {
                    mediaTimeline.Source = uri;
                }
            }
            catch
            {
                // FindResource can throw if resources not yet available; ignore safely here
            }
        }

        #endregion

        #region Properties

        public Storyboard TimelineStory
        {
            get { return (Storyboard)this.FindResource(nameof(TimelineStory)); }
        }

        // 外部に位置や状態を通知するためのイベント
        public event EventHandler<TimeSpan> PositionChanged;
        public event EventHandler<TimeSpan> DurationChanged;
        public event EventHandler<Size> VideoSizeChanged;

        public double DefaultVideoMarginLeft { get; private set; }
        public double DefaultVideoMarginTop { get; private set; }

        protected bool IsPlaying
        {
            get { return this.Media.Clock != null && !this.IsPaused && !this.IsStopped; }
        }
        //外部から再生状態を知りたい場合に使う
        public bool IsPlayingPublic
        {
            get { return this.IsPlaying; }
        }

        protected bool IsPaused
        {
            get { return this.Media.Clock != null && this.Media.Clock.IsPaused; }
        }
        //外部から停止状態を知りたい場合に使う
        public bool IsPausedPublic
        {
            get { return this.IsPaused; }
        }

        protected bool IsStopped
        {
            get { return this.Media.Clock == null || this.Media.Clock.CurrentState.HasFlag(ClockState.Stopped); }
        }
        public bool IsStoppedPublic
        {
            get { return this.IsStopped; }
        }

        #endregion

        #region Constructors

        public MediaPlayer()
        {
            InitializeComponent();
        }

        #endregion

        #region Container Events

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.MediaPlayPause:
                    this.Play();
                    break;

                case Key.Space:
                    if (this.IsPlaying)
                    {
                        this.Pause();
                    }
                    else
                    {
                        this.Play();
                    }
                    break;

                case Key.Left:
                    this.Rewind(TimeSpan.FromSeconds(10));
                    break;

                case Key.Right:
                    this.Forward(TimeSpan.FromSeconds(10));
                    break;
            }
        }

        #endregion

        #region Control Events

        private void Media_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            // Storyboard の MediaTimeline がリソース内バインディングで Source を得られない場合があるため、
            // 明示的に Media.Source を割り当ててから再生する。
            var mediaTimeline = this.TimelineStory.Children.OfType<MediaTimeline>().FirstOrDefault();
            if (mediaTimeline != null && mediaTimeline.Source == null)
            {
                mediaTimeline.Source = this.Media.Source;
            }

            // Media.Source が確実にあるなら Play を呼ぶ。なければ MediaOpened で開始する方が安全。
            if (this.Media.Source != null)
            {
                this.Play();
                this.Stop();
            }
        }

        private void Media_MediaOpened(object sender, EventArgs e)
        {
            this.SeekSlider.Maximum = this.Media.NaturalDuration.TimeSpan.TotalMilliseconds;
            // ここで開始したければ Play() を呼べます（MediaOpened はソースが確定した後なので安全）。
            //this.Play();
            //this.Pause();

            // DurationChanged を発火
            DurationChanged?.Invoke(this, this.Media.NaturalDuration.TimeSpan);

            //初期位置を取得して保持
            this.DefaultVideoMarginLeft = this.Media.Margin.Left;
            this.DefaultVideoMarginTop = this.Media.Margin.Top;

            //動画のサイズを取得して発火
            videoWidthNow = (int)this.Media.ActualWidth;
            videoHeightNow = (int)this.Media.ActualHeight;
            VideoSizeChanged?.Invoke(this, new Size(videoWidthNow, videoHeightNow));
            App.WriteLog("ファイルの読み込みが完了しました");

            App.WriteLog("元動画のサイズ: " + (int)this.Media.NaturalVideoWidth + "x" + (int)this.Media.NaturalVideoWidth);
        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.Pause();
            this.Seek(TimeSpan.FromSeconds(0));
        }

        private void Media_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsPlaying)
            {
                this.Pause();
            }
            if (this.IsStopped)
            {
                this.Play();
            }
            if (this.IsPaused)
            {
                this.Play(TimeSpan.FromMilliseconds(this.SeekSlider.Value));
            }
        }

        private void MediaTimeline_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            this.SeekSlider.Value = this.Media.Position.TotalMilliseconds;

            // PositionChanged を発火
            PositionChanged?.Invoke(this, this.Media.Position);
        }

        private void SeekSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SeekSlider.IsMoveToPointEnabled = true;
            if (this.IsPlaying)
            {
                this.Pause();
            }
            if (this.IsStopped)
            {
                this.Play(TimeSpan.FromMilliseconds(this.SeekSlider.Value));
                this.Pause();
            }
        }

        private void SeekSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsPaused)
            {
                this.Play(TimeSpan.FromMilliseconds(this.SeekSlider.Value));
            }
            this.SeekSlider.IsMoveToPointEnabled = false;
        }

        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.SeekSlider.IsMoveToPointEnabled)
            {
                this.TimelineStory.Seek(TimeSpan.FromMilliseconds(this.SeekSlider.Value));
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        #endregion

        #region Methods

        public void Play()
        {
            if (this.IsStopped)
            {
                this.TimelineStory.Begin();
            }
            if (this.IsPaused)
            {
                this.TimelineStory.Resume();
            }
        }

        public void Play(TimeSpan position
        )
        {
            this.Seek(position);
            this.Play();
        }

        public void Seek(TimeSpan timeSpan)
        {
            var value = timeSpan;
            if (value.TotalMilliseconds < 0)
            {
                value = new TimeSpan();
            }
            this.TimelineStory.Seek(value);
        }

        public void Rewind(TimeSpan timeSpan)
        {
            this.Seek(TimeSpan.FromMilliseconds(this.SeekSlider.Value).Subtract(timeSpan));
        }

        public void Forward(TimeSpan timeSpan)
        {
            this.Seek(TimeSpan.FromMilliseconds(this.SeekSlider.Value).Add(timeSpan));
        }

        public void Pause()
        {
            this.TimelineStory.Pause();
        }

        public void Stop()
        {
            this.TimelineStory.Stop();
        }

        #endregion

        
        
        public void ChangeVideoHeight(double height)
        {
            this.Media.Height = height;
        }
        /* RenderTransfotmを使うため不使用
         public void ChangeVideoPosition(double x, double y)
        {
            Thickness margin = this.Media.Margin;
            margin.Left = DefaultVideoMarginLeft + x;
            margin.Top = DefaultVideoMarginTop + y;
            this.Media.Margin = margin;
        }*/


        //MediaElementを左に1px移動するテスト用メソッド
        public void TestMoveLeft()
        {
            Thickness margin = this.Media.Margin;
            margin.Left -= 1;
            this.Media.Margin = margin;
        }
        //テスト
        private void TestMoveLeft(object sender, RoutedEventArgs e)
        {
            this.Media.Width = this.Media.Width - 1;

        }


        public int videoWidthNow { get; private set; }
        public int videoHeightNow { get; private set; }

        private void Media_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 正しく幅と高さを更新（元コードは高さを二回代入するバグあり）
            videoWidthNow = (int)this.Media.ActualWidth;
            videoHeightNow = (int)this.Media.ActualHeight;

            // イベント発火（UI スレッド上なのでそのまま呼べる）
            VideoSizeChanged?.Invoke(this, new Size(videoWidthNow, videoHeightNow));
        }
    }
}
