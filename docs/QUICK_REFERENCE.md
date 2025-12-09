# Quick Reference Guide

> A condensed reference for all public APIs, functions, and components. For detailed documentation, see the [full documentation index](./INDEX.md).

## 🚀 Quick Start

```bash
# Install
npm install @example/api-client

# Set API key
export API_KEY=your-api-key

# Make first request
curl -H "Authorization: Bearer $API_KEY" \
  https://api.example.com/api/v1/users
```

## 📡 API Endpoints

### Authentication
```bash
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
```

### Users
```bash
GET    /api/v1/users          # List users
GET    /api/v1/users/:id      # Get user
POST   /api/v1/users          # Create user
PUT    /api/v1/users/:id      # Update user
DELETE /api/v1/users/:id      # Delete user
```

### Common Parameters
| Parameter | Type | Description |
|-----------|------|-------------|
| page | number | Page number (default: 1) |
| limit | number | Items per page (default: 20) |
| sort | string | Sort field |
| order | string | Sort order (asc/desc) |
| search | string | Search query |

## 🔐 Authentication

### Bearer Token
```javascript
fetch('https://api.example.com/api/v1/users', {
  headers: {
    'Authorization': 'Bearer YOUR_ACCESS_TOKEN'
  }
});
```

### OAuth 2.0 Flow
```javascript
// 1. Redirect to authorization endpoint
window.location = `${AUTH_URL}?client_id=${CLIENT_ID}&redirect_uri=${REDIRECT_URI}`;

// 2. Exchange code for token
const token = await fetch(TOKEN_URL, {
  method: 'POST',
  body: JSON.stringify({ code, client_id, client_secret })
});
```

## 📝 Function Documentation Template

```javascript
/**
 * Brief description of what the function does.
 *
 * @param {Type} paramName - Parameter description
 * @returns {Type} Return value description
 * @throws {ErrorType} When this error occurs
 *
 * @example
 * const result = functionName(arg);
 * console.log(result); // Expected output
 */
function functionName(paramName) {
  // Implementation
}
```

## 🎨 Component Documentation Template

```tsx
/**
 * Component description.
 *
 * @component
 * @example
 * <ComponentName prop="value">
 *   Content
 * </ComponentName>
 */
interface ComponentProps {
  /** Prop description */
  prop: string;
  /** Optional prop description */
  optionalProp?: number;
  /** Event handler description */
  onEvent?: (data: any) => void;
}

export function ComponentName({ prop, optionalProp, onEvent }: ComponentProps) {
  // Implementation
}
```

## ⚡ Common Patterns

### Error Handling
```javascript
try {
  const data = await fetchData();
  return data;
} catch (error) {
  if (error.code === 'NOT_FOUND') {
    // Handle not found
  } else if (error.code === 'UNAUTHORIZED') {
    // Handle auth error
  } else {
    // Handle other errors
  }
  throw error;
}
```

### Retry with Exponential Backoff
```javascript
async function retryWithBackoff(fn, maxRetries = 3) {
  let delay = 1000;
  for (let i = 0; i < maxRetries; i++) {
    try {
      return await fn();
    } catch (error) {
      if (i === maxRetries - 1) throw error;
      await new Promise(resolve => setTimeout(resolve, delay));
      delay *= 2;
    }
  }
}
```

### Caching
```javascript
const cache = new Map();

async function fetchWithCache(key, fetcher, ttl = 60000) {
  const cached = cache.get(key);
  if (cached && Date.now() < cached.expiresAt) {
    return cached.data;
  }
  
  const data = await fetcher();
  cache.set(key, {
    data,
    expiresAt: Date.now() + ttl
  });
  
  return data;
}
```

### Parallel Requests
```javascript
// Execute in parallel
const [users, posts, comments] = await Promise.all([
  fetchUsers(),
  fetchPosts(),
  fetchComments()
]);

// Execute with concurrency limit
async function parallelWithLimit(tasks, limit) {
  const results = [];
  const executing = [];
  
  for (const task of tasks) {
    const promise = task().then(result => {
      executing.splice(executing.indexOf(promise), 1);
      return result;
    });
    
    results.push(promise);
    executing.push(promise);
    
    if (executing.length >= limit) {
      await Promise.race(executing);
    }
  }
  
  return Promise.all(results);
}
```

## 🔧 HTTP Status Codes

| Code | Meaning | Action |
|------|---------|--------|
| 200 | OK | Success |
| 201 | Created | Resource created |
| 204 | No Content | Success, no response body |
| 400 | Bad Request | Fix request parameters |
| 401 | Unauthorized | Authenticate |
| 403 | Forbidden | Check permissions |
| 404 | Not Found | Check resource exists |
| 429 | Too Many Requests | Implement rate limiting |
| 500 | Internal Error | Retry or contact support |

## 📦 Response Format

### Success Response
```json
{
  "id": "resource-id",
  "field": "value",
  "createdAt": "2025-01-15T10:00:00Z",
  "updatedAt": "2025-01-15T10:00:00Z"
}
```

