using System.Diagnostics;
using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class HomeController : Controller
{
    private readonly IGymRepository _repository;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IGymRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public IActionResult Index()
    {
        ViewData["Section"] = "Dashboard";
        ViewData["SectionController"] = "Home";
        ViewData["PageTitle"] = "Fitness dashboard";

        var topExercises = _repository.Sessions
            .SelectMany(x => x.SetEntries)
            .GroupBy(x => x.Exercise?.Name ?? "Unknown")
            .Select(g => new ExerciseUsageStat
            {
                ExerciseName = g.Key,
                SetCount = g.Count(),
                RepetitionCount = g.Sum(x => x.Repetitions)
            })
            .OrderByDescending(x => x.SetCount)
            .Take(4)
            .ToList();

        var viewModel = new HomeDashboardViewModel
        {
            UsersCount = _repository.Users.Count,
            ProgramsCount = _repository.Programs.Count,
            SessionsCount = _repository.Sessions.Count,
            ExercisesCount = _repository.Exercises.Count,
            RecentSessions = _repository.Sessions
                .OrderByDescending(x => x.SessionDate)
                .Take(5)
                .ToList(),
            TopExercises = topExercises
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        ViewData["Section"] = "Info";
        ViewData["SectionController"] = "Home";
        ViewData["PageTitle"] = "Privacy";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var reqId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View(new ErrorViewModel { RequestId = reqId });
    }
}