using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoubanMovieExport
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static readonly string BasePath = System.AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string Douban_CookiePath = BasePath + @"Cookie\Douban.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void button_Login_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(BasePath + @"Cookie"))
            {
                Directory.CreateDirectory(BasePath + @"Cookie");
            }
            FileInfo file = new FileInfo(Douban_CookiePath);

            LoginWindow LW = new LoginWindow("https://accounts.douban.com/login");
            //LoginWindow LW = new LoginWindow("https://movie.douban.com/");
            LW.Owner = this;
            LW.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //LW.CookieLocalPath = Baidu_CookiePath;
            if ((bool)LW.ShowDialog())
            {
                List<string> cookieList = LW.CookieList;
                if (cookieList != null && cookieList.Count() > 0)
                {
                    // Save cookies
                    await LoginWindow.SaveCookies(cookieList, file);
                    await this.ShowMessageAsync("Succeed", "Login succeed!");
                    return;
                }
            }
        }
    }
}
