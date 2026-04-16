using GymApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace GymApp.Controllers;

public class MeasurementsController : Controller
{
    private readonly IGymRepository _repository;

    public MeasurementsController(IGymRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string user = "all")
    {
        ViewData["Section"] = "Mjerenja";
        ViewData["SectionController"] = "Measurements";
        ViewData["PageTitle"] = "Mjerenja tijela";
        ViewData["SelectedFilter"] = user;

        var measurements = _repository.Measurements.AsEnumerable();
        measurements = user switch
        {
            "ivan" => measurements.Where(x => string.Equals(x.User?.FirstName, "Ivan", StringComparison.OrdinalIgnoreCase)),
            "petra" => measurements.Where(x => string.Equals(x.User?.FirstName, "Petra", StringComparison.OrdinalIgnoreCase)),
            "luka" => measurements.Where(x => string.Equals(x.User?.FirstName, "Luka", StringComparison.OrdinalIgnoreCase)),
            _ => measurements
        };

        return View(measurements.OrderByDescending(x => x.RecordedAt));
    }

    public IActionResult Details(Guid id)
    {
        var measurement = _repository.GetMeasurement(id);
        if (measurement is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Mjerenja";
        ViewData["SectionController"] = "Measurements";
        ViewData["PageTitle"] = $"Mjerenje: {measurement.User?.FirstName} {measurement.User?.LastName}";
        return View(measurement);
    }
}
