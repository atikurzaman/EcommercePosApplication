import { useState, useEffect, useCallback } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Package, AlertTriangle, TrendingDown, XCircle, ChevronLeft, ChevronRight,
  Loader2, RefreshCw, Plus, Minus, UploadCloud, X, SlidersHorizontal,
  DollarSign,
} from 'lucide-react';
import { inventoryApi, type StockItem } from '@/api/inventoryApi';
import { warehouseApiV2, type Warehouse } from '@/api/posApi';
import toast from 'react-hot-toast';

// ── Types ─────────────────────────────────────────────────────────────────

type StockStatus = 'ok' | 'low' | 'critical' | 'out';

interface AdjustForm {
  quantity: number;
  adjustmentType: 'ADD' | 'REMOVE' | 'SET';
  reason: string;
  notes: string;
}

// ── Helpers ───────────────────────────────────────────────────────────────

function formatCurrency(n: number) {
  return new Intl.NumberFormat('en-BD', {
    style: 'currency', currency: 'BDT', minimumFractionDigits: 0,
  }).format(n);
}

function getStockStatus(item: StockItem): StockStatus {
  if (item.quantityOnHand <= 0) return 'out';
  if (item.reorderLevel > 0 && item.quantityOnHand <= item.reorderLevel * 0.5) return 'critical';
  if (item.reorderLevel > 0 && item.quantityOnHand <= item.reorderLevel) return 'low';
  return 'ok';
}

const STATUS_CONFIG: Record<StockStatus, { label: string; badgeClass: string }> = {
  ok:       { label: 'In Stock',    badgeClass: 'nx-badge nx-badge-success' },
  low:      { label: 'Low Stock',   badgeClass: 'nx-badge nx-badge-warning' },
  critical: { label: 'Critical',    badgeClass: 'nx-badge bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400' },
  out:      { label: 'Out of Stock',badgeClass: 'nx-badge nx-badge-danger' },
};

function StatusBadge({ status }: { status: StockStatus }) {
  const cfg = STATUS_CONFIG[status];
  return <span className={cfg.badgeClass}>{cfg.label}</span>;
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('en-BD', { day: 'numeric', month: 'short', year: 'numeric' });
}

// ── Component ─────────────────────────────────────────────────────────────

