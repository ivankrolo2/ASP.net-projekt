namespace GymApp.Models

open System
open System.Collections.Generic

type ExerciseCategory =
    | Strength = 0
    | Hypertrophy = 1
    | Cardio = 2
    | Mobility = 3

type WorkoutDifficulty =
    | Easy = 0
    | Medium = 1
    | Hard = 2

type UserProfile() =
    member val Id: Guid = Guid.Empty with get, set
    member val FirstName: string = "" with get, set
    member val LastName: string = "" with get, set
    member val Email: string = "" with get, set
    member val DateOfBirth: DateTime = DateTime.MinValue with get, set
    member val HeightCm: float = 0.0 with get, set
    member val WeightKg: float = 0.0 with get, set
    member val CreatedAt: DateTime = DateTime.UtcNow with get, set
    member val Sessions: List<WorkoutSession> = List<WorkoutSession>() with get, set
    member val Programs: List<TrainingProgram> = List<TrainingProgram>() with get, set
    member val Measurements: List<BodyMeasurement> = List<BodyMeasurement>() with get, set

and TrainingProgram() =
    member val Id: Guid = Guid.Empty with get, set
    member val Name: string = "" with get, set
    member val Goal: string = "" with get, set
    member val Weeks: int = 0 with get, set
    member val IsActive: bool = true with get, set
    member val CreatedAt: DateTime = DateTime.UtcNow with get, set
    member val Difficulty: WorkoutDifficulty = WorkoutDifficulty.Medium with get, set
    member val CoachName: string = "" with get, set
    member val ProgramExercises: List<ProgramExercise> = List<ProgramExercise>() with get, set

and Exercise() =
    member val Id: Guid = Guid.Empty with get, set
    member val Name: string = "" with get, set
    member val Description: string = "" with get, set
    member val Category: ExerciseCategory = ExerciseCategory.Strength with get, set
    member val PrimaryMuscleGroup: string = "" with get, set
    member val Equipment: string = "" with get, set
    member val IsCompound: bool = false with get, set
    member val CreatedAt: DateTime = DateTime.UtcNow with get, set
    member val ProgramExercises: List<ProgramExercise> = List<ProgramExercise>() with get, set

and WorkoutSession() =
    member val Id: Guid = Guid.Empty with get, set
    member val User: UserProfile | null = null with get, set
    member val Program: TrainingProgram | null = null with get, set
    member val SessionDate: DateTime = DateTime.MinValue with get, set
    member val DurationMinutes: int = 0 with get, set
    member val Notes: string = "" with get, set
    member val Rating: int = 0 with get, set
    member val TotalVolumeKg: float = 0.0 with get, set
    member val SetEntries: List<SetEntry> = List<SetEntry>() with get, set
    member val GymLocation: GymLocation | null = null with get, set

and SetEntry() =
    member val Id: Guid = Guid.Empty with get, set
    member val Session: WorkoutSession | null = null with get, set
    member val Exercise: Exercise | null = null with get, set
    member val SetNumber: int = 0 with get, set
    member val Repetitions: int = 0 with get, set
    member val WeightKg: float = 0.0 with get, set

and BodyMeasurement() =
    member val Id: Guid = Guid.Empty with get, set
    member val User: UserProfile | null = null with get, set
    member val RecordedAt: DateTime = DateTime.MinValue with get, set
    member val BodyWeightKg: float = 0.0 with get, set
    member val BodyFatPercentage: float = 0.0 with get, set

and GymLocation() =
    member val Id: Guid = Guid.Empty with get, set
    member val Name: string = "" with get, set
    member val City: string = "" with get, set
    member val Capacity: int = 0 with get, set

and ProgramExercise() =
    member val Id: Guid = Guid.Empty with get, set
    member val Program: TrainingProgram | null = null with get, set
    member val Exercise: Exercise | null = null with get, set
    member val DayOfWeek: int = 1 with get, set
    member val TargetSets: int = 0 with get, set
    member val TargetReps: int = 0 with get, set
