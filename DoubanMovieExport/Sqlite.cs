using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubanMovieExport
{
    public class Sqlite
    {
        private static string DataBase_Movie = "Data Source =" + System.AppDomain.CurrentDomain.BaseDirectory + @"DB\Movie.db";
        private static string Path_Movie = System.AppDomain.CurrentDomain.BaseDirectory + @"DB\Movie.db";

        public static async void FirstCreate()
        {
            try
            {
                if (!File.Exists(Path_Movie))
                {
                    FileInfo info = new FileInfo(Path_Movie);
                    if (!Directory.Exists(info.DirectoryName))
                    {
                        Directory.CreateDirectory(info.DirectoryName);
                    }
                    using (SQLiteConnection sqlite = new SQLiteConnection(DataBase_Movie))
                    {
                        sqlite.Open();
                        string executeSQL = "CREATE TABLE IF NOT EXISTS `movielist` (`id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,`title` TEXT NOT NULL,`subtitle` TEXT,`URL` TEXT NOT NULL,`image` TEXT,`intro` TEXT,`tags` TEXT,`date` TEXT,`comment` TEXT);";
                        using (SQLiteCommand command = new SQLiteCommand(executeSQL, sqlite))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
            }catch(Exception)
            {

            }

        }

        public static Task<bool> Insert(string title, string subtitle, string URL, string image, string intro, string tags, string date, string comment)
        {
            return Task.Factory.StartNew(()=> {
                try
                {
                    using (SQLiteConnection sqlite = new SQLiteConnection(DataBase_Movie))
                    {
                        sqlite.Open();
                        string executeSQL = "INSERT INTO `movielist` (`id`, `title`, `subtitle`, `URL`, `image`, `intro`, `tags`, `date`, `comment`) VALUES (NULL, '" + title + "', '" + subtitle + "', '" + URL + "', '" + image + "', '" + intro + "', '" + tags + "', '" + date + "', '" + comment + "');";
                        using (SQLiteCommand command = new SQLiteCommand(executeSQL, sqlite))
                        {
                            command.ExecuteNonQuery();
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}
