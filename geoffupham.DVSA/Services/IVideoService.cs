using geoffupham.DVSA.Models;

namespace geoffupham.DVSA.Services;

public interface IVideoService
{
    IEnumerable<VideoFile> GetAllVideos();
    Task<List<VideoFile>> GetAllVideosAsync();
    Task<bool> UploadVideoAsync(IFormFile file);
}
