import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { 
  Plus, ChevronLeft, ChevronRight, Loader2, CheckCircle
} from 'lucide-react';
import { inventoryApi, InventoryAdjustment } from '@/api/inventoryApi';
import { warehouseApi, Warehouse } from '@/api/warehouseApi';

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-BD', { 
    day: 'numeric', month: 'short', year: 'numeric' 
  });
}

export default function InventoryAdjustmentsPage() {
  const [adjustments, setAdjustments] = useState<InventoryAdjustment[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [selectedWarehouse, setSelectedWarehouse] = useState('');
  const [totalCount, setTotalCount] = useState(0);
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    warehouseId: '',
    adjustmentType: 'INCREASE',
    reason: '',
    notes: '',
    lines: [{ productId: '', quantityAdjusted: 0, reason: '' }]
  });
  const pageSize = 15;

  useEffect(() => {
    warehouseApi.getAll({ pageSize: 100 }).then(res => {
      if (res.data?.data?.items) setWarehouses(res.data.data.items);
    });
  }, []);

  useEffect(() => {
    setLoading(true);
    inventoryApi.getInventoryAdjustments({
      pageIndex: currentPage - 1,
      pageSize,
      warehouseId: selectedWarehouse || undefined,
    }).then(res => {
      if (res.data?.data) {
        setAdjustments(res.data.data.items || []);
        setTotalCount(res.data.data.totalCount || 0);
      }
    }).finally(() => setLoading(false));
  }, [currentPage, selectedWarehouse]);

  const handleCreate = async () => {
    if (!formData.warehouseId || !formData.reason) return;
    setSubmitting(true);
    try {
      await inventoryApi.createInventoryAdjustment(formData);
      setShowCreateDialog(false);
      setFormData({ warehouseId: '', adjustmentType: 'INCREASE', reason: '', notes: '', lines: [{ productId: '', quantityAdjusted: 0, reason: '' }] });
      setCurrentPage(1);
      inventoryApi.getInventoryAdjustments({ pageIndex: 0, pageSize, warehouseId: selectedWarehouse || undefined }).then(res => {
        if (res.data?.data) {
          setAdjustments(res.data.data.items || []);
          setTotalCount(res.data.data.totalCount || 0);
        }
      });
    } catch (error) {
      console.error('Error creating adjustment:', error);
    } finally {
      setSubmitting(false);
    }
  };

  const handleApprove = async (id: string) => {
    try {
      await inventoryApi.approveInventoryAdjustment(id);
      setAdjustments(prev => prev.map(a => a.id === id ? { ...a, isApproved: true } : a));
    } catch (error) {
      console.error('Error approving adjustment:', error);
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-6">
      <div className="nx-page-header flex items-center justify-between">
        <div>
          <h1 className="nx-page-title">Inventory Adjustments</h1>
          <p className="nx-page-subtitle">Record and approve stock adjustments</p>
        </div>
        <Button onClick={() => setShowCreateDialog(true)}>
          <Plus className="w-4 h-4 mr-2" />
          New Adjustment
        </Button>
      </div>

      <Card>
        <div className="p-4 border-b">
          <select 
            className="nx-input nx-select w-48"
            value={selectedWarehouse}
            onChange={(e) => { setSelectedWarehouse(e.target.value); setCurrentPage(1); }}
          >
            <option value="">All Warehouses</option>
            {warehouses.map(wh => (
              <option key={wh.id} value={wh.id}>{wh.name}</option>
            ))}
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
                    <th>Adjustment No</th>
                    <th>Date</th>
                    <th>Warehouse</th>
                    <th>Type</th>
                    <th>Reason</th>
                    <th>Status</th>
                    <th>Created By</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {adjustments.map(adj => (
                    <tr key={adj.id}>
                      <td className="font-mono text-sm">{adj.adjustmentNo}</td>
                      <td>{formatDate(adj.adjustmentDate)}</td>
                      <td>{adj.warehouseName}</td>
                      <td>
                        <Badge variant={adj.adjustmentType === 'INCREASE' ? 'default' : 'destructive'}>
                          {adj.adjustmentType}
                        </Badge>
                      </td>
                      <td className="max-w-xs truncate">{adj.reason}</td>
                      <td>
                        {adj.isApproved ? (
                          <Badge variant="outline" className="text-green-600">Approved</Badge>
                        ) : (
                          <Badge variant="secondary">Pending</Badge>
                        )}
                      </td>
                      <td>{adj.createdBy || '-'}</td>
                      <td>
                        {!adj.isApproved && (
                          <Button size="sm" variant="outline" onClick={() => handleApprove(adj.id)}>
                            <CheckCircle className="w-4 h-4 mr-1" />
                            Approve
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
                <p className="text-sm text-muted-foreground">Showing {adjustments.length} of {totalCount}</p>
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
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Create Inventory Adjustment</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium">Warehouse</label>
                <select 
                  className="nx-input nx-select mt-1 w-full"
                  value={formData.warehouseId}
                  onChange={(e) => setFormData(prev => ({ ...prev, warehouseId: e.target.value }))}
                >
                  <option value="">Select warehouse</option>
                  {warehouses.map(wh => (
                    <option key={wh.id} value={wh.id}>{wh.name}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="text-sm font-medium">Type</label>
                <select 
                  className="nx-input nx-select mt-1 w-full"
                  value={formData.adjustmentType}
                  onChange={(e) => setFormData(prev => ({ ...prev, adjustmentType: e.target.value }))}
                >
                  <option value="INCREASE">Increase</option>
                  <option value="DECREASE">Decrease</option>
                </select>
              </div>
            </div>
            <div>
              <label className="text-sm font-medium">Reason</label>
              <Input 
                className="mt-1"
                placeholder="e.g., Cycle count, Damaged goods"
                value={formData.reason}
                onChange={(e) => setFormData(prev => ({ ...prev, reason: e.target.value }))}
              />
            </div>
            <div>
              <label className="text-sm font-medium">Notes (optional)</label>
              <Input 
                className="mt-1"
                placeholder="Additional details"
                value={formData.notes}
                onChange={(e) => setFormData(prev => ({ ...prev, notes: e.target.value }))}
              />
            </div>
            <div className="border rounded p-3">
              <p className="text-sm font-medium mb-2">Adjustment Lines</p>
              <p className="text-xs text-muted-foreground">Product selection and quantity adjustment will be implemented with product autocomplete</p>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowCreateDialog(false)}>Cancel</Button>
            <Button onClick={handleCreate} disabled={submitting || !formData.warehouseId || !formData.reason}>
              {submitting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}
              Create Adjustment
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}