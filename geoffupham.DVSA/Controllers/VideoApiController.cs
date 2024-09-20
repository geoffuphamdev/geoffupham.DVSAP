using geoffupham.DVSA.Models;
using geoffupham.DVSA.Services;
using Microsoft.AspNetCore.Mvc;

namespace geoffupham.DVSA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoApiController : ControllerBase
{
    private readonly ILogger<VideoApiController> _logger;
    private readonly IVideoService _videoService;

    public VideoApiController(ILogger<VideoApiController> logger,IVideoService videoService)
    {
        _logger = logger;
        _videoService = videoService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadVideos([FromForm] List<IFormFile> files)
    {
        if (files == null || !files.Any())
            return BadRequest("No files uploaded");

        var uploadedFiles = new List<VideoFile>();
        foreach (var file in files)
        {
            if (!file.ContentType.Equals("video/mp4", StringComparison.OrdinalIgnoreCase))
                return BadRequest($"File {file.FileName} is not an MP4 file. Only MP4 files are allowed.");

            try
            {
                var result = await _videoService.UploadVideoAsync(file);
                if (result)
                {
                    uploadedFiles.Add(new VideoFile
                    {
                        Name = file.FileName,
                        Size = file.Length
                    });
                }
                else
                {
                    return BadRequest($"Upload failed for file: {file.FileName}");
                }
            }
            catch (IOException ex)
            {
                // Log the exception
                _logger.LogError(ex, $"Error uploading file: {file.FileName}");
                return StatusCode(500, $"An error occurred while uploading {file.FileName}. Please try again later.");
            }
        }

        try
        {
            // Get the updated list of all videos
            var allVideos = await _videoService.GetAllVideosAsync();
            return Ok(allVideos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving video list after upload");
            return StatusCode(500, "Uploads were successful, but there was an error retrieving the updated video list.");
        }
    }
}