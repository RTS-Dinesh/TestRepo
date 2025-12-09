"""
Collections Module
==================

This module provides enhanced collection operations and data structures
for working with lists, dictionaries, and other iterables. Functions
include flattening, chunking, grouping, and advanced filtering operations.

Classes
-------
OrderedSet
    A set that maintains insertion order.

Functions
---------
flatten(iterable, depth)
    Flatten nested iterables to a specified depth.
chunk(iterable, size)
    Split an iterable into chunks of a specified size.
group_by(iterable, key_func)
    Group items by a key function.
unique(iterable, key_func)
    Remove duplicates while preserving order.
deep_merge(dict1, dict2)
    Recursively merge two dictionaries.
pluck(items, key)
    Extract a specific key from a list of dictionaries.
partition(iterable, predicate)
    Split an iterable into two lists based on a predicate.

Examples
--------
>>> from src.collections import flatten, chunk, group_by
>>> flatten([[1, 2], [3, [4, 5]]])
[1, 2, 3, [4, 5]]
>>> list(chunk([1, 2, 3, 4, 5], 2))
[[1, 2], [3, 4], [5]]
>>> group_by([{'type': 'a', 'val': 1}, {'type': 'a', 'val': 2}], lambda x: x['type'])
{'a': [{'type': 'a', 'val': 1}, {'type': 'a', 'val': 2}]}
"""

from typing import (
    Any,
    Callable,
    Dict,
    Iterable,
    Iterator,
    List,
    Optional,
    Tuple,
    TypeVar,
    Union,
)
from collections.abc import MutableSet

__all__ = [
    "OrderedSet",
    "flatten",
    "chunk",
    "group_by",
    "unique",
    "deep_merge",
    "pluck",
    "partition",
]

T = TypeVar('T')
K = TypeVar('K')
V = TypeVar('V')


class OrderedSet(MutableSet):
    """
    A set that maintains insertion order.
    
    OrderedSet combines the uniqueness property of sets with the ordering
    property of lists. Elements are stored in the order they were first
    added, and duplicate insertions are ignored.
    
    Parameters
    ----------
    iterable : iterable, optional
        An optional iterable of elements to initialize the set.
    
    Attributes
    ----------
    items : dict
        Internal dictionary storing elements as keys (values are None).
    
    Examples
    --------
    >>> s = OrderedSet([3, 1, 4, 1, 5, 9, 2, 6, 5])
    >>> list(s)
    [3, 1, 4, 5, 9, 2, 6]
    
    >>> s.add(0)
    >>> s.add(3)  # Already exists, no change
    >>> list(s)
    [3, 1, 4, 5, 9, 2, 6, 0]
    
    >>> 4 in s
    True
    >>> s.discard(4)
    >>> 4 in s
    False
    
    >>> OrderedSet([1, 2, 3]) | OrderedSet([3, 4, 5])
    OrderedSet([1, 2, 3, 4, 5])
    
    Notes
    -----
    - Time complexity for add, remove, and contains is O(1) average case
    - Memory usage is higher than a regular set due to ordering overhead
    - Iteration is O(n) where n is the number of elements
    
    See Also
    --------
    unique : Remove duplicates from an iterable while preserving order.
    """
    
    def __init__(self, iterable: Optional[Iterable[T]] = None):
        self.items: Dict[T, None] = {}
        if iterable is not None:
            for item in iterable:
                self.add(item)
    
    def add(self, value: T) -> None:
        """
        Add an element to the set.
        
        If the element already exists, the set is unchanged and the
        original insertion order is preserved.
        
        Parameters
        ----------
        value : T
            The element to add.
        
        Examples
        --------
        >>> s = OrderedSet()
        >>> s.add(1)
        >>> s.add(2)
        >>> s.add(1)  # No effect
        >>> list(s)
        [1, 2]
        """
        self.items[value] = None
    
    def discard(self, value: T) -> None:
        """
        Remove an element from the set if present.
        
        If the element is not in the set, do nothing.
        
        Parameters
        ----------
        value : T
            The element to remove.
        
        Examples
        --------
        >>> s = OrderedSet([1, 2, 3])
        >>> s.discard(2)
        >>> list(s)
        [1, 3]
        >>> s.discard(5)  # No error
        """
        self.items.pop(value, None)
    
    def __contains__(self, value: object) -> bool:
        """Check if an element is in the set."""
        return value in self.items
    
    def __iter__(self) -> Iterator[T]:
        """Iterate over elements in insertion order."""
        return iter(self.items.keys())
    
    def __len__(self) -> int:
        """Return the number of elements in the set."""
        return len(self.items)
    
    def __repr__(self) -> str:
        """Return a string representation of the OrderedSet."""
        return f"OrderedSet({list(self.items.keys())})"
    
    def __eq__(self, other: object) -> bool:
        """Check equality with another OrderedSet or set."""
        if isinstance(other, OrderedSet):
            return list(self) == list(other)
        if isinstance(other, set):
            return set(self) == other
        return False


