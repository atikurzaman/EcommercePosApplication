import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { 
  Plus, ChevronLeft, ChevronRight, Loader2
} from 'lucide-react';
import { inventoryApi, StockTransfer } from '@/api/inventoryApi';
import { warehouseApi, Warehouse } from '@/api/warehouseApi';

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-BD', { 
    day: 'numeric', month: 'short', year: 'numeric' 
  });
}

const statusColors: Record<string, string> = {
  PENDING: 'secondary',
  IN_TRANSIT: 'default',
  RECEIVED: 'outline',
  CANCELLED: 'destructive'
};

export default function StockTransfersPage() {
  const [transfers, setTransfers] = useState<StockTransfer[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [selectedWarehouse, setSelectedWarehouse] = useState('');
  const [status, setStatus] = useState('');
  const [totalCount, setTotalCount] = useState(0);
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [submitting] = useState(false);
  const pageSize = 15;

  useEffect(() => {
    warehouseApi.getAll({ pageSize: 100 }).then(res => {
      if (res.data?.data?.items) setWarehouses(res.data.data.items);
    });
  }, []);

  useEffect(() => {
    setLoading(true);
    inventoryApi.getStockTransfers({
      pageIndex: currentPage - 1,
      pageSize,
      fromWarehouseId: selectedWarehouse || undefined,
      status: status || undefined,
    }).then(res => {
      if (res.data?.data) {
        setTransfers(res.data.data.items || []);
        setTotalCount(res.data.data.totalCount || 0);
      }
    }).finally(() => setLoading(false));
  }, [currentPage, selectedWarehouse, status]);

  const handleReceive = async (id: string) => {
    try {
      await inventoryApi.receiveStockTransfer(id);
      setTransfers(prev => prev.map(t => t.id === id ? { ...t, status: 'RECEIVED' } : t));
    } catch (error) {
      console.error('Error receiving transfer:', error);
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-6">
      <div className="nx-page-header flex items-center justify-between">
        <div>
          <h1 className="nx-page-title">Stock Transfers</h1>
          <p className="nx-page-subtitle">Transfer stock between warehouses</p>
        </div>
        <Button onClick={() => setShowCreateDialog(true)}>
          <Plus className="w-4 h-4 mr-2" />
          New Transfer
        </Button>
      </div>

      <Card>
        <div className="p-4 border-b flex gap-4">
          <select 
            className="nx-input nx-select w-40"
            value={selectedWarehouse}
            onChange={(e) => { setSelectedWarehouse(e.target.value); setCurrentPage(1); }}
          >
            <option value="">From Any</option>
            {warehouses.map(wh => (
              <option key={wh.id} value={wh.id}>{wh.name}</option>
            ))}
          </select>
          <select 
            className="nx-input nx-select w-40"
            value={status}
            onChange={(e) => { setStatus(e.target.value); setCurrentPage(1); }}
          >
            <option value="">All Status</option>
            <option value="PENDING">Pending</option>
            <option value="IN_TRANSIT">In Transit</option>
            <option value="RECEIVED">Received</option>
          </select>
        </div>

        {loading ? (
          <div className="flex items-center justify-center p-12">
            <Loader2 className="w-8 h-8 animate-spin" />
          </div>
        ) : (
          <>
            <div className="nx-table-wrap">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Transfer No</th>
                    <th>Date</th>
                    <th>From</th>
                    <th>To</th>
                    <th>Status</th>
                    <th>Created By</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {transfers.map(t => (
                    <tr key={t.id}>
                      <td className="font-mono text-sm">{t.transferNo}</td>
                      <td>{formatDate(t.transferDate)}</td>
                      <td>{t.fromWarehouseName}</td>
                      <td>{t.toWarehouseName}</td>
                      <td>
                        <Badge variant={statusColors[t.status] as any || 'secondary'}>
                          {t.status}
                        </Badge>
                      </td>
                      <td>{t.createdBy || '-'}</td>
                      <td>
                        {t.status === 'PENDING' && (
                          <Button size="sm" onClick={() => handleReceive(t.id)}>
                            Receive
                          </Button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {totalPages > 1 && (
              <div className="flex items-center justify-between p-4 border-t">
                <p className="text-sm text-muted-foreground">Showing {transfers.length} of {totalCount}</p>
                <div className="flex items-center gap-2">
                  <Button variant="outline" size="sm" disabled={currentPage === 1} onClick={() => setCurrentPage(c => c - 1)}>
                    <ChevronLeft className="w-4 h-4" />
                  </Button>
                  <span className="text-sm">Page {currentPage} of {totalPages}</span>
                  <Button variant="outline" size="sm" disabled={currentPage >= totalPages} onClick={() => setCurrentPage(c => c + 1)}>
                    <ChevronRight className="w-4 h-4" />
                  </Button>
                </div>
              </div>
            )}
          </>
        )}
      </Card>

      <Dialog open={showCreateDialog} onOpenChange={setShowCreateDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Create Stock Transfer</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium">From Warehouse</label>
                <select className="nx-input nx-select mt-1 w-full">
                  <option value="">Select</option>
                  {warehouses.map(wh => (
                    <option key={wh.id} value={wh.id}>{wh.name}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="text-sm font-medium">To Warehouse</label>
                <select className="nx-input nx-select mt-1 w-full">
                  <option value="">Select</option>
                  {warehouses.map(wh => (
                    <option key={wh.id} value={wh.id}>{wh.name}</option>
                  ))}
                </select>
              </div>
            </div>
            <div>
              <label className="text-sm font-medium">Notes (optional)</label>
              <Input className="mt-1" placeholder="Transfer notes" />
            </div>
            <div className="border rounded p-3">
              <p className="text-sm font-medium mb-2">Transfer Items</p>
              <p className="text-xs text-muted-foreground">Product selection will be implemented with product autocomplete</p>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowCreateDialog(false)}>Cancel</Button>
            <Button disabled={submitting}>
              {submitting && <Loader2 className="w-4 h-4 animate-spin mr-2" />}
              Create Transfer
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}