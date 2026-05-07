---
name: edit-form
description: "Use when: creating edit/create forms for an entity with GET/POST actions, validation, and Razor form views. Keywords: edit form, create form, POST, validation, Razor form."
argument-hint: "Entity name and fields to edit"
---

# Edit/Create form skill

## When to Use
- Need a new edit or create form page
- Need GET/POST actions with validation

## Procedure
1. Add GET action to load the form (new or existing entity).
2. Add POST action with `[ValidateAntiForgeryToken]` and `ModelState` checks.
3. Persist changes via repository or `DbContext`.
4. Create Razor view(s) with inputs and validation messages.
5. Redirect to a list/details page after successful save.
