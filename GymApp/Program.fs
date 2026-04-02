namespace GymApp

#nowarn "20"

open System
open System.Collections.Generic
open System.Linq
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open GymApp.Models

module Program =
    let exitCode = 0

    let private createExercise name category muscle equipment isCompound =
        let exercise = Exercise()
        exercise.Id <- Guid.NewGuid()
        exercise.Name <- name
        exercise.Description <- $"{name} - osnovna vjezba za {muscle}"
        exercise.Category <- category
        exercise.PrimaryMuscleGroup <- muscle
        exercise.Equipment <- equipment
        exercise.IsCompound <- isCompound
        exercise.CreatedAt <- DateTime.UtcNow
        exercise

    let private createProgram name goal weeks difficulty coachName =
        let program = TrainingProgram()
        program.Id <- Guid.NewGuid()
        program.Name <- name
        program.Goal <- goal
        program.Weeks <- weeks
        program.IsActive <- true
        program.CreatedAt <- DateTime.UtcNow
        program.Difficulty <- difficulty
        program.CoachName <- coachName
        program

    let private createUser firstName lastName email dob heightCm weightKg =
        let user = UserProfile()
        user.Id <- Guid.NewGuid()
        user.FirstName <- firstName
        user.LastName <- lastName
        user.Email <- email
        user.DateOfBirth <- dob
        user.HeightCm <- heightCm
        user.WeightKg <- weightKg
        user.CreatedAt <- DateTime.UtcNow
        user

    let private connectProgramExercise (program: TrainingProgram) (exercise: Exercise) dayOfWeek targetSets targetReps =
        let link = ProgramExercise()
        link.Id <- Guid.NewGuid()
        link.Program <- program
        link.Exercise <- exercise
        link.DayOfWeek <- dayOfWeek
        link.TargetSets <- targetSets
        link.TargetReps <- targetReps
        program.ProgramExercises.Add(link)
        exercise.ProgramExercises.Add(link)

    let private createSession (user: UserProfile) (program: TrainingProgram) (gym: GymLocation) sessionDate duration notes rating =
        let session = WorkoutSession()
        session.Id <- Guid.NewGuid()
        session.User <- user
        session.Program <- program
        session.GymLocation <- gym
        session.SessionDate <- sessionDate
        session.DurationMinutes <- duration
        session.Notes <- notes
        session.Rating <- rating
        user.Sessions.Add(session)
        session

    let private addSetEntry (session: WorkoutSession) (exercise: Exercise) setNo reps weight =
        let entry = SetEntry()
        entry.Id <- Guid.NewGuid()
        entry.Session <- session
        entry.Exercise <- exercise
        entry.SetNumber <- setNo
        entry.Repetitions <- reps
        entry.WeightKg <- weight
        session.SetEntries.Add(entry)
        session.TotalVolumeKg <- session.TotalVolumeKg + (float reps * weight)

    let private addMeasurement (user: UserProfile) date weight bodyFat =
        let measurement = BodyMeasurement()
        measurement.Id <- Guid.NewGuid()
        measurement.User <- user
        measurement.RecordedAt <- date
        measurement.BodyWeightKg <- weight
        measurement.BodyFatPercentage <- bodyFat
        user.Measurements.Add(measurement)

    let private seedAndRunLinqQueries () =
        let cityCenter = GymLocation(Id = Guid.NewGuid(), Name = "City Gym", City = "Zagreb", Capacity = 220)
        let eastSide = GymLocation(Id = Guid.NewGuid(), Name = "East Fitness", City = "Zagreb", Capacity = 140)

        let benchPress = createExercise "Bench Press" ExerciseCategory.Strength "Prsa" "Sipka" true
        let squat = createExercise "Back Squat" ExerciseCategory.Strength "Noge" "Sipka" true
        let deadlift = createExercise "Deadlift" ExerciseCategory.Strength "Ledja" "Sipka" true
        let pullup = createExercise "Pull-Up" ExerciseCategory.Hypertrophy "Ledja" "Sipka" true
        let shoulderPress = createExercise "Shoulder Press" ExerciseCategory.Hypertrophy "Ramena" "Bucice" true
        let plank = createExercise "Plank" ExerciseCategory.Mobility "Core" "Vlastita tezina" false

        let beginnersStrength = createProgram "Pocetnicka snaga" "Povecanje osnovne snage" 8 WorkoutDifficulty.Medium "Marko Horvat"
        let upperLower = createProgram "Upper/Lower" "Hipertrofija i volumen" 10 WorkoutDifficulty.Hard "Ana Simic"
        let fatLoss = createProgram "Cut Program" "Redukcija masti" 6 WorkoutDifficulty.Medium "Luka Kovac"

        connectProgramExercise beginnersStrength benchPress 1 4 6
        connectProgramExercise beginnersStrength squat 2 4 6
        connectProgramExercise beginnersStrength deadlift 3 3 5
        connectProgramExercise upperLower benchPress 1 4 8
        connectProgramExercise upperLower pullup 1 4 10
        connectProgramExercise upperLower shoulderPress 2 4 10
        connectProgramExercise fatLoss squat 2 3 10
        connectProgramExercise fatLoss plank 3 3 60

        let ivan = createUser "Ivan" "Krolo" "ivan@example.com" (DateTime(2001, 5, 10)) 184.0 89.0
        let petra = createUser "Petra" "Novak" "petra@example.com" (DateTime(1999, 8, 21)) 170.0 66.0
        let luka = createUser "Luka" "Peric" "luka@example.com" (DateTime(2000, 11, 3)) 178.0 82.0

        ivan.Programs.Add(beginnersStrength)
        petra.Programs.Add(upperLower)
        luka.Programs.Add(fatLoss)

        let ivanS1 = createSession ivan beginnersStrength cityCenter (DateTime(2026, 3, 2)) 70 "Dobar trening" 5
        addSetEntry ivanS1 benchPress 1 6 70.0
        addSetEntry ivanS1 benchPress 2 6 72.5
        addSetEntry ivanS1 squat 1 6 90.0

        let ivanS2 = createSession ivan beginnersStrength cityCenter (DateTime(2026, 3, 9)) 75 "Napredak na cucnju" 5
        addSetEntry ivanS2 squat 1 6 95.0
        addSetEntry ivanS2 squat 2 6 95.0
        addSetEntry ivanS2 deadlift 1 5 115.0

        let petraS1 = createSession petra upperLower eastSide (DateTime(2026, 3, 3)) 65 "Solidan upper day" 4
        addSetEntry petraS1 benchPress 1 8 45.0
        addSetEntry petraS1 pullup 1 8 0.0
        addSetEntry petraS1 shoulderPress 1 10 20.0

        let petraS2 = createSession petra upperLower eastSide (DateTime(2026, 3, 10)) 68 "Bolja forma" 5
        addSetEntry petraS2 benchPress 1 8 47.5
        addSetEntry petraS2 pullup 1 9 0.0
        addSetEntry petraS2 shoulderPress 1 10 22.5

        let lukaS1 = createSession luka fatLoss eastSide (DateTime(2026, 3, 4)) 55 "Kardio nakon treninga" 4
        addSetEntry lukaS1 squat 1 10 70.0
        addSetEntry lukaS1 plank 1 1 0.0

        let lukaS2 = createSession luka fatLoss cityCenter (DateTime(2026, 3, 11)) 58 "Umor, ali odradeno" 4
        addSetEntry lukaS2 squat 1 10 72.5
        addSetEntry lukaS2 plank 1 1 0.0

        addMeasurement ivan (DateTime(2026, 3, 1)) 89.0 19.0
        addMeasurement ivan (DateTime(2026, 3, 15)) 88.0 18.4
        addMeasurement petra (DateTime(2026, 3, 1)) 66.0 24.0
        addMeasurement petra (DateTime(2026, 3, 15)) 65.2 23.3
        addMeasurement luka (DateTime(2026, 3, 1)) 82.0 21.0
        addMeasurement luka (DateTime(2026, 3, 15)) 80.9 20.1

        let users = List<UserProfile>([ ivan; petra; luka ])
        let sessions = users.SelectMany(fun u -> u.Sessions :> seq<WorkoutSession>).ToList()

        let topSessionsByVolume =
            sessions
                .OrderByDescending(fun s -> s.TotalVolumeKg)
                .Take(3)
                .ToList()

        let exerciseUsage =
            sessions
                .SelectMany(fun s -> s.SetEntries :> seq<SetEntry>)
                .GroupBy(fun se -> se.Exercise.Name)
                .Select(fun g -> (g.Key, g.Count(), g.Sum(fun x -> x.Repetitions)))
                .OrderByDescending(fun (_, setCount, _) -> setCount)
                .ToList()

        let bodyweightChanges =
            users
                .Where(fun u -> u.Measurements.Count >= 2)
                .Select(fun u ->
                    let ordered = u.Measurements.OrderBy(fun m -> m.RecordedAt).ToList()
                    let first = ordered.First()
                    let last = ordered.Last()
                    ($"{u.FirstName} {u.LastName}", first.BodyWeightKg, last.BodyWeightKg, last.BodyWeightKg - first.BodyWeightKg))
                .OrderBy(fun (_, _, _, diff) -> diff)
                .ToList()

        let usersPerProgram =
            users
                .SelectMany(fun u -> u.Programs.Select(fun p -> (p.Name, $"{u.FirstName} {u.LastName}")))
                .GroupBy(fun (programName, _) -> programName)
                .Select(fun g -> (g.Key, g.Count()))
                .ToList()

        Console.WriteLine("=== LINQ rezultati: Evidencija treninga ===")
        Console.WriteLine("Top 3 treninga po volumenu (kg):")
        for session in topSessionsByVolume do
            let sessionDateText = session.SessionDate.ToString("dd.MM.yyyy")
            Console.WriteLine($"- {session.User.FirstName} {session.User.LastName}: {session.TotalVolumeKg} kg ({sessionDateText})")

        Console.WriteLine("\nKoristenje vjezbi (broj setova / ukupno ponavljanja):")
        for (exerciseName, setCount, totalReps) in exerciseUsage do
            Console.WriteLine($"- {exerciseName}: {setCount} setova / {totalReps} ponavljanja")

        Console.WriteLine("\nPromjena tjelesne tezine po korisniku:")
        for (fullName, firstWeight, lastWeight, diff) in bodyweightChanges do
            Console.WriteLine($"- {fullName}: {firstWeight} kg -> {lastWeight} kg (promjena {diff} kg)")

        Console.WriteLine("\nBroj korisnika po programu:")
        for (programName, userCount) in usersPerProgram do
            Console.WriteLine($"- {programName}: {userCount} korisnik(a)")

        Console.WriteLine("=== Kraj LINQ rezultata ===\n")

    [<EntryPoint>]
    let main args =
        seedAndRunLinqQueries ()

        let builder = WebApplication.CreateBuilder(args)

        builder
            .Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation()

        builder.Services.AddRazorPages()

        let app = builder.Build()

        if not (builder.Environment.IsDevelopment()) then
            app.UseExceptionHandler("/Home/Error")
            app.UseHsts() |> ignore // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

        app.UseHttpsRedirection()

        app.UseStaticFiles()
        app.UseRouting()
        app.UseAuthorization()

        app.MapControllerRoute(name = "default", pattern = "{controller=Home}/{action=Index}/{id?}")

        app.MapRazorPages()

        app.Run()

        exitCode
