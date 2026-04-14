import apiClient from './client';

export interface StockItem {
  id: string;
  productId: string;
  productName: string;
  variantId?: string;
  warehouseId: string;
  warehouseName: string;
  quantityOnHand: number;
  reservedQuantity: number;
  averageCostPrice: number;
  reorderLevel: number;
  lastUpdatedAt: string;
}

export interface StockMovement {
  id: string;
  productId: string;
  productName: string;
  variantId?: string;
  movementTypeCode: string;
  movementTypeName: string;
  fromWarehouseId?: string;
  fromWarehouseName?: string;
  toWarehouseId?: string;
  toWarehouseName?: string;
  quantityIn: number;
  quantityOut: number;
  balanceAfter: number;
  referenceType?: string;
  referenceNumber?: string;
  occurredAt: string;
}

export interface InventoryAdjustment {
  id: string;
  adjustmentNo: string;
  warehouseId: string;
  warehouseName: string;
  adjustmentDate: string;
  adjustmentType: string;
  reason: string;
  isApproved: boolean;
  approvedAt?: string;
  createdAt: string;
  createdBy?: string;
}

export interface StockTransfer {
  id: string;
  transferNo: string;
  fromWarehouseId: string;
  fromWarehouseName: string;
  toWarehouseId: string;
  toWarehouseName: string;
  transferDate: string;
  status: string;
  createdAt: string;
  createdBy?: string;
}

export interface ReorderRule {
  id: string;
  productId: string;
  productName: string;
  variantId?: string;
  warehouseId?: string;
  warehouseName?: string;
  preferredSupplierId?: string;
  preferredSupplierName?: string;
  reorderLevel: number;
  reorderQuantity: number;
  isActive: boolean;
}

export interface StockFilter {
  pageIndex?: number;
  pageSize?: number;
  search?: string;
  warehouseId?: string;
  categoryId?: string;
}

export const inventoryApi = {
  getStockItems: (filter?: StockFilter) =>
    apiClient.get<{ data: { items: StockItem[]; totalCount: number } }>('/stock-items', {
      params: filter,
    }),

  getStockItemById: (id: string) =>
    apiClient.get<{ data: StockItem }>(`/stock-items/${id}`),

  getLowStockItems: () =>
    apiClient.get<{ data: StockItem[] }>('/stock-items/low-stock'),

  updateReorderLevel: (id: string, reorderLevel: number) =>
    apiClient.put<{ data: { id: string; reorderLevel: number } }>(`/stock-items/${id}/reorder-level`, { reorderLevel }),

  getStockMovements: (filter?: {
    pageIndex?: number;
    pageSize?: number;
    search?: string;
    startDate?: string;
    endDate?: string;
    movementTypeCode?: string;
    warehouseId?: string;
  }) =>
    apiClient.get<{ data: { items: StockMovement[]; totalCount: number } }>('/stock-movements', {
      params: filter,
    }),

  getMovementTypes: () =>
    apiClient.get<{ data: { typeCode: string; displayName: string }[] }>('/stock-movements/types'),

  getInventoryAdjustments: (filter?: { pageIndex?: number; pageSize?: number; warehouseId?: string }) =>
    apiClient.get<{ data: { items: InventoryAdjustment[]; totalCount: number } }>('/inventory-adjustments', {
      params: filter,
    }),

  getInventoryAdjustmentById: (id: string) =>
    apiClient.get<{ data: InventoryAdjustment }>(`/inventory-adjustments/${id}`),

  createInventoryAdjustment: (data: {
    warehouseId: string;
    adjustmentType: string;
    reason: string;
    notes?: string;
    lines: { productId: string; variantId?: string; quantityAdjusted: number; reason: string }[];
  }) =>
    apiClient.post<{ data: { id: string; adjustmentNo: string } }>('/inventory-adjustments', data),

  approveInventoryAdjustment: (id: string) =>
    apiClient.post<{ data: { id: string; approvedAt: string } }>(`/inventory-adjustments/${id}/approve`),

  getStockTransfers: (filter?: {
    pageIndex?: number;
    pageSize?: number;
    fromWarehouseId?: string;
    toWarehouseId?: string;
    status?: string;
  }) =>
    apiClient.get<{ data: { items: StockTransfer[]; totalCount: number } }>('/stock-transfers', {
      params: filter,
    }),

  getStockTransferById: (id: string) =>
    apiClient.get<{ data: StockTransfer }>(`/stock-transfers/${id}`),

  createStockTransfer: (data: {
    fromWarehouseId: string;
    toWarehouseId: string;
    notes?: string;
    lines: { productId: string; variantId?: string; quantity: number; unitCost: number }[];
  }) =>
    apiClient.post<{ data: { id: string; transferNo: string } }>('/stock-transfers', data),

  receiveStockTransfer: (id: string) =>
    apiClient.post<{ data: { id: string; status: string } }>(`/stock-transfers/${id}/receive`),

  getReorderRules: (filter?: {
    pageIndex?: number;
    pageSize?: number;
    warehouseId?: string;
    activeOnly?: boolean;
  }) =>
    apiClient.get<{ data: { items: ReorderRule[]; totalCount: number } }>('/reorder-rules', {
      params: filter,
    }),

  getReorderRuleById: (id: string) =>
    apiClient.get<{ data: ReorderRule }>(`/reorder-rules/${id}`),

  createReorderRule: (data: {
    productId: string;
    variantId?: string;
    warehouseId?: string;
    preferredSupplierId?: string;
    reorderLevel: number;
    reorderQuantity: number;
    notifyUserId?: string;
  }) =>
    apiClient.post<{ data: { id: string } }>('/reorder-rules', data),

  updateReorderRule: (id: string, data: {
    warehouseId?: string;
    preferredSupplierId?: string;
    reorderLevel: number;
    reorderQuantity: number;
    notifyUserId?: string;
    isActive: boolean;
  }) =>
    apiClient.put<{ data: { id: string } }>(`/reorder-rules/${id}`, data),

  deleteReorderRule: (id: string) =>
    apiClient.delete<void>(`/reorder-rules/${id}`),

  toggleReorderRuleActive: (id: string) =>
    apiClient.post<{ data: { id: string; isActive: boolean } }>(`/reorder-rules/${id}/toggle-active`),
};