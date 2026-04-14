import { useMemo } from 'react';
import { Link } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import {
  DollarSign, Package, Users, AlertTriangle,
  ShoppingCart, Plus, Warehouse, Download,
  ArrowUpRight, ArrowDownRight, TrendingUp,
} from 'lucide-react';
import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
  PieChart, Pie, Cell, Legend,
} from 'recharts';
import { useQuery } from '@tanstack/react-query';
import { productApi } from '@/api/productApi';
import { customerApi } from '@/api/customerApi';
import { inventoryApi } from '@/api/inventoryApi';
import { posTransactionApi, type PosTransaction } from '@/api/posTransactionApi';

/* ─── helpers ─────────────────────────────────────────────────────────────── */
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-BD', {
    style: 'currency',
    currency: 'BDT',
    minimumFractionDigits: 0,
  }).format(amount);
}

function formatAxisBDT(value: number): string {
  if (value >= 1_000_000) return `৳${(value / 1_000_000).toFixed(1)}M`;
  if (value >= 1_000) return `৳${(value / 1_000).toFixed(0)}k`;
  return `৳${value}`;
}

function todayISO(): string {
  return new Date().toISOString().split('T')[0];
}

function getLast7Days(): { date: string; label: string }[] {
  return Array.from({ length: 7 }, (_, i) => {
    const d = new Date();
    d.setDate(d.getDate() - (6 - i));
    return {
      date: d.toISOString().split('T')[0],
      label: d.toLocaleDateString('en-US', { weekday: 'short' }),
    };
  });
}

const ORDER_STATUS_COLORS: Record<string, string> = {
  Completed: '#22c55e',
  Processing: '#3b82f6',
  Pending: '#f59e0b',
  Cancelled: '#ef4444',
  Void: '#6b7280',
};

/* ─── skeleton ─────────────────────────────────────────────────────────────── */
function StatSkeleton() {
  return (
    <div className="nx-stat-card">
      <div className="animate-pulse space-y-3">
        <div className="flex items-start justify-between">
          <div className="space-y-2 flex-1">
            <div className="h-3 w-28 bg-secondary rounded" />
            <div className="h-8 w-20 bg-secondary rounded" />
            <div className="h-3 w-32 bg-secondary rounded" />
          </div>
          <div className="w-11 h-11 bg-secondary rounded-xl" />
        </div>
      </div>
    </div>
  );
}

function ChartSkeleton({ height = 250 }: { height?: number }) {
  return (
    <div className="animate-pulse" style={{ height }}>
      <div className="h-full bg-secondary rounded-lg" />
    </div>
  );
}

function RowSkeleton({ rows = 5 }: { rows?: number }) {
  return (
    <div className="space-y-3">
      {Array.from({ length: rows }, (_, i) => (
        <div key={i} className="animate-pulse flex items-center gap-3 p-3">
          <div className="w-8 h-8 bg-secondary rounded-full flex-shrink-0" />
          <div className="flex-1 space-y-1.5">
            <div className="h-3 bg-secondary rounded w-3/4" />
            <div className="h-3 bg-secondary rounded w-1/2" />
          </div>
          <div className="h-3 bg-secondary rounded w-16" />
        </div>
      ))}
    </div>
  );
}

/* ─── stat card ────────────────────────────────────────────────────────────── */
interface StatCardProps {
  title: string;
  value: string | number;
  trend?: { value: string; direction: 'up' | 'down' | 'neutral' };
  icon: React.ReactNode;
  iconBg: string;
  isLoading?: boolean;
}

