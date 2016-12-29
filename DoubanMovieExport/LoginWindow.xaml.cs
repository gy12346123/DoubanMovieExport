using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoubanMovieExport
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        public string Cookies { get; set; }

        public List<string> CookieList;

        private string URL;

        private bool forOnce = true;

        public string CookieLocalPath { get; set; }


        public LoginWindow(string address)
        {
            URL = address;
            CookieList = new List<string>();
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CookieLocalPath != null && !CookieLocalPath.Equals(""))
            {
                LoadCookies(CookieLocalPath);
            }
            webBrowser.Address = URL;
            // Show browser
            webBrowser.Visibility = Visibility.Visible;
            // Browser frame load end event
            webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
        }

        private async void WebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            // Get html source
            var result = await webBrowser.GetSourceAsync();
            if (result != null)
            {
                this.Dispatcher.Invoke(() => {
                    string address = webBrowser.Address;
                    if (address.Equals("https://www.douban.com/"))
                    {
                        if (forOnce)
                        {
                            forOnce = false;
                            // Login succeed, split url and parameters
                            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
                            CookieVisitor visitor = new CookieVisitor();
                            visitor.SendCookie += visitor_SendCookie;
                            cookieManager.VisitAllCookies(visitor);
                            this.DialogResult = true;
                        }
                    }
                });
            }
        }

        public static Task SaveCookies(List<string> cookieList, FileInfo file)
        {
            return Task.Factory.StartNew(()=> {
                if (!Directory.Exists(file.DirectoryName))
                {
                    Directory.CreateDirectory(file.DirectoryName);
                }
                using (StreamWriter SW = new StreamWriter(new FileStream(file.FullName, FileMode.Create)))
                {
                    foreach (string cookie in cookieList)
                    {
                        SW.WriteLine(cookie);
                    }
                }
            });
        }

        private async void LoadCookies(string file)
        {
            CookieList.Clear();
            using (StreamReader SR = new StreamReader(new FileStream(file, FileMode.Open)))
            {
                while (!SR.EndOfStream)
                {
                    CookieList.Add(SR.ReadLine());
                }
            }
            var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
            foreach (string cookie in CookieList)
            {
                string[] param = cookie.Split('$');
                await cookieManager.SetCookieAsync("https://www.douban.com", new CefSharp.Cookie
                {
                    Domain = param[0],
                    Name = param[1],
                    Value = param[2],
                    Expires = DateTime.MaxValue
                });
            }
        }

        private void visitor_SendCookie(CefSharp.Cookie obj)
        {
            CookieList.Add(string.Format("{0}${1}${2}${3}", obj.Domain, obj.Name, obj.Value, obj.Expires));
        }
    }

    public class CookieVisitor : CefSharp.ICookieVisitor
    {
        public event Action<CefSharp.Cookie> SendCookie;

        public void Dispose()
        {
        }

        public bool Visit(CefSharp.Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            deleteCookie = false;
            if (SendCookie != null)
            {
                SendCookie(cookie);
            }

            return true;
        }
    }
}
