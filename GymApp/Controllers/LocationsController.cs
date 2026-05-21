using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class LocationsController : Controller
{
    private readonly IGymRepository _repository;

    public LocationsController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("lokacije/{city?}")]
    public IActionResult Index(string city = "all", string q = "")
    {
        ViewData["Section"] = "Lokacije";
        ViewData["SectionController"] = "Locations";
        ViewData["PageTitle"] = "Sve gym lokacije";
        ViewData["SelectedFilter"] = city;
        ViewData["Search"] = q;

        var locations = FilterLocations(city, q);

        return View(locations.OrderBy(x => x.City).ThenBy(x => x.Name));
    }

    [HttpGet("lokacije/pretraga")]
    public IActionResult Search(string city = "all", string q = "")
    {
        var locations = FilterLocations(city, q)
            .OrderBy(x => x.City)
            .ThenBy(x => x.Name);

        return PartialView("_List", locations);
    }

    [HttpGet("lokacije/autocomplete")]
    public IActionResult Autocomplete(string q = "")
    {
        var query = q?.Trim() ?? string.Empty;
        var results = _repository.Locations
            .Where(x =>
                string.IsNullOrWhiteSpace(query) ||
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.City.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.City)
            .ThenBy(x => x.Name)
            .Take(20)
            .Select(x => new
            {
                id = x.Id,
                label = x.Name,
                meta = x.City
            });

        return Json(results);
    }

    [HttpGet("lokacije/detalji/{id:guid}")]
    public IActionResult Details(Guid id)
    {
        var location = _repository.GetLocation(id);
        if (location is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Lokacije";
        ViewData["SectionController"] = "Locations";
        ViewData["PageTitle"] = $"Lokacija: {location.Name}";
        return View(location);
    }

    [HttpGet("lokacije/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Lokacije";
        ViewData["SectionController"] = "Locations";
        ViewData["PageTitle"] = "Nova lokacija";
        return View(new GymLocation());
    }

    [HttpPost("lokacije/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(GymLocation location)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Lokacije";
            ViewData["SectionController"] = "Locations";
            ViewData["PageTitle"] = "Nova lokacija";
            return View(location);
        }

        if (location.Id == Guid.Empty)
        {
            location.Id = Guid.NewGuid();
        }

        _repository.AddLocation(location);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("lokacije/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var location = _repository.GetLocation(id);
        if (location is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Lokacije";
        ViewData["SectionController"] = "Locations";
        ViewData["PageTitle"] = $"Uredi: {location.Name}";
        return View(location);
    }

    [HttpPost("lokacije/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, GymLocation location)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Lokacije";
            ViewData["SectionController"] = "Locations";
            ViewData["PageTitle"] = "Uredi lokaciju";
            return View(location);
        }

        var existing = _repository.GetLocation(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = location.Name;
        existing.City = location.City;
        existing.Capacity = location.Capacity;

        _repository.UpdateLocation(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("lokacije/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var location = _repository.GetLocation(id);
        if (location is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Lokacije";
        ViewData["SectionController"] = "Locations";
        ViewData["PageTitle"] = $"Obrisi: {location.Name}";
        return View(location);
    }

    [HttpPost("lokacije/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var location = _repository.GetLocation(id);
        if (location is null)
        {
            return NotFound();
        }

        _repository.DeleteLocation(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<GymLocation> FilterLocations(string city, string q)
    {
        var locations = _repository.Locations.AsEnumerable();
        if (city != "all")
        {
            locations = locations.Where(x => string.Equals(x.City, city, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            locations = locations.Where(x =>
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.City.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return locations;
    }
}
