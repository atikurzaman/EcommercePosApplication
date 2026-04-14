import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Search, Filter, ChevronLeft, ChevronRight, Loader2, X, Eye,
  Calculator, Clock, DollarSign
} from 'lucide-react';
import { cashShiftApi, type CashShift, type CashShiftSummary } from '@/api/posApi';

export default function CashShifts() {
  const [shifts, setShifts] = useState<CashShift[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(10);

  // Detail / Summary
  const [selectedSummary, setSelectedSummary] = useState<CashShiftSummary | null>(null);
  const [summaryLoading, setSummaryLoading] = useState(false);

  // Close shift
  const [closingShift, setClosingShift] = useState<CashShift | null>(null);
  const [closingCash, setClosingCash] = useState(0);
  const [saving, setSaving] = useState(false);

  const fetchShifts = async () => {
    setLoading(true);
    try {
      const res = await cashShiftApi.getAll({ pageIndex: currentPage - 1, pageSize, search: searchQuery || undefined });
      const data = res.data as unknown as { items: CashShift[]; totalCount: number };
      setShifts(data?.items || []);
      setTotalCount(data?.totalCount || 0);
    } catch {
      /* ignore */
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchShifts(); }, [currentPage]);

  const handleSearch = () => { setCurrentPage(1); fetchShifts(); };

  const viewSummary = async (shift: CashShift) => {
    setSummaryLoading(true);
    try {
      const res = await cashShiftApi.getSummary(shift.id);
      setSelectedSummary(res.data as unknown as CashShiftSummary);
    } catch {
      /* ignore */
    } finally {
      setSummaryLoading(false);
    }
  };

  const handleClose = async () => {
    if (!closingShift) return;
    setSaving(true);
    try {
      await cashShiftApi.close(closingShift.id, { closingCash });
      setClosingShift(null);
      setClosingCash(0);
      fetchShifts();
    } catch (err) {
      console.error('Close shift failed:', err);
    } finally {
      setSaving(false);
    }
  };

  const fmt = (n: number) => n.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' });
  const fmtDate = (d?: string) => d ? new Date(d).toLocaleString() : '-';
  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Cash Shifts</h1>
          <p className="nx-page-subtitle">Manage POS cash shifts</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Shifts</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value success">{shifts.filter(s => s.status === 'Open').length}</div>
          <div className="nx-stat-label">Open Shifts</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{shifts.filter(s => s.status === 'Closed').length}</div>
          <div className="nx-stat-label">Closed Shifts</div>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input
                type="text"
                placeholder="Search shifts..."
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
                    <th>Warehouse</th>
                    <th>Counter</th>
                    <th>Opened By</th>
                    <th>Status</th>
                    <th>Opened</th>
                    <th>Closed</th>
                    <th>Sales</th>
                    <th style={{ width: 120 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {shifts.map(s => (
                    <tr key={s.id}>
                      <td>{s.warehouseName}</td>
                      <td>{s.posCounterName}</td>
                      <td>{s.openedByName}</td>
                      <td>
                        <span className={`nx-badge ${s.status === 'Open' ? 'nx-badge-success' : 'nx-badge-default'}`}>
                          {s.status}
                        </span>
                      </td>
                      <td className="text-xs">{fmtDate(s.openedAt)}</td>
                      <td className="text-xs">{fmtDate(s.closedAt)}</td>
                      <td className="font-medium">{fmt(s.totalSales)}</td>
                      <td>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => viewSummary(s)} title="View Summary">
                            <Eye className="w-4 h-4" />
                          </Button>
                          {s.status === 'Open' && (
                            <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => setClosingShift(s)} title="Close Shift">
                              <Clock className="w-4 h-4" />
                            </Button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                  {shifts.length === 0 && (
                    <tr><td colSpan={8} className="text-center text-muted-foreground py-8">No shifts found</td></tr>
                  )}
                </tbody>
              </table>
            </div>
            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">Showing {shifts.length} of {totalCount}</p>
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

      {/* Summary Modal */}
      {selectedSummary && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg max-h-[80vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">Shift Summary</h2>
              <Button variant="ghost" size="icon" onClick={() => setSelectedSummary(null)}><X className="w-4 h-4" /></Button>
            </div>
            {summaryLoading ? (
              <div className="flex items-center justify-center p-8"><Loader2 className="w-8 h-8 animate-spin" /></div>
            ) : (
              <div className="p-4 space-y-4">
                <div className="grid grid-cols-2 gap-3 text-sm">
                  <div><span className="text-muted-foreground">Warehouse:</span> {selectedSummary.warehouseName}</div>
                  <div><span className="text-muted-foreground">Counter:</span> {selectedSummary.posCounterName}</div>
                  <div><span className="text-muted-foreground">Opened:</span> {fmtDate(selectedSummary.openedAt)}</div>
                  <div><span className="text-muted-foreground">Closed:</span> {fmtDate(selectedSummary.closedAt)}</div>
                  <div><span className="text-muted-foreground">Opening Cash:</span> {fmt(selectedSummary.openingCash)}</div>
                  <div><span className="text-muted-foreground">Closing Cash:</span> {fmt(selectedSummary.closingCash || 0)}</div>
                </div>

                <div className="border rounded-lg p-3 space-y-2">
                  <h3 className="font-medium flex items-center gap-2"><DollarSign className="w-4 h-4" /> Payment Breakdown</h3>
                  <div className="grid grid-cols-2 gap-2 text-sm">
                    <div className="flex justify-between"><span>Transactions:</span><span className="font-medium">{selectedSummary.transactionCount}</span></div>
                    <div className="flex justify-between"><span>Total Sales:</span><span className="font-medium">{fmt(selectedSummary.totalSales)}</span></div>
                    <div className="flex justify-between"><span>Cash:</span><span>{fmt(selectedSummary.cashPayments)}</span></div>
                    <div className="flex justify-between"><span>Card:</span><span>{fmt(selectedSummary.cardPayments)}</span></div>
                    <div className="flex justify-between"><span>Mobile:</span><span>{fmt(selectedSummary.mobilePayments)}</span></div>
                    <div className="flex justify-between"><span>Other:</span><span>{fmt(selectedSummary.otherPayments)}</span></div>
                  </div>
                </div>

                {selectedSummary.drawerEvents && selectedSummary.drawerEvents.length > 0 && (
                  <div className="border rounded-lg p-3 space-y-2">
                    <h3 className="font-medium flex items-center gap-2"><Calculator className="w-4 h-4" /> Drawer Events</h3>
                    <div className="space-y-1">
                      {selectedSummary.drawerEvents.map(ev => (
                        <div key={ev.id} className="flex justify-between text-sm">
                          <span>{ev.eventType} {ev.reason ? `- ${ev.reason}` : ''}</span>
                          <span className="font-medium">{fmt(ev.amount)}</span>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      )}

      {/* Close Shift Modal */}
      {closingShift && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-sm p-6">
            <h2 className="text-lg font-semibold mb-4">Close Shift</h2>
            <p className="text-sm text-muted-foreground mb-4">
              Counter: {closingShift.posCounterName} | Opened: {fmtDate(closingShift.openedAt)}
            </p>
            <div className="space-y-3">
              <div>
                <label className="text-sm font-medium">Closing Cash Amount *</label>
                <Input type="number" value={closingCash} onChange={e => setClosingCash(parseFloat(e.target.value) || 0)} className="mt-1" />
              </div>
              <div className="flex justify-end gap-2">
                <Button variant="outline" onClick={() => { setClosingShift(null); setClosingCash(0); }}>Cancel</Button>
                <Button onClick={handleClose} disabled={saving}>
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
