import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Search, Filter, ChevronLeft, ChevronRight, Loader2, X, Eye,
  Plus, Calendar
} from 'lucide-react';
import { dayEndSummaryApi, warehouseApiV2, type DayEndSummary, type Warehouse } from '@/api/posApi';

export default function DayEndSummaries() {
  const [summaries, setSummaries] = useState<DayEndSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(10);

  // Detail
  const [selected, setSelected] = useState<DayEndSummary | null>(null);

  // Generate
  const [showGenerate, setShowGenerate] = useState(false);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [genForm, setGenForm] = useState({ warehouseId: '', summaryDate: '' });
  const [saving, setSaving] = useState(false);

  const fetchSummaries = async () => {
    setLoading(true);
    try {
      const res = await dayEndSummaryApi.getAll({ pageIndex: currentPage - 1, pageSize, search: searchQuery || undefined });
      const data = res.data as unknown as { items: DayEndSummary[]; totalCount: number };
      setSummaries(data?.items || []);
      setTotalCount(data?.totalCount || 0);
    } catch {
      /* ignore */
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchSummaries(); }, [currentPage]);

  const handleSearch = () => { setCurrentPage(1); fetchSummaries(); };

  const openGenerate = async () => {
    try {
      const res = await warehouseApiV2.getAll({ pageSize: 100 });
      setWarehouses((res.data as unknown as { items: Warehouse[] })?.items || []);
    } catch { /* ignore */ }
    setGenForm({ warehouseId: '', summaryDate: new Date().toISOString().split('T')[0] });
    setShowGenerate(true);
  };

  const handleGenerate = async () => {
    if (!genForm.warehouseId || !genForm.summaryDate) return;
    setSaving(true);
    try {
      await dayEndSummaryApi.generate(genForm);
      setShowGenerate(false);
      fetchSummaries();
    } catch (err) {
      console.error('Generate failed:', err);
    } finally {
      setSaving(false);
    }
  };

  const viewDetail = async (s: DayEndSummary) => {
    try {
      const res = await dayEndSummaryApi.getById(s.id);
      setSelected(res.data as unknown as DayEndSummary);
    } catch { /* ignore */ }
  };

  const fmt = (n: number) => n.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' });
  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Day-End Summaries</h1>
          <p className="nx-page-subtitle">Daily sales and operations summaries</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openGenerate}>
            <Plus className="w-4 h-4 mr-2" /> Generate Summary
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
                placeholder="Search summaries..."
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
                    <th>Date</th>
                    <th>Warehouse</th>
                    <th>Sales</th>
                    <th>Amount</th>
                    <th>Cash</th>
                    <th>Card</th>
                    <th>Mobile</th>
                    <th>Returns</th>
                    <th>Status</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {summaries.map(s => (
                    <tr key={s.id}>
                      <td className="flex items-center gap-2">
                        <Calendar className="w-4 h-4 text-muted-foreground" />
                        {s.summaryDate ? new Date(s.summaryDate).toLocaleDateString() : '-'}
                      </td>
                      <td>{s.warehouseName}</td>
                      <td>{s.salesCount}</td>
                      <td className="font-medium">{fmt(s.salesAmount)}</td>
                      <td>{fmt(s.cashAmount)}</td>
                      <td>{fmt(s.cardAmount)}</td>
                      <td>{fmt(s.mobileAmount)}</td>
                      <td>{s.returnsCount} ({fmt(s.returnsAmount)})</td>
                      <td>
                        <span className={`nx-badge ${s.status === 'Final' ? 'nx-badge-success' : 'nx-badge-warning'}`}>
                          {s.status}
                        </span>
                      </td>
                      <td>
                        <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => viewDetail(s)}>
                          <Eye className="w-4 h-4" />
                        </Button>
                      </td>
                    </tr>
                  ))}
                  {summaries.length === 0 && (
                    <tr><td colSpan={10} className="text-center text-muted-foreground py-8">No summaries found</td></tr>
                  )}
                </tbody>
              </table>
            </div>
            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">Showing {summaries.length} of {totalCount}</p>
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
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold">Summary Details</h2>
              <Button variant="ghost" size="icon" onClick={() => setSelected(null)}><X className="w-4 h-4" /></Button>
            </div>
            <div className="space-y-3 text-sm">
              <div className="grid grid-cols-2 gap-2">
                <div><span className="text-muted-foreground">Date:</span> {new Date(selected.summaryDate).toLocaleDateString()}</div>
                <div><span className="text-muted-foreground">Warehouse:</span> {selected.warehouseName}</div>
                <div><span className="text-muted-foreground">Sales Count:</span> {selected.salesCount}</div>
                <div><span className="text-muted-foreground">Sales Amount:</span> {fmt(selected.salesAmount)}</div>
                <div><span className="text-muted-foreground">Cash:</span> {fmt(selected.cashAmount)}</div>
                <div><span className="text-muted-foreground">Card:</span> {fmt(selected.cardAmount)}</div>
                <div><span className="text-muted-foreground">Mobile:</span> {fmt(selected.mobileAmount)}</div>
                <div><span className="text-muted-foreground">Returns:</span> {selected.returnsCount} ({fmt(selected.returnsAmount)})</div>
                <div><span className="text-muted-foreground">Expenses:</span> {fmt(selected.expensesAmount)}</div>
                <div><span className="text-muted-foreground">Net Amount:</span> <strong>{fmt(selected.netAmount)}</strong></div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Generate Modal */}
      {showGenerate && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-sm p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold">Generate Day-End Summary</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowGenerate(false)}><X className="w-4 h-4" /></Button>
            </div>
            <div className="space-y-4">
              <div>
                <label className="text-sm font-medium">Warehouse *</label>
                <select
                  className="nx-input nx-select w-full mt-1"
                  value={genForm.warehouseId}
                  onChange={e => setGenForm({ ...genForm, warehouseId: e.target.value })}
                >
                  <option value="">Select warehouse</option>
                  {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
                </select>
              </div>
              <div>
                <label className="text-sm font-medium">Date *</label>
                <Input
                  type="date"
                  value={genForm.summaryDate}
                  onChange={e => setGenForm({ ...genForm, summaryDate: e.target.value })}
                  className="mt-1"
                />
              </div>
              <div className="flex justify-end gap-2">
                <Button variant="outline" onClick={() => setShowGenerate(false)}>Cancel</Button>
                <Button onClick={handleGenerate} disabled={saving || !genForm.warehouseId || !genForm.summaryDate}>
                  {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  Generate
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
