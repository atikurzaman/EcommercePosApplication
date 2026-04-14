import { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/hooks/useAuth';
import { usePermissions } from '@/hooks/usePermissions';
import { useTheme } from '@/contexts/ThemeContext';
import { MENU_IDS } from '@/hooks/usePermissions';
import {
  LayoutDashboard,
  Package,
  ShoppingCart,
  Users,
  Menu,
  Warehouse,
  Truck,
  FileText,
  BarChart3,
  Settings,
  LogOut,
  X,
  Receipt,
  DollarSign,
  UserPlus,
  Heart,
  Layers,
  Tag,
  Star,
  Archive,
  ClipboardList,
  TrendingUp,
  PackageX,
  RefreshCcw,
  Home,
  CreditCard,
  Calculator,
  Bell,
  Search,
  CircleDot,
  Palette,
  Coins,
  Crown,
  Percent,
  ArrowDownUp,
  PackageCheck,
  ListChecks,
  Database,
  Shield,
  KeyRound,
  LayoutList,
  UserCog,
  UserCircle,
  Sun,
  Moon,
} from 'lucide-react';

const catalogItems = [
  { href: '/products', label: 'Products', icon: Package, menuId: MENU_IDS.PRODUCTS },
  { href: '/categories', label: 'Categories', icon: Layers, menuId: MENU_IDS.CATEGORIES },
  { href: '/brands', label: 'Brands', icon: Tag, menuId: MENU_IDS.BRANDS },
  { href: '/tags', label: 'Tags', icon: Star },
  { href: '/collections', label: 'Collections', icon: Archive },
];

const salesItems = [
  { href: '/orders', label: 'Orders', icon: ShoppingCart, menuId: MENU_IDS.ORDERS },
  { href: '/invoices', label: 'Invoices', icon: FileText },
  { href: '/payments', label: 'Payments', icon: CreditCard },
  { href: '/shipments', label: 'Shipments', icon: Truck },
  { href: '/returns', label: 'Returns', icon: RefreshCcw },
];

const posItems = [
  { href: '/pos', label: 'POS Terminal', icon: ShoppingCart, menuId: MENU_IDS.POS_TERMINAL },
  { href: '/pos/transactions', label: 'Transactions', icon: Receipt },
  { href: '/pos/shifts', label: 'Cash Shifts', icon: Calculator },
  { href: '/pos/expenses', label: 'Expenses', icon: DollarSign },
  { href: '/pos/returns', label: 'Returns', icon: RefreshCcw },
  { href: '/pos/day-end', label: 'Day End', icon: FileText },
  { href: '/warehouses', label: 'Warehouses', icon: Warehouse },
];

const inventoryItems = [
  { href: '/inventory', label: 'Stock Items', icon: Package, menuId: MENU_IDS.INVENTORY_STOCK },
  { href: '/inventory/movements', label: 'Movements', icon: TrendingUp },
  { href: '/inventory/adjustments', label: 'Adjustments', icon: PackageX },
  { href: '/inventory/transfers', label: 'Transfers', icon: Truck },
];

const customerItems = [
  { href: '/customers', label: 'Customers', icon: Users, menuId: MENU_IDS.CUSTOMERS },
  { href: '/customers/profiles', label: 'Profiles', icon: UserPlus },
  { href: '/customers/addresses', label: 'Addresses', icon: Home },
  { href: '/customers/loyalty', label: 'Loyalty', icon: Heart },
];

const procurementItems = [
  { href: '/purchase-orders', label: 'Purchase Orders', icon: ClipboardList },
  { href: '/suppliers', label: 'Suppliers', icon: Truck },
];

const statusDefinitionItems = [
  { href: '/settings/order-statuses', label: 'Order Statuses', icon: ListChecks },
  { href: '/settings/payment-statuses', label: 'Payment Statuses', icon: CreditCard },
  { href: '/settings/shipment-statuses', label: 'Shipment Statuses', icon: Truck },
  { href: '/settings/return-statuses', label: 'Return Statuses', icon: RefreshCcw },
];

const securityItems = [
  { href: '/users', label: 'Users', icon: UserCog },
  { href: '/roles', label: 'Roles', icon: Shield },
  { href: '/permissions', label: 'Permissions', icon: KeyRound },
  { href: '/menus', label: 'Menus', icon: LayoutList },
];

const referenceDataItems = [
  { href: '/settings/payment-methods', label: 'Payment Methods', icon: CreditCard },
  { href: '/settings/discount-types', label: 'Discount Types', icon: Percent },
  { href: '/settings/customer-tiers', label: 'Customer Tiers', icon: Crown },
  { href: '/settings/product-conditions', label: 'Product Conditions', icon: PackageCheck },
  { href: '/settings/wishlist-types', label: 'Wishlist Types', icon: Heart },
  { href: '/settings/stock-movement-types', label: 'Movement Types', icon: ArrowDownUp },
  { href: '/currencies', label: 'Currencies', icon: Coins },
  { href: '/colors', label: 'Colors', icon: Palette },
];

interface NavSectionProps {
  title: string;
  icon: React.ElementType;
  items: { href: string; label: string; icon: React.ElementType; menuId?: string }[];
  defaultOpen?: boolean;
}

function NavSection({ title, icon: Icon, items, defaultOpen = false }: NavSectionProps) {
  const [isOpen, setIsOpen] = useState(defaultOpen);
  const location = useLocation();
  const { canAccess } = usePermissions();

  const visibleItems = items.filter(item => !item.menuId || canAccess(item.menuId));
  
  if (visibleItems.length === 0) return null;

  const isActiveSection = visibleItems.some(item => location.pathname.startsWith(item.href));

  useEffect(() => {
    if (isActiveSection) setIsOpen(true);
  }, [isActiveSection]);

  return (
    <div className="nx-nav-section">
      <div 
        className={cn("nx-nav-section-header", isOpen || isActiveSection ? "open" : "")}
        onClick={() => setIsOpen(!isOpen)}
      >
        <span className="flex items-center gap-2">
          <Icon className="w-4 h-4" style={{ opacity: 0.5 }} />
          {title}
        </span>
        <span className={cn("nx-caret", isOpen || isActiveSection ? "open" : "")}>▶</span>
      </div>
      <div className={cn("nx-nav-children", (isOpen || isActiveSection) ? "open" : "")}>
        {visibleItems.map((item) => {
          const ItemIcon = item.icon;
          const isActive = location.pathname === item.href;
          return (
            <Link
              key={item.href}
              to={item.href}
              className={cn("nx-nav-item", isActive ? "active" : "")}
            >
              <ItemIcon className="nx-icon" />
              <span>{item.label}</span>
            </Link>
          );
        })}
      </div>
    </div>
  );
}

export function Sidebar({ className, onClose }: { className?: string; onClose?: () => void }) {
  const location = useLocation();
  const { logout } = useAuth();
  const { theme, toggleTheme } = useTheme();

  return (
    <aside className={cn("flex flex-col h-full nx-sidebar", className)}>
      <div className="nx-sidebar-header">
        <div className="nx-sidebar-logo">N</div>
        <div className="nx-sidebar-brand">
          <span className="nx-sidebar-title">NEXUS</span>
          <span className="nx-sidebar-subtitle">Admin Portal</span>
        </div>
      </div>
      
      <div className="nx-sidebar-search">
        <Search className="w-4 h-4" style={{ opacity: 0.5 }} />
        <input type="text" placeholder="Search..." />
      </div>
      
      <nav className="flex-1 nx-sidebar-nav overflow-y-auto">
        <Link to="/" className={cn("nx-nav-item", location.pathname === "/" ? "active" : "")}>
          <LayoutDashboard className="nx-icon" />
          <span>Dashboard</span>
        </Link>
        
        <NavSection title="Catalog" icon={Layers} items={catalogItems} />
        <NavSection title="Sales" icon={ShoppingCart} items={salesItems} />
        <NavSection title="POS" icon={Receipt} items={posItems} />
        <NavSection title="Inventory" icon={Warehouse} items={inventoryItems} />
        <NavSection title="Customers" icon={Users} items={customerItems} />
        <NavSection title="Procurement" icon={ClipboardList} items={procurementItems} />
        <NavSection title="Status Definitions" icon={CircleDot} items={statusDefinitionItems} />
        <NavSection title="Reference Data" icon={Database} items={referenceDataItems} />
        <NavSection title="Access Control" icon={Shield} items={securityItems} />

        <Link to="/reports" className={cn("nx-nav-item", location.pathname.startsWith("/reports") ? "active" : "")}>
          <BarChart3 className="nx-icon" />
          <span>Reports</span>
        </Link>

        <Link to="/settings" className={cn("nx-nav-item", location.pathname === "/settings" ? "active" : "")}>
          <Settings className="nx-icon" />
          <span>Settings</span>
        </Link>
      </nav>
      
      <div className="p-2 border-t">
        <Button
          variant="ghost"
          size="sm"
          className="nx-theme-toggle w-full justify-start"
          onClick={toggleTheme}
        >
          {theme === 'light' ? <Moon className="w-4 h-4 mr-2" /> : <Sun className="w-4 h-4 mr-2" />}
          {theme === 'light' ? 'Dark Mode' : 'Light Mode'}
        </Button>
        <Button
          variant="ghost"
          size="sm"
          className="w-full justify-start text-muted-foreground hover:bg-accent"
          onClick={() => logout()}
        >
          <LogOut className="w-4 h-4 mr-2" />
          Logout
        </Button>
      </div>
    </aside>
  );
}

export function Header({ title }: { title?: string }) {
  const location = useLocation();
  const { user } = useAuth();
  const { toggleTheme, theme } = useTheme();
  const [pageTitle, setPageTitle] = useState(title || 'Dashboard');

  useEffect(() => {
    const path = location.pathname;
    if (path === '/') setPageTitle('Dashboard');
    else if (path === '/products') setPageTitle('Products');
    else if (path === '/orders') setPageTitle('Orders');
    else if (path === '/customers') setPageTitle('Customers');
    else if (path === '/categories') setPageTitle('Categories');
    else if (path === '/inventory') setPageTitle('Inventory');
    else if (path === '/pos') setPageTitle('POS Terminal');
    else if (path === '/reports') setPageTitle('Reports');
    else if (path === '/settings') setPageTitle('Settings');
    else setPageTitle(path.split('/')[1]?.charAt(0).toUpperCase() + path.split('/')[1]?.slice(1) || 'Dashboard');
  }, [location.pathname, title]);

  return (
    <header className="nx-topbar">
      <div className="nx-breadcrumb">
        <span>NEXUS</span>
        <span className="nx-breadcrumb-separator">/</span>
        <span className="nx-breadcrumb-current">{pageTitle}</span>
      </div>
      
      <div className="nx-topbar-search">
        <Search className="w-4 h-4" style={{ opacity: 0.5 }} />
        <input type="text" placeholder="Search anything..." />
      </div>
      
      <div className="nx-topbar-actions">
        <button className="nx-topbar-btn" title="Notifications">
          <Bell className="w-5 h-5" />
        </button>
        <button className="nx-topbar-btn" title="Toggle Theme" onClick={toggleTheme}>
          {theme === 'light' ? <Moon className="w-5 h-5" /> : <Sun className="w-5 h-5" />}
        </button>
        <Link to="/pos" className="nx-topbar-btn" title="POS">
          <ShoppingCart className="w-5 h-5" />
        </Link>
        <Link to="/settings" className="nx-topbar-btn" title="Settings">
          <Settings className="w-5 h-5" />
        </Link>
        <Link to="/profile" className="nx-topbar-btn" title="Profile">
          <UserCircle className="w-5 h-5" />
        </Link>
        <div className="nx-topbar-avatar">
          {user?.firstName?.charAt(0) || 'A'}{user?.lastName?.charAt(0) || 'U'}
        </div>
      </div>
    </header>
  );
}

interface MainLayoutProps {
  children: React.ReactNode;
}

export function MainLayout({ children }: MainLayoutProps) {
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <div className="nx-shell">
      <div className="hidden md:flex w-64 flex-col">
        <Sidebar />
      </div>
      
      <Button 
        variant="ghost" 
        size="icon" 
        className="md:hidden absolute top-4 left-4 z-50"
        onClick={() => setMobileOpen(true)}
      >
        <Menu className="w-5 h-5" />
      </Button>

      {mobileOpen && (
        <div className="fixed inset-0 z-40 md:hidden">
          <div 
            className="fixed inset-0 bg-black/50"
            onClick={() => setMobileOpen(false)}
          />
          <div className="fixed left-0 top-0 bottom-0 w-72 bg-sidebar z-50 overflow-y-auto">
            <div className="flex justify-end p-2">
              <Button 
                variant="ghost" 
                size="icon"
                onClick={() => setMobileOpen(false)}
              >
                <X className="w-5 h-5" />
              </Button>
            </div>
            <Sidebar onClose={() => setMobileOpen(false)} />
          </div>
        </div>
      )}

      <div className="flex-1 flex flex-col overflow-hidden">
        <Header />
        <main className="flex-1 overflow-auto nx-content">
          {children}
        </main>
      </div>
    </div>
  );
}
