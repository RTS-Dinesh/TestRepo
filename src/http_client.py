"""
HTTP Client Module
==================

This module provides a simple, well-documented HTTP client for making
API requests. It wraps Python's urllib library to provide a clean,
intuitive interface for common HTTP operations.

Classes
-------
HttpClient
    A configurable HTTP client for making requests.
HttpResponse
    A response object containing status, headers, and body.
HttpError
    Exception raised for HTTP-related errors.

Functions
---------
get(url, headers, params, timeout)
    Perform a GET request.
post(url, data, headers, timeout)
    Perform a POST request with JSON data.
request(method, url, **kwargs)
    Perform a generic HTTP request.

Examples
--------
>>> from src.http_client import get, post, HttpClient
>>> response = get("https://api.example.com/users")
>>> response.status_code
200
>>> response.json()
{'users': [...]}

>>> client = HttpClient(base_url="https://api.example.com")
>>> client.get("/users")
<HttpResponse [200]>
"""

import json
import urllib.request
import urllib.parse
import urllib.error
from typing import Any, Dict, List, Optional, Union
from dataclasses import dataclass, field

__all__ = [
    "HttpClient",
    "HttpResponse",
    "HttpError",
    "get",
    "post",
    "put",
    "delete",
    "request",
]


class HttpError(Exception):
    """
    Exception raised for HTTP-related errors.
    
    This exception is raised when an HTTP request fails due to network
    issues, server errors, or invalid responses.
    
    Attributes
    ----------
    message : str
        Human-readable error description.
    status_code : int, optional
        HTTP status code if available.
    response : HttpResponse, optional
        The response object if available.
    
    Examples
    --------
    >>> try:
    ...     response = get("https://api.example.com/not-found")
    ... except HttpError as e:
    ...     print(f"Request failed: {e.status_code} - {e.message}")
    Request failed: 404 - Not Found
    """
    
    def __init__(
        self,
        message: str,
        status_code: Optional[int] = None,
        response: Optional['HttpResponse'] = None
    ):
        self.message = message
        self.status_code = status_code
        self.response = response
        super().__init__(message)


@dataclass
class HttpResponse:
    """
    A response object containing HTTP response data.
    
    This class encapsulates the response from an HTTP request, providing
    convenient access to status code, headers, and body content.
    
    Attributes
    ----------
    status_code : int
        The HTTP status code (e.g., 200, 404, 500).
    headers : dict
        Response headers as a dictionary.
    body : bytes
        Raw response body as bytes.
    url : str
        The final URL after any redirects.
    
    Examples
    --------
    >>> response = get("https://api.example.com/data")
    >>> response.status_code
    200
    >>> response.ok
    True
    >>> data = response.json()
    >>> response.text
    '{"key": "value"}'
    """
    
    status_code: int
    headers: Dict[str, str] = field(default_factory=dict)
    body: bytes = b""
    url: str = ""
    
    @property
    def ok(self) -> bool:
        """
        Check if the response was successful.
        
        Returns
        -------
        bool
            True if status code is in the 2xx range.
        
        Examples
        --------
        >>> response.status_code = 200
        >>> response.ok
        True
        >>> response.status_code = 404
        >>> response.ok
        False
        """
        return 200 <= self.status_code < 300
    
    @property
    def text(self) -> str:
        """
        Get the response body as a string.
        
        Decodes the response body using UTF-8 encoding.
        
        Returns
        -------
        str
            The response body decoded as a string.
        
        Examples
        --------
        >>> response.body = b'Hello, World!'
        >>> response.text
        'Hello, World!'
        """
        return self.body.decode('utf-8')
    
    def json(self) -> Any:
        """
        Parse the response body as JSON.
        
        Decodes the response body and parses it as JSON.
        
        Returns
        -------
        any
            The parsed JSON data (dict, list, or primitive).
        
        Raises
        ------
        json.JSONDecodeError
            If the response body is not valid JSON.
        
        Examples
        --------
        >>> response.body = b'{"name": "Alice", "age": 30}'
        >>> response.json()
        {'name': 'Alice', 'age': 30}
        """
        return json.loads(self.text)
    
    def __repr__(self) -> str:
        """Return a string representation of the response."""
        return f"<HttpResponse [{self.status_code}]>"


