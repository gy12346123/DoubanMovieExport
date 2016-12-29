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

        public static UIData uiData;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            uiData = new UIData();
            grid_Main.DataContext = uiData;
        }

        private async void button_Login_Click(object sender, RoutedEventArgs e)
        {
            try
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
            }catch (Exception)
            {
                uiData.Exception++;
            }
        }

        private async void button_Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int endNum = await Export.GetEndItemNumAsync("https://movie.douban.com/mine?status=collect");
                if (endNum < 1)
                {
                    await this.ShowMessageAsync("Failed", "Get the number end of page error.");
                    return;
                }
                uiData.TotalPage = endNum / 15;
                Sqlite.FirstCreate();

                for (int i = 0; i <= endNum; i += 15)
                {
                    var items = await Export.GetMovieAsync(string.Format("https://movie.douban.com/mine?status=collect&start={0}&sort=time&rating=all&filter=all&mode=grid", i));
                    if (i != 0)
                    {
                        uiData.LoadPage = i / 15 + 1;
                    }
                    else
                    {
                        uiData.LoadPage = 1;
                    }

                    foreach (var movie in items)
                    {
                        if (await Sqlite.Insert(movie.Title, movie.SubTitle, movie.URL, movie.Image, movie.Intro, movie.Tags, movie.Date, movie.Comment))
                        {
                            uiData.SaveItem++;
                        }
                    }
                }
            }catch (Exception)
            {
                uiData.Exception++;
            }
        }
    }
}
