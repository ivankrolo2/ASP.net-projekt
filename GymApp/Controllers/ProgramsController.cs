using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class ProgramsController : Controller
{
    private readonly IGymRepository _repository;

    public ProgramsController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("programi/{difficulty?}")]
    public IActionResult Index(string difficulty = "all", string q = "")
    {
        ViewData["Section"] = "Programi";
        ViewData["SectionController"] = "Programs";
        ViewData["PageTitle"] = "Lista programa";
        ViewData["SelectedFilter"] = difficulty;
        ViewData["Search"] = q;

        var programs = FilterPrograms(difficulty, q);

        return View(programs.OrderBy(x => x.Name));
    }

    [HttpGet("programi/pretraga")]
    public IActionResult Search(string difficulty = "all", string q = "")
    {
        var programs = FilterPrograms(difficulty, q)
            .OrderBy(x => x.Name);

        return PartialView("_List", programs);
    }

    [HttpGet("programi/detalji/{id:guid}")]
    public IActionResult Details(Guid id)
    {
        var program = _repository.GetProgram(id);
        if (program is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Programi";
        ViewData["SectionController"] = "Programs";
        ViewData["PageTitle"] = $"Detalji: {program.Name}";
        return View(program);
    }

    [HttpGet("programi/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Programi";
        ViewData["SectionController"] = "Programs";
        ViewData["PageTitle"] = "Novi program";
        return View(new TrainingProgram());
    }

    [HttpPost("programi/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TrainingProgram program)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Programi";
            ViewData["SectionController"] = "Programs";
            ViewData["PageTitle"] = "Novi program";
            return View(program);
        }

        if (program.Id == Guid.Empty)
        {
            program.Id = Guid.NewGuid();
        }

        program.CreatedAt = DateTime.UtcNow;
        _repository.AddProgram(program);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("programi/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var program = _repository.GetProgram(id);
        if (program is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Programi";
        ViewData["SectionController"] = "Programs";
        ViewData["PageTitle"] = $"Uredi: {program.Name}";
        return View(program);
    }

    [HttpPost("programi/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, TrainingProgram program)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Programi";
            ViewData["SectionController"] = "Programs";
            ViewData["PageTitle"] = "Uredi program";
            return View(program);
        }

        var existing = _repository.GetProgram(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = program.Name;
        existing.Goal = program.Goal;
        existing.Weeks = program.Weeks;
        existing.IsActive = program.IsActive;
        existing.Difficulty = program.Difficulty;
        existing.CoachName = program.CoachName;

        _repository.UpdateProgram(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("programi/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var program = _repository.GetProgram(id);
        if (program is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Programi";
        ViewData["SectionController"] = "Programs";
        ViewData["PageTitle"] = $"Obrisi: {program.Name}";
        return View(program);
    }

    [HttpPost("programi/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var program = _repository.GetProgram(id);
        if (program is null)
        {
            return NotFound();
        }

        _repository.DeleteProgram(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<TrainingProgram> FilterPrograms(string difficulty, string q)
    {
        var programs = _repository.Programs.AsEnumerable();
        if (difficulty != "all" && Enum.TryParse<WorkoutDifficulty>(difficulty, true, out var parsedDifficulty))
        {
            programs = programs.Where(x => x.Difficulty == parsedDifficulty);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            programs = programs.Where(x =>
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Goal.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.CoachName.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return programs;
    }
}
