import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { 
  ChevronLeft, ChevronRight, Loader2
} from 'lucide-react';
import { inventoryApi, StockMovement } from '@/api/inventoryApi';
import { warehouseApi, Warehouse } from '@/api/warehouseApi';

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-BD', { 
    day: 'numeric', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' 
  });
}

export default function StockMovementsPage() {
  const [movements, setMovements] = useState<StockMovement[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [selectedWarehouse, setSelectedWarehouse] = useState('');
  const [movementType, setMovementType] = useState('');
  const [movementTypes, setMovementTypes] = useState<{ typeCode: string; displayName: string }[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const pageSize = 20;

  useEffect(() => {
    inventoryApi.getMovementTypes().then(res => {
      if (res.data?.data) setMovementTypes(res.data.data);
    });
    warehouseApi.getAll({ pageSize: 100 }).then(res => {
      if (res.data?.data?.items) setWarehouses(res.data.data.items);
    });
  }, []);

  useEffect(() => {
    setLoading(true);
    inventoryApi.getStockMovements({
      pageIndex: currentPage - 1,
      pageSize,
      warehouseId: selectedWarehouse || undefined,
      movementTypeCode: movementType || undefined,
      startDate: startDate || undefined,
      endDate: endDate || undefined,
    }).then(res => {
      if (res.data?.data) {
        setMovements(res.data.data.items || []);
        setTotalCount(res.data.data.totalCount || 0);
      }
    }).finally(() => setLoading(false));
  }, [currentPage, selectedWarehouse, movementType, startDate, endDate]);

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Stock Movements</h1>
          <p className="nx-page-subtitle">Track all stock-in and stock-out transactions</p>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b flex flex-wrap items-center gap-4">
          <select 
            className="nx-input nx-select w-40"
            value={selectedWarehouse}
            onChange={(e) => { setSelectedWarehouse(e.target.value); setCurrentPage(1); }}
          >
            <option value="">All Warehouses</option>
            {warehouses.map(wh => (
              <option key={wh.id} value={wh.id}>{wh.name}</option>
            ))}
          </select>
          <select 
            className="nx-input nx-select w-40"
            value={movementType}
            onChange={(e) => { setMovementType(e.target.value); setCurrentPage(1); }}
          >
            <option value="">All Types</option>
            {movementTypes.map(t => (
              <option key={t.typeCode} value={t.typeCode}>{t.displayName}</option>
            ))}
          </select>
          <input 
            type="date" 
            className="nx-input w-36"
            value={startDate}
            onChange={(e) => { setStartDate(e.target.value); setCurrentPage(1); }}
          />
          <span className="text-muted-foreground">to</span>
          <input 
            type="date" 
            className="nx-input w-36"
            value={endDate}
            onChange={(e) => { setEndDate(e.target.value); setCurrentPage(1); }}
          />
          <Button variant="outline" size="sm" onClick={() => { setStartDate(''); setEndDate(''); setCurrentPage(1); }}>
            Clear Dates
          </Button>
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
                    <th>Date</th>
                    <th>Product</th>
                    <th>Type</th>
                    <th>From Warehouse</th>
                    <th>To Warehouse</th>
                    <th style={{ textAlign: 'right' }}>Qty In</th>
                    <th style={{ textAlign: 'right' }}>Qty Out</th>
                    <th style={{ textAlign: 'right' }}>Balance</th>
                    <th>Reference</th>
                  </tr>
                </thead>
                <tbody>
                  {movements.map(m => (
                    <tr key={m.id}>
                      <td className="text-sm">{formatDate(m.occurredAt)}</td>
                      <td className="font-medium">{m.productName}</td>
                      <td>
                        <Badge variant="outline">{m.movementTypeName}</Badge>
                      </td>
                      <td>{m.fromWarehouseName || '-'}</td>
                      <td>{m.toWarehouseName || '-'}</td>
                      <td style={{ textAlign: 'right' }} className="text-green-600">
                        {m.quantityIn > 0 ? m.quantityIn : '-'}
                      </td>
                      <td style={{ textAlign: 'right' }} className="text-red-600">
                        {m.quantityOut > 0 ? m.quantityOut : '-'}
                      </td>
                      <td style={{ textAlign: 'right' }}>{m.balanceAfter}</td>
                      <td className="text-sm text-muted-foreground">
                        {m.referenceNumber || m.referenceType || '-'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {totalPages > 1 && (
              <div className="flex items-center justify-between p-4 border-t">
                <p className="text-sm text-muted-foreground">Showing {movements.length} of {totalCount}</p>
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
    </div>
  );
}