class HttpClient:
    """
    A configurable HTTP client for making requests.
    
    HttpClient provides a reusable client with configurable base URL,
    default headers, and timeout settings. It supports all common
    HTTP methods and handles JSON encoding/decoding automatically.
    
    Parameters
    ----------
    base_url : str, optional
        Base URL to prepend to all request paths.
    headers : dict, optional
        Default headers to include in all requests.
    timeout : float, default 30.0
        Default timeout in seconds for requests.
    
    Attributes
    ----------
    base_url : str
        The base URL for all requests.
    default_headers : dict
        Headers included in every request.
    timeout : float
        Default request timeout in seconds.
    
    Examples
    --------
    Create a client with base URL and authentication:
    
    >>> client = HttpClient(
    ...     base_url="https://api.example.com/v1",
    ...     headers={"Authorization": "Bearer token123"}
    ... )
    >>> response = client.get("/users")
    >>> users = response.json()
    
    Make requests with custom headers per-request:
    
    >>> response = client.post(
    ...     "/users",
    ...     data={"name": "Alice"},
    ...     headers={"X-Request-ID": "abc123"}
    ... )
    
    See Also
    --------
    get : Module-level function for simple GET requests.
    post : Module-level function for simple POST requests.
    """
    
    def __init__(
        self,
        base_url: str = "",
        headers: Optional[Dict[str, str]] = None,
        timeout: float = 30.0
    ):
        self.base_url = base_url.rstrip('/')
        self.default_headers = headers or {}
        self.timeout = timeout
    
    def request(
        self,
        method: str,
        path: str,
        data: Optional[Union[Dict, List]] = None,
        headers: Optional[Dict[str, str]] = None,
        params: Optional[Dict[str, str]] = None,
        timeout: Optional[float] = None
    ) -> HttpResponse:
        """
        Perform an HTTP request.
        
        Makes an HTTP request with the specified method, path, and options.
        Automatically handles JSON encoding for request bodies and
        URL encoding for query parameters.
        
        Parameters
        ----------
        method : str
            HTTP method (GET, POST, PUT, DELETE, PATCH, etc.).
        path : str
            URL path (appended to base_url if set).
        data : dict or list, optional
            Request body to be JSON-encoded.
        headers : dict, optional
            Additional headers for this request.
        params : dict, optional
            Query parameters to append to the URL.
        timeout : float, optional
            Request timeout in seconds. Uses default if not specified.
        
        Returns
        -------
        HttpResponse
            The response from the server.
        
        Raises
        ------
        HttpError
            If the request fails or returns a non-2xx status code.
        
        Examples
        --------
        >>> client = HttpClient(base_url="https://api.example.com")
        >>> response = client.request("GET", "/users", params={"page": "1"})
        >>> response = client.request("POST", "/users", data={"name": "Alice"})
        """
        # Build URL
        url = f"{self.base_url}{path}" if self.base_url else path
        
        # Add query parameters
        if params:
            query_string = urllib.parse.urlencode(params)
            url = f"{url}?{query_string}"
        
        # Merge headers
        request_headers = {**self.default_headers, **(headers or {})}
        
        # Prepare request body
        body = None
        if data is not None:
            body = json.dumps(data).encode('utf-8')
            request_headers.setdefault('Content-Type', 'application/json')
        
        # Create request
        req = urllib.request.Request(
            url,
            data=body,
            headers=request_headers,
            method=method.upper()
        )
        
        # Execute request
        request_timeout = timeout if timeout is not None else self.timeout
        
        try:
            with urllib.request.urlopen(req, timeout=request_timeout) as resp:
                return HttpResponse(
                    status_code=resp.status,
                    headers=dict(resp.headers),
                    body=resp.read(),
                    url=resp.url
                )
        except urllib.error.HTTPError as e:
            response = HttpResponse(
                status_code=e.code,
                headers=dict(e.headers) if e.headers else {},
                body=e.read() if e.fp else b"",
                url=url
            )
            raise HttpError(
                message=str(e.reason),
                status_code=e.code,
                response=response
            ) from e
        except urllib.error.URLError as e:
            raise HttpError(message=str(e.reason)) from e
    
    def get(
        self,
        path: str,
        headers: Optional[Dict[str, str]] = None,
        params: Optional[Dict[str, str]] = None,
        timeout: Optional[float] = None
    ) -> HttpResponse:
        """
        Perform a GET request.
        
        Parameters
        ----------
        path : str
            URL path (appended to base_url if set).
        headers : dict, optional
            Additional headers for this request.
        params : dict, optional
            Query parameters to append to the URL.
        timeout : float, optional
            Request timeout in seconds.
        
        Returns
        -------
        HttpResponse
            The response from the server.
        
        Examples
        --------
        >>> client = HttpClient(base_url="https://api.example.com")
        >>> response = client.get("/users")
        >>> response = client.get("/users", params={"page": "1", "limit": "10"})
        """
        return self.request("GET", path, headers=headers, params=params, timeout=timeout)
    
    def post(
        self,
        path: str,
        data: Optional[Union[Dict, List]] = None,
        headers: Optional[Dict[str, str]] = None,
        timeout: Optional[float] = None
    ) -> HttpResponse:
        """
        Perform a POST request.
        
        Parameters
        ----------
        path : str
            URL path (appended to base_url if set).
        data : dict or list, optional
            Request body to be JSON-encoded.
        headers : dict, optional
            Additional headers for this request.
        timeout : float, optional
            Request timeout in seconds.
        
        Returns
        -------
        HttpResponse
            The response from the server.
        
        Examples
        --------
        >>> client = HttpClient(base_url="https://api.example.com")
        >>> response = client.post("/users", data={"name": "Alice", "email": "alice@example.com"})
        """
        return self.request("POST", path, data=data, headers=headers, timeout=timeout)
    
    def put(
        self,
        path: str,
        data: Optional[Union[Dict, List]] = None,
        headers: Optional[Dict[str, str]] = None,
        timeout: Optional[float] = None
    ) -> HttpResponse:
        """
        Perform a PUT request.
        
        Parameters
        ----------
        path : str
            URL path (appended to base_url if set).
        data : dict or list, optional
            Request body to be JSON-encoded.
        headers : dict, optional
            Additional headers for this request.
        timeout : float, optional
            Request timeout in seconds.
        
        Returns
        -------
        HttpResponse
            The response from the server.
        
        Examples
        --------
        >>> client = HttpClient(base_url="https://api.example.com")
        >>> response = client.put("/users/1", data={"name": "Alice Updated"})
        """
        return self.request("PUT", path, data=data, headers=headers, timeout=timeout)
    
    def delete(
        self,
        path: str,
        headers: Optional[Dict[str, str]] = None,
        timeout: Optional[float] = None
    ) -> HttpResponse:
        """
        Perform a DELETE request.
        
        Parameters
        ----------
        path : str
            URL path (appended to base_url if set).
        headers : dict, optional
            Additional headers for this request.
        timeout : float, optional
            Request timeout in seconds.
        
        Returns
        -------
        HttpResponse
            The response from the server.
        
        Examples
        --------
        >>> client = HttpClient(base_url="https://api.example.com")
        >>> response = client.delete("/users/1")
        """
        return self.request("DELETE", path, headers=headers, timeout=timeout)


