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
        public MainWindow()
        {
            InitializeComponent();
            
            var bounds = Screen.Bounds;
            Window LeftWindow = new Window
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
            LeftWindow.Show();
            LeftWindow.WindowState = WindowState.Maximized;
        }
    }
}