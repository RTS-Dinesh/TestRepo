# Angular 19 vs Angular 21

This document provides a high-level comparison and an upgrade guide between
Angular 19 and Angular 21. It avoids version-specific feature claims and
focuses on what typically changes across two major Angular releases so you can
evaluate your own project against the official release notes.

## Quick take

- **Angular 19** is two major versions behind Angular 21.
- **Angular 21** typically includes two cycles of new features, new defaults,
  and removal of APIs that were deprecated in or before Angular 19.
- If your goal is **stability**, you may stay on 19 until you are ready to
  absorb two major upgrades. If your goal is **new features and long-term
  support**, plan to move to 21.

## What usually changes across two major versions

Use this as a checklist when comparing 19 and 21 for your app:

1. **Breaking changes and removals**
   - Deprecated APIs in 19 may be removed by 21.
   - Long-deprecated behaviors or flags might be deleted.

2. **Tooling and build defaults**
   - CLI defaults can change (builders, dev-server behavior, optimization
     settings).
   - TypeScript and Node.js minimum versions often advance.

3. **Framework APIs**
   - New APIs or patterns are added; older patterns may be de-emphasized.
   - Migration schematics may refactor code to new idioms.

4. **Rendering and performance**
   - Rendering, hydration, or change detection improvements may land.
   - Compiler and bundler updates can alter output or build performance.

5. **Ecosystem compatibility**
   - Third-party libraries (RxJS, Angular Material, Nx, etc.) may require
     matching major versions.

## Upgrade path: 19 -> 20 -> 21

Angular upgrades are safest one major version at a time:

1. **Prepare**
   - Update Node.js and TypeScript to the versions required by Angular 19.
   - Ensure tests are passing on 19 before starting the upgrade.

2. **Upgrade to 20**
   - Run: `ng update @angular/core@20 @angular/cli@20`
   - Apply migrations and fix any breaking changes.

3. **Upgrade to 21**
   - Run: `ng update @angular/core@21 @angular/cli@21`
   - Apply migrations and fix any breaking changes.

4. **Validate**
   - Run unit, integration, and e2e tests.
   - Verify SSR or hydration behavior if you use it.

## Decision guide

Choose **Angular 19** if:
- Your app is stable and you want to minimize change.
- You rely on dependencies that are not yet verified on 21.

Choose **Angular 21** if:
- You want the latest features and the longest support window.
- You can budget time for two major migrations.

## Next steps

For accurate feature-level differences, consult:
- Angular 20 release notes
- Angular 21 release notes
- The Angular update guide for official migration steps