def flatten(
    iterable: Iterable[Any],
    depth: int = 1
) -> List[Any]:
    """
    Flatten nested iterables to a specified depth.
    
    Recursively unpacks nested lists, tuples, and other iterables up to
    the specified depth, returning a single flat list.
    
    Parameters
    ----------
    iterable : iterable
        The nested iterable to flatten.
    depth : int, default 1
        Maximum nesting depth to flatten. Use -1 for unlimited depth.
        A depth of 0 returns the original items as a list.
    
    Returns
    -------
    list
        A flattened list of elements.
    
    Examples
    --------
    >>> flatten([[1, 2], [3, 4]])
    [1, 2, 3, 4]
    >>> flatten([[1, [2, 3]], [4, [5, 6]]])
    [1, [2, 3], 4, [5, 6]]
    >>> flatten([[1, [2, 3]], [4, [5, 6]]], depth=2)
    [1, 2, 3, 4, 5, 6]
    >>> flatten([[[1]], [[2]]], depth=-1)  # Unlimited depth
    [1, 2]
    >>> flatten([1, 2, 3], depth=0)
    [1, 2, 3]
    
    Notes
    -----
    - Strings are not flattened (treated as atomic values)
    - Dictionaries are not flattened (only their keys would be returned)
    - Generator expressions are consumed during flattening
    
    See Also
    --------
    chunk : Split an iterable into fixed-size chunks.
    """
    result = []
    
    for item in iterable:
        # Don't flatten strings or dicts
        if isinstance(item, (str, bytes, dict)):
            result.append(item)
        elif hasattr(item, '__iter__') and depth != 0:
            # Recursively flatten
            new_depth = depth - 1 if depth > 0 else -1
            result.extend(flatten(item, depth=new_depth))
        else:
            result.append(item)
    
    return result


def chunk(
    iterable: Iterable[T],
    size: int
) -> Iterator[List[T]]:
    """
    Split an iterable into chunks of a specified size.
    
    Divides an iterable into smaller lists of the specified size.
    The last chunk may contain fewer elements if the iterable length
    is not evenly divisible by the chunk size.
    
    Parameters
    ----------
    iterable : iterable
        The iterable to split into chunks.
    size : int
        The maximum size of each chunk. Must be positive.
    
    Yields
    ------
    list
        Lists containing up to `size` elements each.
    
    Raises
    ------
    ValueError
        If size is less than 1.
    
    Examples
    --------
    >>> list(chunk([1, 2, 3, 4, 5], 2))
    [[1, 2], [3, 4], [5]]
    >>> list(chunk([1, 2, 3, 4, 5, 6], 3))
    [[1, 2, 3], [4, 5, 6]]
    >>> list(chunk([], 3))
    []
    >>> list(chunk(range(10), 4))
    [[0, 1, 2, 3], [4, 5, 6, 7], [8, 9]]
    
    Notes
    -----
    This function returns a generator, so it's memory-efficient for
    large iterables. Convert to a list if you need random access.
    
    See Also
    --------
    flatten : Flatten nested iterables.
    partition : Split into two groups based on a predicate.
    """
    if size < 1:
        raise ValueError(f"Chunk size must be positive, got {size}")
    
    items = list(iterable)
    for i in range(0, len(items), size):
        yield items[i:i + size]


