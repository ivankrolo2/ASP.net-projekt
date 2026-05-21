using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymApp.Controllers;

public class SetEntriesController : Controller
{
    private readonly IGymRepository _repository;

    public SetEntriesController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("setovi/{exercise?}")]
    public IActionResult Index(string exercise = "all", string q = "")
    {
        ViewData["Section"] = "Setovi";
        ViewData["SectionController"] = "SetEntries";
        ViewData["PageTitle"] = "Evidencija setova";
        ViewData["SelectedFilter"] = exercise;
        ViewData["Search"] = q;

        var setEntries = FilterSetEntries(exercise, q);

        return View(setEntries
            .OrderByDescending(x => x.Session?.SessionDate)
            .ThenBy(x => x.SetNumber));
    }

    [HttpGet("setovi/pretraga")]
    public IActionResult Search(string exercise = "all", string q = "")
    {
        var setEntries = FilterSetEntries(exercise, q)
            .OrderByDescending(x => x.Session?.SessionDate)
            .ThenBy(x => x.SetNumber);

        return PartialView("_List", setEntries);
    }

    [HttpGet("setovi/detalji/{id:guid}")]
    public IActionResult Details(Guid id)
    {
        var setEntry = _repository.GetSetEntry(id);
        if (setEntry is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Setovi";
        ViewData["SectionController"] = "SetEntries";
        ViewData["PageTitle"] = $"Set #{setEntry.SetNumber}";
        return View(setEntry);
    }

    [HttpGet("setovi/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Setovi";
        ViewData["SectionController"] = "SetEntries";
        ViewData["PageTitle"] = "Novi set";
        PopulateSelectLists();
        return View(new SetEntry());
    }

    [HttpPost("setovi/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SetEntry setEntry)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Setovi";
            ViewData["SectionController"] = "SetEntries";
            ViewData["PageTitle"] = "Novi set";
            PopulateSelectLists(setEntry.WorkoutSessionId, setEntry.ExerciseId);
            return View(setEntry);
        }

        if (setEntry.Id == Guid.Empty)
        {
            setEntry.Id = Guid.NewGuid();
        }

        _repository.AddSetEntry(setEntry);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("setovi/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var setEntry = _repository.GetSetEntry(id);
        if (setEntry is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Setovi";
        ViewData["SectionController"] = "SetEntries";
        ViewData["PageTitle"] = "Uredi set";
        PopulateSelectLists(setEntry.WorkoutSessionId, setEntry.ExerciseId);
        return View(setEntry);
    }

    [HttpPost("setovi/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, SetEntry setEntry)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Setovi";
            ViewData["SectionController"] = "SetEntries";
            ViewData["PageTitle"] = "Uredi set";
            PopulateSelectLists(setEntry.WorkoutSessionId, setEntry.ExerciseId);
            return View(setEntry);
        }

        var existing = _repository.GetSetEntry(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.WorkoutSessionId = setEntry.WorkoutSessionId;
        existing.ExerciseId = setEntry.ExerciseId;
        existing.SetNumber = setEntry.SetNumber;
        existing.Repetitions = setEntry.Repetitions;
        existing.WeightKg = setEntry.WeightKg;

        _repository.UpdateSetEntry(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("setovi/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var setEntry = _repository.GetSetEntry(id);
        if (setEntry is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Setovi";
        ViewData["SectionController"] = "SetEntries";
        ViewData["PageTitle"] = "Obrisi set";
        return View(setEntry);
    }

    [HttpPost("setovi/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var setEntry = _repository.GetSetEntry(id);
        if (setEntry is null)
        {
            return NotFound();
        }

        _repository.DeleteSetEntry(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<SetEntry> FilterSetEntries(string exercise, string q)
    {
        var setEntries = _repository.SetEntries.AsEnumerable();
        setEntries = exercise switch
        {
            "big3" => setEntries.Where(x =>
                string.Equals(x.Exercise?.Name, "Bench Press", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Exercise?.Name, "Back Squat", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Exercise?.Name, "Deadlift", StringComparison.OrdinalIgnoreCase)),
            "bodyweight" => setEntries.Where(x => string.Equals(x.Exercise?.Equipment, "Vlastita tezina", StringComparison.OrdinalIgnoreCase)),
            _ => setEntries
        };

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            setEntries = setEntries.Where(x =>
                (x.Exercise?.Name ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                ($"{x.Session?.User?.FirstName} {x.Session?.User?.LastName}").Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (x.Session != null && x.Session.SessionDate.ToString("dd.MM.yyyy")
                    .Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        return setEntries;
    }

    private void PopulateSelectLists(Guid? sessionId = null, Guid? exerciseId = null)
    {
        var sessions = _repository.Sessions
            .OrderByDescending(x => x.SessionDate)
            .Select(x => new
            {
                x.Id,
                Name = $"{x.SessionDate:dd.MM.yyyy} - {x.User?.FirstName} {x.User?.LastName}"
            });
        var exercises = _repository.Exercises
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name });

        ViewData["Sessions"] = new SelectList(sessions, "Id", "Name", sessionId);
        ViewData["Exercises"] = new SelectList(exercises, "Id", "Name", exerciseId);
    }
}
