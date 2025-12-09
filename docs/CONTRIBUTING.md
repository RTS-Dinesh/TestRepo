# Contributing to Documentation

## Overview

This guide explains how to document public APIs, functions, and components in this project.

## Documentation Requirements

All public APIs, functions, and components must include:

### 1. Function/API Documentation

```javascript
/**
 * Brief description of what the function does.
 * 
 * @param {Type} paramName - Description of the parameter
 * @param {Type} [optionalParam] - Description of optional parameter
 * @returns {Type} Description of return value
 * @throws {ErrorType} When this error occurs
 * 
 * @example
 * // Example usage
 * const result = myFunction('value1', 'value2');
 * console.log(result); // Expected output
 */
function myFunction(paramName, optionalParam) {
  // Implementation
}
```

### 2. Component Documentation

```javascript
/**
 * ComponentName - Brief description
 * 
 * @component
 * @param {Object} props - Component props
 * @param {string} props.title - The title to display
 * @param {Function} props.onClick - Click handler function
 * @param {boolean} [props.disabled=false] - Whether component is disabled
 * 
 * @example
 * <ComponentName 
 *   title="Hello World" 
 *   onClick={() => console.log('clicked')}
 *   disabled={false}
 * />
 */
function ComponentName({ title, onClick, disabled = false }) {
  // Implementation
}
```

### 3. API Endpoint Documentation

```javascript
/**
 * GET /api/resource/:id
 * 
 * Retrieves a specific resource by ID.
 * 
 * @route GET /api/resource/:id
 * @param {string} id - Resource identifier
 * @returns {Object} Resource object
 * @throws {404} Resource not found
 * 
 * @example
 * GET /api/resource/123
 * Response: { id: 123, name: "Resource Name" }
 */
```

## Documentation Checklist

When adding or updating code, ensure:

- [ ] All public functions have JSDoc comments
- [ ] All public components have prop documentation
- [ ] All API endpoints are documented
- [ ] Examples are provided for complex functions
- [ ] Error cases are documented
- [ ] Type information is included
- [ ] Usage instructions are clear

## Generating Documentation

### For JavaScript/TypeScript Projects

If using JSDoc:
```bash
npm install -g jsdoc
jsdoc -c jsdoc.json
```

### For Python Projects

If using Sphinx:
```bash
pip install sphinx
sphinx-quickstart
```

### For Other Languages

- **Go**: Use `godoc` (built-in)
- **Rust**: Use `cargo doc`
- **Java**: Use Javadoc
- **C/C++**: Use Doxygen

## Documentation Structure

The documentation should be organized as:

```
docs/
├── README.md              # Documentation guide
├── CONTRIBUTING.md        # This file
├── api/                   # API reference
│   ├── endpoints.md
│   └── authentication.md
├── components/            # Component documentation
│   └── [component-name].md
└── examples/              # Code examples
    └── [example-name].md
```

## Examples

### Good Documentation Example

```javascript
/**
 * Calculates the total price including tax.
 * 
 * @param {number} basePrice - The base price before tax
 * @param {number} [taxRate=0.1] - The tax rate (default: 10%)
 * @returns {number} The total price including tax
 * @throws {TypeError} If basePrice is not a number
 * @throws {RangeError} If basePrice is negative
 * 
 * @example
 * // Basic usage
 * const total = calculateTotal(100);
 * console.log(total); // 110
 * 
 * @example
 * // With custom tax rate
 * const total = calculateTotal(100, 0.15);
 * console.log(total); // 115
 */
function calculateTotal(basePrice, taxRate = 0.1) {
  if (typeof basePrice !== 'number') {
    throw new TypeError('basePrice must be a number');
  }
  if (basePrice < 0) {
    throw new RangeError('basePrice cannot be negative');
  }
  return basePrice * (1 + taxRate);
}
```

### Poor Documentation Example

```javascript
// Calculates total
function calculateTotal(basePrice, taxRate) {
  return basePrice * (1 + taxRate);
}
```

## Maintaining Documentation

- Update documentation when code changes
- Review documentation during code reviews
- Keep examples up to date
- Remove outdated information
- Add migration guides for breaking changes
