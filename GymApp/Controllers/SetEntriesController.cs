using GymApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class SetEntriesController : Controller
{
    private readonly IGymRepository _repository;

    public SetEntriesController(IGymRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string exercise = "all")
    {
        ViewData["Section"] = "Setovi";
        ViewData["SectionController"] = "SetEntries";
        ViewData["PageTitle"] = "Evidencija setova";
        ViewData["SelectedFilter"] = exercise;

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

        return View(setEntries
            .OrderByDescending(x => x.Session?.SessionDate)
            .ThenBy(x => x.SetNumber));
    }

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
}
