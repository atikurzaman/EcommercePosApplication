import { useState, useEffect, useCallback } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Search, ChevronLeft, ChevronRight, Loader2, X, Eye, XCircle,
  Receipt, Download, Filter, RefreshCw, Calendar, TrendingUp,
  ShoppingBag, Banknote, CreditCard, Smartphone,
} from 'lucide-react';
import {
  posTransactionApiV2, warehouseApiV2,
  type PosTransaction, type PosTransactionDetail, type Warehouse,
} from '@/api/posApi';
import toast from 'react-hot-toast';

// ── Types ─────────────────────────────────────────────────────────────────

type TxStatus = 'Completed' | 'Held' | 'Voided' | '';

// ── Helpers ───────────────────────────────────────────────────────────────

function fmt(n: number) {
  return n.toLocaleString('en-BD', {
    style: 'currency', currency: 'BDT', minimumFractionDigits: 2,
  });
}

function fmtDate(d: string) {
  return new Date(d).toLocaleString('en-BD', {
    day: 'numeric', month: 'short', year: 'numeric',
    hour: '2-digit', minute: '2-digit',
  });
}

function fmtDateShort(d: string) {
  return new Date(d).toLocaleDateString('en-BD', {
    day: 'numeric', month: 'short',
  });
}

const STATUS_BADGE: Record<string, string> = {
  Completed: 'nx-badge nx-badge-success',
  Held:      'nx-badge nx-badge-warning',
  Voided:    'nx-badge nx-badge-danger',
};

function StatusBadge({ status }: { status: string }) {
  return (
    <span className={STATUS_BADGE[status] || 'nx-badge nx-badge-neutral'}>{status}</span>
  );
}

const PAYMENT_ICON: Record<string, React.ElementType> = {
  CASH:   Banknote,
  CARD:   CreditCard,
  MOBILE: Smartphone,
};

function PaymentIcon({ method }: { method: string }) {
  const Icon = PAYMENT_ICON[method?.toUpperCase()] ?? Banknote;
  return <Icon className="w-3.5 h-3.5 inline mr-0.5" />;
}

// ── CSV export ────────────────────────────────────────────────────────────

function exportCSV(rows: PosTransaction[]) {
  const headers = [
    'Receipt #', 'Date', 'Customer', 'Cashier', 'Warehouse',
    'Items', 'Subtotal', 'Discount', 'Grand Total', 'Paid', 'Change', 'Status',
  ];
  const lines = rows.map(t => [
    t.receiptNumber,
    t.saleDate ? new Date(t.saleDate).toISOString() : '',
    t.customerName || 'Walk-in',
    t.cashierName || '',
    t.warehouseName || '',
    t.totalItemQuantity,
    t.subTotal,
    t.discountAmount,
    t.grandTotal,
    t.paidAmount,
    t.changeAmount,
    t.status,
  ].map(v => `"${v}"`).join(','));

  const csv = [headers.join(','), ...lines].join('\n');
  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = `pos-transactions-${new Date().toISOString().slice(0, 10)}.csv`;
  a.click();
  URL.revokeObjectURL(url);
}

// ── Transaction Detail Modal ──────────────────────────────────────────────

