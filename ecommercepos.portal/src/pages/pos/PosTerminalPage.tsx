import { useState, useEffect, useCallback, useRef } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Minus, Trash2, ShoppingCart, Search, Loader2, Package,
  Calculator, Pause, RotateCcw, XCircle, DollarSign, CreditCard,
  Smartphone, Wallet, X, User, Clock, CheckCircle2,
} from 'lucide-react';
import { productApi, type Product } from '@/api/productApi';
import { categoryApi } from '@/api/categoryApi';
import { customerApi, type Customer } from '@/api/customerApi';
import {
  cashShiftApi, posCounterApi, posTransactionApiV2, warehouseApiV2,
  type CashShift, type PosCounter, type Warehouse, type PosTransaction,
} from '@/api/posApi';
import toast from 'react-hot-toast';

// ── Types ──────────────────────────────────────────────────────────────────

interface CartItem {
  productId: string;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  lineTotal: number;
}

interface SelectedCustomer {
  id: string;
  code: string;
  phone: string;
}

// ── Constants ─────────────────────────────────────────────────────────────

const PAYMENT_METHODS = [
  { key: 'CASH',   label: 'Cash',   icon: DollarSign  },
  { key: 'CARD',   label: 'Card',   icon: CreditCard  },
  { key: 'MOBILE', label: 'Mobile', icon: Smartphone  },
  { key: 'WALLET', label: 'Wallet', icon: Wallet      },
];

const TAX_RATE = 0.05;

// ── Sub-components ────────────────────────────────────────────────────────