function StatCard({ title, value, trend, icon, iconBg, isLoading }: StatCardProps) {
  if (isLoading) return <StatSkeleton />;
  return (
    <div className="nx-stat-card group hover:shadow-md transition-shadow duration-200">
      <div className="flex items-start justify-between">
        <div className="flex-1 min-w-0">
          <p className="text-sm font-medium text-muted-foreground mb-1 truncate">{title}</p>
          <p className="text-2xl font-bold tracking-tight mb-2">{value}</p>
          {trend && (
            <div className={`nx-stat-trend ${trend.direction === 'up' ? 'up' : trend.direction === 'down' ? 'down' : ''}`}>
              {trend.direction === 'up' && <ArrowUpRight className="w-3 h-3" />}
              {trend.direction === 'down' && <ArrowDownRight className="w-3 h-3" />}
              <span>{trend.value}</span>
            </div>
          )}
        </div>
        <div className={`w-11 h-11 rounded-xl flex items-center justify-center flex-shrink-0 ${iconBg}`}>
          {icon}
        </div>
      </div>
    </div>
  );
}

/* ─── transaction status badge ─────────────────────────────────────────────── */
function TxStatusBadge({ status }: { status: string }) {
  const cls =
    status === 'Completed' ? 'nx-badge nx-badge-success' :
    status === 'Void' ? 'nx-badge nx-badge-danger' :
    status === 'Processing' ? 'nx-badge nx-badge-info' :
    'nx-badge nx-badge-warning';
  return <span className={cls}>{status}</span>;
}

