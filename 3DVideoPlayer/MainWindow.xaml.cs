using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using _3DVideoPlayer;
using MessageBox = System.Windows.MessageBox;
using Size = System.Windows.Size;

namespace _3DVideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { //define
        const int LEFT_SCREEN_DEFAULT = 0;
        const int RIGHT_SCREEN_DEFAULT = 1;

        //外部のstaticインスタンスを保持
        DisplaysManager dm;

        // 左右のスクリーンのウィンドウ参照（クラスフィールドとして保持）
        private Window? leftWindow,rightWindow;


        //左右のプレイヤーのウィンドウとコントロール参照
        Window leftPlayer,rightPlayer;

        WPF_MediaPlayer.Controls.MediaPlayer leftMediaPlayerControl,rightMediaPlayerControl;
        // フィードバックを防ぐためのフラグ
        private bool isLeftSeekDragging = false;
        private bool isRightSeekDragging = false;

        //移動量
        private int Left_PosX = 0;
        private int Left_PosY = 0;
        private int Right_PosX = 0;
        private int Right_PosY = 0;


        //左右のスクリーンのインスタンス
        public MainWindow()
        {
            InitializeComponent();

            // DisplayManagerインスタンスを取得して保持
            dm = DisplaysManager.Instance;

            const int MONITOR1 = 0;
            const int MONITOR2 = 1;

            dm.ShowDisplaysInfo();
        }

        //再生機能

        private void Button_CreatePlayerWindow_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void LeftMediaPlayerControl_DurationChanged(object sender, System.TimeSpan duration)
        {
            // Media の長さがわかったら MainWindow のスライダー最大値を設定
            Dispatcher.Invoke(() =>
            {
                LeftSeekSlider.Maximum = duration.TotalMilliseconds;
            });
        }

        private void LeftMediaPlayerControl_PositionChanged(object sender, System.TimeSpan position)
        {
            // Media 再生位置が更新されたら MainWindow のスライダーを同期（ユーザーがドラッグ中は更新しない）
            if (isLeftSeekDragging) return;
            Dispatcher.Invoke(() =>
            {
                LeftSeekSlider.Value = position.TotalMilliseconds;
            });//イベントは UI スレッド以外から来る可能性があるため、UI 要素の更新を UI スレッドで行う。
        }

        //MediaPlayerの操作関連(MediaPlayer.csからコピー)
        private void LeftSeekSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isLeftSeekDragging = true;

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
        private void LeftSeekSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (leftMediaPlayerControl != null)
            {
                if (leftMediaPlayerControl.IsPausedPublic)
                {
                    leftMediaPlayerControl.Play(TimeSpan.FromMilliseconds(LeftSeekSlider.Value));
                }
                leftMediaPlayerControl.SeekSlider.IsMoveToPointEnabled = false;
            }

