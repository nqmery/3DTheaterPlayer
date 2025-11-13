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

namespace TestMultiDisplay
{
    internal class DisplaysManager
    {
        // シングルトン的に使う静的インスタンス
        public static DisplaysManager Instance { get; } = new DisplaysManager();

        // 外部から new できないようにする（必要なら internal にしても可）
        private DisplaysManager() { }

        //テスト用 ディスプレイの情報をログに表示
        public void ShowDisplaysInfo()
        {
            uint displayCount = (uint)System.Windows.Forms.Screen.AllScreens.Length;
            App.WriteLog($"ディスプレイの数:{displayCount}");
            foreach (System.Windows.Forms.Screen s in System.Windows.Forms.Screen.AllScreens)
            {
                //ディスプレイの変数の番号を表示
                App.WriteLog($"デバイス番号:{Array.IndexOf(System.Windows.Forms.Screen.AllScreens, s)}");
                //ディスプレイのデバイス名を表示
                App.WriteLog($"デバイス名:{s.DeviceName}");
                //ディスプレイの左上の座標を表示
                App.WriteLog($"X:{s.Bounds.X} Y:{s.Bounds.Y}");
                //ディスプレイの大きさを表示
                App.WriteLog($"高さ:{s.Bounds.Height} 幅:{s.Bounds.Width}");
            }

            /*
            System.Windows.Forms.Screen s = System.Windows.Forms.Screen.AllScreens[0];
            //ディスプレイの変数の番号を表示
            App.WriteLog($"デバイス番号:{Array.IndexOf(System.Windows.Forms.Screen.AllScreens, s)}");
            //ディスプレイのデバイス名を表示
            App.WriteLog($"デバイス名:{s.DeviceName}");
            //ディスプレイの左上の座標を表示
            App.WriteLog($"X:{s.Bounds.X} Y:{s.Bounds.Y}");
            //ディスプレイの大きさを表示
            App.WriteLog($"高さ:{s.Bounds.Height} 幅:{s.Bounds.Width}");
            */
        }
    }
}
