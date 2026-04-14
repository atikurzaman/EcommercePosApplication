import apiClient from './client';

// ── Types ──────────────────────────────────────────────────────────────────

export interface CashShift {
  id: string;
  warehouseId: string;
  warehouseName: string;
  posCounterId: string;
  posCounterName: string;
  openedById: string;
  openedByName: string;
  closedById?: string;
  closedByName?: string;
  openedAt: string;
  closedAt?: string;
  openingCash: number;
  closingCash?: number;
  expectedCash?: number;
  cashDifference?: number;
  totalSales: number;
  totalReturns: number;
  status: string;
}

export interface CashShiftSummary extends CashShift {
  transactionCount: number;
  cashPayments: number;
  cardPayments: number;
  mobilePayments: number;
  otherPayments: number;
  drawerEvents: CashDrawerEvent[];
}

export interface PosCounter {
  id: string;
  warehouseId: string;
  warehouseName: string;
  counterName: string;
  counterCode: string;
  isActive: boolean;
}

export interface PosTerminal {
  id: string;
  posCounterId: string;
  posCounterName: string;
  terminalName: string;
  terminalCode: string;
  deviceIdentifier?: string;
  isActive: boolean;
}

export interface CashDrawerEvent {
  id: string;
  cashShiftId: string;
  eventType: string;
  amount: number;
  reason?: string;
  createdAt: string;
  createdByName?: string;
}

export interface DayEndSummary {
  id: string;
  warehouseId: string;
  warehouseName: string;
  summaryDate: string;
  salesCount: number;
  salesAmount: number;
  cashAmount: number;
  cardAmount: number;
  mobileAmount: number;
  returnsCount: number;
  returnsAmount: number;
  expensesAmount: number;
  netAmount: number;
  status: string;
  generatedAt?: string;
}

export interface Expense {
  id: string;
  warehouseId: string;
  warehouseName: string;
  category: string;
  description: string;
  amount: number;
  paymentMethod: string;
  expenseDate: string;
  approvedById?: string;
  approvedByName?: string;
  createdByName?: string;
  notes?: string;
}

export interface PosTransaction {
  id: string;
  receiptNumber: string;
  saleDate: string;
  saleType: string;
  subTotal: number;
  discountAmount: number;
  grandTotal: number;
  paidAmount: number;
  changeAmount: number;
  totalItemQuantity: number;
  status: string;
  cashierName?: string;
  customerName?: string;
  warehouseName?: string;
}

export interface PosTransactionDetail extends PosTransaction {
  totalTaxAmount: number;
  roundOffAmount: number;
  voidReason?: string;
  notes?: string;
  customerPhone?: string;
  lines: PosTransactionLine[];
  payments: PaymentTender[];
}

export interface PosTransactionLine {
  id: string;
  productName: string;
  sku?: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
  taxAmount: number;
  lineTotal: number;
}

export interface PaymentTender {
  id: string;
  paymentMethod: string;
  amount: number;
  cardLastFour?: string;
}

export interface PosReturn {
  id: string;
  returnNumber: string;
  returnDate: string;
  warehouseId: string;
  warehouseName: string;
  customerName?: string;
  originalReceiptNumber?: string;
  totalAmount: number;
  itemCount: number;
  status: string;
  reason?: string;
}

export interface PosReturnDetail extends PosReturn {
  lines: PosReturnLine[];
  notes?: string;
  processedByName?: string;
}

export interface PosReturnLine {
  id: string;
  productName: string;
  sku?: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
  reason?: string;
}

export interface Warehouse {
  id: string;
  code: string;
  name: string;
  siteType: string;
  contactPerson?: string;
  managerName?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  area?: string;
  phone?: string;
  email?: string;
  isDefault: boolean;
  isActive: boolean;
  createdAt?: string;
}

export interface WarehouseStats {
  totalCounters: number;
  totalTerminals: number;
  activeShifts: number;
  todaySales: number;
}

// ── Filters ────────────────────────────────────────────────────────────────

interface PaginationFilter {
  pageIndex?: number;
  pageSize?: number;
  search?: string;
}

interface PosTransactionFilter extends PaginationFilter {
  warehouseId?: string;
  cashierId?: string;
  status?: string;
  startDate?: string;
  endDate?: string;
}

interface ExpenseFilter extends PaginationFilter {
  warehouseId?: string;
  category?: string;
  startDate?: string;
  endDate?: string;
}

// ── Paginated response helper ──────────────────────────────────────────────

interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
}

// ── API Objects ────────────────────────────────────────────────────────────

export const cashShiftApi = {
  getAll: (filter?: PaginationFilter) =>
    apiClient.get<PaginatedResponse<CashShift>>('/cash-shifts', { params: filter }),

  getActive: (warehouseId?: string) =>
    apiClient.get<CashShift[]>('/cash-shifts/active', { params: { warehouseId } }),

  getSummary: (id: string) =>
    apiClient.get<CashShiftSummary>(`/cash-shifts/${id}/summary`),

  open: (data: { warehouseId: string; posCounterId: string; openingCash: number }) =>
    apiClient.post<CashShift>('/cash-shifts/open', data),

  close: (id: string, data: { closingCash: number; notes?: string }) =>
    apiClient.post<CashShift>(`/cash-shifts/${id}/close`, data),
};

