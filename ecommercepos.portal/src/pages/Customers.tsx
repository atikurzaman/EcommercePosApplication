import { useState, useCallback } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Eye, Edit, Trash2,
  ChevronLeft, ChevronRight, Loader2, X,
  Phone, Mail, MapPin, Users, Star,
  Calendar, CreditCard, ShoppingBag,
  TrendingUp, UserCheck, UserPlus, Award,
  Download, Filter,
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import {
  customerApi,
  type Customer,
  type CustomerDetail,
} from '@/api/customerApi';

/* ─── helpers ──────────────────────────────────────────────────────────────── */
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-BD', {
    style: 'currency',
    currency: 'BDT',
    minimumFractionDigits: 0,
  }).format(amount);
}

function formatDate(dateStr?: string | null): string {
  if (!dateStr) return '—';
  return new Date(dateStr).toLocaleDateString('en-GB', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  });
}

function getInitials(phone: string, email?: string): string {
  if (email) {
    const name = email.split('@')[0];
    const parts = name.split(/[._-]/);
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase();
    }
    return name.slice(0, 2).toUpperCase();
  }
  return phone.slice(-2);
}

function getAvatarColor(id: string): string {
  const colors = [
    'bg-blue-500',   'bg-purple-500', 'bg-green-500',
    'bg-orange-500', 'bg-teal-500',   'bg-pink-500',
    'bg-indigo-500', 'bg-red-500',    'bg-yellow-500',
    'bg-cyan-500',
  ];
  const hash = id.split('').reduce((acc, c) => acc + c.charCodeAt(0), 0);
  return colors[hash % colors.length];
}

function getTierBadge(tierName?: string) {
  if (!tierName) return null;
  const tier = tierName.toLowerCase();
  if (tier.includes('gold') || tier.includes('premium'))
    return { cls: 'nx-badge bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400', icon: '★' };
  if (tier.includes('silver'))
    return { cls: 'nx-badge bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300', icon: '◆' };
  if (tier.includes('platinum') || tier.includes('vip'))
    return { cls: 'nx-badge bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400', icon: '♦' };
  return { cls: 'nx-badge nx-badge-info', icon: '' };
}

const PAGE_SIZE = 15;

/* ─── form ─────────────────────────────────────────────────────────────────── */
interface CustomerFormData {
  phone: string;
  customerType: string;
  email: string;
  alternatePhone: string;
  dateOfBirth: string;
  gender: string;
  companyName: string;
  addressLine1: string;
  city: string;
  country: string;
  creditLimit: number;
  isActive: boolean;
}

const emptyForm: CustomerFormData = {
  phone:        '',
  customerType: 'RETAIL',
  email:        '',
  alternatePhone: '',
  dateOfBirth:  '',
  gender:       '',
  companyName:  '',
  addressLine1: '',
  city:         'Dhaka',
  country:      'Bangladesh',
  creditLimit:  0,
  isActive:     true,
};

/* ─── skeleton ─────────────────────────────────────────────────────────────── */
function StatSkeleton() {
  return (
    <div className="nx-stat-card animate-pulse">
      <div className="flex items-start justify-between">
        <div className="space-y-2">
          <div className="h-3 w-28 bg-secondary rounded" />
          <div className="h-8 w-20 bg-secondary rounded" />
          <div className="h-3 w-24 bg-secondary rounded" />
        </div>
        <div className="w-11 h-11 bg-secondary rounded-xl" />
      </div>
    </div>
  );
}

function TableSkeleton() {
  return (
    <div className="divide-y divide-border">
      {Array.from({ length: 8 }, (_, i) => (
        <div key={i} className="flex items-center gap-4 px-4 py-3 animate-pulse">
          <div className="w-10 h-10 bg-secondary rounded-full flex-shrink-0" />
          <div className="flex-1 space-y-1.5">
            <div className="h-3.5 bg-secondary rounded w-32" />
            <div className="h-3 bg-secondary rounded w-24" />
          </div>
          <div className="h-3 bg-secondary rounded w-24 hidden sm:block" />
          <div className="h-5 bg-secondary rounded-full w-16 hidden md:block" />
          <div className="h-3 bg-secondary rounded w-20 hidden lg:block" />
          <div className="h-5 bg-secondary rounded-full w-14" />
          <div className="h-7 bg-secondary rounded w-20" />
        </div>
      ))}
    </div>
  );
}

