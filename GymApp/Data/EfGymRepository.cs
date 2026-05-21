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

    public void AddUser(UserProfile user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void UpdateUser(UserProfile user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void DeleteUser(Guid id)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == id);
        if (user is null)
        {
            return;
        }

        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    public void AddProgram(TrainingProgram program)
    {
        _context.Programs.Add(program);
        _context.SaveChanges();
    }

    public void UpdateProgram(TrainingProgram program)
    {
        _context.Programs.Update(program);
        _context.SaveChanges();
    }

    public void DeleteProgram(Guid id)
    {
        var program = _context.Programs.FirstOrDefault(x => x.Id == id);
        if (program is null)
        {
            return;
        }

        _context.Programs.Remove(program);
        _context.SaveChanges();
    }

    public void AddExercise(Exercise exercise)
    {
        _context.Exercises.Add(exercise);
        _context.SaveChanges();
    }

    public void UpdateExercise(Exercise exercise)
    {
        _context.Exercises.Update(exercise);
        _context.SaveChanges();
    }

    public void DeleteExercise(Guid id)
    {
        var exercise = _context.Exercises.FirstOrDefault(x => x.Id == id);
        if (exercise is null)
        {
            return;
        }

        _context.Exercises.Remove(exercise);
        _context.SaveChanges();
    }

    public void AddSession(WorkoutSession session)
    {
        _context.Sessions.Add(session);
        _context.SaveChanges();
    }

    public void UpdateSession(WorkoutSession session)
    {
        _context.Sessions.Update(session);
        _context.SaveChanges();
    }

    public void DeleteSession(Guid id)
    {
        var session = _context.Sessions.FirstOrDefault(x => x.Id == id);
        if (session is null)
        {
            return;
        }

        _context.Sessions.Remove(session);
        _context.SaveChanges();
    }

    public void AddSetEntry(SetEntry setEntry)
    {
        _context.SetEntries.Add(setEntry);
        _context.SaveChanges();
    }

    public void UpdateSetEntry(SetEntry setEntry)
    {
        _context.SetEntries.Update(setEntry);
        _context.SaveChanges();
    }

    public void DeleteSetEntry(Guid id)
    {
        var setEntry = _context.SetEntries.FirstOrDefault(x => x.Id == id);
        if (setEntry is null)
        {
            return;
        }

        _context.SetEntries.Remove(setEntry);
        _context.SaveChanges();
    }

    public void AddMeasurement(BodyMeasurement measurement)
    {
        _context.Measurements.Add(measurement);
        _context.SaveChanges();
    }

    public void UpdateMeasurement(BodyMeasurement measurement)
    {
        _context.Measurements.Update(measurement);
        _context.SaveChanges();
    }

    public void DeleteMeasurement(Guid id)
    {
        var measurement = _context.Measurements.FirstOrDefault(x => x.Id == id);
        if (measurement is null)
        {
            return;
        }

        _context.Measurements.Remove(measurement);
        _context.SaveChanges();
    }

    public void AddLocation(GymLocation location)
    {
        _context.Locations.Add(location);
        _context.SaveChanges();
    }

    public void UpdateLocation(GymLocation location)
    {
        _context.Locations.Update(location);
        _context.SaveChanges();
    }

    public void DeleteLocation(Guid id)
    {
        var location = _context.Locations.FirstOrDefault(x => x.Id == id);
        if (location is null)
        {
            return;
        }

        _context.Locations.Remove(location);
        _context.SaveChanges();
    }

    public void AddProgramExercise(ProgramExercise programExercise)
    {
        _context.ProgramExercises.Add(programExercise);
        _context.SaveChanges();
    }

    public void UpdateProgramExercise(ProgramExercise programExercise)
    {
        _context.ProgramExercises.Update(programExercise);
        _context.SaveChanges();
    }

    public void DeleteProgramExercise(Guid id)
    {
        var programExercise = _context.ProgramExercises.FirstOrDefault(x => x.Id == id);
        if (programExercise is null)
        {
            return;
        }

        _context.ProgramExercises.Remove(programExercise);
        _context.SaveChanges();
    }

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

    public void DeleteCoach(Guid id)
    {
        var coach = _context.Coaches.FirstOrDefault(x => x.Id == id);
        if (coach is null)
        {
            return;
        }

        _context.Coaches.Remove(coach);
        _context.SaveChanges();
    }
}
