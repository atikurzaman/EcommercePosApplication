import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { 
  ChevronLeft, ChevronRight, Loader2, RefreshCw
} from 'lucide-react';
import { orderApi, Order, OrderDetail, OrderStats } from '@/api/orderApi';

const statusColors: Record<string, string> = {
  PENDING: 'warning',
  PROCESSING: 'info',
  CONFIRMED: 'info',
  SHIPPED: 'default',
  DELIVERED: 'success',
  CANCELLED: 'destructive',
};

function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-BD', { style: 'currency', currency: 'BDT', minimumFractionDigits: 0 }).format(amount);
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-BD', { 
    day: 'numeric', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' 
  });
}

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState('');
  const [totalCount, setTotalCount] = useState(0);
  const [stats, setStats] = useState<OrderStats | null>(null);
  const [selectedOrder, setSelectedOrder] = useState<OrderDetail | null>(null);
  const [orderLoading, setOrderLoading] = useState(false);
  const [showStatusDialog, setShowStatusDialog] = useState(false);
  const [newStatus, setNewStatus] = useState('');
  const pageSize = 15;

  useEffect(() => {
    orderApi.getStats().then(res => {
      if (res.data?.data) setStats(res.data.data);
    });
  }, []);

  useEffect(() => {
    setLoading(true);
    orderApi.getAll({
      pageIndex: currentPage - 1,
      pageSize,
      statusCode: statusFilter || undefined,
    }).then(res => {
      if (res.data?.data) {
        setOrders(res.data.data.items || []);
        setTotalCount(res.data.data.totalCount || 0);
      }
    }).finally(() => setLoading(false));
  }, [currentPage, statusFilter]);

  const handleViewOrder = async (id: string) => {
    setOrderLoading(true);
    try {
      const res = await orderApi.getById(id);
      if (res.data?.data) setSelectedOrder(res.data.data);
    } catch (error) {
      console.error('Error fetching order:', error);
    } finally {
      setOrderLoading(false);
    }
  };

  const handleUpdateStatus = async () => {
    if (!selectedOrder || !newStatus) return;
    try {
      await orderApi.updateStatus(selectedOrder.id, newStatus);
      setSelectedOrder(prev => prev ? { ...prev, statusCode: newStatus } : null);
      setShowStatusDialog(false);
      setNewStatus('');
    } catch (error) {
      console.error('Error updating status:', error);
    }
  };

  const handleCancelOrder = async () => {
    if (!selectedOrder) return;
    const reason = prompt('Enter cancellation reason:');
    if (!reason) return;
    try {
      await orderApi.cancel(selectedOrder.id, reason);
      setSelectedOrder(prev => prev ? { ...prev, statusCode: 'CANCELLED' } : null);
    } catch (error) {
      console.error('Error cancelling order:', error);
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Orders</h1>
          <p className="nx-page-subtitle">Manage and track customer orders</p>
        </div>
      </div>

      {stats && (
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
          <Card className="p-4">
            <div className="text-2xl font-bold">{stats.totalOrders}</div>
            <div className="text-sm text-muted-foreground">Total</div>
          </Card>
          <Card className="p-4">
            <div className="text-2xl font-bold text-yellow-600">{stats.pendingOrders}</div>
            <div className="text-sm text-muted-foreground">Pending</div>
          </Card>
          <Card className="p-4">
            <div className="text-2xl font-bold text-blue-600">{stats.processingOrders}</div>
            <div className="text-sm text-muted-foreground">Processing</div>
          </Card>
          <Card className="p-4">
            <div className="text-2xl font-bold text-purple-600">{stats.shippedOrders}</div>
            <div className="text-sm text-muted-foreground">Shipped</div>
          </Card>
          <Card className="p-4">
            <div className="text-2xl font-bold text-green-600">{stats.deliveredOrders}</div>
            <div className="text-sm text-muted-foreground">Delivered</div>
          </Card>
          <Card className="p-4">
            <div className="text-2xl font-bold">{formatCurrency(stats.todaySales)}</div>
            <div className="text-sm text-muted-foreground">Today's Sales</div>
          </Card>
        </div>
      )}

      <Card>
        <div className="p-4 border-b flex items-center justify-between">
          <select 
            className="nx-input nx-select w-40"
            value={statusFilter}
            onChange={(e) => { setStatusFilter(e.target.value); setCurrentPage(1); }}
          >
            <option value="">All Status</option>
            <option value="PENDING">Pending</option>
            <option value="CONFIRMED">Confirmed</option>
            <option value="PROCESSING">Processing</option>
            <option value="SHIPPED">Shipped</option>
            <option value="DELIVERED">Delivered</option>
            <option value="CANCELLED">Cancelled</option>
          </select>
          <Button variant="outline" size="sm" onClick={() => orderApi.getStats().then(res => { if (res.data?.data) setStats(res.data.data); })}>
            <RefreshCw className="w-4 h-4 mr-2" />
            Refresh Stats
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
                    <th>Order #</th>
                    <th>Customer</th>
                    <th>Phone</th>
                    <th>Status</th>
                    <th style={{ textAlign: 'right' }}>Total</th>
                    <th>Date</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {orders.map(order => (
                    <tr key={order.id}>
                      <td className="font-mono text-sm">{order.orderNumber}</td>
                      <td>{order.customerName}</td>
                      <td className="text-sm text-muted-foreground">{order.customerPhone}</td>
                      <td>
                        <Badge variant={statusColors[order.statusCode] as any || 'secondary'}>
                          {order.statusName}
                        </Badge>
                      </td>
                      <td style={{ textAlign: 'right' }} className="font-medium">
                        {formatCurrency(order.totalAmount)}
                      </td>
                      <td className="text-sm text-muted-foreground">{formatDate(order.orderDate)}</td>
                      <td>
                        <Button size="sm" variant="ghost" onClick={() => handleViewOrder(order.id)}>
                          View
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {totalPages > 1 && (
              <div className="flex items-center justify-between p-4 border-t">
                <p className="text-sm text-muted-foreground">Showing {orders.length} of {totalCount}</p>
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

      <Dialog open={!!selectedOrder} onOpenChange={(open) => !open && setSelectedOrder(null)}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Order #{selectedOrder?.orderNumber}</DialogTitle>
          </DialogHeader>
          {orderLoading ? (
            <div className="flex items-center justify-center p-8">
              <Loader2 className="w-8 h-8 animate-spin" />
            </div>
          ) : selectedOrder && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-muted-foreground">Customer</p>
                  <p className="font-medium">{selectedOrder.customerName}</p>
                  <p className="text-sm">{selectedOrder.customerPhone}</p>
                  {selectedOrder.customerEmail && <p className="text-sm text-muted-foreground">{selectedOrder.customerEmail}</p>}
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Status</p>
                  <Badge variant={statusColors[selectedOrder.statusCode] as any || 'secondary'}>
                    {selectedOrder.statusName}
                  </Badge>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Order Date</p>
                  <p className="font-medium">{formatDate(selectedOrder.orderDate)}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Warehouse</p>
                  <p className="font-medium">{selectedOrder.warehouseName || 'N/A'}</p>
                </div>
              </div>

              <div className="border-t pt-4">
                <p className="font-medium mb-2">Shipping Address</p>
                <p className="text-sm">{selectedOrder.shippingAddress.address}</p>
                <p className="text-sm">{selectedOrder.shippingAddress.city}</p>
              </div>

              <div className="border-t pt-4">
                <p className="font-medium mb-2">Order Items</p>
                <table className="nx-table text-sm">
                  <thead>
                    <tr>
                      <th>Product</th>
                      <th style={{ textAlign: 'right' }}>Qty</th>
                      <th style={{ textAlign: 'right' }}>Price</th>
                      <th style={{ textAlign: 'right' }}>Total</th>
                    </tr>
                  </thead>
                  <tbody>
                    {selectedOrder.items.map(item => (
                      <tr key={item.id}>
                        <td>
                          <p className="font-medium">{item.productName}</p>
                          {item.variantName && <p className="text-xs text-muted-foreground">{item.variantName}</p>}
                        </td>
                        <td style={{ textAlign: 'right' }}>{item.quantity}</td>
                        <td style={{ textAlign: 'right' }}>{formatCurrency(item.unitPrice)}</td>
                        <td style={{ textAlign: 'right' }}>{formatCurrency(item.totalPrice)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              <div className="border-t pt-4">
                <div className="flex justify-between">
                  <span>Subtotal</span>
                  <span>{formatCurrency(selectedOrder.subtotal)}</span>
                </div>
                <div className="flex justify-between">
                  <span>Shipping</span>
                  <span>{formatCurrency(selectedOrder.shippingAmount)}</span>
                </div>
                <div className="flex justify-between">
                  <span>Tax</span>
                  <span>{formatCurrency(selectedOrder.taxAmount)}</span>
                </div>
                <div className="flex justify-between">
                  <span>Discount</span>
                  <span>-{formatCurrency(selectedOrder.discountAmount)}</span>
                </div>
                <div className="flex justify-between font-semibold text-lg border-t pt-2 mt-2">
                  <span>Total</span>
                  <span>{formatCurrency(selectedOrder.totalAmount)}</span>
                </div>
              </div>

              {selectedOrder.payments.length > 0 && (
                <div className="border-t pt-4">
                  <p className="font-medium mb-2">Payments</p>
                  {selectedOrder.payments.map(p => (
                    <div key={p.id} className="flex justify-between text-sm">
                      <span>{p.paymentMethod}</span>
                      <span>{formatCurrency(p.amount)} ({p.statusCode})</span>
                    </div>
                  ))}
                </div>
              )}

              <div className="border-t pt-4 flex gap-2">
                {selectedOrder.statusCode !== 'CANCELLED' && selectedOrder.statusCode !== 'DELIVERED' && (
                  <>
                    <Button onClick={() => setShowStatusDialog(true)}>Update Status</Button>
                    <Button variant="destructive" onClick={handleCancelOrder}>Cancel Order</Button>
                  </>
                )}
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>

      <Dialog open={showStatusDialog} onOpenChange={setShowStatusDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Update Order Status</DialogTitle>
          </DialogHeader>
          <select 
            className="nx-input nx-select w-full"
            value={newStatus}
            onChange={(e) => setNewStatus(e.target.value)}
          >
            <option value="">Select status</option>
            <option value="PENDING">Pending</option>
            <option value="CONFIRMED">Confirmed</option>
            <option value="PROCESSING">Processing</option>
            <option value="SHIPPED">Shipped</option>
            <option value="DELIVERED">Delivered</option>
            <option value="CANCELLED">Cancelled</option>
          </select>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowStatusDialog(false)}>Cancel</Button>
            <Button onClick={handleUpdateStatus} disabled={!newStatus}>Update</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}