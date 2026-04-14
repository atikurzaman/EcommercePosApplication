import { useState, useEffect, useCallback } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Edit, Trash2, ChevronLeft, ChevronRight, Loader2, X,
  Warehouse as WarehouseIcon, ToggleLeft, ToggleRight, MapPin, Phone,
  Mail, User2, Tag, ChevronRight as ChevRight, Star, Activity,
  Monitor, ArrowLeft,
} from 'lucide-react';
import {
  warehouseApiV2, posCounterApi, posTerminalApi,
  type Warehouse, type PosCounter, type PosTerminal, type WarehouseStats,
} from '@/api/posApi';
import toast from 'react-hot-toast';

// ── Constants ─────────────────────────────────────────────────────────────

const SITE_TYPES = ['Warehouse', 'Store', 'Outlet', 'Distribution Center'];

const SITE_TYPE_BADGE: Record<string, string> = {
  'Warehouse':            'nx-badge nx-badge-info',
  'Store':                'nx-badge bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
  'Outlet':               'nx-badge nx-badge-success',
  'Distribution Center':  'nx-badge nx-badge-warning',
};

// ── Types ─────────────────────────────────────────────────────────────────

interface WarehouseForm {
  code: string;
  name: string;
  siteType: string;
  contactPerson: string;
  managerName: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  area: string;
  phone: string;
  email: string;
  isDefault: boolean;
  isActive: boolean;
}

const emptyForm: WarehouseForm = {
  code: '', name: '', siteType: 'Warehouse',
  contactPerson: '', managerName: '',
  addressLine1: '', addressLine2: '',
  city: '', area: '',
  phone: '', email: '',
  isDefault: false, isActive: true,
};

// ── Sub-components ────────────────────────────────────────────────────────

function SiteTypeBadge({ type }: { type: string }) {
  const cls = SITE_TYPE_BADGE[type] || 'nx-badge nx-badge-neutral';
  return <span className={cls}>{type}</span>;
}

// ── Warehouse Detail View ─────────────────────────────────────────────────

