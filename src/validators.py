"""
Validators Module
=================

This module provides a comprehensive set of data validation functions for common
data types and formats. All validators return boolean values indicating whether
the input meets the validation criteria.

Functions
---------
is_valid_email(email)
    Validate an email address format.
is_valid_url(url, allowed_schemes)
    Validate a URL format with optional scheme restrictions.
is_valid_phone(phone, country_code)
    Validate phone number format for different countries.
is_valid_credit_card(number)
    Validate credit card number using Luhn algorithm.
is_in_range(value, min_val, max_val, inclusive)
    Check if a numeric value falls within a specified range.

Examples
--------
>>> from src.validators import is_valid_email, is_valid_url
>>> is_valid_email("user@example.com")
True
>>> is_valid_url("https://example.com", allowed_schemes=["https"])
True
"""

import re
from typing import List, Optional, Union

__all__ = [
    "is_valid_email",
    "is_valid_url", 
    "is_valid_phone",
    "is_valid_credit_card",
    "is_in_range",
    "ValidationError",
]


class ValidationError(Exception):
    """
    Exception raised when validation fails.
    
    This exception is raised by validator functions when they encounter
    invalid input that cannot be processed.
    
    Attributes
    ----------
    message : str
        Human-readable description of the validation error.
    field : str, optional
        Name of the field that failed validation.
    value : any, optional
        The value that failed validation.
    
    Examples
    --------
    >>> raise ValidationError("Invalid email format", field="email", value="bad-email")
    ValidationError: Invalid email format (field: email, value: bad-email)
    """
    
    def __init__(
        self,
        message: str,
        field: Optional[str] = None,
        value: Optional[any] = None
    ):
        self.message = message
        self.field = field
        self.value = value
        super().__init__(self._format_message())
    
    def _format_message(self) -> str:
        """Format the error message with field and value context."""
        msg = self.message
        if self.field:
            msg += f" (field: {self.field}"
            if self.value is not None:
                msg += f", value: {self.value}"
            msg += ")"
        return msg


def is_valid_email(email: str) -> bool:
    """
    Validate an email address format.
    
    Checks if the provided string matches a valid email address pattern
    according to RFC 5322 simplified specification.
    
    Parameters
    ----------
    email : str
        The email address string to validate.
    
    Returns
    -------
    bool
        True if the email format is valid, False otherwise.
    
    Examples
    --------
    >>> is_valid_email("user@example.com")
    True
    >>> is_valid_email("user.name+tag@example.co.uk")
    True
    >>> is_valid_email("invalid-email")
    False
    >>> is_valid_email("@example.com")
    False
    >>> is_valid_email("")
    False
    
    Notes
    -----
    This function only validates the format of the email address.
    It does not verify that the email address actually exists or
    that the domain is valid.
    
    See Also
    --------
    is_valid_url : Validate URL format.
    """
    if not email or not isinstance(email, str):
        return False
    
    # RFC 5322 compliant email pattern (simplified)
    pattern = r'^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
    return bool(re.match(pattern, email))


def is_valid_url(
    url: str,
    allowed_schemes: Optional[List[str]] = None
) -> bool:
    """
    Validate a URL format with optional scheme restrictions.
    
    Checks if the provided string is a properly formatted URL. Optionally
    restricts validation to specific URL schemes (protocols).
    
    Parameters
    ----------
    url : str
        The URL string to validate.
    allowed_schemes : list of str, optional
        List of allowed URL schemes (e.g., ['http', 'https']).
        If None, allows 'http', 'https', and 'ftp' by default.
    
    Returns
    -------
    bool
        True if the URL format is valid and uses an allowed scheme,
        False otherwise.
    
    Examples
    --------
    >>> is_valid_url("https://example.com")
    True
    >>> is_valid_url("https://example.com/path?query=value")
    True
    >>> is_valid_url("ftp://files.example.com")
    True
    >>> is_valid_url("https://example.com", allowed_schemes=["https"])
    True
    >>> is_valid_url("http://example.com", allowed_schemes=["https"])
    False
    >>> is_valid_url("not-a-url")
    False
    
    Notes
    -----
    This function validates URL format but does not check if the URL
    is accessible or if the domain exists.
    
    See Also
    --------
    is_valid_email : Validate email format.
    """
    if not url or not isinstance(url, str):
        return False
    
    if allowed_schemes is None:
        allowed_schemes = ['http', 'https', 'ftp']
    
    # URL pattern
    pattern = (
        r'^(' + '|'.join(re.escape(s) for s in allowed_schemes) + r')://'
        r'[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?'
        r'(\.[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?)*'
        r'(:\d+)?'
        r'(/[^\s]*)?$'
    )
    return bool(re.match(pattern, url))


