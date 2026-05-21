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
    public IActionResult Index(string category = "all", string q = "")
    {
        ViewData["Section"] = "Vjezbe";
        ViewData["SectionController"] = "Exercises";
        ViewData["PageTitle"] = "Lista vjezbi";
        ViewData["SelectedFilter"] = category;
        ViewData["Search"] = q;

        var exercises = FilterExercises(category, q);

        return View(exercises.OrderBy(x => x.Name));
    }

    [HttpGet("vjezbe/pretraga")]
    public IActionResult Search(string category = "all", string q = "")
    {
        var exercises = FilterExercises(category, q)
            .OrderBy(x => x.Name);

        return PartialView("_List", exercises);
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

    [HttpGet("vjezbe/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Vjezbe";
        ViewData["SectionController"] = "Exercises";
        ViewData["PageTitle"] = "Nova vjezba";
        return View(new Exercise());
    }

    [HttpPost("vjezbe/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Exercise exercise)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Vjezbe";
            ViewData["SectionController"] = "Exercises";
            ViewData["PageTitle"] = "Nova vjezba";
            return View(exercise);
        }

        if (exercise.Id == Guid.Empty)
        {
            exercise.Id = Guid.NewGuid();
        }

        exercise.CreatedAt = DateTime.UtcNow;
        _repository.AddExercise(exercise);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("vjezbe/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var exercise = _repository.GetExercise(id);
        if (exercise is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Vjezbe";
        ViewData["SectionController"] = "Exercises";
        ViewData["PageTitle"] = $"Uredi: {exercise.Name}";
        return View(exercise);
    }

    [HttpPost("vjezbe/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, Exercise exercise)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Vjezbe";
            ViewData["SectionController"] = "Exercises";
            ViewData["PageTitle"] = "Uredi vjezbu";
            return View(exercise);
        }

        var existing = _repository.GetExercise(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = exercise.Name;
        existing.Description = exercise.Description;
        existing.Category = exercise.Category;
        existing.PrimaryMuscleGroup = exercise.PrimaryMuscleGroup;
        existing.Equipment = exercise.Equipment;
        existing.IsCompound = exercise.IsCompound;

        _repository.UpdateExercise(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("vjezbe/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var exercise = _repository.GetExercise(id);
        if (exercise is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Vjezbe";
        ViewData["SectionController"] = "Exercises";
        ViewData["PageTitle"] = $"Obrisi: {exercise.Name}";
        return View(exercise);
    }

    [HttpPost("vjezbe/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var exercise = _repository.GetExercise(id);
        if (exercise is null)
        {
            return NotFound();
        }

        _repository.DeleteExercise(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<Exercise> FilterExercises(string category, string q)
    {
        var exercises = _repository.Exercises.AsEnumerable();
        if (category != "all" && Enum.TryParse<ExerciseCategory>(category, true, out var parsedCategory))
        {
            exercises = exercises.Where(x => x.Category == parsedCategory);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            exercises = exercises.Where(x =>
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.PrimaryMuscleGroup.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Equipment.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return exercises;
    }
}
