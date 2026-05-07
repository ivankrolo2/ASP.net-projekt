using GymApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class UsersController : Controller
{
    private readonly IGymRepository _repository;

    public UsersController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("korisnici/{activity?}")]
    public IActionResult Index(string activity = "all")
    {
        ViewData["Section"] = "Korisnici";
        ViewData["SectionController"] = "Users";
        ViewData["PageTitle"] = "Lista korisnika";
        ViewData["SelectedFilter"] = activity;

        var users = _repository.Users.AsEnumerable();
        users = activity switch
        {
            "high" => users.Where(x => x.Sessions.Count >= 2),
            "low" => users.Where(x => x.Sessions.Count <= 1),
            _ => users
        };

        return View(users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName));
    }

    [HttpGet("korisnici/detalji/{id:guid}")]
    public IActionResult Details(Guid id)
    {
        var user = _repository.GetUser(id);
        if (user is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Korisnici";
        ViewData["SectionController"] = "Users";
        ViewData["PageTitle"] = $"Detalji: {user.FirstName} {user.LastName}";
        return View(user);
    }
}
