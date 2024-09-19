using Microsoft.AspNetCore.Mvc;
using geoffupham.DVSA.Services;
using geoffupham.DVSA.Models;
using System.Diagnostics;
using geoffupham.DVSA.ViewModels;

namespace geoffupham.DVSA.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IVideoService _videoService;

    public HomeController(ILogger<HomeController> logger,IVideoService videoService)
    {
        _logger = logger;
        _videoService = videoService;
    }

    public IActionResult Index()
    {
        var viewModel = new HomeViewModel
        {
            Videos = (IEnumerable<VideoFile>)_videoService.GetAllVideos() //?? new List<VideoFile>()
        };
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetVideos()
    {
        var videos = _videoService.GetAllVideos();
        return Json(videos);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


