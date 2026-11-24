using System;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace EmployeeDirectory
{
    public static class FileCache
    {
        private static string cacheDirectory;
        private static JeffWilcox.Utilities.Silverlight.MD5 md5;
        private static object locker = new object();
        private static Dictionary<string, Task<bool>> downloadTasks = new Dictionary<string, Task<bool>>();

        static FileCache()
        {
            Init();
        }

        static void Init()
        {
            md5 = JeffWilcox.Utilities.Silverlight.MD5.Create("MD5");
            cacheDirectory = Path.Combine(FileSystem.CacheDirectory, "Cache");
            Directory.CreateDirectory(cacheDirectory);
        }

        public static async Task<string> Download(string url)
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(url.Trim()));
            var fileName = string.Join("", hash.Select(x => x.ToString("x2")));

            return await Download(url, fileName);
        }

        public static async Task<string> Download(Uri url, string email)
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(email.Trim()));
            var fileName = string.Join("", hash.Select(x => x.ToString("x2")));

            return await Download(url.AbsoluteUri, fileName);
        }

        public static async Task<string> Download(string url, string fileName)
        {
            try
            {
                var path = Path.Combine(cacheDirectory, fileName);
                if (File.Exists(path) && !downloadTasks.ContainsKey(path))
                {
                    return path;
                }

                var success = await GetDownload(url, path);
                return success ? path : "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "";
            }
        }

        private static Task<bool> GetDownload(string url, string fileName)
        {
            lock (locker)
            {
                Task<bool> task;
                if (downloadTasks.TryGetValue(fileName, out task))
                    return task;

                downloadTasks.Add(fileName, task = download(url, fileName));
                return task;
            }
        }

        private static async Task<bool> download(string url, string fileName)
        {
            try
            {
                var client = new HttpClient();
                var data = await client.GetByteArrayAsync(url);
                var fileNamePaths = fileName.Split('\\');
                fileName = fileNamePaths[fileNamePaths.Length - 1];
                var filePath = Path.Combine(cacheDirectory, fileName);
                
                await File.WriteAllBytesAsync(filePath, data);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
            // Clean up file if it exists and there was an error
            var cleanupPath = Path.Combine(cacheDirectory, fileName);
            if (File.Exists(cleanupPath))
            {
                try
                {
                    File.Delete(cleanupPath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            
            return false;
        }
    }
}

