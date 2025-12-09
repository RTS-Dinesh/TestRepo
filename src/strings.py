"""
Strings Module
==============

This module provides string manipulation and formatting utilities for common
text processing tasks. Functions include case conversion, slugification,
truncation, and text sanitization.

Functions
---------
slugify(text, separator, lowercase)
    Convert text to URL-friendly slug format.
truncate(text, max_length, suffix)
    Truncate text to a maximum length with optional suffix.
to_camel_case(text)
    Convert text to camelCase format.
to_snake_case(text)
    Convert text to snake_case format.
to_title_case(text)
    Convert text to Title Case format.
strip_html(html)
    Remove HTML tags from text.
mask_sensitive(text, visible_chars, mask_char)
    Mask sensitive data showing only specified characters.

Examples
--------
>>> from src.strings import slugify, truncate, to_camel_case
>>> slugify("Hello World!")
'hello-world'
>>> truncate("This is a long text", max_length=10)
'This is...'
>>> to_camel_case("hello_world")
'helloWorld'
"""

import re
from typing import Optional
import unicodedata

__all__ = [
    "slugify",
    "truncate",
    "to_camel_case",
    "to_snake_case",
    "to_title_case",
    "strip_html",
    "mask_sensitive",
]


def slugify(
    text: str,
    separator: str = "-",
    lowercase: bool = True
) -> str:
    """
    Convert text to URL-friendly slug format.
    
    Transforms a string into a URL-safe slug by removing special characters,
    replacing spaces with a separator, and optionally converting to lowercase.
    Unicode characters are transliterated to ASCII equivalents when possible.
    
    Parameters
    ----------
    text : str
        The input text to convert to a slug.
    separator : str, default "-"
        The character to use as a word separator.
    lowercase : bool, default True
        If True, convert the slug to lowercase.
    
    Returns
    -------
    str
        The slugified text suitable for use in URLs.
    
    Examples
    --------
    >>> slugify("Hello World!")
    'hello-world'
    >>> slugify("Hello World!", separator="_")
    'hello_world'
    >>> slugify("Hello World!", lowercase=False)
    'Hello-World'
    >>> slugify("  Multiple   Spaces  ")
    'multiple-spaces'
    >>> slugify("Special @#$ Characters!")
    'special-characters'
    >>> slugify("Café Résumé")
    'cafe-resume'
    
    Notes
    -----
    - Leading and trailing separators are removed
    - Multiple consecutive separators are collapsed to one
    - Unicode characters are normalized using NFKD normalization
    
    See Also
    --------
    to_snake_case : Convert to snake_case format.
    to_camel_case : Convert to camelCase format.
    """
    if not text:
        return ""
    
    # Normalize unicode characters (é -> e, etc.)
    text = unicodedata.normalize('NFKD', text)
    text = text.encode('ascii', 'ignore').decode('ascii')
    
    # Convert to lowercase if requested
    if lowercase:
        text = text.lower()
    
    # Replace non-alphanumeric characters with separator
    text = re.sub(r'[^a-zA-Z0-9]+', separator, text)
    
    # Remove leading/trailing separators and collapse multiple separators
    text = re.sub(f'{re.escape(separator)}+', separator, text)
    text = text.strip(separator)
    
    return text


def truncate(
    text: str,
    max_length: int,
    suffix: str = "..."
) -> str:
    """
    Truncate text to a maximum length with optional suffix.
    
    Shortens text that exceeds the maximum length, appending a suffix
    to indicate truncation. The total length including suffix will not
    exceed max_length.
    
    Parameters
    ----------
    text : str
        The input text to truncate.
    max_length : int
        The maximum total length of the output string, including suffix.
        Must be greater than the length of the suffix.
    suffix : str, default "..."
        The string to append when text is truncated.
    
    Returns
    -------
    str
        The truncated text with suffix if truncation occurred,
        or the original text if it was already short enough.
    
    Raises
    ------
    ValueError
        If max_length is less than or equal to the suffix length.
    
    Examples
    --------
    >>> truncate("This is a long text", max_length=10)
    'This is...'
    >>> truncate("Short", max_length=10)
    'Short'
    >>> truncate("Hello World", max_length=8, suffix="…")
    'Hello W…'
    >>> truncate("Hello", max_length=5)
    'Hello'
    
    Notes
    -----
    The function attempts to truncate at word boundaries when possible,
    but prioritizes respecting the max_length constraint.
    
    See Also
    --------
    mask_sensitive : Mask parts of sensitive text.
    """
    if not text:
        return ""
    
    if max_length <= len(suffix):
        raise ValueError(
            f"max_length ({max_length}) must be greater than "
            f"suffix length ({len(suffix)})"
        )
    
    if len(text) <= max_length:
        return text
    
    # Truncate and add suffix
    truncated_length = max_length - len(suffix)
    return text[:truncated_length] + suffix


