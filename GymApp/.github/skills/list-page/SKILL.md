---
name: list-page
description: "Use when: creating a new list page (Index) for an entity with controller action, routing, and Razor view table. Keywords: list page, index view, controller action, routing, Razor table."
argument-hint: "Entity name and filter requirements"
---

# List page skill

## When to Use
- Need a new list/index page for an entity
- Need a new controller action that renders a table

## Procedure
1. Choose the entity and data source (repository or DbContext).
2. Add an `Index` action and route for the list page.
3. Create `Views/<Controller>/Index.cshtml` with a table/list.
4. Add optional filters using query parameters.
5. Add link(s) to details or edit actions.