            isLeftSeekDragging = false;
        }


        private void LeftSeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // ユーザーがメインウィンドウのスライダーを操作している場合、メディアプレイヤー側にシークを伝える
            if (isLeftSeekDragging && leftMediaPlayerControl != null)
            {
                leftMediaPlayerControl.Seek(TimeSpan.FromMilliseconds(LeftSeekSlider.Value));
            }
        }

        private void LeftPlayButton_Click(object sender, RoutedEventArgs e)
        {
            leftMediaPlayerControl?.Play();
        }

        private void LeftPauseButton_Click(object sender, RoutedEventArgs e)
        {
            leftMediaPlayerControl?.Pause();
        }

        private void LeftStopButton_Click(object sender, RoutedEventArgs e)
        {
            leftMediaPlayerControl?.Stop();
        }


        private void LeftMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            leftMediaPlayerControl.TestMoveLeft();
        }



        private void LeftButton_Resize_Click(object sender, RoutedEventArgs e)
        {

            int videoHeight = int.Parse(this.LeftVideoHeight.Text);
            leftMediaPlayerControl.ChangeVideoHeight(videoHeight);

        }

        //コントロールの位置変更（RenderTransform）
        private void LeftButton_Move_Click(object sender, RoutedEventArgs e)
        {
            var tfg = new TransformGroup();
            Left_PosX = int.Parse(this.LeftPosX.Text);
            Left_PosY = int.Parse(this.LeftPosY.Text);

            tfg.Children.Add(new TranslateTransform(Left_PosX, Left_PosY));

            leftMediaPlayerControl.Media.RenderTransform = tfg;

            this.LeftVideoPosNow.Text = "X:" + Left_PosX.ToString() + " Y:" + Left_PosY.ToString();//表示を更新
        }

        // Right-side handlers
        private void RightMediaPlayerControl_DurationChanged(object sender, System.TimeSpan duration)
        {
            Dispatcher.Invoke(() =>
            {
                RightSeekSlider.Maximum = duration.TotalMilliseconds;
            });
        }

        private void RightMediaPlayerControl_PositionChanged(object sender, System.TimeSpan position)
        {
            if (isRightSeekDragging) return;
            Dispatcher.Invoke(() =>
            {
                RightSeekSlider.Value = position.TotalMilliseconds;
            });
        }

        private void RightSeekSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isRightSeekDragging = true;

            if (rightMediaPlayerControl != null)
            {
                rightMediaPlayerControl.SeekSlider.IsMoveToPointEnabled = true;
                if (rightMediaPlayerControl.IsPlayingPublic)
                {
                    rightMediaPlayerControl.Pause();
                }
                if (rightMediaPlayerControl.IsStoppedPublic)
                {
                    rightMediaPlayerControl.Play(TimeSpan.FromMilliseconds(rightMediaPlayerControl.SeekSlider.Value));
                    rightMediaPlayerControl.Pause();
                }
            }
        }

        private void RightSeekSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (rightMediaPlayerControl != null)
            {
                if (rightMediaPlayerControl.IsPausedPublic)
                {
                    rightMediaPlayerControl.Play(TimeSpan.FromMilliseconds(RightSeekSlider.Value));
                }
                rightMediaPlayerControl.SeekSlider.IsMoveToPointEnabled = false;
            }

            isRightSeekDragging = false;
        }

        private void RightSeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isRightSeekDragging && rightMediaPlayerControl != null)
            {
                rightMediaPlayerControl.Seek(TimeSpan.FromMilliseconds(RightSeekSlider.Value));
            }
        }

        private void RightPlayButton_Click(object sender, RoutedEventArgs e)
        {
            rightMediaPlayerControl?.Play();
        }

        private void RightPauseButton_Click(object sender, RoutedEventArgs e)
        {
            rightMediaPlayerControl?.Pause();
        }

        private void RightStopButton_Click(object sender, RoutedEventArgs e)
        {
            rightMediaPlayerControl?.Stop();
        }

        private void RightButton_Resize_Click(object sender, RoutedEventArgs e)
        {
            int videoHeight = int.Parse(this.RightVideoHeight.Text);
            rightMediaPlayerControl.ChangeVideoHeight(videoHeight);
        }

        private void RightButton_Move_Click(object sender, RoutedEventArgs e)
        {
            var tfg = new TransformGroup();
            Right_PosX = int.Parse(this.RightPosX.Text);
            Right_PosY = int.Parse(this.RightPosY.Text);

            tfg.Children.Add(new TranslateTransform(Right_PosX, Right_PosY));

            rightMediaPlayerControl.Media.RenderTransform = tfg;

            this.RightVideoPosNow.Text = "X:" + Right_PosX.ToString() + " Y:" + Right_PosY.ToString();//表示を更新
        }

        //テスト用 ボタン押したらログ出る
        private void LogButtonPressed(object sender, RoutedEventArgs e)
        {
            dm.InitializeDisplays(int.Parse(NumLeftDisplay.Text), int.Parse(NumRightDisplay.Text));
        }

        public void Start()
        {
            LogDisplay.Clear();
            App.WriteLog("プログラムが起動しました。");
            dm.ShowDisplaysInfo();
            dm.InitializeDisplays(LEFT_SCREEN_DEFAULT, RIGHT_SCREEN_DEFAULT);

        }


        //ログを書き込む(App.xaml.csの静的メソッドから呼ばれる)
        public void AppendLog(string str)
        {
            LogDisplay.AppendText("[" + System.DateTime.Now.ToString("hh:mm:ss.ff") + "]" + str + "\n");
            // スクロールを一番下に移動
            LogDisplay.ScrollToEnd();
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            /* 仮
            var bounds = dm.LeftScreen.Bounds;
            leftWindow = new Window
            {
                Background = new SolidColorBrush(Colors.Black),
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.None,
                //Topmost = true,
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height
            };
            leftWindow.Show();
            leftWindow.WindowState = WindowState.Maximized;
            */

            // UserControl を Window にラップして表示する
            leftMediaPlayerControl = new WPF_MediaPlayer.Controls.MediaPlayer();

            // イベント購読（シークバーの同期用）
            leftMediaPlayerControl.PositionChanged += LeftMediaPlayerControl_PositionChanged;
            leftMediaPlayerControl.DurationChanged += LeftMediaPlayerControl_DurationChanged;

            // ここで動画サイズイベントを購読
            leftMediaPlayerControl.VideoSizeChanged += LeftMediaPlayerControl_VideoSizeChanged;

            var bounds = dm.LeftScreen.Bounds;

            leftPlayer = new Window
            {
                Title = "左",
                Content = leftMediaPlayerControl,
                Background = new SolidColorBrush(Colors.Black),
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.None,
                //Topmost = true,
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height
            };
            leftPlayer.Show();
            leftPlayer.WindowState = WindowState.Maximized;

            App.WriteLog("左画面のメディアプレイヤーウィンドウを作成しました。");
            App.WriteLog("動画サイズ: " + leftMediaPlayerControl.Media.ActualWidth + "x" + leftMediaPlayerControl.Media.ActualHeight);

        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            // Create right media player window similar to left
            rightMediaPlayerControl = new WPF_MediaPlayer.Controls.MediaPlayer();

            // subscribe events
            rightMediaPlayerControl.PositionChanged += RightMediaPlayerControl_PositionChanged;
            rightMediaPlayerControl.DurationChanged += RightMediaPlayerControl_DurationChanged;
            rightMediaPlayerControl.VideoSizeChanged += RightMediaPlayerControl_VideoSizeChanged;

            var bounds = dm.RightScreen.Bounds;
            rightPlayer = new Window
            {
                Title = "右",
                Content = rightMediaPlayerControl,
                Background = new SolidColorBrush(Colors.Black),
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.None,
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height
            };
            rightPlayer.Show();
            rightPlayer.WindowState = WindowState.Maximized;

            App.WriteLog("右画面のメディアプレイヤーウィンドウを作成しました。");
            App.WriteLog("動画サイズ: " + rightMediaPlayerControl.Media.ActualWidth + "x" + rightMediaPlayerControl.Media.ActualHeight);

        }

        private void NumRightDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("設定を保存しますか？",
                    "終了",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MessageBox.Show("TODO:設定の保存処理", "保存処理が実装されていません", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LeftMediaPlayerControl_VideoSizeChanged(object sender, Size newSize)
        {
            // UI スレッドで TextBlock を更新
            Dispatcher.Invoke(() =>
            {
                LeftVideoWidthNow.Text = ((int)newSize.Width).ToString();
                LeftVideoHeightNow.Text = ((int)newSize.Height).ToString();
            });
        }

        private void LeftDragging(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void LeftDrop(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
                {
                    return;
                }

                var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, true);
                if (files == null || files.Length == 0) return;

                var filePath = files[0];
                if (string.IsNullOrEmpty(filePath)) return;

                // 左側のメディアプレイヤーにファイルをセット
                if (leftMediaPlayerControl != null)
                {
                    try
                    {
                        var uri = new Uri(filePath);
                        // MediaElement と MediaPlayer の Source を更新
                        leftMediaPlayerControl.Media.Source = uri;
                        //leftMediaPlayerControl.Source = uri; // dependency property for timeline binding
                        leftMediaPlayerControl.Source = new Uri(filePath, UriKind.Absolute);
                        

                        App.WriteLog("LeftDrop: loaded file " + filePath);

                        // 自動再生
                        // leftMediaPlayerControl.Play();
                    }
                    catch (Exception ex)
                    {
                        App.WriteLog("LeftDrop: failed to set source - " + ex.Message);
                    }
                }
            }
            finally
            {
                e.Handled = true;
            }
        }

        // Right drag handlers
        private void RightDragging(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void RightDrop(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
                {
                    return;
                }

                var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, true);
                if (files == null || files.Length == 0) return;

                var filePath = files[0];
                if (string.IsNullOrEmpty(filePath)) return;

                // 右側のメディアプレイヤーにファイルをセット
                if (rightMediaPlayerControl != null)
                {
                    try
                    {
                        var uri = new Uri(filePath);
                        rightMediaPlayerControl.Media.Source = uri;
                        rightMediaPlayerControl.Source = new Uri(filePath, UriKind.Absolute);
                        //rightMediaPlayerControl.Play();

                        App.WriteLog("RightDrop: loaded file " + filePath);
                    }
                    catch (Exception ex)
                    {
                        App.WriteLog("RightDrop: failed to set source - " + ex.Message);
                    }
                }
            }
            finally
            {
                e.Handled = true;
            }
        }

        private void RightMediaPlayerControl_VideoSizeChanged(object sender, Size newSize)
        {
            Dispatcher.Invoke(() =>
            {
                RightVideoWidthNow.Text = ((int)newSize.Width).ToString();
                RightVideoHeightNow.Text = ((int)newSize.Height).ToString();
            });
        }

        private void ButtonAllPlay_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    if (leftMediaPlayerControl != null)
                    {
                        leftMediaPlayerControl.Play();
                    }
                }
                catch (Exception ex)
                {
                    App.WriteLog("ButtonAllPlay: left play failed - " + ex.Message);
                }

                try
                {
                    if (rightMediaPlayerControl != null)
                    {
                        rightMediaPlayerControl.Play();
                    }
                }
                catch (Exception ex)
                {
                    App.WriteLog("ButtonAllPlay: right play failed - " + ex.Message);
                }
            });
        }
    }
}
