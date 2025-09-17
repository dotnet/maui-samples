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
using System.Text.Json; // Added for JSON serialization

namespace EmployeeDirectory.Core.Data;

// Removed XmlRoot attribute; JSON serialization does not need it
public class XmlFavoritesRepository : IFavoritesRepository
{
    private static readonly object _lockObject = new object();
    private volatile bool _isCommitting = false;

    // Fixed storage file name (always JSON)
    private const string StorageFileName = "XamarinFavorites.json";

    // JSON serializer options
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public string IsolatedStorageName { get; set; } = StorageFileName; // Always set to fixed file name

    public event EventHandler Changed;

    public List<Person> People { get; set; } = new List<Person>(); // Always initialize

    public XmlFavoritesRepository()
    {
        People = new List<Person>();
        IsolatedStorageName = StorageFileName;
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
        // Ignore provided name; always use fixed JSON file
        isolatedStorageName = StorageFileName;
        DebugLog($"OpenIsolatedStorage called (forced file): {isolatedStorageName}");

        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, StorageFileName);
            DebugLogFileInfo(filePath, "OpenIsolatedStorage");

            var directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                DebugLogDirectoryInfo(directoryPath);
            }

            if (File.Exists(filePath))
            {
                DebugLog("File exists, attempting to deserialize (JSON)");

                string content;
                try
                {
                    content = await File.ReadAllTextAsync(filePath, Encoding.UTF8).ConfigureAwait(false);
                }
                catch (Exception fileEx)
                {
                    DebugLogException("File read", fileEx);
                    throw;
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    DebugLog("File is empty, creating new repository");
                    return CreateNewRepository(StorageFileName);
                }

                DebugLogXmlContent(content, "OpenIsolatedStorage(JSON raw)");

                try
                {
                    // Deserialize JSON
                    var repo = JsonSerializer.Deserialize<XmlFavoritesRepository>(content, JsonOptions) ?? new XmlFavoritesRepository();
                    if (repo.People == null)
                        repo.People = new List<Person>();

                    repo.IsolatedStorageName = StorageFileName;
                    repo.DebugLogRepositoryState("OpenIsolatedStorage - Success(JSON)");
                    return repo;
                }
                catch (Exception deserEx)
                {
                    DebugLogException("JSON Deserialization", deserEx);
                    return CreateNewRepository(StorageFileName);
                }
            }
            else
            {
                DebugLog("File does not exist, creating new repository");
                return CreateNewRepository(StorageFileName);
            }
        }
        catch (Exception ex)
        {
            DebugLogException("OpenIsolatedStorage", ex);
            return CreateNewRepository(StorageFileName);
        }
    }

    public async static Task<XmlFavoritesRepository> OpenFile(string path)
    {
        // This method reads from app package assets; leave behavior but still normalize stored name
        DebugLog($"OpenFile called with: {path}");

        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(path).ConfigureAwait(false);
            DebugLog($"Opened app package file stream for: {path}");

            string content;
            using (var reader = new StreamReader(stream))
            {
                content = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                DebugLog("App package file is empty, creating new repository");
                return CreateNewRepository(StorageFileName);
            }

            DebugLogXmlContent(content, "OpenFile(JSON raw)");

            try
            {
                var repo = JsonSerializer.Deserialize<XmlFavoritesRepository>(content, JsonOptions) ?? new XmlFavoritesRepository();
                if (repo.People == null)
                {
                    DebugLog("People was null after deserialization, initializing new list");
                    repo.People = new List<Person>();
                }
                repo.IsolatedStorageName = StorageFileName;
                repo.DebugLogRepositoryState("OpenFile - Success(JSON)");
                return repo;
            }
            catch (Exception deserEx)
            {
                DebugLogException("JSON Deserialization(OpenFile)", deserEx);
                return CreateNewRepository(StorageFileName);
            }
        }
        catch (Exception ex)
        {
            DebugLogException("OpenFile", ex);
            return CreateNewRepository(StorageFileName);
        }
    }

    private static XmlFavoritesRepository CreateNewRepository(string isolatedStorageName)
    {
        var newRepo = new XmlFavoritesRepository
        {
            IsolatedStorageName = StorageFileName,
            People = new List<Person>()
        };
        newRepo.DebugLogRepositoryState("CreateNewRepository");
        return newRepo;
    }

    private async Task Commit()
    {
        DebugLog("Commit called");

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

        // Always ensure file name is the fixed JSON file
        IsolatedStorageName = StorageFileName;

        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, StorageFileName);
            var directoryPath = Path.GetDirectoryName(filePath);

            DebugLog($"Commit target file: {filePath}");
            if (!string.IsNullOrEmpty(directoryPath))
            {
                DebugLogDirectoryInfo(directoryPath);
                Directory.CreateDirectory(directoryPath);
                DebugLog("Directory created/verified");
            }

            var jsonContent = await Task.Run(() =>
            {
                try
                {
                    List<Person> currentPeople;
                    lock (_lockObject)
                    {
                        currentPeople = People != null ? new List<Person>(People) : new List<Person>();
                    }

                    var repoToSerialize = new XmlFavoritesRepository
                    {
                        IsolatedStorageName = StorageFileName,
                        People = currentPeople
                    };

                    var content = JsonSerializer.Serialize(repoToSerialize, JsonOptions);
                    DebugLog("Serialization (JSON) completed");
                    return content;
                }
                catch (Exception serEx)
                {
                    DebugLogException("Serialization(JSON)", serEx);
                    throw;
                }
            }).ConfigureAwait(false);

            DebugLogXmlContent(jsonContent, "Commit(JSON)");
            await File.WriteAllTextAsync(filePath, jsonContent, Encoding.UTF8).ConfigureAwait(false);
            DebugLog("File written successfully");
            DebugLogFileInfo(filePath, "Commit - After write");
        }
        catch (Exception ex)
        {
            DebugLogException("Commit", ex);
        }

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