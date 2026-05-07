using GymApp.Models;

namespace GymApp.Data;

public class MockGymRepository : IGymRepository
{
    private readonly List<UserProfile> _users = new();
    private readonly List<TrainingProgram> _programs = new();
    private readonly List<Exercise> _exercises = new();
    private readonly List<WorkoutSession> _sessions = new();
    private readonly List<SetEntry> _setEntries = new();
    private readonly List<BodyMeasurement> _measurements = new();
    private readonly List<GymLocation> _locations = new();
    private readonly List<ProgramExercise> _programExercises = new();
    private readonly List<Coach> _coaches = new();

    public MockGymRepository()
    {
        Seed();
    }

    public IReadOnlyList<UserProfile> Users => _users;
    public IReadOnlyList<TrainingProgram> Programs => _programs;
    public IReadOnlyList<Exercise> Exercises => _exercises;
    public IReadOnlyList<WorkoutSession> Sessions => _sessions;
    public IReadOnlyList<SetEntry> SetEntries => _setEntries;
    public IReadOnlyList<BodyMeasurement> Measurements => _measurements;
    public IReadOnlyList<GymLocation> Locations => _locations;
    public IReadOnlyList<ProgramExercise> ProgramExercises => _programExercises;
    public IReadOnlyList<Coach> Coaches => _coaches;

    public UserProfile? GetUser(Guid id) => _users.FirstOrDefault(x => x.Id == id);
    public TrainingProgram? GetProgram(Guid id) => _programs.FirstOrDefault(x => x.Id == id);
    public Exercise? GetExercise(Guid id) => _exercises.FirstOrDefault(x => x.Id == id);
    public WorkoutSession? GetSession(Guid id) => _sessions.FirstOrDefault(x => x.Id == id);
    public SetEntry? GetSetEntry(Guid id) => _setEntries.FirstOrDefault(x => x.Id == id);
    public BodyMeasurement? GetMeasurement(Guid id) => _measurements.FirstOrDefault(x => x.Id == id);
    public GymLocation? GetLocation(Guid id) => _locations.FirstOrDefault(x => x.Id == id);
    public ProgramExercise? GetProgramExercise(Guid id) => _programExercises.FirstOrDefault(x => x.Id == id);
    public Coach? GetCoach(Guid id) => _coaches.FirstOrDefault(x => x.Id == id);

    public void AddCoach(Coach coach)
    {
        _coaches.Add(coach);
    }

    public void UpdateCoach(Coach coach)
    {
        var existing = _coaches.FirstOrDefault(x => x.Id == coach.Id);
        if (existing is null)
        {
            return;
        }

        existing.FirstName = coach.FirstName;
        existing.LastName = coach.LastName;
        existing.Email = coach.Email;
        existing.Specialty = coach.Specialty;
    }

    private void Seed()
    {
        var cityCenter = new GymLocation { Id = Guid.NewGuid(), Name = "City Gym", City = "Zagreb", Capacity = 220 };
        var eastSide = new GymLocation { Id = Guid.NewGuid(), Name = "East Fitness", City = "Zagreb", Capacity = 140 };

        _locations.AddRange([cityCenter, eastSide]);

        var benchPress = CreateExercise("Bench Press", ExerciseCategory.Strength, "Prsa", "Sipka", true);
        var squat = CreateExercise("Back Squat", ExerciseCategory.Strength, "Noge", "Sipka", true);
        var deadlift = CreateExercise("Deadlift", ExerciseCategory.Strength, "Ledja", "Sipka", true);
        var pullup = CreateExercise("Pull-Up", ExerciseCategory.Hypertrophy, "Ledja", "Sipka", true);
        var shoulderPress = CreateExercise("Shoulder Press", ExerciseCategory.Hypertrophy, "Ramena", "Bucice", true);
        var plank = CreateExercise("Plank", ExerciseCategory.Mobility, "Core", "Vlastita tezina", false);

        _exercises.AddRange([benchPress, squat, deadlift, pullup, shoulderPress, plank]);

        var beginnersStrength = CreateProgram("Pocetnicka snaga", "Povecanje osnovne snage", 8, WorkoutDifficulty.Medium, "Marko Horvat");
        var upperLower = CreateProgram("Upper/Lower", "Hipertrofija i volumen", 10, WorkoutDifficulty.Hard, "Ana Simic");
        var fatLoss = CreateProgram("Cut Program", "Redukcija masti", 6, WorkoutDifficulty.Medium, "Luka Kovac");

        _programs.AddRange([beginnersStrength, upperLower, fatLoss]);

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

        _users.AddRange([ivan, petra, luka]);

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

        _coaches.AddRange([
            new Coach
            {
                Id = Guid.NewGuid(),
                FirstName = "Marko",
                LastName = "Horvat",
                Email = "marko.horvat@example.com",
                Specialty = "Strength",
                CreatedAt = DateTime.UtcNow
            },
            new Coach
            {
                Id = Guid.NewGuid(),
                FirstName = "Ana",
                LastName = "Simic",
                Email = "ana.simic@example.com",
                Specialty = "Hypertrophy",
                CreatedAt = DateTime.UtcNow
            },
            new Coach
            {
                Id = Guid.NewGuid(),
                FirstName = "Luka",
                LastName = "Kovac",
                Email = "luka.kovac@example.com",
                Specialty = "Cardio",
                CreatedAt = DateTime.UtcNow
            }
        ]);
    }

    private static Exercise CreateExercise(string name, ExerciseCategory category, string muscle, string equipment, bool isCompound)
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

    private static TrainingProgram CreateProgram(string name, string goal, int weeks, WorkoutDifficulty difficulty, string coachName)
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

    private static UserProfile CreateUser(string firstName, string lastName, string email, DateTime dob, double heightCm, double weightKg)
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

    private void ConnectProgramExercise(TrainingProgram program, Exercise exercise, int dayOfWeek, int targetSets, int targetReps)
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
        _programExercises.Add(link);
    }

    private WorkoutSession CreateSession(UserProfile user, TrainingProgram program, GymLocation gym, DateTime sessionDate, int duration, string notes, int rating)
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
        _sessions.Add(session);
        return session;
    }

    private void AddSetEntry(WorkoutSession session, Exercise exercise, int setNo, int reps, double weight)
    {
        var entry = new SetEntry
        {
            Id = Guid.NewGuid(),
            Session = session,
            Exercise = exercise,
            SetNumber = setNo,
            Repetitions = reps,
            WeightKg = Round2(weight)
        };

        session.SetEntries.Add(entry);
        session.TotalVolumeKg = Round2(session.TotalVolumeKg + (reps * entry.WeightKg));
        _setEntries.Add(entry);
    }

    private void AddMeasurement(UserProfile user, DateTime date, double weight, double bodyFat)
    {
        var measurement = new BodyMeasurement
        {
            Id = Guid.NewGuid(),
            User = user,
            RecordedAt = date,
            BodyWeightKg = Round2(weight),
            BodyFatPercentage = Round2(bodyFat)
        };

        user.Measurements.Add(measurement);
        _measurements.Add(measurement);
    }

    private static double Round2(double value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
