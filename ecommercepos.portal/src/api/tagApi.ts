import apiClient from './client';

export interface Tag {
  id: string;
  name: string;
  slug: string;
  createdAt: string;
  productCount: number;
}

export const tagApi = {
  getAll: (filter?: { pageIndex?: number; pageSize?: number; search?: string }) =>
    apiClient.get<{ items: Tag[]; totalCount: number }>('/tags', {
      params: filter,
    }),

  getWithCount: () =>
    apiClient.get<{ items: Tag[] }>('/tags/with-count'),

  getById: (id: string) =>
    apiClient.get<{ data: Tag }>(`/tags/${id}`),

  create: (data: { name: string; slug?: string }) =>
    apiClient.post<Tag>('/tags', data),

  update: (id: string, data: { name: string; slug?: string }) =>
    apiClient.put<Tag>(`/tags/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/tags/${id}`),
};