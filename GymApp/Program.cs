using GymApp.Models;

var builder = WebApplication.CreateBuilder(args);

SeedAndRunLinqQueries();

builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

static Exercise CreateExercise(string name, ExerciseCategory category, string muscle, string equipment, bool isCompound)
{
    return new Exercise
    {
        Id = Guid.NewGuid(),
        Name = name,
        Description = $"{name} - osnovna vjezba za {muscle}",
        Category = category,
        PrimaryMuscleGroup = muscle,
        Equipment = equipment,
        IsCompound = isCompound,
        CreatedAt = DateTime.UtcNow
    };
}

static TrainingProgram CreateProgram(string name, string goal, int weeks, WorkoutDifficulty difficulty, string coachName)
{
    return new TrainingProgram
    {
        Id = Guid.NewGuid(),
        Name = name,
        Goal = goal,
        Weeks = weeks,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        Difficulty = difficulty,
        CoachName = coachName
    };
}

static UserProfile CreateUser(string firstName, string lastName, string email, DateTime dob, double heightCm, double weightKg)
{
    return new UserProfile
    {
        Id = Guid.NewGuid(),
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        DateOfBirth = dob,
        HeightCm = heightCm,
        WeightKg = weightKg,
        CreatedAt = DateTime.UtcNow
    };
}

static void ConnectProgramExercise(TrainingProgram program, Exercise exercise, int dayOfWeek, int targetSets, int targetReps)
{
    var link = new ProgramExercise
    {
        Id = Guid.NewGuid(),
        Program = program,
        Exercise = exercise,
        DayOfWeek = dayOfWeek,
        TargetSets = targetSets,
        TargetReps = targetReps
    };

    program.ProgramExercises.Add(link);
    exercise.ProgramExercises.Add(link);
}

static WorkoutSession CreateSession(UserProfile user, TrainingProgram program, GymLocation gym, DateTime sessionDate, int duration, string notes, int rating)
{
    var session = new WorkoutSession
    {
        Id = Guid.NewGuid(),
        User = user,
        Program = program,
        GymLocation = gym,
        SessionDate = sessionDate,
        DurationMinutes = duration,
        Notes = notes,
        Rating = rating
    };

    user.Sessions.Add(session);
    return session;
}

static void AddSetEntry(WorkoutSession session, Exercise exercise, int setNo, int reps, double weight)
{
    var entry = new SetEntry
    {
        Id = Guid.NewGuid(),
        Session = session,
        Exercise = exercise,
        SetNumber = setNo,
        Repetitions = reps,
        WeightKg = weight
    };

    session.SetEntries.Add(entry);
    session.TotalVolumeKg += reps * weight;
}

static void AddMeasurement(UserProfile user, DateTime date, double weight, double bodyFat)
{
    var measurement = new BodyMeasurement
    {
        Id = Guid.NewGuid(),
        User = user,
        RecordedAt = date,
        BodyWeightKg = weight,
        BodyFatPercentage = bodyFat
    };

    user.Measurements.Add(measurement);
}

