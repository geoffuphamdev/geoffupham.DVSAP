using geoffupham.DVSA.Models;
using geoffupham.DVSA.Services;
using geoffupham.DVSA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace geoffupham.DVSA.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IVideoService _videoService;

    public HomeController(ILogger<HomeController> logger, IVideoService videoService)
    {
        _logger = logger;
        _videoService = videoService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            Videos =  await _videoService.GetAllVideosAsync() //?? new List<VideoFile>()
        };
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetVideos()
    {
        var videos = await _videoService.GetAllVideosAsync();
        return Json(videos);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


