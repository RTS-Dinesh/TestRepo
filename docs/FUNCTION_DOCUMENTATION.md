# Function Documentation Guide

## Table of Contents

1. [Overview](#overview)
2. [Documentation Standards](#documentation-standards)
3. [JavaScript/TypeScript Functions](#javascripttypescript-functions)
4. [Python Functions](#python-functions)
5. [Java Functions](#java-functions)
6. [Go Functions](#go-functions)
7. [Async Functions](#async-functions)
8. [Higher-Order Functions](#higher-order-functions)
9. [Utility Functions](#utility-functions)
10. [Best Practices](#best-practices)

## Overview

This guide provides comprehensive standards and examples for documenting functions across different programming languages. Well-documented functions improve code maintainability, collaboration, and developer experience.

## Documentation Standards

Every function should include:

1. **Description:** Clear explanation of what the function does
2. **Parameters:** Type, name, and description of each parameter
3. **Return Value:** Type and description of the return value
4. **Exceptions/Errors:** Any errors or exceptions that might be thrown
5. **Examples:** One or more usage examples
6. **Notes:** Additional information, edge cases, or important considerations
7. **See Also:** Related functions or documentation references

---

## JavaScript/TypeScript Functions

### JSDoc Format

```javascript
/**
 * Calculates the sum of two numbers.
 *
 * @param {number} a - The first number
 * @param {number} b - The second number
 * @returns {number} The sum of a and b
 * @throws {TypeError} If either parameter is not a number
 *
 * @example
 * const result = add(5, 3);
 * console.log(result); // 8
 *
 * @example
 * // Handles decimals
 * const result = add(2.5, 3.7);
 * console.log(result); // 6.2
 */
function add(a, b) {
  if (typeof a !== 'number' || typeof b !== 'number') {
    throw new TypeError('Both parameters must be numbers');
  }
  return a + b;
}
```

### TypeScript with Type Annotations

```typescript
/**
 * Fetches user data from the API.
 *
 * @param userId - The unique identifier of the user
 * @param options - Optional configuration for the request
 * @returns A promise that resolves to the user object
 * @throws {Error} If the user is not found or the request fails
 *
 * @example
 * ```typescript
 * const user = await fetchUser('user-123');
 * console.log(user.name);
 * ```
 *
 * @example
 * ```typescript
 * // With options
 * const user = await fetchUser('user-123', {
 *   includeProfile: true,
 *   fields: ['name', 'email']
 * });
 * ```
 */
async function fetchUser(
  userId: string,
  options?: {
    includeProfile?: boolean;
    fields?: string[];
  }
): Promise<User> {
  const url = buildUserUrl(userId, options);
  const response = await fetch(url);
  
  if (!response.ok) {
    throw new Error(`Failed to fetch user: ${response.statusText}`);
  }
  
  return response.json();
}

interface User {
  id: string;
  name: string;
  email: string;
  profile?: UserProfile;
}

interface UserProfile {
  avatar: string;
  bio: string;
  location: string;
}
```

### Complex Function Example

```typescript
/**
 * Validates and transforms form data according to a schema.
 *
 * This function takes raw form data and a validation schema, validates each field,
 * transforms the data according to the schema rules, and returns either the
 * validated data or a list of validation errors.
 *
 * @template T - The type of the validated data object
 *
 * @param data - Raw form data as key-value pairs
 * @param schema - Validation schema defining rules for each field
 * @param options - Optional configuration for validation behavior
 * @param options.strict - If true, reject data with fields not in schema (default: false)
 * @param options.stripUnknown - If true, remove fields not in schema (default: true)
 * @param options.abortEarly - If true, stop validation on first error (default: false)
 *
 * @returns A result object containing either validated data or validation errors
 *
 * @throws {TypeError} If schema is invalid or data is not an object
 *
 * @example
 * ```typescript
 * const schema = {
 *   name: { type: 'string', required: true, minLength: 2 },
 *   email: { type: 'email', required: true },
 *   age: { type: 'number', min: 18, max: 120 }
 * };
 *
 * const result = validateFormData(
 *   { name: 'John', email: 'john@example.com', age: 25 },
 *   schema
 * );
 *
 * if (result.isValid) {
 *   console.log('Valid data:', result.data);
 * } else {
 *   console.log('Errors:', result.errors);
 * }
 * ```
 *
 * @example
 * ```typescript
 * // With strict mode
 * const result = validateFormData(
 *   { name: 'John', email: 'john@example.com', extra: 'field' },
 *   schema,
 *   { strict: true }
 * );
 * // Will fail because 'extra' is not in schema
 * ```
 *
 * @see {@link ValidationSchema} for schema format details
 * @see {@link ValidationResult} for return value structure
 *
 * @since 1.0.0
 * @public
 */
function validateFormData<T extends Record<string, any>>(
  data: Record<string, any>,
  schema: ValidationSchema,
  options: ValidationOptions = {}
): ValidationResult<T> {
  const {
    strict = false,
    stripUnknown = true,
    abortEarly = false
  } = options;

  if (!isObject(data)) {
    throw new TypeError('Data must be an object');
  }

  if (!isValidSchema(schema)) {
    throw new TypeError('Invalid validation schema');
  }

  const errors: ValidationError[] = [];
  const validatedData: Partial<T> = {};

  // Validation logic here...
  for (const [key, rules] of Object.entries(schema)) {
    // Validate each field
    const value = data[key];
    const fieldErrors = validateField(key, value, rules);
    
    if (fieldErrors.length > 0) {
      errors.push(...fieldErrors);
      if (abortEarly) break;
    } else {
      validatedData[key as keyof T] = transformValue(value, rules);
    }
  }

  // Check for unknown fields
  if (strict) {
    for (const key of Object.keys(data)) {
      if (!(key in schema)) {
        errors.push({
          field: key,
          message: `Unknown field: ${key}`
        });
      }
    }
  } else if (stripUnknown) {
    // Strip unknown fields (default behavior)
  } else {
    // Keep unknown fields
    for (const [key, value] of Object.entries(data)) {
      if (!(key in schema)) {
        validatedData[key as keyof T] = value;
      }
    }
  }

  return {
    isValid: errors.length === 0,
    data: errors.length === 0 ? (validatedData as T) : undefined,
    errors: errors.length > 0 ? errors : undefined
  };
}

interface ValidationSchema {
  [field: string]: FieldValidationRules;
}

interface FieldValidationRules {
  type: 'string' | 'number' | 'email' | 'boolean' | 'date';
  required?: boolean;
  minLength?: number;
  maxLength?: number;
  min?: number;
  max?: number;
  pattern?: RegExp;
  custom?: (value: any) => boolean | string;
}

interface ValidationOptions {
  strict?: boolean;
  stripUnknown?: boolean;
  abortEarly?: boolean;
}

interface ValidationResult<T> {
  isValid: boolean;
  data?: T;
  errors?: ValidationError[];
}

interface ValidationError {
  field: string;
  message: string;
}
```

### Array/Collection Functions

```typescript
/**
 * Groups array elements by a specified key.
 *
 * Takes an array of objects and groups them into a Map where the keys are
 * determined by the keySelector function and the values are arrays of objects
 * that share the same key.
 *
 * @template T - The type of elements in the array
 * @template K - The type of the grouping key
 *
 * @param array - The array to group
 * @param keySelector - Function that extracts the grouping key from each element
 *
 * @returns A Map where keys are group identifiers and values are arrays of grouped elements
 *
 * @example
 * ```typescript
 * interface Person {
 *   name: string;
 *   age: number;
 *   city: string;
 * }
 *
 * const people: Person[] = [
 *   { name: 'Alice', age: 25, city: 'NYC' },
 *   { name: 'Bob', age: 30, city: 'LA' },
 *   { name: 'Charlie', age: 25, city: 'NYC' }
 * ];
 *
 * // Group by age
 * const byAge = groupBy(people, person => person.age);
 * console.log(byAge.get(25)); // [Alice, Charlie]
 *
 * // Group by city
 * const byCity = groupBy(people, person => person.city);
 * console.log(byCity.get('NYC')); // [Alice, Charlie]
 * ```
 *
 * @see {@link Map} for information about the return type
 */
function groupBy<T, K>(
  array: T[],
  keySelector: (item: T) => K
): Map<K, T[]> {
  const groups = new Map<K, T[]>();

  for (const item of array) {
    const key = keySelector(item);
    const group = groups.get(key) || [];
    group.push(item);
    groups.set(key, group);
  }

  return groups;
}
```

---

## Python Functions

### Docstring Format (Google Style)

```python
def calculate_statistics(numbers, percentiles=None):
    """
    Calculates various statistical measures for a list of numbers.

    This function computes mean, median, standard deviation, and optionally
    specified percentiles for a given list of numbers.

    Args:
        numbers (List[float]): A list of numeric values to analyze.
        percentiles (Optional[List[int]]): List of percentiles to calculate
            (0-100). If None, calculates [25, 50, 75]. Defaults to None.

    Returns:
        dict: A dictionary containing the following keys:
            - mean (float): The arithmetic mean
            - median (float): The median value
            - std_dev (float): The standard deviation
            - percentiles (dict): Requested percentiles as {percentile: value}

    Raises:
        ValueError: If numbers list is empty or contains non-numeric values.
        TypeError: If numbers is not a list or percentiles contains invalid values.

    Examples:
        >>> calculate_statistics([1, 2, 3, 4, 5])
        {
            'mean': 3.0,
            'median': 3.0,
            'std_dev': 1.414,
            'percentiles': {25: 2.0, 50: 3.0, 75: 4.0}
        }

        >>> calculate_statistics([10, 20, 30], percentiles=[10, 90])
        {
            'mean': 20.0,
            'median': 20.0,
            'std_dev': 8.165,
            'percentiles': {10: 12.0, 90: 28.0}
        }

    Note:
        - The function uses Bessel's correction for standard deviation
        - Percentile calculation uses linear interpolation
        - Empty lists will raise a ValueError

    See Also:
        - numpy.percentile: For more advanced percentile calculations
        - statistics.mean: Standard library alternative

    Since:
        Version 1.0.0
    """
    import statistics
    import numpy as np

    if not numbers:
        raise ValueError("Numbers list cannot be empty")

    if not isinstance(numbers, list):
        raise TypeError("Numbers must be a list")

    if not all(isinstance(n, (int, float)) for n in numbers):
        raise ValueError("All elements must be numeric")

    if percentiles is None:
        percentiles = [25, 50, 75]

    result = {
        'mean': statistics.mean(numbers),
        'median': statistics.median(numbers),
        'std_dev': statistics.stdev(numbers) if len(numbers) > 1 else 0.0,
        'percentiles': {}
    }

    for p in percentiles:
        if not 0 <= p <= 100:
            raise ValueError(f"Percentile must be between 0 and 100, got {p}")
        result['percentiles'][p] = np.percentile(numbers, p)

    return result
```

### NumPy-Style Docstring

```python
def process_dataframe(df, operations, inplace=False):
    """
    Apply a series of operations to a pandas DataFrame.

    Parameters
    ----------
    df : pandas.DataFrame
        The DataFrame to process.
    operations : list of dict
        List of operations to apply. Each operation is a dict with keys:
        - 'type': str, operation type ('filter', 'transform', 'aggregate')
        - 'column': str, column name to operate on
        - 'function': callable, function to apply
        - 'params': dict, optional parameters for the function
    inplace : bool, optional
        If True, modify the DataFrame in place. Default is False.

    Returns
    -------
    pandas.DataFrame
        The processed DataFrame. If inplace=True, returns the modified
        original DataFrame.

    Raises
    ------
    ValueError
        If operations list is empty or contains invalid operations.
    KeyError
        If specified column doesn't exist in the DataFrame.
    TypeError
        If df is not a pandas DataFrame.

    See Also
    --------
    pandas.DataFrame.apply : Apply a function along an axis
    pandas.DataFrame.transform : Apply function to groups
    pandas.DataFrame.filter : Subset rows or columns

    Notes
    -----
    Operations are applied in the order they appear in the list.
    Filter operations remove rows that don't meet the criteria.
    Transform operations modify column values in place.
    Aggregate operations reduce the DataFrame to summary statistics.

    Examples
    --------
    >>> import pandas as pd
    >>> df = pd.DataFrame({
    ...     'name': ['Alice', 'Bob', 'Charlie'],
    ...     'age': [25, 30, 35],
    ...     'salary': [50000, 60000, 70000]
    ... })

    Apply a filter operation:

    >>> operations = [
    ...     {
    ...         'type': 'filter',
    ...         'column': 'age',
    ...         'function': lambda x: x > 25
    ...     }
    ... ]
    >>> result = process_dataframe(df, operations)
    >>> print(result)
          name  age  salary
    1      Bob   30   60000
    2  Charlie   35   70000

    Apply multiple operations:

    >>> operations = [
    ...     {
    ...         'type': 'transform',
    ...         'column': 'salary',
    ...         'function': lambda x: x * 1.1  # 10% raise
    ...     },
    ...     {
    ...         'type': 'filter',
    ...         'column': 'age',
    ...         'function': lambda x: x >= 30
    ...     }
    ... ]
    >>> result = process_dataframe(df, operations)

    References
    ----------
    .. [1] pandas documentation: https://pandas.pydata.org/docs/
    """
    import pandas as pd

    if not isinstance(df, pd.DataFrame):
        raise TypeError("df must be a pandas DataFrame")

    if not operations:
        raise ValueError("Operations list cannot be empty")

    # Create a copy if not inplace
    result_df = df if inplace else df.copy()

    for op in operations:
        if 'type' not in op or 'column' not in op or 'function' not in op:
            raise ValueError("Each operation must have 'type', 'column', and 'function'")

        col = op['column']
        func = op['function']
        params = op.get('params', {})

        if col not in result_df.columns:
            raise KeyError(f"Column '{col}' not found in DataFrame")

        if op['type'] == 'filter':
            result_df = result_df[result_df[col].apply(func, **params)]
        elif op['type'] == 'transform':
            result_df[col] = result_df[col].apply(func, **params)
        elif op['type'] == 'aggregate':
            result_df = result_df.groupby(col).agg(func, **params)
        else:
            raise ValueError(f"Unknown operation type: {op['type']}")

    return result_df
```

### Async Function Example (Python)

```python
async def fetch_multiple_users(user_ids, max_concurrent=5):
    """
    Fetch multiple users concurrently from the API.

    Retrieves user data for multiple user IDs in parallel, with a limit
    on the number of concurrent requests to avoid overwhelming the API.

    Args:
        user_ids (List[str]): List of user IDs to fetch.
        max_concurrent (int, optional): Maximum number of concurrent
            requests. Defaults to 5.

    Returns:
        List[dict]: List of user objects in the same order as user_ids.
            If a user fetch fails, the corresponding entry will be None.

    Raises:
        ValueError: If user_ids is empty or max_concurrent < 1.
        aiohttp.ClientError: If there's a network error.

    Example:
        >>> import asyncio
        >>> user_ids = ['user-1', 'user-2', 'user-3']
        >>> users = asyncio.run(fetch_multiple_users(user_ids))
        >>> for user in users:
        ...     if user:
        ...         print(f"{user['name']}: {user['email']}")

        Fetch with custom concurrency:
        >>> users = asyncio.run(
        ...     fetch_multiple_users(user_ids, max_concurrent=10)
        ... )

    Note:
        - Uses aiohttp for async HTTP requests
        - Implements semaphore to limit concurrent requests
        - Failed requests return None instead of raising exceptions
        - Preserves order of input user_ids in output

    See Also:
        - fetch_user: Synchronous version for single user
        - aiohttp.ClientSession: For session management
    """
    import asyncio
    import aiohttp
    from typing import List, Optional

    if not user_ids:
        raise ValueError("user_ids cannot be empty")

    if max_concurrent < 1:
        raise ValueError("max_concurrent must be at least 1")

    async def fetch_single_user(
        session: aiohttp.ClientSession,
        user_id: str,
        semaphore: asyncio.Semaphore
    ) -> Optional[dict]:
        """Fetch a single user with semaphore control."""
        async with semaphore:
            try:
                url = f"https://api.example.com/api/v1/users/{user_id}"
                async with session.get(url) as response:
                    if response.status == 200:
                        return await response.json()
                    else:
                        print(f"Failed to fetch user {user_id}: {response.status}")
                        return None
            except Exception as e:
                print(f"Error fetching user {user_id}: {e}")
                return None

    # Create semaphore to limit concurrent requests
    semaphore = asyncio.Semaphore(max_concurrent)

    # Create session and fetch all users
    async with aiohttp.ClientSession() as session:
        tasks = [
            fetch_single_user(session, user_id, semaphore)
            for user_id in user_ids
        ]
        users = await asyncio.gather(*tasks)

    return users
```

---

## Java Functions

### JavaDoc Format

```java
/**
 * Validates and formats a phone number according to E.164 standard.
 *
 * <p>This method takes a phone number string in various formats and converts it
 * to the international E.164 format (+[country code][number]). It handles common
 * formatting variations including spaces, dashes, parentheses, and different
 * country code representations.
 *
 * <p>Supported input formats:
 * <ul>
 *   <li>+1-555-123-4567</li>
 *   <li>(555) 123-4567</li>
 *   <li>555.123.4567</li>
 *   <li>5551234567</li>
 * </ul>
 *
 * @param phoneNumber the phone number string to validate and format
 * @param countryCode the ISO 3166-1 alpha-2 country code (e.g., "US", "GB")
 * @return the formatted phone number in E.164 format
 *
 * @throws IllegalArgumentException if the phone number is invalid or empty
 * @throws NullPointerException if phoneNumber or countryCode is null
 *
 * @since 1.0
 * @see <a href="https://www.itu.int/rec/T-REC-E.164/">ITU-T E.164 Standard</a>
 *
 * @example
 * <pre>{@code
 * String formatted = formatPhoneNumber("(555) 123-4567", "US");
 * System.out.println(formatted); // +15551234567
 *
 * String intl = formatPhoneNumber("+44 20 7123 4567", "GB");
 * System.out.println(intl); // +442071234567
 * }</pre>
 */
public String formatPhoneNumber(String phoneNumber, String countryCode) 
        throws IllegalArgumentException {
    
    if (phoneNumber == null || countryCode == null) {
        throw new NullPointerException("Phone number and country code cannot be null");
    }

    if (phoneNumber.trim().isEmpty()) {
        throw new IllegalArgumentException("Phone number cannot be empty");
    }

    // Remove all non-digit characters except leading +
    String cleaned = phoneNumber.replaceAll("[^0-9+]", "");

    // Validate and format based on country code
    PhoneNumberUtil phoneUtil = PhoneNumberUtil.getInstance();
    try {
        Phonenumber.PhoneNumber number = phoneUtil.parse(cleaned, countryCode);
        if (!phoneUtil.isValidNumber(number)) {
            throw new IllegalArgumentException("Invalid phone number: " + phoneNumber);
        }
        return phoneUtil.format(number, PhoneNumberUtil.PhoneNumberFormat.E164);
    } catch (NumberParseException e) {
        throw new IllegalArgumentException("Failed to parse phone number: " + phoneNumber, e);
    }
}
```

### Complex Java Method

```java
/**
 * Processes a batch of orders with transaction management and error handling.
 *
 * <p>This method processes multiple orders in a single database transaction,
 * applying business rules, updating inventory, and generating notifications.
 * If any order fails validation or processing, the entire batch is rolled back.
 *
 * <p><strong>Processing Steps:</strong>
 * <ol>
 *   <li>Validate all orders in the batch</li>
 *   <li>Check inventory availability</li>
 *   <li>Calculate totals and apply discounts</li>
 *   <li>Update order status and inventory</li>
 *   <li>Generate confirmation notifications</li>
 * </ol>
 *
 * @param orders the list of orders to process; must not be null or empty
 * @param options processing options including validation level and notification preferences
 * @return a {@link BatchProcessingResult} containing successfully processed orders and any errors
 *
 * @throws IllegalArgumentException if orders list is null or empty
 * @throws InsufficientInventoryException if any product is out of stock
 * @throws DatabaseException if there's a database transaction error
 *
 * @implNote This method uses pessimistic locking to prevent race conditions
 *           when updating inventory. The transaction isolation level is
 *           set to SERIALIZABLE.
 *
 * @implSpec Implementations must ensure that either all orders are processed
 *           successfully or none are (atomicity guarantee).
 *
 * @since 2.0
 * @version 2.1 - Added support for batch notifications
 * @author John Doe
 *
 * @see Order
 * @see ProcessingOptions
 * @see BatchProcessingResult
 *
 * @example
 * <pre>{@code
 * // Process a batch of orders with default options
 * List<Order> orders = Arrays.asList(order1, order2, order3);
 * ProcessingOptions options = ProcessingOptions.builder()
 *     .validationLevel(ValidationLevel.STRICT)
 *     .sendNotifications(true)
 *     .build();
 *
 * try {
 *     BatchProcessingResult result = orderService.processBatch(orders, options);
 *     System.out.printf("Processed %d orders successfully%n",
 *                      result.getSuccessfulOrders().size());
 *
 *     if (result.hasErrors()) {
 *         result.getErrors().forEach(error ->
 *             System.err.printf("Order %s failed: %s%n",
 *                             error.getOrderId(),
 *                             error.getMessage())
 *         );
 *     }
 * } catch (InsufficientInventoryException e) {
 *     System.err.println("Not enough inventory: " + e.getMessage());
 * }
 * }</pre>
 */
@Transactional(isolation = Isolation.SERIALIZABLE, rollbackFor = Exception.class)
public BatchProcessingResult processBatch(
        @NonNull List<Order> orders,
        @NonNull ProcessingOptions options)
        throws InsufficientInventoryException, DatabaseException {

    // Validate input
    if (orders == null || orders.isEmpty()) {
        throw new IllegalArgumentException("Orders list cannot be null or empty");
    }

    log.info("Processing batch of {} orders", orders.size());

    List<Order> successfulOrders = new ArrayList<>();
    List<ProcessingError> errors = new ArrayList<>();

    try {
        // Step 1: Validate all orders
        List<ValidationError> validationErrors = validateOrders(orders, options.getValidationLevel());
        if (!validationErrors.isEmpty()) {
            throw new BatchValidationException("Batch validation failed", validationErrors);
        }

        // Step 2: Check inventory for all orders
        Map<String, Integer> requiredInventory = calculateRequiredInventory(orders);
        checkInventoryAvailability(requiredInventory);

        // Step 3: Process each order
        for (Order order : orders) {
            try {
                processOrder(order, options);
                successfulOrders.add(order);
            } catch (OrderProcessingException e) {
                errors.add(new ProcessingError(order.getId(), e.getMessage()));
                log.error("Failed to process order {}: {}", order.getId(), e.getMessage());
            }
        }

        // Step 4: Update inventory
        updateInventory(requiredInventory);

        // Step 5: Send notifications
        if (options.isSendNotifications() && !successfulOrders.isEmpty()) {
            notificationService.sendBatchConfirmations(successfulOrders);
        }

        log.info("Batch processing complete: {} successful, {} errors",
                successfulOrders.size(), errors.size());

        return BatchProcessingResult.builder()
                .successfulOrders(successfulOrders)
                .errors(errors)
                .processedAt(Instant.now())
                .build();

    } catch (Exception e) {
        log.error("Batch processing failed: {}", e.getMessage());
        throw new DatabaseException("Failed to process batch", e);
    }
}
```

---

## Go Functions

### Go Documentation Format

```go
// CalculateDiscount calculates the discount amount for a purchase.
//
// This function applies various discount rules based on the customer type,
// purchase amount, and any active promotions. It returns the discount amount
// in the same currency as the purchase amount.
//
// Parameters:
//   - amount: The original purchase amount (must be >= 0)
//   - customerType: The type of customer ("regular", "premium", "vip")
//   - promoCode: Optional promotion code (empty string if none)
//
// Returns:
//   - float64: The discount amount to be subtracted from the purchase
//   - error: An error if the calculation fails or parameters are invalid
//
// Errors:
//   - ErrInvalidAmount: If amount is negative
//   - ErrInvalidCustomerType: If customerType is not recognized
//   - ErrInvalidPromoCode: If promoCode is provided but not found
//
// Examples:
//
//	// Regular customer, no promo code
//	discount, err := CalculateDiscount(100.00, "regular", "")
//	if err != nil {
//	    log.Fatal(err)
//	}
//	fmt.Printf("Discount: $%.2f\n", discount) // Discount: $0.00
//
//	// Premium customer with promo code
//	discount, err := CalculateDiscount(100.00, "premium", "SAVE10")
//	if err != nil {
//	    log.Fatal(err)
//	}
//	fmt.Printf("Discount: $%.2f\n", discount) // Discount: $15.00
//
// Notes:
//   - Discounts are cumulative: customer type discount + promo code discount
//   - Maximum discount is 50% of the original amount
//   - VIP customers receive free shipping (handled separately)
//
// See also: ApplyDiscount, ValidatePromoCode
func CalculateDiscount(amount float64, customerType string, promoCode string) (float64, error) {
    // Validate amount
    if amount < 0 {
        return 0, ErrInvalidAmount
    }

    // Validate customer type
    validTypes := map[string]float64{
        "regular": 0.0,
        "premium": 0.10,
        "vip":     0.15,
    }

    baseDiscount, exists := validTypes[customerType]
    if !exists {
        return 0, fmt.Errorf("%w: %s", ErrInvalidCustomerType, customerType)
    }

    // Calculate base discount
    discount := amount * baseDiscount

    // Apply promo code if provided
    if promoCode != "" {
        promoDiscount, err := getPromoDiscount(promoCode, amount)
        if err != nil {
            return 0, fmt.Errorf("%w: %s", ErrInvalidPromoCode, promoCode)
        }
        discount += promoDiscount
    }

    // Cap discount at 50%
    maxDiscount := amount * 0.50
    if discount > maxDiscount {
        discount = maxDiscount
    }

    return discount, nil
}
```

### Go Function with Context

```go
// FetchUserWithContext retrieves user data from the API with context support.
//
// This function makes an HTTP request to fetch user information, supporting
// context cancellation and timeout. It's designed for use in concurrent
// operations where you may need to cancel the request early.
//
// Parameters:
//   - ctx: Context for cancellation and timeout control
//   - userID: The unique identifier of the user to fetch
//   - options: Optional request configuration (can be nil for defaults)
//
// Returns:
//   - *User: The user object with populated fields
//   - error: An error if the request fails or is cancelled
//
// Errors:
//   - context.DeadlineExceeded: If the request times out
//   - context.Canceled: If the context is cancelled
//   - ErrUserNotFound: If the user doesn't exist (HTTP 404)
//   - ErrUnauthorized: If authentication fails (HTTP 401)
//   - ErrServerError: For HTTP 5xx responses
//
// Examples:
//
//	// Basic usage with timeout
//	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
//	defer cancel()
//
//	user, err := FetchUserWithContext(ctx, "user-123", nil)
//	if err != nil {
//	    if errors.Is(err, context.DeadlineExceeded) {
//	        log.Println("Request timed out")
//	    } else {
//	        log.Printf("Error: %v\n", err)
//	    }
//	    return
//	}
//	fmt.Printf("User: %s (%s)\n", user.Name, user.Email)
//
//	// With custom options
//	opts := &FetchOptions{
//	    IncludeProfile: true,
//	    Fields:         []string{"name", "email", "profile"},
//	}
//	user, err := FetchUserWithContext(ctx, "user-123", opts)
//
//	// With cancellation
//	ctx, cancel := context.WithCancel(context.Background())
//	go func() {
//	    // Cancel after some condition
//	    time.Sleep(2 * time.Second)
//	    cancel()
//	}()
//	user, err := FetchUserWithContext(ctx, "user-123", nil)
//
// Thread Safety:
// This function is safe for concurrent use. Multiple goroutines can call
// this function simultaneously without external synchronization.
//
// Performance Notes:
//   - Uses connection pooling from http.DefaultClient
//   - Reuses JSON decoder for better performance
//   - Average response time: ~100ms (depends on network)
//
// See also: FetchUsers, FetchUserProfile, User
func FetchUserWithContext(ctx context.Context, userID string, options *FetchOptions) (*User, error) {
    // Validate input
    if ctx == nil {
        return nil, errors.New("context cannot be nil")
    }

    if userID == "" {
        return nil, errors.New("userID cannot be empty")
    }

    // Build URL with options
    url := buildUserURL(userID, options)

    // Create request with context
    req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
    if err != nil {
        return nil, fmt.Errorf("failed to create request: %w", err)
    }

    // Set headers
    req.Header.Set("Accept", "application/json")
    if options != nil && options.APIKey != "" {
        req.Header.Set("Authorization", "Bearer "+options.APIKey)
    }

    // Make request
    resp, err := http.DefaultClient.Do(req)
    if err != nil {
        // Check if context was cancelled
        if ctx.Err() != nil {
            return nil, ctx.Err()
        }
        return nil, fmt.Errorf("request failed: %w", err)
    }
    defer resp.Body.Close()

    // Handle status codes
    switch resp.StatusCode {
    case http.StatusOK:
        // Success - decode response
        var user User
        if err := json.NewDecoder(resp.Body).Decode(&user); err != nil {
            return nil, fmt.Errorf("failed to decode response: %w", err)
        }
        return &user, nil

    case http.StatusNotFound:
        return nil, fmt.Errorf("%w: %s", ErrUserNotFound, userID)

    case http.StatusUnauthorized:
        return nil, ErrUnauthorized

    case http.StatusInternalServerError, http.StatusBadGateway, http.StatusServiceUnavailable:
        return nil, fmt.Errorf("%w: status code %d", ErrServerError, resp.StatusCode)

    default:
        return nil, fmt.Errorf("unexpected status code: %d", resp.StatusCode)
    }
}

// User represents a user in the system.
type User struct {
    ID        string    `json:"id"`
    Name      string    `json:"name"`
    Email     string    `json:"email"`
    Profile   *Profile  `json:"profile,omitempty"`
    CreatedAt time.Time `json:"createdAt"`
}

// Profile contains extended user information.
type Profile struct {
    Avatar   string `json:"avatar"`
    Bio      string `json:"bio"`
    Location string `json:"location"`
}

// FetchOptions configures the user fetch request.
type FetchOptions struct {
    IncludeProfile bool     // Include full profile information
    Fields         []string // Specific fields to return
    APIKey         string   // API key for authentication
}
```

---

## Async Functions

### JavaScript/TypeScript Async Examples

```typescript
/**
 * Retries an async operation with exponential backoff.
 *
 * This function attempts to execute an async operation multiple times with
 * increasing delays between attempts. Useful for handling transient failures
 * in network requests or external service calls.
 *
 * @template T - The return type of the async operation
 *
 * @param operation - The async function to retry
 * @param options - Retry configuration options
 * @param options.maxRetries - Maximum number of retry attempts (default: 3)
 * @param options.initialDelay - Initial delay in milliseconds (default: 1000)
 * @param options.maxDelay - Maximum delay in milliseconds (default: 30000)
 * @param options.backoffMultiplier - Multiplier for exponential backoff (default: 2)
 * @param options.onRetry - Optional callback called before each retry
 *
 * @returns A promise that resolves with the operation result
 * @throws The last error if all retry attempts fail
 *
 * @example
 * ```typescript
 * // Retry a fetch request
 * const data = await retryWithBackoff(
 *   () => fetch('https://api.example.com/data').then(r => r.json()),
 *   { maxRetries: 5, initialDelay: 500 }
 * );
 * ```
 *
 * @example
 * ```typescript
 * // With retry callback
 * const result = await retryWithBackoff(
 *   async () => {
 *     const response = await fetch('https://api.example.com/users');
 *     if (!response.ok) throw new Error('Failed');
 *     return response.json();
 *   },
 *   {
 *     maxRetries: 3,
 *     onRetry: (attempt, error) => {
 *       console.log(`Retry attempt ${attempt}: ${error.message}`);
 *     }
 *   }
 * );
 * ```
 */
async function retryWithBackoff<T>(
  operation: () => Promise<T>,
  options: {
    maxRetries?: number;
    initialDelay?: number;
    maxDelay?: number;
    backoffMultiplier?: number;
    onRetry?: (attempt: number, error: Error) => void;
  } = {}
): Promise<T> {
  const {
    maxRetries = 3,
    initialDelay = 1000,
    maxDelay = 30000,
    backoffMultiplier = 2,
    onRetry
  } = options;

  let lastError: Error;
  let delay = initialDelay;

  for (let attempt = 0; attempt <= maxRetries; attempt++) {
    try {
      return await operation();
    } catch (error) {
      lastError = error as Error;

      if (attempt === maxRetries) {
        throw lastError;
      }

      if (onRetry) {
        onRetry(attempt + 1, lastError);
      }

      // Wait before retrying
      await new Promise(resolve => setTimeout(resolve, delay));

      // Calculate next delay with exponential backoff
      delay = Math.min(delay * backoffMultiplier, maxDelay);
    }
  }

  throw lastError!;
}
```

---

## Higher-Order Functions

```typescript
/**
 * Creates a memoized version of a function.
 *
 * Returns a new function that caches the results of the original function
 * based on its arguments. Subsequent calls with the same arguments return
 * the cached result instead of recomputing.
 *
 * @template TArgs - Tuple type of function arguments
 * @template TReturn - Return type of the function
 *
 * @param fn - The function to memoize
 * @param options - Memoization options
 * @param options.maxCacheSize - Maximum number of cached results (default: unlimited)
 * @param options.ttl - Time to live for cache entries in milliseconds (default: unlimited)
 * @param options.keyGenerator - Custom function to generate cache keys
 *
 * @returns A memoized version of the original function
 *
 * @example
 * ```typescript
 * // Memoize an expensive calculation
 * const fibonacci = memoize((n: number): number => {
 *   if (n <= 1) return n;
 *   return fibonacci(n - 1) + fibonacci(n - 2);
 * });
 *
 * console.log(fibonacci(40)); // Computed
 * console.log(fibonacci(40)); // Cached
 * ```
 *
 * @example
 * ```typescript
 * // With options
 * const fetchUser = memoize(
 *   async (id: string) => {
 *     const response = await fetch(`/api/users/${id}`);
 *     return response.json();
 *   },
 *   {
 *     maxCacheSize: 100,
 *     ttl: 60000, // 1 minute
 *   }
 * );
 * ```
 *
 * @see {@link debounce} for rate limiting function calls
 * @see {@link throttle} for controlling function execution frequency
 */
function memoize<TArgs extends any[], TReturn>(
  fn: (...args: TArgs) => TReturn,
  options: {
    maxCacheSize?: number;
    ttl?: number;
    keyGenerator?: (...args: TArgs) => string;
  } = {}
): (...args: TArgs) => TReturn {
  const {
    maxCacheSize = Infinity,
    ttl,
    keyGenerator = (...args) => JSON.stringify(args)
  } = options;

  const cache = new Map<string, { value: TReturn; timestamp: number }>();

  return function memoized(...args: TArgs): TReturn {
    const key = keyGenerator(...args);

    // Check if we have a cached value
    if (cache.has(key)) {
      const cached = cache.get(key)!;

      // Check TTL if specified
      if (ttl && Date.now() - cached.timestamp > ttl) {
        cache.delete(key);
      } else {
        return cached.value;
      }
    }

    // Compute the result
    const result = fn(...args);

    // Add to cache
    cache.set(key, { value: result, timestamp: Date.now() });

    // Enforce max cache size (LRU eviction)
    if (cache.size > maxCacheSize) {
      const firstKey = cache.keys().next().value;
      cache.delete(firstKey);
    }

    return result;
  };
}
```

---

## Utility Functions

```typescript
/**
 * Deeply clones an object or array.
 *
 * Creates a deep copy of the input value, handling nested objects, arrays,
 * dates, and other common JavaScript types. Circular references are detected
 * and handled appropriately.
 *
 * @template T - The type of value to clone
 *
 * @param value - The value to clone
 * @param options - Cloning options
 * @param options.includeNonEnumerable - Include non-enumerable properties (default: false)
 * @param options.includeSymbols - Include symbol properties (default: false)
 *
 * @returns A deep clone of the input value
 *
 * @throws {TypeError} If the value contains circular references and options don't handle them
 *
 * @example
 * ```typescript
 * const original = {
 *   name: 'John',
 *   address: {
 *     city: 'NYC',
 *     zip: '10001'
 *   },
 *   hobbies: ['reading', 'gaming']
 * };
 *
 * const cloned = deepClone(original);
 * cloned.address.city = 'LA';
 * console.log(original.address.city); // 'NYC' (unchanged)
 * ```
 *
 * @example
 * ```typescript
 * // Clone with dates and arrays
 * const data = {
 *   created: new Date(),
 *   tags: ['typescript', 'javascript'],
 *   metadata: { version: 1 }
 * };
 *
 * const copy = deepClone(data);
 * ```
 *
 * @note This function does not clone functions or DOM nodes
 */
function deepClone<T>(
  value: T,
  options: {
    includeNonEnumerable?: boolean;
    includeSymbols?: boolean;
  } = {}
): T {
  const {
    includeNonEnumerable = false,
    includeSymbols = false
  } = options;

  // Handle primitive types and null
  if (value === null || typeof value !== 'object') {
    return value;
  }

  // Handle Date
  if (value instanceof Date) {
    return new Date(value.getTime()) as any;
  }

  // Handle Array
  if (Array.isArray(value)) {
    return value.map(item => deepClone(item, options)) as any;
  }

  // Handle RegExp
  if (value instanceof RegExp) {
    return new RegExp(value.source, value.flags) as any;
  }

  // Handle Map
  if (value instanceof Map) {
    const cloned = new Map();
    value.forEach((val, key) => {
      cloned.set(key, deepClone(val, options));
    });
    return cloned as any;
  }

  // Handle Set
  if (value instanceof Set) {
    const cloned = new Set();
    value.forEach(val => {
      cloned.add(deepClone(val, options));
    });
    return cloned as any;
  }

  // Handle Object
  const cloned: any = Object.create(Object.getPrototypeOf(value));

  // Get property descriptors
  const descriptors = includeNonEnumerable
    ? Object.getOwnPropertyDescriptors(value)
    : Object.getOwnPropertyDescriptors(value);

  // Clone enumerable properties
  for (const key of Object.keys(value)) {
    cloned[key] = deepClone((value as any)[key], options);
  }

  // Clone symbol properties if requested
  if (includeSymbols) {
    const symbols = Object.getOwnPropertySymbols(value);
    for (const symbol of symbols) {
      cloned[symbol] = deepClone((value as any)[symbol], options);
    }
  }

  return cloned;
}
```

---

## Best Practices

### 1. Always Document Public Functions
Every public function should have comprehensive documentation including description, parameters, return values, and examples.

### 2. Provide Clear Examples
Include at least one realistic example showing how to use the function. Multiple examples for different use cases are even better.

### 3. Document Error Conditions
Clearly specify what errors or exceptions the function might throw and under what conditions.

### 4. Include Type Information
Always specify the types of parameters and return values, either through static typing (TypeScript, Java) or documentation (JSDoc, Python docstrings).

### 5. Explain Complex Logic
If a function implements complex algorithms or business logic, provide explanations or links to relevant documentation.

### 6. Keep Documentation Updated
Update documentation whenever you change function behavior, parameters, or return values.

### 7. Use Consistent Format
Follow the standard documentation format for your language (JSDoc for JavaScript, JavaDoc for Java, etc.).

### 8. Document Side Effects
Clearly note if a function has side effects like modifying global state, making network requests, or changing input parameters.

### 9. Specify Preconditions and Postconditions
Document any assumptions or requirements about the function's input and guarantees about its output.

### 10. Link Related Functions
Use "See Also" sections to reference related functions, classes, or documentation.

---

## Tools and Automation

### Generating Documentation

- **JavaScript/TypeScript:** JSDoc, TypeDoc, documentation.js
- **Python:** Sphinx, pdoc, mkdocstrings
- **Java:** JavaDoc (built-in)
- **Go:** godoc (built-in)

### Linting Documentation

- **markdownlint:** For documentation files
- **vale:** Prose linting
- **documentation-lint:** Language-specific doc linters

### Continuous Integration

Add documentation checks to your CI/CD pipeline:

```yaml
# Example: GitHub Actions
- name: Generate Documentation
  run: npm run docs:generate

- name: Check Documentation
  run: npm run docs:lint

- name: Deploy Documentation
  run: npm run docs:deploy
```

---

## Conclusion

Comprehensive function documentation is essential for maintainable codebases. Following these standards ensures that your functions are well-documented, easy to understand, and simple to use.

For more information, see:
- [API Documentation Guide](./API_DOCUMENTATION.md)
- [Component Documentation Guide](./COMPONENT_DOCUMENTATION.md)
- [Best Practices Guide](./BEST_PRACTICES.md)
