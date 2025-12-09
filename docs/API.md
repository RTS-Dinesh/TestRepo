# API Reference

Complete API documentation for all public modules, classes, and functions in TestRepo.

## Table of Contents

- [Validators Module](#validators-module)
  - [ValidationError](#validationerror)
  - [is_valid_email](#is_valid_email)
  - [is_valid_url](#is_valid_url)
  - [is_valid_phone](#is_valid_phone)
  - [is_valid_credit_card](#is_valid_credit_card)
  - [is_in_range](#is_in_range)
- [Strings Module](#strings-module)
  - [slugify](#slugify)
  - [truncate](#truncate)
  - [to_camel_case](#to_camel_case)
  - [to_snake_case](#to_snake_case)
  - [to_title_case](#to_title_case)
  - [strip_html](#strip_html)
  - [mask_sensitive](#mask_sensitive)
- [Collections Module](#collections-module)
  - [OrderedSet](#orderedset)
  - [flatten](#flatten)
  - [chunk](#chunk)
  - [group_by](#group_by)
  - [unique](#unique)
  - [deep_merge](#deep_merge)
  - [pluck](#pluck)
  - [partition](#partition)
- [HTTP Client Module](#http-client-module)
  - [HttpClient](#httpclient)
  - [HttpResponse](#httpresponse)
  - [HttpError](#httperror)
  - [get](#get)
  - [post](#post)
  - [put](#put)
  - [delete](#delete)
  - [request](#request)

---

## Validators Module

`src.validators`

Data validation utilities for common types and formats.

### ValidationError

Exception raised when validation fails.

```python
class ValidationError(Exception):
    """
    Exception raised when validation fails.
    
    Attributes:
        message (str): Human-readable description of the validation error.
        field (str, optional): Name of the field that failed validation.
        value (any, optional): The value that failed validation.
    """
```

#### Constructor

```python
ValidationError(message: str, field: Optional[str] = None, value: Optional[any] = None)
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `message` | `str` | Human-readable error description |
| `field` | `str`, optional | Name of the field that failed validation |
| `value` | `any`, optional | The value that failed validation |

**Example:**

```python
from src.validators import ValidationError

try:
    email = "invalid-email"
    if not is_valid_email(email):
        raise ValidationError("Invalid email format", field="email", value=email)
except ValidationError as e:
    print(e.message)  # "Invalid email format"
    print(e.field)    # "email"
    print(e.value)    # "invalid-email"
```

---

### is_valid_email

Validate an email address format.

```python
def is_valid_email(email: str) -> bool
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `email` | `str` | The email address string to validate |

**Returns:**

| Type | Description |
|------|-------------|
| `bool` | `True` if the email format is valid, `False` otherwise |

**Examples:**

```python
from src.validators import is_valid_email

is_valid_email("user@example.com")  # True
is_valid_email("user.name+tag@example.co.uk")  # True
is_valid_email("invalid-email")  # False
is_valid_email("@example.com")  # False
is_valid_email("")  # False
```

**Notes:**

- Validates format only, does not verify email existence
- Uses simplified RFC 5322 pattern
- Empty strings and non-string inputs return `False`

---

### is_valid_url

Validate a URL format with optional scheme restrictions.

```python
def is_valid_url(url: str, allowed_schemes: Optional[List[str]] = None) -> bool
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `url` | `str` | - | The URL string to validate |
| `allowed_schemes` | `List[str]`, optional | `['http', 'https', 'ftp']` | List of allowed URL schemes |

**Returns:**

| Type | Description |
|------|-------------|
| `bool` | `True` if the URL format is valid and uses an allowed scheme |

**Examples:**

```python
from src.validators import is_valid_url

is_valid_url("https://example.com")  # True
is_valid_url("https://example.com/path?query=value")  # True
is_valid_url("ftp://files.example.com")  # True
is_valid_url("https://example.com", allowed_schemes=["https"])  # True
is_valid_url("http://example.com", allowed_schemes=["https"])  # False
is_valid_url("not-a-url")  # False
```

---

### is_valid_phone

Validate phone number format for different countries.

```python
def is_valid_phone(phone: str, country_code: str = "US") -> bool
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `phone` | `str` | - | Phone number string (may include formatting) |
| `country_code` | `str` | `"US"` | ISO 3166-1 alpha-2 country code |

**Supported Country Codes:**

| Code | Country | Format |
|------|---------|--------|
| `US` | United States | 10 digits, optional +1 |
| `UK` | United Kingdom | 10-11 digits, optional +44 |
| `DE` | Germany | 10-14 digits, optional +49 |

**Returns:**

| Type | Description |
|------|-------------|
| `bool` | `True` if the phone number format is valid for the country |

**Raises:**

| Exception | Description |
|-----------|-------------|
| `ValueError` | If an unsupported country code is provided |

**Examples:**

```python
from src.validators import is_valid_phone

is_valid_phone("(555) 123-4567")  # True (US)
is_valid_phone("+1-555-123-4567", country_code="US")  # True
is_valid_phone("+44 20 7946 0958", country_code="UK")  # True
is_valid_phone("12345")  # False
```

---

### is_valid_credit_card

Validate credit card number using the Luhn algorithm.

```python
def is_valid_credit_card(number: str) -> bool
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `number` | `str` | Credit card number (may include spaces/dashes) |

**Returns:**

| Type | Description |
|------|-------------|
| `bool` | `True` if the credit card number passes Luhn validation |

**Examples:**

```python
from src.validators import is_valid_credit_card

is_valid_credit_card("4532015112830366")  # True (Visa test number)
is_valid_credit_card("4532-0151-1283-0366")  # True
is_valid_credit_card("1234567890123456")  # False
```

**Notes:**

- Validates checksum only, not card issuer or status
- Accepts numbers 13-19 digits long
- Automatically removes spaces and dashes

---

### is_in_range

Check if a numeric value falls within a specified range.

```python
def is_in_range(
    value: Union[int, float],
    min_val: Optional[Union[int, float]] = None,
    max_val: Optional[Union[int, float]] = None,
    inclusive: bool = True
) -> bool
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `value` | `int` or `float` | - | The numeric value to check |
| `min_val` | `int` or `float`, optional | `None` | Minimum allowed value |
| `max_val` | `int` or `float`, optional | `None` | Maximum allowed value |
| `inclusive` | `bool` | `True` | Whether bounds are inclusive |

**Returns:**

| Type | Description |
|------|-------------|
| `bool` | `True` if value is within the specified range |

**Examples:**

```python
from src.validators import is_in_range

is_in_range(5, min_val=0, max_val=10)  # True
is_in_range(10, min_val=0, max_val=10, inclusive=True)  # True
is_in_range(10, min_val=0, max_val=10, inclusive=False)  # False
is_in_range(5, min_val=0)  # True (no upper bound)
is_in_range(-5, min_val=0)  # False
```

---

## Strings Module

`src.strings`

String manipulation and formatting utilities.

### slugify

Convert text to URL-friendly slug format.

```python
def slugify(text: str, separator: str = "-", lowercase: bool = True) -> str
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `text` | `str` | - | Input text to convert |
| `separator` | `str` | `"-"` | Word separator character |
| `lowercase` | `bool` | `True` | Convert to lowercase |

**Returns:**

| Type | Description |
|------|-------------|
| `str` | URL-safe slug |

**Examples:**

```python
from src.strings import slugify

slugify("Hello World!")  # "hello-world"
slugify("Hello World!", separator="_")  # "hello_world"
slugify("Hello World!", lowercase=False)  # "Hello-World"
slugify("  Multiple   Spaces  ")  # "multiple-spaces"
slugify("Café Résumé")  # "cafe-resume"
```

---

### truncate

Truncate text to a maximum length with optional suffix.

```python
def truncate(text: str, max_length: int, suffix: str = "...") -> str
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `text` | `str` | - | Input text to truncate |
| `max_length` | `int` | - | Maximum total length including suffix |
| `suffix` | `str` | `"..."` | String to append when truncated |

**Returns:**

| Type | Description |
|------|-------------|
| `str` | Truncated text with suffix, or original if short enough |

**Raises:**

| Exception | Description |
|-----------|-------------|
| `ValueError` | If max_length <= suffix length |

**Examples:**

```python
from src.strings import truncate

truncate("This is a long text", max_length=10)  # "This is..."
truncate("Short", max_length=10)  # "Short"
truncate("Hello World", max_length=8, suffix="…")  # "Hello W…"
```

---

### to_camel_case

Convert text to camelCase format.

```python
def to_camel_case(text: str) -> str
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `text` | `str` | Input text to convert |

**Returns:**

| Type | Description |
|------|-------------|
| `str` | Text in camelCase format |

**Examples:**

```python
from src.strings import to_camel_case

to_camel_case("hello_world")  # "helloWorld"
to_camel_case("hello-world")  # "helloWorld"
to_camel_case("Hello World")  # "helloWorld"
to_camel_case("API_response_handler")  # "apiResponseHandler"
```

---

### to_snake_case

Convert text to snake_case format.

```python
def to_snake_case(text: str) -> str
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `text` | `str` | Input text to convert |

**Returns:**

| Type | Description |
|------|-------------|
| `str` | Text in snake_case format |

**Examples:**

```python
from src.strings import to_snake_case

to_snake_case("helloWorld")  # "hello_world"
to_snake_case("HelloWorld")  # "hello_world"
to_snake_case("APIResponseHandler")  # "api_response_handler"
```

---

### to_title_case

Convert text to Title Case format.

```python
def to_title_case(text: str) -> str
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `text` | `str` | Input text to convert |

**Returns:**

| Type | Description |
|------|-------------|
| `str` | Text in Title Case format |

**Examples:**

```python
from src.strings import to_title_case

to_title_case("hello world")  # "Hello World"
to_title_case("a tale of two cities")  # "A Tale of Two Cities"
to_title_case("hello_world")  # "Hello World"
```

**Notes:**

- Common small words (a, an, the, of, etc.) are kept lowercase unless first/last

---

### strip_html

Remove HTML tags from text.

```python
def strip_html(html: str) -> str
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `html` | `str` | HTML string to process |

**Returns:**

| Type | Description |
|------|-------------|
| `str` | Text with HTML tags removed |

**Examples:**

```python
from src.strings import strip_html

strip_html("<p>Hello <b>World</b>!</p>")  # "Hello World!"
strip_html("&lt;escaped&gt; &amp; entities")  # "<escaped> & entities"
```

---

### mask_sensitive

Mask sensitive data showing only specified characters.

```python
def mask_sensitive(text: str, visible_chars: int = 4, mask_char: str = "*") -> str
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `text` | `str` | - | Sensitive text to mask |
| `visible_chars` | `int` | `4` | Characters to show at end |
| `mask_char` | `str` | `"*"` | Character for masking |

**Returns:**

| Type | Description |
|------|-------------|
| `str` | Masked text with last visible_chars shown |

**Examples:**

```python
from src.strings import mask_sensitive

mask_sensitive("4532015112830366")  # "************0366"
mask_sensitive("secret_api_key_12345", visible_chars=5)  # "***************12345"
mask_sensitive("password", visible_chars=2, mask_char="#")  # "######rd"
```

---

## Collections Module

`src.collections`

Enhanced collection operations and data structures.

### OrderedSet

A set that maintains insertion order.

```python
class OrderedSet(MutableSet):
    """
    A set that maintains insertion order.
    
    Elements are stored in the order they were first added.
    Duplicate insertions are ignored.
    """
```

#### Constructor

```python
OrderedSet(iterable: Optional[Iterable[T]] = None)
```

#### Methods

| Method | Description |
|--------|-------------|
| `add(value)` | Add an element to the set |
| `discard(value)` | Remove an element if present |
| `__contains__(value)` | Check if element exists |
| `__iter__()` | Iterate in insertion order |
| `__len__()` | Return number of elements |

**Examples:**

```python
from src.collections import OrderedSet

s = OrderedSet([3, 1, 4, 1, 5, 9, 2, 6, 5])
list(s)  # [3, 1, 4, 5, 9, 2, 6]

s.add(0)
s.add(3)  # Already exists, no change
list(s)  # [3, 1, 4, 5, 9, 2, 6, 0]

4 in s  # True
s.discard(4)
4 in s  # False

# Set operations
OrderedSet([1, 2, 3]) | OrderedSet([3, 4, 5])  # OrderedSet([1, 2, 3, 4, 5])
```

---

### flatten

Flatten nested iterables to a specified depth.

```python
def flatten(iterable: Iterable[Any], depth: int = 1) -> List[Any]
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `iterable` | `Iterable` | - | Nested iterable to flatten |
| `depth` | `int` | `1` | Maximum depth to flatten (-1 for unlimited) |

**Returns:**

| Type | Description |
|------|-------------|
| `List` | Flattened list of elements |

**Examples:**

```python
from src.collections import flatten

flatten([[1, 2], [3, 4]])  # [1, 2, 3, 4]
flatten([[1, [2, 3]], [4, [5, 6]]])  # [1, [2, 3], 4, [5, 6]]
flatten([[1, [2, 3]], [4, [5, 6]]], depth=2)  # [1, 2, 3, 4, 5, 6]
flatten([[[1]], [[2]]], depth=-1)  # [1, 2]
```

---

### chunk

Split an iterable into chunks of a specified size.

```python
def chunk(iterable: Iterable[T], size: int) -> Iterator[List[T]]
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `iterable` | `Iterable[T]` | Iterable to split |
| `size` | `int` | Maximum size of each chunk (must be positive) |

**Yields:**

| Type | Description |
|------|-------------|
| `List[T]` | Lists containing up to `size` elements |

**Raises:**

| Exception | Description |
|-----------|-------------|
| `ValueError` | If size < 1 |

**Examples:**

```python
from src.collections import chunk

list(chunk([1, 2, 3, 4, 5], 2))  # [[1, 2], [3, 4], [5]]
list(chunk(range(10), 3))  # [[0, 1, 2], [3, 4, 5], [6, 7, 8], [9]]
```

---

### group_by

Group items by a key function.

```python
def group_by(iterable: Iterable[T], key_func: Callable[[T], K]) -> Dict[K, List[T]]
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `iterable` | `Iterable[T]` | Items to group |
| `key_func` | `Callable[[T], K]` | Function returning grouping key |

**Returns:**

| Type | Description |
|------|-------------|
| `Dict[K, List[T]]` | Dictionary mapping keys to lists of items |

**Examples:**

```python
from src.collections import group_by

group_by([1, 2, 3, 4, 5, 6], lambda x: x % 2)
# {1: [1, 3, 5], 0: [2, 4, 6]}

users = [
    {'name': 'Alice', 'dept': 'Engineering'},
    {'name': 'Bob', 'dept': 'Sales'},
    {'name': 'Carol', 'dept': 'Engineering'},
]
group_by(users, lambda u: u['dept'])
# {'Engineering': [...], 'Sales': [...]}
```

---

### unique

Remove duplicates while preserving order.

```python
def unique(iterable: Iterable[T], key_func: Optional[Callable[[T], Any]] = None) -> List[T]
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `iterable` | `Iterable[T]` | - | Iterable to deduplicate |
| `key_func` | `Callable[[T], Any]`, optional | `None` | Function to extract comparison key |

**Returns:**

| Type | Description |
|------|-------------|
| `List[T]` | List with duplicates removed, preserving first occurrences |

**Examples:**

```python
from src.collections import unique

unique([1, 2, 2, 3, 1, 4, 3])  # [1, 2, 3, 4]
unique(['Apple', 'apple', 'BANANA'], key_func=str.lower)  # ['Apple', 'BANANA']
```

---

### deep_merge

Recursively merge two dictionaries.

```python
def deep_merge(dict1: Dict[K, V], dict2: Dict[K, V]) -> Dict[K, V]
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `dict1` | `Dict[K, V]` | Base dictionary |
| `dict2` | `Dict[K, V]` | Dictionary to merge into dict1 |

**Returns:**

| Type | Description |
|------|-------------|
| `Dict[K, V]` | New dictionary with merged contents |

**Examples:**

```python
from src.collections import deep_merge

deep_merge({'a': 1}, {'b': 2})  # {'a': 1, 'b': 2}

deep_merge(
    {'user': {'name': 'Alice', 'age': 30}},
    {'user': {'age': 31, 'city': 'NYC'}}
)
# {'user': {'name': 'Alice', 'age': 31, 'city': 'NYC'}}
```

---

### pluck

Extract a specific key from a list of dictionaries.

```python
def pluck(items: Iterable[Dict[str, Any]], key: str) -> List[Any]
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `items` | `Iterable[Dict]` | Iterable of dictionaries |
| `key` | `str` | Key to extract from each dictionary |

**Returns:**

| Type | Description |
|------|-------------|
| `List[Any]` | List of extracted values (None for missing keys) |

**Examples:**

```python
from src.collections import pluck

users = [{'id': 1, 'name': 'Alice'}, {'id': 2, 'name': 'Bob'}]
pluck(users, 'name')  # ['Alice', 'Bob']
pluck(users, 'id')  # [1, 2]
```

---

### partition

Split an iterable into two lists based on a predicate.

```python
def partition(iterable: Iterable[T], predicate: Callable[[T], bool]) -> Tuple[List[T], List[T]]
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| `iterable` | `Iterable[T]` | Items to partition |
| `predicate` | `Callable[[T], bool]` | Function returning True or False |

**Returns:**

| Type | Description |
|------|-------------|
| `Tuple[List[T], List[T]]` | (true_items, false_items) |

**Examples:**

```python
from src.collections import partition

evens, odds = partition([1, 2, 3, 4, 5, 6], lambda x: x % 2 == 0)
# evens: [2, 4, 6], odds: [1, 3, 5]
```

---

## HTTP Client Module

`src.http_client`

Simple HTTP client for making API requests.

### HttpClient

Configurable HTTP client for making requests.

```python
class HttpClient:
    """
    A configurable HTTP client for making requests.
    
    Attributes:
        base_url (str): Base URL for all requests
        default_headers (dict): Headers included in every request
        timeout (float): Default request timeout in seconds
    """
```

#### Constructor

```python
HttpClient(
    base_url: str = "",
    headers: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
)
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `base_url` | `str` | `""` | Base URL to prepend to all paths |
| `headers` | `Dict[str, str]`, optional | `None` | Default headers for all requests |
| `timeout` | `float` | `30.0` | Default timeout in seconds |

#### Methods

| Method | Description |
|--------|-------------|
| `request(method, path, **kwargs)` | Generic HTTP request |
| `get(path, **kwargs)` | GET request |
| `post(path, data, **kwargs)` | POST request |
| `put(path, data, **kwargs)` | PUT request |
| `delete(path, **kwargs)` | DELETE request |

**Examples:**

```python
from src.http_client import HttpClient

# Create configured client
client = HttpClient(
    base_url="https://api.example.com/v1",
    headers={"Authorization": "Bearer token123"},
    timeout=60.0
)

# Make requests
users = client.get("/users").json()
user = client.get("/users/1", params={"include": "posts"}).json()
new_user = client.post("/users", data={"name": "Alice"}).json()
client.put("/users/1", data={"name": "Alice Updated"})
client.delete("/users/1")
```

---

### HttpResponse

Response object containing HTTP response data.

```python
@dataclass
class HttpResponse:
    """
    Attributes:
        status_code (int): HTTP status code
        headers (Dict[str, str]): Response headers
        body (bytes): Raw response body
        url (str): Final URL after redirects
    """
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `ok` | `bool` | True if status code is 2xx |
| `text` | `str` | Response body decoded as UTF-8 |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `json()` | `Any` | Parse body as JSON |

**Examples:**

```python
response = client.get("/data")
response.status_code  # 200
response.ok  # True
response.headers  # {'Content-Type': 'application/json', ...}
response.text  # '{"key": "value"}'
response.json()  # {'key': 'value'}
```

---

### HttpError

Exception raised for HTTP-related errors.

```python
class HttpError(Exception):
    """
    Attributes:
        message (str): Error description
        status_code (int, optional): HTTP status code
        response (HttpResponse, optional): Response object if available
    """
```

**Examples:**

```python
from src.http_client import get, HttpError

try:
    response = get("https://api.example.com/not-found")
except HttpError as e:
    print(f"Error: {e.status_code} - {e.message}")
    if e.response:
        print(f"Body: {e.response.text}")
```

---

### get

Perform a GET request.

```python
def get(
    url: str,
    headers: Optional[Dict[str, str]] = None,
    params: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `url` | `str` | - | Full URL to request |
| `headers` | `Dict[str, str]`, optional | `None` | Request headers |
| `params` | `Dict[str, str]`, optional | `None` | Query parameters |
| `timeout` | `float` | `30.0` | Timeout in seconds |

**Returns:** `HttpResponse`

**Examples:**

```python
from src.http_client import get

response = get("https://api.example.com/users")
response = get(
    "https://api.example.com/users",
    params={"page": "1"},
    headers={"Authorization": "Bearer token"}
)
```

---

### post

Perform a POST request with JSON data.

```python
def post(
    url: str,
    data: Optional[Union[Dict, List]] = None,
    headers: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `url` | `str` | - | Full URL to request |
| `data` | `Dict` or `List`, optional | `None` | Request body (JSON-encoded) |
| `headers` | `Dict[str, str]`, optional | `None` | Request headers |
| `timeout` | `float` | `30.0` | Timeout in seconds |

**Returns:** `HttpResponse`

**Examples:**

```python
from src.http_client import post

response = post(
    "https://api.example.com/users",
    data={"name": "Alice", "email": "alice@example.com"}
)
```

---

### put

Perform a PUT request with JSON data.

```python
def put(
    url: str,
    data: Optional[Union[Dict, List]] = None,
    headers: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse
```

**Parameters:** Same as `post`

**Returns:** `HttpResponse`

---

### delete

Perform a DELETE request.

```python
def delete(
    url: str,
    headers: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `url` | `str` | - | Full URL to request |
| `headers` | `Dict[str, str]`, optional | `None` | Request headers |
| `timeout` | `float` | `30.0` | Timeout in seconds |

**Returns:** `HttpResponse`

---

### request

Perform a generic HTTP request.

```python
def request(
    method: str,
    url: str,
    data: Optional[Union[Dict, List]] = None,
    headers: Optional[Dict[str, str]] = None,
    params: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse
```

**Parameters:**

| Name | Type | Default | Description |
|------|------|---------|-------------|
| `method` | `str` | - | HTTP method (GET, POST, PUT, DELETE, etc.) |
| `url` | `str` | - | Full URL to request |
| `data` | `Dict` or `List`, optional | `None` | Request body (JSON-encoded) |
| `headers` | `Dict[str, str]`, optional | `None` | Request headers |
| `params` | `Dict[str, str]`, optional | `None` | Query parameters |
| `timeout` | `float` | `30.0` | Timeout in seconds |

**Returns:** `HttpResponse`

**Examples:**

```python
from src.http_client import request

response = request("PATCH", "https://api.example.com/users/1", data={"status": "active"})
```

---

## Type Definitions

### Common Types

```python
from typing import TypeVar, Callable, Dict, List, Optional, Union, Iterable, Iterator, Tuple

T = TypeVar('T')  # Generic type variable
K = TypeVar('K')  # Key type variable
V = TypeVar('V')  # Value type variable
```

---

## Error Handling

All modules follow consistent error handling patterns:

1. **Invalid Input**: Functions return `False` or raise `ValueError` for invalid inputs
2. **HTTP Errors**: `HttpError` is raised with status code and response details
3. **Validation Errors**: `ValidationError` provides field and value context

### Best Practices

```python
# Validators - check return value
if not is_valid_email(user_input):
    handle_invalid_email()

# HTTP Client - catch HttpError
try:
    response = client.get("/resource")
except HttpError as e:
    if e.status_code == 404:
        handle_not_found()
    else:
        handle_error(e)

# Collections - validate input
try:
    chunks = list(chunk(data, size=0))  # Raises ValueError
except ValueError as e:
    print(f"Invalid chunk size: {e}")
```
