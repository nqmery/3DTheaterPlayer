using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        Window leftPlayer;
        WPF_MediaPlayer.Controls.MediaPlayer leftMediaPlayerControl;

        // フィードバックを防ぐためのフラグ
        private bool isMainSeekDragging = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_CreatePlayerWindow_Click(object sender, RoutedEventArgs e)
        {
            // UserControl を Window にラップして表示する
            leftMediaPlayerControl = new WPF_MediaPlayer.Controls.MediaPlayer();

            // イベント購読（シークバーの同期用）
            leftMediaPlayerControl.PositionChanged += LeftMediaPlayerControl_PositionChanged;
            leftMediaPlayerControl.DurationChanged += LeftMediaPlayerControl_DurationChanged;

            leftPlayer = new Window
            {
                Title = "左",
                Content = leftMediaPlayerControl,
                Background = new SolidColorBrush(Colors.Black),
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.None,
                //Topmost = true,
                Left = -300,
                Top = -1000,
                Width = 854,
                Height = 480
            };
            leftPlayer.Show();
            leftPlayer.WindowState = WindowState.Maximized;
        }

        private void LeftMediaPlayerControl_DurationChanged(object sender, System.TimeSpan duration)
        {
            // Media の長さがわかったら MainWindow のスライダー最大値を設定
            Dispatcher.Invoke(() =>
            {
                SeekSlider.Maximum = duration.TotalMilliseconds;
            });
        }

        private void LeftMediaPlayerControl_PositionChanged(object sender, System.TimeSpan position)
        {
            // Media 再生位置が更新されたら MainWindow のスライダーを同期（ユーザーがドラッグ中は更新しない）
            if (isMainSeekDragging) return;
            Dispatcher.Invoke(() =>
            {
                SeekSlider.Value = position.TotalMilliseconds;
            });//イベントは UI スレッド以外から来る可能性があるため、UI 要素の更新を UI スレッドで行う。
        }

        //MediaPlayerの操作関連(MediaPlayer.csからコピー)
        private void SeekSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMainSeekDragging = true;

            if (leftMediaPlayerControl != null)
            {
                leftMediaPlayerControl.SeekSlider.IsMoveToPointEnabled = true;
                if (leftMediaPlayerControl.IsPlayingPublic)
                {
                    leftMediaPlayerControl.Pause();
                }
                if (leftMediaPlayerControl.IsStoppedPublic)
                {
                    leftMediaPlayerControl.Play(TimeSpan.FromMilliseconds(leftMediaPlayerControl.SeekSlider.Value));
                    leftMediaPlayerControl.Pause();
                }
            }
        }
        private void SeekSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (leftMediaPlayerControl != null)
            {
                if (leftMediaPlayerControl.IsPausedPublic)
                {
                    leftMediaPlayerControl.Play(TimeSpan.FromMilliseconds(SeekSlider.Value));
                }
                leftMediaPlayerControl.SeekSlider.IsMoveToPointEnabled = false;
            }

            isMainSeekDragging = false;
        }


        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // ユーザーがメインウィンドウのスライダーを操作している場合、メディアプレイヤー側にシークを伝える
            if (isMainSeekDragging && leftMediaPlayerControl != null)
            {
                leftMediaPlayerControl.Seek(TimeSpan.FromMilliseconds(SeekSlider.Value));
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            leftMediaPlayerControl?.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            leftMediaPlayerControl?.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            leftMediaPlayerControl?.Stop();
        }

        
        private void MoveLeft_Click(object sender, RoutedEventArgs e)
        {
            leftMediaPlayerControl.TestMoveLeft();
        }


        
        private void Button_Resize_Click(object sender, RoutedEventArgs e)
        {
            
            int videoHeight = int.Parse(this.VideoHeight.Text);
            leftMediaPlayerControl.ChangeVideoHeight(videoHeight);
            
        }

        //コントロールの位置変更（RenderTransform）
        private void Button_Move_Click(object sender, RoutedEventArgs e)
        {
            var tfg = new TransformGroup();
            int posX = int.Parse(this.PosX.Text);
            int posY = int.Parse(this.PosY.Text);

            tfg.Children.Add(new TranslateTransform(posX, posY));

            leftMediaPlayerControl.Media.RenderTransform = tfg;
            /*
            int posX = int.Parse(this.PosX.Text);
            int posY = int.Parse(this.PosY.Text);
            leftMediaPlayerControl.ChangeVideoPosition(posX, posY);
            */
        }
    }
}