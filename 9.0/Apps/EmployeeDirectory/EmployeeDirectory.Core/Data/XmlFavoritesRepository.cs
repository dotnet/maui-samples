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
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text;

namespace EmployeeDirectory.Core.Data;

[XmlRoot("Favorites")]
public class XmlFavoritesRepository : IFavoritesRepository
{
    private static readonly object _lockObject = new object();
    private volatile bool _isCommitting = false;

    public string IsolatedStorageName { get; set; }

    public event EventHandler Changed;

    public List<Person> People { get; set; } = new List<Person>(); // Always initialize

    public XmlFavoritesRepository()
    {
        People = new List<Person>();
        DebugLog("XmlFavoritesRepository constructor called");
    }

    #region Debug Methods

    private static void DebugLog(string message)
    {
        Debug.WriteLine($"[XmlFavoritesRepository] {DateTime.Now:HH:mm:ss.fff} - {message}");
    }

    private static void DebugLogException(string operation, Exception ex)
    {
        Debug.WriteLine($"[XmlFavoritesRepository] {DateTime.Now:HH:mm:ss.fff} - ERROR in {operation}:");
        Debug.WriteLine($"[XmlFavoritesRepository] Exception Type: {ex.GetType().Name}");
        Debug.WriteLine($"[XmlFavoritesRepository] Message: {ex.Message}");
        Debug.WriteLine($"[XmlFavoritesRepository] Stack Trace: {ex.StackTrace}");
        
        if (ex.InnerException != null)
        {
            Debug.WriteLine($"[XmlFavoritesRepository] Inner Exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
        }
    }

    private static void DebugLogFileInfo(string filePath, string operation)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                Debug.WriteLine($"[XmlFavoritesRepository] {operation} - File exists: {filePath}");
                Debug.WriteLine($"[XmlFavoritesRepository] File size: {fileInfo.Length} bytes");
                Debug.WriteLine($"[XmlFavoritesRepository] Last modified: {fileInfo.LastWriteTime}");
            }
            else
            {
                Debug.WriteLine($"[XmlFavoritesRepository] {operation} - File does not exist: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[XmlFavoritesRepository] Error checking file info: {ex.Message}");
        }
    }

    private static void DebugLogDirectoryInfo(string directoryPath)
    {
        try
        {
            Debug.WriteLine($"[XmlFavoritesRepository] AppDataDirectory: {FileSystem.AppDataDirectory}");
            Debug.WriteLine($"[XmlFavoritesRepository] Target directory: {directoryPath}");
            
            if (Directory.Exists(directoryPath))
            {
                Debug.WriteLine($"[XmlFavoritesRepository] Directory exists: {directoryPath}");
                var files = Directory.GetFiles(directoryPath);
                Debug.WriteLine($"[XmlFavoritesRepository] Files in directory: {files.Length}");
                foreach (var file in files)
                {
                    Debug.WriteLine($"[XmlFavoritesRepository] - {Path.GetFileName(file)}");
                }
            }
            else
            {
                Debug.WriteLine($"[XmlFavoritesRepository] Directory does not exist: {directoryPath}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[XmlFavoritesRepository] Error checking directory info: {ex.Message}");
        }
    }

    private void DebugLogRepositoryState(string operation)
    {
        Debug.WriteLine($"[XmlFavoritesRepository] {operation} - Repository state:");
        Debug.WriteLine($"[XmlFavoritesRepository] IsolatedStorageName: {IsolatedStorageName ?? "NULL"}");
        Debug.WriteLine($"[XmlFavoritesRepository] People count: {People?.Count ?? -1}");
        
        if (People != null && People.Count > 0)
        {
            Debug.WriteLine($"[XmlFavoritesRepository] First person: {People[0]?.Name ?? "NULL"} (ID: {People[0]?.Id ?? "NULL"})");
        }
    }

    private static void DebugLogXmlContent(string xmlContent, string operation)
    {
        Debug.WriteLine($"[XmlFavoritesRepository] {operation} - XML Content (first 500 chars):");
        var preview = xmlContent.Length > 500 ? xmlContent.Substring(0, 500) + "..." : xmlContent;
        Debug.WriteLine($"[XmlFavoritesRepository] {preview}");
    }

    #endregion

    public async static Task<XmlFavoritesRepository> OpenIsolatedStorage(string isolatedStorageName)
    {
        DebugLog($"OpenIsolatedStorage called with: {isolatedStorageName}");
        
        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, isolatedStorageName);
            DebugLogFileInfo(filePath, "OpenIsolatedStorage");
            
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                DebugLogDirectoryInfo(directoryPath);
            }
            
            if (File.Exists(filePath))
            {
                DebugLog("File exists, attempting to deserialize");
                
                // Read file content with better error handling
                string xmlContent;
                try
                {
                    xmlContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8).ConfigureAwait(false);
                }
                catch (Exception fileEx)
                {
                    DebugLogException("File read", fileEx);
                    throw;
                }
                
                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    DebugLog("File is empty, creating new repository");
                    return CreateNewRepository(isolatedStorageName);
                }
                
                DebugLogXmlContent(xmlContent, "OpenIsolatedStorage");
                
                // Deserialize on background thread
                var repo = await Task.Run(() =>
                {
                    try
                    {
                        using var stringReader = new StringReader(xmlContent);
                        var serializer = new XmlSerializer(typeof(XmlFavoritesRepository));
                        DebugLog("XmlSerializer created successfully");
                        
                        var result = (XmlFavoritesRepository)serializer.Deserialize(stringReader);
                        DebugLog("Deserialization completed");
                        return result;
                    }
                    catch (Exception deserEx)
                    {
                        DebugLogException("Deserialization", deserEx);
                        throw;
                    }
                }).ConfigureAwait(false);
                
                if (repo.People == null)
                {
                    DebugLog("People was null after deserialization, initializing new list");
                    repo.People = new List<Person>();
                }
                
                repo.IsolatedStorageName = isolatedStorageName;
                repo.DebugLogRepositoryState("OpenIsolatedStorage - Success");
                
                return repo;
            }
            else
            {
                DebugLog("File does not exist, creating new repository");
                return CreateNewRepository(isolatedStorageName);
            }
        }
        catch (Exception ex)
        {
            DebugLogException("OpenIsolatedStorage", ex);
            return CreateNewRepository(isolatedStorageName);
        }
    }

    public async static Task<XmlFavoritesRepository> OpenFile(string path)
    {
        DebugLog($"OpenFile called with: {path}");
        
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(path).ConfigureAwait(false);
            DebugLog($"Opened app package file stream for: {path}");
            
            // Read content first
            string xmlContent;
            using (var reader = new StreamReader(stream))
            {
                xmlContent = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            
            if (string.IsNullOrWhiteSpace(xmlContent))
            {
                DebugLog("App package file is empty, creating new repository");
                return CreateNewRepository(Path.GetFileName(path));
            }
            
            DebugLogXmlContent(xmlContent, "OpenFile");
            
            // Deserialize on background thread
            var repo = await Task.Run(() =>
            {
                try
                {
                    using var stringReader = new StringReader(xmlContent);
                    var serializer = new XmlSerializer(typeof(XmlFavoritesRepository));
                    DebugLog("XmlSerializer created for OpenFile");
                    
                    var result = (XmlFavoritesRepository)serializer.Deserialize(stringReader);
                    DebugLog("OpenFile deserialization completed");
                    return result;
                }
                catch (Exception deserEx)
                {
                    DebugLogException("OpenFile deserialization", deserEx);
                    throw;
                }
            }).ConfigureAwait(false);
            
            if (repo.People == null)
            {
                DebugLog("People was null after OpenFile deserialization, initializing new list");
                repo.People = new List<Person>();
            }
            
            repo.IsolatedStorageName = Path.GetFileName(path);
            repo.DebugLogRepositoryState("OpenFile - Success");
            
            return repo;
        }
        catch (Exception ex)
        {
            DebugLogException("OpenFile", ex);
            return CreateNewRepository(Path.GetFileName(path));
        }
    }

    private static XmlFavoritesRepository CreateNewRepository(string isolatedStorageName)
    {
        var newRepo = new XmlFavoritesRepository
        {
            IsolatedStorageName = isolatedStorageName,
            People = new List<Person>()
        };
        newRepo.DebugLogRepositoryState("CreateNewRepository");
        return newRepo;
    }

    private async Task Commit()
    {
        DebugLog("Commit called");
        
        // Prevent multiple concurrent commits
        lock (_lockObject)
        {
            if (_isCommitting)
            {
                DebugLog("Commit already in progress, skipping");
                return;
            }
            _isCommitting = true;
        }

        try
        {
            await CommitInternal().ConfigureAwait(false);
        }
        finally
        {
            lock (_lockObject)
            {
                _isCommitting = false;
            }
        }
    }

    private async Task CommitInternal()
    {
        DebugLogRepositoryState("Commit - Start");
        
        if (string.IsNullOrEmpty(IsolatedStorageName))
        {
            DebugLog("ERROR: IsolatedStorageName is null or empty, cannot commit");
            return;
        }
        
        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, IsolatedStorageName);
            var directoryPath = Path.GetDirectoryName(filePath);
            
            DebugLog($"Commit target file: {filePath}");
            if (!string.IsNullOrEmpty(directoryPath))
            {
                DebugLogDirectoryInfo(directoryPath);
                
                // Ensure directory exists
                Directory.CreateDirectory(directoryPath);
                DebugLog("Directory created/verified");
            }
            
            // Serialize on background thread
            var xmlContent = await Task.Run(() =>
            {
                try
                {
                    using var memoryStream = new MemoryStream();
                    var serializer = new XmlSerializer(typeof(XmlFavoritesRepository));
                    DebugLog("XmlSerializer created for Commit");
                    
                    // Create a copy of the current state to avoid threading issues
                    var currentPeople = new List<Person>();
                    lock (_lockObject)
                    {
                        if (People != null)
                        {
                            currentPeople.AddRange(People);
                        }
                    }
                    
                    var repoToSerialize = new XmlFavoritesRepository
                    {
                        IsolatedStorageName = this.IsolatedStorageName,
                        People = currentPeople
                    };
                    
                    serializer.Serialize(memoryStream, repoToSerialize);
                    var content = Encoding.UTF8.GetString(memoryStream.ToArray());
                    DebugLog("Serialization completed");
                    return content;
                }
                catch (Exception serEx)
                {
                    DebugLogException("Serialization", serEx);
                    throw;
                }
            }).ConfigureAwait(false);
            
            DebugLogXmlContent(xmlContent, "Commit");
            
            // Write to file
            await File.WriteAllTextAsync(filePath, xmlContent, Encoding.UTF8).ConfigureAwait(false);
            
            DebugLog("File written successfully");
            DebugLogFileInfo(filePath, "Commit - After write");
        }
        catch (Exception ex)
        {
            DebugLogException("Commit", ex);
        }

        // Fire changed event on main thread
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var ev = Changed;
                if (ev != null)
                {
                    DebugLog("Firing Changed event");
                    ev(this, EventArgs.Empty);
                }
                else
                {
                    DebugLog("No Changed event handlers registered");
                }
            });
        }
        catch (Exception eventEx)
        {
            DebugLogException("Changed event", eventEx);
        }
    }

    #region IFavoritesRepository implementation

    public IEnumerable<Person> GetAll()
    {
        lock (_lockObject)
        {
            DebugLog($"GetAll called, returning {People?.Count ?? 0} people");
            return People?.ToList() ?? new List<Person>();
        }
    }

    public Person FindById(string id)
    {
        DebugLog($"FindById called with id: {id}");
        lock (_lockObject)
        {
            var result = People?.FirstOrDefault(x => x.Id == id);
            DebugLog($"FindById result: {(result != null ? "Found" : "Not found")}");
            return result;
        }
    }

    public bool IsFavorite(Person person)
    {
        if (person == null)
        {
            DebugLog("IsFavorite called with null person");
            return false;
        }
        
        lock (_lockObject)
        {
            var result = People?.Any(x => x.Id == person.Id) ?? false;
            DebugLog($"IsFavorite called for person {person.Name} (ID: {person.Id}): {result}");
            return result;
        }
    }

    public void InsertOrUpdate(Person person)
    {
        if (person == null)
        {
            DebugLog("InsertOrUpdate called with null person");
            return;
        }
        
        DebugLog($"InsertOrUpdate called for person {person.Name} (ID: {person.Id})");
        
        lock (_lockObject)
        {
            DebugLogRepositoryState("InsertOrUpdate - Before");
            
            var existing = People?.FirstOrDefault(x => x.Id == person.Id);
            if (existing != null)
            {
                DebugLog("Removing existing person");
                People?.Remove(existing);
            }

            People?.Add(person);
            DebugLogRepositoryState("InsertOrUpdate - After add");
        }
        
        DebugLog("Starting commit task");
        Task.Run(async () => await Commit().ConfigureAwait(false));
    }

    public void Delete(Person person)
    {
        if (person == null)
        {
            DebugLog("Delete called with null person");
            return;
        }
        
        DebugLog($"Delete called for person {person.Name} (ID: {person.Id})");
        
        int changeCount = 0;
        lock (_lockObject)
        {
            DebugLogRepositoryState("Delete - Before");
            
            var originalCount = People?.Count ?? 0;
            if (People != null)
            {
                var newPeople = People.Where(p => p.Id != person.Id).ToList();
                changeCount = originalCount - newPeople.Count;
                People.Clear();
                People.AddRange(newPeople);
            }
            
            DebugLog($"Removing {changeCount} people");
            DebugLogRepositoryState("Delete - After");
        }
        
        if (changeCount > 0)
        {
            DebugLog("Starting commit task for delete");
            Task.Run(async () => await Commit().ConfigureAwait(false));
        }
        else
        {
            DebugLog("No changes made, skipping commit");
        }
    }

    #endregion
}