export default function StockItemsPage() {
  // Data
  const [items, setItems] = useState<StockItem[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [totalCount, setTotalCount] = useState(0);

  // Pagination & filters
  const [currentPage, setCurrentPage] = useState(1);
  const [filterWarehouse, setFilterWarehouse] = useState('');
  const [filterStatus, setFilterStatus] = useState<'' | StockStatus>('');
  const pageSize = 15;

  // Stats derived from current page + totals from API
  const [stats, setStats] = useState({ totalSkus: 0, totalValue: 0, lowStock: 0, outOfStock: 0 });

  // Quick Adjust modal
  const [adjustTarget, setAdjustTarget] = useState<StockItem | null>(null);
  const [adjustForm, setAdjustForm] = useState<AdjustForm>({
    quantity: 1, adjustmentType: 'ADD', reason: '', notes: '',
  });
  const [adjustSaving, setAdjustSaving] = useState(false);

  // ── Data fetching ──────────────────────────────────────────────────────

  const fetchItems = useCallback(async () => {
    setLoading(true);
    try {
      if (filterStatus === 'out' || filterStatus === 'low' || filterStatus === 'critical') {
        const res = await inventoryApi.getLowStockItems();
        const allLow: StockItem[] = (res.data as unknown as { data: StockItem[] })?.data || [];
        const filtered = filterStatus === 'out'
          ? allLow.filter(i => i.quantityOnHand <= 0)
          : filterStatus === 'critical'
            ? allLow.filter(i => i.quantityOnHand > 0 && i.reorderLevel > 0 && i.quantityOnHand <= i.reorderLevel * 0.5)
            : allLow.filter(i => i.quantityOnHand > 0 && i.quantityOnHand <= i.reorderLevel);
        setItems(filtered);
        setTotalCount(filtered.length);
      } else {
        const res = await inventoryApi.getStockItems({
          pageIndex: currentPage - 1,
          pageSize,
          warehouseId: filterWarehouse || undefined,
        });
        const data = (res.data as unknown as { data: { items: StockItem[]; totalCount: number } })?.data;
        setItems(data?.items || []);
        setTotalCount(data?.totalCount || 0);
      }
    } catch {
      toast.error('Failed to load stock items');
    } finally {
      setLoading(false);
    }
  }, [currentPage, filterWarehouse, filterStatus]);

  const fetchStats = useCallback(async () => {
    try {
      // Summary stats: all items + low stock
      const [allRes, lowRes] = await Promise.all([
        inventoryApi.getStockItems({ pageIndex: 0, pageSize: 1 }),
        inventoryApi.getLowStockItems(),
      ]);
      const allData = (allRes.data as unknown as { data: { totalCount: number; items: StockItem[] } })?.data;
      const lowItems: StockItem[] = (lowRes.data as unknown as { data: StockItem[] })?.data || [];

      const outCount = lowItems.filter(i => i.quantityOnHand <= 0).length;
      const lowCount = lowItems.filter(i => i.quantityOnHand > 0).length;

      setStats({
        totalSkus: allData?.totalCount || 0,
        totalValue: (allData?.items || []).reduce((s, i) => s + i.quantityOnHand * i.averageCostPrice, 0),
        lowStock: lowCount,
        outOfStock: outCount,
      });
    } catch { /* ignore */ }
  }, []);

  useEffect(() => {
    fetchItems();
  }, [fetchItems]);

  useEffect(() => {
    fetchStats();
  }, [fetchStats]);

  useEffect(() => {
    warehouseApiV2.getAll({ pageSize: 100 }).then(res => {
      setWarehouses((res.data as unknown as { items: Warehouse[] })?.items || []);
    }).catch(() => { /* ignore */ });
  }, []);

  // ── Adjust modal ──────────────────────────────────────────────────────

  const openAdjust = (item: StockItem) => {
    setAdjustTarget(item);
    setAdjustForm({ quantity: 1, adjustmentType: 'ADD', reason: '', notes: '' });
  };

  const handleAdjust = async () => {
    if (!adjustTarget) return;
    if (!adjustForm.reason.trim()) {
      toast.error('Please provide a reason');
      return;
    }
    setAdjustSaving(true);
    try {
      const qtyAdjusted = adjustForm.adjustmentType === 'REMOVE'
        ? -Math.abs(adjustForm.quantity)
        : Math.abs(adjustForm.quantity);

      await inventoryApi.createInventoryAdjustment({
        warehouseId: adjustTarget.warehouseId,
        adjustmentType: adjustForm.adjustmentType,
        reason: adjustForm.reason,
        notes: adjustForm.notes || undefined,
        lines: [{
          productId: adjustTarget.productId,
          variantId: adjustTarget.variantId,
          quantityAdjusted: qtyAdjusted,
          reason: adjustForm.reason,
        }],
      });
      toast.success('Stock adjusted successfully');
      setAdjustTarget(null);
      fetchItems();
      fetchStats();
    } catch {
      toast.error('Adjustment failed');
    } finally {
      setAdjustSaving(false);
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  // ── Bulk import placeholder ────────────────────────────────────────────

  const handleBulkImport = () => {
    toast('Bulk import via CSV coming soon', { icon: '📤' });
  };

  // ── Page value for visible items ──────────────────────────────────────

  const pageValue = items.reduce((s, i) => s + i.quantityOnHand * i.averageCostPrice, 0);

  return (
    <div className="space-y-6">
      {/* Page header */}
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Stock Items</h1>
          <p className="nx-page-subtitle">Monitor and manage inventory levels across warehouses</p>
        </div>
        <div className="nx-page-actions">
          <Button variant="outline" size="sm" onClick={handleBulkImport}>
            <UploadCloud className="w-4 h-4 mr-2" /> Bulk Import
          </Button>
          <Button variant="outline" size="sm" onClick={() => { fetchItems(); fetchStats(); }}>
            <RefreshCw className="w-4 h-4 mr-2" /> Refresh
          </Button>
        </div>
      </div>

      {/* Stats cards */}
      <div className="nx-stats-grid">
        <div className="nx-stat-card">
          <div className="flex items-start justify-between mb-2">
            <div className="w-9 h-9 rounded-lg bg-blue-100 dark:bg-blue-900/30 flex items-center justify-center">
              <Package className="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
          <div className="nx-stat-value info">{stats.totalSkus.toLocaleString()}</div>
          <div className="nx-stat-label">Total SKUs</div>
        </div>

        <div className="nx-stat-card">
          <div className="flex items-start justify-between mb-2">
            <div className="w-9 h-9 rounded-lg bg-green-100 dark:bg-green-900/30 flex items-center justify-center">
              <DollarSign className="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div className="nx-stat-value success">{formatCurrency(pageValue)}</div>
          <div className="nx-stat-label">Visible Page Value</div>
        </div>

        <div className="nx-stat-card">
          <div className="flex items-start justify-between mb-2">
            <div className="w-9 h-9 rounded-lg bg-yellow-100 dark:bg-yellow-900/30 flex items-center justify-center">
              <AlertTriangle className="w-5 h-5 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
          <div className="nx-stat-value warning">{stats.lowStock}</div>
          <div className="nx-stat-label">Low Stock Items</div>
        </div>

        <div className="nx-stat-card">
          <div className="flex items-start justify-between mb-2">
            <div className="w-9 h-9 rounded-lg bg-red-100 dark:bg-red-900/30 flex items-center justify-center">
              <TrendingDown className="w-5 h-5 text-red-600 dark:text-red-400" />
            </div>
          </div>
          <div className="nx-stat-value" style={{ color: 'var(--nx-danger)' }}>{stats.outOfStock}</div>
          <div className="nx-stat-label">Out of Stock</div>
        </div>
      </div>

      {/* Main card */}
      <div className="nx-card">
        {/* Toolbar */}
        <div className="p-4 border-b flex flex-wrap items-center gap-3">
          <div className="flex items-center gap-2 flex-1 min-w-0">
            <SlidersHorizontal className="w-4 h-4 text-muted-foreground shrink-0" />
            <span className="text-sm font-medium text-muted-foreground shrink-0">Filter:</span>

            <select
              className="nx-input nx-select w-44"
              value={filterWarehouse}
              onChange={e => { setFilterWarehouse(e.target.value); setCurrentPage(1); }}
            >
              <option value="">All Warehouses</option>
              {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
            </select>

            <select
              className="nx-input nx-select w-40"
              value={filterStatus}
              onChange={e => { setFilterStatus(e.target.value as '' | StockStatus); setCurrentPage(1); }}
            >
              <option value="">All Statuses</option>
              <option value="ok">In Stock</option>
              <option value="low">Low Stock</option>
              <option value="critical">Critical</option>
              <option value="out">Out of Stock</option>
            </select>

            {(filterWarehouse || filterStatus) && (
              <Button
                variant="ghost"
                size="sm"
                className="h-8 text-xs"
                onClick={() => { setFilterWarehouse(''); setFilterStatus(''); setCurrentPage(1); }}
              >
                <X className="w-3.5 h-3.5 mr-1" /> Clear
              </Button>
            )}
          </div>

          <p className="text-sm text-muted-foreground shrink-0">
            {totalCount.toLocaleString()} item{totalCount !== 1 ? 's' : ''}
          </p>
        </div>

        {/* Table */}
        {loading ? (
          <div className="flex items-center justify-center p-16">
            <Loader2 className="w-8 h-8 animate-spin text-muted-foreground" />
          </div>
        ) : items.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-16 text-muted-foreground">
            <Package className="w-12 h-12 mb-3 opacity-20" />
            <p className="font-medium">No stock items found</p>
            <p className="text-sm mt-1">Adjust your filters and try again</p>
          </div>
        ) : (
          <>
            <div className="overflow-auto">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>SKU / Product</th>
                    <th>Warehouse</th>
                    <th className="text-right">On Hand</th>
                    <th className="text-right">Reserved</th>
                    <th className="text-right">Available</th>
                    <th className="text-right">Avg Cost</th>
                    <th className="text-right">Value</th>
                    <th className="text-right">Reorder Level</th>
                    <th>Status</th>
                    <th>Last Updated</th>
                    <th className="text-center">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map(item => {
                    const status = getStockStatus(item);
                    const available = item.quantityOnHand - item.reservedQuantity;
                    return (
                      <tr key={item.id}>
                        <td>
                          <div className="flex items-center gap-3">
                            <div className="w-8 h-8 rounded-lg bg-secondary flex items-center justify-center shrink-0">
                              <Package className="w-4 h-4 text-muted-foreground" />
                            </div>
                            <div className="min-w-0">
                              <p className="font-semibold text-sm truncate">{item.productName}</p>
                              {item.variantId && (
                                <p className="text-xs text-muted-foreground">Variant</p>
                              )}
                            </div>
                          </div>
                        </td>
                        <td>
                          <span className="text-sm">{item.warehouseName}</span>
                        </td>
                        <td className="text-right font-semibold">
                          {item.quantityOnHand.toLocaleString()}
                        </td>
                        <td className="text-right text-muted-foreground">
                          {item.reservedQuantity.toLocaleString()}
                        </td>
                        <td className={`text-right font-medium ${available < 0 ? 'text-red-600' : ''}`}>
                          {available.toLocaleString()}
                        </td>
                        <td className="text-right text-muted-foreground">
                          {formatCurrency(item.averageCostPrice)}
                        </td>
                        <td className="text-right font-medium">
                          {formatCurrency(item.quantityOnHand * item.averageCostPrice)}
                        </td>
                        <td className="text-right">
                          <span className={`text-sm font-medium ${
                            item.reorderLevel > 0 && item.quantityOnHand <= item.reorderLevel
                              ? 'text-amber-600'
                              : 'text-muted-foreground'
                          }`}>
                            {item.reorderLevel}
                          </span>
                        </td>
                        <td>
                          <StatusBadge status={status} />
                        </td>
                        <td className="text-sm text-muted-foreground">
                          {item.lastUpdatedAt ? formatDate(item.lastUpdatedAt) : '—'}
                        </td>
                        <td className="text-center">
                          <Button
                            variant="outline"
                            size="sm"
                            className="h-7 text-xs px-2.5"
                            onClick={() => openAdjust(item)}
                          >
                            <SlidersHorizontal className="w-3 h-3 mr-1" /> Adjust
                          </Button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            {!filterStatus && totalPages > 1 && (
              <div className="flex items-center justify-between px-4 py-3 border-t">
                <p className="text-sm text-muted-foreground">
                  Showing {items.length} of {totalCount.toLocaleString()} items
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

      {/* Quick Adjust Modal */}
      {adjustTarget && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl shadow-2xl w-full max-w-md overflow-hidden">
            {/* Header */}
            <div className="flex items-center justify-between px-5 py-4 border-b">
              <div>
                <h2 className="text-lg font-semibold">Quick Stock Adjust</h2>
                <p className="text-sm text-muted-foreground mt-0.5 truncate max-w-[280px]">
                  {adjustTarget.productName} &middot; {adjustTarget.warehouseName}
                </p>
              </div>
              <Button variant="ghost" size="icon" onClick={() => setAdjustTarget(null)}>
                <X className="w-4 h-4" />
              </Button>
            </div>

            <div className="p-5 space-y-4">
              {/* Current stock info */}
              <div className="grid grid-cols-3 gap-3">
                <div className="text-center p-3 rounded-xl bg-secondary/50">
                  <p className="text-2xl font-bold">{adjustTarget.quantityOnHand}</p>
                  <p className="text-xs text-muted-foreground mt-0.5">Current</p>
                </div>
                <div className="text-center p-3 rounded-xl bg-secondary/50">
                  <p className="text-2xl font-bold text-amber-600">{adjustTarget.reservedQuantity}</p>
                  <p className="text-xs text-muted-foreground mt-0.5">Reserved</p>
                </div>
                <div className="text-center p-3 rounded-xl bg-secondary/50">
                  <p className="text-2xl font-bold">{adjustTarget.reorderLevel}</p>
                  <p className="text-xs text-muted-foreground mt-0.5">Reorder At</p>
                </div>
              </div>

              {/* Adjustment type */}
              <div>
                <label className="text-sm font-semibold block mb-1.5">Adjustment Type</label>
                <div className="grid grid-cols-2 gap-2">
                  {(['ADD', 'REMOVE'] as const).map(type => (
                    <button
                      key={type}
                      onClick={() => setAdjustForm(f => ({ ...f, adjustmentType: type }))}
                      className={[
                        'flex items-center justify-center gap-2 py-2.5 rounded-lg border font-medium text-sm transition-all',
                        adjustForm.adjustmentType === type
                          ? type === 'ADD'
                            ? 'bg-green-600 text-white border-green-600'
                            : 'bg-red-600 text-white border-red-600'
                          : 'border-input text-muted-foreground hover:bg-secondary',
                      ].join(' ')}
                    >
                      {type === 'ADD'
                        ? <><Plus className="w-4 h-4" /> Add Stock</>
                        : <><Minus className="w-4 h-4" /> Remove Stock</>
                      }
                    </button>
                  ))}
                </div>
              </div>

              {/* Quantity */}
              <div>
                <label className="text-sm font-semibold block mb-1.5">Quantity</label>
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => setAdjustForm(f => ({ ...f, quantity: Math.max(1, f.quantity - 1) }))}
                    className="w-10 h-10 rounded-lg border flex items-center justify-center hover:bg-secondary transition-colors"
                  >
                    <Minus className="w-4 h-4" />
                  </button>
                  <Input
                    type="number"
                    min={1}
                    className="flex-1 text-center text-lg font-bold h-10"
                    value={adjustForm.quantity}
                    onChange={e => setAdjustForm(f => ({ ...f, quantity: Math.max(1, parseInt(e.target.value) || 1) }))}
                  />
                  <button
                    onClick={() => setAdjustForm(f => ({ ...f, quantity: f.quantity + 1 }))}
                    className="w-10 h-10 rounded-lg border flex items-center justify-center hover:bg-secondary transition-colors"
                  >
                    <Plus className="w-4 h-4" />
                  </button>
                </div>
                <p className="text-xs text-muted-foreground mt-1.5 text-center">
                  New balance will be:{' '}
                  <span className="font-bold text-foreground">
                    {adjustForm.adjustmentType === 'ADD'
                      ? adjustTarget.quantityOnHand + adjustForm.quantity
                      : Math.max(0, adjustTarget.quantityOnHand - adjustForm.quantity)
                    }
                  </span>
                </p>
              </div>

              {/* Reason */}
              <div>
                <label className="text-sm font-semibold block mb-1.5">
                  Reason <span className="text-destructive">*</span>
                </label>
                <select
                  className="nx-input nx-select w-full"
                  value={adjustForm.reason}
                  onChange={e => setAdjustForm(f => ({ ...f, reason: e.target.value }))}
                >
                  <option value="">Select reason...</option>
                  {(adjustForm.adjustmentType === 'ADD'
                    ? ['Purchase receipt', 'Returned goods', 'Production input', 'Found in warehouse', 'Opening stock']
                    : ['Damaged / expired', 'Sold offline', 'Theft / shrinkage', 'Sample / write-off', 'Transfer out']
                  ).map(r => <option key={r} value={r}>{r}</option>)}
                  <option value="Other">Other</option>
                </select>
              </div>

              {/* Notes */}
              <div>
                <label className="text-sm font-semibold block mb-1.5">Notes (optional)</label>
                <Input
                  placeholder="Additional notes..."
                  value={adjustForm.notes}
                  onChange={e => setAdjustForm(f => ({ ...f, notes: e.target.value }))}
                />
              </div>

              {/* Actions */}
              <div className="flex gap-2 pt-1">
                <Button variant="outline" className="flex-1" onClick={() => setAdjustTarget(null)}>
                  Cancel
                </Button>
                <Button
                  className={`flex-1 ${adjustForm.adjustmentType === 'ADD' ? 'bg-green-600 hover:bg-green-700' : 'bg-red-600 hover:bg-red-700'} text-white`}
                  onClick={handleAdjust}
                  disabled={adjustSaving || !adjustForm.reason}
                >
                  {adjustSaving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {adjustForm.adjustmentType === 'ADD' ? 'Add' : 'Remove'} {adjustForm.quantity} Unit{adjustForm.quantity !== 1 ? 's' : ''}
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
