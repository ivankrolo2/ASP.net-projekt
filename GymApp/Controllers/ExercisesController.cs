using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class ExercisesController : Controller
{
    private readonly IGymRepository _repository;

    public ExercisesController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("vjezbe/{category?}")]
    public IActionResult Index(string category = "all")
    {
        ViewData["Section"] = "Vjezbe";
        ViewData["SectionController"] = "Exercises";
        ViewData["PageTitle"] = "Lista vjezbi";
        ViewData["SelectedFilter"] = category;

        var exercises = _repository.Exercises.AsEnumerable();
        if (category != "all" && Enum.TryParse<ExerciseCategory>(category, true, out var parsedCategory))
        {
            exercises = exercises.Where(x => x.Category == parsedCategory);
        }

        return View(exercises.OrderBy(x => x.Name));
    }

    [HttpGet("vjezbe/detalji/{id:guid}")]
    public IActionResult Details(Guid id)
    {
        var exercise = _repository.GetExercise(id);
        if (exercise is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Vjezbe";
        ViewData["SectionController"] = "Exercises";
        ViewData["PageTitle"] = $"Detalji: {exercise.Name}";
        return View(exercise);
    }
}
