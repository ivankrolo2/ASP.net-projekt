using GymApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class ProgramExercisesController : Controller
{
    private readonly IGymRepository _repository;

    public ProgramExercisesController(IGymRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string day = "all")
    {
        ViewData["Section"] = "Program vjezbe";
        ViewData["SectionController"] = "ProgramExercises";
        ViewData["PageTitle"] = "Poveznice program-vjezba";
        ViewData["SelectedFilter"] = day;

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

        return View(programExercises
            .OrderBy(x => x.Program?.Name)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.Exercise?.Name));
    }

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
}