/* ─── order status badge ──────────────────────────────────────────────────── */
function OrderStatusBadge({ status }: { status: string }) {
  const cls =
    status === 'Completed' ? 'nx-badge nx-badge-success' :
    status === 'Cancelled' ? 'nx-badge nx-badge-danger' :
    status === 'Processing' ? 'nx-badge nx-badge-info' :
    'nx-badge nx-badge-warning';
  return <span className={cls}>{status}</span>;
}

/* ─── customer avatar ────────────────────────────────────────────────────── */
function CustomerAvatar({ customer, size = 'md' }: { customer: Customer; size?: 'sm' | 'md' | 'lg' }) {
  const initials = getInitials(customer.phone, customer.email);
  const colorClass = getAvatarColor(customer.id);
  const sizeClass = size === 'sm' ? 'w-8 h-8 text-xs' : size === 'lg' ? 'w-14 h-14 text-xl' : 'w-10 h-10 text-sm';
  return (
    <div className={`${sizeClass} ${colorClass} rounded-full flex items-center justify-center text-white font-semibold flex-shrink-0`}>
      {initials}
    </div>
  );
}

/* ─── profile drawer ─────────────────────────────────────────────────────── */
interface ProfileDrawerProps {
  customerId: string | null;
  onClose: () => void;
  onEdit: (customer: Customer) => void;
}

