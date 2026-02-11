# MongoDB Compass UI

This repository now includes a lightweight, runnable **MongoDB Compass-style UI
prototype** built with plain HTML, CSS, and JavaScript.

The goal is to demonstrate the core Compass workflow in a small, dependency-free
example:

- Database and collection explorer (left sidebar)
- Query bar for Filter / Projection / Sort / Limit / Skip
- Documents table view
- Document details panel (JSON + field summary)
- Compass-style tabs (Documents, Aggregations, Schema, Explain Plan, Indexes)

## Project structure

- `examples/mongodb-compass-ui/index.html` - App shell and layout
- `examples/mongodb-compass-ui/styles.css` - Compass-inspired dark UI styling
- `examples/mongodb-compass-ui/app.js` - Data model, filtering, sorting, and rendering
- `examples/mongodb-compass-ui/README.md` - Usage and feature notes

## Run locally

No build step is required.

1. Open `examples/mongodb-compass-ui/index.html` in your browser.
2. Select a collection from the left sidebar.
3. Enter JSON in the query fields and click **Find**.

You can also serve files with any static server if preferred.

## Example query snippets

Filter:
```json
{ "year": { "$gte": 2010 }, "rating": { "$gt": 8 } }
```

Sort:
```json
{ "rating": -1, "year": -1 }
```

Projection:
```json
{ "title": 1, "year": 1, "rating": 1, "_id": 1 }
```
