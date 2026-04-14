import apiClient from './client';

export interface Customer {
  id: string;
  customerCode: string;
  customerType: string;
  phone: string;
  email?: string;
  tierName?: string;
  loyaltyPoints: number;
  isActive: boolean;
  registrationDate: string;
  lastPurchaseDate?: string;
}

export interface CustomerDetail extends Customer {
  alternatePhone?: string;
  dateOfBirth?: string;
  gender?: string;
  companyName?: string;
  taxNumber?: string;
  addressLine1?: string;
  city?: string;
  country?: string;
  balance: number;
  creditLimit?: number;
  tier?: {
    tierCode: string;
    displayName: string;
    discountPct: number;
    pointsMultiplier: number;
  };
  addresses: CustomerAddress[];
  recentOrders: CustomerOrder[];
}

export interface CustomerAddress {
  id: string;
  addressType: string;
  label?: string;
  fullName: string;
  phoneNumber: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state?: string;
  postalCode?: string;
  isDefault: boolean;
}

export interface CustomerOrder {
  id: string;
  orderNumber: string;
  status: string;
  totalAmount: number;
  orderDate: string;
}

export interface CustomerStats {
  totalCustomers: number;
  activeCustomers: number;
  newCustomersToday: number;
  totalLoyaltyPoints: number;
}

export interface CustomerFilter {
  pageIndex?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean;
}

export const customerApi = {
  getAll: (filter?: CustomerFilter) =>
    apiClient.get<{ data: { items: Customer[]; totalCount: number } }>('/customers', {
      params: filter,
    }),

  getById: (id: string) =>
    apiClient.get<{ data: CustomerDetail }>(`/customers/${id}`),

  create: (data: {
    phone: string;
    customerType?: string;
    alternatePhone?: string;
    email?: string;
    dateOfBirth?: string;
    gender?: string;
    companyName?: string;
    taxNumber?: string;
    addressLine1?: string;
    city?: string;
    country?: string;
    creditLimit?: number;
  }) =>
    apiClient.post<{ data: { id: string; customerCode: string } }>('/customers', data),

  update: (id: string, data: {
    phone?: string;
    alternatePhone?: string;
    email?: string;
    dateOfBirth?: string;
    gender?: string;
    companyName?: string;
    taxNumber?: string;
    addressLine1?: string;
    city?: string;
    country?: string;
    creditLimit?: number;
    isActive?: boolean;
  }) =>
    apiClient.put<{ data: { id: string } }>(`/customers/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/customers/${id}`),

  toggleActive: (id: string) =>
    apiClient.post<{ data: { id: string; isActive: boolean } }>(`/customers/${id}/toggle-active`),

  getStats: () =>
    apiClient.get<{ data: CustomerStats }>('/customers/stats'),

  getAddresses: (customerId: string) =>
    apiClient.get<{ data: CustomerAddress[] }>(`/customers/addresses/${customerId}`),
};