export const posCounterApi = {
  getAll: (filter?: PaginationFilter) =>
    apiClient.get<PaginatedResponse<PosCounter>>('/pos-counters', { params: filter }),

  getById: (id: string) =>
    apiClient.get<PosCounter>(`/pos-counters/${id}`),

  create: (data: Partial<PosCounter>) =>
    apiClient.post<PosCounter>('/pos-counters', data),

  update: (id: string, data: Partial<PosCounter>) =>
    apiClient.put<PosCounter>(`/pos-counters/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/pos-counters/${id}`),
};

export const posTerminalApi = {
  getAll: (filter?: PaginationFilter) =>
    apiClient.get<PaginatedResponse<PosTerminal>>('/pos-terminals', { params: filter }),

  create: (data: Partial<PosTerminal>) =>
    apiClient.post<PosTerminal>('/pos-terminals', data),

  update: (id: string, data: Partial<PosTerminal>) =>
    apiClient.put<PosTerminal>(`/pos-terminals/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/pos-terminals/${id}`),
};

export const cashDrawerEventApi = {
  getByShift: (shiftId: string) =>
    apiClient.get<CashDrawerEvent[]>(`/cash-drawer-events/shift/${shiftId}`),

  record: (data: { cashShiftId: string; eventType: string; amount: number; reason?: string }) =>
    apiClient.post<CashDrawerEvent>('/cash-drawer-events', data),
};

export const dayEndSummaryApi = {
  getAll: (filter?: PaginationFilter & { warehouseId?: string }) =>
    apiClient.get<PaginatedResponse<DayEndSummary>>('/day-end-summaries', { params: filter }),

  getById: (id: string) =>
    apiClient.get<DayEndSummary>(`/day-end-summaries/${id}`),

  generate: (data: { warehouseId: string; summaryDate: string }) =>
    apiClient.post<DayEndSummary>('/day-end-summaries/generate', data),
};

export const expenseApi = {
  getAll: (filter?: ExpenseFilter) =>
    apiClient.get<PaginatedResponse<Expense>>('/expenses', { params: filter }),

  getById: (id: string) =>
    apiClient.get<Expense>(`/expenses/${id}`),

  create: (data: Partial<Expense>) =>
    apiClient.post<Expense>('/expenses', data),

  update: (id: string, data: Partial<Expense>) =>
    apiClient.put<Expense>(`/expenses/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/expenses/${id}`),
};

export const posTransactionApiV2 = {
  getAll: (filter?: PosTransactionFilter) =>
    apiClient.get<PaginatedResponse<PosTransaction>>('/pos-transactions', { params: filter }),

  getById: (id: string) =>
    apiClient.get<PosTransactionDetail>(`/pos-transactions/${id}`),

  create: (data: {
    cashShiftId: string;
    posCounterId: string;
    customerId?: string;
    customerName?: string;
    customerPhone?: string;
    saleType?: string;
    lines: { productId: string; productName: string; sku?: string; quantity: number; unitPrice: number; discountAmount?: number; taxAmount?: number; lineTotal: number }[];
    payments: { paymentMethod: string; amount: number }[];
    notes?: string;
  }) =>
    apiClient.post<PosTransaction>('/pos-transactions', data),

  hold: (data: {
    cashShiftId: string;
    posCounterId: string;
    customerName?: string;
    lines: { productId: string; productName: string; sku?: string; quantity: number; unitPrice: number; lineTotal: number }[];
    notes?: string;
  }) =>
    apiClient.post<PosTransaction>('/pos-transactions/hold', data),

  resume: (id: string) =>
    apiClient.post<PosTransactionDetail>(`/pos-transactions/${id}/resume`),

  void: (id: string, data: { voidedBy: string; reason: string }) =>
    apiClient.post(`/pos-transactions/${id}/void`, data),
};

export const posReturnApi = {
  getAll: (filter?: PaginationFilter & { warehouseId?: string }) =>
    apiClient.get<PaginatedResponse<PosReturn>>('/pos-returns', { params: filter }),

  getById: (id: string) =>
    apiClient.get<PosReturnDetail>(`/pos-returns/${id}`),

  process: (data: {
    originalTransactionId?: string;
    warehouseId: string;
    customerName?: string;
    reason: string;
    lines: { productId: string; productName: string; sku?: string; quantity: number; unitPrice: number; lineTotal: number; reason?: string }[];
    notes?: string;
  }) =>
    apiClient.post<PosReturn>('/pos-returns', data),
};

export const warehouseApiV2 = {
  getAll: (filter?: PaginationFilter) =>
    apiClient.get<PaginatedResponse<Warehouse>>('/warehouses', { params: filter }),

  getById: (id: string) =>
    apiClient.get<Warehouse>(`/warehouses/${id}`),

  create: (data: Partial<Warehouse>) =>
    apiClient.post<Warehouse>('/warehouses', data),

  update: (id: string, data: Partial<Warehouse>) =>
    apiClient.put<Warehouse>(`/warehouses/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/warehouses/${id}`),

  toggleActive: (id: string) =>
    apiClient.post<Warehouse>(`/warehouses/${id}/toggle-active`),

  getStats: (id: string) =>
    apiClient.get<WarehouseStats>(`/warehouses/${id}/stats`),
};
