const DATASET = {
  sample_mflix: {
    movies: [
      {
        _id: "m1",
        title: "Interstellar",
        year: 2014,
        rating: 8.7,
        runtime: 169,
        genres: ["Adventure", "Drama", "Sci-Fi"],
        director: "Christopher Nolan",
      },
      {
        _id: "m2",
        title: "Inception",
        year: 2010,
        rating: 8.8,
        runtime: 148,
        genres: ["Action", "Sci-Fi"],
        director: "Christopher Nolan",
      },
      {
        _id: "m3",
        title: "Arrival",
        year: 2016,
        rating: 7.9,
        runtime: 116,
        genres: ["Drama", "Sci-Fi"],
        director: "Denis Villeneuve",
      },
      {
        _id: "m4",
        title: "Dune",
        year: 2021,
        rating: 8.1,
        runtime: 155,
        genres: ["Adventure", "Sci-Fi"],
        director: "Denis Villeneuve",
      },
      {
        _id: "m5",
        title: "The Dark Knight",
        year: 2008,
        rating: 9.0,
        runtime: 152,
        genres: ["Action", "Crime"],
        director: "Christopher Nolan",
      },
      {
        _id: "m6",
        title: "Blade Runner 2049",
        year: 2017,
        rating: 8.0,
        runtime: 164,
        genres: ["Drama", "Mystery", "Sci-Fi"],
        director: "Denis Villeneuve",
      },
    ],
    users: [
      {
        _id: "u1",
        name: "Anita Rao",
        email: "anita.rao@example.com",
        plan: "premium",
        active: true,
        lastLogin: "2026-02-09T08:45:00Z",
        tags: ["mobile", "beta"],
      },
      {
        _id: "u2",
        name: "Liam Carter",
        email: "liam.carter@example.com",
        plan: "standard",
        active: false,
        lastLogin: "2026-01-23T14:11:00Z",
        tags: ["web"],
      },
      {
        _id: "u3",
        name: "Nora Silva",
        email: "nora.silva@example.com",
        plan: "premium",
        active: true,
        lastLogin: "2026-02-10T17:31:00Z",
        tags: ["ios", "referral"],
      },
      {
        _id: "u4",
        name: "Dev Malhotra",
        email: "dev.malhotra@example.com",
        plan: "free",
        active: true,
        lastLogin: "2026-02-01T05:02:00Z",
        tags: [],
      },
    ],
  },
  sales: {
    orders: [
      {
        _id: "o1",
        customerId: "c1",
        status: "paid",
        amount: 1250.45,
        createdAt: "2026-02-01T10:20:00Z",
        items: 3,
      },
      {
        _id: "o2",
        customerId: "c3",
        status: "processing",
        amount: 220.0,
        createdAt: "2026-02-08T11:00:00Z",
        items: 1,
      },
      {
        _id: "o3",
        customerId: "c2",
        status: "cancelled",
        amount: 97.99,
        createdAt: "2026-01-28T16:15:00Z",
        items: 2,
      },
      {
        _id: "o4",
        customerId: "c1",
        status: "paid",
        amount: 450.75,
        createdAt: "2026-02-09T09:40:00Z",
        items: 2,
      },
    ],
    customers: [
      {
        _id: "c1",
        name: "Solara Tech",
        tier: "enterprise",
        region: "US",
        active: true,
        joinedAt: "2025-11-02T00:00:00Z",
      },
      {
        _id: "c2",
        name: "Metro Supplies",
        tier: "standard",
        region: "IN",
        active: true,
        joinedAt: "2025-08-16T00:00:00Z",
      },
      {
        _id: "c3",
        name: "Northwind Retail",
        tier: "standard",
        region: "UK",
        active: false,
        joinedAt: "2024-12-20T00:00:00Z",
      },
    ],
  },
};

const state = {
  database: "sample_mflix",
  collection: "movies",
  selectedIndex: 0,
  activeTab: "documents",
  shownDocuments: [],
};

const dom = {
  collectionTree: document.getElementById("collectionTree"),
  dbName: document.getElementById("dbName"),
  collectionName: document.getElementById("collectionName"),
  tabButtons: Array.from(document.querySelectorAll(".tab")),
  queryControls: document.getElementById("queryControls"),
  filterInput: document.getElementById("filterInput"),
  projectionInput: document.getElementById("projectionInput"),
  sortInput: document.getElementById("sortInput"),
  limitInput: document.getElementById("limitInput"),
  skipInput: document.getElementById("skipInput"),
  findButton: document.getElementById("findButton"),
  resetButton: document.getElementById("resetButton"),
  documentsView: document.getElementById("documentsView"),
  documentsBody: document.getElementById("documentsBody"),
  placeholderView: document.getElementById("placeholderView"),
  placeholderTitle: document.getElementById("placeholderTitle"),
  documentPreview: document.getElementById("documentPreview"),
  fieldSummary: document.getElementById("fieldSummary"),
  statusText: document.getElementById("statusText"),
  resultCount: document.getElementById("resultCount"),
};