function TxDetailModal({
  txn,
  onClose,
  onVoid,
}: {
  txn: PosTransactionDetail;
  onClose: () => void;
  onVoid: (t: PosTransaction) => void;
}) {
  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-background rounded-2xl shadow-2xl w-full max-w-2xl max-h-[88vh] flex flex-col overflow-hidden">
        {/* Header */}
        <div className="flex items-center justify-between px-5 py-4 border-b shrink-0">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 rounded-xl bg-primary/10 flex items-center justify-center">
              <Receipt className="w-5 h-5 text-primary" />
            </div>
            <div>
              <h2 className="text-base font-bold font-mono">{txn.receiptNumber}</h2>
              <p className="text-xs text-muted-foreground">
                {txn.saleDate ? fmtDate(txn.saleDate) : ''}
              </p>
            </div>
          </div>
          <div className="flex items-center gap-2">
            <StatusBadge status={txn.status} />
            {txn.status === 'Completed' && (
              <Button
                variant="outline"
                size="sm"
                className="h-8 text-xs text-red-600 border-red-200 hover:bg-red-50"
                onClick={() => onVoid(txn)}
              >
                <XCircle className="w-3.5 h-3.5 mr-1" /> Void
              </Button>
            )}
            <Button variant="ghost" size="icon" onClick={onClose}>
              <X className="w-4 h-4" />
            </Button>
          </div>
        </div>

        <div className="flex-1 overflow-auto">
          {/* Meta info */}
          <div className="grid grid-cols-3 gap-4 p-5 border-b bg-secondary/10 text-sm">
            <div>
              <p className="text-xs text-muted-foreground uppercase tracking-wide font-semibold mb-0.5">Cashier</p>
              <p className="font-medium">{txn.cashierName || '—'}</p>
            </div>
            <div>
              <p className="text-xs text-muted-foreground uppercase tracking-wide font-semibold mb-0.5">Customer</p>
              <p className="font-medium">{txn.customerName || 'Walk-in'}</p>
              {txn.customerPhone && <p className="text-xs text-muted-foreground">{txn.customerPhone}</p>}
            </div>
            <div>
              <p className="text-xs text-muted-foreground uppercase tracking-wide font-semibold mb-0.5">Warehouse</p>
              <p className="font-medium">{txn.warehouseName || '—'}</p>
            </div>
            <div>
              <p className="text-xs text-muted-foreground uppercase tracking-wide font-semibold mb-0.5">Sale Type</p>
              <p className="font-medium">{txn.saleType}</p>
            </div>
            <div>
              <p className="text-xs text-muted-foreground uppercase tracking-wide font-semibold mb-0.5">Items</p>
              <p className="font-medium">{txn.totalItemQuantity}</p>
            </div>
            {txn.notes && (
              <div>
                <p className="text-xs text-muted-foreground uppercase tracking-wide font-semibold mb-0.5">Notes</p>
                <p className="font-medium">{txn.notes}</p>
              </div>
            )}
          </div>

          {/* Void reason */}
          {txn.voidReason && (
            <div className="mx-5 mt-4 p-3 rounded-xl bg-red-50 border border-red-200 dark:bg-red-950 dark:border-red-800 text-sm">
              <span className="font-semibold text-red-700 dark:text-red-400">Void Reason: </span>
              <span className="text-red-700 dark:text-red-400">{txn.voidReason}</span>
            </div>
          )}

          {/* Line items */}
          <div className="p-5">
            <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide mb-3">
              Line Items
            </h3>
            <div className="rounded-xl border overflow-hidden">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Product</th>
                    <th>SKU</th>
                    <th className="text-right">Qty</th>
                    <th className="text-right">Unit Price</th>
                    <th className="text-right">Discount</th>
                    <th className="text-right">Tax</th>
                    <th className="text-right">Line Total</th>
                  </tr>
                </thead>
                <tbody>
                  {txn.lines?.map(l => (
                    <tr key={l.id}>
                      <td className="font-medium">{l.productName}</td>
                      <td className="text-muted-foreground text-xs font-mono">{l.sku || '—'}</td>
                      <td className="text-right">{l.quantity}</td>
                      <td className="text-right">{fmt(l.unitPrice)}</td>
                      <td className="text-right text-green-600">
                        {l.discountAmount > 0 ? `-${fmt(l.discountAmount)}` : '—'}
                      </td>
                      <td className="text-right text-muted-foreground">{fmt(l.taxAmount)}</td>
                      <td className="text-right font-semibold">{fmt(l.lineTotal)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Payments */}
          <div className="px-5 pb-5">
            <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide mb-3">
              Payments
            </h3>
            <div className="rounded-xl border overflow-hidden">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Method</th>
                    <th className="text-right">Amount</th>
                    <th>Card Last 4</th>
                  </tr>
                </thead>
                <tbody>
                  {txn.payments?.map(p => (
                    <tr key={p.id}>
                      <td>
                        <span className="flex items-center gap-1.5 font-medium text-sm">
                          <PaymentIcon method={p.paymentMethod} />
                          {p.paymentMethod}
                        </span>
                      </td>
                      <td className="text-right font-semibold">{fmt(p.amount)}</td>
                      <td className="text-muted-foreground">{p.cardLastFour ? `•••• ${p.cardLastFour}` : '—'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Totals summary */}
          <div className="px-5 pb-5">
            <div className="rounded-xl border bg-secondary/10 p-4 space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Subtotal</span>
                <span>{fmt(txn.subTotal)}</span>
              </div>
              {txn.discountAmount > 0 && (
                <div className="flex justify-between text-green-600">
                  <span>Discount</span>
                  <span>-{fmt(txn.discountAmount)}</span>
                </div>
              )}
              <div className="flex justify-between text-muted-foreground">
                <span>Tax</span>
                <span>{fmt(txn.totalTaxAmount)}</span>
              </div>
              {txn.roundOffAmount !== 0 && (
                <div className="flex justify-between text-muted-foreground">
                  <span>Round Off</span>
                  <span>{fmt(txn.roundOffAmount)}</span>
                </div>
              )}
              <div className="flex justify-between font-bold text-base border-t pt-2 mt-1">
                <span>Grand Total</span>
                <span className="text-primary">{fmt(txn.grandTotal)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Paid</span>
                <span className="font-medium">{fmt(txn.paidAmount)}</span>
              </div>
              {txn.changeAmount > 0 && (
                <div className="flex justify-between font-semibold text-green-600">
                  <span>Change</span>
                  <span>{fmt(txn.changeAmount)}</span>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

// ── Void Modal ────────────────────────────────────────────────────────────

function VoidModal({
  txn,
  onClose,
  onConfirm,
  saving,
}: {
  txn: PosTransaction;
  onClose: () => void;
  onConfirm: (reason: string) => void;
  saving: boolean;
}) {
  const [reason, setReason] = useState('');
  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-[60] p-4">
      <div className="bg-background rounded-2xl shadow-2xl w-full max-w-sm overflow-hidden">
        <div className="bg-destructive/10 border-b border-destructive/20 px-5 py-4">
          <h2 className="text-lg font-semibold text-destructive flex items-center gap-2">
            <XCircle className="w-5 h-5" /> Void Transaction
          </h2>
          <p className="text-sm text-muted-foreground mt-0.5 font-mono">{txn.receiptNumber}</p>
        </div>
        <div className="p-5 space-y-4">
          <div>
            <label className="text-sm font-semibold block mb-1.5">
              Reason <span className="text-destructive">*</span>
            </label>
            <textarea
              value={reason}
              onChange={e => setReason(e.target.value)}
              placeholder="Enter reason for voiding this transaction..."
              className="nx-input w-full h-24 resize-none"
              autoFocus
            />
          </div>
          <div className="flex gap-2">
            <Button variant="outline" className="flex-1" onClick={onClose}>Cancel</Button>
            <Button
              variant="destructive"
              className="flex-1"
              onClick={() => onConfirm(reason)}
              disabled={saving || !reason.trim()}
            >
              {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
              Void Transaction
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}

// ── Main Component ────────────────────────────────────────────────────────

export default function PosTransactions() {
  // Data
  const [transactions, setTransactions] = useState<PosTransaction[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [totalCount, setTotalCount] = useState(0);

  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 15;

  // Filters
  const [search, setSearch] = useState('');
  const [filterWarehouse, setFilterWarehouse] = useState('');
  const [filterStatus, setFilterStatus] = useState<TxStatus>('');
  const [filterCashier, setFilterCashier] = useState('');
  const [filterStart, setFilterStart] = useState('');
  const [filterEnd, setFilterEnd] = useState('');

  // Selected detail
  const [detailTxn, setDetailTxn] = useState<PosTransactionDetail | null>(null);
  const [detailLoading, setDetailLoading] = useState(false);

  // Void
  const [voidTarget, setVoidTarget] = useState<PosTransaction | null>(null);
  const [voidSaving, setVoidSaving] = useState(false);

  // ── Fetching ──────────────────────────────────────────────────────────

  const fetchTransactions = useCallback(async () => {
    setLoading(true);
    try {
      const res = await posTransactionApiV2.getAll({
        pageIndex: currentPage - 1,
        pageSize,
        search: search || undefined,
        warehouseId: filterWarehouse || undefined,
        status: filterStatus || undefined,
        startDate: filterStart || undefined,
        endDate: filterEnd || undefined,
      });
      const data = res.data as unknown as { items: PosTransaction[]; totalCount: number };
      setTransactions(data?.items || []);
      setTotalCount(data?.totalCount || 0);
    } catch {
      toast.error('Failed to load transactions');
    } finally {
      setLoading(false);
    }
  }, [currentPage, search, filterWarehouse, filterStatus, filterStart, filterEnd]);

  useEffect(() => { fetchTransactions(); }, [fetchTransactions]);

  useEffect(() => {
    warehouseApiV2.getAll({ pageSize: 100 }).then(res => {
      setWarehouses((res.data as unknown as { items: Warehouse[] })?.items || []);
    }).catch(() => { /* ignore */ });
  }, []);

  const applyFilters = () => { setCurrentPage(1); fetchTransactions(); };

  const clearFilters = () => {
    setSearch('');
    setFilterWarehouse('');
    setFilterStatus('');
    setFilterCashier('');
    setFilterStart('');
    setFilterEnd('');
    setCurrentPage(1);
  };

  const hasFilters = search || filterWarehouse || filterStatus || filterCashier || filterStart || filterEnd;

  // ── Detail ────────────────────────────────────────────────────────────

  const viewDetail = async (t: PosTransaction) => {
    setDetailLoading(true);
    try {
      const res = await posTransactionApiV2.getById(t.id);
      setDetailTxn(res.data as unknown as PosTransactionDetail);
    } catch {
      toast.error('Failed to load transaction details');
    } finally {
      setDetailLoading(false);
    }
  };

  // ── Void ──────────────────────────────────────────────────────────────

  const handleVoid = async (reason: string) => {
    if (!voidTarget) return;
    setVoidSaving(true);
    try {
      await posTransactionApiV2.void(voidTarget.id, { voidedBy: 'admin', reason });
      toast.success('Transaction voided');
      setVoidTarget(null);
      setDetailTxn(null);
      fetchTransactions();
    } catch {
      toast.error('Void failed');
    } finally {
      setVoidSaving(false);
    }
  };

  // ── Summary totals ────────────────────────────────────────────────────

  const completedTxns = transactions.filter(t => t.status === 'Completed');
  const summarySubtotal  = completedTxns.reduce((s, t) => s + t.subTotal, 0);
  const summaryDiscount  = completedTxns.reduce((s, t) => s + t.discountAmount, 0);
  const summaryTotal     = completedTxns.reduce((s, t) => s + t.grandTotal, 0);
  const summaryItems     = completedTxns.reduce((s, t) => s + t.totalItemQuantity, 0);

  // ── CSV ───────────────────────────────────────────────────────────────

  const handleExport = () => {
    if (transactions.length === 0) { toast.error('No data to export'); return; }
    exportCSV(transactions);
    toast.success(`Exported ${transactions.length} rows`);
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-6">
      {/* Page header */}
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">POS Transactions</h1>
          <p className="nx-page-subtitle">Complete history of all sales transactions</p>
        </div>
        <div className="nx-page-actions">
          <Button variant="outline" size="sm" onClick={handleExport}>
            <Download className="w-4 h-4 mr-2" /> Export CSV
          </Button>
          <Button variant="outline" size="sm" onClick={() => { setCurrentPage(1); fetchTransactions(); }}>
            <RefreshCw className="w-4 h-4 mr-2" /> Refresh
          </Button>
        </div>
      </div>

      {/* Summary stat cards */}
      {completedTxns.length > 0 && (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <div className="nx-stat-card">
            <div className="flex items-center gap-2 mb-2">
              <ShoppingBag className="w-4 h-4 text-blue-500" />
              <span className="text-xs text-muted-foreground font-medium uppercase tracking-wide">Transactions</span>
            </div>
            <div className="nx-stat-value info">{completedTxns.length}</div>
            <div className="nx-stat-label">{summaryItems} items sold</div>
          </div>
          <div className="nx-stat-card">
            <div className="flex items-center gap-2 mb-2">
              <TrendingUp className="w-4 h-4 text-green-500" />
              <span className="text-xs text-muted-foreground font-medium uppercase tracking-wide">Revenue</span>
            </div>
            <div className="nx-stat-value success">{fmt(summaryTotal)}</div>
            <div className="nx-stat-label">Grand total</div>
          </div>
          <div className="nx-stat-card">
            <div className="flex items-center gap-2 mb-2">
              <Calendar className="w-4 h-4 text-amber-500" />
              <span className="text-xs text-muted-foreground font-medium uppercase tracking-wide">Subtotal</span>
            </div>
            <div className="nx-stat-value">{fmt(summarySubtotal)}</div>
            <div className="nx-stat-label">Before tax &amp; discount</div>
          </div>
          <div className="nx-stat-card">
            <div className="flex items-center gap-2 mb-2">
              <Filter className="w-4 h-4 text-purple-500" />
              <span className="text-xs text-muted-foreground font-medium uppercase tracking-wide">Discounts</span>
            </div>
            <div className="nx-stat-value" style={{ color: 'var(--nx-purple)' }}>{fmt(summaryDiscount)}</div>
            <div className="nx-stat-label">Total saved</div>
          </div>
        </div>
      )}

      {/* Main card */}
      <div className="nx-card">
        {/* Filters toolbar */}
        <div className="p-4 border-b space-y-3">
          {/* Top row */}
          <div className="flex flex-wrap items-center gap-3">
            <div className="nx-table-search flex-1 min-w-[200px] max-w-xs">
              <Search className="w-4 h-4 shrink-0" />
              <input
                type="text"
                placeholder="Search receipt #..."
                value={search}
                onChange={e => setSearch(e.target.value)}
                onKeyDown={e => e.key === 'Enter' && applyFilters()}
              />
            </div>

            <select
              className="nx-input nx-select w-40"
              value={filterWarehouse}
              onChange={e => setFilterWarehouse(e.target.value)}
            >
              <option value="">All Warehouses</option>
              {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
            </select>

            <select
              className="nx-input nx-select w-36"
              value={filterStatus}
              onChange={e => setFilterStatus(e.target.value as TxStatus)}
            >
              <option value="">All Statuses</option>
              <option value="Completed">Completed</option>
              <option value="Held">Held</option>
              <option value="Voided">Voided</option>
            </select>

            <Input
              type="text"
              placeholder="Cashier..."
              className="h-9 w-32 text-sm"
              value={filterCashier}
              onChange={e => setFilterCashier(e.target.value)}
            />
          </div>

          {/* Date row */}
          <div className="flex flex-wrap items-center gap-3">
            <div className="flex items-center gap-2">
              <Calendar className="w-4 h-4 text-muted-foreground" />
              <span className="text-sm font-medium text-muted-foreground">Date Range:</span>
            </div>
            <input
              type="date"
              className="nx-input h-9 w-38 text-sm"
              style={{ width: '10rem' }}
              value={filterStart}
              onChange={e => setFilterStart(e.target.value)}
            />
            <span className="text-muted-foreground text-sm">to</span>
            <input
              type="date"
              className="nx-input h-9 w-38 text-sm"
              style={{ width: '10rem' }}
              value={filterEnd}
              onChange={e => setFilterEnd(e.target.value)}
            />

            <Button size="sm" onClick={applyFilters} className="h-9">
              <Filter className="w-4 h-4 mr-2" /> Apply
            </Button>

            {hasFilters && (
              <Button variant="ghost" size="sm" className="h-9 text-muted-foreground" onClick={clearFilters}>
                <X className="w-4 h-4 mr-1" /> Clear
              </Button>
            )}

            <span className="text-sm text-muted-foreground ml-auto">
              {totalCount.toLocaleString()} transaction{totalCount !== 1 ? 's' : ''}
            </span>
          </div>
        </div>

        {/* Table */}
        {loading ? (
          <div className="flex items-center justify-center py-16">
            <Loader2 className="w-8 h-8 animate-spin text-muted-foreground" />
          </div>
        ) : transactions.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-16 text-muted-foreground">
            <Receipt className="w-12 h-12 mb-3 opacity-20" />
            <p className="font-medium">No transactions found</p>
            <p className="text-sm mt-1">Try adjusting your filters</p>
          </div>
        ) : (
          <>
            <div className="overflow-auto">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Receipt #</th>
                    <th>Date &amp; Time</th>
                    <th>Customer</th>
                    <th>Cashier</th>
                    <th className="text-right">Items</th>
                    <th className="text-right">Subtotal</th>
                    <th className="text-right">Discount</th>
                    <th className="text-right">Total</th>
                    <th>Payment</th>
                    <th>Status</th>
                    <th className="text-center" style={{ width: 90 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {transactions.map(t => (
                    <tr
                      key={t.id}
                      className="cursor-pointer"
                      onClick={() => viewDetail(t)}
                    >
                      <td>
                        <code className="text-xs bg-secondary px-2 py-1 rounded font-mono">
                          {t.receiptNumber}
                        </code>
                      </td>
                      <td className="text-sm">
                        <div className="font-medium">
                          {t.saleDate ? fmtDateShort(t.saleDate) : '—'}
                        </div>
                        <div className="text-xs text-muted-foreground">
                          {t.saleDate ? new Date(t.saleDate).toLocaleTimeString('en-BD', { hour: '2-digit', minute: '2-digit' }) : ''}
                        </div>
                      </td>
                      <td>
                        <span className="text-sm">{t.customerName || <span className="text-muted-foreground italic">Walk-in</span>}</span>
                      </td>
                      <td className="text-sm text-muted-foreground">{t.cashierName || '—'}</td>
                      <td className="text-right font-medium">{t.totalItemQuantity}</td>
                      <td className="text-right text-muted-foreground">{fmt(t.subTotal)}</td>
                      <td className="text-right text-green-600">
                        {t.discountAmount > 0 ? `-${fmt(t.discountAmount)}` : '—'}
                      </td>
                      <td className="text-right font-bold">{fmt(t.grandTotal)}</td>
                      <td className="text-sm text-muted-foreground">—</td>
                      <td>
                        <StatusBadge status={t.status} />
                      </td>
                      <td onClick={e => e.stopPropagation()}>
                        <div className="flex items-center justify-center gap-1">
                          <Button
                            variant="ghost"
                            size="icon"
                            className="w-7 h-7"
                            onClick={() => viewDetail(t)}
                            title="View details"
                            disabled={detailLoading}
                          >
                            {detailLoading ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Eye className="w-3.5 h-3.5" />}
                          </Button>
                          {t.status === 'Completed' && (
                            <Button
                              variant="ghost"
                              size="icon"
                              className="w-7 h-7 text-red-500 hover:text-red-700"
                              onClick={() => setVoidTarget(t)}
                              title="Void"
                            >
                              <XCircle className="w-3.5 h-3.5" />
                            </Button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>

                {/* Summary totals row */}
                {completedTxns.length > 0 && (
                  <tfoot>
                    <tr className="bg-secondary/40 font-semibold">
                      <td colSpan={4} className="px-4 py-2.5 text-sm">
                        Page Totals ({completedTxns.length} completed)
                      </td>
                      <td className="text-right px-4 py-2.5 text-sm">{summaryItems}</td>
                      <td className="text-right px-4 py-2.5 text-sm">{fmt(summarySubtotal)}</td>
                      <td className="text-right px-4 py-2.5 text-sm text-green-600">
                        {summaryDiscount > 0 ? `-${fmt(summaryDiscount)}` : '—'}
                      </td>
                      <td className="text-right px-4 py-2.5 text-sm text-primary">{fmt(summaryTotal)}</td>
                      <td colSpan={3} />
                    </tr>
                  </tfoot>
                )}
              </table>
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="flex items-center justify-between px-4 py-3 border-t">
                <p className="text-sm text-muted-foreground">
                  Showing {transactions.length} of {totalCount.toLocaleString()}
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

      {/* Detail modal */}
      {detailTxn && (
        <TxDetailModal
          txn={detailTxn}
          onClose={() => setDetailTxn(null)}
          onVoid={t => { setVoidTarget(t); }}
        />
      )}

      {/* Void modal */}
      {voidTarget && (
        <VoidModal
          txn={voidTarget}
          onClose={() => setVoidTarget(null)}
          onConfirm={handleVoid}
          saving={voidSaving}
        />
      )}
    </div>
  );
}
