import apiClient from './client';

export interface Unit {
  id: string;
  shortName: string;
  name: string;
  description?: string;
  baseUnitId?: string;
  conversionFactor?: number;
  isActive: boolean;
}

export const unitApi = {
  getAll: (filter?: { pageIndex?: number; pageSize?: number; search?: string }) =>
    apiClient.get<{ items: Unit[]; totalCount: number; pageIndex: number; pageSize: number }>('/units', {
      params: filter,
    }),

  getById: (id: string) =>
    apiClient.get<Unit>(`/units/${id}`),

  create: (data: Partial<Unit>) =>
    apiClient.post<Unit>('/units', data),

  update: (id: string, data: Partial<Unit>) =>
    apiClient.put<Unit>(`/units/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/units/${id}`),
};