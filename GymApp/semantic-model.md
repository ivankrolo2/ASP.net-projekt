# Semantic DB model

## Overview
This document summarizes the domain models, key properties, and relationships used by EF Core.

## Models and main properties

### UserProfile
- Id (PK)
- FirstName, LastName, Email
- DateOfBirth, HeightCm, WeightKg, CreatedAt
- Relationships: Sessions (1-many), Programs (many-many), Measurements (1-many)

### TrainingProgram
- Id (PK)
- Name, Goal, Weeks, IsActive, CreatedAt, Difficulty, CoachName
- Relationships: ProgramExercises (1-many), Users (many-many)

### Exercise
- Id (PK)
- Name, Description, Category, PrimaryMuscleGroup, Equipment, IsCompound, CreatedAt
- Relationships: ProgramExercises (1-many)

### WorkoutSession
- Id (PK)
- UserId (FK), ProgramId (FK, nullable), GymLocationId (FK, nullable)
- SessionDate, DurationMinutes, Notes, Rating, TotalVolumeKg
- Relationships: User (many-1), Program (many-1), GymLocation (many-1), SetEntries (1-many)

### SetEntry
- Id (PK)
- WorkoutSessionId (FK), ExerciseId (FK)
- SetNumber, Repetitions, WeightKg
- Relationships: WorkoutSession (many-1), Exercise (many-1)

### BodyMeasurement
- Id (PK)
- UserId (FK)
- RecordedAt, BodyWeightKg, BodyFatPercentage
- Relationships: User (many-1)

### GymLocation
- Id (PK)
- Name, City, Capacity
- Relationships: Sessions (1-many)

### ProgramExercise
- Id (PK)
- TrainingProgramId (FK), ExerciseId (FK)
- DayOfWeek, TargetSets, TargetReps
- Relationships: TrainingProgram (many-1), Exercise (many-1)

### Enums
- ExerciseCategory: Strength, Hypertrophy, Cardio, Mobility
- WorkoutDifficulty: Easy, Medium, Hard

## Relationships summary
- UserProfile 1-many WorkoutSession
- UserProfile 1-many BodyMeasurement
- UserProfile many-many TrainingProgram (join table: UserProgram)
- TrainingProgram 1-many ProgramExercise
- Exercise 1-many ProgramExercise
- WorkoutSession 1-many SetEntry
- GymLocation 1-many WorkoutSession
- ProgramExercise many-1 TrainingProgram
- ProgramExercise many-1 Exercise
- SetEntry many-1 WorkoutSession
- SetEntry many-1 Exercise
- BodyMeasurement many-1 UserProfile
