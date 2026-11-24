using Microsoft.Extensions.AI;
using System.Text.Json;
using ChatClientWithTools.Models;
using System.IO;
using System.Linq;

namespace ChatClientWithTools.Services.Tools;

public class FileOperationsTool : AIFunction
{
    public override string Name => "list_files";
    public override string Description => "Lists files and directories in a specified path";

    public override JsonElement JsonSchema => JsonSerializer.SerializeToElement(new
    {
        type = "object",
        properties = new
        {
            path = new
            {
                type = "string",
                description = "Directory path or common name (Documents, Desktop, Downloads)",
                // default captured in logic
            },
            max_files = new
            {
                type = "integer",
                description = "Maximum number of entries to return (1-100)",
                minimum = 1,
                maximum = 100
            }
        }
    });

    protected override ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var path = GetString(arguments, "path", "Documents");
        var max = Math.Clamp(GetInt(arguments, "max_files", 20), 1, 100);
        var resolved = ResolveCommonPath(path);

        try
        {
            if (!Directory.Exists(resolved))
            {
                return ValueTask.FromResult<object?>(new FileListResult
                {
                    Path = resolved,
                    Files = new List<FileListResult.FileInfo>()
                });
            }

            var dirInfo = new DirectoryInfo(resolved);
            var entries = new List<FileListResult.FileInfo>();

            foreach (var dir in dirInfo.GetDirectories().Take(max))
            {
                entries.Add(new FileListResult.FileInfo
                {
                    Name = dir.Name,
                    FullPath = dir.FullName,
                    Size = 0,
                    Modified = dir.LastWriteTime,
                    IsDirectory = true
                });
                if (entries.Count >= max) break;
            }

            if (entries.Count < max)
            {
                foreach (var file in dirInfo.GetFiles().Take(max - entries.Count))
                {
                    entries.Add(new FileListResult.FileInfo
                    {
                        Name = file.Name,
                        FullPath = file.FullName,
                        Size = file.Length,
                        Modified = file.LastWriteTime,
                        IsDirectory = false
                    });
                }
            }

            return ValueTask.FromResult<object?>(new FileListResult
            {
                Path = resolved,
                Files = entries.OrderBy(e => !e.IsDirectory).ThenBy(e => e.Name).ToList()
            });
        }
        catch
        {
            return ValueTask.FromResult<object?>(new FileListResult
            {
                Path = resolved,
                Files = new List<FileListResult.FileInfo>()
            });
        }
    }

    private static string GetString(AIFunctionArguments args, string name, string def = "") =>
        args.TryGetValue(name, out var v) ? v?.ToString() ?? def : def;

    private static int GetInt(AIFunctionArguments args, string name, int def = 0) =>
        args.TryGetValue(name, out var v) && v is not null && int.TryParse(v.ToString(), out var i) ? i : def;

    private static string ResolveCommonPath(string path)
    {
        var lower = path?.Trim().ToLowerInvariant() ?? string.Empty;
        return lower switch
        {
            "documents" => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "desktop" => Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "downloads" => GetDownloadsFolder(),
            "pictures" => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "music" => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            "videos" => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            "home" => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            _ => Path.IsPathRooted(path ?? string.Empty) ? path! : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path ?? string.Empty)
        };
    }

    private static string GetDownloadsFolder()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var downloads = Path.Combine(userProfile, "Downloads");
        return Directory.Exists(downloads) ? downloads : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }
}