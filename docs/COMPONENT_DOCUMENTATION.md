# Component Documentation Guide

## Table of Contents

1. [Overview](#overview)
2. [React Components](#react-components)
3. [Vue Components](#vue-components)
4. [Angular Components](#angular-components)
5. [Web Components](#web-components)
6. [UI Component Libraries](#ui-component-libraries)
7. [State Management](#state-management)
8. [Best Practices](#best-practices)

## Overview

This guide provides comprehensive standards for documenting UI components, including props/inputs, events, slots, styling, and usage examples. Well-documented components make it easier for developers to understand, use, and maintain your component library.

---

## React Components

### Functional Component with TypeScript

```typescript
import React, { useState, useEffect, ReactNode } from 'react';

/**
 * A customizable button component with various styles and states.
 *
 * This component provides a flexible button with support for different variants,
 * sizes, loading states, and icon integration. It follows accessibility best
 * practices and includes keyboard navigation support.
 *
 * @component
 * @example
 * ```tsx
 * // Basic usage
 * <Button onClick={() => console.log('clicked')}>
 *   Click Me
 * </Button>
 *
 * // With variant and size
 * <Button variant="primary" size="large" onClick={handleSubmit}>
 *   Submit
 * </Button>
 *
 * // Loading state
 * <Button isLoading disabled>
 *   Saving...
 * </Button>
 *
 * // With icon
 * <Button leftIcon={<IconCheck />} variant="success">
 *   Confirm
 * </Button>
 * ```
 */

export interface ButtonProps {
  /**
   * The content to display inside the button.
   */
  children: ReactNode;

  /**
   * Visual style variant of the button.
   * @default 'default'
   */
  variant?: 'default' | 'primary' | 'secondary' | 'success' | 'danger' | 'ghost';

  /**
   * Size of the button.
   * @default 'medium'
   */
  size?: 'small' | 'medium' | 'large';

  /**
   * If true, the button will take up the full width of its container.
   * @default false
   */
  fullWidth?: boolean;

  /**
   * If true, displays a loading spinner and disables the button.
   * @default false
   */
  isLoading?: boolean;

  /**
   * If true, the button is disabled and cannot be clicked.
   * @default false
   */
  disabled?: boolean;

  /**
   * Icon to display on the left side of the button text.
   */
  leftIcon?: ReactNode;

  /**
   * Icon to display on the right side of the button text.
   */
  rightIcon?: ReactNode;

  /**
   * Callback fired when the button is clicked.
   */
  onClick?: (event: React.MouseEvent<HTMLButtonElement>) => void;

  /**
   * HTML button type attribute.
   * @default 'button'
   */
  type?: 'button' | 'submit' | 'reset';

  /**
   * Additional CSS class names to apply to the button.
   */
  className?: string;

  /**
   * Aria label for accessibility.
   */
  ariaLabel?: string;

  /**
   * Data attribute for testing.
   */
  dataTestId?: string;
}

export const Button: React.FC<ButtonProps> = ({
  children,
  variant = 'default',
  size = 'medium',
  fullWidth = false,
  isLoading = false,
  disabled = false,
  leftIcon,
  rightIcon,
  onClick,
  type = 'button',
  className = '',
  ariaLabel,
  dataTestId,
}) => {
  // Component styles based on props
  const baseClasses = 'btn';
  const variantClasses = {
    default: 'btn-default',
    primary: 'btn-primary',
    secondary: 'btn-secondary',
    success: 'btn-success',
    danger: 'btn-danger',
    ghost: 'btn-ghost',
  };
  const sizeClasses = {
    small: 'btn-sm',
    medium: 'btn-md',
    large: 'btn-lg',
  };

  const classes = [
    baseClasses,
    variantClasses[variant],
    sizeClasses[size],
    fullWidth && 'btn-full-width',
    isLoading && 'btn-loading',
    disabled && 'btn-disabled',
    className,
  ]
    .filter(Boolean)
    .join(' ');

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    if (!disabled && !isLoading && onClick) {
      onClick(event);
    }
  };

  return (
    <button
      type={type}
      className={classes}
      onClick={handleClick}
      disabled={disabled || isLoading}
      aria-label={ariaLabel}
      aria-busy={isLoading}
      data-testid={dataTestId}
    >
      {isLoading && <span className="btn-spinner" aria-hidden="true" />}
      {!isLoading && leftIcon && <span className="btn-icon-left">{leftIcon}</span>}
      <span className="btn-content">{children}</span>
      {!isLoading && rightIcon && <span className="btn-icon-right">{rightIcon}</span>}
    </button>
  );
};

Button.displayName = 'Button';

// Default export
export default Button;
```

### Complex Component Example: Data Table

```typescript
import React, { useState, useMemo, useCallback } from 'react';

/**
 * A feature-rich data table component with sorting, filtering, and pagination.
 *
 * This component displays tabular data with built-in support for sorting,
 * filtering, pagination, row selection, and custom cell rendering. It's
 * designed to handle large datasets efficiently using virtualization.
 *
 * @component
 *
 * @example
 * ```tsx
 * // Basic usage
 * const columns = [
 *   { key: 'name', label: 'Name', sortable: true },
 *   { key: 'email', label: 'Email', sortable: true },
 *   { key: 'role', label: 'Role' },
 * ];
 *
 * const data = [
 *   { id: '1', name: 'John Doe', email: 'john@example.com', role: 'Admin' },
 *   { id: '2', name: 'Jane Smith', email: 'jane@example.com', role: 'User' },
 * ];
 *
 * <DataTable
 *   columns={columns}
 *   data={data}
 *   keyField="id"
 * />
 * ```
 *
 * @example
 * ```tsx
 * // With custom cell rendering and actions
 * const columns = [
 *   {
 *     key: 'name',
 *     label: 'Name',
 *     sortable: true,
 *     renderCell: (row) => (
 *       <div className="flex items-center">
 *         <Avatar src={row.avatar} />
 *         <span>{row.name}</span>
 *       </div>
 *     ),
 *   },
 *   {
 *     key: 'status',
 *     label: 'Status',
 *     renderCell: (row) => <StatusBadge status={row.status} />,
 *   },
 *   {
 *     key: 'actions',
 *     label: 'Actions',
 *     renderCell: (row) => (
 *       <>
 *         <Button size="small" onClick={() => handleEdit(row.id)}>Edit</Button>
 *         <Button size="small" variant="danger" onClick={() => handleDelete(row.id)}>
 *           Delete
 *         </Button>
 *       </>
 *     ),
 *   },
 * ];
 *
 * <DataTable
 *   columns={columns}
 *   data={users}
 *   keyField="id"
 *   selectable
 *   onSelectionChange={setSelectedUsers}
 *   pagination={{
 *     pageSize: 20,
 *     showSizeChanger: true,
 *   }}
 * />
 * ```
 *
 * @see {@link Column} for column configuration options
 * @see {@link PaginationConfig} for pagination settings
 */

export interface Column<T = any> {
  /**
   * Unique identifier for the column.
   */
  key: string;

  /**
   * Display label for the column header.
   */
  label: string;

  /**
   * If true, enables sorting for this column.
   * @default false
   */
  sortable?: boolean;

  /**
   * If true, enables filtering for this column.
   * @default false
   */
  filterable?: boolean;

  /**
   * Custom width for the column (e.g., '200px', '20%').
   */
  width?: string;

  /**
   * Text alignment for the column.
   * @default 'left'
   */
  align?: 'left' | 'center' | 'right';

  /**
   * Custom render function for cell content.
   */
  renderCell?: (row: T, column: Column<T>) => React.ReactNode;

  /**
   * Custom render function for header content.
   */
  renderHeader?: (column: Column<T>) => React.ReactNode;

  /**
   * Custom sort function for this column.
   */
  sortFunction?: (a: T, b: T) => number;

  /**
   * If true, the column is hidden.
   * @default false
   */
  hidden?: boolean;
}

export interface PaginationConfig {
  /**
   * Number of rows per page.
   * @default 10
   */
  pageSize?: number;

  /**
   * Current page number (0-indexed).
   * @default 0
   */
  currentPage?: number;

  /**
   * If true, shows page size selector.
   * @default false
   */
  showSizeChanger?: boolean;

  /**
   * Available page size options.
   * @default [10, 20, 50, 100]
   */
  pageSizeOptions?: number[];

  /**
   * Callback fired when page changes.
   */
  onPageChange?: (page: number) => void;

  /**
   * Callback fired when page size changes.
   */
  onPageSizeChange?: (pageSize: number) => void;
}

export interface DataTableProps<T = any> {
  /**
   * Array of column definitions.
   */
  columns: Column<T>[];

  /**
   * Array of data objects to display.
   */
  data: T[];

  /**
   * Field name to use as the unique key for each row.
   * @default 'id'
   */
  keyField?: keyof T;

  /**
   * If true, adds checkboxes for row selection.
   * @default false
   */
  selectable?: boolean;

  /**
   * Array of selected row keys.
   */
  selectedRows?: any[];

  /**
   * Callback fired when row selection changes.
   */
  onSelectionChange?: (selectedKeys: any[]) => void;

  /**
   * Pagination configuration. If undefined, pagination is disabled.
   */
  pagination?: PaginationConfig;

  /**
   * If true, shows a loading state.
   * @default false
   */
  loading?: boolean;

  /**
   * Message to display when there's no data.
   * @default 'No data available'
   */
  emptyMessage?: string;

  /**
   * Callback fired when a row is clicked.
   */
  onRowClick?: (row: T, index: number) => void;

  /**
   * Additional CSS class names for the table.
   */
  className?: string;

  /**
   * If true, makes the table header sticky.
   * @default false
   */
  stickyHeader?: boolean;

  /**
   * Custom styles for the table container.
   */
  style?: React.CSSProperties;

  /**
   * Default sort configuration.
   */
  defaultSort?: {
    columnKey: string;
    direction: 'asc' | 'desc';
  };
}

export const DataTable = <T extends Record<string, any>>({
  columns,
  data,
  keyField = 'id' as keyof T,
  selectable = false,
  selectedRows = [],
  onSelectionChange,
  pagination,
  loading = false,
  emptyMessage = 'No data available',
  onRowClick,
  className = '',
  stickyHeader = false,
  style,
  defaultSort,
}: DataTableProps<T>) => {
  // State management
  const [sortConfig, setSortConfig] = useState(
    defaultSort || { columnKey: '', direction: 'asc' as 'asc' | 'desc' }
  );
  const [currentPage, setCurrentPage] = useState(pagination?.currentPage || 0);
  const [pageSize, setPageSize] = useState(pagination?.pageSize || 10);

  // Computed values
  const sortedData = useMemo(() => {
    if (!sortConfig.columnKey) return data;

    const column = columns.find((col) => col.key === sortConfig.columnKey);
    if (!column || !column.sortable) return data;

    const sorted = [...data].sort((a, b) => {
      if (column.sortFunction) {
        return column.sortFunction(a, b);
      }

      const aValue = a[sortConfig.columnKey];
      const bValue = b[sortConfig.columnKey];

      if (aValue < bValue) return -1;
      if (aValue > bValue) return 1;
      return 0;
    });

    return sortConfig.direction === 'desc' ? sorted.reverse() : sorted;
  }, [data, sortConfig, columns]);

  const paginatedData = useMemo(() => {
    if (!pagination) return sortedData;

    const start = currentPage * pageSize;
    const end = start + pageSize;
    return sortedData.slice(start, end);
  }, [sortedData, currentPage, pageSize, pagination]);

  // Event handlers
  const handleSort = useCallback((columnKey: string) => {
    setSortConfig((prev) => ({
      columnKey,
      direction:
        prev.columnKey === columnKey && prev.direction === 'asc' ? 'desc' : 'asc',
    }));
  }, []);

  const handleSelectAll = useCallback(
    (checked: boolean) => {
      if (!onSelectionChange) return;

      if (checked) {
        const allKeys = paginatedData.map((row) => row[keyField]);
        onSelectionChange(allKeys);
      } else {
        onSelectionChange([]);
      }
    },
    [paginatedData, keyField, onSelectionChange]
  );

  const handleSelectRow = useCallback(
    (rowKey: any, checked: boolean) => {
      if (!onSelectionChange) return;

      if (checked) {
        onSelectionChange([...selectedRows, rowKey]);
      } else {
        onSelectionChange(selectedRows.filter((key) => key !== rowKey));
      }
    },
    [selectedRows, onSelectionChange]
  );

  // Render helpers
  const renderHeader = () => (
    <thead className={stickyHeader ? 'sticky-header' : ''}>
      <tr>
        {selectable && (
          <th className="select-cell">
            <input
              type="checkbox"
              checked={selectedRows.length === paginatedData.length}
              onChange={(e) => handleSelectAll(e.target.checked)}
              aria-label="Select all rows"
            />
          </th>
        )}
        {columns
          .filter((col) => !col.hidden)
          .map((column) => (
            <th
              key={column.key}
              style={{ width: column.width, textAlign: column.align }}
              className={column.sortable ? 'sortable' : ''}
              onClick={() => column.sortable && handleSort(column.key)}
            >
              {column.renderHeader ? column.renderHeader(column) : column.label}
              {column.sortable && sortConfig.columnKey === column.key && (
                <span className="sort-indicator">
                  {sortConfig.direction === 'asc' ? '↑' : '↓'}
                </span>
              )}
            </th>
          ))}
      </tr>
    </thead>
  );

  const renderBody = () => {
    if (loading) {
      return (
        <tbody>
          <tr>
            <td colSpan={columns.length + (selectable ? 1 : 0)} className="loading-cell">
              Loading...
            </td>
          </tr>
        </tbody>
      );
    }

    if (paginatedData.length === 0) {
      return (
        <tbody>
          <tr>
            <td colSpan={columns.length + (selectable ? 1 : 0)} className="empty-cell">
              {emptyMessage}
            </td>
          </tr>
        </tbody>
      );
    }

    return (
      <tbody>
        {paginatedData.map((row, rowIndex) => {
          const rowKey = row[keyField];
          const isSelected = selectedRows.includes(rowKey);

          return (
            <tr
              key={rowKey}
              className={isSelected ? 'selected' : ''}
              onClick={() => onRowClick?.(row, rowIndex)}
            >
              {selectable && (
                <td className="select-cell">
                  <input
                    type="checkbox"
                    checked={isSelected}
                    onChange={(e) => handleSelectRow(rowKey, e.target.checked)}
                    aria-label={`Select row ${rowIndex + 1}`}
                  />
                </td>
              )}
              {columns
                .filter((col) => !col.hidden)
                .map((column) => (
                  <td key={column.key} style={{ textAlign: column.align }}>
                    {column.renderCell
                      ? column.renderCell(row, column)
                      : row[column.key]}
                  </td>
                ))}
            </tr>
          );
        })}
      </tbody>
    );
  };

  const renderPagination = () => {
    if (!pagination) return null;

    const totalPages = Math.ceil(sortedData.length / pageSize);

    return (
      <div className="table-pagination">
        <div className="pagination-info">
          Showing {currentPage * pageSize + 1} to{' '}
          {Math.min((currentPage + 1) * pageSize, sortedData.length)} of{' '}
          {sortedData.length} entries
        </div>
        <div className="pagination-controls">
          <button
            onClick={() => setCurrentPage(Math.max(0, currentPage - 1))}
            disabled={currentPage === 0}
          >
            Previous
          </button>
          <span>
            Page {currentPage + 1} of {totalPages}
          </span>
          <button
            onClick={() => setCurrentPage(Math.min(totalPages - 1, currentPage + 1))}
            disabled={currentPage >= totalPages - 1}
          >
            Next
          </button>
        </div>
        {pagination.showSizeChanger && (
          <select
            value={pageSize}
            onChange={(e) => {
              setPageSize(Number(e.target.value));
              setCurrentPage(0);
            }}
          >
            {(pagination.pageSizeOptions || [10, 20, 50, 100]).map((size) => (
              <option key={size} value={size}>
                {size} per page
              </option>
            ))}
          </select>
        )}
      </div>
    );
  };

  return (
    <div className={`data-table-container ${className}`} style={style}>
      <table className="data-table">
        {renderHeader()}
        {renderBody()}
      </table>
      {renderPagination()}
    </div>
  );
};

DataTable.displayName = 'DataTable';

export default DataTable;
```

---

## Vue Components

### Vue 3 Composition API Component

```vue
<template>
  <!--
    Modal Component
    
    A flexible modal dialog component with support for custom headers,
    footers, and content. Includes built-in animation transitions and
    focus trap for accessibility.
    
    @example
    <Modal
      :open="isModalOpen"
      title="Confirm Action"
      @close="isModalOpen = false"
      @confirm="handleConfirm"
    >
      <p>Are you sure you want to proceed?</p>
    </Modal>
    
    @example With custom footer
    <Modal :open="isOpen" title="Custom Modal" :show-footer="false">
      <template #default>
        <p>Modal content goes here</p>
      </template>
      <template #footer>
        <Button @click="handleCustomAction">Custom Action</Button>
      </template>
    </Modal>
  -->
  <Teleport to="body">
    <Transition name="modal-fade">
      <div
        v-if="open"
        class="modal-overlay"
        @click="handleOverlayClick"
        role="dialog"
        aria-modal="true"
        :aria-labelledby="titleId"
      >
        <div
          class="modal-container"
          :class="[sizeClass, customClass]"
          @click.stop
        >
          <!-- Header -->
          <div v-if="showHeader" class="modal-header">
            <slot name="header">
              <h2 :id="titleId" class="modal-title">{{ title }}</h2>
            </slot>
            <button
              v-if="closable"
              class="modal-close"
              @click="handleClose"
              aria-label="Close modal"
            >
              <IconClose />
            </button>
          </div>

          <!-- Body -->
          <div class="modal-body">
            <slot></slot>
          </div>

          <!-- Footer -->
          <div v-if="showFooter" class="modal-footer">
            <slot name="footer">
              <Button variant="secondary" @click="handleClose">
                {{ cancelText }}
              </Button>
              <Button variant="primary" @click="handleConfirm" :loading="loading">
                {{ confirmText }}
              </Button>
            </slot>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
/**
 * Modal Component
 * 
 * A customizable modal dialog with built-in accessibility features.
 * 
 * @component
 * @example
 * <Modal :open="true" title="My Modal" @close="handleClose">
 *   Content goes here
 * </Modal>
 */

import { computed, watch, onMounted, onUnmounted, ref } from 'vue';
import { useFocusTrap } from '@vueuse/integrations/useFocusTrap';
import Button from './Button.vue';
import IconClose from './icons/IconClose.vue';

/**
 * Component Props
 */
export interface ModalProps {
  /**
   * Controls whether the modal is visible.
   */
  open: boolean;

  /**
   * Title text for the modal header.
   */
  title?: string;

  /**
   * Size of the modal.
   * @default 'medium'
   */
  size?: 'small' | 'medium' | 'large' | 'fullscreen';

  /**
   * If true, shows the header section.
   * @default true
   */
  showHeader?: boolean;

  /**
   * If true, shows the footer section.
   * @default true
   */
  showFooter?: boolean;

  /**
   * If true, shows the close button in the header.
   * @default true
   */
  closable?: boolean;

  /**
   * If true, clicking the overlay closes the modal.
   * @default true
   */
  closeOnOverlayClick?: boolean;

  /**
   * If true, pressing Escape closes the modal.
   * @default true
   */
  closeOnEscape?: boolean;

  /**
   * Text for the confirm button.
   * @default 'Confirm'
   */
  confirmText?: string;

  /**
   * Text for the cancel button.
   * @default 'Cancel'
   */
  cancelText?: string;

  /**
   * If true, shows loading state on confirm button.
   * @default false
   */
  loading?: boolean;

  /**
   * Additional CSS classes to apply to the modal.
   */
  customClass?: string;
}

const props = withDefaults(defineProps<ModalProps>(), {
  size: 'medium',
  showHeader: true,
  showFooter: true,
  closable: true,
  closeOnOverlayClick: true,
  closeOnEscape: true,
  confirmText: 'Confirm',
  cancelText: 'Cancel',
  loading: false,
});

/**
 * Component Emits
 */
export interface ModalEmits {
  /**
   * Emitted when the modal should be closed.
   * @param event - The close event
   */
  (e: 'close'): void;

  /**
   * Emitted when the confirm button is clicked.
   * @param event - The confirm event
   */
  (e: 'confirm'): void;

  /**
   * Emitted when the modal is opened.
   * @param event - The open event
   */
  (e: 'open'): void;
}

const emit = defineEmits<ModalEmits>();

// Unique ID for accessibility
const titleId = ref(`modal-title-${Math.random().toString(36).substr(2, 9)}`);

// Computed classes
const sizeClass = computed(() => `modal-${props.size}`);

/**
 * Handles the close action.
 */
const handleClose = () => {
  emit('close');
};

/**
 * Handles the confirm action.
 */
const handleConfirm = () => {
  emit('confirm');
};

/**
 * Handles clicks on the overlay.
 */
const handleOverlayClick = () => {
  if (props.closeOnOverlayClick) {
    handleClose();
  }
};

/**
 * Handles keyboard events.
 */
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && props.closeOnEscape && props.open) {
    handleClose();
  }
};

// Lifecycle hooks
onMounted(() => {
  document.addEventListener('keydown', handleKeydown);
});

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown);
});

// Watch for modal open state
watch(
  () => props.open,
  (newValue) => {
    if (newValue) {
      emit('open');
      // Prevent body scroll when modal is open
      document.body.style.overflow = 'hidden';
    } else {
      // Restore body scroll when modal is closed
      document.body.style.overflow = '';
    }
  }
);

// Cleanup on unmount
onUnmounted(() => {
  document.body.style.overflow = '';
});
</script>

<script lang="ts">
/**
 * Slots available in this component:
 * 
 * @slot default - Main content of the modal
 * @slot header - Custom header content (replaces default title)
 * @slot footer - Custom footer content (replaces default buttons)
 */
export default {
  name: 'Modal',
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-container {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  max-height: 90vh;
  overflow-y: auto;
}

.modal-small {
  width: 400px;
}

.modal-medium {
  width: 600px;
}

.modal-large {
  width: 900px;
}

.modal-fullscreen {
  width: 100%;
  height: 100%;
  max-height: 100vh;
  border-radius: 0;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.modal-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.modal-close {
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.5rem;
  color: #6b7280;
}

.modal-close:hover {
  color: #1f2937;
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1.5rem;
  border-top: 1px solid #e5e7eb;
}

/* Transitions */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}
</style>
```

---

## Angular Components

### Angular Component with TypeScript

```typescript
/**
 * Card Component
 * 
 * A flexible card component for displaying content with optional header,
 * footer, and actions. Supports different variants and sizes.
 * 
 * @component
 * @example
 * <app-card
 *   title="User Profile"
 *   [elevated]="true"
 *   [loading]="isLoading"
 * >
 *   <p>Card content goes here</p>
 * </app-card>
 * 
 * @example With custom header and footer
 * <app-card>
 *   <ng-container header>
 *     <h3>Custom Header</h3>
 *   </ng-container>
 *   
 *   <p>Main content</p>
 *   
 *   <ng-container footer>
 *     <button (click)="handleAction()">Action</button>
 *   </ng-container>
 * </app-card>
 */

import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  ViewEncapsulation,
} from '@angular/core';

/**
 * Card variant types.
 */
export type CardVariant = 'default' | 'outlined' | 'elevated';

/**
 * Card size options.
 */
export type CardSize = 'small' | 'medium' | 'large';

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None,
})
export class CardComponent {
  /**
   * Title text for the card header.
   */
  @Input() title?: string;

  /**
   * Subtitle text for the card header.
   */
  @Input() subtitle?: string;

  /**
   * Visual variant of the card.
   * @default 'default'
   */
  @Input() variant: CardVariant = 'default';

  /**
   * Size of the card.
   * @default 'medium'
   */
  @Input() size: CardSize = 'medium';

  /**
   * If true, shows a loading state with skeleton.
   * @default false
   */
  @Input() loading = false;

  /**
   * If true, makes the card clickable with hover effects.
   * @default false
   */
  @Input() clickable = false;

  /**
   * If true, disables the card interactions.
   * @default false
   */
  @Input() disabled = false;

  /**
   * If true, adds elevation shadow to the card.
   * @default false
   */
  @Input() elevated = false;

  /**
   * If true, shows a divider between header and body.
   * @default true
   */
  @Input() showDivider = true;

  /**
   * Additional CSS classes to apply to the card.
   */
  @Input() customClass?: string;

  /**
   * Data attribute for testing.
   */
  @Input() dataTestId?: string;

  /**
   * Emitted when the card is clicked (only if clickable is true).
   */
  @Output() cardClick = new EventEmitter<MouseEvent>();

  /**
   * Emitted when an action button in the card is clicked.
   */
  @Output() actionClick = new EventEmitter<string>();

  /**
   * Gets the CSS classes for the card based on props.
   */
  get cardClasses(): string[] {
    return [
      'card',
      `card-${this.variant}`,
      `card-${this.size}`,
      this.elevated && 'card-elevated',
      this.clickable && 'card-clickable',
      this.disabled && 'card-disabled',
      this.customClass,
    ].filter(Boolean) as string[];
  }

  /**
   * Handles card click events.
   * @param event - The mouse event
   */
  handleClick(event: MouseEvent): void {
    if (!this.disabled && this.clickable) {
      this.cardClick.emit(event);
    }
  }

  /**
   * Handles action button clicks.
   * @param actionId - The identifier of the clicked action
   */
  handleAction(actionId: string): void {
    this.actionClick.emit(actionId);
  }
}
```

```html
<!-- card.component.html -->

<!--
  Card Template
  
  Provides slots for header, body, footer, and actions content.
-->
<div
  [class]="cardClasses.join(' ')"
  [attr.data-testid]="dataTestId"
  (click)="handleClick($event)"
  [attr.role]="clickable ? 'button' : null"
  [attr.tabindex]="clickable && !disabled ? 0 : null"
>
  <!-- Loading State -->
  <div *ngIf="loading" class="card-skeleton">
    <div class="skeleton-header"></div>
    <div class="skeleton-body"></div>
    <div class="skeleton-footer"></div>
  </div>

  <!-- Card Content -->
  <ng-container *ngIf="!loading">
    <!-- Header -->
    <div *ngIf="title || subtitle" class="card-header" [class.with-divider]="showDivider">
      <ng-content select="[header]"></ng-content>
      
      <div *ngIf="!hasHeaderSlot" class="card-header-content">
        <h3 *ngIf="title" class="card-title">{{ title }}</h3>
        <p *ngIf="subtitle" class="card-subtitle">{{ subtitle }}</p>
      </div>

      <div class="card-header-actions">
        <ng-content select="[headerActions]"></ng-content>
      </div>
    </div>

    <!-- Body -->
    <div class="card-body">
      <ng-content></ng-content>
    </div>

    <!-- Footer -->
    <div *ngIf="hasFooterSlot" class="card-footer">
      <ng-content select="[footer]"></ng-content>
    </div>

    <!-- Actions -->
    <div *ngIf="hasActionsSlot" class="card-actions">
      <ng-content select="[actions]"></ng-content>
    </div>
  </ng-container>
</div>
```

---

## Web Components

### Custom Element with TypeScript

```typescript
/**
 * Progress Bar Web Component
 * 
 * A customizable progress bar element that can be used in any web application.
 * Supports both determinate and indeterminate states, multiple variants,
 * and accessibility features.
 * 
 * @element progress-bar
 * 
 * @attr {number} value - Current progress value (0-100)
 * @attr {number} max - Maximum progress value (default: 100)
 * @attr {string} variant - Visual variant ('default' | 'success' | 'warning' | 'danger')
 * @attr {boolean} indeterminate - If true, shows indeterminate animation
 * @attr {boolean} show-label - If true, displays the percentage label
 * @attr {string} label - Custom label text
 * @attr {string} size - Size variant ('small' | 'medium' | 'large')
 * 
 * @fires progress-complete - Fired when progress reaches 100%
 * @fires progress-change - Fired when progress value changes
 * 
 * @csspart container - The progress bar container
 * @csspart track - The progress bar track (background)
 * @csspart fill - The progress bar fill (foreground)
 * @csspart label - The progress label
 * 
 * @cssprop [--progress-bar-height=8px] - Height of the progress bar
 * @cssprop [--progress-bar-bg=#e5e7eb] - Background color of the track
 * @cssprop [--progress-bar-fill=#3b82f6] - Fill color of the progress
 * @cssprop [--progress-bar-border-radius=4px] - Border radius
 * 
 * @example
 * ```html
 * <!-- Basic usage -->
 * <progress-bar value="75"></progress-bar>
 * 
 * <!-- With label and variant -->
 * <progress-bar
 *   value="50"
 *   variant="success"
 *   show-label
 *   label="Uploading..."
 * ></progress-bar>
 * 
 * <!-- Indeterminate -->
 * <progress-bar indeterminate></progress-bar>
 * ```
 * 
 * @example JavaScript
 * ```js
 * const progressBar = document.querySelector('progress-bar');
 * 
 * // Update progress
 * progressBar.value = 75;
 * 
 * // Listen to events
 * progressBar.addEventListener('progress-complete', (e) => {
 *   console.log('Progress complete!');
 * });
 * 
 * progressBar.addEventListener('progress-change', (e) => {
 *   console.log('Progress:', e.detail.value);
 * });
 * ```
 */

class ProgressBar extends HTMLElement {
  // Observed attributes
  static get observedAttributes() {
    return [
      'value',
      'max',
      'variant',
      'indeterminate',
      'show-label',
      'label',
      'size',
    ];
  }

  // Private properties
  private _value = 0;
  private _max = 100;
  private _variant: string = 'default';
  private _indeterminate = false;
  private _showLabel = false;
  private _label = '';
  private _size: string = 'medium';

  // Elements
  private container!: HTMLDivElement;
  private track!: HTMLDivElement;
  private fill!: HTMLDivElement;
  private labelElement!: HTMLSpanElement;

  constructor() {
    super();
    this.attachShadow({ mode: 'open' });
  }

  /**
   * Gets the current progress value.
   */
  get value(): number {
    return this._value;
  }

  /**
   * Sets the current progress value.
   * @fires progress-change
   * @fires progress-complete
   */
  set value(val: number) {
    const oldValue = this._value;
    this._value = Math.max(0, Math.min(val, this._max));

    if (oldValue !== this._value) {
      this.render();
      this.dispatchEvent(
        new CustomEvent('progress-change', {
          detail: { value: this._value, max: this._max },
          bubbles: true,
          composed: true,
        })
      );

      if (this._value >= this._max) {
        this.dispatchEvent(
          new CustomEvent('progress-complete', {
            bubbles: true,
            composed: true,
          })
        );
      }
    }
  }

  /**
   * Gets the maximum progress value.
   */
  get max(): number {
    return this._max;
  }

  /**
   * Sets the maximum progress value.
   */
  set max(val: number) {
    this._max = Math.max(1, val);
    this.render();
  }

  /**
   * Lifecycle callback - component connected to DOM.
   */
  connectedCallback() {
    this.render();
  }

  /**
   * Lifecycle callback - attribute changed.
   */
  attributeChangedCallback(name: string, oldValue: string, newValue: string) {
    if (oldValue === newValue) return;

    switch (name) {
      case 'value':
        this.value = parseFloat(newValue) || 0;
        break;
      case 'max':
        this.max = parseFloat(newValue) || 100;
        break;
      case 'variant':
        this._variant = newValue;
        this.render();
        break;
      case 'indeterminate':
        this._indeterminate = this.hasAttribute('indeterminate');
        this.render();
        break;
      case 'show-label':
        this._showLabel = this.hasAttribute('show-label');
        this.render();
        break;
      case 'label':
        this._label = newValue;
        this.render();
        break;
      case 'size':
        this._size = newValue;
        this.render();
        break;
    }
  }

  /**
   * Renders the component.
   */
  private render() {
    if (!this.shadowRoot) return;

    const percentage = (this._value / this._max) * 100;
    const labelText = this._label || `${Math.round(percentage)}%`;

    this.shadowRoot.innerHTML = `
      <style>
        :host {
          display: block;
          width: 100%;
        }

        .container {
          display: flex;
          flex-direction: column;
          gap: 0.5rem;
        }

        .track {
          position: relative;
          width: 100%;
          background-color: var(--progress-bar-bg, #e5e7eb);
          border-radius: var(--progress-bar-border-radius, 4px);
          overflow: hidden;
        }

        .track.small {
          height: var(--progress-bar-height, 4px);
        }

        .track.medium {
          height: var(--progress-bar-height, 8px);
        }

        .track.large {
          height: var(--progress-bar-height, 12px);
        }

        .fill {
          height: 100%;
          transition: width 0.3s ease;
          border-radius: var(--progress-bar-border-radius, 4px);
        }

        .fill.default {
          background-color: var(--progress-bar-fill, #3b82f6);
        }

        .fill.success {
          background-color: var(--progress-bar-fill, #10b981);
        }

        .fill.warning {
          background-color: var(--progress-bar-fill, #f59e0b);
        }

        .fill.danger {
          background-color: var(--progress-bar-fill, #ef4444);
        }

        .fill.indeterminate {
          width: 30% !important;
          animation: indeterminate 1.5s infinite ease-in-out;
        }

        @keyframes indeterminate {
          0% {
            transform: translateX(-100%);
          }
          100% {
            transform: translateX(400%);
          }
        }

        .label {
          font-size: 0.875rem;
          color: #6b7280;
          text-align: center;
        }

        :host([hidden]) {
          display: none;
        }
      </style>

      <div class="container" part="container">
        ${
          this._showLabel
            ? `<span class="label" part="label">${labelText}</span>`
            : ''
        }
        <div class="track ${this._size}" part="track">
          <div
            class="fill ${this._variant} ${
              this._indeterminate ? 'indeterminate' : ''
            }"
            part="fill"
            style="width: ${this._indeterminate ? '30' : percentage}%"
            role="progressbar"
            aria-valuenow="${this._value}"
            aria-valuemin="0"
            aria-valuemax="${this._max}"
            aria-label="${labelText}"
          ></div>
        </div>
      </div>
    `;
  }
}

// Register the custom element
if (!customElements.get('progress-bar')) {
  customElements.define('progress-bar', ProgressBar);
}

export default ProgressBar;
```

---

## Best Practices

### 1. Complete Prop Documentation
Document every prop with type, description, default value, and examples.

### 2. Document Events
List all events/emitters with their payload structure and when they're triggered.

### 3. Provide Multiple Examples
Show basic usage, advanced features, and edge cases in separate examples.

### 4. Document Slots/Children
Clearly explain all available slots or children composition patterns.

### 5. Accessibility Information
Document ARIA attributes, keyboard navigation, and screen reader support.

### 6. Style Customization
Document CSS classes, CSS custom properties, and theming options.

### 7. Performance Notes
Mention virtualization, memoization, or other performance optimizations.

### 8. Browser Compatibility
List browser support and any required polyfills.

### 9. Testing Utilities
Provide data-testid attributes and testing examples.

### 10. Migration Guides
Document breaking changes between versions with migration examples.

---

## Documentation Tools

### React
- **react-docgen:** Extract component information
- **Storybook:** Interactive component documentation
- **Docz:** MDX-based documentation

### Vue
- **vue-docgen-api:** Extract component information
- **Storybook:** Interactive component documentation  
- **VitePress:** Vue-powered static site generator

### Angular
- **Compodoc:** Angular documentation tool
- **Storybook:** Interactive component documentation
- **ng-doc:** Documentation generator

### Web Components
- **custom-elements-manifest:** Metadata for web components
- **Storybook:** Interactive component documentation
- **web-component-analyzer:** Analyze and document custom elements

---

## Conclusion

Well-documented components are essential for maintainable UI libraries and applications. Following these patterns ensures that your components are easy to understand, use, and maintain.

For more documentation guides, see:
- [API Documentation](./API_DOCUMENTATION.md)
- [Function Documentation](./FUNCTION_DOCUMENTATION.md)
- [Best Practices](./BEST_PRACTICES.md)
