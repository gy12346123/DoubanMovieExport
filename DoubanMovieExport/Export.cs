using DotNet4.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DoubanMovieExport.Struct;

namespace DoubanMovieExport
{
    public static class Export
    {
        /// <summary>
        /// Http time out
        /// </summary>
        private static int Timeout = 30000;

        /// <summary>
        /// Set http Get or Post timeout value
        /// </summary>
        /// <param name="time">Millisecond</param>
        public static void SetTimeout(int time)
        {
            Timeout = time;
        }

        /// <summary>
        /// Cookies
        /// </summary>
        private static string Cookies;

        /// <summary>
        /// Load cookies from local file
        /// </summary>
        /// <param name="path">Local path</param>
        /// <param name="overwrite">Read cookies from path forcibly</param>
        public static void LoadCookie(string path, bool overwrite = false)
        {
            if (Cookies == null || Cookies.Equals("") || overwrite)
            {
                using (StreamReader SR = new StreamReader(new FileStream(path, FileMode.Open)))
                {
                    StringBuilder SB = new StringBuilder();
                    while (!SR.EndOfStream)
                    {
                        string[] param = SR.ReadLine().Split('$');
                        SB.Append(string.Format("{0}={1}; ", param[1], param[2]));
                    }
                    Cookies = SB.ToString();
                }
            }
        }

        /// <summary>
        /// Async load cookies form local file
        /// </summary>
        /// <param name="path">Local path</param>
        /// <param name="overwrite">Read cookies from path forcibly</param>
        /// <returns></returns>
        public static Task LoadCookieAsync(string path, bool overwrite = false)
        {
            return Task.Factory.StartNew(() => {
                LoadCookie(path, overwrite);
            });
        }

        /// <summary>
        /// Check cookie
        /// </summary>
        /// <returns></returns>
        public static bool CheckCookie()
        {
            if (Cookies == null || Cookies.Equals(""))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check cookie file exist
        /// </summary>
        /// <param name="path">Local path</param>
        /// <returns></returns>
        public static bool IsCookieFileExist(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get movie item from url
        /// </summary>
        /// <param name="url">Moive page</param>
        /// <returns></returns>
        public static MovieItem[] GetMovie(string url)
        {
            if (!CheckCookie()) LoadCookie(MainWindow.Douban_CookiePath);
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,
                Encoding = Encoding.UTF8,
                Timeout = Export.Timeout,
                Referer = url,
                Host = "movie.douban.com",
                Cookie = Cookies
            };

            string result = http.GetHtml(item).Html;

            var matches = Regex.Matches(result, "<div class=\"item\" >[\\s\\S]*?</ul>");

            if (matches.Count > 0)
            {
                MovieItem[] movieItem = new MovieItem[matches.Count];
                int count = 0;
                foreach (Match match in matches)
                {
                    string movieItemStr = match.Value;
                    var imgMatch = Regex.Match(movieItemStr, "(?<=src=\").*?(?=\" class=\"\">)");
                    if (imgMatch.Success)
                    {
                        movieItem[count].Image = imgMatch.Value;
                    }

                    var URLMatch = Regex.Match(movieItemStr, "(?<=href=\").*?(?=\" class=\"nbg\">)");
                    if (URLMatch.Success)
                    {
                        movieItem[count].URL = URLMatch.Value;
                    }

                    var titleMatch = Regex.Match(movieItemStr, "(?<=<em>).*?(?=</em>)");
                    if (titleMatch.Success)
                    {
                        movieItem[count].Title = titleMatch.Value;
                    }

                    var subTitleMatch = Regex.Match(movieItemStr, "(?<=</em>)[\\s\\S]*?(?=</a>)");
                    if (subTitleMatch.Success)
                    {
                        movieItem[count].SubTitle = subTitleMatch.Value.Replace("\n                             ", "").Replace("\n                        ", "");
                    }

                    var introMatch = Regex.Match(movieItemStr, "(?<=<li class=\"intro\">).*?(?=</li>)");
                    if (introMatch.Success)
                    {
                        movieItem[count].Intro = introMatch.Value;
                    }

                    var dateMatch = Regex.Match(movieItemStr, "(?<=<span class=\"date\">).*?(?=</span>)");
                    if (dateMatch.Success)
                    {
                        movieItem[count].Date = dateMatch.Value;
                    }

                    var tagsMatch = Regex.Match(movieItemStr, "(?<=<span class=\"tags\">).*?(?=</span>)");
                    if (tagsMatch.Success)
                    {
                        movieItem[count].Tags = tagsMatch.Value.Replace("标签: ", "");
                    }

                    var commentMatch = Regex.Match(movieItemStr, "(?<=<span class=\"comment\">).*?(?=</span>)");
                    if (commentMatch.Success)
                    {
                        movieItem[count].Comment = commentMatch.Value;
                    }

                    count++;
                }
                return movieItem;
            }
            throw new Exception("No match..");
        }

        /// <summary>
        /// Get movie item from url
        /// </summary>
        /// <param name="url">Moive page</param>
        /// <returns></returns>
        public static Task<MovieItem[]> GetMovieAsync(string url)
        {
            return Task.Factory.StartNew(()=> {
                try
                {
                    return GetMovie(url);
                }catch (Exception)
                {
                    Task.Delay(5000);
                    return GetMovie(url);
                }
            });
        }

        /// <summary>
        /// Get total item count
        /// </summary>
        /// <param name="url">List page</param>
        /// <returns></returns>
        public static int GetEndItemNum(string url)
        {
            if (!CheckCookie()) LoadCookie(MainWindow.Douban_CookiePath);
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,
                Encoding = Encoding.UTF8,
                Timeout = Export.Timeout,
                Referer = url,
                Host = "movie.douban.com",
                Cookie = Cookies
            };

            string result = http.GetHtml(item).Html;

            var match = Regex.Match(result, "<span class=\"break\">[\\s\\S]*?<span class=\"next\">");
            if (match.Success)
            {
                var numMatches = Regex.Matches(match.Value, "(?<=start=)[0-9]{1,}");
                int[] num = new int[numMatches.Count];
                int count = 0;
                foreach(Match numMatch in numMatches)
                {
                    num[count] = Convert.ToInt32(numMatch.Value);
                    count++;
                }
                return num.Max();
            }
            return -1;
        }

        /// <summary>
        /// Get total item count async
        /// </summary>
        /// <param name="url">List page</param>
        /// <returns></returns>
        public static Task<int> GetEndItemNumAsync(string url)
        {
            return Task.Factory.StartNew(()=> {
                try
                {
                    return GetEndItemNum(url);
                }catch (Exception)
                {
                    Task.Delay(5000);
                    return GetEndItemNum(url);
                }
            });
        }
    }
}
