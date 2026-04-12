# Project Conventions & Patterns

## 1. API Response Envelope

All API responses follow this structure:

```typescript
// Success Response
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... },
  "errors": null,
  "pagination": {
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10
  }
}

// Error Response
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": ["Validation error 1", "Validation error 2"]
}
```

## 2. Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `validation` | 400 | Input validation failed |
| `not_found` | 404 | Resource not found |
| `conflict` | 409 | Duplicate or conflict |
| `unauthorized` | 401 | Authentication required |
| `forbidden` | 403 | Permission denied |
| `bad_request` | 400 | Invalid request |
| `internal` | 500 | Server error |

## 3. Pagination DTO

```typescript
interface PaginationParams {
  pageIndex: number;    // 0-based
  pageSize: number;      // 10, 20, 50, 100
  search?: string;       // Search term
  sortBy?: string;      // Column name
  sortDir?: 'asc' | 'desc';
  filters?: Filter[];    // Advanced filters
}

interface Filter {
  field: string;
  operator: 'eq' | 'ne' | 'gt' | 'lt' | 'contains' | 'in';
  value: any;
}
```

## 4. Currency Handling

- Base currency: BDT (৳)
- No paisa decimals
- Display: `new Intl.NumberFormat('en-BD', { style: 'currency', currency: 'BDT' }).format(amount)`
- Storage: Decimal(18,2)

## 5. Soft Delete

All entities use soft delete via `IsDeleted` column:
- Global query filter applied in DbContext
- Use `.IgnoreQueryFilters()` for admin queries

## 6. Audit Columns

| Column | Type | Description |
|--------|------|-------------|
| `CreatedAt` | DateTime | Auto-set on create |
| `CreatedBy` | Guid | Current user ID |
| `UpdatedAt` | DateTime | Auto-set on update |
| `UpdatedBy` | Guid | Current user ID |
| `IsDeleted` | bool | Soft delete flag |

## 7. File Upload

- Local development: `/uploads` folder
- Production: Azure Blob Storage
- Max size: 5MB
- Allowed types: jpg, png, webp, gif, pdf

## 8. JWT Authentication

- Access token: 60 minutes expiry
- Refresh token: 7 days expiry
- Storage: httpOnly cookie (recommended) or memory
- 401 → Silent refresh → Retry original request

## 9. Naming Conventions

### Backend
- Entities: `PascalCase` (e.g., `Products`, `OrderItems`)
- DbSet: Plural `PascalCase` (e.g., `Products`)
- Endpoints: `/api/[resource]` lowercase plural
- Commands/Queries: `[Action][Entity]` (e.g., `CreateProduct`, `GetProducts`)

### Frontend
- Components: `PascalCase` (e.g., `ProductList.tsx`)
- Hooks: `camelCase` with `use` prefix (e.g., `useProducts.ts`)
- API: `camelCase` endpoints
- Types: `PascalCase` with suffix (e.g., `ProductResponse`)

## 10. State Management

- **Server State**: TanStack Query (React Query)
- **Client State**: Zustand for UI state (sidebar, theme)
- **Form State**: React Hook Form + Zod

## 11. Validation

- Backend: FluentValidation
- Frontend: Zod schemas matching backend validators

## 12. Database

- Provider: SQL Server
- Migrations: Code-first EF Core
- Connection: Configure in `appsettings.json`