# Module-level convenience functions using a default client
_default_client = HttpClient()


def request(
    method: str,
    url: str,
    data: Optional[Union[Dict, List]] = None,
    headers: Optional[Dict[str, str]] = None,
    params: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse:
    """
    Perform a generic HTTP request.
    
    A convenience function for making one-off HTTP requests without
    creating an HttpClient instance.
    
    Parameters
    ----------
    method : str
        HTTP method (GET, POST, PUT, DELETE, PATCH, etc.).
    url : str
        The full URL to request.
    data : dict or list, optional
        Request body to be JSON-encoded.
    headers : dict, optional
        Request headers.
    params : dict, optional
        Query parameters to append to the URL.
    timeout : float, default 30.0
        Request timeout in seconds.
    
    Returns
    -------
    HttpResponse
        The response from the server.
    
    Examples
    --------
    >>> response = request("GET", "https://api.example.com/users")
    >>> response = request("POST", "https://api.example.com/users", 
    ...                    data={"name": "Alice"})
    
    See Also
    --------
    get : Convenience function for GET requests.
    post : Convenience function for POST requests.
    HttpClient : Reusable client with configuration.
    """
    client = HttpClient(timeout=timeout)
    return client.request(method, url, data=data, headers=headers, params=params)


def get(
    url: str,
    headers: Optional[Dict[str, str]] = None,
    params: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse:
    """
    Perform a GET request.
    
    A convenience function for making GET requests without creating
    an HttpClient instance.
    
    Parameters
    ----------
    url : str
        The full URL to request.
    headers : dict, optional
        Request headers.
    params : dict, optional
        Query parameters to append to the URL.
    timeout : float, default 30.0
        Request timeout in seconds.
    
    Returns
    -------
    HttpResponse
        The response from the server.
    
    Examples
    --------
    >>> response = get("https://api.example.com/users")
    >>> if response.ok:
    ...     users = response.json()
    
    >>> response = get(
    ...     "https://api.example.com/users",
    ...     params={"page": "1", "limit": "10"},
    ...     headers={"Authorization": "Bearer token123"}
    ... )
    
    See Also
    --------
    post : Convenience function for POST requests.
    HttpClient : Reusable client with configuration.
    """
    return request("GET", url, headers=headers, params=params, timeout=timeout)


def post(
    url: str,
    data: Optional[Union[Dict, List]] = None,
    headers: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse:
    """
    Perform a POST request with JSON data.
    
    A convenience function for making POST requests without creating
    an HttpClient instance. The data parameter is automatically
    JSON-encoded.
    
    Parameters
    ----------
    url : str
        The full URL to request.
    data : dict or list, optional
        Request body to be JSON-encoded.
    headers : dict, optional
        Request headers.
    timeout : float, default 30.0
        Request timeout in seconds.
    
    Returns
    -------
    HttpResponse
        The response from the server.
    
    Examples
    --------
    >>> response = post(
    ...     "https://api.example.com/users",
    ...     data={"name": "Alice", "email": "alice@example.com"}
    ... )
    >>> if response.ok:
    ...     new_user = response.json()
    
    See Also
    --------
    get : Convenience function for GET requests.
    put : Convenience function for PUT requests.
    HttpClient : Reusable client with configuration.
    """
    return request("POST", url, data=data, headers=headers, timeout=timeout)


def put(
    url: str,
    data: Optional[Union[Dict, List]] = None,
    headers: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse:
    """
    Perform a PUT request with JSON data.
    
    A convenience function for making PUT requests without creating
    an HttpClient instance.
    
    Parameters
    ----------
    url : str
        The full URL to request.
    data : dict or list, optional
        Request body to be JSON-encoded.
    headers : dict, optional
        Request headers.
    timeout : float, default 30.0
        Request timeout in seconds.
    
    Returns
    -------
    HttpResponse
        The response from the server.
    
    Examples
    --------
    >>> response = put(
    ...     "https://api.example.com/users/1",
    ...     data={"name": "Alice Updated"}
    ... )
    
    See Also
    --------
    post : Convenience function for POST requests.
    delete : Convenience function for DELETE requests.
    """
    return request("PUT", url, data=data, headers=headers, timeout=timeout)


def delete(
    url: str,
    headers: Optional[Dict[str, str]] = None,
    timeout: float = 30.0
) -> HttpResponse:
    """
    Perform a DELETE request.
    
    A convenience function for making DELETE requests without creating
    an HttpClient instance.
    
    Parameters
    ----------
    url : str
        The full URL to request.
    headers : dict, optional
        Request headers.
    timeout : float, default 30.0
        Request timeout in seconds.
    
    Returns
    -------
    HttpResponse
        The response from the server.
    
    Examples
    --------
    >>> response = delete("https://api.example.com/users/1")
    >>> if response.ok:
    ...     print("User deleted successfully")
    
    See Also
    --------
    get : Convenience function for GET requests.
    put : Convenience function for PUT requests.
    """
    return request("DELETE", url, headers=headers, timeout=timeout)
