//
//  Copyright 2012, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Maui.Storage;

namespace EmployeeDirectory.Utilities
{
    public abstract class ImageDownloader
    {
        readonly string cacheDirectory;
        readonly HttpClient httpClient;
        readonly TimeSpan cacheDuration;
        readonly SemaphoreSlim semaphore;

        public ImageDownloader(int maxConcurrentDownloads = 2) : this(TimeSpan.FromDays(1), maxConcurrentDownloads)
        {
        }

        public ImageDownloader(TimeSpan cacheDuration, int maxConcurrentDownloads = 2)
        {
            this.cacheDuration = cacheDuration;
            this.semaphore = new SemaphoreSlim(maxConcurrentDownloads, maxConcurrentDownloads);
            this.httpClient = new HttpClient();
            
            cacheDirectory = Path.Combine(FileSystem.CacheDirectory, "ImageCache");
        }

        public bool HasLocallyCachedCopy(Uri uri)
        {
            var now = DateTime.UtcNow;
            var filename = Uri.EscapeDataString(uri.AbsoluteUri);
            var lastWriteTime = GetLastWriteTimeUtc(filename);

            return lastWriteTime.HasValue &&
                   (now - lastWriteTime.Value) < cacheDuration;
        }

        public async Task<object> GetImageAsync(Uri uri)
        {
            Directory.CreateDirectory(cacheDirectory);
            return await GetImage(uri);
        }

        public async Task<object> GetImage(Uri uri)
        {
            var filename = Uri.EscapeDataString(uri.AbsoluteUri);

            if (HasLocallyCachedCopy(uri))
            {
                using (var stream = await OpenStorage(filename, FileAccess.Read))
                {
                    return LoadImage(stream);
                }
            }
            else
            {
                await semaphore.WaitAsync();
                try
                {
                    using (var downloadStream = await httpClient.GetStreamAsync(uri))
                    using (var fileStream = await OpenStorage(filename, FileAccess.Write))
                    {
                        await downloadStream.CopyToAsync(fileStream);
                    }
                }
                finally
                {
                    semaphore.Release();
                }

                using (var stream = await OpenStorage(filename, FileAccess.Read))
                {
                    return LoadImage(stream);
                }
            }
        }

        protected virtual DateTime? GetLastWriteTimeUtc(string fileName)
        {
            var filePath = Path.Combine(cacheDirectory, fileName);
            if (File.Exists(filePath))
            {
                return File.GetLastWriteTimeUtc(filePath);
            }
            return null;
        }

        protected virtual async Task<Stream> OpenStorage(string fileName, FileAccess mode)
        {
            var filePath = Path.Combine(cacheDirectory, fileName);
            
            return mode switch
            {
                FileAccess.Read => File.OpenRead(filePath),
                FileAccess.Write => File.OpenWrite(filePath),
                _ => File.Open(filePath, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite)
            };
        }

        protected abstract object LoadImage(Stream stream);

        public void Dispose()
        {
            httpClient?.Dispose();
            semaphore?.Dispose();
        }
    }
}

