# API Documentation Guide

## Table of Contents

1. [Overview](#overview)
2. [RESTful API Documentation](#restful-api-documentation)
3. [GraphQL API Documentation](#graphql-api-documentation)
4. [Authentication & Authorization](#authentication--authorization)
5. [Error Handling](#error-handling)
6. [Rate Limiting](#rate-limiting)
7. [Versioning](#versioning)
8. [Examples](#examples)

## Overview

This guide provides comprehensive documentation for all public APIs in the project. Each API endpoint includes detailed information about request/response formats, authentication requirements, and usage examples.

## RESTful API Documentation

### Endpoint Template

```
METHOD /api/v1/resource/:id
```

**Description:** Brief description of what this endpoint does.

**Authentication:** Required | Optional | None

**Authorization:** List of required roles/permissions

**Request Parameters:**

| Parameter | Type | Location | Required | Description |
|-----------|------|----------|----------|-------------|
| id | string | path | Yes | Unique identifier of the resource |
| param1 | string | query | No | Description of query parameter |

**Request Headers:**

| Header | Type | Required | Description |
|--------|------|----------|-------------|
| Authorization | string | Yes | Bearer token for authentication |
| Content-Type | string | Yes | application/json |

**Request Body:**

```json
{
  "field1": "string",
  "field2": 123,
  "field3": {
    "nestedField": "value"
  }
}
```

**Response:**

**Status Code:** 200 OK

```json
{
  "id": "resource-id",
  "field1": "string",
  "field2": 123,
  "createdAt": "2025-01-01T00:00:00Z",
  "updatedAt": "2025-01-01T00:00:00Z"
}
```

**Error Responses:**

| Status Code | Description | Response Body |
|-------------|-------------|---------------|
| 400 | Bad Request | `{"error": "Invalid input", "details": [...]}` |
| 401 | Unauthorized | `{"error": "Authentication required"}` |
| 403 | Forbidden | `{"error": "Insufficient permissions"}` |
| 404 | Not Found | `{"error": "Resource not found"}` |
| 500 | Internal Server Error | `{"error": "Internal server error"}` |

**Example Usage:**

```bash
curl -X POST \
  https://api.example.com/api/v1/resource/123 \
  -H 'Authorization: Bearer YOUR_ACCESS_TOKEN' \
  -H 'Content-Type: application/json' \
  -d '{
    "field1": "example",
    "field2": 456
  }'
```

```javascript
// JavaScript/Node.js Example
const response = await fetch('https://api.example.com/api/v1/resource/123', {
  method: 'POST',
  headers: {
    'Authorization': 'Bearer YOUR_ACCESS_TOKEN',
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    field1: 'example',
    field2: 456
  })
});

const data = await response.json();
console.log(data);
```

```python
# Python Example
import requests

url = 'https://api.example.com/api/v1/resource/123'
headers = {
    'Authorization': 'Bearer YOUR_ACCESS_TOKEN',
    'Content-Type': 'application/json'
}
data = {
    'field1': 'example',
    'field2': 456
}

response = requests.post(url, json=data, headers=headers)
result = response.json()
print(result)
```

---

## Example: User Management API

### GET /api/v1/users

**Description:** Retrieve a paginated list of users.

**Authentication:** Required

**Authorization:** `admin`, `user_manager`

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| page | integer | No | 1 | Page number for pagination |
| limit | integer | No | 20 | Number of items per page (max: 100) |
| sort | string | No | createdAt | Sort field (createdAt, name, email) |
| order | string | No | desc | Sort order (asc, desc) |
| search | string | No | - | Search query for filtering users |

**Response:**

```json
{
  "data": [
    {
      "id": "user-123",
      "name": "John Doe",
      "email": "john@example.com",
      "role": "user",
      "createdAt": "2025-01-01T00:00:00Z",
      "updatedAt": "2025-01-05T12:30:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 100,
    "totalPages": 5
  }
}
```

**Example:**

```bash
curl -X GET \
  'https://api.example.com/api/v1/users?page=1&limit=20&sort=name&order=asc' \
  -H 'Authorization: Bearer YOUR_ACCESS_TOKEN'
```

### POST /api/v1/users

**Description:** Create a new user.

**Authentication:** Required

**Authorization:** `admin`, `user_manager`

**Request Body:**

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "securePassword123",
  "role": "user"
}
```

**Validation Rules:**

- `name`: Required, string, 2-100 characters
- `email`: Required, valid email format, unique
- `password`: Required, minimum 8 characters, must contain uppercase, lowercase, and number
- `role`: Required, one of: `user`, `admin`, `moderator`

**Response:** 201 Created

```json
{
  "id": "user-456",
  "name": "John Doe",
  "email": "john@example.com",
  "role": "user",
  "createdAt": "2025-01-10T00:00:00Z"
}
```

**Example:**

```javascript
const createUser = async (userData) => {
  try {
    const response = await fetch('https://api.example.com/api/v1/users', {
      method: 'POST',
      headers: {
        'Authorization': 'Bearer YOUR_ACCESS_TOKEN',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(userData)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const newUser = await response.json();
    return newUser;
  } catch (error) {
    console.error('Error creating user:', error);
    throw error;
  }
};

// Usage
const userData = {
  name: 'John Doe',
  email: 'john@example.com',
  password: 'SecurePass123',
  role: 'user'
};

createUser(userData)
  .then(user => console.log('Created user:', user))
  .catch(err => console.error('Failed:', err));
```

### GET /api/v1/users/:id

**Description:** Retrieve a specific user by ID.

**Authentication:** Required

**Authorization:** `admin`, `user_manager`, or the user themselves

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | string | Yes | Unique user identifier |

**Response:** 200 OK

```json
{
  "id": "user-123",
  "name": "John Doe",
  "email": "john@example.com",
  "role": "user",
  "profile": {
    "avatar": "https://example.com/avatars/user-123.jpg",
    "bio": "Software developer",
    "location": "San Francisco, CA"
  },
  "createdAt": "2025-01-01T00:00:00Z",
  "updatedAt": "2025-01-05T12:30:00Z",
  "lastLoginAt": "2025-01-09T08:15:00Z"
}
```

**Example:**

```python
import requests

def get_user(user_id, access_token):
    """
    Retrieve a user by ID.
    
    Args:
        user_id (str): The unique identifier of the user
        access_token (str): Bearer token for authentication
        
    Returns:
        dict: User data
        
    Raises:
        requests.HTTPError: If the request fails
    """
    url = f'https://api.example.com/api/v1/users/{user_id}'
    headers = {'Authorization': f'Bearer {access_token}'}
    
    response = requests.get(url, headers=headers)
    response.raise_for_status()
    
    return response.json()

# Usage
try:
    user = get_user('user-123', 'YOUR_ACCESS_TOKEN')
    print(f"User: {user['name']} ({user['email']})")
except requests.HTTPError as e:
    print(f"Error: {e}")
```

### PUT /api/v1/users/:id

**Description:** Update an existing user.

**Authentication:** Required

**Authorization:** `admin`, `user_manager`, or the user themselves

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | string | Yes | Unique user identifier |

**Request Body:**

```json
{
  "name": "Jane Doe",
  "email": "jane@example.com",
  "profile": {
    "bio": "Senior software developer",
    "location": "New York, NY"
  }
}
```

**Response:** 200 OK

```json
{
  "id": "user-123",
  "name": "Jane Doe",
  "email": "jane@example.com",
  "role": "user",
  "profile": {
    "avatar": "https://example.com/avatars/user-123.jpg",
    "bio": "Senior software developer",
    "location": "New York, NY"
  },
  "updatedAt": "2025-01-10T15:45:00Z"
}
```

### DELETE /api/v1/users/:id

**Description:** Delete a user account.

**Authentication:** Required

**Authorization:** `admin`

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | string | Yes | Unique user identifier |

**Response:** 204 No Content

**Example:**

```bash
curl -X DELETE \
  https://api.example.com/api/v1/users/user-123 \
  -H 'Authorization: Bearer YOUR_ACCESS_TOKEN'
```

---

## GraphQL API Documentation

### Schema Overview

```graphql
type Query {
  user(id: ID!): User
  users(page: Int, limit: Int, search: String): UserConnection
}

type Mutation {
  createUser(input: CreateUserInput!): User
  updateUser(id: ID!, input: UpdateUserInput!): User
  deleteUser(id: ID!): Boolean
}

type User {
  id: ID!
  name: String!
  email: String!
  role: Role!
  profile: Profile
  createdAt: DateTime!
  updatedAt: DateTime!
}

type Profile {
  avatar: String
  bio: String
  location: String
}

enum Role {
  USER
  ADMIN
  MODERATOR
}

input CreateUserInput {
  name: String!
  email: String!
  password: String!
  role: Role!
}

input UpdateUserInput {
  name: String
  email: String
  profile: ProfileInput
}

input ProfileInput {
  bio: String
  location: String
}

type UserConnection {
  edges: [UserEdge!]!
  pageInfo: PageInfo!
  totalCount: Int!
}

type UserEdge {
  node: User!
  cursor: String!
}

type PageInfo {
  hasNextPage: Boolean!
  hasPreviousPage: Boolean!
  startCursor: String
  endCursor: String
}
```

### Query Examples

**Fetch a single user:**

```graphql
query GetUser($id: ID!) {
  user(id: $id) {
    id
    name
    email
    role
    profile {
      avatar
      bio
      location
    }
    createdAt
  }
}
```

Variables:
```json
{
  "id": "user-123"
}
```

**Fetch multiple users:**

```graphql
query GetUsers($page: Int, $limit: Int, $search: String) {
  users(page: $page, limit: $limit, search: $search) {
    edges {
      node {
        id
        name
        email
        role
      }
      cursor
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
      startCursor
      endCursor
    }
    totalCount
  }
}
```

Variables:
```json
{
  "page": 1,
  "limit": 20,
  "search": "john"
}
```

**Create a user:**

```graphql
mutation CreateUser($input: CreateUserInput!) {
  createUser(input: $input) {
    id
    name
    email
    role
    createdAt
  }
}
```

Variables:
```json
{
  "input": {
    "name": "John Doe",
    "email": "john@example.com",
    "password": "SecurePass123",
    "role": "USER"
  }
}
```

**Update a user:**

```graphql
mutation UpdateUser($id: ID!, $input: UpdateUserInput!) {
  updateUser(id: $id, input: $input) {
    id
    name
    email
    profile {
      bio
      location
    }
    updatedAt
  }
}
```

Variables:
```json
{
  "id": "user-123",
  "input": {
    "name": "Jane Doe",
    "profile": {
      "bio": "Senior developer",
      "location": "New York"
    }
  }
}
```

### JavaScript Client Example

```javascript
// Using Apollo Client
import { ApolloClient, InMemoryCache, gql } from '@apollo/client';

const client = new ApolloClient({
  uri: 'https://api.example.com/graphql',
  cache: new InMemoryCache(),
  headers: {
    authorization: `Bearer ${YOUR_ACCESS_TOKEN}`,
  },
});

// Query example
const GET_USER = gql`
  query GetUser($id: ID!) {
    user(id: $id) {
      id
      name
      email
      role
      profile {
        bio
        location
      }
    }
  }
`;

async function fetchUser(userId) {
  try {
    const { data } = await client.query({
      query: GET_USER,
      variables: { id: userId },
    });
    return data.user;
  } catch (error) {
    console.error('Error fetching user:', error);
    throw error;
  }
}

// Mutation example
const CREATE_USER = gql`
  mutation CreateUser($input: CreateUserInput!) {
    createUser(input: $input) {
      id
      name
      email
      createdAt
    }
  }
`;

async function createUser(userData) {
  try {
    const { data } = await client.mutate({
      mutation: CREATE_USER,
      variables: { input: userData },
    });
    return data.createUser;
  } catch (error) {
    console.error('Error creating user:', error);
    throw error;
  }
}

// Usage
fetchUser('user-123').then(user => console.log(user));

createUser({
  name: 'John Doe',
  email: 'john@example.com',
  password: 'SecurePass123',
  role: 'USER',
}).then(newUser => console.log('Created:', newUser));
```

---

## Authentication & Authorization

### Authentication Methods

#### 1. Bearer Token Authentication

**Header Format:**
```
Authorization: Bearer <access_token>
```

**Example:**
```bash
curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  https://api.example.com/api/v1/users
```

#### 2. API Key Authentication

**Header Format:**
```
X-API-Key: <api_key>
```

**Example:**
```bash
curl -H "X-API-Key: your-api-key-here" \
  https://api.example.com/api/v1/data
```

### Obtaining an Access Token

**Endpoint:** `POST /api/v1/auth/login`

**Request:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

### Refreshing an Access Token

**Endpoint:** `POST /api/v1/auth/refresh`

**Request:**
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

### Authorization Roles

| Role | Permissions |
|------|-------------|
| `admin` | Full access to all resources and operations |
| `moderator` | Read/write access to content, read access to users |
| `user` | Read/write access to own resources, read access to public resources |
| `guest` | Read-only access to public resources |

---

## Error Handling

### Standard Error Response Format

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable error message",
    "details": [
      {
        "field": "email",
        "message": "Email address is already in use"
      }
    ],
    "timestamp": "2025-01-10T12:00:00Z",
    "requestId": "req-123456"
  }
}
```

### Common Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `VALIDATION_ERROR` | 400 | Request validation failed |
| `UNAUTHORIZED` | 401 | Authentication is required or invalid |
| `FORBIDDEN` | 403 | Insufficient permissions |
| `NOT_FOUND` | 404 | Requested resource does not exist |
| `CONFLICT` | 409 | Resource already exists or conflict with current state |
| `RATE_LIMIT_EXCEEDED` | 429 | Too many requests |
| `INTERNAL_ERROR` | 500 | Internal server error |
| `SERVICE_UNAVAILABLE` | 503 | Service temporarily unavailable |

### Error Handling Best Practices

```javascript
async function handleApiCall() {
  try {
    const response = await fetch('https://api.example.com/api/v1/users', {
      headers: {
        'Authorization': 'Bearer YOUR_TOKEN'
      }
    });

    if (!response.ok) {
      const error = await response.json();
      
      // Handle specific error codes
      switch (error.error.code) {
        case 'UNAUTHORIZED':
          // Redirect to login or refresh token
          console.error('Authentication required');
          break;
        case 'RATE_LIMIT_EXCEEDED':
          // Implement retry with backoff
          console.error('Rate limit exceeded, retrying later');
          break;
        case 'VALIDATION_ERROR':
          // Display validation errors to user
          console.error('Validation errors:', error.error.details);
          break;
        default:
          console.error('API error:', error.error.message);
      }
      
      throw error;
    }

    return await response.json();
  } catch (error) {
    console.error('Request failed:', error);
    throw error;
  }
}
```

---

## Rate Limiting

### Rate Limit Headers

All API responses include rate limit information in headers:

```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1641945600
```

| Header | Description |
|--------|-------------|
| `X-RateLimit-Limit` | Maximum number of requests allowed in the time window |
| `X-RateLimit-Remaining` | Number of requests remaining in current window |
| `X-RateLimit-Reset` | Unix timestamp when the rate limit resets |

### Rate Limits by Tier

| Tier | Requests per Hour | Requests per Minute |
|------|-------------------|---------------------|
| Free | 1,000 | 60 |
| Basic | 10,000 | 200 |
| Pro | 100,000 | 1,000 |
| Enterprise | Unlimited | Custom |

### Handling Rate Limits

```javascript
async function apiCallWithRateLimit(url, options) {
  const response = await fetch(url, options);
  
  // Check rate limit headers
  const limit = response.headers.get('X-RateLimit-Limit');
  const remaining = response.headers.get('X-RateLimit-Remaining');
  const reset = response.headers.get('X-RateLimit-Reset');
  
  console.log(`Rate limit: ${remaining}/${limit} remaining`);
  
  if (response.status === 429) {
    const resetTime = new Date(reset * 1000);
    const waitTime = resetTime - Date.now();
    
    console.log(`Rate limited. Retry after ${waitTime}ms`);
    
    // Wait and retry
    await new Promise(resolve => setTimeout(resolve, waitTime));
    return apiCallWithRateLimit(url, options);
  }
  
  return response;
}
```

---

## Versioning

### API Version Strategy

The API uses URL-based versioning:

```
https://api.example.com/api/v1/resource
https://api.example.com/api/v2/resource
```

### Version Lifecycle

- **Current Version:** v2 (recommended)
- **Supported Versions:** v1, v2
- **Deprecated Versions:** None
- **Sunset Policy:** 12 months notice before deprecation

### Migration Guide

When migrating from v1 to v2:

1. **Breaking Changes:**
   - Date format changed from Unix timestamp to ISO 8601
   - Pagination changed from offset-based to cursor-based
   - Some field names changed for consistency

2. **Example Migration:**

**v1 Response:**
```json
{
  "user_id": "123",
  "created": 1641945600
}
```

**v2 Response:**
```json
{
  "id": "user-123",
  "createdAt": "2025-01-01T00:00:00Z"
}
```

---

## Examples

### Complete CRUD Example (JavaScript)

```javascript
class UserAPI {
  constructor(baseURL, accessToken) {
    this.baseURL = baseURL;
    this.accessToken = accessToken;
  }

  async request(endpoint, options = {}) {
    const url = `${this.baseURL}${endpoint}`;
    const config = {
      ...options,
      headers: {
        'Authorization': `Bearer ${this.accessToken}`,
        'Content-Type': 'application/json',
        ...options.headers,
      },
    };

    const response = await fetch(url, config);
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error.message);
    }

    return response.status === 204 ? null : await response.json();
  }

  // List users
  async listUsers(params = {}) {
    const queryString = new URLSearchParams(params).toString();
    return this.request(`/api/v1/users?${queryString}`);
  }

  // Get user by ID
  async getUser(id) {
    return this.request(`/api/v1/users/${id}`);
  }

  // Create user
  async createUser(userData) {
    return this.request('/api/v1/users', {
      method: 'POST',
      body: JSON.stringify(userData),
    });
  }

  // Update user
  async updateUser(id, userData) {
    return this.request(`/api/v1/users/${id}`, {
      method: 'PUT',
      body: JSON.stringify(userData),
    });
  }

  // Delete user
  async deleteUser(id) {
    return this.request(`/api/v1/users/${id}`, {
      method: 'DELETE',
    });
  }
}

// Usage
const api = new UserAPI('https://api.example.com', 'YOUR_ACCESS_TOKEN');

// List users
const users = await api.listUsers({ page: 1, limit: 20 });
console.log('Users:', users);

// Get specific user
const user = await api.getUser('user-123');
console.log('User:', user);

// Create user
const newUser = await api.createUser({
  name: 'John Doe',
  email: 'john@example.com',
  password: 'SecurePass123',
  role: 'user',
});
console.log('Created:', newUser);

// Update user
const updatedUser = await api.updateUser('user-123', {
  name: 'Jane Doe',
});
console.log('Updated:', updatedUser);

// Delete user
await api.deleteUser('user-123');
console.log('Deleted successfully');
```

### Complete CRUD Example (Python)

```python
import requests
from typing import Dict, Optional, List
from urllib.parse import urlencode


class UserAPI:
    def __init__(self, base_url: str, access_token: str):
        self.base_url = base_url
        self.access_token = access_token
        self.session = requests.Session()
        self.session.headers.update({
            'Authorization': f'Bearer {access_token}',
            'Content-Type': 'application/json'
        })

    def _request(self, method: str, endpoint: str, **kwargs) -> Optional[Dict]:
        """Make an API request."""
        url = f'{self.base_url}{endpoint}'
        response = self.session.request(method, url, **kwargs)
        response.raise_for_status()
        
        return response.json() if response.status_code != 204 else None

    def list_users(self, **params) -> Dict:
        """List users with optional parameters."""
        query_string = urlencode(params)
        endpoint = f'/api/v1/users?{query_string}' if query_string else '/api/v1/users'
        return self._request('GET', endpoint)

    def get_user(self, user_id: str) -> Dict:
        """Get a user by ID."""
        return self._request('GET', f'/api/v1/users/{user_id}')

    def create_user(self, user_data: Dict) -> Dict:
        """Create a new user."""
        return self._request('POST', '/api/v1/users', json=user_data)

    def update_user(self, user_id: str, user_data: Dict) -> Dict:
        """Update an existing user."""
        return self._request('PUT', f'/api/v1/users/{user_id}', json=user_data)

    def delete_user(self, user_id: str) -> None:
        """Delete a user."""
        self._request('DELETE', f'/api/v1/users/{user_id}')


# Usage example
if __name__ == '__main__':
    api = UserAPI('https://api.example.com', 'YOUR_ACCESS_TOKEN')

    # List users
    users = api.list_users(page=1, limit=20, sort='name')
    print(f"Found {users['pagination']['total']} users")

    # Get specific user
    user = api.get_user('user-123')
    print(f"User: {user['name']} ({user['email']})")

    # Create user
    new_user = api.create_user({
        'name': 'John Doe',
        'email': 'john@example.com',
        'password': 'SecurePass123',
        'role': 'user'
    })
    print(f"Created user: {new_user['id']}")

    # Update user
    updated_user = api.update_user('user-123', {
        'name': 'Jane Doe',
        'profile': {
            'bio': 'Senior developer'
        }
    })
    print(f"Updated user: {updated_user['name']}")

    # Delete user
    api.delete_user('user-123')
    print('User deleted successfully')
```

---

## Best Practices

### 1. Always Use HTTPS
All API calls should be made over HTTPS to ensure data security.

### 2. Handle Errors Gracefully
Implement proper error handling and provide meaningful error messages to users.

### 3. Implement Retry Logic
For failed requests, implement exponential backoff retry logic:

```javascript
async function retryRequest(fn, maxRetries = 3, delay = 1000) {
  for (let i = 0; i < maxRetries; i++) {
    try {
      return await fn();
    } catch (error) {
      if (i === maxRetries - 1) throw error;
      await new Promise(resolve => setTimeout(resolve, delay * Math.pow(2, i)));
    }
  }
}
```

### 4. Cache Responses
Cache API responses when appropriate to reduce load and improve performance.

### 5. Use Pagination
Always use pagination for endpoints that return lists of items.

### 6. Validate Input
Validate all input data before sending requests to the API.

### 7. Keep Tokens Secure
Never expose API keys or access tokens in client-side code or public repositories.

### 8. Monitor Rate Limits
Always check rate limit headers and implement appropriate throttling.

### 9. Use Appropriate HTTP Methods
- GET: Retrieve resources
- POST: Create resources
- PUT: Update resources (full update)
- PATCH: Partial update
- DELETE: Delete resources

### 10. Version Your API Calls
Always specify the API version in your requests to ensure compatibility.

---

## Support

For additional help or questions:

- **Documentation:** https://docs.example.com
- **API Status:** https://status.example.com
- **Support Email:** api-support@example.com
- **Community Forum:** https://community.example.com

## Changelog

### v2.0.0 (2025-01-01)
- Changed date format to ISO 8601
- Implemented cursor-based pagination
- Added GraphQL API support
- Improved error response format

### v1.0.0 (2024-01-01)
- Initial API release
- RESTful endpoints for user management
- Bearer token authentication
