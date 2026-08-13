using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace LocalChatClientWithTools.Services.Tools;

public class FileOperationsTool
{
    public static AIFunction CreateAIFunction(IServiceProvider services)
        => AIFunctionFactory.Create(
            services.GetRequiredService<FileOperationsTool>().ListFiles,
            name: "list_files",
            serializerOptions: ToolJsonContext.Default.Options);

    [Description("Lists files and directories in a specified path")]
    public FileListResult ListFiles(
        [Description("Directory path or common name (Documents, Desktop, Downloads)")] string path = "Documents",
        [Description("Maximum number of entries to return (1-100)")] int maxFiles = 20)
    {
        var max = Math.Clamp(maxFiles, 1, 100);
        var resolved = ResolveCommonPath(path);

        try
        {
            if (!Directory.Exists(resolved))
            {
                return new FileListResult
                {
                    Path = resolved,
                    Files = new List<FileListResult.FileEntry>()
                };
            }

            var dirInfo = new DirectoryInfo(resolved);
            var entries = new List<FileListResult.FileEntry>();

            foreach (var dir in dirInfo.GetDirectories().Take(max))
            {
                entries.Add(new FileListResult.FileEntry
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
                    entries.Add(new FileListResult.FileEntry
                    {
                        Name = file.Name,
                        FullPath = file.FullName,
                        Size = file.Length,
                        Modified = file.LastWriteTime,
                        IsDirectory = false
                    });
                }
            }

            return new FileListResult
            {
                Path = resolved,
                Files = entries.OrderBy(e => !e.IsDirectory).ThenBy(e => e.Name).ToList()
            };
        }
        catch
        {
            return new FileListResult
            {
                Path = resolved,
                Files = new List<FileListResult.FileEntry>()
            };
        }
    }

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

    public record FileListResult
    {
        public required string Path { get; init; }
        public required List<FileEntry> Files { get; init; }

        public record FileEntry
        {
            public required string Name { get; init; }
            public required string FullPath { get; init; }
            public required long Size { get; init; }
            public required DateTimeOffset Modified { get; init; }
            public required bool IsDirectory { get; init; }
            public string Icon => IsDirectory ? "📁" : GetFileIcon();

            private string GetFileIcon()
            {
                var extension = System.IO.Path.GetExtension(Name).ToLower();
                return extension switch
                {
                    ".txt" => "📄",
                    ".pdf" => "📕",
                    ".doc" or ".docx" => "📘",
                    ".xls" or ".xlsx" => "📗",
                    ".ppt" or ".pptx" => "📙",
                    ".jpg" or ".jpeg" or ".png" or ".gif" => "🖼️",
                    ".mp4" or ".avi" or ".mov" => "🎬",
                    ".mp3" or ".wav" => "🎵",
                    ".zip" or ".rar" => "📦",
                    ".exe" => "⚙️",
                    _ => "📄"
                };
            }
        }
    }
}