def group_by(
    iterable: Iterable[T],
    key_func: Callable[[T], K]
) -> Dict[K, List[T]]:
    """
    Group items by a key function.
    
    Creates a dictionary where keys are the results of applying the
    key function to each item, and values are lists of items that
    produced that key.
    
    Parameters
    ----------
    iterable : iterable
        The items to group.
    key_func : callable
        A function that takes an item and returns a grouping key.
    
    Returns
    -------
    dict
        A dictionary mapping keys to lists of items.
    
    Examples
    --------
    >>> group_by([1, 2, 3, 4, 5, 6], lambda x: x % 2)
    {1: [1, 3, 5], 0: [2, 4, 6]}
    
    >>> users = [
    ...     {'name': 'Alice', 'dept': 'Engineering'},
    ...     {'name': 'Bob', 'dept': 'Sales'},
    ...     {'name': 'Carol', 'dept': 'Engineering'},
    ... ]
    >>> group_by(users, lambda u: u['dept'])
    {'Engineering': [{'name': 'Alice', 'dept': 'Engineering'}, 
                     {'name': 'Carol', 'dept': 'Engineering'}],
     'Sales': [{'name': 'Bob', 'dept': 'Sales'}]}
    
    >>> words = ['apple', 'banana', 'cherry', 'apricot']
    >>> group_by(words, lambda w: w[0])
    {'a': ['apple', 'apricot'], 'b': ['banana'], 'c': ['cherry']}
    
    Notes
    -----
    - Items are grouped in the order they appear in the input
    - The key function should return hashable values
    
    See Also
    --------
    partition : Split into exactly two groups.
    unique : Remove duplicates based on a key function.
    """
    result: Dict[K, List[T]] = {}
    
    for item in iterable:
        key = key_func(item)
        if key not in result:
            result[key] = []
        result[key].append(item)
    
    return result


def unique(
    iterable: Iterable[T],
    key_func: Optional[Callable[[T], Any]] = None
) -> List[T]:
    """
    Remove duplicates while preserving order.
    
    Returns a list with duplicate elements removed, keeping only the
    first occurrence of each element. Optionally uses a key function
    to determine uniqueness.
    
    Parameters
    ----------
    iterable : iterable
        The iterable to deduplicate.
    key_func : callable, optional
        A function to extract a comparison key from each element.
        If None, elements are compared directly.
    
    Returns
    -------
    list
        A list with duplicates removed, preserving first occurrences.
    
    Examples
    --------
    >>> unique([1, 2, 2, 3, 1, 4, 3])
    [1, 2, 3, 4]
    
    >>> unique(['Apple', 'apple', 'BANANA', 'banana'], key_func=str.lower)
    ['Apple', 'BANANA']
    
    >>> users = [
    ...     {'id': 1, 'name': 'Alice'},
    ...     {'id': 2, 'name': 'Bob'},
    ...     {'id': 1, 'name': 'Alice Updated'},
    ... ]
    >>> unique(users, key_func=lambda u: u['id'])
    [{'id': 1, 'name': 'Alice'}, {'id': 2, 'name': 'Bob'}]
    
    See Also
    --------
    OrderedSet : A set that maintains insertion order.
    group_by : Group items by a key function.
    """
    seen: set = set()
    result: List[T] = []
    
    for item in iterable:
        key = key_func(item) if key_func else item
        if key not in seen:
            seen.add(key)
            result.append(item)
    
    return result


