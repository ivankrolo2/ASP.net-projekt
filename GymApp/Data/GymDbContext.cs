using GymApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Data;

public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }

    public DbSet<UserProfile> Users => Set<UserProfile>();
    public DbSet<TrainingProgram> Programs => Set<TrainingProgram>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WorkoutSession> Sessions => Set<WorkoutSession>();
    public DbSet<SetEntry> SetEntries => Set<SetEntry>();
    public DbSet<BodyMeasurement> Measurements => Set<BodyMeasurement>();
    public DbSet<GymLocation> Locations => Set<GymLocation>();
    public DbSet<ProgramExercise> ProgramExercises => Set<ProgramExercise>();
    public DbSet<Coach> Coaches => Set<Coach>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>()
            .HasMany(x => x.Programs)
            .WithMany(x => x.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserProgram",
                right => right.HasOne<TrainingProgram>().WithMany().HasForeignKey("TrainingProgramId"),
                left => left.HasOne<UserProfile>().WithMany().HasForeignKey("UserProfileId"));

        modelBuilder.Entity<WorkoutSession>()
            .HasMany(x => x.SetEntries)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.WorkoutSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProgramExercise>()
            .HasOne(x => x.Program)
            .WithMany(x => x.ProgramExercises)
            .HasForeignKey(x => x.TrainingProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProgramExercise>()
            .HasOne(x => x.Exercise)
            .WithMany(x => x.ProgramExercises)
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SetEntry>()
            .HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BodyMeasurement>()
            .HasOne(x => x.User)
            .WithMany(x => x.Measurements)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkoutSession>()
            .HasOne(x => x.User)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkoutSession>()
            .HasOne(x => x.GymLocation)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.GymLocationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
