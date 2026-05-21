using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymApp.Controllers;

public class ProgramExercisesController : Controller
{
    private readonly IGymRepository _repository;

    public ProgramExercisesController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("program-vjezbe/{day?}")]
    public IActionResult Index(string day = "all", string q = "")
    {
        ViewData["Section"] = "Program vjezbe";
        ViewData["SectionController"] = "ProgramExercises";
        ViewData["PageTitle"] = "Poveznice program-vjezba";
        ViewData["SelectedFilter"] = day;
        ViewData["Search"] = q;

        var programExercises = FilterProgramExercises(day, q);

        return View(programExercises
            .OrderBy(x => x.Program?.Name)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.Exercise?.Name));
    }

    [HttpGet("program-vjezbe/pretraga")]
    public IActionResult Search(string day = "all", string q = "")
    {
        var programExercises = FilterProgramExercises(day, q)
            .OrderBy(x => x.Program?.Name)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.Exercise?.Name);

        return PartialView("_List", programExercises);
    }

    [HttpGet("program-vjezbe/detalji/{id:guid}")]
    public IActionResult Details(Guid id)
    {
        var programExercise = _repository.GetProgramExercise(id);
        if (programExercise is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Program vjezbe";
        ViewData["SectionController"] = "ProgramExercises";
        ViewData["PageTitle"] = $"Poveznica: {programExercise.Program?.Name}";
        return View(programExercise);
    }

    [HttpGet("program-vjezbe/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Program vjezbe";
        ViewData["SectionController"] = "ProgramExercises";
        ViewData["PageTitle"] = "Nova poveznica";
        PopulateSelectLists();
        return View(new ProgramExercise());
    }

    [HttpPost("program-vjezbe/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProgramExercise programExercise)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Program vjezbe";
            ViewData["SectionController"] = "ProgramExercises";
            ViewData["PageTitle"] = "Nova poveznica";
            PopulateSelectLists(programExercise.TrainingProgramId, programExercise.ExerciseId);
            return View(programExercise);
        }

        if (programExercise.Id == Guid.Empty)
        {
            programExercise.Id = Guid.NewGuid();
        }

        _repository.AddProgramExercise(programExercise);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("program-vjezbe/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var programExercise = _repository.GetProgramExercise(id);
        if (programExercise is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Program vjezbe";
        ViewData["SectionController"] = "ProgramExercises";
        ViewData["PageTitle"] = "Uredi poveznicu";
        PopulateSelectLists(programExercise.TrainingProgramId, programExercise.ExerciseId);
        return View(programExercise);
    }

    [HttpPost("program-vjezbe/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, ProgramExercise programExercise)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Program vjezbe";
            ViewData["SectionController"] = "ProgramExercises";
            ViewData["PageTitle"] = "Uredi poveznicu";
            PopulateSelectLists(programExercise.TrainingProgramId, programExercise.ExerciseId);
            return View(programExercise);
        }

        var existing = _repository.GetProgramExercise(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.TrainingProgramId = programExercise.TrainingProgramId;
        existing.ExerciseId = programExercise.ExerciseId;
        existing.DayOfWeek = programExercise.DayOfWeek;
        existing.TargetSets = programExercise.TargetSets;
        existing.TargetReps = programExercise.TargetReps;

        _repository.UpdateProgramExercise(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("program-vjezbe/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var programExercise = _repository.GetProgramExercise(id);
        if (programExercise is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Program vjezbe";
        ViewData["SectionController"] = "ProgramExercises";
        ViewData["PageTitle"] = "Obrisi poveznicu";
        return View(programExercise);
    }

    [HttpPost("program-vjezbe/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var programExercise = _repository.GetProgramExercise(id);
        if (programExercise is null)
        {
            return NotFound();
        }

        _repository.DeleteProgramExercise(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<ProgramExercise> FilterProgramExercises(string day, string q)
    {
        var programExercises = _repository.ProgramExercises.AsEnumerable();
        if (day == "d1")
        {
            programExercises = programExercises.Where(x => x.DayOfWeek == 1);
        }
        else if (day == "d2")
        {
            programExercises = programExercises.Where(x => x.DayOfWeek == 2);
        }
        else if (day == "d3")
        {
            programExercises = programExercises.Where(x => x.DayOfWeek == 3);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            programExercises = programExercises.Where(x =>
                (x.Program?.Name ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (x.Exercise?.Name ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return programExercises;
    }

    private void PopulateSelectLists(Guid? programId = null, Guid? exerciseId = null)
    {
        var programs = _repository.Programs
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name });
        var exercises = _repository.Exercises
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name });

        ViewData["Programs"] = new SelectList(programs, "Id", "Name", programId);
        ViewData["Exercises"] = new SelectList(exercises, "Id", "Name", exerciseId);
    }
}
