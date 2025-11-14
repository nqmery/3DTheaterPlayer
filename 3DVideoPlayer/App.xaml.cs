using System.Configuration;
using System.Data;
using System.Windows;

using Application = System.Windows.Application;//WPFとWinForms間の参照のバッティングを防ぐ

namespace _3DVideoPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow mainWindow;
        //起動前処理
        //https://zenn.dev/chai0917/articles/4101d69a00fbc2#%E3%82%B9%E3%82%BF%E3%83%BC%E3%83%88%E3%82%A2%E3%83%83%E3%83%97%E3%83%AB%E3%83%BC%E3%83%81%E3%83%B3
        private void App_Startup(object sender, StartupEventArgs e)
        {
            // メインウィンドウ
            mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.Start();//ウィンドウ表示後に開始処理を呼ぶ
        }

        //終了時処理
        private void App_Exit(object sender, ExitEventArgs e)
        {

        }

        // ログを書き込む（静的メソッド）
        public static void WriteLog(string text)
        {
            var mw = Application.Current?.MainWindow as MainWindow;
            if (mw == null) return;
            // UI スレッドで実行
            mw.Dispatcher.Invoke(() => mw.AppendLog(text));
        }
    }

}
