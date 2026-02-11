# MongoDB Compass UI prototype

This folder contains a browser-based prototype that mimics the core
MongoDB Compass Documents workflow.

## Included features

- Database / collection explorer
- Collection tabs (Documents, Aggregations, Schema, Explain Plan, Indexes)
- Query controls:
  - `Filter` (JSON object)
  - `Projection` (JSON object)
  - `Sort` (JSON object)
  - `Limit`
  - `Skip`
- Documents table with selectable rows
- JSON details view + per-field type summary

## Run

Open `index.html` directly in your browser.

Optional (Python static server):

```bash
cd examples/mongodb-compass-ui
python3 -m http.server 8080
```

Then visit `http://localhost:8080`.

## Query examples

Filter movies by rating and year:

```json
{ "rating": { "$gt": 8 }, "year": { "$gte": 2010 } }
```

Sort by newest and highest rated:

```json
{ "year": -1, "rating": -1 }
```

Only show selected fields:

```json
{ "title": 1, "year": 1, "rating": 1, "_id": 0 }
```

Case-insensitive regex:

```json
{ "title": { "$regex": "dark", "$options": "i" } }
```
