import apiClient from './client';

export interface Supplier {
  id: string;
  supplierCode: string;
  supplierName: string;
  contactPerson: string;
  email: string;
  phone: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  country: string;
  taxId: string;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export const supplierApi = {
  getAll: (filter?: { pageIndex?: number; pageSize?: number; search?: string }) =>
    apiClient.get<{ items: Supplier[]; totalCount: number; pageIndex: number; pageSize: number }>('/suppliers', {
      params: filter,
    }),

  getById: (id: string) =>
    apiClient.get<Supplier>(`/suppliers/${id}`),

  create: (data: Partial<Supplier>) =>
    apiClient.post<Supplier>('/suppliers', data),

  update: (id: string, data: Partial<Supplier>) =>
    apiClient.put<Supplier>(`/suppliers/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/suppliers/${id}`),
};