def deep_merge(
    dict1: Dict[K, V],
    dict2: Dict[K, V]
) -> Dict[K, V]:
    """
    Recursively merge two dictionaries.
    
    Creates a new dictionary by deep merging dict2 into dict1.
    Nested dictionaries are merged recursively; other values
    from dict2 override those in dict1.
    
    Parameters
    ----------
    dict1 : dict
        The base dictionary.
    dict2 : dict
        The dictionary to merge into dict1.
    
    Returns
    -------
    dict
        A new dictionary with merged contents.
    
    Examples
    --------
    >>> deep_merge({'a': 1, 'b': 2}, {'b': 3, 'c': 4})
    {'a': 1, 'b': 3, 'c': 4}
    
    >>> deep_merge(
    ...     {'user': {'name': 'Alice', 'age': 30}},
    ...     {'user': {'age': 31, 'city': 'NYC'}}
    ... )
    {'user': {'name': 'Alice', 'age': 31, 'city': 'NYC'}}
    
    >>> deep_merge(
    ...     {'a': {'b': {'c': 1}}},
    ...     {'a': {'b': {'d': 2}}}
    ... )
    {'a': {'b': {'c': 1, 'd': 2}}}
    
    Notes
    -----
    - Original dictionaries are not modified
    - Non-dict values in dict2 always override dict1 values
    - Lists are not merged; dict2 lists replace dict1 lists
    
    See Also
    --------
    pluck : Extract values from a list of dictionaries.
    """
    result = dict1.copy()
    
    for key, value in dict2.items():
        if (
            key in result and
            isinstance(result[key], dict) and
            isinstance(value, dict)
        ):
            result[key] = deep_merge(result[key], value)
        else:
            result[key] = value
    
    return result


def pluck(
    items: Iterable[Dict[str, Any]],
    key: str
) -> List[Any]:
    """
    Extract a specific key from a list of dictionaries.
    
    Returns a list containing the value of the specified key from
    each dictionary in the input. Missing keys result in None values.
    
    Parameters
    ----------
    items : iterable of dict
        An iterable of dictionaries.
    key : str
        The key to extract from each dictionary.
    
    Returns
    -------
    list
        A list of values extracted from each dictionary.
    
    Examples
    --------
    >>> users = [
    ...     {'id': 1, 'name': 'Alice'},
    ...     {'id': 2, 'name': 'Bob'},
    ...     {'id': 3, 'name': 'Carol'},
    ... ]
    >>> pluck(users, 'name')
    ['Alice', 'Bob', 'Carol']
    >>> pluck(users, 'id')
    [1, 2, 3]
    
    >>> items = [{'a': 1}, {'a': 2, 'b': 3}, {'b': 4}]
    >>> pluck(items, 'a')
    [1, 2, None]
    
    Notes
    -----
    For nested key access, consider using a custom list comprehension
    or a library like `glom` for more complex extraction patterns.
    
    See Also
    --------
    group_by : Group items by a key function.
    deep_merge : Recursively merge dictionaries.
    """
    return [item.get(key) for item in items]


def partition(
    iterable: Iterable[T],
    predicate: Callable[[T], bool]
) -> Tuple[List[T], List[T]]:
    """
    Split an iterable into two lists based on a predicate.
    
    Divides elements into two groups: those for which the predicate
    returns True, and those for which it returns False.
    
    Parameters
    ----------
    iterable : iterable
        The items to partition.
    predicate : callable
        A function that returns True or False for each item.
    
    Returns
    -------
    tuple of (list, list)
        A tuple of (true_items, false_items) where true_items contains
        elements for which predicate returned True, and false_items
        contains the rest.
    
    Examples
    --------
    >>> partition([1, 2, 3, 4, 5, 6], lambda x: x % 2 == 0)
    ([2, 4, 6], [1, 3, 5])
    
    >>> partition(['apple', 'banana', 'cherry'], lambda s: len(s) > 5)
    (['banana', 'cherry'], ['apple'])
    
    >>> adults, minors = partition(
    ...     [{'name': 'Alice', 'age': 30}, {'name': 'Bob', 'age': 15}],
    ...     lambda u: u['age'] >= 18
    ... )
    >>> adults
    [{'name': 'Alice', 'age': 30}]
    >>> minors
    [{'name': 'Bob', 'age': 15}]
    
    See Also
    --------
    group_by : Group items into multiple groups.
    chunk : Split into fixed-size chunks.
    """
    true_items: List[T] = []
    false_items: List[T] = []
    
    for item in iterable:
        if predicate(item):
            true_items.append(item)
        else:
            false_items.append(item)
    
    return true_items, false_items
