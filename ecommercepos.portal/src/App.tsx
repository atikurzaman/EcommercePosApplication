import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from '@/hooks/useAuth';
import { ThemeProvider } from '@/contexts/ThemeContext';
import { MainLayout } from '@/layouts';
import { Dashboard, Products, Orders, Customers, Categories, Brands, Suppliers, Employees, PosTerminal, Attributes, Tags, Units, StockItems, StockMovements, InventoryAdjustments, StockTransfers, OrderStatuses, PaymentStatuses, PaymentMethods, ShipmentStatuses, ReturnStatuses, DiscountTypes, CustomerTiers, ProductConditions, WishlistTypes, StockMovementTypes, Currencies, Colors, UsersPage, RolesPage, PermissionsPage, MenusPage, UserProfile, ProductDetail, AttributeTypes, Collections, PosTerminalPage, CashShifts, PosTransactions, Expenses, DayEndSummaries, PosReturns, Warehouses } from '@/pages';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import Login from '@/pages/Login';
import { Outlet } from 'react-router-dom';

function AppRoutes() {
  const { isAuthenticated } = useAuth();

  return (
    <Routes>
      <Route 
        path="/login" 
        element={isAuthenticated ? <Navigate to="/" replace /> : <Login />} 
      />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <MainLayout>
              <Outlet />
            </MainLayout>
          </ProtectedRoute>
        }
      >
        <Route index element={<Dashboard />} />
        <Route path="products" element={<Products />} />
        <Route path="products/:id" element={<ProductDetail />} />
        <Route path="categories" element={<Categories />} />
        <Route path="brands" element={<Brands />} />
        <Route path="attributes" element={<AttributeTypes />} />
        <Route path="tags" element={<Tags />} />
        <Route path="collections" element={<Collections />} />
        <Route path="orders" element={<Orders />} />
        <Route path="invoices" element={<Orders />} />
        <Route path="payments" element={<Orders />} />
        <Route path="shipments" element={<Orders />} />
        <Route path="returns" element={<Orders />} />
        <Route path="pos" element={<PosTerminalPage />} />
        <Route path="pos/terminal" element={<PosTerminalPage />} />
        <Route path="pos/transactions" element={<PosTransactions />} />
        <Route path="pos/shifts" element={<CashShifts />} />
        <Route path="pos/expenses" element={<Expenses />} />
        <Route path="pos/returns" element={<PosReturns />} />
        <Route path="pos/day-end" element={<DayEndSummaries />} />
        <Route path="inventory" element={<StockItems />} />
        <Route path="inventory/stock" element={<StockItems />} />
        <Route path="inventory/movements" element={<StockMovements />} />
        <Route path="inventory/adjustments" element={<InventoryAdjustments />} />
        <Route path="inventory/transfers" element={<StockTransfers />} />
        <Route path="customers" element={<Customers />} />
        <Route path="customers/profiles" element={<Customers />} />
        <Route path="customers/addresses" element={<Customers />} />
        <Route path="customers/loyalty" element={<Customers />} />
        <Route path="purchase-orders" element={<Dashboard />} />
        <Route path="suppliers" element={<Suppliers />} />
        <Route path="reports" element={<Dashboard />} />
        <Route path="settings" element={<Dashboard />} />
        <Route path="branches" element={<Dashboard />} />
        <Route path="expenses" element={<Dashboard />} />
        <Route path="employees" element={<Employees />} />
        <Route path="warehouses" element={<Warehouses />} />
        <Route path="tax-rates" element={<Dashboard />} />
        <Route path="units" element={<Units />} />
        <Route path="shipping-methods" element={<Dashboard />} />
        <Route path="payment-methods" element={<PaymentMethods />} />
        <Route path="delivery-zones" element={<Dashboard />} />
        <Route path="pickup-points" element={<Dashboard />} />
        <Route path="currencies" element={<Currencies />} />
        <Route path="colors" element={<Colors />} />
        <Route path="settings/order-statuses" element={<OrderStatuses />} />
        <Route path="settings/payment-statuses" element={<PaymentStatuses />} />
        <Route path="settings/payment-methods" element={<PaymentMethods />} />
        <Route path="settings/shipment-statuses" element={<ShipmentStatuses />} />
        <Route path="settings/return-statuses" element={<ReturnStatuses />} />
        <Route path="settings/discount-types" element={<DiscountTypes />} />
        <Route path="settings/customer-tiers" element={<CustomerTiers />} />
        <Route path="settings/product-conditions" element={<ProductConditions />} />
        <Route path="settings/wishlist-types" element={<WishlistTypes />} />
        <Route path="settings/stock-movement-types" element={<StockMovementTypes />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="roles" element={<RolesPage />} />
        <Route path="permissions" element={<PermissionsPage />} />
        <Route path="menus" element={<MenusPage />} />
        <Route path="profile" element={<UserProfile />} />
      </Route>
    </Routes>
  );
}

function App() {
  return (
    <BrowserRouter>
      <ThemeProvider>
        <AuthProvider>
          <AppRoutes />
        </AuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  );
}

export default App;
