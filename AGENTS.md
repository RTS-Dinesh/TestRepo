# AGENTS.md

## Cursor Cloud specific instructions

### Repository overview

This is a general-purpose "TestRepo" containing an Angular 21 application on the current branch along with isolated technology samples on other branches. The `main` branch contains only a bare `README.md`.

### Angular application

- **Framework:** Angular 21.1.5 with standalone components
- **Testing:** Vitest 4.x (via `@angular/build:unit-test` builder)
- **Linting:** ESLint via `@angular-eslint/schematics`
- **Package manager:** npm (see `package-lock.json`)

### Key commands

All commands run from the workspace root (`/workspace`):

| Task | Command |
|------|---------|
| Dev server | `ng serve` (port 4200) |
| Build | `ng build` |
| Test | `ng test --watch=false` |
| Lint | `ng lint` |

### Caveats

- Angular CLI (`@angular/cli`) must be installed globally: `npm install -g @angular/cli`
- The dev server binds to `localhost:4200` by default. Use `--host 0.0.0.0` to expose externally.
- Production build output goes to `dist/angular-app/`.
- No backend services, databases, or Docker containers are required.
