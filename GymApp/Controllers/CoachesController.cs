using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class CoachesController : Controller
{
    private readonly IGymRepository _repository;

    public CoachesController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("treneri")]
    public IActionResult Index(string q = "")
    {
        ViewData["Section"] = "Treneri";
        ViewData["SectionController"] = "Coaches";
        ViewData["PageTitle"] = "Lista trenera";
        ViewData["Search"] = q;

        var coaches = FilterCoaches(q)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName);

        return View(coaches);
    }

    [HttpGet("treneri/pretraga")]
    public IActionResult Search(string q = "")
    {
        var coaches = FilterCoaches(q)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName);

        return PartialView("_List", coaches);
    }

    [HttpGet("treneri/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Treneri";
        ViewData["SectionController"] = "Coaches";
        ViewData["PageTitle"] = "Novi trener";
        return View(new Coach());
    }

    [HttpPost("treneri/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Coach coach)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Treneri";
            ViewData["SectionController"] = "Coaches";
            ViewData["PageTitle"] = "Novi trener";
            return View(coach);
        }

        if (coach.Id == Guid.Empty)
        {
            coach.Id = Guid.NewGuid();
        }

        coach.CreatedAt = DateTime.UtcNow;
        _repository.AddCoach(coach);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("treneri/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var coach = _repository.GetCoach(id);
        if (coach is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Treneri";
        ViewData["SectionController"] = "Coaches";
        ViewData["PageTitle"] = $"Uredi: {coach.FirstName} {coach.LastName}";
        return View(coach);
    }

    [HttpPost("treneri/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, Coach coach)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Treneri";
            ViewData["SectionController"] = "Coaches";
            ViewData["PageTitle"] = "Uredi trenera";
            return View(coach);
        }

        var existing = _repository.GetCoach(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.FirstName = coach.FirstName;
        existing.LastName = coach.LastName;
        existing.Email = coach.Email;
        existing.Specialty = coach.Specialty;

        _repository.UpdateCoach(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("treneri/detalji/{id:guid}")]
    public IActionResult Details(Guid id)
    {
        var coach = _repository.GetCoach(id);
        if (coach is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Treneri";
        ViewData["SectionController"] = "Coaches";
        ViewData["PageTitle"] = $"Detalji: {coach.FirstName} {coach.LastName}";
        return View(coach);
    }

    [HttpGet("treneri/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var coach = _repository.GetCoach(id);
        if (coach is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Treneri";
        ViewData["SectionController"] = "Coaches";
        ViewData["PageTitle"] = $"Obrisi: {coach.FirstName} {coach.LastName}";
        return View(coach);
    }

    [HttpPost("treneri/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var coach = _repository.GetCoach(id);
        if (coach is null)
        {
            return NotFound();
        }

        _repository.DeleteCoach(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<Coach> FilterCoaches(string q)
    {
        var coaches = _repository.Coaches.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            coaches = coaches.Where(x =>
                x.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Specialty.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return coaches;
    }
}
