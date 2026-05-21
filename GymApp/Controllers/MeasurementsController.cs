using GymApp.Data;
using GymApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymApp.Controllers;

public class MeasurementsController : Controller
{
    private readonly IGymRepository _repository;

    public MeasurementsController(IGymRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("mjerenja/{user?}")]
    public IActionResult Index(string user = "all", string q = "")
    {
        ViewData["Section"] = "Mjerenja";
        ViewData["SectionController"] = "Measurements";
        ViewData["PageTitle"] = "Mjerenja tijela";
        ViewData["SelectedFilter"] = user;
        ViewData["Search"] = q;

        var measurements = FilterMeasurements(user, q);

        return View(measurements.OrderByDescending(x => x.RecordedAt));
    }

    [HttpGet("mjerenja/pretraga")]
    public IActionResult Search(string user = "all", string q = "")
    {
        var measurements = FilterMeasurements(user, q)
            .OrderByDescending(x => x.RecordedAt);

        return PartialView("_List", measurements);
    }

    [HttpGet("mjerenja/detalji/{id:guid}")]
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

    [HttpGet("mjerenja/novi")]
    public IActionResult Create()
    {
        ViewData["Section"] = "Mjerenja";
        ViewData["SectionController"] = "Measurements";
        ViewData["PageTitle"] = "Novo mjerenje";
        PopulateUsersSelectList();
        return View(new BodyMeasurement());
    }

    [HttpPost("mjerenja/novi")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BodyMeasurement measurement)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Mjerenja";
            ViewData["SectionController"] = "Measurements";
            ViewData["PageTitle"] = "Novo mjerenje";
            PopulateUsersSelectList(measurement.UserId);
            return View(measurement);
        }

        if (measurement.Id == Guid.Empty)
        {
            measurement.Id = Guid.NewGuid();
        }

        _repository.AddMeasurement(measurement);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("mjerenja/uredi/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var measurement = _repository.GetMeasurement(id);
        if (measurement is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Mjerenja";
        ViewData["SectionController"] = "Measurements";
        ViewData["PageTitle"] = $"Uredi mjerenje";
        PopulateUsersSelectList(measurement.UserId);
        return View(measurement);
    }

    [HttpPost("mjerenja/uredi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, BodyMeasurement measurement)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Section"] = "Mjerenja";
            ViewData["SectionController"] = "Measurements";
            ViewData["PageTitle"] = "Uredi mjerenje";
            PopulateUsersSelectList(measurement.UserId);
            return View(measurement);
        }

        var existing = _repository.GetMeasurement(id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.UserId = measurement.UserId;
        existing.RecordedAt = measurement.RecordedAt;
        existing.BodyWeightKg = measurement.BodyWeightKg;
        existing.BodyFatPercentage = measurement.BodyFatPercentage;

        _repository.UpdateMeasurement(existing);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("mjerenja/obrisi/{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var measurement = _repository.GetMeasurement(id);
        if (measurement is null)
        {
            return NotFound();
        }

        ViewData["Section"] = "Mjerenja";
        ViewData["SectionController"] = "Measurements";
        ViewData["PageTitle"] = "Obrisi mjerenje";
        return View(measurement);
    }

    [HttpPost("mjerenja/obrisi/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        var measurement = _repository.GetMeasurement(id);
        if (measurement is null)
        {
            return NotFound();
        }

        _repository.DeleteMeasurement(id);
        return RedirectToAction(nameof(Index));
    }

    private IEnumerable<BodyMeasurement> FilterMeasurements(string user, string q)
    {
        var measurements = _repository.Measurements.AsEnumerable();
        measurements = user switch
        {
            "ivan" => measurements.Where(x => string.Equals(x.User?.FirstName, "Ivan", StringComparison.OrdinalIgnoreCase)),
            "petra" => measurements.Where(x => string.Equals(x.User?.FirstName, "Petra", StringComparison.OrdinalIgnoreCase)),
            "luka" => measurements.Where(x => string.Equals(x.User?.FirstName, "Luka", StringComparison.OrdinalIgnoreCase)),
            _ => measurements
        };

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim();
            measurements = measurements.Where(x =>
                ($"{x.User?.FirstName} {x.User?.LastName}").Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.RecordedAt.ToString("dd.MM.yyyy").Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return measurements;
    }

    private void PopulateUsersSelectList(Guid? selectedId = null)
    {
        var items = _repository.Users
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new { x.Id, Name = $"{x.FirstName} {x.LastName}" });

        ViewData["Users"] = new SelectList(items, "Id", "Name", selectedId);
    }
}
