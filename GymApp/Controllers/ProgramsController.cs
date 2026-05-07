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
    public IActionResult Index(string difficulty = "all")
    {
        ViewData["Section"] = "Programi";
        ViewData["SectionController"] = "Programs";
        ViewData["PageTitle"] = "Lista programa";
        ViewData["SelectedFilter"] = difficulty;

        var programs = _repository.Programs.AsEnumerable();
        if (difficulty != "all" && Enum.TryParse<WorkoutDifficulty>(difficulty, true, out var parsedDifficulty))
        {
            programs = programs.Where(x => x.Difficulty == parsedDifficulty);
        }

        return View(programs.OrderBy(x => x.Name));
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
}