def is_valid_phone(
    phone: str,
    country_code: str = "US"
) -> bool:
    """
    Validate phone number format for different countries.
    
    Checks if the provided string matches a valid phone number pattern
    for the specified country.
    
    Parameters
    ----------
    phone : str
        The phone number string to validate. May include formatting
        characters like spaces, dashes, and parentheses.
    country_code : str, default "US"
        ISO 3166-1 alpha-2 country code. Currently supports:
        - "US": United States (10 digits, optional +1)
        - "UK": United Kingdom (10-11 digits, optional +44)
        - "DE": Germany (10-14 digits, optional +49)
    
    Returns
    -------
    bool
        True if the phone number format is valid for the specified
        country, False otherwise.
    
    Raises
    ------
    ValueError
        If an unsupported country code is provided.
    
    Examples
    --------
    >>> is_valid_phone("(555) 123-4567")
    True
    >>> is_valid_phone("+1-555-123-4567", country_code="US")
    True
    >>> is_valid_phone("+44 20 7946 0958", country_code="UK")
    True
    >>> is_valid_phone("12345")
    False
    
    Notes
    -----
    This function validates the format only and does not verify
    that the phone number is actually in service.
    """
    if not phone or not isinstance(phone, str):
        return False
    
    # Remove common formatting characters
    cleaned = re.sub(r'[\s\-\(\)\.]', '', phone)
    
    patterns = {
        "US": r'^(\+1)?[2-9]\d{9}$',
        "UK": r'^(\+44)?[1-9]\d{9,10}$',
        "DE": r'^(\+49)?[1-9]\d{9,13}$',
    }
    
    if country_code not in patterns:
        raise ValueError(
            f"Unsupported country code: {country_code}. "
            f"Supported codes: {list(patterns.keys())}"
        )
    
    return bool(re.match(patterns[country_code], cleaned))


def is_valid_credit_card(number: str) -> bool:
    """
    Validate credit card number using the Luhn algorithm.
    
    Checks if the provided credit card number passes the Luhn checksum
    validation, which is used by most major credit card companies.
    
    Parameters
    ----------
    number : str
        The credit card number to validate. May include spaces or
        dashes which will be removed before validation.
    
    Returns
    -------
    bool
        True if the credit card number passes Luhn validation,
        False otherwise.
    
    Examples
    --------
    >>> is_valid_credit_card("4532015112830366")  # Valid Visa test number
    True
    >>> is_valid_credit_card("4532-0151-1283-0366")
    True
    >>> is_valid_credit_card("1234567890123456")
    False
    
    Notes
    -----
    This function only validates the checksum. It does not verify:
    - Whether the card is actually issued
    - Whether the card is active or expired
    - The card's credit limit or status
    
    The Luhn algorithm works by:
    1. Starting from the rightmost digit, double every second digit
    2. If doubling results in a number > 9, subtract 9
    3. Sum all the digits
    4. If the total modulo 10 equals 0, the number is valid
    
    References
    ----------
    .. [1] https://en.wikipedia.org/wiki/Luhn_algorithm
    
    See Also
    --------
    is_in_range : Validate numeric ranges.
    """
    if not number or not isinstance(number, str):
        return False
    
    # Remove spaces and dashes
    cleaned = re.sub(r'[\s\-]', '', number)
    
    # Check that it's all digits and reasonable length
    if not cleaned.isdigit() or len(cleaned) < 13 or len(cleaned) > 19:
        return False
    
    # Luhn algorithm
    digits = [int(d) for d in cleaned]
    odd_digits = digits[-1::-2]
    even_digits = digits[-2::-2]
    
    checksum = sum(odd_digits)
    for digit in even_digits:
        doubled = digit * 2
        checksum += doubled - 9 if doubled > 9 else doubled
    
    return checksum % 10 == 0


def is_in_range(
    value: Union[int, float],
    min_val: Optional[Union[int, float]] = None,
    max_val: Optional[Union[int, float]] = None,
    inclusive: bool = True
) -> bool:
    """
    Check if a numeric value falls within a specified range.
    
    Validates that a number is between minimum and maximum bounds.
    Either bound can be None to indicate no limit in that direction.
    
    Parameters
    ----------
    value : int or float
        The numeric value to check.
    min_val : int or float, optional
        The minimum allowed value. If None, no lower bound is applied.
    max_val : int or float, optional
        The maximum allowed value. If None, no upper bound is applied.
    inclusive : bool, default True
        If True, the bounds are inclusive (>= and <=).
        If False, the bounds are exclusive (> and <).
    
    Returns
    -------
    bool
        True if the value is within the specified range, False otherwise.
    
    Examples
    --------
    >>> is_in_range(5, min_val=0, max_val=10)
    True
    >>> is_in_range(10, min_val=0, max_val=10, inclusive=True)
    True
    >>> is_in_range(10, min_val=0, max_val=10, inclusive=False)
    False
    >>> is_in_range(5, min_val=0)  # No upper bound
    True
    >>> is_in_range(-5, min_val=0)
    False
    >>> is_in_range(3.14, min_val=3.0, max_val=4.0)
    True
    
    See Also
    --------
    is_valid_credit_card : Validate credit card numbers.
    """
    if not isinstance(value, (int, float)):
        return False
    
    if min_val is not None:
        if inclusive:
            if value < min_val:
                return False
        else:
            if value <= min_val:
                return False
    
    if max_val is not None:
        if inclusive:
            if value > max_val:
                return False
        else:
            if value >= max_val:
                return False
    
    return True
