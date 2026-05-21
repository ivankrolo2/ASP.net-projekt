using GymApp.Data;
using GymApp.Models;
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
    public IActionResult Index(string activity = "all", string q = "")
    {
        ViewData["Section"] = "Korisnici";
        ViewData["SectionController"] = "Users";
        ViewData["PageTitle"] = "Lista korisnika";
        ViewData["SelectedFilter"] = activity;
        ViewData["Search"] = q;

        var users = FilterUsers(activity, q);

        return View(users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName));
    }

    [HttpGet("korisnici/pretraga")]
    public IActionResult Search(string activity = "all", string q = "")
    {
        var users = FilterUsers(activity, q)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName);

        return PartialView("_List", users);
    }

    [HttpGet("korisnici/autocomplete")]
    public IActionResult Autocomplete(string q = "")
    {
        var query = q?.Trim() ?? string.Empty;
        var results = _repository.Users
            .Where(x =>
                string.IsNullOrWhiteSpace(query) ||
                x.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Take(20)
            .Select(x => new
            {
                id = x.Id,
                label = $"{x.FirstName} {x.LastName}",
                meta = x.Email
            });

        return Json(results);
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

    [HttpGet("korisnici/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Korisnici";
        ViewData["SectionController"] = "Users";
        ViewData["PageTitle"] = "Novi korisnik";
        return View(new UserProfile());
    }

    [HttpPost("korisnici/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserProfile user)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Korisnici";
            ViewData["SectionController"] = "Users";
            ViewData["PageTitle"] = "Novi korisnik";
            return View(user);
        }

        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }

        user.CreatedAt = DateTime.UtcNow;
        _repository.AddUser(user);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("korisnici/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var user = _repository.GetUser(id);
        if (user is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Korisnici";
        ViewData["SectionController"] = "Users";
        ViewData["PageTitle"] = $"Uredi: {user.FirstName} {user.LastName}";
        return View(user);
    }

    [HttpPost("korisnici/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, UserProfile user)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Korisnici";
            ViewData["SectionController"] = "Users";
            ViewData["PageTitle"] = "Uredi korisnika";
            return View(user);
        }

        var existing = _repository.GetUser(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        existing.DateOfBirth = user.DateOfBirth;
        existing.HeightCm = user.HeightCm;
        existing.WeightKg = user.WeightKg;

        _repository.UpdateUser(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("korisnici/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var user = _repository.GetUser(id);
        if (user is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Korisnici";
        ViewData["SectionController"] = "Users";
        ViewData["PageTitle"] = $"Obrisi: {user.FirstName} {user.LastName}";
        return View(user);
    }

    [HttpPost("korisnici/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var user = _repository.GetUser(id);
        if (user is null)
        {
            return NotFound();
        }

        _repository.DeleteUser(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<UserProfile> FilterUsers(string activity, string q)
    {
        var users = _repository.Users.AsEnumerable();
        users = activity switch
        {
            "high" => users.Where(x => x.Sessions.Count >= 2),
            "low" => users.Where(x => x.Sessions.Count <= 1),
            _ => users
        };

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            users = users.Where(x =>
                x.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Email.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return users;
    }
}