bootstrap();

function bootstrap() {
  renderCollectionTree();
  bindEvents();
  runQuery();
}

function bindEvents() {
  dom.findButton.addEventListener("click", runQuery);
  dom.resetButton.addEventListener("click", () => {
    dom.filterInput.value = "{ }";
    dom.projectionInput.value = "{ }";
    dom.sortInput.value = "{ }";
    dom.limitInput.value = "20";
    dom.skipInput.value = "0";
    setStatus("Filters reset");
    runQuery();
  });

  dom.tabButtons.forEach((button) => {
    button.addEventListener("click", () => {
      state.activeTab = button.dataset.tab;
      renderTabs();
    });
  });
}

function renderCollectionTree() {
  const fragment = document.createDocumentFragment();

  Object.entries(DATASET).forEach(([database, collections]) => {
    const wrapper = document.createElement("section");
    wrapper.className = "db-group";

    const label = document.createElement("h2");
    label.className = "db-label";
    label.textContent = database;
    wrapper.appendChild(label);

    Object.keys(collections).forEach((collection) => {
      const button = document.createElement("button");
      button.type = "button";
      button.className = "collection-item";
      if (state.database === database && state.collection === collection) {
        button.classList.add("active");
      }
      button.textContent = collection;
      button.addEventListener("click", () => {
        state.database = database;
        state.collection = collection;
        state.selectedIndex = 0;
        renderCollectionTree();
        runQuery();
      });
      wrapper.appendChild(button);
    });

    fragment.appendChild(wrapper);
  });

  dom.collectionTree.innerHTML = "";
  dom.collectionTree.appendChild(fragment);
}

function renderTabs() {
  dom.tabButtons.forEach((button) => {
    button.classList.toggle("active", button.dataset.tab === state.activeTab);
  });

  const isDocuments = state.activeTab === "documents";
  dom.queryControls.classList.toggle("hidden", !isDocuments);
  dom.documentsView.classList.toggle("hidden", !isDocuments);
  dom.placeholderView.classList.toggle("hidden", isDocuments);

  if (!isDocuments) {
    const label = state.activeTab
      .split("-")
      .map((part) => part[0].toUpperCase() + part.slice(1))
      .join(" ");
    dom.placeholderTitle.textContent = label;
    setStatus(`${label} section selected`);
  } else {
    setStatus("Documents section selected");
  }
}

function runQuery() {
  dom.dbName.textContent = state.database;
  dom.collectionName.textContent = state.collection;

  let filter;
  let projection;
  let sort;

  try {
    filter = parseJsonInput(dom.filterInput.value, "Filter");
    projection = parseJsonInput(dom.projectionInput.value, "Projection");
    sort = parseJsonInput(dom.sortInput.value, "Sort");
  } catch (error) {
    setStatus(error.message, true);
    return;
  }

  const limit = sanitizePositiveInteger(dom.limitInput.value, 20);
  const skip = sanitizePositiveInteger(dom.skipInput.value, 0);

  const source = DATASET[state.database][state.collection];
  const filtered = source.filter((document) => matchesFilter(document, filter));
  const sorted = sortDocuments(filtered, sort);
  const sliced = sorted.slice(skip, skip + limit);
  const projected = sliced.map((document) => applyProjection(document, projection));

  state.shownDocuments = projected;
  state.selectedIndex = 0;
  renderDocumentsTable();

  const shownCount = projected.length;
  dom.resultCount.textContent = `${shownCount} of ${filtered.length} documents`;
  setStatus(`Query executed on ${state.database}.${state.collection}`);
  renderTabs();
}

function renderDocumentsTable() {
  dom.documentsBody.innerHTML = "";

  if (!state.shownDocuments.length) {
    const row = document.createElement("tr");
    const cell = document.createElement("td");
    cell.colSpan = 2;
    cell.textContent = "No documents match the current query.";
    row.appendChild(cell);
    dom.documentsBody.appendChild(row);
    dom.documentPreview.textContent = "{ }";
    dom.fieldSummary.innerHTML = "";
    return;
  }

  state.shownDocuments.forEach((document, index) => {
    const row = document.createElement("tr");
    row.classList.toggle("active", index === state.selectedIndex);
    row.addEventListener("click", () => {
      state.selectedIndex = index;
      renderDocumentsTable();
    });

    const idCell = document.createElement("td");
    idCell.textContent = String(document._id ?? "(no _id)");

    const docCell = document.createElement("td");
    docCell.textContent = minifyDocument(document);

    row.append(idCell, docCell);
    dom.documentsBody.appendChild(row);
  });

  renderDetails(state.shownDocuments[state.selectedIndex]);
}

function renderDetails(document) {
  dom.documentPreview.textContent = JSON.stringify(document, null, 2);

  const summary = Object.entries(document).map(([field, value]) => {
    const type = Array.isArray(value) ? "array" : value === null ? "null" : typeof value;
    return `${field}: ${type}`;
  });

  dom.fieldSummary.innerHTML = "";
  summary.forEach((entry) => {
    const item = document.createElement("li");
    item.textContent = entry;
    dom.fieldSummary.appendChild(item);
  });
}

