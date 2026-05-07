# Routing semantic model

This sitemap lists each available URL, its controller/action, and the view used.

## Custom attribute routes

| URL pattern | Controller.Action | View |
| --- | --- | --- |
| /korisnici/{activity?} | Users.Index | Views/Users/Index.cshtml |
| /korisnici/detalji/{id:guid} | Users.Details | Views/Users/Details.cshtml |
| /programi/{difficulty?} | Programs.Index | Views/Programs/Index.cshtml |
| /programi/detalji/{id:guid} | Programs.Details | Views/Programs/Details.cshtml |
| /vjezbe/{category?} | Exercises.Index | Views/Exercises/Index.cshtml |
| /vjezbe/detalji/{id:guid} | Exercises.Details | Views/Exercises/Details.cshtml |
| /treninzi/{month?} | Sessions.Index | Views/Sessions/Index.cshtml |
| /treninzi/detalji/{id:guid} | Sessions.Details | Views/Sessions/Details.cshtml |
| /treneri | Coaches.Index | Views/Coaches/Index.cshtml |
| /treneri/novi | Coaches.Create | Views/Coaches/Create.cshtml |
| /treneri/uredi/{id:guid} | Coaches.Edit | Views/Coaches/Edit.cshtml |

## Conventional routes (default MVC)

| URL pattern | Controller.Action | View |
| --- | --- | --- |
| / | Home.Index | Views/Home/Index.cshtml |
| /Home/Index | Home.Index | Views/Home/Index.cshtml |
| /Home/Privacy | Home.Privacy | Views/Home/Privacy.cshtml |
| /Home/Error | Home.Error | Views/Shared/Error.cshtml |
| /Locations/Index?city={city} | Locations.Index | Views/Locations/Index.cshtml |
| /Locations/Details/{id} | Locations.Details | Views/Locations/Details.cshtml |
| /Measurements/Index?user={user} | Measurements.Index | Views/Measurements/Index.cshtml |
| /Measurements/Details/{id} | Measurements.Details | Views/Measurements/Details.cshtml |
| /ProgramExercises/Index?day={day} | ProgramExercises.Index | Views/ProgramExercises/Index.cshtml |
| /ProgramExercises/Details/{id} | ProgramExercises.Details | Views/ProgramExercises/Details.cshtml |
| /SetEntries/Index?exercise={exercise} | SetEntries.Index | Views/SetEntries/Index.cshtml |
| /SetEntries/Details/{id} | SetEntries.Details | Views/SetEntries/Details.cshtml |
