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
using System.Windows.Forms;
using System.Drawing;


namespace TestMultiDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        //外部のstaticインスタンスを保持
        DisplaysManager dm;


        public MainWindow()
        {
            InitializeComponent();

            // DisplayManagerインスタンスを取得して保持
            dm = DisplaysManager.Instance;

            const int MONITOR1 = 0;
            const int MONITOR2 = 1;

            dm.ShowDisplaysInfo();
            /*
            if (Screen.AllScreens.Length < 2)
            {
                App.WriteLog("Not Multi Monitors!");
            }
            else
            {

                
                Screen Screen1 = Screen.AllScreens[MONITOR1];
                Screen Screen2 = Screen.AllScreens[MONITOR2];
                
                var bounds1 = Screen1.Bounds;
                Window window2 = new Window
                {
                    Background = new SolidColorBrush(Colors.Red),
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    WindowState = WindowState.Normal,
                    WindowStyle = WindowStyle.None,
                    //Topmost = true,
                    Left = bounds1.Left,
                    Top = bounds1.Top,
                    Width = bounds1.Width,
                    Height = bounds1.Height
                };
                window2.Show();
                window2.WindowState = WindowState.Maximized;

                var bounds2 = Screen2.Bounds;
                Window window3 = new Window
                {
                    Background = new SolidColorBrush(Colors.Blue),
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    WindowState = WindowState.Normal,
                    WindowStyle = WindowStyle.None,
                    //Topmost = true,
                    Left = bounds2.Left,
                    Top = bounds2.Top,
                    Width = bounds2.Width,
                    Height = bounds2.Height
                };
                window3.Show();
                window3.WindowState = WindowState.Maximized;*/
        }
        
        

        //テスト用 ボタン押したらログ出る
        private void LogButtonPressed(object sender, RoutedEventArgs e)
        {
            dm.ShowDisplaysInfo();
        }

        public void Start()
        {
            LogDisplay.Clear();
            App.WriteLog("プログラムが起動しました。");
            dm.ShowDisplaysInfo();

        }


        //ログを書き込む(App.xaml.csの静的メソッドから呼ばれる)
        public void AppendLog(string str)
        {
            LogDisplay.AppendText("[" + System.DateTime.Now.ToString("hh:mm:ss.ff") + "]" + str + "\n");
            // スクロールを一番下に移動
            LogDisplay.ScrollToEnd();
        }
    }
    
    
    }