### List Response
```json
{
  "data": [
    { "id": "1", "name": "Item 1" },
    { "id": "2", "name": "Item 2" }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 100,
    "totalPages": 5
  }
}
```

### Error Response
```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable message",
    "details": [
      {
        "field": "email",
        "message": "Email is required"
      }
    ]
  }
}
```

## 🎯 React Patterns

### Custom Hook
```typescript
function useAPI<T>(endpoint: string) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    let mounted = true;
    
    fetch(endpoint)
      .then(res => res.json())
      .then(data => {
        if (mounted) {
          setData(data);
          setLoading(false);
        }
      })
      .catch(err => {
        if (mounted) {
          setError(err);
          setLoading(false);
        }
      });

    return () => { mounted = false; };
  }, [endpoint]);

  return { data, loading, error };
}
```

### Context Pattern
```tsx
const APIContext = createContext<APIClient | null>(null);

export function APIProvider({ children, apiKey }) {
  const client = useMemo(
    () => new APIClient(apiKey),
    [apiKey]
  );

  return (
    <APIContext.Provider value={client}>
      {children}
    </APIContext.Provider>
  );
}

export function useAPIClient() {
  const context = useContext(APIContext);
  if (!context) {
    throw new Error('useAPIClient must be used within APIProvider');
  }
  return context;
}
```

## 🎨 Vue Patterns

### Composable
```typescript
export function useAPI<T>(endpoint: string) {
  const data = ref<T | null>(null);
  const loading = ref(true);
  const error = ref<Error | null>(null);

  const fetch = async () => {
    loading.value = true;
    error.value = null;
    
    try {
      const response = await fetch(endpoint);
      data.value = await response.json();
    } catch (e) {
      error.value = e as Error;
    } finally {
      loading.value = false;
    }
  };

  onMounted(fetch);

  return { data, loading, error, refetch: fetch };
}
```

## 🧪 Testing

### Unit Test
```typescript
describe('fetchUser', () => {
  it('should fetch user data', async () => {
    const mockUser = { id: '1', name: 'John' };
    global.fetch = jest.fn().mockResolvedValue({
      ok: true,
      json: async () => mockUser
    });

    const user = await fetchUser('1');
    
    expect(user).toEqual(mockUser);
    expect(fetch).toHaveBeenCalledWith(
      'https://api.example.com/users/1',
      expect.any(Object)
    );
  });
});
```

### Integration Test
```typescript
describe('UserAPI', () => {
  let api: UserAPI;

  beforeEach(() => {
    api = new UserAPI('https://api.test.com', 'test-key');
  });

  it('should handle CRUD operations', async () => {
    // Create
    const user = await api.create({
      name: 'Test',
      email: 'test@example.com'
    });
    expect(user.id).toBeDefined();

    // Read
    const fetched = await api.get(user.id);
    expect(fetched.name).toBe('Test');

    // Update
    const updated = await api.update(user.id, {
      name: 'Updated'
    });
    expect(updated.name).toBe('Updated');

    // Delete
    await api.delete(user.id);
    await expect(api.get(user.id)).rejects.toThrow();
  });
});
```

## 📚 Documentation Checklist

When documenting, ensure you have:

- [ ] Clear description
- [ ] All parameters documented with types
- [ ] Return value documented
- [ ] Errors/exceptions listed
- [ ] At least one basic example
- [ ] Advanced example (if applicable)
- [ ] Error handling example
- [ ] Expected output shown

## 🔗 Quick Links

| Document | Purpose |
|----------|---------|
| [Full Index](./INDEX.md) | Complete documentation index |
| [API Docs](./API_DOCUMENTATION.md) | Detailed API reference |
| [Functions](./FUNCTION_DOCUMENTATION.md) | Function documentation guide |
| [Components](./COMPONENT_DOCUMENTATION.md) | Component documentation |
| [Examples](./USAGE_EXAMPLES.md) | Real-world examples |
| [Best Practices](./BEST_PRACTICES.md) | Documentation guidelines |

## 💡 Pro Tips

1. **Always use HTTPS** in production
2. **Validate input** before making requests
3. **Handle errors gracefully** with user-friendly messages
4. **Implement retry logic** for network requests
5. **Cache when appropriate** to reduce API calls
6. **Use pagination** for large datasets
7. **Monitor rate limits** to avoid throttling
8. **Version your APIs** for backwards compatibility
9. **Test all examples** in documentation
10. **Keep docs updated** with code changes

## 🆘 Common Issues

### CORS Errors
```javascript
// Use proxy in development
// Or configure CORS on server
app.use(cors({
  origin: 'http://localhost:3000',
  credentials: true
}));
```

### Rate Limiting
```javascript
// Check headers
const remaining = response.headers.get('X-RateLimit-Remaining');
if (remaining < 10) {
  console.warn('Approaching rate limit');
}
```

### Token Expiry
```javascript
// Refresh token before it expires
if (Date.now() >= tokenExpiry - 60000) {
  await refreshToken();
}
```

---

**Need more details?** See the [complete documentation](./INDEX.md)

**Last Updated:** 2025-01-15