static void SeedAndRunLinqQueries()
{
    var cityCenter = new GymLocation { Id = Guid.NewGuid(), Name = "City Gym", City = "Zagreb", Capacity = 220 };
    var eastSide = new GymLocation { Id = Guid.NewGuid(), Name = "East Fitness", City = "Zagreb", Capacity = 140 };

    var benchPress = CreateExercise("Bench Press", ExerciseCategory.Strength, "Prsa", "Sipka", true);
    var squat = CreateExercise("Back Squat", ExerciseCategory.Strength, "Noge", "Sipka", true);
    var deadlift = CreateExercise("Deadlift", ExerciseCategory.Strength, "Ledja", "Sipka", true);
    var pullup = CreateExercise("Pull-Up", ExerciseCategory.Hypertrophy, "Ledja", "Sipka", true);
    var shoulderPress = CreateExercise("Shoulder Press", ExerciseCategory.Hypertrophy, "Ramena", "Bucice", true);
    var plank = CreateExercise("Plank", ExerciseCategory.Mobility, "Core", "Vlastita tezina", false);

    var beginnersStrength = CreateProgram("Pocetnicka snaga", "Povecanje osnovne snage", 8, WorkoutDifficulty.Medium, "Marko Horvat");
    var upperLower = CreateProgram("Upper/Lower", "Hipertrofija i volumen", 10, WorkoutDifficulty.Hard, "Ana Simic");
    var fatLoss = CreateProgram("Cut Program", "Redukcija masti", 6, WorkoutDifficulty.Medium, "Luka Kovac");

    ConnectProgramExercise(beginnersStrength, benchPress, 1, 4, 6);
    ConnectProgramExercise(beginnersStrength, squat, 2, 4, 6);
    ConnectProgramExercise(beginnersStrength, deadlift, 3, 3, 5);
    ConnectProgramExercise(upperLower, benchPress, 1, 4, 8);
    ConnectProgramExercise(upperLower, pullup, 1, 4, 10);
    ConnectProgramExercise(upperLower, shoulderPress, 2, 4, 10);
    ConnectProgramExercise(fatLoss, squat, 2, 3, 10);
    ConnectProgramExercise(fatLoss, plank, 3, 3, 60);

    var ivan = CreateUser("Ivan", "Krolo", "ivan@example.com", new DateTime(2001, 5, 10), 184.0, 89.0);
    var petra = CreateUser("Petra", "Novak", "petra@example.com", new DateTime(1999, 8, 21), 170.0, 66.0);
    var luka = CreateUser("Luka", "Peric", "luka@example.com", new DateTime(2000, 11, 3), 178.0, 82.0);

    ivan.Programs.Add(beginnersStrength);
    petra.Programs.Add(upperLower);
    luka.Programs.Add(fatLoss);

    var ivanS1 = CreateSession(ivan, beginnersStrength, cityCenter, new DateTime(2026, 3, 2), 70, "Dobar trening", 5);
    AddSetEntry(ivanS1, benchPress, 1, 6, 70.0);
    AddSetEntry(ivanS1, benchPress, 2, 6, 72.5);
    AddSetEntry(ivanS1, squat, 1, 6, 90.0);

    var ivanS2 = CreateSession(ivan, beginnersStrength, cityCenter, new DateTime(2026, 3, 9), 75, "Napredak na cucnju", 5);
    AddSetEntry(ivanS2, squat, 1, 6, 95.0);
    AddSetEntry(ivanS2, squat, 2, 6, 95.0);
    AddSetEntry(ivanS2, deadlift, 1, 5, 115.0);

    var petraS1 = CreateSession(petra, upperLower, eastSide, new DateTime(2026, 3, 3), 65, "Solidan upper day", 4);
    AddSetEntry(petraS1, benchPress, 1, 8, 45.0);
    AddSetEntry(petraS1, pullup, 1, 8, 0.0);
    AddSetEntry(petraS1, shoulderPress, 1, 10, 20.0);

    var petraS2 = CreateSession(petra, upperLower, eastSide, new DateTime(2026, 3, 10), 68, "Bolja forma", 5);
    AddSetEntry(petraS2, benchPress, 1, 8, 47.5);
    AddSetEntry(petraS2, pullup, 1, 9, 0.0);
    AddSetEntry(petraS2, shoulderPress, 1, 10, 22.5);

    var lukaS1 = CreateSession(luka, fatLoss, eastSide, new DateTime(2026, 3, 4), 55, "Kardio nakon treninga", 4);
    AddSetEntry(lukaS1, squat, 1, 10, 70.0);
    AddSetEntry(lukaS1, plank, 1, 1, 0.0);

    var lukaS2 = CreateSession(luka, fatLoss, cityCenter, new DateTime(2026, 3, 11), 58, "Umor, ali odradeno", 4);
    AddSetEntry(lukaS2, squat, 1, 10, 72.5);
    AddSetEntry(lukaS2, plank, 1, 1, 0.0);

    AddMeasurement(ivan, new DateTime(2026, 3, 1), 89.0, 19.0);
    AddMeasurement(ivan, new DateTime(2026, 3, 15), 88.0, 18.4);
    AddMeasurement(petra, new DateTime(2026, 3, 1), 66.0, 24.0);
    AddMeasurement(petra, new DateTime(2026, 3, 15), 65.2, 23.3);
    AddMeasurement(luka, new DateTime(2026, 3, 1), 82.0, 21.0);
    AddMeasurement(luka, new DateTime(2026, 3, 15), 80.9, 20.1);

    var users = new List<UserProfile> { ivan, petra, luka };
    var sessions = users.SelectMany(u => u.Sessions).ToList();

    var topSessionsByVolume = sessions
        .OrderByDescending(s => s.TotalVolumeKg)
        .Take(3)
        .ToList();

    var exerciseUsage = sessions
        .SelectMany(s => s.SetEntries)
        .GroupBy(se => se.Exercise?.Name ?? "Unknown")
        .Select(g => new { ExerciseName = g.Key, SetCount = g.Count(), TotalReps = g.Sum(x => x.Repetitions) })
        .OrderByDescending(x => x.SetCount)
        .ToList();

    var bodyweightChanges = users
        .Where(u => u.Measurements.Count >= 2)
        .Select(u =>
        {
            var ordered = u.Measurements.OrderBy(m => m.RecordedAt).ToList();
            var first = ordered.First();
            var last = ordered.Last();
            return new
            {
                FullName = $"{u.FirstName} {u.LastName}",
                FirstWeight = first.BodyWeightKg,
                LastWeight = last.BodyWeightKg,
                Diff = last.BodyWeightKg - first.BodyWeightKg
            };
        })
        .OrderBy(x => x.Diff)
        .ToList();

    var usersPerProgram = users
        .SelectMany(u => u.Programs.Select(p => new { ProgramName = p.Name, UserName = $"{u.FirstName} {u.LastName}" }))
        .GroupBy(x => x.ProgramName)
        .Select(g => new { ProgramName = g.Key, UserCount = g.Count() })
        .ToList();

    Console.WriteLine("=== LINQ rezultati: Evidencija treninga ===");
    Console.WriteLine("Top 3 treninga po volumenu (kg):");
    foreach (var session in topSessionsByVolume)
    {
        var sessionDateText = session.SessionDate.ToString("dd.MM.yyyy");
        Console.WriteLine($"- {session.User?.FirstName} {session.User?.LastName}: {session.TotalVolumeKg} kg ({sessionDateText})");
    }

    Console.WriteLine("\nKoristenje vjezbi (broj setova / ukupno ponavljanja):");
    foreach (var item in exerciseUsage)
    {
        Console.WriteLine($"- {item.ExerciseName}: {item.SetCount} setova / {item.TotalReps} ponavljanja");
    }

    Console.WriteLine("\nPromjena tjelesne tezine po korisniku:");
    foreach (var item in bodyweightChanges)
    {
        Console.WriteLine($"- {item.FullName}: {item.FirstWeight} kg -> {item.LastWeight} kg (promjena {item.Diff} kg)");
    }

    Console.WriteLine("\nBroj korisnika po programu:");
    foreach (var item in usersPerProgram)
    {
        Console.WriteLine($"- {item.ProgramName}: {item.UserCount} korisnik(a)");
    }

    Console.WriteLine("=== Kraj LINQ rezultata ===\n");
}