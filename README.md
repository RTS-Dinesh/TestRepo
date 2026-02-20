# TestRepo

## EnumerableList

`EnumerableList` is a small, list-like collection that adds chainable enumerable
helpers (map/filter/reduce style) while still behaving like a normal list.

```python
from enumerable_list import EnumerableList

numbers = EnumerableList([1, 2, 3, 4, 5])
evens = numbers.where(lambda n: n % 2 == 0)
squares = evens.select(lambda n: n * n)
total = squares.reduce(lambda acc, n: acc + n, 0)
```
