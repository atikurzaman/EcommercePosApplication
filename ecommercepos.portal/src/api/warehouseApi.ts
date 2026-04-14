import apiClient from './client';

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

export const warehouseApi = {
  getAll: (filter?: { pageIndex?: number; pageSize?: number; search?: string }) =>
    apiClient.get<{ data: { items: Warehouse[]; totalCount: number } }>('/warehouses', {
      params: filter,
    }),

  getById: (id: string) =>
    apiClient.get<Warehouse>(`/warehouses/${id}`),

  create: (data: Partial<Warehouse>) =>
    apiClient.post<Warehouse>('/warehouses', data),

  update: (id: string, data: Partial<Warehouse>) =>
    apiClient.put<Warehouse>(`/warehouses/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/warehouses/${id}`),
};