def to_camel_case(text: str) -> str:
    """
    Convert text to camelCase format.
    
    Transforms a string from various formats (snake_case, kebab-case,
    space-separated, etc.) to camelCase where the first word is lowercase
    and subsequent words are capitalized.
    
    Parameters
    ----------
    text : str
        The input text to convert.
    
    Returns
    -------
    str
        The text converted to camelCase.
    
    Examples
    --------
    >>> to_camel_case("hello_world")
    'helloWorld'
    >>> to_camel_case("hello-world")
    'helloWorld'
    >>> to_camel_case("Hello World")
    'helloWorld'
    >>> to_camel_case("HelloWorld")
    'helloWorld'
    >>> to_camel_case("already_camelCase")
    'alreadyCamelCase'
    >>> to_camel_case("API_response_handler")
    'apiResponseHandler'
    
    See Also
    --------
    to_snake_case : Convert to snake_case format.
    to_title_case : Convert to Title Case format.
    """
    if not text:
        return ""
    
    # Split on non-alphanumeric characters and capital letters
    words = re.split(r'[-_\s]+|(?<=[a-z])(?=[A-Z])', text)
    words = [w for w in words if w]  # Remove empty strings
    
    if not words:
        return ""
    
    # First word lowercase, rest title case
    result = words[0].lower()
    for word in words[1:]:
        result += word.capitalize()
    
    return result


def to_snake_case(text: str) -> str:
    """
    Convert text to snake_case format.
    
    Transforms a string from various formats (camelCase, PascalCase,
    kebab-case, space-separated, etc.) to snake_case where words are
    lowercase and separated by underscores.
    
    Parameters
    ----------
    text : str
        The input text to convert.
    
    Returns
    -------
    str
        The text converted to snake_case.
    
    Examples
    --------
    >>> to_snake_case("helloWorld")
    'hello_world'
    >>> to_snake_case("HelloWorld")
    'hello_world'
    >>> to_snake_case("hello-world")
    'hello_world'
    >>> to_snake_case("Hello World")
    'hello_world'
    >>> to_snake_case("APIResponseHandler")
    'api_response_handler'
    >>> to_snake_case("already_snake_case")
    'already_snake_case'
    
    See Also
    --------
    to_camel_case : Convert to camelCase format.
    slugify : Convert to URL-friendly slug format.
    """
    if not text:
        return ""
    
    # Insert underscore before uppercase letters (for camelCase)
    text = re.sub(r'([a-z0-9])([A-Z])', r'\1_\2', text)
    
    # Handle consecutive uppercase (e.g., "API" -> "API_" before the next word)
    text = re.sub(r'([A-Z]+)([A-Z][a-z])', r'\1_\2', text)
    
    # Replace non-alphanumeric with underscore
    text = re.sub(r'[-\s]+', '_', text)
    
    # Lowercase and clean up multiple underscores
    text = text.lower()
    text = re.sub(r'_+', '_', text)
    text = text.strip('_')
    
    return text