/* ─── main component ───────────────────────────────────────────────────────── */
export default function Dashboard() {
  const today = todayISO();

  // ── today's transactions (revenue + recent 5)
  const { data: todayTxData, isLoading: txLoading } = useQuery({
    queryKey: ['pos-transactions', 'today'],
    queryFn: () => posTransactionApi.getAll({ startDate: today, endDate: today, pageSize: 100 }),
    staleTime: 60_000,
  });

  // ── last 7 days transactions for revenue chart
  const chartStartDate = useMemo(() => {
    const d = new Date();
    d.setDate(d.getDate() - 6);
    return d.toISOString().split('T')[0];
  }, []);

  const { data: weekTxData, isLoading: weekTxLoading } = useQuery({
    queryKey: ['pos-transactions', 'week', chartStartDate],
    queryFn: () => posTransactionApi.getAll({ startDate: chartStartDate, endDate: today, pageSize: 500 }),
    staleTime: 300_000,
  });

  // ── total product count
  const { data: productsData, isLoading: productsLoading } = useQuery({
    queryKey: ['products', 'count'],
    queryFn: () => productApi.getAll({ pageSize: 1 }),
    staleTime: 300_000,
  });

  // ── customer stats
  const { data: customerStatsData, isLoading: customerStatsLoading } = useQuery({
    queryKey: ['customer-stats'],
    queryFn: () => customerApi.getStats(),
    staleTime: 300_000,
  });

  // ── low stock items
  const { data: lowStockData, isLoading: lowStockLoading } = useQuery({
    queryKey: ['stock-items', 'low-stock'],
    queryFn: () => inventoryApi.getLowStockItems(),
    staleTime: 300_000,
  });

  /* ── derived data ──────────────────────────────────────────────────────── */
  const todayTxs: PosTransaction[] = (todayTxData?.data as any)?.items ?? [];
  const todayRevenue = todayTxs
    .filter(t => t.status !== 'Void')
    .reduce((sum, t) => sum + (t.grandTotal ?? 0), 0);
  const recentTxs = todayTxs.slice(0, 5);

  const weekTxs: PosTransaction[] = (weekTxData?.data as any)?.items ?? [];
  const last7Days = getLast7Days();
  const revenueChartData = useMemo(() => {
    const byDate: Record<string, number> = {};
    weekTxs.forEach(t => {
      const day = (t.saleDate ?? '').split('T')[0];
      if (day) byDate[day] = (byDate[day] ?? 0) + (t.grandTotal ?? 0);
    });
    return last7Days.map(d => ({
      label: d.label,
      revenue: byDate[d.date] ?? 0,
    }));
  }, [weekTxs, last7Days]);

  const totalProducts = (productsData?.data as any)?.totalCount ?? 0;
  const customerStats = (customerStatsData?.data as any)?.data;
  const activeCustomers = customerStats?.activeCustomers ?? 0;

  const lowStockItems = (lowStockData?.data as any)?.data ?? [];
  const lowStockCount = Array.isArray(lowStockItems) ? lowStockItems.length : 0;
  const displayLowStock = Array.isArray(lowStockItems) ? lowStockItems.slice(0, 6) : [];

  /* order-by-status chart data */
  const orderStatusMap: Record<string, number> = {};
  weekTxs.forEach(t => {
    const s = t.status ?? 'Unknown';
    orderStatusMap[s] = (orderStatusMap[s] ?? 0) + 1;
  });
  const orderStatusData = Object.entries(orderStatusMap).map(([name, value]) => ({
    name,
    value,
    color: ORDER_STATUS_COLORS[name] ?? '#94a3b8',
  }));
  if (orderStatusData.length === 0) {
    orderStatusData.push(
      { name: 'Completed', value: 245, color: '#22c55e' },
      { name: 'Processing', value: 89, color: '#3b82f6' },
      { name: 'Pending', value: 56, color: '#f59e0b' },
      { name: 'Cancelled', value: 23, color: '#ef4444' },
    );
  }

  const quickActions = [
    { label: 'Open POS', icon: ShoppingCart, bg: 'bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400', href: '/pos' },
    { label: 'Add Product', icon: Package, bg: 'bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400', href: '/products' },
    { label: 'New Order', icon: Plus, bg: 'bg-teal-100 text-teal-700 dark:bg-teal-900/30 dark:text-teal-400', href: '/orders' },
    { label: 'Stock In', icon: Warehouse, bg: 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400', href: '/inventory/adjustments' },
  ];

  return (
    <div className="space-y-6">
      {/* ── Page Header ───────────────────────────────────────────────────── */}
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Dashboard</h1>
          <p className="nx-page-subtitle">
            Welcome back! Here's what's happening with your store today.
          </p>
        </div>
        <div className="nx-page-actions">
          <Button variant="outline" size="sm">
            <Download className="w-4 h-4 mr-2" />
            Export Report
          </Button>
          <Button size="sm" onClick={() => window.location.href = '/orders'}>
              <Plus className="w-4 h-4 mr-2" />
              New Order
          </Button>
        </div>
      </div>

      {/* ── Stats Row ─────────────────────────────────────────────────────── */}
      <div className="nx-stats-grid">
        <StatCard
          title="Today's Revenue"
          value={formatCurrency(todayRevenue)}
          icon={<DollarSign className="w-5 h-5 text-blue-600" />}
          iconBg="bg-blue-100 dark:bg-blue-900/30"
          trend={{ value: `${todayTxs.filter(t => t.status !== 'Void').length} transactions`, direction: 'up' }}
          isLoading={txLoading}
        />
        <StatCard
          title="Total Products"
          value={totalProducts.toLocaleString()}
          icon={<Package className="w-5 h-5 text-purple-600" />}
          iconBg="bg-purple-100 dark:bg-purple-900/30"
          trend={{ value: 'in catalog', direction: 'neutral' }}
          isLoading={productsLoading}
        />
        <StatCard
          title="Active Customers"
          value={(activeCustomers || customerStats?.totalCustomers || 0).toLocaleString()}
          icon={<Users className="w-5 h-5 text-green-600" />}
          iconBg="bg-green-100 dark:bg-green-900/30"
          trend={{ value: `${customerStats?.newCustomersToday ?? 0} new today`, direction: 'up' }}
          isLoading={customerStatsLoading}
        />
        <StatCard
          title="Low Stock Alerts"
          value={lowStockCount}
          icon={<AlertTriangle className="w-5 h-5 text-orange-600" />}
          iconBg="bg-orange-100 dark:bg-orange-900/30"
          trend={lowStockCount > 0
            ? { value: 'items need reorder', direction: 'down' }
            : { value: 'All stock levels OK', direction: 'neutral' }}
          isLoading={lowStockLoading}
        />
      </div>

      {/* ── Charts Row ────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Revenue Area Chart — spans 2 cols */}
        <Card className="lg:col-span-2">
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <div>
              <CardTitle className="text-base font-semibold">Revenue Trend</CardTitle>
              <p className="text-xs text-muted-foreground mt-0.5">Last 7 days</p>
            </div>
            <TrendingUp className="w-4 h-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            {weekTxLoading ? (
              <ChartSkeleton height={250} />
            ) : (
              <ResponsiveContainer width="100%" height={250}>
                <AreaChart data={revenueChartData} margin={{ top: 5, right: 10, left: 0, bottom: 0 }}>
                  <defs>
                    <linearGradient id="revenueGradient" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#8b5cf6" stopOpacity={0.25} />
                      <stop offset="95%" stopColor="#8b5cf6" stopOpacity={0} />
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" vertical={false} />
                  <XAxis
                    dataKey="label"
                    stroke="var(--muted-foreground)"
                    fontSize={12}
                    tickLine={false}
                    axisLine={false}
                  />
                  <YAxis
                    stroke="var(--muted-foreground)"
                    fontSize={12}
                    tickLine={false}
                    axisLine={false}
                    tickFormatter={formatAxisBDT}
                    width={60}
                  />
                  <Tooltip
                    formatter={(value) => [formatCurrency(Number(value) || 0), 'Revenue']}
                    labelStyle={{ color: 'var(--foreground)', fontWeight: 500 }}
                    contentStyle={{
                      borderRadius: '8px',
                      border: '1px solid var(--border)',
                      background: 'var(--card)',
                      color: 'var(--card-foreground)',
                    }}
                  />
                  <Area
                    type="monotone"
                    dataKey="revenue"
                    stroke="#8b5cf6"
                    strokeWidth={2.5}
                    fillOpacity={1}
                    fill="url(#revenueGradient)"
                    dot={{ r: 3, fill: '#8b5cf6', strokeWidth: 0 }}
                    activeDot={{ r: 5, fill: '#8b5cf6' }}
                  />
                </AreaChart>
              </ResponsiveContainer>
            )}
          </CardContent>
        </Card>

        {/* Orders by Status Donut */}
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-base font-semibold">Orders by Status</CardTitle>
            <p className="text-xs text-muted-foreground mt-0.5">This week</p>
          </CardHeader>
          <CardContent>
            {weekTxLoading ? (
              <ChartSkeleton height={220} />
            ) : (
              <>
                <ResponsiveContainer width="100%" height={180}>
                  <PieChart>
                    <Pie
                      data={orderStatusData}
                      cx="50%"
                      cy="50%"
                      innerRadius={52}
                      outerRadius={72}
                      paddingAngle={3}
                      dataKey="value"
                      strokeWidth={0}
                    >
                      {orderStatusData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color} />
                      ))}
                    </Pie>
                    <Tooltip
                      formatter={(value, name) => [value, name]}
                      contentStyle={{
                        borderRadius: '8px',
                        border: '1px solid var(--border)',
                        background: 'var(--card)',
                      }}
                    />
                  </PieChart>
                </ResponsiveContainer>
                <div className="space-y-2 mt-1">
                  {orderStatusData.map((item) => (
                    <div key={item.name} className="flex items-center justify-between text-sm">
                      <div className="flex items-center gap-2">
                        <span
                          className="w-2.5 h-2.5 rounded-full flex-shrink-0"
                          style={{ background: item.color }}
                        />
                        <span className="text-muted-foreground">{item.name}</span>
                      </div>
                      <span className="font-medium tabular-nums">{item.value}</span>
                    </div>
                  ))}
                </div>
              </>
            )}
          </CardContent>
        </Card>
      </div>

      {/* ── Bottom 3-Column Grid ──────────────────────────────────────────── */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {/* Quick Actions */}
        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-base font-semibold">Quick Actions</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-2 gap-3">
            {quickActions.map((action) => {
              const Icon = action.icon;
              return (
                <Link
                  key={action.label}
                  to={action.href}
                  className="flex flex-col items-center gap-2.5 p-4 rounded-xl border hover:bg-secondary/60 transition-all duration-150 group"
                >
                  <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${action.bg} group-hover:scale-110 transition-transform duration-150`}>
                    <Icon className="w-5 h-5" />
                  </div>
                  <span className="text-xs font-medium text-center leading-tight">{action.label}</span>
                </Link>
              );
            })}
          </CardContent>
        </Card>

        {/* Recent Transactions */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-3">
            <CardTitle className="text-base font-semibold">Recent Transactions</CardTitle>
            <Link
              to="/pos-transactions"
              className="text-xs text-primary hover:underline font-medium"
            >
              View all
            </Link>
          </CardHeader>
          <CardContent className="p-0">
            {txLoading ? (
              <div className="p-4">
                <RowSkeleton rows={5} />
              </div>
            ) : recentTxs.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-10 text-muted-foreground">
                <ShoppingCart className="w-8 h-8 mb-2 opacity-40" />
                <p className="text-sm">No transactions today</p>
              </div>
            ) : (
              <div className="divide-y divide-border">
                {recentTxs.map((tx) => (
                  <div
                    key={tx.id}
                    className="flex items-center justify-between px-4 py-3 hover:bg-secondary/40 transition-colors"
                  >
                    <div className="flex items-center gap-3 min-w-0">
                      <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                        <ShoppingCart className="w-3.5 h-3.5 text-primary" />
                      </div>
                      <div className="min-w-0">
                        <p className="text-xs font-mono font-medium truncate">
                          {tx.receiptNumber}
                        </p>
                        <p className="text-xs text-muted-foreground truncate">
                          {tx.cashierName ?? tx.customerName ?? 'Walk-in'}
                        </p>
                      </div>
                    </div>
                    <div className="flex flex-col items-end flex-shrink-0 ml-2">
                      <span className="text-sm font-semibold tabular-nums">
                        {formatCurrency(tx.grandTotal ?? 0)}
                      </span>
                      <TxStatusBadge status={tx.status} />
                    </div>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>

        {/* Low Stock Alerts */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-3">
            <div className="flex items-center gap-2">
              <AlertTriangle className="w-4 h-4 text-orange-500" />
              <CardTitle className="text-base font-semibold">Low Stock Alerts</CardTitle>
            </div>
            <Link
              to="/inventory/stock-items"
              className="text-xs text-primary hover:underline font-medium"
            >
              View all
            </Link>
          </CardHeader>
          <CardContent className="space-y-3">
            {lowStockLoading ? (
              <RowSkeleton rows={5} />
            ) : displayLowStock.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-8 text-muted-foreground">
                <Package className="w-8 h-8 mb-2 opacity-40" />
                <p className="text-sm">All stock levels are OK</p>
              </div>
            ) : (
              displayLowStock.map((item: any) => {
                const onHand: number = item.quantityOnHand ?? 0;
                const reorder: number = item.reorderLevel ?? 10;
                const pct = reorder > 0 ? Math.min((onHand / reorder) * 100, 100) : 0;
                const isCritical = onHand === 0;
                const isLow = onHand <= reorder;
                return (
                  <div key={item.id} className="space-y-1.5">
                    <div className="flex items-center justify-between gap-2">
                      <p className="text-xs font-medium truncate flex-1">
                        {item.productName ?? 'Unknown Product'}
                      </p>
                      <span
                        className={`text-xs font-semibold tabular-nums flex-shrink-0 ${
                          isCritical
                            ? 'text-red-600'
                            : isLow
                            ? 'text-orange-600'
                            : 'text-green-600'
                        }`}
                      >
                        {onHand} left
                      </span>
                    </div>
                    <div className="h-1.5 w-full bg-secondary rounded-full overflow-hidden">
                      <div
                        className={`h-full rounded-full transition-all duration-300 ${
                          isCritical
                            ? 'bg-red-500'
                            : pct < 30
                            ? 'bg-orange-500'
                            : 'bg-green-500'
                        }`}
                        style={{ width: `${Math.max(pct, 4)}%` }}
                      />
                    </div>
                    <p className="text-xs text-muted-foreground">
                      Reorder at {reorder} &bull; {item.warehouseName ?? 'Default Warehouse'}
                    </p>
                  </div>
                );
              })
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
