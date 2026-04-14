import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Filter, ChevronLeft, ChevronRight, Loader2, X, Eye,
  RotateCcw, Trash2
} from 'lucide-react';
import {
  posReturnApi, warehouseApiV2,
  type PosReturn, type PosReturnDetail, type PosReturnLine, type Warehouse
} from '@/api/posApi';

interface ReturnLineForm {
  productId: string;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
  reason: string;
}

export default function PosReturns() {
  const [returns, setReturns] = useState<PosReturn[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(10);

  // Detail
  const [selected, setSelected] = useState<PosReturnDetail | null>(null);

  // Process return
  const [showProcess, setShowProcess] = useState(false);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [processForm, setProcessForm] = useState({
    warehouseId: '',
    customerName: '',
    reason: '',
    originalTransactionId: '',
    notes: '',
  });
  const [returnLines, setReturnLines] = useState<ReturnLineForm[]>([]);

  const fetchReturns = async () => {
    setLoading(true);
    try {
      const res = await posReturnApi.getAll({ pageIndex: currentPage - 1, pageSize, search: searchQuery || undefined });
      const data = res.data as unknown as { items: PosReturn[]; totalCount: number };
      setReturns(data?.items || []);
      setTotalCount(data?.totalCount || 0);
    } catch {
      /* ignore */
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchReturns(); }, [currentPage]);

  const handleSearch = () => { setCurrentPage(1); fetchReturns(); };

  const viewDetail = async (r: PosReturn) => {
    try {
      const res = await posReturnApi.getById(r.id);
      setSelected(res.data as unknown as PosReturnDetail);
    } catch { /* ignore */ }
  };

  const openProcess = async () => {
    try {
      const res = await warehouseApiV2.getAll({ pageSize: 100 });
      setWarehouses((res.data as unknown as { items: Warehouse[] })?.items || []);
    } catch { /* ignore */ }
    setProcessForm({ warehouseId: '', customerName: '', reason: '', originalTransactionId: '', notes: '' });
    setReturnLines([{ productId: '', productName: '', sku: '', quantity: 1, unitPrice: 0, lineTotal: 0, reason: '' }]);
    setShowProcess(true);
  };

  const updateLine = (idx: number, field: keyof ReturnLineForm, value: string | number) => {
    setReturnLines(lines => lines.map((l, i) => {
      if (i !== idx) return l;
      const updated = { ...l, [field]: value };
      if (field === 'quantity' || field === 'unitPrice') {
        updated.lineTotal = updated.quantity * updated.unitPrice;
      }
      return updated;
    }));
  };

  const addLine = () => {
    setReturnLines([...returnLines, { productId: '', productName: '', sku: '', quantity: 1, unitPrice: 0, lineTotal: 0, reason: '' }]);
  };

  const removeLine = (idx: number) => {
    setReturnLines(returnLines.filter((_, i) => i !== idx));
  };

  const handleProcess = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!processForm.warehouseId || returnLines.length === 0) return;
    setSaving(true);
    try {
      await posReturnApi.process({
        warehouseId: processForm.warehouseId,
        customerName: processForm.customerName || undefined,
        reason: processForm.reason,
        originalTransactionId: processForm.originalTransactionId || undefined,
        lines: returnLines.filter(l => l.productName),
        notes: processForm.notes || undefined,
      });
      setShowProcess(false);
      fetchReturns();
    } catch (err) {
      console.error('Process return failed:', err);
    } finally {
      setSaving(false);
    }
  };

  const fmt = (n: number) => n.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' });
  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">POS Returns</h1>
          <p className="nx-page-subtitle">Manage product returns</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openProcess}>
            <Plus className="w-4 h-4 mr-2" /> Process Return
          </Button>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input
                type="text"
                placeholder="Search returns..."
                value={searchQuery}
                onChange={e => setSearchQuery(e.target.value)}
                onKeyDown={e => e.key === 'Enter' && handleSearch()}
              />
            </div>
            <Button variant="outline" size="sm" onClick={handleSearch}>
              <Filter className="w-4 h-4 mr-2" /> Search
            </Button>
          </div>
        </div>

        {loading ? (
          <div className="flex items-center justify-center p-8"><Loader2 className="w-8 h-8 animate-spin" /></div>
        ) : (
          <>
            <div className="nx-table-wrap">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Return #</th>
                    <th>Date</th>
                    <th>Warehouse</th>
                    <th>Customer</th>
                    <th>Amount</th>
                    <th>Items</th>
                    <th>Status</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {returns.map(r => (
                    <tr key={r.id}>
                      <td><code className="text-xs bg-secondary px-2 py-1 rounded">{r.returnNumber}</code></td>
                      <td>{r.returnDate ? new Date(r.returnDate).toLocaleDateString() : '-'}</td>
                      <td>{r.warehouseName}</td>
                      <td>{r.customerName || 'Walk-in'}</td>
                      <td className="font-medium">{fmt(r.totalAmount)}</td>
                      <td>{r.itemCount}</td>
                      <td>
                        <span className={`nx-badge ${r.status === 'Completed' ? 'nx-badge-success' : 'nx-badge-warning'}`}>
                          {r.status}
                        </span>
                      </td>
                      <td>
                        <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => viewDetail(r)}>
                          <Eye className="w-4 h-4" />
                        </Button>
                      </td>
                    </tr>
                  ))}
                  {returns.length === 0 && (
                    <tr><td colSpan={8} className="text-center text-muted-foreground py-8">No returns found</td></tr>
                  )}
                </tbody>
              </table>
            </div>
            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">Showing {returns.length} of {totalCount}</p>
              <div className="flex items-center gap-2">
                <Button variant="outline" size="sm" disabled={currentPage === 1} onClick={() => setCurrentPage(currentPage - 1)}>
                  <ChevronLeft className="w-4 h-4" />
                </Button>
                <span className="text-sm">Page {currentPage} of {totalPages || 1}</span>
                <Button variant="outline" size="sm" disabled={currentPage >= totalPages} onClick={() => setCurrentPage(currentPage + 1)}>
                  <ChevronRight className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </>
        )}
      </Card>

      {/* Detail Modal */}
      {selected && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg max-h-[80vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold flex items-center gap-2">
                <RotateCcw className="w-5 h-5" /> Return {selected.returnNumber}
              </h2>
              <Button variant="ghost" size="icon" onClick={() => setSelected(null)}><X className="w-4 h-4" /></Button>
            </div>
            <div className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-2 text-sm">
                <div><span className="text-muted-foreground">Date:</span> {new Date(selected.returnDate).toLocaleDateString()}</div>
                <div><span className="text-muted-foreground">Customer:</span> {selected.customerName || 'Walk-in'}</div>
                <div><span className="text-muted-foreground">Warehouse:</span> {selected.warehouseName}</div>
                <div><span className="text-muted-foreground">Total:</span> <strong>{fmt(selected.totalAmount)}</strong></div>
                {selected.reason && <div className="col-span-2"><span className="text-muted-foreground">Reason:</span> {selected.reason}</div>}
              </div>

              {selected.lines && selected.lines.length > 0 && (
                <div className="border rounded-lg">
                  <table className="nx-table">
                    <thead>
                      <tr>
                        <th>Product</th>
                        <th>Qty</th>
                        <th>Price</th>
                        <th>Total</th>
                      </tr>
                    </thead>
                    <tbody>
                      {selected.lines.map((l: PosReturnLine) => (
                        <tr key={l.id}>
                          <td>{l.productName}</td>
                          <td>{l.quantity}</td>
                          <td>{fmt(l.unitPrice)}</td>
                          <td className="font-medium">{fmt(l.lineTotal)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Process Return Modal */}
      {showProcess && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-2xl max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">Process Return</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowProcess(false)}><X className="w-4 h-4" /></Button>
            </div>
            <form onSubmit={handleProcess} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Warehouse *</label>
                  <select
                    className="nx-input nx-select w-full mt-1"
                    value={processForm.warehouseId}
                    onChange={e => setProcessForm({ ...processForm, warehouseId: e.target.value })}
                    required
                  >
                    <option value="">Select warehouse</option>
                    {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
                  </select>
                </div>
                <div>
                  <label className="text-sm font-medium">Customer Name</label>
                  <Input value={processForm.customerName} onChange={e => setProcessForm({ ...processForm, customerName: e.target.value })} className="mt-1" />
                </div>
                <div>
                  <label className="text-sm font-medium">Original Receipt #</label>
                  <Input value={processForm.originalTransactionId} onChange={e => setProcessForm({ ...processForm, originalTransactionId: e.target.value })} className="mt-1" />
                </div>
                <div>
                  <label className="text-sm font-medium">Reason *</label>
                  <Input value={processForm.reason} onChange={e => setProcessForm({ ...processForm, reason: e.target.value })} required className="mt-1" />
                </div>
              </div>

              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <label className="text-sm font-medium">Return Items</label>
                  <Button type="button" variant="outline" size="sm" onClick={addLine}>
                    <Plus className="w-3 h-3 mr-1" /> Add Item
                  </Button>
                </div>
                {returnLines.map((line, idx) => (
                  <div key={idx} className="grid grid-cols-12 gap-2 items-end">
                    <div className="col-span-4">
                      <Input placeholder="Product name" value={line.productName} onChange={e => updateLine(idx, 'productName', e.target.value)} />
                    </div>
                    <div className="col-span-2">
                      <Input type="number" placeholder="Qty" value={line.quantity} onChange={e => updateLine(idx, 'quantity', parseInt(e.target.value) || 0)} />
                    </div>
                    <div className="col-span-2">
                      <Input type="number" placeholder="Price" value={line.unitPrice} onChange={e => updateLine(idx, 'unitPrice', parseFloat(e.target.value) || 0)} />
                    </div>
                    <div className="col-span-2">
                      <Input value={fmt(line.lineTotal)} disabled />
                    </div>
                    <div className="col-span-2 flex justify-end">
                      {returnLines.length > 1 && (
                        <Button type="button" variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => removeLine(idx)}>
                          <Trash2 className="w-4 h-4" />
                        </Button>
                      )}
                    </div>
                  </div>
                ))}
              </div>

              <div>
                <label className="text-sm font-medium">Notes</label>
                <Input value={processForm.notes} onChange={e => setProcessForm({ ...processForm, notes: e.target.value })} className="mt-1" />
              </div>

              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowProcess(false)}>Cancel</Button>
                <Button type="submit" disabled={saving}>
                  {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  Process Return
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