function LiveClock() {
  const [now, setNow] = useState(new Date());
  useEffect(() => {
    const t = setInterval(() => setNow(new Date()), 1000);
    return () => clearInterval(t);
  }, []);
  return (
    <span className="font-mono text-sm tabular-nums">
      {now.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
    </span>
  );
}

// ── Main Component ────────────────────────────────────────────────────────

export default function PosTerminalPage() {
  // Shift state
  const [activeShift, setActiveShift] = useState<CashShift | null>(null);
  const [showOpenShift, setShowOpenShift] = useState(false);
  const [showCloseShift, setShowCloseShift] = useState(false);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [counters, setCounters] = useState<PosCounter[]>([]);
  const [shiftForm, setShiftForm] = useState({ warehouseId: '', posCounterId: '', openingCash: '0' });
  const [closeShiftForm, setCloseShiftForm] = useState({ closingCash: '0', notes: '' });
  const [shiftLoading, setShiftLoading] = useState(true);

  // Products state
  const [products, setProducts] = useState<Product[]>([]);
  const [prodLoading, setProdLoading] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [activeCategory, setActiveCategory] = useState('');
  const [categories, setCategories] = useState<{ id: string; name: string }[]>([]);

  // Customer state
  const [customerSearch, setCustomerSearch] = useState('');
  const [customerResults, setCustomerResults] = useState<Customer[]>([]);
  const [selectedCustomer, setSelectedCustomer] = useState<SelectedCustomer | null>(null);
  const [showCustomerDropdown, setShowCustomerDropdown] = useState(false);
  const [walkInName, setWalkInName] = useState('');
  const customerRef = useRef<HTMLDivElement>(null);

  // Cart state
  const [cart, setCart] = useState<CartItem[]>([]);
  const [selectedPayment, setSelectedPayment] = useState('CASH');
  const [amountReceived, setAmountReceived] = useState('');
  const [saving, setSaving] = useState(false);

  // Held transactions
  const [showHeld, setShowHeld] = useState(false);
  const [heldTxns, setHeldTxns] = useState<PosTransaction[]>([]);

  // Receipt
  const [showReceipt, setShowReceipt] = useState(false);
  const [lastTxn, setLastTxn] = useState<PosTransaction | null>(null);

  // Void dialog
  const [showVoidDialog, setShowVoidDialog] = useState(false);
  const [voidReason, setVoidReason] = useState('');

  // ── Init ────────────────────────────────────────────────────────────────

  useEffect(() => {
    checkActiveShift();
    fetchCategories();
  }, []);

  // Close customer dropdown on outside click
  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (customerRef.current && !customerRef.current.contains(e.target as Node)) {
        setShowCustomerDropdown(false);
      }
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, []);

  const checkActiveShift = async () => {
    setShiftLoading(true);
    try {
      const res = await cashShiftApi.getActive();
      const shifts = res.data as unknown as CashShift[];
      if (Array.isArray(shifts) && shifts.length > 0) {
        setActiveShift(shifts[0]);
      } else {
        setShowOpenShift(true);
        await loadShiftOptions();
      }
    } catch {
      setShowOpenShift(true);
      await loadShiftOptions();
    } finally {
      setShiftLoading(false);
    }
  };

  const loadShiftOptions = async () => {
    try {
      const [wRes, cRes] = await Promise.all([
        warehouseApiV2.getAll({ pageSize: 100 }),
        posCounterApi.getAll({ pageSize: 100 }),
      ]);
      setWarehouses((wRes.data as unknown as { items: Warehouse[] })?.items || []);
      setCounters((cRes.data as unknown as { items: PosCounter[] })?.items || []);
    } catch { /* ignore */ }
  };

  // ── Shift actions ────────────────────────────────────────────────────────

  const openShift = async () => {
    if (!shiftForm.warehouseId || !shiftForm.posCounterId) {
      toast.error('Select a warehouse and counter first');
      return;
    }
    setSaving(true);
    try {
      const res = await cashShiftApi.open({
        warehouseId: shiftForm.warehouseId,
        posCounterId: shiftForm.posCounterId,
        openingCash: parseFloat(shiftForm.openingCash) || 0,
      });
      setActiveShift(res.data as unknown as CashShift);
      setShowOpenShift(false);
      toast.success('Shift opened — ready to sell!');
    } catch {
      toast.error('Failed to open shift');
    } finally {
      setSaving(false);
    }
  };

  const handleCloseShift = async () => {
    if (!activeShift) return;
    setSaving(true);
    try {
      await cashShiftApi.close(activeShift.id, {
        closingCash: parseFloat(closeShiftForm.closingCash) || 0,
        notes: closeShiftForm.notes || undefined,
      });
      setActiveShift(null);
      setShowCloseShift(false);
      setShowOpenShift(true);
      await loadShiftOptions();
      clearCart();
      toast.success('Shift closed successfully');
    } catch {
      toast.error('Failed to close shift');
    } finally {
      setSaving(false);
    }
  };

  // ── Products ─────────────────────────────────────────────────────────────

  const fetchCategories = async () => {
    try {
      const res = await categoryApi.getFlat();
      const items = res.data?.items || [];
      setCategories(items.map(c => ({ id: c.id, name: c.name || '' })));
    } catch { /* ignore */ }
  };

  const fetchProducts = useCallback(async (q?: string, catId?: string) => {
    setProdLoading(true);
    try {
      const res = await productApi.getAll({
        pageIndex: 0,
        pageSize: 60,
        search: q || undefined,
        categoryId: catId || undefined,
        isActive: true,
      });
      if (res.data?.items) setProducts(res.data.items);
    } catch { /* ignore */ }
    finally { setProdLoading(false); }
  }, []);

  useEffect(() => {
    if (!activeShift) return;
    const t = setTimeout(() => fetchProducts(searchQuery, activeCategory), 300);
    return () => clearTimeout(t);
  }, [searchQuery, activeCategory, activeShift, fetchProducts]);

  useEffect(() => {
    if (activeShift) fetchProducts();
  }, [activeShift, fetchProducts]);

  // ── Customer search ──────────────────────────────────────────────────────

  useEffect(() => {
    if (customerSearch.length < 2) { setCustomerResults([]); return; }
    const t = setTimeout(async () => {
      try {
        const res = await customerApi.getAll({ search: customerSearch, pageSize: 6 });
        const items = (res.data as unknown as { data: { items: Customer[] } })?.data?.items || [];
        setCustomerResults(items);
        setShowCustomerDropdown(items.length > 0);
      } catch { /* ignore */ }
    }, 300);
    return () => clearTimeout(t);
  }, [customerSearch]);

  // ── Cart helpers ─────────────────────────────────────────────────────────

  const addToCart = (p: Product) => {
    setCart(prev => {
      const existing = prev.find(c => c.productId === p.id);
      if (existing) {
        return prev.map(c => c.productId === p.id
          ? { ...c, quantity: c.quantity + 1, lineTotal: (c.quantity + 1) * c.unitPrice }
          : c
        );
      }
      return [...prev, {
        productId: p.id,
        productName: p.productName || '',
        sku: p.sku || '',
        quantity: 1,
        unitPrice: p.sellPrice || 0,
        discount: 0,
        lineTotal: p.sellPrice || 0,
      }];
    });
  };

  const updateQty = (productId: string, qty: number) => {
    if (qty <= 0) { removeItem(productId); return; }
    setCart(prev => prev.map(c => c.productId === productId
      ? { ...c, quantity: qty, lineTotal: qty * c.unitPrice * (1 - c.discount / 100) }
      : c
    ));
  };

  const removeItem = (productId: string) =>
    setCart(prev => prev.filter(c => c.productId !== productId));

  const clearCart = () => {
    setCart([]);
    setSelectedCustomer(null);
    setCustomerSearch('');
    setWalkInName('');
    setAmountReceived('');
  };

  // ── Calculations ──────────────────────────────────────────────────────────

  const subtotal = cart.reduce((s, c) => s + c.lineTotal, 0);
  const tax      = subtotal * TAX_RATE;
  const grandTotal = subtotal + tax;
  const amountNum  = parseFloat(amountReceived) || 0;
  const change     = amountNum - grandTotal;

  // ── Quick-amount presets for cash ─────────────────────────────────────────

  const quickAmounts = (() => {
    if (grandTotal <= 0) return [];
    const steps = [10, 50, 100, 500, 1000];
    const result: number[] = [];
    for (const step of steps) {
      const v = Math.ceil(grandTotal / step) * step;
      if (!result.includes(v) && result.length < 4) result.push(v);
    }
    return result.slice(0, 4);
  })();

  // ── Actions ───────────────────────────────────────────────────────────────

  const completeSale = async () => {
    if (!activeShift || cart.length === 0) return;
    if (selectedPayment === 'CASH' && amountNum > 0 && amountNum < grandTotal) {
      toast.error('Amount received is less than total');
      return;
    }
    setSaving(true);
    try {
      const paidAmount = selectedPayment === 'CASH' && amountNum > 0 ? amountNum : grandTotal;
      const res = await posTransactionApiV2.create({
        cashShiftId: activeShift.id,
        posCounterId: activeShift.posCounterId,
        customerId: selectedCustomer?.id,
        customerName: selectedCustomer?.code || walkInName || undefined,
        customerPhone: selectedCustomer?.phone || undefined,
        saleType: 'REGULAR',
        lines: cart.map(c => ({
          productId: c.productId,
          productName: c.productName,
          sku: c.sku,
          quantity: c.quantity,
          unitPrice: c.unitPrice,
          discountAmount: c.unitPrice * c.quantity * c.discount / 100,
          taxAmount: c.lineTotal * TAX_RATE,
          lineTotal: c.lineTotal * (1 + TAX_RATE),
        })),
        payments: [{ paymentMethod: selectedPayment, amount: paidAmount }],
      });
      setLastTxn(res.data as unknown as PosTransaction);
      setShowReceipt(true);
      clearCart();
      toast.success('Sale completed successfully!');
    } catch {
      toast.error('Checkout failed — please try again');
    } finally {
      setSaving(false);
    }
  };

  const holdSale = async () => {
    if (!activeShift || cart.length === 0) return;
    setSaving(true);
    try {
      await posTransactionApiV2.hold({
        cashShiftId: activeShift.id,
        posCounterId: activeShift.posCounterId,
        customerName: selectedCustomer?.code || walkInName || undefined,
        lines: cart.map(c => ({
          productId: c.productId,
          productName: c.productName,
          sku: c.sku,
          quantity: c.quantity,
          unitPrice: c.unitPrice,
          lineTotal: c.lineTotal,
        })),
      });
      clearCart();
      toast.success('Transaction held');
    } catch {
      toast.error('Hold failed');
    } finally {
      setSaving(false);
    }
  };

  const loadHeldTransactions = async () => {
    try {
      const res = await posTransactionApiV2.getAll({ status: 'HELD', pageSize: 50 });
      const data = res.data as unknown as { items: PosTransaction[] };
      setHeldTxns(data?.items || []);
      setShowHeld(true);
    } catch { toast.error('Failed to load held transactions'); }
  };

  const resumeTransaction = async (id: string) => {
    try {
      const res = await posTransactionApiV2.resume(id);
      const detail = res.data as unknown as { lines: CartItem[] };
      if (detail?.lines) {
        setCart(detail.lines.map(l => ({
          productId: (l as unknown as { productId?: string }).productId || l.sku || '',
          productName: l.productName,
          sku: l.sku,
          quantity: l.quantity,
          unitPrice: l.unitPrice,
          discount: 0,
          lineTotal: l.lineTotal,
        })));
      }
      setShowHeld(false);
      toast.success('Transaction resumed');
    } catch {
      toast.error('Resume failed');
    }
  };

  const handleVoid = () => {
    if (!voidReason.trim()) return;
    clearCart();
    setShowVoidDialog(false);
    setVoidReason('');
    toast.success('Transaction voided');
  };

  const fmt = (n: number) =>
    n.toLocaleString('en-BD', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

  // ── Loading screen ────────────────────────────────────────────────────────

  if (shiftLoading) {
    return (
      <div className="flex items-center justify-center h-[calc(100vh-5rem)]">
        <div className="text-center">
          <Loader2 className="w-12 h-12 animate-spin text-primary mx-auto mb-4" />
          <p className="text-muted-foreground font-medium">Loading POS Terminal...</p>
        </div>
      </div>
    );
  }

  // ── Open Shift screen ─────────────────────────────────────────────────────

  if (showOpenShift && !activeShift) {
    return (
      <div className="flex items-center justify-center h-[calc(100vh-5rem)] bg-secondary/30">
        <div className="bg-background border rounded-2xl shadow-2xl w-full max-w-md overflow-hidden">
          <div className="bg-primary px-6 py-5 text-primary-foreground">
            <div className="flex items-center gap-3">
              <div className="w-11 h-11 bg-primary-foreground/15 rounded-xl flex items-center justify-center shrink-0">
                <Calculator className="w-6 h-6" />
              </div>
              <div>
                <h2 className="text-xl font-bold">Open Cash Shift</h2>
                <p className="text-primary-foreground/70 text-sm">Start your selling session</p>
              </div>
            </div>
          </div>

          <div className="p-6 space-y-4">
            <div>
              <label className="text-sm font-semibold block mb-1.5">
                Warehouse <span className="text-destructive">*</span>
              </label>
              <select
                className="nx-input nx-select w-full"
                value={shiftForm.warehouseId}
                onChange={e => setShiftForm({ ...shiftForm, warehouseId: e.target.value, posCounterId: '' })}
              >
                <option value="">Select warehouse...</option>
                {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
              </select>
            </div>

            <div>
              <label className="text-sm font-semibold block mb-1.5">
                POS Counter <span className="text-destructive">*</span>
              </label>
              <select
                className="nx-input nx-select w-full"
                value={shiftForm.posCounterId}
                onChange={e => setShiftForm({ ...shiftForm, posCounterId: e.target.value })}
              >
                <option value="">Select counter...</option>
                {counters
                  .filter(c => !shiftForm.warehouseId || c.warehouseId === shiftForm.warehouseId)
                  .map(c => <option key={c.id} value={c.id}>{c.counterName}</option>)
                }
              </select>
            </div>

            <div>
              <label className="text-sm font-semibold block mb-1.5">Opening Cash (BDT)</label>
              <div className="relative">
                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground font-semibold">৳</span>
                <Input
                  type="number"
                  className="pl-7"
                  value={shiftForm.openingCash}
                  onChange={e => setShiftForm({ ...shiftForm, openingCash: e.target.value })}
                  placeholder="0.00"
                />
              </div>
            </div>

            <Button
              className="w-full h-11 text-base font-semibold"
              onClick={openShift}
              disabled={saving || !shiftForm.warehouseId || !shiftForm.posCounterId}
            >
              {saving
                ? <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                : <CheckCircle2 className="w-5 h-5 mr-2" />
              }
              Open Shift &amp; Start Selling
            </Button>
          </div>
        </div>
      </div>
    );
  }

  // ── Main POS layout ───────────────────────────────────────────────────────

  return (
    <div className="flex h-[calc(100vh-5rem)] -mx-6 -mb-6">

      {/* ══ LEFT PANEL ══════════════════════════════════════════════════════ */}
      <div className="flex-1 flex flex-col bg-secondary/10 min-w-0 overflow-hidden">

        {/* Shift Info Bar */}
        <div className="flex items-center justify-between px-4 py-2 bg-background border-b shrink-0">
          <div className="flex items-center gap-3 text-sm">
            <span className="flex items-center gap-1.5">
              <span className="w-2 h-2 rounded-full bg-green-500 animate-pulse" />
              <span className="text-xs font-bold text-green-600 tracking-wide">SHIFT OPEN</span>
            </span>
            {activeShift && (
              <>
                <span className="text-border">|</span>
                <span className="text-muted-foreground font-medium">{activeShift.warehouseName}</span>
                <span className="text-border">·</span>
                <span className="text-muted-foreground">{activeShift.posCounterName}</span>
                <span className="text-border">·</span>
                <span className="font-semibold">{activeShift.openedByName}</span>
              </>
            )}
          </div>
          <div className="flex items-center gap-3">
            <div className="flex items-center gap-1.5 text-muted-foreground text-xs">
              <Clock className="w-3.5 h-3.5" />
              <LiveClock />
              <span className="hidden sm:inline">
                · {new Date().toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })}
              </span>
            </div>
            <Button
              variant="outline"
              size="sm"
              className="h-7 text-xs gap-1"
              onClick={loadHeldTransactions}
            >
              <Pause className="w-3 h-3" /> Held
            </Button>
            <Button
              variant="outline"
              size="sm"
              className="h-7 text-xs text-red-600 border-red-200 hover:bg-red-50 dark:hover:bg-red-950"
              onClick={() => {
                setCloseShiftForm({ closingCash: '0', notes: '' });
                setShowCloseShift(true);
              }}
            >
              Close Shift
            </Button>
          </div>
        </div>

        {/* Category Tabs */}
        <div className="flex items-stretch border-b bg-background overflow-x-auto scrollbar-none shrink-0">
          {(['', ...categories.map(c => c.id)] as string[]).map(catId => {
            const label = catId === '' ? 'All Items' : (categories.find(c => c.id === catId)?.name ?? '');
            const active = activeCategory === catId;
            return (
              <button
                key={catId}
                onClick={() => setActiveCategory(catId)}
                className={`shrink-0 px-4 py-2.5 text-sm font-medium border-b-2 whitespace-nowrap transition-colors ${
                  active
                    ? 'border-primary text-primary'
                    : 'border-transparent text-muted-foreground hover:text-foreground'
                }`}
              >
                {label}
              </button>
            );
          })}
        </div>

        {/* Search */}
        <div className="px-4 py-2.5 bg-background border-b shrink-0">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
            <Input
              placeholder="Search by name, SKU, or barcode..."
              className="pl-9 h-9"
              value={searchQuery}
              onChange={e => setSearchQuery(e.target.value)}
            />
            {searchQuery && (
              <button
                onClick={() => setSearchQuery('')}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
              >
                <X className="w-4 h-4" />
              </button>
            )}
          </div>
        </div>

        {/* Product Grid */}
        <div className="flex-1 overflow-auto p-4">
          {prodLoading ? (
            <div className="flex items-center justify-center h-40">
              <Loader2 className="w-8 h-8 animate-spin text-muted-foreground" />
            </div>
          ) : products.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-48 text-muted-foreground">
              <Package className="w-14 h-14 mb-3 opacity-20" />
              <p className="font-semibold text-base">No products found</p>
              <p className="text-sm mt-1 opacity-70">Try a different search or category</p>
            </div>
          ) : (
            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-3">
              {products.map(p => {
                const inCart = cart.find(c => c.productId === p.id);
                const outOfStock = (p.quantity ?? 0) <= 0;
                const lowStock = !outOfStock && (p.quantity ?? 0) <= (p.reorderLevel ?? 0);
                return (
                  <button
                    key={p.id}
                    onClick={() => !outOfStock && addToCart(p)}
                    disabled={outOfStock}
                    className={[
                      'relative rounded-xl border bg-background p-3 text-left transition-all',
                      outOfStock
                        ? 'opacity-50 cursor-not-allowed'
                        : 'hover:shadow-md hover:border-primary/40 hover:-translate-y-0.5 active:translate-y-0 cursor-pointer',
                      inCart ? 'border-primary/70 ring-1 ring-primary/30 bg-primary/5' : '',
                    ].join(' ')}
                  >
                    {/* In-cart badge */}
                    {inCart && (
                      <span className="absolute top-2 right-2 z-10 w-5 h-5 rounded-full bg-primary text-primary-foreground text-xs font-bold flex items-center justify-center">
                        {inCart.quantity}
                      </span>
                    )}

                    {/* Product image */}
                    <div className="w-full aspect-square rounded-lg mb-2.5 bg-secondary flex items-center justify-center overflow-hidden">
                      {p.imageUrl
                        ? <img src={p.imageUrl} alt={p.productName} className="w-full h-full object-cover" />
                        : <Package className="w-8 h-8 text-muted-foreground/40" />
                      }
                    </div>

                    <p className="font-semibold text-xs leading-tight line-clamp-2 min-h-[2rem]">
                      {p.productName}
                    </p>
                    <p className="text-muted-foreground text-xs mt-0.5 truncate">{p.sku}</p>

                    <div className="flex items-center justify-between mt-1.5 gap-1">
                      <span className="text-sm font-bold text-primary">
                        ৳{(p.sellPrice || 0).toLocaleString()}
                      </span>
                      <span className={[
                        'text-xs px-1.5 py-0.5 rounded font-medium',
                        outOfStock ? 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400' :
                        lowStock   ? 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400' :
                                     'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400',
                      ].join(' ')}>
                        {outOfStock ? 'Out' : p.quantity}
                      </span>
                    </div>
                  </button>
                );
              })}
            </div>
          )}
        </div>
      </div>

      {/* ══ RIGHT PANEL (Cart) ═══════════════════════════════════════════════ */}
      <div className="w-[380px] shrink-0 flex flex-col bg-background border-l">

        {/* Customer section */}
        <div className="px-4 pt-3 pb-3 border-b shrink-0" ref={customerRef}>
          <div className="relative">
            <User className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none z-10" />

            {selectedCustomer ? (
              <div className="flex items-center gap-2 px-3 py-2 pl-9 border rounded-lg bg-secondary/30 text-sm">
                <div className="flex-1 min-w-0">
                  <span className="font-semibold">{selectedCustomer.code}</span>
                  <span className="text-muted-foreground ml-2 text-xs">{selectedCustomer.phone}</span>
                </div>
                <button
                  onClick={() => { setSelectedCustomer(null); setCustomerSearch(''); setWalkInName(''); }}
                  className="text-muted-foreground hover:text-foreground transition-colors"
                >
                  <X className="w-3.5 h-3.5" />
                </button>
              </div>
            ) : (
              <Input
                className="pl-9 h-9 text-sm"
                placeholder="Customer (search or walk-in name)..."
                value={customerSearch}
                onChange={e => { setCustomerSearch(e.target.value); setWalkInName(e.target.value); }}
                onFocus={() => customerResults.length > 0 && setShowCustomerDropdown(true)}
              />
            )}

            {/* Customer dropdown */}
            {showCustomerDropdown && customerResults.length > 0 && !selectedCustomer && (
              <div className="absolute left-0 right-0 top-full mt-1 bg-background border rounded-lg shadow-xl z-30 max-h-48 overflow-auto">
                {customerResults.map(c => (
                  <button
                    key={c.id}
                    className="w-full text-left px-3 py-2.5 hover:bg-secondary transition-colors text-sm flex items-center gap-3"
                    onClick={() => {
                      setSelectedCustomer({ id: c.id, code: c.customerCode, phone: c.phone });
                      setCustomerSearch('');
                      setShowCustomerDropdown(false);
                    }}
                  >
                    <div className="w-7 h-7 rounded-full bg-primary/15 text-primary flex items-center justify-center text-xs font-bold shrink-0">
                      {c.customerCode?.charAt(0) || 'C'}
                    </div>
                    <div className="min-w-0">
                      <p className="font-semibold truncate">{c.customerCode}</p>
                      <p className="text-muted-foreground text-xs">{c.phone}</p>
                    </div>
                  </button>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Cart items */}
        <div className="flex-1 overflow-auto">
          {cart.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-full text-muted-foreground py-10">
              <ShoppingCart className="w-14 h-14 mb-3 opacity-20" />
              <p className="font-semibold">Cart is empty</p>
              <p className="text-sm mt-1 opacity-70">Tap a product to add it</p>
            </div>
          ) : (
            <div className="divide-y">
              {cart.map(item => (
                <div key={item.productId} className="px-4 py-2.5 flex items-center gap-2 hover:bg-secondary/20 transition-colors">
                  <div className="flex-1 min-w-0">
                    <p className="font-medium text-sm leading-tight truncate">{item.productName}</p>
                    <p className="text-xs text-muted-foreground mt-0.5">৳{fmt(item.unitPrice)} each</p>
                  </div>

                  {/* Qty controls */}
                  <div className="flex items-center gap-1 shrink-0">
                    <button
                      onClick={() => updateQty(item.productId, item.quantity - 1)}
                      className="w-6 h-6 rounded border flex items-center justify-center hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                    >
                      <Minus className="w-3 h-3" />
                    </button>
                    <input
                      type="number"
                      min={1}
                      value={item.quantity}
                      onChange={e => updateQty(item.productId, parseInt(e.target.value) || 1)}
                      className="w-9 h-6 text-center text-xs font-bold border rounded bg-background focus:outline-none focus:ring-1 focus:ring-primary"
                    />
                    <button
                      onClick={() => updateQty(item.productId, item.quantity + 1)}
                      className="w-6 h-6 rounded border flex items-center justify-center hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                    >
                      <Plus className="w-3 h-3" />
                    </button>
                  </div>

                  <div className="text-right shrink-0 flex flex-col items-end gap-0.5 min-w-[60px]">
                    <span className="text-sm font-bold">৳{fmt(item.lineTotal)}</span>
                    <button
                      onClick={() => removeItem(item.productId)}
                      className="text-red-400 hover:text-red-600 transition-colors"
                    >
                      <Trash2 className="w-3 h-3" />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Totals + Payment (shown only when cart has items) */}
        {cart.length > 0 && (
          <div className="border-t shrink-0">

            {/* Totals */}
            <div className="px-4 py-2.5 space-y-1 border-b bg-secondary/10">
              <div className="flex justify-between text-sm text-muted-foreground">
                <span>{cart.length} line{cart.length !== 1 ? 's' : ''}</span>
                <span>Subtotal: ৳{fmt(subtotal)}</span>
              </div>
              <div className="flex justify-between text-sm text-muted-foreground">
                <span>Tax ({(TAX_RATE * 100).toFixed(0)}%)</span>
                <span>৳{fmt(tax)}</span>
              </div>
              <div className="flex justify-between text-lg font-bold border-t pt-1.5 mt-1">
                <span>TOTAL</span>
                <span className="text-primary">৳{fmt(grandTotal)}</span>
              </div>
            </div>

            {/* Payment method tabs */}
            <div className="px-4 py-2.5 border-b">
              <div className="grid grid-cols-4 gap-1.5">
                {PAYMENT_METHODS.map(m => {
                  const Icon = m.icon;
                  const active = selectedPayment === m.key;
                  return (
                    <button
                      key={m.key}
                      onClick={() => setSelectedPayment(m.key)}
                      className={[
                        'flex flex-col items-center gap-0.5 py-2 px-1 rounded-lg border text-xs font-medium transition-all',
                        active
                          ? 'bg-primary text-primary-foreground border-primary shadow-sm'
                          : 'text-muted-foreground border-input hover:bg-secondary',
                      ].join(' ')}
                    >
                      <Icon className="w-4 h-4" />
                      {m.label}
                    </button>
                  );
                })}
              </div>
            </div>

            {/* Cash received + change */}
            {selectedPayment === 'CASH' && (
              <div className="px-4 py-2.5 border-b">
                <label className="text-xs font-bold text-muted-foreground uppercase tracking-wider block mb-1.5">
                  Amount Received
                </label>
                <div className="relative">
                  <span className="absolute left-3 top-1/2 -translate-y-1/2 text-lg font-bold text-muted-foreground">৳</span>
                  <input
                    type="number"
                    value={amountReceived}
                    onChange={e => setAmountReceived(e.target.value)}
                    placeholder={fmt(grandTotal)}
                    className="w-full pl-8 pr-4 py-2 text-xl font-bold border-2 rounded-xl focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary bg-secondary/30 transition-all"
                  />
                </div>

                {/* Change display */}
                {amountNum > 0 && (
                  <div className={`flex justify-between mt-2 font-semibold text-sm rounded-lg px-3 py-1.5 ${
                    change >= 0 ? 'bg-green-50 text-green-700 dark:bg-green-900/20 dark:text-green-400' : 'bg-red-50 text-red-700 dark:bg-red-900/20 dark:text-red-400'
                  }`}>
                    <span>{change >= 0 ? 'Change' : 'Short'}</span>
                    <span>৳{fmt(Math.abs(change))}</span>
                  </div>
                )}

                {/* Quick preset amounts */}
                {quickAmounts.length > 0 && (
                  <div className="grid grid-cols-4 gap-1.5 mt-2">
                    {quickAmounts.map(amt => (
                      <button
                        key={amt}
                        onClick={() => setAmountReceived(amt.toString())}
                        className="py-1 text-xs font-semibold bg-secondary hover:bg-primary hover:text-primary-foreground rounded-lg transition-colors border"
                      >
                        ৳{amt.toLocaleString()}
                      </button>
                    ))}
                  </div>
                )}
              </div>
            )}

            {/* Action buttons */}
            <div className="px-4 py-3 space-y-2">
              {/* CHARGE */}
              <Button
                className="w-full h-12 text-base font-bold bg-green-600 hover:bg-green-700 active:bg-green-800 text-white shadow-sm"
                onClick={completeSale}
                disabled={saving || (selectedPayment === 'CASH' && amountNum > 0 && amountNum < grandTotal)}
              >
                {saving
                  ? <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                  : <CheckCircle2 className="w-5 h-5 mr-2" />
                }
                CHARGE ৳{fmt(grandTotal)}
              </Button>

              {/* Secondary actions */}
              <div className="grid grid-cols-3 gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  className="h-9 text-xs font-semibold"
                  onClick={holdSale}
                  disabled={saving}
                >
                  <Pause className="w-3.5 h-3.5 mr-1" /> HOLD
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  className="h-9 text-xs font-semibold text-destructive border-destructive/30 hover:bg-destructive/10"
                  onClick={() => setShowVoidDialog(true)}
                >
                  <XCircle className="w-3.5 h-3.5 mr-1" /> VOID
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  className="h-9 text-xs font-semibold"
                  onClick={clearCart}
                >
                  <X className="w-3.5 h-3.5 mr-1" /> CLEAR
                </Button>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* ══ MODALS ════════════════════════════════════════════════════════════ */}

      {/* Receipt Modal */}
      {showReceipt && lastTxn && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl shadow-2xl w-full max-w-sm overflow-hidden">
            <div className="bg-green-600 px-6 py-6 text-white text-center">
              <CheckCircle2 className="w-14 h-14 mx-auto mb-2" />
              <h2 className="text-2xl font-bold">Sale Complete!</h2>
              <p className="text-green-100 font-mono text-sm mt-1">{lastTxn.receiptNumber}</p>
            </div>
            <div className="p-6 space-y-3">
              <div className="flex justify-between text-sm text-muted-foreground py-1.5 border-b">
                <span>Items</span>
                <span className="font-medium text-foreground">{lastTxn.totalItemQuantity}</span>
              </div>
              <div className="flex justify-between text-xl font-bold">
                <span>Total</span>
                <span className="text-green-600">৳{fmt(lastTxn.grandTotal)}</span>
              </div>
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">Paid</span>
                <span>৳{fmt(lastTxn.paidAmount)}</span>
              </div>
              {lastTxn.changeAmount > 0 && (
                <div className="flex justify-between text-sm font-semibold text-green-600">
                  <span>Change</span>
                  <span>৳{fmt(lastTxn.changeAmount)}</span>
                </div>
              )}
              <div className="grid grid-cols-2 gap-3 pt-3">
                <Button variant="outline" onClick={() => window.print()} className="h-11">
                  Print
                </Button>
                <Button
                  className="h-11 bg-green-600 hover:bg-green-700 text-white"
                  onClick={() => setShowReceipt(false)}
                >
                  New Sale
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Held Transactions Modal */}
      {showHeld && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl shadow-2xl w-full max-w-lg max-h-[70vh] flex flex-col overflow-hidden">
            <div className="flex items-center justify-between px-5 py-4 border-b shrink-0">
              <h2 className="text-lg font-semibold flex items-center gap-2">
                <Pause className="w-5 h-5 text-amber-500" /> Held Transactions
              </h2>
              <Button variant="ghost" size="icon" onClick={() => setShowHeld(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <div className="flex-1 overflow-auto p-4">
              {heldTxns.length === 0 ? (
                <div className="text-center text-muted-foreground py-10">
                  <Pause className="w-10 h-10 mx-auto mb-2 opacity-20" />
                  <p className="font-medium">No held transactions</p>
                </div>
              ) : (
                <div className="space-y-2">
                  {heldTxns.map(t => (
                    <div key={t.id} className="flex items-center justify-between px-4 py-3 border rounded-xl hover:bg-secondary/30 transition-colors">
                      <div>
                        <p className="font-semibold text-sm">{t.receiptNumber || 'Draft'}</p>
                        <p className="text-xs text-muted-foreground mt-0.5">
                          {t.customerName || 'Walk-in'} &middot; {t.totalItemQuantity} item{t.totalItemQuantity !== 1 ? 's' : ''}
                        </p>
                      </div>
                      <div className="flex items-center gap-3">
                        <span className="font-bold">৳{fmt(t.grandTotal)}</span>
                        <Button size="sm" className="h-8 gap-1" onClick={() => resumeTransaction(t.id)}>
                          <RotateCcw className="w-3.5 h-3.5" /> Resume
                        </Button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Void Dialog */}
      {showVoidDialog && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl shadow-2xl w-full max-w-sm overflow-hidden">
            <div className="bg-destructive/10 border-b border-destructive/20 px-5 py-4">
              <h2 className="text-lg font-semibold text-destructive flex items-center gap-2">
                <XCircle className="w-5 h-5" /> Void Transaction
              </h2>
              <p className="text-sm text-muted-foreground mt-0.5">This will clear the current cart</p>
            </div>
            <div className="p-5 space-y-4">
              <div>
                <label className="text-sm font-semibold block mb-1.5">
                  Reason <span className="text-destructive">*</span>
                </label>
                <textarea
                  value={voidReason}
                  onChange={e => setVoidReason(e.target.value)}
                  placeholder="Enter reason for voiding this transaction..."
                  className="nx-input w-full h-20 resize-none"
                />
              </div>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  className="flex-1"
                  onClick={() => { setShowVoidDialog(false); setVoidReason(''); }}
                >
                  Cancel
                </Button>
                <Button
                  variant="destructive"
                  className="flex-1"
                  onClick={handleVoid}
                  disabled={!voidReason.trim()}
                >
                  Void Sale
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Close Shift Modal */}
      {showCloseShift && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl shadow-2xl w-full max-w-sm overflow-hidden">
            <div className="px-5 py-4 border-b">
              <h2 className="text-lg font-semibold">Close Shift</h2>
              <p className="text-sm text-muted-foreground mt-0.5">End your selling session</p>
            </div>
            <div className="p-5 space-y-4">
              <div>
                <label className="text-sm font-semibold block mb-1.5">Closing Cash (BDT)</label>
                <div className="relative">
                  <span className="absolute left-3 top-1/2 -translate-y-1/2 font-bold text-muted-foreground">৳</span>
                  <Input
                    type="number"
                    className="pl-7"
                    value={closeShiftForm.closingCash}
                    onChange={e => setCloseShiftForm({ ...closeShiftForm, closingCash: e.target.value })}
                    placeholder="0.00"
                  />
                </div>
              </div>
              <div>
                <label className="text-sm font-semibold block mb-1.5">Notes (optional)</label>
                <textarea
                  value={closeShiftForm.notes}
                  onChange={e => setCloseShiftForm({ ...closeShiftForm, notes: e.target.value })}
                  placeholder="Any end-of-shift notes..."
                  className="nx-input w-full h-20 resize-none"
                />
              </div>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  className="flex-1"
                  onClick={() => setShowCloseShift(false)}
                >
                  Cancel
                </Button>
                <Button
                  variant="destructive"
                  className="flex-1"
                  onClick={handleCloseShift}
                  disabled={saving}
                >
                  {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  Close Shift
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