function ProfileDrawer({ customerId, onClose, onEdit }: ProfileDrawerProps) {
  const { data, isLoading } = useQuery({
    queryKey: ['customer-detail', customerId],
    queryFn: () => customerApi.getById(customerId!),
    enabled: !!customerId,
    staleTime: 60_000,
  });

  const detail: CustomerDetail | null = (data?.data as any)?.data ?? null;

  if (!customerId) return null;

  return (
    <>
      {/* Backdrop */}
      <div
        className="fixed inset-0 bg-black/40 backdrop-blur-sm z-40"
        onClick={onClose}
      />

      {/* Drawer */}
      <div className="fixed right-0 top-0 h-full w-full max-w-md bg-background shadow-2xl z-50 flex flex-col animate-in slide-in-from-right duration-300">
        {/* Drawer header */}
        <div className="flex items-center justify-between px-5 py-4 border-b">
          <h2 className="font-semibold text-base">Customer Profile</h2>
          <Button variant="ghost" size="icon" onClick={onClose}>
            <X className="w-4 h-4" />
          </Button>
        </div>

        <div className="flex-1 overflow-y-auto">
          {isLoading ? (
            <div className="flex items-center justify-center h-40">
              <Loader2 className="w-6 h-6 animate-spin text-muted-foreground" />
            </div>
          ) : !detail ? (
            <div className="flex flex-col items-center justify-center h-40 text-muted-foreground">
              <p className="text-sm">Could not load customer details</p>
            </div>
          ) : (
            <>
              {/* Profile header */}
              <div className="px-5 py-5 border-b bg-secondary/30">
                <div className="flex items-start gap-4">
                  <CustomerAvatar customer={detail} size="lg" />
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 flex-wrap">
                      <h3 className="font-semibold text-base">
                        {detail.companyName || detail.phone}
                      </h3>
                      {detail.tierName && (() => {
                        const badge = getTierBadge(detail.tierName);
                        return badge ? (
                          <span className={badge.cls}>
                            {badge.icon && <span className="mr-1">{badge.icon}</span>}
                            {detail.tierName}
                          </span>
                        ) : null;
                      })()}
                    </div>
                    <p className="text-sm text-muted-foreground mt-0.5">
                      {detail.customerCode}
                    </p>
                    <div className="flex items-center gap-1 mt-1">
                      <span className={`nx-badge ${detail.isActive ? 'nx-badge-success' : 'nx-badge-danger'}`}>
                        {detail.isActive ? 'Active' : 'Inactive'}
                      </span>
                      <span className="nx-badge nx-badge-neutral">{detail.customerType}</span>
                    </div>
                  </div>
                </div>

                {/* Mini stats */}
                <div className="grid grid-cols-3 gap-3 mt-4">
                  <div className="text-center">
                    <p className="text-lg font-bold tabular-nums">{detail.loyaltyPoints.toLocaleString()}</p>
                    <p className="text-xs text-muted-foreground">Points</p>
                  </div>
                  <div className="text-center">
                    <p className="text-lg font-bold tabular-nums">{detail.recentOrders?.length ?? 0}</p>
                    <p className="text-xs text-muted-foreground">Orders</p>
                  </div>
                  <div className="text-center">
                    <p className="text-lg font-bold tabular-nums">
                      {formatCurrency(detail.balance ?? 0)}
                    </p>
                    <p className="text-xs text-muted-foreground">Balance</p>
                  </div>
                </div>
              </div>

              {/* Contact info */}
              <div className="px-5 py-4 border-b">
                <p className="text-xs font-semibold text-muted-foreground uppercase tracking-wide mb-3">
                  Contact Information
                </p>
                <div className="space-y-2.5">
                  <div className="flex items-center gap-3 text-sm">
                    <Phone className="w-4 h-4 text-muted-foreground flex-shrink-0" />
                    <span>{detail.phone}</span>
                  </div>
                  {detail.alternatePhone && (
                    <div className="flex items-center gap-3 text-sm">
                      <Phone className="w-4 h-4 text-muted-foreground flex-shrink-0" />
                      <span className="text-muted-foreground">{detail.alternatePhone}</span>
                    </div>
                  )}
                  {detail.email && (
                    <div className="flex items-center gap-3 text-sm">
                      <Mail className="w-4 h-4 text-muted-foreground flex-shrink-0" />
                      <span className="truncate">{detail.email}</span>
                    </div>
                  )}
                  {detail.addressLine1 && (
                    <div className="flex items-start gap-3 text-sm">
                      <MapPin className="w-4 h-4 text-muted-foreground flex-shrink-0 mt-0.5" />
                      <span className="text-muted-foreground">
                        {[detail.addressLine1, detail.city, detail.country]
                          .filter(Boolean)
                          .join(', ')}
                      </span>
                    </div>
                  )}
                </div>
              </div>

              {/* Account info */}
              <div className="px-5 py-4 border-b">
                <p className="text-xs font-semibold text-muted-foreground uppercase tracking-wide mb-3">
                  Account Details
                </p>
                <div className="grid grid-cols-2 gap-3 text-sm">
                  <div>
                    <p className="text-muted-foreground text-xs mb-0.5">Registered</p>
                    <p className="font-medium">{formatDate(detail.registrationDate)}</p>
                  </div>
                  <div>
                    <p className="text-muted-foreground text-xs mb-0.5">Last Purchase</p>
                    <p className="font-medium">{formatDate(detail.lastPurchaseDate)}</p>
                  </div>
                  {detail.creditLimit != null && detail.creditLimit > 0 && (
                    <div>
                      <p className="text-muted-foreground text-xs mb-0.5">Credit Limit</p>
                      <p className="font-medium">{formatCurrency(detail.creditLimit)}</p>
                    </div>
                  )}
                  {detail.tier && (
                    <div>
                      <p className="text-muted-foreground text-xs mb-0.5">Points Multiplier</p>
                      <p className="font-medium">{detail.tier.pointsMultiplier}×</p>
                    </div>
                  )}
                </div>
              </div>

              {/* Recent orders */}
              <div className="px-5 py-4">
                <p className="text-xs font-semibold text-muted-foreground uppercase tracking-wide mb-3">
                  Recent Orders
                </p>
                {(detail.recentOrders?.length ?? 0) === 0 ? (
                  <div className="flex flex-col items-center justify-center py-6 text-muted-foreground">
                    <ShoppingBag className="w-8 h-8 mb-2 opacity-30" />
                    <p className="text-sm">No orders yet</p>
                  </div>
                ) : (
                  <div className="space-y-2">
                    {detail.recentOrders.map(order => (
                      <div
                        key={order.id}
                        className="flex items-center justify-between p-3 rounded-xl border hover:bg-secondary/40 transition-colors"
                      >
                        <div className="min-w-0">
                          <p className="text-sm font-mono font-medium truncate">{order.orderNumber}</p>
                          <p className="text-xs text-muted-foreground">{formatDate(order.orderDate)}</p>
                        </div>
                        <div className="flex flex-col items-end gap-1 flex-shrink-0 ml-3">
                          <span className="text-sm font-semibold tabular-nums">
                            {formatCurrency(order.totalAmount)}
                          </span>
                          <OrderStatusBadge status={order.status} />
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </>
          )}
        </div>

        {/* Drawer footer */}
        {detail && (
          <div className="px-5 py-4 border-t bg-secondary/20 flex gap-3">
            <Button
              variant="outline"
              size="sm"
              className="flex-1"
              onClick={() => { onEdit(detail); onClose(); }}
            >
              <Edit className="w-4 h-4 mr-2" />
              Edit Customer
            </Button>
          </div>
        )}
      </div>
    </>
  );
}

/* ─── main component ───────────────────────────────────────────────────────── */
export default function Customers() {
  const queryClient = useQueryClient();

  // ── filter state
  const [searchInput, setSearchInput]   = useState('');
  const [searchQuery, setSearchQuery]   = useState('');
  const [currentPage, setCurrentPage]   = useState(1);
  const [statusFilter, setStatusFilter] = useState('');

  // ── modal / drawer state
  const [showModal, setShowModal]               = useState(false);
  const [editingCustomer, setEditingCustomer]   = useState<Customer | null>(null);
  const [formData, setFormData]                 = useState<CustomerFormData>(emptyForm);
  const [deleteModal, setDeleteModal]           = useState<Customer | null>(null);
  const [drawerCustomerId, setDrawerCustomerId] = useState<string | null>(null);

  /* ── queries ──────────────────────────────────────────────────────────── */
  const { data: statsData, isLoading: statsLoading } = useQuery({
    queryKey: ['customer-stats'],
    queryFn: () => customerApi.getStats(),
    staleTime: 300_000,
  });

  const { data: listData, isLoading: listLoading } = useQuery({
    queryKey: ['customers', currentPage, statusFilter, searchQuery],
    queryFn: () => customerApi.getAll({
      pageIndex: currentPage - 1,
      pageSize: PAGE_SIZE,
      search: searchQuery || undefined,
      isActive: statusFilter !== '' ? statusFilter === 'true' : undefined,
    }),
  });

  /* ── mutations ──────────────────────────────────────────────────────── */
  const createMutation = useMutation({
    mutationFn: customerApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      queryClient.invalidateQueries({ queryKey: ['customer-stats'] });
      setShowModal(false);
      toast.success('Customer created');
    },
    onError: () => toast.error('Failed to create customer'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Parameters<typeof customerApi.update>[1] }) =>
      customerApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      queryClient.invalidateQueries({ queryKey: ['customer-stats'] });
      setShowModal(false);
      toast.success('Customer updated');
    },
    onError: () => toast.error('Failed to update customer'),
  });

  const deleteMutation = useMutation({
    mutationFn: customerApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      queryClient.invalidateQueries({ queryKey: ['customer-stats'] });
      setDeleteModal(null);
      toast.success('Customer deleted');
    },
    onError: () => toast.error('Failed to delete customer'),
  });

  const toggleMutation = useMutation({
    mutationFn: customerApi.toggleActive,
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      queryClient.invalidateQueries({ queryKey: ['customer-detail', id] });
    },
    onError: () => toast.error('Failed to update status'),
  });

  /* ── derived ──────────────────────────────────────────────────────────── */
  const stats      = (statsData?.data as any)?.data;
  const customers: Customer[] = (listData?.data as any)?.data?.items ?? (listData?.data as any)?.items ?? [];
  const totalCount = (listData?.data as any)?.data?.totalCount ?? (listData?.data as any)?.totalCount ?? 0;
  const totalPages = Math.ceil(totalCount / PAGE_SIZE);

  /* ── handlers ────────────────────────────────────────────────────────── */
  const applySearch = useCallback(() => {
    setSearchQuery(searchInput);
    setCurrentPage(1);
  }, [searchInput]);

  const openCreateModal = () => {
    setEditingCustomer(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (customer: Customer) => {
    setEditingCustomer(customer);
    setFormData({
      phone:        customer.phone ?? '',
      customerType: customer.customerType ?? 'RETAIL',
      email:        customer.email ?? '',
      alternatePhone: '',
      dateOfBirth:  '',
      gender:       '',
      companyName:  '',
      addressLine1: '',
      city:         'Dhaka',
      country:      'Bangladesh',
      creditLimit:  0,
      isActive:     customer.isActive ?? true,
    });
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const payload = {
      phone:        formData.phone,
      customerType: formData.customerType,
      email:        formData.email || undefined,
      alternatePhone: formData.alternatePhone || undefined,
      dateOfBirth:  formData.dateOfBirth || undefined,
      gender:       formData.gender || undefined,
      companyName:  formData.companyName || undefined,
      addressLine1: formData.addressLine1 || undefined,
      city:         formData.city || undefined,
      country:      formData.country || undefined,
      creditLimit:  formData.creditLimit || undefined,
    };
    if (editingCustomer) {
      updateMutation.mutate({ id: editingCustomer.id, data: { ...payload, isActive: formData.isActive } });
    } else {
      createMutation.mutate(payload);
    }
  };

  /* ── render ─────────────────────────────────────────────────────────────── */
  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Customers</h1>
          <p className="nx-page-subtitle">Manage your customer database and loyalty program</p>
        </div>
        <div className="nx-page-actions">
          <Button variant="outline" size="sm">
            <Download className="w-4 h-4 mr-2" />
            Export
          </Button>
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Customer
          </Button>
        </div>
      </div>

      {/* Stats Row */}
      <div className="nx-stats-grid">
        {statsLoading ? (
          Array.from({ length: 4 }, (_, i) => <StatSkeleton key={i} />)
        ) : (
          <>
            <div className="nx-stat-card">
              <div className="flex items-start justify-between">
                <div>
                  <p className="text-sm font-medium text-muted-foreground mb-1">Total Customers</p>
                  <p className="text-2xl font-bold tabular-nums">
                    {(stats?.totalCustomers ?? 0).toLocaleString()}
                  </p>
                  <p className="text-xs text-muted-foreground mt-1.5">all time</p>
                </div>
                <div className="w-11 h-11 rounded-xl bg-blue-100 dark:bg-blue-900/30 flex items-center justify-center flex-shrink-0">
                  <Users className="w-5 h-5 text-blue-600" />
                </div>
              </div>
            </div>
            <div className="nx-stat-card">
              <div className="flex items-start justify-between">
                <div>
                  <p className="text-sm font-medium text-muted-foreground mb-1">Active</p>
                  <p className="text-2xl font-bold text-green-600 tabular-nums">
                    {(stats?.activeCustomers ?? 0).toLocaleString()}
                  </p>
                  <p className="text-xs text-muted-foreground mt-1.5">
                    {stats?.totalCustomers
                      ? `${Math.round((stats.activeCustomers / stats.totalCustomers) * 100)}% of total`
                      : '—'}
                  </p>
                </div>
                <div className="w-11 h-11 rounded-xl bg-green-100 dark:bg-green-900/30 flex items-center justify-center flex-shrink-0">
                  <UserCheck className="w-5 h-5 text-green-600" />
                </div>
              </div>
            </div>
            <div className="nx-stat-card">
              <div className="flex items-start justify-between">
                <div>
                  <p className="text-sm font-medium text-muted-foreground mb-1">New Today</p>
                  <p className="text-2xl font-bold text-purple-600 tabular-nums">
                    {stats?.newCustomersToday ?? 0}
                  </p>
                  <p className="text-xs text-muted-foreground mt-1.5">registered today</p>
                </div>
                <div className="w-11 h-11 rounded-xl bg-purple-100 dark:bg-purple-900/30 flex items-center justify-center flex-shrink-0">
                  <UserPlus className="w-5 h-5 text-purple-600" />
                </div>
              </div>
            </div>
            <div className="nx-stat-card">
              <div className="flex items-start justify-between">
                <div>
                  <p className="text-sm font-medium text-muted-foreground mb-1">Loyalty Points</p>
                  <p className="text-2xl font-bold text-yellow-600 tabular-nums">
                    {(stats?.totalLoyaltyPoints ?? 0).toLocaleString()}
                  </p>
                  <p className="text-xs text-muted-foreground mt-1.5">total issued</p>
                </div>
                <div className="w-11 h-11 rounded-xl bg-yellow-100 dark:bg-yellow-900/30 flex items-center justify-center flex-shrink-0">
                  <Award className="w-5 h-5 text-yellow-600" />
                </div>
              </div>
            </div>
          </>
        )}
      </div>

      {/* Main Table Card */}
      <Card>
        {/* Toolbar */}
        <div className="p-4 border-b">
          <div className="flex flex-col sm:flex-row gap-3">
            <div className="nx-table-search flex-1 min-w-0">
              <Search className="w-4 h-4 flex-shrink-0" />
              <input
                type="text"
                placeholder="Search by phone, email, code..."
                value={searchInput}
                onChange={(e) => setSearchInput(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && applySearch()}
                className="bg-transparent border-none outline-none text-sm w-full"
              />
            </div>
            <div className="flex items-center gap-2 flex-shrink-0">
              <select
                className="nx-input nx-select text-sm h-9 pl-3 pr-7"
                value={statusFilter}
                onChange={(e) => { setStatusFilter(e.target.value); setCurrentPage(1); }}
              >
                <option value="">All Status</option>
                <option value="true">Active</option>
                <option value="false">Inactive</option>
              </select>
              <Button variant="outline" size="sm" onClick={applySearch}>
                <Filter className="w-4 h-4 mr-1.5" />
                Search
              </Button>
            </div>
          </div>
        </div>

        {/* Table */}
        {listLoading ? (
          <TableSkeleton />
        ) : customers.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-20 text-muted-foreground">
            <div className="w-16 h-16 rounded-2xl bg-secondary flex items-center justify-center mb-4">
              <Users className="w-8 h-8 opacity-40" />
            </div>
            <h3 className="text-base font-semibold text-foreground mb-1">No customers found</h3>
            <p className="text-sm mb-6 text-center max-w-xs">
              Try adjusting your search or add a new customer.
            </p>
            <Button size="sm" onClick={openCreateModal}>
              <Plus className="w-4 h-4 mr-2" />
              Add Customer
            </Button>
          </div>
        ) : (
          <>
            <div className="nx-table-wrap overflow-x-auto">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Customer</th>
                    <th>Phone</th>
                    <th>Tier</th>
                    <th className="text-right">Loyalty Pts</th>
                    <th>Registered</th>
                    <th>Last Purchase</th>
                    <th>Status</th>
                    <th className="w-28">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {customers.map((customer) => {
                    const tierBadge = getTierBadge(customer.tierName);
                    return (
                      <tr key={customer.id}>
                        {/* Avatar + identity */}
                        <td>
                          <div className="flex items-center gap-3">
                            <CustomerAvatar customer={customer} />
                            <div className="min-w-0">
                              <p className="font-medium text-sm truncate max-w-[140px]">
                                {customer.email
                                  ? customer.email.split('@')[0]
                                  : customer.phone}
                              </p>
                              <code className="text-xs text-muted-foreground font-mono">
                                {customer.customerCode}
                              </code>
                            </div>
                          </div>
                        </td>

                        {/* Phone */}
                        <td>
                          <div className="flex items-center gap-1.5 text-sm">
                            <Phone className="w-3.5 h-3.5 text-muted-foreground" />
                            <span className="tabular-nums">{customer.phone}</span>
                          </div>
                          {customer.email && (
                            <div className="flex items-center gap-1.5 text-xs text-muted-foreground mt-0.5">
                              <Mail className="w-3 h-3" />
                              <span className="truncate max-w-[120px]">{customer.email}</span>
                            </div>
                          )}
                        </td>

                        {/* Tier */}
                        <td>
                          {tierBadge ? (
                            <span className={tierBadge.cls}>
                              {tierBadge.icon && <span className="mr-1">{tierBadge.icon}</span>}
                              {customer.tierName}
                            </span>
                          ) : (
                            <span className="text-sm text-muted-foreground">—</span>
                          )}
                        </td>

                        {/* Loyalty points */}
                        <td className="text-right">
                          <div className="flex items-center justify-end gap-1.5">
                            <Star className="w-3.5 h-3.5 text-yellow-500" />
                            <span className="font-medium text-sm tabular-nums">
                              {customer.loyaltyPoints.toLocaleString()}
                            </span>
                          </div>
                        </td>

                        {/* Dates */}
                        <td>
                          <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                            <Calendar className="w-3.5 h-3.5" />
                            <span>{formatDate(customer.registrationDate)}</span>
                          </div>
                        </td>
                        <td>
                          <span className="text-sm text-muted-foreground">
                            {formatDate(customer.lastPurchaseDate)}
                          </span>
                        </td>

                        {/* Status */}
                        <td>
                          <button
                            onClick={() => toggleMutation.mutate(customer.id)}
                            disabled={toggleMutation.isPending}
                            className={`nx-badge cursor-pointer hover:opacity-80 transition-opacity ${
                              customer.isActive ? 'nx-badge-success' : 'nx-badge-danger'
                            }`}
                          >
                            {customer.isActive ? 'Active' : 'Inactive'}
                          </button>
                        </td>

                        {/* Actions */}
                        <td>
                          <div className="flex items-center gap-1">
                            <Button
                              variant="ghost"
                              size="icon"
                              className="w-8 h-8 text-muted-foreground hover:text-foreground"
                              onClick={() => setDrawerCustomerId(customer.id)}
                              title="View profile"
                            >
                              <Eye className="w-3.5 h-3.5" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="w-8 h-8 text-muted-foreground hover:text-foreground"
                              onClick={() => openEditModal(customer)}
                              title="Edit"
                            >
                              <Edit className="w-3.5 h-3.5" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="w-8 h-8 text-red-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20"
                              onClick={() => setDeleteModal(customer)}
                              title="Delete"
                            >
                              <Trash2 className="w-3.5 h-3.5" />
                            </Button>
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            <div className="flex items-center justify-between px-4 py-3 border-t">
              <p className="text-sm text-muted-foreground">
                Showing{' '}
                <span className="font-medium">{(currentPage - 1) * PAGE_SIZE + 1}</span>–
                <span className="font-medium">{Math.min(currentPage * PAGE_SIZE, totalCount)}</span>{' '}
                of <span className="font-medium">{totalCount}</span> customers
              </p>
              <div className="flex items-center gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={currentPage === 1}
                  onClick={() => setCurrentPage(p => p - 1)}
                >
                  <ChevronLeft className="w-4 h-4" />
                </Button>
                <span className="text-sm font-medium tabular-nums px-1">
                  {currentPage} / {totalPages || 1}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={currentPage >= totalPages}
                  onClick={() => setCurrentPage(p => p + 1)}
                >
                  <ChevronRight className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </>
        )}
      </Card>

      {/* ── Create / Edit Modal ──────────────────────────────────────────── */}
      {showModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl w-full max-w-lg max-h-[92vh] overflow-hidden flex flex-col shadow-2xl">
            {/* Header */}
            <div className="flex items-center justify-between px-6 py-4 border-b">
              <div>
                <h2 className="text-lg font-semibold">
                  {editingCustomer ? 'Edit Customer' : 'Add New Customer'}
                </h2>
                <p className="text-xs text-muted-foreground mt-0.5">
                  {editingCustomer
                    ? `Editing: ${editingCustomer.customerCode}`
                    : 'Fill in the customer details below'}
                </p>
              </div>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>

            {/* Form */}
            <form id="customer-form" onSubmit={handleSubmit} className="flex-1 overflow-y-auto p-6">
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium mb-1.5">
                      Phone <span className="text-red-500">*</span>
                    </label>
                    <Input
                      value={formData.phone}
                      onChange={(e) => setFormData(f => ({ ...f, phone: e.target.value }))}
                      placeholder="+880 1XXXXXXXXX"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1.5">Customer Type</label>
                    <select
                      className="nx-input nx-select w-full"
                      value={formData.customerType}
                      onChange={(e) => setFormData(f => ({ ...f, customerType: e.target.value }))}
                    >
                      <option value="RETAIL">Retail</option>
                      <option value="WHOLESALE">Wholesale</option>
                      <option value="ONLINE">Online</option>
                    </select>
                  </div>
                  <div className="col-span-2">
                    <label className="block text-sm font-medium mb-1.5">Email</label>
                    <Input
                      type="email"
                      value={formData.email}
                      onChange={(e) => setFormData(f => ({ ...f, email: e.target.value }))}
                      placeholder="customer@example.com"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1.5">Alternate Phone</label>
                    <Input
                      value={formData.alternatePhone}
                      onChange={(e) => setFormData(f => ({ ...f, alternatePhone: e.target.value }))}
                      placeholder="Optional"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1.5">Gender</label>
                    <select
                      className="nx-input nx-select w-full"
                      value={formData.gender}
                      onChange={(e) => setFormData(f => ({ ...f, gender: e.target.value }))}
                    >
                      <option value="">Not specified</option>
                      <option value="Male">Male</option>
                      <option value="Female">Female</option>
                      <option value="Other">Other</option>
                    </select>
                  </div>
                  <div className="col-span-2">
                    <label className="block text-sm font-medium mb-1.5">Company Name</label>
                    <Input
                      value={formData.companyName}
                      onChange={(e) => setFormData(f => ({ ...f, companyName: e.target.value }))}
                      placeholder="For business / wholesale customers"
                    />
                  </div>
                  <div className="col-span-2">
                    <label className="block text-sm font-medium mb-1.5">Address</label>
                    <Input
                      value={formData.addressLine1}
                      onChange={(e) => setFormData(f => ({ ...f, addressLine1: e.target.value }))}
                      placeholder="Street address"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1.5">City</label>
                    <Input
                      value={formData.city}
                      onChange={(e) => setFormData(f => ({ ...f, city: e.target.value }))}
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1.5">Credit Limit (৳)</label>
                    <Input
                      type="number"
                      min={0}
                      value={formData.creditLimit}
                      onChange={(e) => setFormData(f => ({ ...f, creditLimit: parseFloat(e.target.value) || 0 }))}
                    />
                  </div>
                  {editingCustomer && (
                    <div className="col-span-2">
                      <label className="flex items-center gap-2 cursor-pointer">
                        <input
                          type="checkbox"
                          checked={formData.isActive}
                          onChange={(e) => setFormData(f => ({ ...f, isActive: e.target.checked }))}
                          className="nx-checkbox"
                        />
                        <span className="text-sm font-medium">Active</span>
                      </label>
                    </div>
                  )}
                </div>
              </div>
            </form>

            {/* Footer */}
            <div className="flex items-center justify-end gap-3 px-6 py-4 border-t bg-secondary/20">
              <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
              <Button
                type="submit"
                form="customer-form"
                disabled={createMutation.isPending || updateMutation.isPending}
              >
                {(createMutation.isPending || updateMutation.isPending) && (
                  <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                )}
                {editingCustomer ? 'Update Customer' : 'Create Customer'}
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* ── Delete Confirmation ──────────────────────────────────────────── */}
      {deleteModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl w-full max-w-md p-6 shadow-2xl">
            <div className="flex items-start gap-4 mb-5">
              <div className="w-10 h-10 rounded-full bg-red-100 dark:bg-red-900/30 flex items-center justify-center flex-shrink-0">
                <Trash2 className="w-5 h-5 text-red-600" />
              </div>
              <div>
                <h2 className="text-base font-semibold mb-1">Delete Customer</h2>
                <p className="text-sm text-muted-foreground">
                  Are you sure you want to delete customer{' '}
                  <span className="font-medium text-foreground">{deleteModal.customerCode}</span>?
                  Their order history will be preserved. This action cannot be undone.
                </p>
              </div>
            </div>
            <div className="flex justify-end gap-3">
              <Button variant="outline" onClick={() => setDeleteModal(null)}>Cancel</Button>
              <Button
                variant="destructive"
                onClick={() => deleteMutation.mutate(deleteModal.id)}
                disabled={deleteMutation.isPending}
              >
                {deleteMutation.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                Delete Customer
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* ── Profile Drawer ───────────────────────────────────────────────── */}
      <ProfileDrawer
        customerId={drawerCustomerId}
        onClose={() => setDrawerCustomerId(null)}
        onEdit={(customer) => openEditModal(customer)}
      />
    </div>
  );
}
