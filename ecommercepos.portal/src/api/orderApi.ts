import apiClient from './client';

export interface Order {
  id: string;
  orderNumber: string;
  customerId: string;
  customerName: string;
  customerPhone: string;
  warehouseId?: string;
  warehouseName?: string;
  statusCode: string;
  statusName: string;
  orderDate: string;
  totalAmount: number;
  paidAmount: number;
  refundedAmount: number;
}

export interface OrderDetail {
  id: string;
  orderNumber: string;
  customerId: string;
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  warehouseId?: string;
  warehouseName?: string;
  statusCode: string;
  statusName: string;
  orderDate: string;
  orderConfirmedDate?: string;
  shippedDate?: string;
  deliveredDate?: string;
  cancellationDate?: string;
  cancellationReason?: string;
  subtotal: number;
  shippingAmount: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
  paidAmount: number;
  refundedAmount: number;
  customerNote?: string;
  adminNote?: string;
  shippingAddress: {
    id: string;
    address: string;
    city: string;
    phone?: string;
  };
  billingAddress?: {
    id: string;
    address: string;
    city: string;
    phone?: string;
  };
  items: OrderItem[];
  payments: OrderPayment[];
  shipments: OrderShipment[];
}

export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  variantId?: string;
  variantName?: string;
  sku?: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
  taxAmount: number;
  totalPrice: number;
}

export interface OrderPayment {
  id: string;
  paymentMethod: string;
  amount: number;
  transactionId?: string;
  statusCode: string;
  paidAt?: string;
}

export interface OrderShipment {
  id: string;
  trackingNumber?: string;
  statusCode: string;
  shippedDate?: string;
  deliveredDate?: string;
}

export interface OrderFilter {
  pageIndex?: number;
  pageSize?: number;
  search?: string;
  statusCode?: string;
  customerId?: string;
  warehouseId?: string;
  startDate?: string;
  endDate?: string;
}

export interface OrderStats {
  totalOrders: number;
  pendingOrders: number;
  processingOrders: number;
  shippedOrders: number;
  deliveredOrders: number;
  cancelledOrders: number;
  todayOrders: number;
  todaySales: number;
}

export const orderApi = {
  getAll: (filter?: OrderFilter) =>
    apiClient.get<{ data: { items: Order[]; totalCount: number } }>('/orders', {
      params: filter,
    }),

  getById: (id: string) =>
    apiClient.get<{ data: OrderDetail }>(`/orders/${id}`),

  updateStatus: (id: string, statusCode: string) =>
    apiClient.put<{ data: { id: string; statusCode: string } }>(`/orders/${id}/status`, { statusCode }),

  cancel: (id: string, reason: string) =>
    apiClient.post<{ data: { id: string; statusCode: string } }>(`/orders/${id}/cancel`, { reason }),

  getStats: () =>
    apiClient.get<{ data: OrderStats }>('/orders/stats'),
};