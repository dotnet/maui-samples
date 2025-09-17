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
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;

namespace EmployeeDirectory.Data
{
    [XmlRoot("Favorites")]
    public class XmlFavoritesRepository : IFavoritesRepository
    {
        // NOTE: This XML-based favorites repository is retained for historical parity with the
        // Xamarin.Forms sample. In .NET 8/9+ we observed intermittent FileNotFoundException cases
        // when the runtime attempted to load the XmlSerializer generated assembly in trimmed /
        // linked builds (see: https://github.com/dotnet/runtime/issues/83152). The sample now
        // prefers a JSON-based persistence approach elsewhere to avoid reliance on XmlSerializer
        // code generation, improve reliability across platforms, and reduce size. This class is
        // kept to illustrate the original pattern but may be replaced in future versions.
        public string IsolatedStorageName { get; set; }

        public event EventHandler Changed;

        public List<Person> People { get; set; }

        public async static Task<XmlFavoritesRepository> OpenIsolatedStorage(string isolatedStorageName)
        {
            var serializer = new XmlSerializer(typeof(XmlFavoritesRepository));

            try
            {
                var filePath = Path.Combine(FileSystem.AppDataDirectory, isolatedStorageName);
                
                if (File.Exists(filePath))
                {
                    using var f = new StreamReader(filePath);
                    var repo = (XmlFavoritesRepository)serializer.Deserialize(f);
                    repo.IsolatedStorageName = isolatedStorageName;
                    return repo;
                }
                else
                {
                    return new XmlFavoritesRepository
                    {
                        IsolatedStorageName = isolatedStorageName,
                        People = new List<Person>()
                    };
                }
            }
            catch (Exception)
            {
                return new XmlFavoritesRepository
                {
                    IsolatedStorageName = isolatedStorageName,
                    People = new List<Person>()
                };
            }
        }

        public async static Task<XmlFavoritesRepository> OpenFile(string path)
        {
            var serializer = new XmlSerializer(typeof(XmlFavoritesRepository));

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(path);
                using var f = new StreamReader(stream);
                var repo = (XmlFavoritesRepository)serializer.Deserialize(f);
                repo.IsolatedStorageName = Path.GetFileName(path);
                return repo;
            }
            catch (Exception)
            {
                return new XmlFavoritesRepository
                {
                    IsolatedStorageName = Path.GetFileName(path),
                    People = new List<Person>()
                };
            }
        }

        private async Task Commit()
        {
            var serializer = new XmlSerializer(typeof(XmlFavoritesRepository));

            try
            {
                var filePath = Path.Combine(FileSystem.AppDataDirectory, IsolatedStorageName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                
                using var f = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(f, this);
            }
            catch (Exception)
            {
                // Ignore serialization errors
            }

            var ev = Changed;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        #region IFavoritesRepository implementation

        public IEnumerable<Person> GetAll()
        {
            return People;
        }

        public Person FindById(string id)
        {
            return People.FirstOrDefault(x => x.Id == id);
        }

        public bool IsFavorite(Person person)
        {
            return People.Any(x => x.Id == person.Id);
        }

        public void InsertOrUpdate(Person person)
        {
            var existing = People.FirstOrDefault(x => x.Id == person.Id);
            if (existing != null)
                People.Remove(existing);

            People.Add(person);
            var task = Task.Run(async () =>
            {
                await Commit();
            });
            task.Wait();
        }

        public void Delete(Person person)
        {
            var newPeopleQ = from p in People
                             where p.Id != person.Id
                             select p;
            var newPeople = newPeopleQ.ToList();
            var n = People.Count - newPeople.Count;
            People = newPeople;
            if (n != 0)
            {
                var task = Task.Run(async () =>
                {
                    await Commit();
                });
                task.Wait();
            }
        }

        #endregion
    }
}

