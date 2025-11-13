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
        public MainWindow()
        {
            InitializeComponent();
            
           
        }

        private void Button_CreatePlayerWindow_Click(object sender, RoutedEventArgs e)
        {
            // UserControl を Window にラップして表示する
            var mediaPlayerControl = new WPF_MediaPlayer.Controls.MediaPlayer();
            leftPlayer = new Window
            {
                Title = "左",
                Content = mediaPlayerControl,
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
        /*
private void CreatePlayerWindow(object sender, RoutedEventArgs e)
{
   leftPlayer = new Window
   {
       Background = new SolidColorBrush(Colors.Black),
       WindowStartupLocation = WindowStartupLocation.Manual,
       WindowState = WindowState.Normal,
       WindowStyle = WindowStyle.None,
       //Topmost = true,
       Left = 0,
       Top = 0,
       Width = 1280,
       Height = 720
   };
   leftPlayer.Show();
}*/
    }
}