function parseJsonInput(raw, label) {
  const text = raw.trim();
  if (!text) {
    return {};
  }

  try {
    const parsed = JSON.parse(text);
    if (parsed && typeof parsed === "object" && !Array.isArray(parsed)) {
      return parsed;
    }
    throw new Error(`${label} must be a JSON object`);
  } catch (error) {
    if (error.message.includes("must be a JSON object")) {
      throw error;
    }
    throw new Error(`${label} is not valid JSON`);
  }
}

function sanitizePositiveInteger(raw, fallback) {
  const parsed = Number(raw);
  if (!Number.isFinite(parsed) || parsed < 0) {
    return fallback;
  }
  return Math.floor(parsed);
}

function matchesFilter(document, filter) {
  return Object.entries(filter).every(([field, expected]) => {
    if (field.startsWith("$")) {
      return true;
    }

    const actual = getValueByPath(document, field);

    if (expected && typeof expected === "object" && !Array.isArray(expected)) {
      return Object.entries(expected).every(([operator, value]) =>
        evaluateOperator(actual, operator, value, expected)
      );
    }

    return isDeepEqual(actual, expected);
  });
}

function evaluateOperator(actual, operator, value, criteria) {
  switch (operator) {
    case "$gt":
      return actual > value;
    case "$gte":
      return actual >= value;
    case "$lt":
      return actual < value;
    case "$lte":
      return actual <= value;
    case "$eq":
      return isDeepEqual(actual, value);
    case "$ne":
      return !isDeepEqual(actual, value);
    case "$in":
      return Array.isArray(value) && value.some((entry) => isDeepEqual(entry, actual));
    case "$nin":
      return Array.isArray(value) && value.every((entry) => !isDeepEqual(entry, actual));
    case "$regex": {
      const pattern = String(value);
      const flags = String(criteria.$options ?? "");
      const regex = new RegExp(pattern, flags);
      return typeof actual === "string" && regex.test(actual);
    }
    case "$options":
      return true;
    default:
      return isDeepEqual(actual, criteria);
  }
}

function sortDocuments(documents, sortSpec) {
  const entries = Object.entries(sortSpec).filter(([, direction]) => Number(direction) !== 0);
  if (!entries.length) {
    return [...documents];
  }

  return [...documents].sort((left, right) => {
    for (const [field, direction] of entries) {
      const dir = Number(direction) >= 0 ? 1 : -1;
      const a = getValueByPath(left, field);
      const b = getValueByPath(right, field);

      if (a === b) {
        continue;
      }
      if (a === undefined) {
        return 1;
      }
      if (b === undefined) {
        return -1;
      }
      return a > b ? dir : -dir;
    }
    return 0;
  });
}

function applyProjection(document, projection) {
  const entries = Object.entries(projection);
  if (!entries.length) {
    return deepClone(document);
  }

  const includes = entries.filter(([, value]) => value === 1 || value === true).map(([key]) => key);
  const excludes = entries.filter(([, value]) => value === 0 || value === false).map(([key]) => key);

  if (includes.length) {
    const result = {};
    includes.forEach((field) => {
      const value = getValueByPath(document, field);
      if (value !== undefined) {
        setValueByPath(result, field, value);
      }
    });

    if (!Object.prototype.hasOwnProperty.call(projection, "_id") || projection._id === 1) {
      result._id = document._id;
    }

    return result;
  }

  const result = deepClone(document);
  excludes.forEach((field) => {
    removeValueByPath(result, field);
  });
  return result;
}

function minifyDocument(document) {
  const json = JSON.stringify(document);
  return json.length > 160 ? `${json.slice(0, 157)}...` : json;
}

function setStatus(message, isError = false) {
  dom.statusText.textContent = message;
  dom.statusText.classList.toggle("status-error", isError);
}

function getValueByPath(object, path) {
  return path.split(".").reduce((value, key) => (value == null ? undefined : value[key]), object);
}

function setValueByPath(object, path, value) {
  const parts = path.split(".");
  let current = object;

  parts.forEach((part, index) => {
    const isLast = index === parts.length - 1;
    if (isLast) {
      current[part] = value;
      return;
    }

    if (!current[part] || typeof current[part] !== "object") {
      current[part] = {};
    }
    current = current[part];
  });
}

function removeValueByPath(object, path) {
  const parts = path.split(".");
  let current = object;

  for (let i = 0; i < parts.length - 1; i += 1) {
    current = current?.[parts[i]];
    if (current == null) {
      return;
    }
  }

  delete current?.[parts[parts.length - 1]];
}

function isDeepEqual(left, right) {
  return JSON.stringify(left) === JSON.stringify(right);
}

function deepClone(value) {
  return JSON.parse(JSON.stringify(value));
}
