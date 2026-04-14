import apiClient from './client';

export interface Cart {
  id: string;
  customerId?: string;
  sessionId?: string;
  subTotal: number;
  discountAmount: number;
  total: number;
  couponCode?: string;
  items: CartItem[];
  createdAt?: string;
}

export interface CartItem {
  id: string;
  productId: string;
  productName: string;
  sku?: string;
  imageUrl?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface CartFilter {
  pageIndex?: number;
  pageSize?: number;
  customerId?: string;
}

export const cartApi = {
  getAll: (filter?: CartFilter) =>
    apiClient.get<{ items: Cart[]; totalCount: number; pageIndex: number; pageSize: number }>('/carts', {
      params: filter,
    }),

  getById: (id: string) =>
    apiClient.get<Cart>(`/carts/${id}`),

  create: (data: { customerId?: string; sessionId?: string }) =>
    apiClient.post<{ id: string }>('/carts', data),

  addItem: (data: { cartId: string; productId: string; quantity: number; unitPrice: number }) =>
    apiClient.post('/carts/items', data),

  updateItem: (itemId: string, quantity: number) =>
    apiClient.put(`/carts/items/${itemId}`, { quantity }),

  removeItem: (itemId: string) =>
    apiClient.delete(`/carts/items/${itemId}`),

  applyCoupon: (cartId: string, couponCode: string) =>
    apiClient.post('/carts/apply-coupon', { cartId, couponCode }),
};
