# Usage Examples and Tutorials

## Table of Contents

1. [Overview](#overview)
2. [Quick Start Guide](#quick-start-guide)
3. [Authentication Examples](#authentication-examples)
4. [CRUD Operations](#crud-operations)
5. [Advanced Patterns](#advanced-patterns)
6. [Real-World Scenarios](#real-world-scenarios)
7. [Integration Examples](#integration-examples)
8. [Testing Examples](#testing-examples)
9. [Performance Optimization](#performance-optimization)
10. [Troubleshooting](#troubleshooting)

## Overview

This guide provides practical, real-world examples of how to use the APIs, functions, and components documented in this project. Each example is complete, tested, and ready to use.

---

## Quick Start Guide

### Getting Started in 5 Minutes

```bash
# 1. Install the package
npm install @example/api-client

# 2. Set up environment variables
echo "API_KEY=your-api-key-here" > .env

# 3. Create your first script
cat > quickstart.js << 'EOF'
const { ExampleAPI } = require('@example/api-client');

const client = new ExampleAPI({
  apiKey: process.env.API_KEY
});

async function main() {
  try {
    // Fetch data
    const users = await client.users.list({ limit: 5 });
    console.log('Users:', users);

    // Create new user
    const newUser = await client.users.create({
      name: 'John Doe',
      email: 'john@example.com'
    });
    console.log('Created user:', newUser);
  } catch (error) {
    console.error('Error:', error.message);
  }
}

main();
EOF

# 4. Run it
node quickstart.js
```

---

## Authentication Examples

### Example 1: Basic API Key Authentication

```javascript
const fetch = require('node-fetch');

const API_KEY = process.env.API_KEY;
const BASE_URL = 'https://api.example.com';

async function fetchWithAuth(endpoint) {
  const response = await fetch(`${BASE_URL}${endpoint}`, {
    headers: {
      'Authorization': `Bearer ${API_KEY}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  return response.json();
}

// Usage
fetchWithAuth('/api/v1/users')
  .then(data => console.log(data))
  .catch(err => console.error(err));
```

### Example 2: OAuth 2.0 Authentication Flow

```javascript
const express = require('express');
const axios = require('axios');
const crypto = require('crypto');

const app = express();

// Configuration
const config = {
  clientId: process.env.OAUTH_CLIENT_ID,
  clientSecret: process.env.OAUTH_CLIENT_SECRET,
  redirectUri: 'http://localhost:3000/callback',
  authorizationEndpoint: 'https://auth.example.com/authorize',
  tokenEndpoint: 'https://auth.example.com/token',
};

// Step 1: Initiate OAuth flow
app.get('/login', (req, res) => {
  // Generate state for CSRF protection
  const state = crypto.randomBytes(16).toString('hex');
  req.session.oauthState = state;

  // Build authorization URL
  const authUrl = new URL(config.authorizationEndpoint);
  authUrl.searchParams.append('client_id', config.clientId);
  authUrl.searchParams.append('redirect_uri', config.redirectUri);
  authUrl.searchParams.append('response_type', 'code');
  authUrl.searchParams.append('state', state);
  authUrl.searchParams.append('scope', 'read write');

  // Redirect user to authorization server
  res.redirect(authUrl.toString());
});

// Step 2: Handle callback
app.get('/callback', async (req, res) => {
  const { code, state } = req.query;

  // Verify state to prevent CSRF
  if (state !== req.session.oauthState) {
    return res.status(400).send('Invalid state parameter');
  }

  try {
    // Exchange authorization code for access token
    const tokenResponse = await axios.post(config.tokenEndpoint, {
      grant_type: 'authorization_code',
      code,
      redirect_uri: config.redirectUri,
      client_id: config.clientId,
      client_secret: config.clientSecret,
    });

    const { access_token, refresh_token, expires_in } = tokenResponse.data;

    // Store tokens securely (in production, use encrypted session or secure storage)
    req.session.accessToken = access_token;
    req.session.refreshToken = refresh_token;
    req.session.tokenExpiry = Date.now() + expires_in * 1000;

    res.redirect('/dashboard');
  } catch (error) {
    console.error('Token exchange failed:', error);
    res.status(500).send('Authentication failed');
  }
});

// Step 3: Use access token for API requests
app.get('/api/data', async (req, res) => {
  const { accessToken } = req.session;

  if (!accessToken) {
    return res.status(401).send('Not authenticated');
  }

  try {
    const apiResponse = await axios.get('https://api.example.com/data', {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    res.json(apiResponse.data);
  } catch (error) {
    if (error.response?.status === 401) {
      // Token expired, try to refresh
      return refreshToken(req, res);
    }
    res.status(500).send('API request failed');
  }
});

// Step 4: Refresh token when expired
async function refreshToken(req, res) {
  const { refreshToken } = req.session;

  if (!refreshToken) {
    return res.status(401).send('No refresh token available');
  }

  try {
    const tokenResponse = await axios.post(config.tokenEndpoint, {
      grant_type: 'refresh_token',
      refresh_token: refreshToken,
      client_id: config.clientId,
      client_secret: config.clientSecret,
    });

    const { access_token, expires_in } = tokenResponse.data;

    req.session.accessToken = access_token;
    req.session.tokenExpiry = Date.now() + expires_in * 1000;

    // Retry original request
    res.redirect(req.originalUrl);
  } catch (error) {
    console.error('Token refresh failed:', error);
    res.redirect('/login');
  }
}

app.listen(3000, () => {
  console.log('Server running on http://localhost:3000');
});
```

---

## CRUD Operations

### Example 3: Complete CRUD Implementation

```typescript
import axios, { AxiosInstance, AxiosError } from 'axios';

interface User {
  id: string;
  name: string;
  email: string;
  role: 'admin' | 'user' | 'moderator';
  createdAt: string;
  updatedAt: string;
}

interface CreateUserData {
  name: string;
  email: string;
  password: string;
  role?: 'admin' | 'user' | 'moderator';
}

interface UpdateUserData {
  name?: string;
  email?: string;
  role?: 'admin' | 'user' | 'moderator';
}

interface ListUsersParams {
  page?: number;
  limit?: number;
  sort?: string;
  order?: 'asc' | 'desc';
  search?: string;
}

interface ListUsersResponse {
  data: User[];
  pagination: {
    page: number;
    limit: number;
    total: number;
    totalPages: number;
  };
}

class UserAPI {
  private client: AxiosInstance;

  constructor(baseURL: string, apiKey: string) {
    this.client = axios.create({
      baseURL,
      headers: {
        'Authorization': `Bearer ${apiKey}`,
        'Content-Type': 'application/json',
      },
      timeout: 10000,
    });

    // Add response interceptor for error handling
    this.client.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        if (error.response) {
          // Server responded with error status
          const message = (error.response.data as any)?.error?.message || 'Request failed';
          throw new Error(`${error.response.status}: ${message}`);
        } else if (error.request) {
          // Request made but no response
          throw new Error('No response from server');
        } else {
          // Something else happened
          throw new Error('Request failed: ' + error.message);
        }
      }
    );
  }

  /**
   * List all users with optional filtering and pagination.
   */
  async list(params: ListUsersParams = {}): Promise<ListUsersResponse> {
    const response = await this.client.get<ListUsersResponse>('/api/v1/users', {
      params,
    });
    return response.data;
  }

  /**
   * Get a single user by ID.
   */
  async get(userId: string): Promise<User> {
    const response = await this.client.get<User>(`/api/v1/users/${userId}`);
    return response.data;
  }

  /**
   * Create a new user.
   */
  async create(userData: CreateUserData): Promise<User> {
    const response = await this.client.post<User>('/api/v1/users', userData);
    return response.data;
  }

  /**
   * Update an existing user.
   */
  async update(userId: string, userData: UpdateUserData): Promise<User> {
    const response = await this.client.put<User>(
      `/api/v1/users/${userId}`,
      userData
    );
    return response.data;
  }

  /**
   * Delete a user.
   */
  async delete(userId: string): Promise<void> {
    await this.client.delete(`/api/v1/users/${userId}`);
  }

  /**
   * Batch create multiple users.
   */
  async batchCreate(usersData: CreateUserData[]): Promise<User[]> {
    const response = await this.client.post<{ users: User[] }>(
      '/api/v1/users/batch',
      { users: usersData }
    );
    return response.data.users;
  }

  /**
   * Search users by query.
   */
  async search(query: string, limit = 20): Promise<User[]> {
    const response = await this.client.get<{ users: User[] }>('/api/v1/users/search', {
      params: { q: query, limit },
    });
    return response.data.users;
  }
}

// Usage examples
async function examples() {
  const api = new UserAPI('https://api.example.com', process.env.API_KEY!);

  try {
    // 1. List users with pagination
    console.log('=== List Users ===');
    const usersList = await api.list({
      page: 1,
      limit: 10,
      sort: 'createdAt',
      order: 'desc',
    });
    console.log(`Found ${usersList.pagination.total} users`);
    usersList.data.forEach((user) => {
      console.log(`- ${user.name} (${user.email})`);
    });

    // 2. Create a new user
    console.log('\n=== Create User ===');
    const newUser = await api.create({
      name: 'Jane Smith',
      email: 'jane@example.com',
      password: 'SecurePass123!',
      role: 'user',
    });
    console.log('Created user:', newUser);

    // 3. Get user details
    console.log('\n=== Get User ===');
    const user = await api.get(newUser.id);
    console.log('User details:', user);

    // 4. Update user
    console.log('\n=== Update User ===');
    const updatedUser = await api.update(newUser.id, {
      name: 'Jane Doe',
      role: 'moderator',
    });
    console.log('Updated user:', updatedUser);

    // 5. Search users
    console.log('\n=== Search Users ===');
    const searchResults = await api.search('jane');
    console.log('Search results:', searchResults);

    // 6. Batch create users
    console.log('\n=== Batch Create ===');
    const batchUsers = await api.batchCreate([
      {
        name: 'User 1',
        email: 'user1@example.com',
        password: 'Pass123!',
      },
      {
        name: 'User 2',
        email: 'user2@example.com',
        password: 'Pass123!',
      },
    ]);
    console.log(`Created ${batchUsers.length} users`);

    // 7. Delete user
    console.log('\n=== Delete User ===');
    await api.delete(newUser.id);
    console.log('User deleted successfully');
  } catch (error) {
    console.error('Error:', error.message);
  }
}

// Run examples
examples();
```

---

## Advanced Patterns

### Example 4: Implementing Retry Logic with Exponential Backoff

```typescript
interface RetryOptions {
  maxRetries?: number;
  initialDelay?: number;
  maxDelay?: number;
  backoffMultiplier?: number;
  retryableStatuses?: number[];
  onRetry?: (attempt: number, error: Error) => void;
}

async function fetchWithRetry<T>(
  fetchFn: () => Promise<T>,
  options: RetryOptions = {}
): Promise<T> {
  const {
    maxRetries = 3,
    initialDelay = 1000,
    maxDelay = 30000,
    backoffMultiplier = 2,
    retryableStatuses = [408, 429, 500, 502, 503, 504],
    onRetry,
  } = options;

  let lastError: Error;
  let delay = initialDelay;

  for (let attempt = 0; attempt <= maxRetries; attempt++) {
    try {
      return await fetchFn();
    } catch (error) {
      lastError = error as Error;

      // Check if we should retry
      const shouldRetry =
        attempt < maxRetries &&
        (error as any).response &&
        retryableStatuses.includes((error as any).response.status);

      if (!shouldRetry) {
        throw error;
      }

      // Call retry callback
      if (onRetry) {
        onRetry(attempt + 1, lastError);
      }

      // Wait before retrying
      await new Promise((resolve) => setTimeout(resolve, delay));

      // Calculate next delay with exponential backoff
      delay = Math.min(delay * backoffMultiplier, maxDelay);
    }
  }

  throw lastError!;
}

// Usage
async function fetchUserWithRetry(userId: string) {
  return fetchWithRetry(
    () => fetch(`https://api.example.com/users/${userId}`).then((r) => r.json()),
    {
      maxRetries: 5,
      initialDelay: 500,
      onRetry: (attempt, error) => {
        console.log(`Retry attempt ${attempt}: ${error.message}`);
      },
    }
  );
}
```

### Example 5: Request Caching and Memoization

```typescript
interface CacheEntry<T> {
  data: T;
  timestamp: number;
  expiresAt: number;
}

class CachedAPIClient {
  private cache: Map<string, CacheEntry<any>> = new Map();
  private defaultTTL: number = 60000; // 1 minute

  constructor(private baseURL: string, private apiKey: string) {}

  /**
   * Generates a cache key for a request.
   */
  private getCacheKey(endpoint: string, params?: any): string {
    const paramString = params ? JSON.stringify(params) : '';
    return `${endpoint}:${paramString}`;
  }

  /**
   * Fetches data with caching support.
   */
  async fetch<T>(
    endpoint: string,
    options: {
      params?: any;
      ttl?: number;
      forceRefresh?: boolean;
    } = {}
  ): Promise<T> {
    const { params, ttl = this.defaultTTL, forceRefresh = false } = options;
    const cacheKey = this.getCacheKey(endpoint, params);

    // Check cache first
    if (!forceRefresh) {
      const cached = this.cache.get(cacheKey);
      if (cached && Date.now() < cached.expiresAt) {
        console.log('Cache hit:', cacheKey);
        return cached.data;
      }
    }

    // Make actual request
    console.log('Cache miss:', cacheKey);
    const url = new URL(endpoint, this.baseURL);
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        url.searchParams.append(key, String(value));
      });
    }

    const response = await fetch(url.toString(), {
      headers: {
        Authorization: `Bearer ${this.apiKey}`,
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }

    const data = await response.json();

    // Store in cache
    this.cache.set(cacheKey, {
      data,
      timestamp: Date.now(),
      expiresAt: Date.now() + ttl,
    });

    return data;
  }

  /**
   * Invalidates cache entries.
   */
  invalidate(pattern?: string): void {
    if (!pattern) {
      // Clear all cache
      this.cache.clear();
      return;
    }

    // Clear matching entries
    const regex = new RegExp(pattern);
    for (const key of this.cache.keys()) {
      if (regex.test(key)) {
        this.cache.delete(key);
      }
    }
  }

  /**
   * Gets cache statistics.
   */
  getCacheStats(): {
    size: number;
    entries: Array<{ key: string; age: number; ttl: number }>;
  } {
    const now = Date.now();
    return {
      size: this.cache.size,
      entries: Array.from(this.cache.entries()).map(([key, entry]) => ({
        key,
        age: now - entry.timestamp,
        ttl: entry.expiresAt - now,
      })),
    };
  }
}

// Usage
const client = new CachedAPIClient('https://api.example.com', process.env.API_KEY!);

async function example() {
  // First call - fetches from API
  const users1 = await client.fetch('/api/v1/users', {
    params: { page: 1, limit: 10 },
    ttl: 30000, // Cache for 30 seconds
  });

  // Second call within TTL - returns from cache
  const users2 = await client.fetch('/api/v1/users', {
    params: { page: 1, limit: 10 },
  });

  // Force refresh
  const users3 = await client.fetch('/api/v1/users', {
    params: { page: 1, limit: 10 },
    forceRefresh: true,
  });

  // Invalidate specific cache entries
  client.invalidate('/api/v1/users');

  // Get cache stats
  console.log(client.getCacheStats());
}
```

### Example 6: Parallel and Sequential Request Handling

```typescript
interface Task<T> {
  id: string;
  execute: () => Promise<T>;
}

class TaskExecutor {
  /**
   * Executes tasks in parallel with concurrency limit.
   */
  async parallelWithLimit<T>(
    tasks: Task<T>[],
    concurrencyLimit: number
  ): Promise<Array<{ id: string; result?: T; error?: Error }>> {
    const results: Array<{ id: string; result?: T; error?: Error }> = [];
    const executing: Promise<void>[] = [];

    for (const task of tasks) {
      const promise = task
        .execute()
        .then((result) => {
          results.push({ id: task.id, result });
        })
        .catch((error) => {
          results.push({ id: task.id, error });
        });

      executing.push(promise);

      if (executing.length >= concurrencyLimit) {
        await Promise.race(executing);
        executing.splice(
          executing.findIndex((p) => p === promise),
          1
        );
      }
    }

    await Promise.all(executing);
    return results;
  }

  /**
   * Executes tasks sequentially.
   */
  async sequential<T>(
    tasks: Task<T>[]
  ): Promise<Array<{ id: string; result?: T; error?: Error }>> {
    const results: Array<{ id: string; result?: T; error?: Error }> = [];

    for (const task of tasks) {
      try {
        const result = await task.execute();
        results.push({ id: task.id, result });
      } catch (error) {
        results.push({ id: task.id, error: error as Error });
      }
    }

    return results;
  }

  /**
   * Executes tasks in batches.
   */
  async batched<T>(
    tasks: Task<T>[],
    batchSize: number
  ): Promise<Array<{ id: string; result?: T; error?: Error }>> {
    const results: Array<{ id: string; result?: T; error?: Error }> = [];

    for (let i = 0; i < tasks.length; i += batchSize) {
      const batch = tasks.slice(i, i + batchSize);
      const batchResults = await Promise.allSettled(
        batch.map((task) => task.execute())
      );

      batchResults.forEach((result, index) => {
        if (result.status === 'fulfilled') {
          results.push({ id: batch[index].id, result: result.value });
        } else {
          results.push({ id: batch[index].id, error: result.reason });
        }
      });
    }

    return results;
  }
}

// Usage example
async function processUsers() {
  const executor = new TaskExecutor();

  // Create tasks
  const userIds = ['user-1', 'user-2', 'user-3', 'user-4', 'user-5'];
  const tasks = userIds.map((id) => ({
    id,
    execute: () => fetch(`https://api.example.com/users/${id}`).then((r) => r.json()),
  }));

  // Execute with concurrency limit
  console.log('Parallel with limit (max 2 concurrent):');
  const parallelResults = await executor.parallelWithLimit(tasks, 2);
  console.log(parallelResults);

  // Execute sequentially
  console.log('\nSequential execution:');
  const sequentialResults = await executor.sequential(tasks);
  console.log(sequentialResults);

  // Execute in batches
  console.log('\nBatched execution (batch size 2):');
  const batchedResults = await executor.batched(tasks, 2);
  console.log(batchedResults);
}
```

---

## Real-World Scenarios

### Example 7: Building a Complete User Dashboard

```tsx
import React, { useState, useEffect, useCallback } from 'react';
import { UserAPI } from './api/UserAPI';

interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  createdAt: string;
}

interface DashboardState {
  users: User[];
  loading: boolean;
  error: string | null;
  page: number;
  totalPages: number;
  searchQuery: string;
}

export function UserDashboard() {
  const [state, setState] = useState<DashboardState>({
    users: [],
    loading: true,
    error: null,
    page: 1,
    totalPages: 1,
    searchQuery: '',
  });

  const [selectedUsers, setSelectedUsers] = useState<Set<string>>(new Set());
  const api = new UserAPI('https://api.example.com', process.env.REACT_APP_API_KEY!);

  // Load users
  const loadUsers = useCallback(async () => {
    setState((prev) => ({ ...prev, loading: true, error: null }));

    try {
      const response = await api.list({
        page: state.page,
        limit: 20,
        search: state.searchQuery,
      });

      setState((prev) => ({
        ...prev,
        users: response.data,
        totalPages: response.pagination.totalPages,
        loading: false,
      }));
    } catch (error) {
      setState((prev) => ({
        ...prev,
        error: (error as Error).message,
        loading: false,
      }));
    }
  }, [state.page, state.searchQuery]);

  // Load users on mount and when page/search changes
  useEffect(() => {
    loadUsers();
  }, [loadUsers]);

  // Handle search
  const handleSearch = useCallback((query: string) => {
    setState((prev) => ({ ...prev, searchQuery: query, page: 1 }));
  }, []);

  // Handle pagination
  const handlePageChange = useCallback((newPage: number) => {
    setState((prev) => ({ ...prev, page: newPage }));
  }, []);

  // Handle user selection
  const handleSelectUser = useCallback((userId: string) => {
    setSelectedUsers((prev) => {
      const next = new Set(prev);
      if (next.has(userId)) {
        next.delete(userId);
      } else {
        next.add(userId);
      }
      return next;
    });
  }, []);

  // Handle bulk delete
  const handleBulkDelete = useCallback(async () => {
    if (!window.confirm(`Delete ${selectedUsers.size} users?`)) {
      return;
    }

    setState((prev) => ({ ...prev, loading: true }));

    try {
      await Promise.all(
        Array.from(selectedUsers).map((id) => api.delete(id))
      );

      setSelectedUsers(new Set());
      await loadUsers();
    } catch (error) {
      setState((prev) => ({
        ...prev,
        error: (error as Error).message,
        loading: false,
      }));
    }
  }, [selectedUsers, loadUsers]);

  // Handle user creation
  const handleCreateUser = useCallback(async (userData: any) => {
    setState((prev) => ({ ...prev, loading: true }));

    try {
      await api.create(userData);
      await loadUsers();
    } catch (error) {
      setState((prev) => ({
        ...prev,
        error: (error as Error).message,
        loading: false,
      }));
    }
  }, [loadUsers]);

  return (
    <div className="dashboard">
      <header>
        <h1>User Dashboard</h1>
        <button onClick={() => handleCreateUser({ /* ... */ })}>
          Create User
        </button>
      </header>

      {/* Search bar */}
      <div className="search-bar">
        <input
          type="text"
          placeholder="Search users..."
          value={state.searchQuery}
          onChange={(e) => handleSearch(e.target.value)}
        />
      </div>

      {/* Bulk actions */}
      {selectedUsers.size > 0 && (
        <div className="bulk-actions">
          <span>{selectedUsers.size} users selected</span>
          <button onClick={handleBulkDelete}>Delete Selected</button>
        </div>
      )}

      {/* Error display */}
      {state.error && (
        <div className="error-message">
          Error: {state.error}
        </div>
      )}

      {/* Loading state */}
      {state.loading ? (
        <div className="loading">Loading...</div>
      ) : (
        <>
          {/* User table */}
          <table>
            <thead>
              <tr>
                <th>
                  <input
                    type="checkbox"
                    checked={selectedUsers.size === state.users.length}
                    onChange={(e) => {
                      if (e.target.checked) {
                        setSelectedUsers(new Set(state.users.map((u) => u.id)));
                      } else {
                        setSelectedUsers(new Set());
                      }
                    }}
                  />
                </th>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Created</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {state.users.map((user) => (
                <tr key={user.id}>
                  <td>
                    <input
                      type="checkbox"
                      checked={selectedUsers.has(user.id)}
                      onChange={() => handleSelectUser(user.id)}
                    />
                  </td>
                  <td>{user.name}</td>
                  <td>{user.email}</td>
                  <td>{user.role}</td>
                  <td>{new Date(user.createdAt).toLocaleDateString()}</td>
                  <td>
                    <button onClick={() => {/* edit */}}>Edit</button>
                    <button onClick={() => api.delete(user.id).then(loadUsers)}>
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {/* Pagination */}
          <div className="pagination">
            <button
              disabled={state.page === 1}
              onClick={() => handlePageChange(state.page - 1)}
            >
              Previous
            </button>
            <span>
              Page {state.page} of {state.totalPages}
            </span>
            <button
              disabled={state.page === state.totalPages}
              onClick={() => handlePageChange(state.page + 1)}
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
}
```

---

## Integration Examples

### Example 8: Webhook Integration

```typescript
import express from 'express';
import crypto from 'crypto';
import bodyParser from 'body-parser';

const app = express();

// Raw body parser for webhook signature verification
app.use(bodyParser.json({
  verify: (req: any, res, buf) => {
    req.rawBody = buf.toString('utf8');
  }
}));

interface WebhookEvent {
  id: string;
  type: string;
  timestamp: string;
  data: any;
}

/**
 * Verifies webhook signature using HMAC.
 */
function verifyWebhookSignature(
  payload: string,
  signature: string,
  secret: string
): boolean {
  const expectedSignature = crypto
    .createHmac('sha256', secret)
    .update(payload)
    .digest('hex');

  return crypto.timingSafeEqual(
    Buffer.from(signature),
    Buffer.from(expectedSignature)
  );
}

/**
 * Webhook endpoint handler.
 */
app.post('/webhooks', async (req, res) => {
  const signature = req.headers['x-webhook-signature'] as string;
  const webhookSecret = process.env.WEBHOOK_SECRET!;

  // Verify signature
  if (!signature || !verifyWebhookSignature(req.rawBody, signature, webhookSecret)) {
    console.error('Invalid webhook signature');
    return res.status(401).send('Invalid signature');
  }

  const event: WebhookEvent = req.body;

  // Respond quickly to acknowledge receipt
  res.status(200).send('Webhook received');

  // Process webhook asynchronously
  try {
    await processWebhook(event);
  } catch (error) {
    console.error('Webhook processing failed:', error);
    // In production, you'd want to queue this for retry
  }
});

/**
 * Processes webhook events.
 */
async function processWebhook(event: WebhookEvent): Promise<void> {
  console.log(`Processing webhook: ${event.type} (${event.id})`);

  switch (event.type) {
    case 'user.created':
      await handleUserCreated(event.data);
      break;

    case 'user.updated':
      await handleUserUpdated(event.data);
      break;

    case 'user.deleted':
      await handleUserDeleted(event.data);
      break;

    case 'payment.succeeded':
      await handlePaymentSucceeded(event.data);
      break;

    default:
      console.warn(`Unknown webhook type: ${event.type}`);
  }
}

async function handleUserCreated(data: any) {
  console.log('New user created:', data);
  // Send welcome email, create resources, etc.
}

async function handleUserUpdated(data: any) {
  console.log('User updated:', data);
  // Sync data, update caches, etc.
}

async function handleUserDeleted(data: any) {
  console.log('User deleted:', data);
  // Cleanup resources, cancel subscriptions, etc.
}

async function handlePaymentSucceeded(data: any) {
  console.log('Payment succeeded:', data);
  // Update subscription, send receipt, etc.
}

app.listen(3000, () => {
  console.log('Webhook server listening on port 3000');
});
```

---

## Testing Examples

### Example 9: Unit Testing API Clients

```typescript
import { describe, it, expect, jest, beforeEach } from '@jest/globals';
import { UserAPI } from './UserAPI';

// Mock fetch
global.fetch = jest.fn();

describe('UserAPI', () => {
  let api: UserAPI;

  beforeEach(() => {
    api = new UserAPI('https://api.example.com', 'test-api-key');
    jest.clearAllMocks();
  });

  describe('list()', () => {
    it('should fetch users with default parameters', async () => {
      const mockResponse = {
        data: [
          { id: 'user-1', name: 'John', email: 'john@example.com' },
          { id: 'user-2', name: 'Jane', email: 'jane@example.com' },
        ],
        pagination: { page: 1, limit: 20, total: 2, totalPages: 1 },
      };

      (fetch as jest.Mock).mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse,
      });

      const result = await api.list();

      expect(fetch).toHaveBeenCalledWith(
        'https://api.example.com/api/v1/users',
        expect.objectContaining({
          headers: expect.objectContaining({
            Authorization: 'Bearer test-api-key',
          }),
        })
      );

      expect(result).toEqual(mockResponse);
    });

    it('should handle pagination parameters', async () => {
      const mockResponse = {
        data: [],
        pagination: { page: 2, limit: 10, total: 100, totalPages: 10 },
      };

      (fetch as jest.Mock).mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse,
      });

      await api.list({ page: 2, limit: 10 });

      expect(fetch).toHaveBeenCalledWith(
        expect.stringContaining('page=2&limit=10'),
        expect.any(Object)
      );
    });

    it('should handle API errors', async () => {
      (fetch as jest.Mock).mockResolvedValueOnce({
        ok: false,
        status: 500,
        statusText: 'Internal Server Error',
      });

      await expect(api.list()).rejects.toThrow('500: Internal Server Error');
    });

    it('should handle network errors', async () => {
      (fetch as jest.Mock).mockRejectedValueOnce(new Error('Network error'));

      await expect(api.list()).rejects.toThrow('Network error');
    });
  });

  describe('create()', () => {
    it('should create a user', async () => {
      const userData = {
        name: 'New User',
        email: 'new@example.com',
        password: 'SecurePass123!',
      };

      const mockResponse = {
        id: 'user-new',
        name: 'New User',
        email: 'new@example.com',
        role: 'user',
        createdAt: '2025-01-15T10:00:00Z',
      };

      (fetch as jest.Mock).mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse,
      });

      const result = await api.create(userData);

      expect(fetch).toHaveBeenCalledWith(
        'https://api.example.com/api/v1/users',
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify(userData),
        })
      );

      expect(result).toEqual(mockResponse);
    });

    it('should handle validation errors', async () => {
      (fetch as jest.Mock).mockResolvedValueOnce({
        ok: false,
        status: 400,
        statusText: 'Bad Request',
        json: async () => ({
          error: {
            code: 'VALIDATION_ERROR',
            message: 'Validation failed',
            details: [
              { field: 'email', message: 'Email is already in use' },
            ],
          },
        }),
      });

      await expect(
        api.create({
          name: 'Test',
          email: 'existing@example.com',
          password: 'Pass123!',
        })
      ).rejects.toThrow();
    });
  });
});
```

---

## Performance Optimization

### Example 10: Data Loading Optimization

```typescript
import { useEffect, useState, useRef, useCallback } from 'react';

/**
 * Custom hook for infinite scroll with virtual scrolling.
 */
function useInfiniteScroll<T>(
  fetchData: (page: number) => Promise<{ data: T[]; hasMore: boolean }>,
  options: {
    initialPage?: number;
    pageSize?: number;
    threshold?: number;
  } = {}
) {
  const { initialPage = 1, pageSize = 20, threshold = 0.8 } = options;

  const [items, setItems] = useState<T[]>([]);
  const [page, setPage] = useState(initialPage);
  const [loading, setLoading] = useState(false);
  const [hasMore, setHasMore] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const observerRef = useRef<IntersectionObserver | null>(null);
  const loadingRef = useRef(false);

  const loadMore = useCallback(async () => {
    if (loadingRef.current || !hasMore) return;

    loadingRef.current = true;
    setLoading(true);
    setError(null);

    try {
      const result = await fetchData(page);
      setItems((prev) => [...prev, ...result.data]);
      setHasMore(result.hasMore);
      setPage((prev) => prev + 1);
    } catch (err) {
      setError(err as Error);
    } finally {
      setLoading(false);
      loadingRef.current = false;
    }
  }, [fetchData, page, hasMore]);

  const lastElementRef = useCallback(
    (node: HTMLElement | null) => {
      if (loading) return;

      if (observerRef.current) {
        observerRef.current.disconnect();
      }

      observerRef.current = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting && hasMore) {
          loadMore();
        }
      });

      if (node) {
        observerRef.current.observe(node);
      }
    },
    [loading, hasMore, loadMore]
  );

  useEffect(() => {
    loadMore();
  }, []);

  return {
    items,
    loading,
    error,
    hasMore,
    lastElementRef,
    loadMore,
  };
}

// Usage example
function UserList() {
  const fetchUsers = useCallback(async (page: number) => {
    const response = await fetch(
      `https://api.example.com/users?page=${page}&limit=20`
    );
    const data = await response.json();

    return {
      data: data.users,
      hasMore: page < data.pagination.totalPages,
    };
  }, []);

  const { items, loading, error, lastElementRef } = useInfiniteScroll(fetchUsers);

  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      {items.map((user, index) => (
        <div
          key={user.id}
          ref={index === items.length - 1 ? lastElementRef : null}
        >
          <h3>{user.name}</h3>
          <p>{user.email}</p>
        </div>
      ))}
      {loading && <div>Loading more...</div>}
    </div>
  );
}
```

---

## Troubleshooting

### Common Issues and Solutions

#### Issue 1: CORS Errors

**Problem:**
```
Access to fetch at 'https://api.example.com' from origin 'http://localhost:3000'
has been blocked by CORS policy
```

**Solution:**
```typescript
// Option 1: Use a proxy in development (package.json)
{
  "proxy": "https://api.example.com"
}

// Option 2: Set up CORS on the server
app.use(cors({
  origin: ['http://localhost:3000', 'https://yourdomain.com'],
  credentials: true
}));

// Option 3: Use mode and credentials in fetch
fetch('https://api.example.com/api/users', {
  mode: 'cors',
  credentials: 'include',
  headers: {
    'Content-Type': 'application/json'
  }
});
```

#### Issue 2: Authentication Token Expiry

**Solution:**
```typescript
class APIClient {
  private accessToken: string;
  private refreshToken: string;
  private isRefreshing = false;
  private refreshPromise: Promise<string> | null = null;

  async request(url: string, options: RequestInit = {}) {
    try {
      return await this.makeRequest(url, options);
    } catch (error) {
      if (error.status === 401) {
        // Token expired, try to refresh
        await this.refreshAccessToken();
        return this.makeRequest(url, options);
      }
      throw error;
    }
  }

  private async makeRequest(url: string, options: RequestInit) {
    const response = await fetch(url, {
      ...options,
      headers: {
        ...options.headers,
        Authorization: `Bearer ${this.accessToken}`,
      },
    });

    if (!response.ok) {
      throw { status: response.status, message: await response.text() };
    }

    return response.json();
  }

  private async refreshAccessToken() {
    // Prevent multiple simultaneous refresh calls
    if (this.isRefreshing) {
      return this.refreshPromise;
    }

    this.isRefreshing = true;
    this.refreshPromise = (async () => {
      try {
        const response = await fetch('/api/auth/refresh', {
          method: 'POST',
          body: JSON.stringify({ refreshToken: this.refreshToken }),
        });

        const { accessToken } = await response.json();
        this.accessToken = accessToken;
        return accessToken;
      } finally {
        this.isRefreshing = false;
        this.refreshPromise = null;
      }
    })();

    return this.refreshPromise;
  }
}
```

---

## Additional Resources

- [API Documentation](./API_DOCUMENTATION.md)
- [Function Documentation](./FUNCTION_DOCUMENTATION.md)
- [Component Documentation](./COMPONENT_DOCUMENTATION.md)
- [Best Practices](./BEST_PRACTICES.md)

---

For more examples and support:
- **GitHub:** https://github.com/example/project
- **Documentation:** https://docs.example.com
- **Community:** https://community.example.com
- **Support:** support@example.com
