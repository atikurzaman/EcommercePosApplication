import apiClient from './client';

export interface LookupItem {
  [key: string]: any;
}

export interface LookupConfig {
  endpoint: string;
  codeField: string;
}

export interface PaginationInfo {
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiListResponse<T> {
  success: boolean;
  data: T[];
  pagination?: PaginationInfo;
}

export interface ApiItemResponse<T> {
  success: boolean;
  data: T;
}

export function createLookupApi<T extends LookupItem>(config: LookupConfig) {
  return {
    getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string }) =>
      apiClient.get<ApiListResponse<T>>(config.endpoint, { params }),

    getByCode: (code: string) =>
      apiClient.get<ApiItemResponse<T>>(`${config.endpoint}/${code}`),

    create: (data: Partial<T>) =>
      apiClient.post<ApiItemResponse<T>>(config.endpoint, data),

    update: (code: string, data: Partial<T>) =>
      apiClient.put<ApiItemResponse<T>>(`${config.endpoint}/${code}`, data),

    delete: (code: string) =>
      apiClient.delete(`${config.endpoint}/${code}`),
  };
}
