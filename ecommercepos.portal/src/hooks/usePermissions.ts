import { useState, useEffect } from 'react';

interface MenuPermission {
  menuId: string;
  canView: boolean;
  canAdd: boolean;
  canEdit: boolean;
  canDelete: boolean;
  canApprove: boolean;
}

interface RolePermission {
  roleId: string;
  roleName: string;
  menuPermissions: MenuPermission[];
  permissions: string[];
}

const DEFAULT_PERMISSIONS: RolePermission = {
  roleId: '',
  roleName: 'Admin',
  menuPermissions: [],
  permissions: [],
};

export function usePermissions() {
  const [permissions, setPermissions] = useState<RolePermission>(DEFAULT_PERMISSIONS);
  const [loading] = useState(false);

  useEffect(() => {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        const roleName = user.roles?.[0] || 'Admin';
        setPermissions({
          roleId: user.roleId || '',
          roleName,
          menuPermissions: [],
          permissions: [],
        });
      } catch {
        setPermissions(DEFAULT_PERMISSIONS);
      }
    }
  }, []);

  const hasPermission = (menuId: string, action: 'canView' | 'canAdd' | 'canEdit' | 'canDelete' | 'canApprove'): boolean => {
    const menu = permissions.menuPermissions.find(m => m.menuId === menuId);
    if (menu) return menu[action];
    return permissions.roleName === 'Admin';
  };

  const canAccess = (menuId: string): boolean => {
    return hasPermission(menuId, 'canView');
  };

  return {
    permissions,
    loading,
    hasPermission,
    canAccess,
    isAdmin: permissions.roleName === 'Admin',
    userRole: permissions.roleName,
  };
}

export const MENU_IDS = {
  DASHBOARD: 'dashboard',
  PRODUCTS: 'products',
  CATEGORIES: 'categories',
  BRANDS: 'brands',
  TAGS: 'tags',
  COLLECTIONS: 'collections',
  ORDERS: 'orders',
  INVOICES: 'invoices',
  PAYMENTS: 'payments',
  SHIPMENTS: 'shipments',
  RETURNS: 'returns',
  POS_TERMINAL: 'pos-terminal',
  POS_TRANSACTIONS: 'pos-transactions',
  POS_SHIFTS: 'pos-shifts',
  POS_EXPENSES: 'pos-expenses',
  INVENTORY_STOCK: 'inventory-stock',
  INVENTORY_MOVEMENTS: 'inventory-movements',
  INVENTORY_ADJUSTMENTS: 'inventory-adjustments',
  INVENTORY_TRANSFERS: 'inventory-transfers',
  CUSTOMERS: 'customers',
  CUSTOMER_PROFILES: 'customer-profiles',
  CUSTOMER_ADDRESSES: 'customer-addresses',
  CUSTOMER_LOYALTY: 'customer-loyalty',
  PURCHASE_ORDERS: 'purchase-orders',
  SUPPLIERS: 'suppliers',
  EMPLOYEES: 'employees',
  REPORTS: 'reports',
  SETTINGS: 'settings',
  WAREHOUSES: 'warehouses',
  UNITS: 'units',
  TAX_RATES: 'tax-rates',
  SHIPPING_METHODS: 'shipping-methods',
  PAYMENT_METHODS: 'payment-methods',
};