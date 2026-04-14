import apiClient from './client';

// Types
export interface Permission {
  id: string;
  permissionCode: string;
  name: string;
  module: string;
  description?: string;
  isActive: boolean;
}

export interface Menu {
  id: string;
  menuCode: string;
  menuName: string;
  displayName: string;
  menuUrl?: string;
  iconClass?: string;
  displayOrder: number;
  menuLevel: number;
  permissionCode?: string;
  parentMenuId?: string;
  isActive: boolean;
  isVisible: boolean;
  isExternalLink: boolean;
  openInNewTab: boolean;
  description?: string;
}

export interface MenuTreeItem {
  id: string;
  menuCode: string;
  displayName: string;
  menuUrl?: string;
  iconClass?: string;
  displayOrder: number;
  menuLevel: number;
  isActive: boolean;
  isVisible: boolean;
  children: MenuTreeItem[];
}

export interface Role {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface RoleDetail extends Role {
  permissions: RolePermissionItem[];
  menus: RoleMenuItem[];
}

export interface RolePermissionItem {
  permissionId: string;
  permissionCode: string;
  name: string;
  module: string;
  isGranted: boolean;
}

export interface RoleMenuItem {
  menuId: string;
  menuCode: string;
  displayName: string;
  canView: boolean;
  canAdd: boolean;
  canEdit: boolean;
  canDelete: boolean;
  canApprove: boolean;
}

export interface User {
  id: string;
  userName: string;
  email: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  avatarUrl?: string;
  isActive: boolean;
  emailConfirmed: boolean;
  twoFactorEnabled: boolean;
  preferredLanguage: string;
  timeZone: string;
  createdAt: string;
  lastLoginAt?: string;
  roles: string[] | { roleId: string; roleName: string }[];
}

// API functions
export const permissionApi = {
  getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string; module?: string }) =>
    apiClient.get('/permissions', { params }),
  getModules: () => apiClient.get('/permissions/modules'),
  getById: (id: string) => apiClient.get(`/permissions/${id}`),
  create: (data: Partial<Permission>) => apiClient.post('/permissions', data),
  update: (id: string, data: Partial<Permission>) => apiClient.put(`/permissions/${id}`, data),
  delete: (id: string) => apiClient.delete(`/permissions/${id}`),
};

export const menuApi = {
  getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string }) =>
    apiClient.get('/menus', { params }),
  getTree: () => apiClient.get('/menus/tree'),
  getById: (id: string) => apiClient.get(`/menus/${id}`),
  create: (data: Partial<Menu>) => apiClient.post('/menus', data),
  update: (id: string, data: Partial<Menu>) => apiClient.put(`/menus/${id}`, data),
  delete: (id: string) => apiClient.delete(`/menus/${id}`),
};

export const roleApi = {
  getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string }) =>
    apiClient.get('/roles', { params }),
  getById: (id: string) => apiClient.get(`/roles/${id}`),
  create: (data: Partial<Role>) => apiClient.post('/roles', data),
  update: (id: string, data: Partial<Role>) => apiClient.put(`/roles/${id}`, data),
  delete: (id: string) => apiClient.delete(`/roles/${id}`),
  assignPermissions: (roleId: string, permissions: { permissionId: string; isGranted: boolean }[]) =>
    apiClient.put(`/roles/${roleId}/permissions`, { permissions }),
  assignMenus: (roleId: string, menus: { menuId: string; canView: boolean; canAdd: boolean; canEdit: boolean; canDelete: boolean; canApprove: boolean }[]) =>
    apiClient.put(`/roles/${roleId}/menus`, { menus }),
};

export const userApi = {
  getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string; isActive?: boolean; roleId?: string }) =>
    apiClient.get('/users', { params }),
  getById: (id: string) => apiClient.get(`/users/${id}`),
  update: (id: string, data: Partial<User>) => apiClient.put(`/users/${id}`, data),
  toggleActive: (id: string) => apiClient.post(`/users/${id}/toggle-active`),
  assignRoles: (userId: string, roleIds: string[]) => apiClient.put(`/users/${userId}/roles`, { roleIds }),
  getMenus: (userId: string) => apiClient.get(`/users/${userId}/menus`),
  getPermissions: (userId: string) => apiClient.get(`/users/${userId}/permissions`),
};
