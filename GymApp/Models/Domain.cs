using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime DateOfBirth { get; set; } = DateTime.MinValue;

    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<WorkoutSession> Sessions { get; set; } = new List<WorkoutSession>();
    public virtual ICollection<TrainingProgram> Programs { get; set; } = new List<TrainingProgram>();
    public virtual ICollection<BodyMeasurement> Measurements { get; set; } = new List<BodyMeasurement>();
}

public class TrainingProgram
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string Goal { get; set; } = string.Empty;

    public int Weeks { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public WorkoutDifficulty Difficulty { get; set; } = WorkoutDifficulty.Medium;

    [Required]
    [StringLength(120)]
    public string CoachName { get; set; } = string.Empty;

    public virtual ICollection<ProgramExercise> ProgramExercises { get; set; } = new List<ProgramExercise>();
    public virtual ICollection<UserProfile> Users { get; set; } = new List<UserProfile>();
}

public class Exercise
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(600)]
    public string Description { get; set; } = string.Empty;

    public ExerciseCategory Category { get; set; } = ExerciseCategory.Strength;

    [Required]
    [StringLength(120)]
    public string PrimaryMuscleGroup { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Equipment { get; set; } = string.Empty;

    public bool IsCompound { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<ProgramExercise> ProgramExercises { get; set; } = new List<ProgramExercise>();
}

public class WorkoutSession
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    public Guid UserId { get; set; }

    public Guid? ProgramId { get; set; }

    public Guid? GymLocationId { get; set; }

    public virtual UserProfile? User { get; set; }
    public virtual TrainingProgram? Program { get; set; }

    public DateTime SessionDate { get; set; } = DateTime.MinValue;
    public int DurationMinutes { get; set; }
    [StringLength(800)]
    public string Notes { get; set; } = string.Empty;
    public int Rating { get; set; }
    public double TotalVolumeKg { get; set; }
    public virtual ICollection<SetEntry> SetEntries { get; set; } = new List<SetEntry>();
    public virtual GymLocation? GymLocation { get; set; }
}

public class SetEntry
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    public Guid WorkoutSessionId { get; set; }

    [Required]
    public Guid ExerciseId { get; set; }

    public virtual WorkoutSession? Session { get; set; }
    public virtual Exercise? Exercise { get; set; }
    public int SetNumber { get; set; }
    public int Repetitions { get; set; }
    public double WeightKg { get; set; }
}

public class BodyMeasurement
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    public Guid UserId { get; set; }

    public virtual UserProfile? User { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.MinValue;
    public double BodyWeightKg { get; set; }
    public double BodyFatPercentage { get; set; }
}

public class GymLocation
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    [StringLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string City { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public virtual ICollection<WorkoutSession> Sessions { get; set; } = new List<WorkoutSession>();
}

public class ProgramExercise
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    public Guid TrainingProgramId { get; set; }

    [Required]
    public Guid ExerciseId { get; set; }

    public virtual TrainingProgram? Program { get; set; }
    public virtual Exercise? Exercise { get; set; }
    public int DayOfWeek { get; set; } = 1;
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
}

public class Coach
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(200)]
    public string Specialty { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}