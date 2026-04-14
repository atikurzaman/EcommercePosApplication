import apiClient from './client';

export interface Employee {
  id: string;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  department: string;
  designation: string;
  branchId: string;
  hireDate: string;
  salary: number;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export const employeeApi = {
  getAll: (filter?: { pageIndex?: number; pageSize?: number; search?: string }) =>
    apiClient.get<{ items: Employee[]; totalCount: number; pageIndex: number; pageSize: number }>('/employees', {
      params: filter,
    }),

  getById: (id: string) =>
    apiClient.get<Employee>(`/employees/${id}`),

  create: (data: Partial<Employee>) =>
    apiClient.post<Employee>('/employees', data),

  update: (id: string, data: Partial<Employee>) =>
    apiClient.put<Employee>(`/employees/${id}`, data),

  delete: (id: string) =>
    apiClient.delete(`/employees/${id}`),
};
