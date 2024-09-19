using Microsoft.AspNetCore.Mvc;
using geoffupham.DVSA.Services;
using geoffupham.DVSA.Models;
using System.Collections.Generic;

namespace geoffupham.DVSA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoApiController : ControllerBase
{
    private readonly IVideoService _videoService;

    public VideoApiController(IVideoService videoService)
    {
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

        // Get the updated list of all videos
        var allVideos = await _videoService.GetAllVideosAsync();

        return Ok(allVideos);
    }
}