def to_title_case(text: str) -> str:
    """
    Convert text to Title Case format.
    
    Transforms a string to Title Case where each word's first letter
    is capitalized. Handles common articles and prepositions according
    to standard title case conventions.
    
    Parameters
    ----------
    text : str
        The input text to convert.
    
    Returns
    -------
    str
        The text converted to Title Case.
    
    Examples
    --------
    >>> to_title_case("hello world")
    'Hello World'
    >>> to_title_case("the quick brown fox")
    'The Quick Brown Fox'
    >>> to_title_case("hello_world")
    'Hello World'
    >>> to_title_case("a tale of two cities")
    'A Tale of Two Cities'
    
    Notes
    -----
    Common small words (articles, prepositions, conjunctions) are kept
    lowercase unless they are the first or last word. These include:
    a, an, the, and, but, or, for, nor, on, at, to, by, of, in, etc.
    
    See Also
    --------
    to_camel_case : Convert to camelCase format.
    to_snake_case : Convert to snake_case format.
    """
    if not text:
        return ""
    
    # Small words that should be lowercase (unless first/last)
    small_words = {
        'a', 'an', 'the', 'and', 'but', 'or', 'for', 'nor',
        'on', 'at', 'to', 'by', 'of', 'in', 'as', 'is'
    }
    
    # Split on separators
    words = re.split(r'[-_\s]+', text)
    words = [w for w in words if w]
    
    if not words:
        return ""
    
    result = []
    for i, word in enumerate(words):
        # First and last word always capitalized
        if i == 0 or i == len(words) - 1:
            result.append(word.capitalize())
        elif word.lower() in small_words:
            result.append(word.lower())
        else:
            result.append(word.capitalize())
    
    return ' '.join(result)


def strip_html(html: str) -> str:
    """
    Remove HTML tags from text.
    
    Strips all HTML/XML tags from a string, leaving only the text content.
    Also decodes common HTML entities to their character equivalents.
    
    Parameters
    ----------
    html : str
        The HTML string to process.
    
    Returns
    -------
    str
        The text content with all HTML tags removed.
    
    Examples
    --------
    >>> strip_html("<p>Hello <b>World</b>!</p>")
    'Hello World!'
    >>> strip_html("<div class='test'>Content</div>")
    'Content'
    >>> strip_html("No tags here")
    'No tags here'
    >>> strip_html("<script>alert('xss')</script>Safe text")
    "alert('xss')Safe text"
    >>> strip_html("&lt;escaped&gt; &amp; entities")
    '<escaped> & entities'
    
    Notes
    -----
    This function performs basic HTML stripping and is suitable for
    display purposes. For security-critical applications (e.g., preventing
    XSS attacks), use a dedicated HTML sanitization library.
    
    See Also
    --------
    truncate : Truncate text to a maximum length.
    """
    if not html:
        return ""
    
    # Remove HTML tags
    text = re.sub(r'<[^>]+>', '', html)
    
    # Decode common HTML entities
    entities = {
        '&lt;': '<',
        '&gt;': '>',
        '&amp;': '&',
        '&quot;': '"',
        '&#39;': "'",
        '&apos;': "'",
        '&nbsp;': ' ',
    }
    
    for entity, char in entities.items():
        text = text.replace(entity, char)
    
    return text


def mask_sensitive(
    text: str,
    visible_chars: int = 4,
    mask_char: str = "*"
) -> str:
    """
    Mask sensitive data showing only specified characters.
    
    Replaces most characters in a string with a mask character,
    showing only the last few characters. Useful for displaying
    partial credit card numbers, passwords, API keys, etc.
    
    Parameters
    ----------
    text : str
        The sensitive text to mask.
    visible_chars : int, default 4
        Number of characters to leave visible at the end.
        If greater than or equal to text length, no masking is applied.
    mask_char : str, default "*"
        The character to use for masking. Should be a single character.
    
    Returns
    -------
    str
        The masked text with only the last visible_chars shown.
    
    Examples
    --------
    >>> mask_sensitive("4532015112830366")
    '************0366'
    >>> mask_sensitive("secret_api_key_12345", visible_chars=5)
    '***************12345'
    >>> mask_sensitive("password", visible_chars=2, mask_char="#")
    '######rd'
    >>> mask_sensitive("short", visible_chars=10)
    'short'
    >>> mask_sensitive("abc")
    'abc'
    
    Notes
    -----
    For very short strings (3 characters or less), the original text
    is returned unmasked since masking would provide little benefit.
    
    See Also
    --------
    truncate : Truncate text to a maximum length.
    """
    if not text:
        return ""
    
    # Don't mask very short strings
    if len(text) <= 3 or visible_chars >= len(text):
        return text
    
    masked_length = len(text) - visible_chars
    return (mask_char * masked_length) + text[-visible_chars:]
