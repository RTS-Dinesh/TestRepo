# TestRepo - Python Utility Library

A comprehensive Python utility library providing data validation, string manipulation, collection operations, and HTTP client utilities with full documentation and type hints.

[![Python Version](https://img.shields.io/badge/python-3.8%2B-blue)](https://www.python.org/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Modules](#modules)
  - [Validators](#validators)
  - [Strings](#strings)
  - [Collections](#collections)
  - [HTTP Client](#http-client)
- [API Reference](#api-reference)
- [Examples](#examples)
- [Contributing](#contributing)
- [License](#license)

---

## Installation

### From Source

```bash
git clone https://github.com/example/testrepo.git
cd testrepo
pip install -e .
```

### Requirements

- Python 3.8 or higher
- No external dependencies (uses only Python standard library)

---

## Quick Start

```python
from src import validators, strings, collections, http_client

# Validate data
validators.is_valid_email("user@example.com")  # True
validators.is_valid_url("https://example.com")  # True

# Manipulate strings
strings.slugify("Hello World!")  # "hello-world"
strings.to_camel_case("hello_world")  # "helloWorld"

# Work with collections
collections.flatten([[1, 2], [3, 4]])  # [1, 2, 3, 4]
collections.unique([1, 2, 2, 3])  # [1, 2, 3]

# Make HTTP requests
response = http_client.get("https://api.example.com/data")
data = response.json()
```

---

## Modules

### Validators

The `validators` module provides functions for validating common data formats.

#### Functions

| Function | Description |
|----------|-------------|
| `is_valid_email(email)` | Validate email address format |
| `is_valid_url(url, allowed_schemes)` | Validate URL format with optional scheme restrictions |
| `is_valid_phone(phone, country_code)` | Validate phone number for different countries |
| `is_valid_credit_card(number)` | Validate credit card using Luhn algorithm |
| `is_in_range(value, min_val, max_val, inclusive)` | Check if a number is within a range |

#### Examples

```python
from src.validators import is_valid_email, is_valid_url, is_valid_phone, is_valid_credit_card

# Email validation
is_valid_email("user@example.com")  # True
is_valid_email("invalid-email")  # False
is_valid_email("user.name+tag@example.co.uk")  # True

# URL validation
is_valid_url("https://example.com")  # True
is_valid_url("ftp://files.example.com")  # True
is_valid_url("https://example.com", allowed_schemes=["https"])  # True
is_valid_url("http://example.com", allowed_schemes=["https"])  # False

# Phone validation
is_valid_phone("(555) 123-4567")  # True (US format)
is_valid_phone("+1-555-123-4567", country_code="US")  # True
is_valid_phone("+44 20 7946 0958", country_code="UK")  # True

# Credit card validation (Luhn algorithm)
is_valid_credit_card("4532015112830366")  # True
is_valid_credit_card("4532-0151-1283-0366")  # True (with dashes)
is_valid_credit_card("1234567890123456")  # False

# Range validation
from src.validators import is_in_range

is_in_range(5, min_val=0, max_val=10)  # True
is_in_range(10, min_val=0, max_val=10, inclusive=True)  # True
is_in_range(10, min_val=0, max_val=10, inclusive=False)  # False
is_in_range(5, min_val=0)  # True (no upper bound)
```

#### Exception Handling

```python
from src.validators import ValidationError

try:
    # Your validation logic here
    if not is_valid_email(user_input):
        raise ValidationError("Invalid email format", field="email", value=user_input)
except ValidationError as e:
    print(f"Validation failed: {e.message}")
    print(f"Field: {e.field}, Value: {e.value}")
```

---

### Strings

The `strings` module provides string manipulation and formatting utilities.

#### Functions

| Function | Description |
|----------|-------------|
| `slugify(text, separator, lowercase)` | Convert text to URL-friendly slug |
| `truncate(text, max_length, suffix)` | Truncate text with suffix |
| `to_camel_case(text)` | Convert to camelCase |
| `to_snake_case(text)` | Convert to snake_case |
| `to_title_case(text)` | Convert to Title Case |
| `strip_html(html)` | Remove HTML tags |
| `mask_sensitive(text, visible_chars, mask_char)` | Mask sensitive data |

#### Examples

```python
from src.strings import slugify, truncate, to_camel_case, to_snake_case

# Slugify
slugify("Hello World!")  # "hello-world"
slugify("Hello World!", separator="_")  # "hello_world"
slugify("Café Résumé")  # "cafe-resume" (unicode normalized)

# Truncate
truncate("This is a long text", max_length=10)  # "This is..."
truncate("Short", max_length=10)  # "Short" (unchanged)
truncate("Hello World", max_length=8, suffix="…")  # "Hello W…"

# Case conversion
to_camel_case("hello_world")  # "helloWorld"
to_camel_case("hello-world")  # "helloWorld"
to_camel_case("Hello World")  # "helloWorld"

to_snake_case("helloWorld")  # "hello_world"
to_snake_case("HelloWorld")  # "hello_world"
to_snake_case("APIResponseHandler")  # "api_response_handler"

# Title case with smart handling
from src.strings import to_title_case
to_title_case("the quick brown fox")  # "The Quick Brown Fox"
to_title_case("a tale of two cities")  # "A Tale of Two Cities"

# Strip HTML tags
from src.strings import strip_html
strip_html("<p>Hello <b>World</b>!</p>")  # "Hello World!"
strip_html("&lt;escaped&gt; &amp; entities")  # "<escaped> & entities"

# Mask sensitive data
from src.strings import mask_sensitive
mask_sensitive("4532015112830366")  # "************0366"
mask_sensitive("secret_api_key_12345", visible_chars=5)  # "***************12345"
mask_sensitive("password", visible_chars=2, mask_char="#")  # "######rd"
```

---

### Collections

The `collections` module provides enhanced collection operations and data structures.

#### Classes

| Class | Description |
|-------|-------------|
| `OrderedSet` | A set that maintains insertion order |

#### Functions

| Function | Description |
|----------|-------------|
| `flatten(iterable, depth)` | Flatten nested iterables |
| `chunk(iterable, size)` | Split into fixed-size chunks |
| `group_by(iterable, key_func)` | Group items by key function |
| `unique(iterable, key_func)` | Remove duplicates preserving order |
| `deep_merge(dict1, dict2)` | Recursively merge dictionaries |
| `pluck(items, key)` | Extract key from list of dicts |
| `partition(iterable, predicate)` | Split by predicate into two lists |

#### Examples

```python
from src.collections import flatten, chunk, group_by, unique, OrderedSet

# Flatten nested lists
flatten([[1, 2], [3, 4]])  # [1, 2, 3, 4]
flatten([[1, [2, 3]], [4, [5, 6]]])  # [1, [2, 3], 4, [5, 6]] (depth=1)
flatten([[1, [2, 3]], [4, [5, 6]]], depth=2)  # [1, 2, 3, 4, 5, 6]
flatten([[[1]], [[2]]], depth=-1)  # [1, 2] (unlimited depth)

# Chunk into groups
list(chunk([1, 2, 3, 4, 5], 2))  # [[1, 2], [3, 4], [5]]
list(chunk(range(10), 3))  # [[0, 1, 2], [3, 4, 5], [6, 7, 8], [9]]

# Group by key
group_by([1, 2, 3, 4, 5, 6], lambda x: x % 2)
# {1: [1, 3, 5], 0: [2, 4, 6]}

users = [
    {'name': 'Alice', 'dept': 'Engineering'},
    {'name': 'Bob', 'dept': 'Sales'},
    {'name': 'Carol', 'dept': 'Engineering'},
]
group_by(users, lambda u: u['dept'])
# {'Engineering': [{'name': 'Alice', ...}, {'name': 'Carol', ...}],
#  'Sales': [{'name': 'Bob', ...}]}

# Remove duplicates (order preserved)
unique([1, 2, 2, 3, 1, 4, 3])  # [1, 2, 3, 4]
unique(['Apple', 'apple', 'BANANA', 'banana'], key_func=str.lower)  # ['Apple', 'BANANA']

# OrderedSet
s = OrderedSet([3, 1, 4, 1, 5, 9, 2, 6, 5])
list(s)  # [3, 1, 4, 5, 9, 2, 6] (duplicates removed, order preserved)

# Deep merge dictionaries
from src.collections import deep_merge
deep_merge(
    {'user': {'name': 'Alice', 'age': 30}},
    {'user': {'age': 31, 'city': 'NYC'}}
)
# {'user': {'name': 'Alice', 'age': 31, 'city': 'NYC'}}

# Pluck values from list of dicts
from src.collections import pluck
users = [{'id': 1, 'name': 'Alice'}, {'id': 2, 'name': 'Bob'}]
pluck(users, 'name')  # ['Alice', 'Bob']

# Partition by predicate
from src.collections import partition
evens, odds = partition([1, 2, 3, 4, 5, 6], lambda x: x % 2 == 0)
# evens: [2, 4, 6], odds: [1, 3, 5]
```

---

### HTTP Client

The `http_client` module provides a simple HTTP client for API requests.

#### Classes

| Class | Description |
|-------|-------------|
| `HttpClient` | Configurable HTTP client |
| `HttpResponse` | Response object with status, headers, body |
| `HttpError` | Exception for HTTP errors |

#### Functions

| Function | Description |
|----------|-------------|
| `get(url, headers, params, timeout)` | Perform GET request |
| `post(url, data, headers, timeout)` | Perform POST request |
| `put(url, data, headers, timeout)` | Perform PUT request |
| `delete(url, headers, timeout)` | Perform DELETE request |
| `request(method, url, **kwargs)` | Generic HTTP request |

#### Examples

```python
from src.http_client import get, post, HttpClient, HttpError

# Simple GET request
response = get("https://api.example.com/users")
if response.ok:
    users = response.json()
    print(f"Found {len(users)} users")

# GET with parameters and headers
response = get(
    "https://api.example.com/users",
    params={"page": "1", "limit": "10"},
    headers={"Authorization": "Bearer token123"}
)

# POST with JSON data
response = post(
    "https://api.example.com/users",
    data={"name": "Alice", "email": "alice@example.com"}
)
new_user = response.json()

# Using HttpClient for multiple requests
client = HttpClient(
    base_url="https://api.example.com/v1",
    headers={"Authorization": "Bearer token123"},
    timeout=60.0
)

# All requests use the base URL and default headers
users = client.get("/users").json()
user = client.get("/users/1").json()
new_user = client.post("/users", data={"name": "Bob"}).json()
client.delete("/users/1")

# Error handling
try:
    response = get("https://api.example.com/not-found")
except HttpError as e:
    print(f"Request failed: {e.status_code}")
    print(f"Error message: {e.message}")
    if e.response:
        print(f"Response body: {e.response.text}")

# Response properties
response = get("https://api.example.com/data")
print(response.status_code)  # 200
print(response.ok)  # True
print(response.headers)  # {'Content-Type': 'application/json', ...}
print(response.text)  # Raw text response
print(response.json())  # Parsed JSON
print(response.url)  # Final URL (after redirects)
```

---

## API Reference

For detailed API documentation, see [docs/API.md](docs/API.md).

### Quick Reference

#### Validators Module

```python
# is_valid_email(email: str) -> bool
is_valid_email("user@example.com")

# is_valid_url(url: str, allowed_schemes: List[str] = None) -> bool
is_valid_url("https://example.com", allowed_schemes=["https", "http"])

# is_valid_phone(phone: str, country_code: str = "US") -> bool
is_valid_phone("+1-555-123-4567", country_code="US")

# is_valid_credit_card(number: str) -> bool
is_valid_credit_card("4532-0151-1283-0366")

# is_in_range(value: Union[int, float], min_val: Optional = None, 
#             max_val: Optional = None, inclusive: bool = True) -> bool
is_in_range(5, min_val=0, max_val=10)
```

#### Strings Module

```python
# slugify(text: str, separator: str = "-", lowercase: bool = True) -> str
slugify("Hello World!")

# truncate(text: str, max_length: int, suffix: str = "...") -> str
truncate("Long text", max_length=10)

# to_camel_case(text: str) -> str
to_camel_case("hello_world")

# to_snake_case(text: str) -> str
to_snake_case("helloWorld")

# to_title_case(text: str) -> str
to_title_case("hello world")

# strip_html(html: str) -> str
strip_html("<p>Text</p>")

# mask_sensitive(text: str, visible_chars: int = 4, mask_char: str = "*") -> str
mask_sensitive("1234567890")
```

#### Collections Module

```python
# flatten(iterable: Iterable, depth: int = 1) -> List
flatten([[1, 2], [3, 4]])

# chunk(iterable: Iterable[T], size: int) -> Iterator[List[T]]
list(chunk([1, 2, 3, 4, 5], 2))

# group_by(iterable: Iterable[T], key_func: Callable[[T], K]) -> Dict[K, List[T]]
group_by(items, lambda x: x['type'])

# unique(iterable: Iterable[T], key_func: Callable = None) -> List[T]
unique([1, 2, 2, 3])

# deep_merge(dict1: Dict, dict2: Dict) -> Dict
deep_merge({'a': 1}, {'b': 2})

# pluck(items: Iterable[Dict], key: str) -> List
pluck(users, 'name')

# partition(iterable: Iterable[T], predicate: Callable[[T], bool]) -> Tuple[List, List]
partition(numbers, lambda x: x > 0)
```

#### HTTP Client Module

```python
# get(url: str, headers: Dict = None, params: Dict = None, timeout: float = 30.0) -> HttpResponse
get("https://api.example.com/users")

# post(url: str, data: Union[Dict, List] = None, headers: Dict = None, timeout: float = 30.0) -> HttpResponse
post("https://api.example.com/users", data={"name": "Alice"})

# HttpClient(base_url: str = "", headers: Dict = None, timeout: float = 30.0)
client = HttpClient(base_url="https://api.example.com")
```

---

## Examples

### Real-World Use Cases

#### Form Validation

```python
from src.validators import is_valid_email, is_valid_phone, ValidationError

def validate_contact_form(data: dict) -> list:
    """Validate a contact form and return list of errors."""
    errors = []
    
    if not data.get('email') or not is_valid_email(data['email']):
        errors.append("Please provide a valid email address")
    
    if data.get('phone') and not is_valid_phone(data['phone']):
        errors.append("Please provide a valid phone number")
    
    if not data.get('message') or len(data['message']) < 10:
        errors.append("Message must be at least 10 characters")
    
    return errors

# Usage
form_data = {
    'email': 'user@example.com',
    'phone': '(555) 123-4567',
    'message': 'Hello, I have a question...'
}
errors = validate_contact_form(form_data)
if not errors:
    print("Form is valid!")
```

#### Blog Post URL Generation

```python
from src.strings import slugify, truncate

def generate_post_url(title: str, post_id: int) -> str:
    """Generate a SEO-friendly URL for a blog post."""
    slug = slugify(title)
    # Limit slug length for cleaner URLs
    slug = truncate(slug, max_length=50, suffix="")
    return f"/blog/{post_id}/{slug}"

# Usage
url = generate_post_url("10 Tips for Writing Better Python Code!", 42)
# Result: "/blog/42/10-tips-for-writing-better-python-code"
```

#### API Data Processing

```python
from src.collections import group_by, pluck, unique
from src.http_client import HttpClient

def process_user_data():
    """Fetch and process user data from an API."""
    client = HttpClient(base_url="https://api.example.com")
    
    # Fetch all users
    users = client.get("/users").json()
    
    # Group users by department
    by_department = group_by(users, lambda u: u.get('department', 'Unknown'))
    
    # Get unique email domains
    emails = pluck(users, 'email')
    domains = unique([email.split('@')[1] for email in emails if email])
    
    return {
        'total_users': len(users),
        'departments': list(by_department.keys()),
        'email_domains': domains
    }
```

#### Configuration Merging

```python
from src.collections import deep_merge

# Default configuration
default_config = {
    'app': {
        'name': 'MyApp',
        'debug': False,
        'logging': {
            'level': 'INFO',
            'format': 'standard'
        }
    },
    'database': {
        'host': 'localhost',
        'port': 5432
    }
}

# Environment-specific overrides
production_config = {
    'app': {
        'debug': False,
        'logging': {
            'level': 'WARNING'
        }
    },
    'database': {
        'host': 'db.production.example.com'
    }
}

# Merge configurations
config = deep_merge(default_config, production_config)
# Result:
# {
#     'app': {
#         'name': 'MyApp',
#         'debug': False,
#         'logging': {'level': 'WARNING', 'format': 'standard'}
#     },
#     'database': {'host': 'db.production.example.com', 'port': 5432}
# }
```

---

## Contributing

We welcome contributions! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for new functionality
4. Ensure all tests pass
5. Add documentation for new features
6. Submit a pull request

### Code Style

- Follow PEP 8 guidelines
- Use type hints for all public functions
- Write comprehensive docstrings (NumPy style)
- Include examples in docstrings

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Changelog

### Version 1.0.0

- Initial release
- Added validators module (email, URL, phone, credit card, range)
- Added strings module (slugify, truncate, case conversion, HTML stripping, masking)
- Added collections module (flatten, chunk, group_by, unique, deep_merge, pluck, partition, OrderedSet)
- Added http_client module (HttpClient, HttpResponse, convenience functions)
- Full documentation and examples
