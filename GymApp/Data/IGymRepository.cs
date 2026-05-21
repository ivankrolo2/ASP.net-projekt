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

    void AddUser(UserProfile user);
    void UpdateUser(UserProfile user);
    void DeleteUser(Guid id);

    void AddProgram(TrainingProgram program);
    void UpdateProgram(TrainingProgram program);
    void DeleteProgram(Guid id);

    void AddExercise(Exercise exercise);
    void UpdateExercise(Exercise exercise);
    void DeleteExercise(Guid id);

    void AddSession(WorkoutSession session);
    void UpdateSession(WorkoutSession session);
    void DeleteSession(Guid id);

    void AddSetEntry(SetEntry setEntry);
    void UpdateSetEntry(SetEntry setEntry);
    void DeleteSetEntry(Guid id);

    void AddMeasurement(BodyMeasurement measurement);
    void UpdateMeasurement(BodyMeasurement measurement);
    void DeleteMeasurement(Guid id);

    void AddLocation(GymLocation location);
    void UpdateLocation(GymLocation location);
    void DeleteLocation(Guid id);

    void AddProgramExercise(ProgramExercise programExercise);
    void UpdateProgramExercise(ProgramExercise programExercise);
    void DeleteProgramExercise(Guid id);

    void AddCoach(Coach coach);
    void UpdateCoach(Coach coach);
    void DeleteCoach(Guid id);
}