function WarehouseDetail({
  warehouse,
  onBack,
  onEdit,
}: {
  warehouse: Warehouse;
  onBack: () => void;
  onEdit: (w: Warehouse) => void;
}) {
  const [stats, setStats] = useState<WarehouseStats | null>(null);
  const [counters, setCounters] = useState<PosCounter[]>([]);
  const [terminals, setTerminals] = useState<PosTerminal[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAddCounter, setShowAddCounter] = useState(false);
  const [counterForm, setCounterForm] = useState({ counterName: '', counterCode: '' });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    setLoading(true);
    Promise.all([
      warehouseApiV2.getStats(warehouse.id).catch(() => ({ data: null })),
      posCounterApi.getAll({ pageSize: 100 }).catch(() => ({ data: null })),
      posTerminalApi.getAll({ pageSize: 100 }).catch(() => ({ data: null })),
    ]).then(([sRes, cRes, tRes]) => {
      if (sRes.data) setStats(sRes.data as unknown as WarehouseStats);
      const allCounters = (cRes.data as unknown as { items: PosCounter[] })?.items || [];
      setCounters(allCounters.filter(c => c.warehouseId === warehouse.id));
      const allTerminals = (tRes.data as unknown as { items: PosTerminal[] })?.items || [];
      setTerminals(allTerminals);
    }).finally(() => setLoading(false));
  }, [warehouse.id]);

  const handleAddCounter = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!counterForm.counterName || !counterForm.counterCode) return;
    setSaving(true);
    try {
      await posCounterApi.create({
        warehouseId: warehouse.id,
        counterName: counterForm.counterName,
        counterCode: counterForm.counterCode,
        isActive: true,
      });
      toast.success('Counter added');
      setShowAddCounter(false);
      setCounterForm({ counterName: '', counterCode: '' });
      // refresh
      const cRes = await posCounterApi.getAll({ pageSize: 100 });
      const all = (cRes.data as unknown as { items: PosCounter[] })?.items || [];
      setCounters(all.filter(c => c.warehouseId === warehouse.id));
    } catch {
      toast.error('Failed to add counter');
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteCounter = async (id: string) => {
    if (!confirm('Delete this counter?')) return;
    try {
      await posCounterApi.delete(id);
      setCounters(prev => prev.filter(c => c.id !== id));
      toast.success('Counter removed');
    } catch {
      toast.error('Failed to delete counter');
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="nx-page-header">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={onBack}>
            <ArrowLeft className="w-5 h-5" />
          </Button>
          <div>
            <div className="flex items-center gap-2">
              <h1 className="nx-page-title">{warehouse.name}</h1>
              <SiteTypeBadge type={warehouse.siteType} />
              {warehouse.isDefault && (
                <span className="nx-badge bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-400">
                  <Star className="w-3 h-3 mr-1" />Default
                </span>
              )}
            </div>
            <p className="nx-page-subtitle">
              Code: <span className="font-mono font-semibold">{warehouse.code}</span>
              {warehouse.city && ` · ${warehouse.city}`}
            </p>
          </div>
        </div>
        <div className="nx-page-actions">
          <Button variant="outline" size="sm" onClick={() => onEdit(warehouse)}>
            <Edit className="w-4 h-4 mr-2" /> Edit Warehouse
          </Button>
        </div>
      </div>

      {/* Info + Stats grid */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
        {/* Info card */}
        <div className="nx-card p-5 space-y-3 lg:col-span-2">
          <h3 className="font-semibold text-base">Warehouse Details</h3>
          <div className="grid grid-cols-2 gap-x-6 gap-y-3 text-sm">
            {warehouse.managerName && (
              <div className="flex items-center gap-2">
                <User2 className="w-4 h-4 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground">Manager:</span>
                <span className="font-medium">{warehouse.managerName}</span>
              </div>
            )}
            {warehouse.contactPerson && (
              <div className="flex items-center gap-2">
                <User2 className="w-4 h-4 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground">Contact:</span>
                <span className="font-medium">{warehouse.contactPerson}</span>
              </div>
            )}
            {warehouse.phone && (
              <div className="flex items-center gap-2">
                <Phone className="w-4 h-4 text-muted-foreground shrink-0" />
                <span className="font-medium">{warehouse.phone}</span>
              </div>
            )}
            {warehouse.email && (
              <div className="flex items-center gap-2">
                <Mail className="w-4 h-4 text-muted-foreground shrink-0" />
                <span className="font-medium truncate">{warehouse.email}</span>
              </div>
            )}
            {(warehouse.addressLine1 || warehouse.city) && (
              <div className="flex items-start gap-2 col-span-2">
                <MapPin className="w-4 h-4 text-muted-foreground shrink-0 mt-0.5" />
                <span>
                  {[warehouse.addressLine1, warehouse.addressLine2, warehouse.area, warehouse.city]
                    .filter(Boolean).join(', ')}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Stats */}
        {loading ? (
          <div className="nx-card p-5 flex items-center justify-center">
            <Loader2 className="w-6 h-6 animate-spin text-muted-foreground" />
          </div>
        ) : stats ? (
          <div className="nx-card p-5 space-y-4">
            <h3 className="font-semibold text-base">Live Stats</h3>
            <div className="grid grid-cols-2 gap-3">
              <div className="text-center p-3 rounded-xl bg-secondary/50">
                <p className="text-2xl font-bold text-blue-600">{stats.totalCounters}</p>
                <p className="text-xs text-muted-foreground mt-0.5">Counters</p>
              </div>
              <div className="text-center p-3 rounded-xl bg-secondary/50">
                <p className="text-2xl font-bold text-purple-600">{stats.totalTerminals}</p>
                <p className="text-xs text-muted-foreground mt-0.5">Terminals</p>
              </div>
              <div className="text-center p-3 rounded-xl bg-secondary/50">
                <p className="text-2xl font-bold text-green-600">{stats.activeShifts}</p>
                <p className="text-xs text-muted-foreground mt-0.5">Active Shifts</p>
              </div>
              <div className="text-center p-3 rounded-xl bg-secondary/50">
                <p className="text-2xl font-bold text-amber-600">
                  {stats.todaySales?.toLocaleString() ?? '—'}
                </p>
                <p className="text-xs text-muted-foreground mt-0.5">Today Sales</p>
              </div>
            </div>
          </div>
        ) : null}
      </div>

      {/* POS Counters */}
      <div className="nx-card">
        <div className="flex items-center justify-between px-5 py-4 border-b">
          <h3 className="font-semibold text-base flex items-center gap-2">
            <Monitor className="w-4 h-4 text-muted-foreground" />
            POS Counters
            <span className="nx-badge nx-badge-neutral">{counters.length}</span>
          </h3>
          <Button size="sm" onClick={() => setShowAddCounter(!showAddCounter)}>
            <Plus className="w-4 h-4 mr-2" /> Add Counter
          </Button>
        </div>

        {/* Inline add counter form */}
        {showAddCounter && (
          <form onSubmit={handleAddCounter} className="px-5 py-4 border-b bg-secondary/20">
            <div className="flex items-end gap-3">
              <div className="flex-1">
                <label className="text-xs font-semibold block mb-1">Counter Name *</label>
                <Input
                  placeholder="e.g. Main Counter"
                  value={counterForm.counterName}
                  onChange={e => setCounterForm(f => ({ ...f, counterName: e.target.value }))}
                  required
                  className="h-9"
                />
              </div>
              <div className="w-36">
                <label className="text-xs font-semibold block mb-1">Code *</label>
                <Input
                  placeholder="e.g. CTR-01"
                  value={counterForm.counterCode}
                  onChange={e => setCounterForm(f => ({ ...f, counterCode: e.target.value.toUpperCase() }))}
                  required
                  className="h-9 font-mono"
                />
              </div>
              <Button type="submit" size="sm" className="h-9" disabled={saving}>
                {saving ? <Loader2 className="w-4 h-4 animate-spin" /> : 'Save'}
              </Button>
              <Button
                type="button"
                variant="ghost"
                size="sm"
                className="h-9"
                onClick={() => setShowAddCounter(false)}
              >
                Cancel
              </Button>
            </div>
          </form>
        )}

        {loading ? (
          <div className="flex items-center justify-center py-10">
            <Loader2 className="w-6 h-6 animate-spin text-muted-foreground" />
          </div>
        ) : counters.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-10 text-muted-foreground">
            <Monitor className="w-10 h-10 mb-2 opacity-20" />
            <p className="font-medium">No counters yet</p>
            <p className="text-sm mt-1">Add a counter to enable POS operations</p>
          </div>
        ) : (
          <div className="divide-y">
            {counters.map(counter => {
              const cTerminals = terminals.filter(t => t.posCounterId === counter.id);
              return (
                <div key={counter.id} className="px-5 py-4">
                  <div className="flex items-center justify-between mb-2">
                    <div className="flex items-center gap-3">
                      <div className="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center">
                        <Monitor className="w-4 h-4 text-primary" />
                      </div>
                      <div>
                        <p className="font-semibold text-sm">{counter.counterName}</p>
                        <p className="text-xs text-muted-foreground font-mono">{counter.counterCode}</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-2">
                      <span className={`nx-badge ${counter.isActive ? 'nx-badge-success' : 'nx-badge-neutral'}`}>
                        {counter.isActive ? 'Active' : 'Inactive'}
                      </span>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="w-7 h-7 text-red-500 hover:text-red-700"
                        onClick={() => handleDeleteCounter(counter.id)}
                      >
                        <Trash2 className="w-3.5 h-3.5" />
                      </Button>
                    </div>
                  </div>

                  {/* Terminals for this counter */}
                  {cTerminals.length > 0 && (
                    <div className="ml-11 mt-2 space-y-1.5">
                      {cTerminals.map(t => (
                        <div key={t.id} className="flex items-center gap-2 text-xs">
                          <div className="w-1.5 h-1.5 rounded-full bg-muted-foreground" />
                          <Activity className="w-3 h-3 text-muted-foreground" />
                          <span className="font-medium">{t.terminalName}</span>
                          <span className="text-muted-foreground font-mono">({t.terminalCode})</span>
                          <span className={`nx-badge text-xs ${t.isActive ? 'nx-badge-success' : 'nx-badge-neutral'}`}>
                            {t.isActive ? 'Online' : 'Offline'}
                          </span>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}

// ── Main Warehouses Page ──────────────────────────────────────────────────

export default function Warehouses() {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 12;

  // Detail view
  const [selectedWarehouse, setSelectedWarehouse] = useState<Warehouse | null>(null);

  // Create/Edit modal
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<Warehouse | null>(null);
  const [formData, setFormData] = useState<WarehouseForm>(emptyForm);

  // Delete confirm
  const [deleteTarget, setDeleteTarget] = useState<Warehouse | null>(null);

  // ── Fetching ──────────────────────────────────────────────────────────

  const fetchWarehouses = useCallback(async (q?: string, page = currentPage) => {
    setLoading(true);
    try {
      const res = await warehouseApiV2.getAll({
        pageIndex: page - 1,
        pageSize,
        search: q || undefined,
      });
      const data = res.data as unknown as { items: Warehouse[]; totalCount: number };
      setWarehouses(data?.items || []);
      setTotalCount(data?.totalCount || 0);
    } catch {
      toast.error('Failed to load warehouses');
    } finally {
      setLoading(false);
    }
  }, [currentPage]);

  useEffect(() => { fetchWarehouses(); }, [currentPage]);

  // ── Modal handlers ────────────────────────────────────────────────────

  const openCreate = () => {
    setEditing(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEdit = (w: Warehouse) => {
    setEditing(w);
    setFormData({
      code: w.code, name: w.name, siteType: w.siteType || 'Warehouse',
      contactPerson: w.contactPerson || '', managerName: w.managerName || '',
      addressLine1: w.addressLine1 || '', addressLine2: w.addressLine2 || '',
      city: w.city || '', area: w.area || '',
      phone: w.phone || '', email: w.email || '',
      isDefault: w.isDefault, isActive: w.isActive,
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      if (editing) {
        await warehouseApiV2.update(editing.id, formData);
        toast.success('Warehouse updated');
        // Update the selected warehouse if it's the one being edited
        if (selectedWarehouse?.id === editing.id) {
          setSelectedWarehouse({ ...selectedWarehouse, ...formData });
        }
      } else {
        await warehouseApiV2.create(formData);
        toast.success('Warehouse created');
      }
      setShowModal(false);
      fetchWarehouses();
    } catch {
      toast.error('Save failed');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setSaving(true);
    try {
      await warehouseApiV2.delete(deleteTarget.id);
      toast.success('Warehouse deleted');
      setDeleteTarget(null);
      fetchWarehouses();
    } catch {
      toast.error('Delete failed');
    } finally {
      setSaving(false);
    }
  };

  const handleToggle = async (w: Warehouse, e: React.MouseEvent) => {
    e.stopPropagation();
    try {
      await warehouseApiV2.toggleActive(w.id);
      toast.success(`Warehouse ${w.isActive ? 'deactivated' : 'activated'}`);
      fetchWarehouses();
    } catch {
      toast.error('Toggle failed');
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);
  const activeCount = warehouses.filter(w => w.isActive).length;
  const defaultWh = warehouses.find(w => w.isDefault);

  // ── Detail view ───────────────────────────────────────────────────────

  if (selectedWarehouse) {
    return (
      <WarehouseDetail
        warehouse={selectedWarehouse}
        onBack={() => setSelectedWarehouse(null)}
        onEdit={w => { setSelectedWarehouse(null); openEdit(w); }}
      />
    );
  }

  // ── List view ─────────────────────────────────────────────────────────

  return (
    <div className="space-y-6">
      {/* Page header */}
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Warehouses</h1>
          <p className="nx-page-subtitle">Manage warehouses, stores, and outlets</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreate}>
            <Plus className="w-4 h-4 mr-2" /> Add Warehouse
          </Button>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Warehouses</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value success">{activeCount}</div>
          <div className="nx-stat-label">Active</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value" style={{ color: 'var(--nx-danger)' }}>{totalCount - activeCount}</div>
          <div className="nx-stat-label">Inactive</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value info truncate text-base mt-1">{defaultWh?.name ?? '—'}</div>
          <div className="nx-stat-label">Default Warehouse</div>
        </div>
      </div>

      {/* Table card */}
      <div className="nx-card">
        {/* Toolbar */}
        <div className="flex items-center gap-3 px-4 py-3 border-b">
          <div className="nx-table-search flex-1 max-w-sm">
            <Search className="w-4 h-4 shrink-0" />
            <input
              type="text"
              placeholder="Search warehouses..."
              value={searchQuery}
              onChange={e => setSearchQuery(e.target.value)}
              onKeyDown={e => {
                if (e.key === 'Enter') { setCurrentPage(1); fetchWarehouses(searchQuery, 1); }
              }}
            />
          </div>
          <Button
            variant="outline"
            size="sm"
            onClick={() => { setCurrentPage(1); fetchWarehouses(searchQuery, 1); }}
          >
            <Search className="w-4 h-4 mr-2" /> Search
          </Button>
          {searchQuery && (
            <Button
              variant="ghost"
              size="sm"
              onClick={() => { setSearchQuery(''); setCurrentPage(1); fetchWarehouses('', 1); }}
            >
              <X className="w-4 h-4" />
            </Button>
          )}
        </div>

        {loading ? (
          <div className="flex items-center justify-center py-16">
            <Loader2 className="w-8 h-8 animate-spin text-muted-foreground" />
          </div>
        ) : warehouses.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-16 text-muted-foreground">
            <WarehouseIcon className="w-12 h-12 mb-3 opacity-20" />
            <p className="font-medium">No warehouses found</p>
          </div>
        ) : (
          <>
            <div className="overflow-auto">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Code</th>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Location</th>
                    <th>Manager</th>
                    <th>Phone</th>
                    <th>Status</th>
                    <th className="text-center" style={{ width: 120 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {warehouses.map(w => (
                    <tr
                      key={w.id}
                      className="cursor-pointer"
                      onClick={() => setSelectedWarehouse(w)}
                    >
                      <td>
                        <code className="text-xs bg-secondary px-2 py-1 rounded font-mono">
                          {w.code}
                        </code>
                      </td>
                      <td>
                        <div className="flex items-center gap-2">
                          <div className="w-7 h-7 rounded-lg bg-primary/10 flex items-center justify-center shrink-0">
                            <WarehouseIcon className="w-3.5 h-3.5 text-primary" />
                          </div>
                          <div className="min-w-0">
                            <p className="font-semibold text-sm truncate">{w.name}</p>
                            {w.isDefault && (
                              <span className="text-xs text-amber-600 flex items-center gap-0.5">
                                <Star className="w-2.5 h-2.5" /> Default
                              </span>
                            )}
                          </div>
                        </div>
                      </td>
                      <td><SiteTypeBadge type={w.siteType} /></td>
                      <td>
                        {(w.city || w.area) ? (
                          <div className="flex items-center gap-1 text-sm">
                            <MapPin className="w-3.5 h-3.5 text-muted-foreground" />
                            <span>{[w.area, w.city].filter(Boolean).join(', ')}</span>
                          </div>
                        ) : (
                          <span className="text-muted-foreground">—</span>
                        )}
                      </td>
                      <td>
                        {w.managerName ? (
                          <div className="flex items-center gap-1 text-sm">
                            <User2 className="w-3.5 h-3.5 text-muted-foreground" />
                            {w.managerName}
                          </div>
                        ) : <span className="text-muted-foreground">—</span>}
                      </td>
                      <td className="text-sm text-muted-foreground">{w.phone || '—'}</td>
                      <td>
                        <span className={`nx-badge ${w.isActive ? 'nx-badge-success' : 'nx-badge-neutral'}`}>
                          {w.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td onClick={e => e.stopPropagation()}>
                        <div className="flex items-center justify-center gap-1">
                          <Button
                            variant="ghost"
                            size="icon"
                            className="w-7 h-7"
                            onClick={e => handleToggle(w, e)}
                            title={w.isActive ? 'Deactivate' : 'Activate'}
                          >
                            {w.isActive
                              ? <ToggleRight className="w-4 h-4 text-green-600" />
                              : <ToggleLeft className="w-4 h-4 text-muted-foreground" />
                            }
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="w-7 h-7"
                            onClick={e => { e.stopPropagation(); openEdit(w); }}
                          >
                            <Edit className="w-3.5 h-3.5" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="w-7 h-7 text-red-500 hover:text-red-700"
                            onClick={e => { e.stopPropagation(); setDeleteTarget(w); }}
                          >
                            <Trash2 className="w-3.5 h-3.5" />
                          </Button>
                          <ChevRight className="w-4 h-4 text-muted-foreground" />
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="flex items-center justify-between px-4 py-3 border-t">
                <p className="text-sm text-muted-foreground">
                  Showing {warehouses.length} of {totalCount}
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
                  <span className="text-sm font-medium px-1">
                    Page {currentPage} of {totalPages}
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
            )}
          </>
        )}
      </div>

      {/* ── Create / Edit Modal ────────────────────────────────────────────── */}
      {showModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between px-5 py-4 border-b sticky top-0 bg-background">
              <h2 className="text-lg font-semibold">
                {editing ? 'Edit Warehouse' : 'Create Warehouse'}
              </h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>

            <form onSubmit={handleSubmit} className="p-5">
              <div className="grid grid-cols-2 gap-4">
                {/* Code */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">
                    Code <span className="text-destructive">*</span>
                  </label>
                  <Input
                    value={formData.code}
                    onChange={e => setFormData({ ...formData, code: e.target.value.toUpperCase() })}
                    required
                    placeholder="e.g. WH-01"
                    className="font-mono"
                  />
                </div>

                {/* Name */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">
                    Name <span className="text-destructive">*</span>
                  </label>
                  <Input
                    value={formData.name}
                    onChange={e => setFormData({ ...formData, name: e.target.value })}
                    required
                    placeholder="Warehouse name"
                  />
                </div>

                {/* Site Type */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">Site Type</label>
                  <select
                    className="nx-input nx-select w-full"
                    value={formData.siteType}
                    onChange={e => setFormData({ ...formData, siteType: e.target.value })}
                  >
                    {SITE_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
                  </select>
                </div>

                {/* Manager */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">Manager</label>
                  <Input
                    value={formData.managerName}
                    onChange={e => setFormData({ ...formData, managerName: e.target.value })}
                    placeholder="Manager name"
                  />
                </div>

                {/* Contact Person */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">Contact Person</label>
                  <Input
                    value={formData.contactPerson}
                    onChange={e => setFormData({ ...formData, contactPerson: e.target.value })}
                    placeholder="Contact person"
                  />
                </div>

                {/* Phone */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">Phone</label>
                  <Input
                    value={formData.phone}
                    onChange={e => setFormData({ ...formData, phone: e.target.value })}
                    placeholder="+880 ..."
                  />
                </div>

                {/* Email */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">Email</label>
                  <Input
                    type="email"
                    value={formData.email}
                    onChange={e => setFormData({ ...formData, email: e.target.value })}
                    placeholder="warehouse@example.com"
                  />
                </div>

                {/* City */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">City</label>
                  <Input
                    value={formData.city}
                    onChange={e => setFormData({ ...formData, city: e.target.value })}
                    placeholder="Dhaka"
                  />
                </div>

                {/* Area */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">Area</label>
                  <Input
                    value={formData.area}
                    onChange={e => setFormData({ ...formData, area: e.target.value })}
                    placeholder="Gulshan"
                  />
                </div>

                {/* Address 1 */}
                <div>
                  <label className="text-sm font-semibold block mb-1.5">Address Line 1</label>
                  <Input
                    value={formData.addressLine1}
                    onChange={e => setFormData({ ...formData, addressLine1: e.target.value })}
                    placeholder="Street address"
                  />
                </div>

                {/* Address 2 */}
                <div className="col-span-2">
                  <label className="text-sm font-semibold block mb-1.5">Address Line 2</label>
                  <Input
                    value={formData.addressLine2}
                    onChange={e => setFormData({ ...formData, addressLine2: e.target.value })}
                    placeholder="Apt, floor, building..."
                  />
                </div>

                {/* Flags */}
                <div className="col-span-2 flex items-center gap-6 pt-1">
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      className="nx-checkbox"
                      checked={formData.isActive}
                      onChange={e => setFormData({ ...formData, isActive: e.target.checked })}
                    />
                    <span className="text-sm font-medium">Active</span>
                  </label>
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      className="nx-checkbox"
                      checked={formData.isDefault}
                      onChange={e => setFormData({ ...formData, isDefault: e.target.checked })}
                    />
                    <span className="text-sm font-medium">Set as Default</span>
                  </label>
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-5 mt-4 border-t">
                <Button type="button" variant="outline" onClick={() => setShowModal(false)}>
                  Cancel
                </Button>
                <Button type="submit" disabled={saving}>
                  {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editing ? 'Update Warehouse' : 'Create Warehouse'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* ── Delete Confirm Modal ──────────────────────────────────────────── */}
      {deleteTarget && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl shadow-2xl w-full max-w-sm overflow-hidden">
            <div className="bg-destructive/10 border-b border-destructive/20 px-5 py-4">
              <h2 className="text-lg font-semibold text-destructive">Delete Warehouse</h2>
            </div>
            <div className="p-5">
              <p className="text-sm text-muted-foreground">
                Are you sure you want to delete{' '}
                <span className="font-semibold text-foreground">"{deleteTarget.name}"</span>?
                This action cannot be undone.
              </p>
              <div className="flex justify-end gap-2 mt-5">
                <Button variant="outline" onClick={() => setDeleteTarget(null)}>Cancel</Button>
                <Button
                  variant="destructive"
                  onClick={handleDelete}
                  disabled={saving}
                >
                  {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  Delete
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
