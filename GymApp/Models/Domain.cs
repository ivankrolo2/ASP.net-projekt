namespace GymApp.Models;

public enum ExerciseCategory
{
    Strength = 0,
    Hypertrophy = 1,
    Cardio = 2,
    Mobility = 3
}

public enum WorkoutDifficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public class UserProfile
{
    public Guid Id { get; set; } = Guid.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<WorkoutSession> Sessions { get; set; } = new();
    public List<TrainingProgram> Programs { get; set; } = new();
    public List<BodyMeasurement> Measurements { get; set; } = new();
}

public class TrainingProgram
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public int Weeks { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public WorkoutDifficulty Difficulty { get; set; } = WorkoutDifficulty.Medium;
    public string CoachName { get; set; } = string.Empty;
    public List<ProgramExercise> ProgramExercises { get; set; } = new();
}

public class Exercise
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ExerciseCategory Category { get; set; } = ExerciseCategory.Strength;
    public string PrimaryMuscleGroup { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public bool IsCompound { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<ProgramExercise> ProgramExercises { get; set; } = new();
}

public class WorkoutSession
{
    public Guid Id { get; set; } = Guid.Empty;
    public UserProfile? User { get; set; }
    public TrainingProgram? Program { get; set; }
    public DateTime SessionDate { get; set; } = DateTime.MinValue;
    public int DurationMinutes { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int Rating { get; set; }
    public double TotalVolumeKg { get; set; }
    public List<SetEntry> SetEntries { get; set; } = new();
    public GymLocation? GymLocation { get; set; }
}

public class SetEntry
{
    public Guid Id { get; set; } = Guid.Empty;
    public WorkoutSession? Session { get; set; }
    public Exercise? Exercise { get; set; }
    public int SetNumber { get; set; }
    public int Repetitions { get; set; }
    public double WeightKg { get; set; }
}

public class BodyMeasurement
{
    public Guid Id { get; set; } = Guid.Empty;
    public UserProfile? User { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.MinValue;
    public double BodyWeightKg { get; set; }
    public double BodyFatPercentage { get; set; }
}

public class GymLocation
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Capacity { get; set; }
}

public class ProgramExercise
{
    public Guid Id { get; set; } = Guid.Empty;
    public TrainingProgram? Program { get; set; }
    public Exercise? Exercise { get; set; }
    public int DayOfWeek { get; set; } = 1;
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
}