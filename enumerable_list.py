from __future__ import annotations

from collections.abc import Callable, Iterable, Iterator, MutableSequence
from typing import Any, Generic, TypeVar

T = TypeVar("T")
U = TypeVar("U")

_MISSING = object()


class EnumerableList(MutableSequence[T], Generic[T]):
    """List-like collection with chainable enumerable helpers."""

    def __init__(self, items: Iterable[T] | None = None) -> None:
        self._items = list(items) if items is not None else []

    @classmethod
    def from_iterable(cls, items: Iterable[T]) -> "EnumerableList[T]":
        return cls(items)

    def __len__(self) -> int:
        return len(self._items)

    def __getitem__(self, index: int) -> T:
        return self._items[index]

    def __setitem__(self, index: int, value: T) -> None:
        self._items[index] = value

    def __delitem__(self, index: int) -> None:
        del self._items[index]

    def insert(self, index: int, value: T) -> None:
        self._items.insert(index, value)

    def __iter__(self) -> Iterator[T]:
        return iter(self._items)

    def __repr__(self) -> str:
        return f"EnumerableList({self._items!r})"

    def to_list(self) -> list[T]:
        return list(self._items)

    def map(self, func: Callable[[T], U]) -> "EnumerableList[U]":
        return EnumerableList(func(item) for item in self._items)

    def select(self, func: Callable[[T], U]) -> "EnumerableList[U]":
        return self.map(func)

    def filter(self, predicate: Callable[[T], bool]) -> "EnumerableList[T]":
        return EnumerableList(item for item in self._items if predicate(item))

    def where(self, predicate: Callable[[T], bool]) -> "EnumerableList[T]":
        return self.filter(predicate)

    def reduce(self, func: Callable[[U, T], U], initial: Any = _MISSING) -> U:
        iterator = iter(self._items)
        if initial is _MISSING:
            try:
                acc = next(iterator)
            except StopIteration as exc:
                raise ValueError(
                    "reduce() of empty EnumerableList with no initial value"
                ) from exc
        else:
            acc = initial
        for item in iterator:
            acc = func(acc, item)
        return acc

    def first(self, default: Any = _MISSING) -> T:
        if self._items:
            return self._items[0]
        if default is _MISSING:
            raise ValueError("EnumerableList is empty")
        return default

    def last(self, default: Any = _MISSING) -> T:
        if self._items:
            return self._items[-1]
        if default is _MISSING:
            raise ValueError("EnumerableList is empty")
        return default

    def any(self, predicate: Callable[[T], bool] | None = None) -> bool:
        if predicate is None:
            return bool(self._items)
        return any(predicate(item) for item in self._items)

    def all(self, predicate: Callable[[T], bool] | None = None) -> bool:
        if predicate is None:
            return all(self._items)
        return all(predicate(item) for item in self._items)

    def count(self, predicate: Callable[[T], bool] | None = None) -> int:
        if predicate is None:
            return len(self._items)
        return sum(1 for item in self._items if predicate(item))

    def distinct(self, key: Callable[[T], Any] | None = None) -> "EnumerableList[T]":
        key_func = key if key is not None else lambda item: item
        seen_hashable: set[Any] = set()
        seen_unhashable: list[Any] = []
        result: list[T] = []
        for item in self._items:
            marker = key_func(item)
            try:
                if marker in seen_hashable:
                    continue
                seen_hashable.add(marker)
            except TypeError:
                if marker in seen_unhashable:
                    continue
                seen_unhashable.append(marker)
            result.append(item)
        return EnumerableList(result)

    def chunk(self, size: int) -> "EnumerableList[list[T]]":
        if size <= 0:
            raise ValueError("chunk size must be positive")
        return EnumerableList(
            self._items[i : i + size] for i in range(0, len(self._items), size)
        )

    def for_each(self, func: Callable[[T], None]) -> None:
        for item in self._items:
            func(item)
