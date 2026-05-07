using GymApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Data;

public class EfGymRepository : IGymRepository
{
    private readonly GymDbContext _context;

    public EfGymRepository(GymDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<UserProfile> Users => _context.Users
        .AsNoTracking()
        .Include(x => x.Sessions)
        .Include(x => x.Programs)
        .Include(x => x.Measurements)
        .ToList();

    public IReadOnlyList<TrainingProgram> Programs => _context.Programs
        .AsNoTracking()
        .Include(x => x.ProgramExercises)
        .ThenInclude(x => x.Exercise)
        .Include(x => x.Users)
        .ToList();

    public IReadOnlyList<Exercise> Exercises => _context.Exercises
        .AsNoTracking()
        .Include(x => x.ProgramExercises)
        .ThenInclude(x => x.Program)
        .ToList();

    public IReadOnlyList<WorkoutSession> Sessions => _context.Sessions
        .AsNoTracking()
        .Include(x => x.User)
        .Include(x => x.Program)
        .Include(x => x.GymLocation)
        .Include(x => x.SetEntries)
        .ThenInclude(x => x.Exercise)
        .ToList();

    public IReadOnlyList<SetEntry> SetEntries => _context.SetEntries
        .AsNoTracking()
        .Include(x => x.Session)
        .Include(x => x.Exercise)
        .ToList();

    public IReadOnlyList<BodyMeasurement> Measurements => _context.Measurements
        .AsNoTracking()
        .Include(x => x.User)
        .ToList();

    public IReadOnlyList<GymLocation> Locations => _context.Locations
        .AsNoTracking()
        .Include(x => x.Sessions)
        .ToList();

    public IReadOnlyList<ProgramExercise> ProgramExercises => _context.ProgramExercises
        .AsNoTracking()
        .Include(x => x.Program)
        .Include(x => x.Exercise)
        .ToList();

    public IReadOnlyList<Coach> Coaches => _context.Coaches
        .AsNoTracking()
        .ToList();

    public UserProfile? GetUser(Guid id) => _context.Users
        .AsNoTracking()
        .Include(x => x.Sessions)
        .Include(x => x.Programs)
        .Include(x => x.Measurements)
        .FirstOrDefault(x => x.Id == id);

    public TrainingProgram? GetProgram(Guid id) => _context.Programs
        .AsNoTracking()
        .Include(x => x.ProgramExercises)
        .ThenInclude(x => x.Exercise)
        .Include(x => x.Users)
        .FirstOrDefault(x => x.Id == id);

    public Exercise? GetExercise(Guid id) => _context.Exercises
        .AsNoTracking()
        .Include(x => x.ProgramExercises)
        .ThenInclude(x => x.Program)
        .FirstOrDefault(x => x.Id == id);

    public WorkoutSession? GetSession(Guid id) => _context.Sessions
        .AsNoTracking()
        .Include(x => x.User)
        .Include(x => x.Program)
        .Include(x => x.GymLocation)
        .Include(x => x.SetEntries)
        .ThenInclude(x => x.Exercise)
        .FirstOrDefault(x => x.Id == id);

    public SetEntry? GetSetEntry(Guid id) => _context.SetEntries
        .AsNoTracking()
        .Include(x => x.Session)
        .Include(x => x.Exercise)
        .FirstOrDefault(x => x.Id == id);

    public BodyMeasurement? GetMeasurement(Guid id) => _context.Measurements
        .AsNoTracking()
        .Include(x => x.User)
        .FirstOrDefault(x => x.Id == id);

    public GymLocation? GetLocation(Guid id) => _context.Locations
        .AsNoTracking()
        .Include(x => x.Sessions)
        .FirstOrDefault(x => x.Id == id);

    public ProgramExercise? GetProgramExercise(Guid id) => _context.ProgramExercises
        .AsNoTracking()
        .Include(x => x.Program)
        .Include(x => x.Exercise)
        .FirstOrDefault(x => x.Id == id);

    public Coach? GetCoach(Guid id) => _context.Coaches
        .AsNoTracking()
        .FirstOrDefault(x => x.Id == id);

    public void AddCoach(Coach coach)
    {
        _context.Coaches.Add(coach);
        _context.SaveChanges();
    }

    public void UpdateCoach(Coach coach)
    {
        _context.Coaches.Update(coach);
        _context.SaveChanges();
    }
}
