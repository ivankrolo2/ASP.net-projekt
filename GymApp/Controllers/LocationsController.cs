using GymApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class LocationsController : Controller
{
    private readonly IGymRepository _repository;

    public LocationsController(IGymRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string city = "all")
    {
        ViewData["Section"] = "Lokacije";
        ViewData["SectionController"] = "Locations";
        ViewData["PageTitle"] = "Sve gym lokacije";
        ViewData["SelectedFilter"] = city;

        var locations = _repository.Locations.AsEnumerable();
        if (city != "all")
        {
            locations = locations.Where(x => string.Equals(x.City, city, StringComparison.OrdinalIgnoreCase));
        }

        return View(locations.OrderBy(x => x.City).ThenBy(x => x.Name));
    }

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
}
