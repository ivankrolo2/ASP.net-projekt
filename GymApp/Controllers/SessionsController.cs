using GymApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class SessionsController : Controller
{
    private readonly IGymRepository _repository;

    public SessionsController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("treninzi/{month?}")]
    public IActionResult Index(string month = "all")
    {
        ViewData["Section"] = "Treninzi";
        ViewData["SectionController"] = "Sessions";
        ViewData["PageTitle"] = "Lista treninga";
        ViewData["SelectedFilter"] = month;

        var sessions = _repository.Sessions.AsEnumerable();
        sessions = month switch
        {
            "early" => sessions.Where(x => x.SessionDate.Day <= 7),
            "late" => sessions.Where(x => x.SessionDate.Day >= 8),
            _ => sessions
        };

        return View(sessions.OrderByDescending(x => x.SessionDate));
    }

    [HttpGet("treninzi/detalji/{id:guid}")]
    public IActionResult Details(Guid id)
    {
        var session = _repository.GetSession(id);
        if (session is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Treninzi";
        ViewData["SectionController"] = "Sessions";
        ViewData["PageTitle"] = $"Detalji treninga ({session.SessionDate:dd.MM.yyyy})";
        return View(session);
    }
}
