using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymApp.Controllers;

public class SessionsController : Controller
{
    private readonly IGymRepository _repository;

    public SessionsController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("treninzi/{month?}")]
    public IActionResult Index(string month = "all", string q = "")
    {
        ViewData["Section"] = "Treninzi";
        ViewData["SectionController"] = "Sessions";
        ViewData["PageTitle"] = "Lista treninga";
        ViewData["SelectedFilter"] = month;
        ViewData["Search"] = q;

        var sessions = FilterSessions(month, q);

        return View(sessions.OrderByDescending(x => x.SessionDate));
    }

    [HttpGet("treninzi/pretraga")]
    public IActionResult Search(string month = "all", string q = "")
    {
        var sessions = FilterSessions(month, q)
            .OrderByDescending(x => x.SessionDate);

        return PartialView("_List", sessions);
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

    [HttpGet("treninzi/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Treninzi";
        ViewData["SectionController"] = "Sessions";
        ViewData["PageTitle"] = "Novi trening";
        PopulateSelectLists();
        return View(new WorkoutSession());
    }

    [HttpPost("treninzi/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(WorkoutSession session)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Treninzi";
            ViewData["SectionController"] = "Sessions";
            ViewData["PageTitle"] = "Novi trening";
            PopulateSelectLists(session.UserId, session.ProgramId, session.GymLocationId);
            return View(session);
        }

        if (session.Id == Guid.Empty)
        {
            session.Id = Guid.NewGuid();
        }

        _repository.AddSession(session);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("treninzi/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var session = _repository.GetSession(id);
        if (session is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Treninzi";
        ViewData["SectionController"] = "Sessions";
        ViewData["PageTitle"] = "Uredi trening";
        PopulateSelectLists(session.UserId, session.ProgramId, session.GymLocationId);
        return View(session);
    }

    [HttpPost("treninzi/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, WorkoutSession session)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Treninzi";
            ViewData["SectionController"] = "Sessions";
            ViewData["PageTitle"] = "Uredi trening";
            PopulateSelectLists(session.UserId, session.ProgramId, session.GymLocationId);
            return View(session);
        }

        var existing = _repository.GetSession(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.UserId = session.UserId;
        existing.ProgramId = session.ProgramId;
        existing.GymLocationId = session.GymLocationId;
        existing.SessionDate = session.SessionDate;
        existing.DurationMinutes = session.DurationMinutes;
        existing.Notes = session.Notes;
        existing.Rating = session.Rating;
        existing.TotalVolumeKg = session.TotalVolumeKg;

        _repository.UpdateSession(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("treninzi/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var session = _repository.GetSession(id);
        if (session is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Treninzi";
        ViewData["SectionController"] = "Sessions";
        ViewData["PageTitle"] = "Obrisi trening";
        return View(session);
    }

    [HttpPost("treninzi/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var session = _repository.GetSession(id);
        if (session is null)
        {
            return NotFound();
        }

        _repository.DeleteSession(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<WorkoutSession> FilterSessions(string month, string q)
    {
        var sessions = _repository.Sessions.AsEnumerable();
        sessions = month switch
        {
            "early" => sessions.Where(x => x.SessionDate.Day <= 7),
            "late" => sessions.Where(x => x.SessionDate.Day >= 8),
            _ => sessions
        };

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            sessions = sessions.Where(x =>
                ($"{x.User?.FirstName} {x.User?.LastName}").Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (x.Program?.Name ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.SessionDate.ToString("dd.MM.yyyy").Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return sessions;
    }

    private void PopulateSelectLists(Guid? userId = null, Guid? programId = null, Guid? locationId = null)
    {
        var users = _repository.Users
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new { x.Id, Name = $"{x.FirstName} {x.LastName}" });
        var programs = _repository.Programs
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name });
        var locations = _repository.Locations
            .OrderBy(x => x.City)
            .ThenBy(x => x.Name)
            .Select(x => new { x.Id, Name = $"{x.City} - {x.Name}" });

        ViewData["Users"] = new SelectList(users, "Id", "Name", userId);
        ViewData["Programs"] = new SelectList(programs, "Id", "Name", programId);
        ViewData["Locations"] = new SelectList(locations, "Id", "Name", locationId);
    }
}
