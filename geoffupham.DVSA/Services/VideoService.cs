using geoffupham.DVSA.Models;
using Microsoft.Extensions.Configuration;

namespace geoffupham.DVSA.Services;

public class VideoService : IVideoService
{
    private readonly string _mediaFolder;
    private readonly long _maxFileSize;

    public VideoService(IConfiguration configuration)
    {
        _mediaFolder = configuration.GetValue<string>("MediaFolder");
        _maxFileSize = configuration.GetValue<long>("MaxFileSize");
    }

    public IEnumerable<VideoFile> GetAllVideos()
    {
        var directory = new DirectoryInfo(_mediaFolder);
        var files = directory.GetFiles("*.mp4");

        return files.Select(f => new VideoFile
        {
            Name = f.Name,
            Size = f.Length,
            Path = Path.GetRelativePath(_mediaFolder, f.FullName).Replace('\\', '/')
        });
    }

    public async Task<List<VideoFile>> GetAllVideosAsync()
    {
        var directory = new DirectoryInfo(_mediaFolder);
        var files = await Task.Run(() => directory.GetFiles("*.mp4"));

        return files.Select(f => new VideoFile
        {
            Name = f.Name,
            Size = f.Length,
            Path = Path.GetRelativePath(_mediaFolder, f.FullName).Replace('\\', '/')
        }).ToList();
    }

    public async Task<bool> UploadVideoAsync(IFormFile file)
    {
        if (file.Length > _maxFileSize)
            return false;

        var filePath = Path.Combine(_mediaFolder, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return true;
    }
}