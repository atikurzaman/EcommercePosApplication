import apiClient from './client';

export interface AttributeType {
  id: string;
  name: string;
  slug: string;
  uiType: string;
  affectsPrice: boolean;
  affectsSku: boolean;
  affectsImage: boolean;
  affectsStock: boolean;
  isFilterable: boolean;
  sortOrder: number;
}

export interface AttributeOption {
  id: string;
  value: string;
  displayValue?: string;
  sortOrder: number;
  isActive: boolean;
}

export interface AttributeTypeWithOptions extends AttributeType {
  options: AttributeOption[];
}

export const attributeApi = {
  getAll: (filter?: { pageIndex?: number; pageSize?: number; search?: string }) =>
    apiClient.get<{ items: AttributeType[]; totalCount: number }>('/attribute-types', {
      params: filter,
    }),

  getWithOptions: () =>
    apiClient.get<{ items: AttributeTypeWithOptions[] }>('/attribute-types/with-options'),

  getById: (id: string) =>
    apiClient.get<{ data: AttributeType }>(`/attribute-types/${id}`),

  create: (data: Partial<AttributeType>) =>
    apiClient.post<AttributeType>('/attribute-types', data),

  update: (id: string, data: Partial<AttributeType>) =>
    apiClient.put<AttributeType>(`/attribute-types/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/attribute-types/${id}`),

  getOptions: (attributeTypeId?: string) =>
    apiClient.get<{ items: AttributeOption[] }>('/attribute-options', {
      params: attributeTypeId ? { attributeTypeId } : {},
    }),

  createOption: (data: { attributeTypeId: string; value: string; displayValue?: string; sortOrder: number }) =>
    apiClient.post<AttributeOption>('/attribute-options', data),

  updateOption: (id: string, data: Partial<AttributeOption>) =>
    apiClient.put<AttributeOption>(`/attribute-options/${id}`, data),

  deleteOption: (id: string) =>
    apiClient.delete(`/attribute-options/${id}`),
};