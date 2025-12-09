"""
TestRepo - A Comprehensive Python Utility Library
==================================================

This package provides a collection of utility functions and classes for common
programming tasks including data validation, string manipulation, collection
operations, and HTTP client utilities.

Modules
-------
validators
    Data validation utilities for common types (email, URL, phone, etc.)
strings
    String manipulation and formatting utilities
collections
    Enhanced collection operations and data structures
http_client
    Simple HTTP client for making API requests

Quick Start
-----------
>>> from src import validators, strings, collections
>>> validators.is_valid_email("user@example.com")
True
>>> strings.slugify("Hello World!")
'hello-world'
>>> collections.flatten([[1, 2], [3, 4]])
[1, 2, 3, 4]

Version
-------
1.0.0
"""

__version__ = "1.0.0"
__author__ = "TestRepo Contributors"
__license__ = "MIT"

from . import validators
from . import strings
from . import collections
from . import http_client

__all__ = [
    "validators",
    "strings", 
    "collections",
    "http_client",
    "__version__",
    "__author__",
    "__license__",
]
