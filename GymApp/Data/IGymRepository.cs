using GymApp.Models;

namespace GymApp.Data;

public interface IGymRepository
{
    IReadOnlyList<UserProfile> Users { get; }
    IReadOnlyList<TrainingProgram> Programs { get; }
    IReadOnlyList<Exercise> Exercises { get; }
    IReadOnlyList<WorkoutSession> Sessions { get; }
    IReadOnlyList<SetEntry> SetEntries { get; }
    IReadOnlyList<BodyMeasurement> Measurements { get; }
    IReadOnlyList<GymLocation> Locations { get; }
    IReadOnlyList<ProgramExercise> ProgramExercises { get; }
    IReadOnlyList<Coach> Coaches { get; }

    UserProfile? GetUser(Guid id);
    TrainingProgram? GetProgram(Guid id);
    Exercise? GetExercise(Guid id);
    WorkoutSession? GetSession(Guid id);
    SetEntry? GetSetEntry(Guid id);
    BodyMeasurement? GetMeasurement(Guid id);
    GymLocation? GetLocation(Guid id);
    ProgramExercise? GetProgramExercise(Guid id);
    Coach? GetCoach(Guid id);

    void AddCoach(Coach coach);
    void UpdateCoach(Coach coach);
}
