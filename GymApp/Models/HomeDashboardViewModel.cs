namespace GymApp.Models;

public class HomeDashboardViewModel
{
    public int UsersCount { get; set; }
    public int ProgramsCount { get; set; }
    public int SessionsCount { get; set; }
    public int ExercisesCount { get; set; }
    public IReadOnlyList<WorkoutSession> RecentSessions { get; set; } = [];
    public IReadOnlyList<ExerciseUsageStat> TopExercises { get; set; } = [];
}

public class ExerciseUsageStat
{
    public string ExerciseName { get; set; } = string.Empty;
    public int SetCount { get; set; }
    public int RepetitionCount { get; set; }
}
