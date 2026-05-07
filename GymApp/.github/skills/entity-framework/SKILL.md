---
name: entity-framework
description: "Use when: adding or updating EF Core models, DbContext, relationships, or generating migrations and database updates. Keywords: EF Core, Entity Framework, DbContext, migration, database update."
argument-hint: "Describe the EF change or migration goal"
---

# Entity Framework skill

## When to Use
- Adding or changing EF Core entities and relationships
- Updating `DbContext` configuration
- Creating migrations or updating the database

## Procedure
1. Identify the model changes (classes, properties, relationships).
2. Update annotations or fluent API in `DbContext` as needed.
3. Add or update `DbSet<>` properties in `DbContext`.
4. Regenerate migrations:
   - `dotnet ef migrations add <Name>`
5. Apply changes to DB when required:
   - `dotnet ef database update`
6. Verify controllers/repositories still compile with the new model.
