import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { 
  Plus, Minus, Trash2, ShoppingCart, Search, 
  Loader2, Package, Calculator
} from 'lucide-react';
import { productApi, type Product } from '@/api/productApi';
import { posTransactionApi } from '@/api/posTransactionApi';

interface CartItem {
  id: string;
  productId: string;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

const paymentMethods = ['CASH', 'CARD', 'UPI', 'WALLET'];

export default function PosTerminal() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [cart, setCart] = useState<CartItem[]>([]);
  const [selectedPayment, setSelectedPayment] = useState('CASH');
  const [cashReceived, setCashReceived] = useState(0);
  const [showReceipt, setShowReceipt] = useState(false);
  const [lastTransaction, setLastTransaction] = useState<any>(null);
  const [customerName, setCustomerName] = useState('');
  const [customerPhone, setCustomerPhone] = useState('');

  const fetchProducts = async () => {
    setLoading(true);
    try {
      const response = await productApi.getAll({ pageIndex: 0, pageSize: 100, search: searchQuery || undefined });
      if (response.data?.items) {
        setProducts(response.data.items);
      }
    } catch (error) {
      console.error('Error fetching products:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => {
      if (searchQuery) fetchProducts();
    }, 300);
    return () => clearTimeout(timer);
  }, [searchQuery]);

  const addToCart = (product: Product) => {
    const existingItem = cart.find(item => item.productId === product.id);
    if (existingItem) {
      setCart(cart.map(item => 
        item.productId === product.id 
          ? { ...item, quantity: item.quantity + 1, lineTotal: (item.quantity + 1) * item.unitPrice }
          : item
      ));
    } else {
      setCart([...cart, {
        id: Date.now().toString(),
        productId: product.id,
        productName: product.productName || '',
        sku: product.sku || '',
        quantity: 1,
        unitPrice: product.sellPrice || 0,
        lineTotal: product.sellPrice || 0
      }]);
    }
  };

  const updateQuantity = (id: string, delta: number) => {
    setCart(cart.map(item => {
      if (item.productId === id) {
        const newQty = Math.max(1, item.quantity + delta);
        return { ...item, quantity: newQty, lineTotal: newQty * item.unitPrice };
      }
      return item;
    }));
  };

  const removeFromCart = (id: string) => {
    setCart(cart.filter(item => item.productId !== id));
  };

  const subtotal = cart.reduce((sum, item) => sum + item.lineTotal, 0);
  const tax = subtotal * 0.05;
  const grandTotal = subtotal + tax;
  const change = typeof cashReceived === 'number' ? cashReceived - grandTotal : 0;

  const handleCheckout = async () => {
    if (cart.length === 0) return;
    setSaving(true);
    try {
      const lines = cart.map(item => ({
        id: crypto.randomUUID(),
        productId: item.productId,
        productName: item.productName,
        sku: item.sku,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
        discountPercent: 0,
        discountAmount: 0,
        taxAmount: item.lineTotal * 0.05,
        lineTotal: item.lineTotal * 1.05
      }));

      const payments = [{
        id: crypto.randomUUID(),
        paymentMethod: selectedPayment,
        amount: cashReceived > 0 ? cashReceived : grandTotal
      }];

      const response = await posTransactionApi.create({
        cashShiftId: '00000000-0000-0000-0000-000000000001',
        posCounterId: '00000000-0000-0000-0000-000000000001',
        customerName,
        customerPhone,
        saleType: 'REGULAR',
        lines,
        payments
      });

      setLastTransaction(response.data);
      setShowReceipt(true);
      setCart([]);
      setCustomerName('');
      setCustomerPhone('');
      setCashReceived(0);
    } catch (error) {
      console.error('Error creating transaction:', error);
    } finally {
      setSaving(false);
    }
  };

  const filteredProducts = products.filter(p => 
    (p.productName || '').toLowerCase().includes(searchQuery.toLowerCase()) ||
    (p.sku || '').toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="flex h-[calc(100vh-4rem)] gap-4 p-4">
      {/* Product Grid */}
      <div className="flex-1 flex flex-col gap-4">
        <div className="flex gap-2">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input 
              placeholder="Search products..." 
              className="pl-9"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>
        </div>

        <div className="flex-1 overflow-auto">
          {loading ? (
            <div className="flex items-center justify-center h-full">
              <Loader2 className="w-8 h-8 animate-spin" />
            </div>
          ) : (
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-2">
              {filteredProducts.map((product) => (
                <button
                  key={product.id}
                  onClick={() => addToCart(product)}
                  className="p-3 bg-card border rounded-lg text-left hover:bg-accent transition-colors"
                >
                  <div className="w-full aspect-square bg-secondary rounded-lg flex items-center justify-center mb-2">
                    <Package className="w-8 h-8 text-muted-foreground" />
                  </div>
                  <p className="font-medium text-sm truncate">{product.productName}</p>
                  <p className="text-xs text-muted-foreground">{(product.sellPrice || 0).toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</p>
                </button>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Cart Panel */}
      <Card className="w-[400px] flex flex-col">
        <CardHeader className="pb-2">
          <div className="flex items-center justify-between">
            <CardTitle className="text-lg flex items-center gap-2">
              <ShoppingCart className="w-5 h-5" />
              Cart ({cart.length})
            </CardTitle>
            {cart.length > 0 && (
              <Button variant="ghost" size="sm" onClick={() => setCart([])}>
                Clear
              </Button>
            )}
          </div>
          <div className="flex gap-2">
            <Input 
              placeholder="Customer Name" 
              value={customerName}
              onChange={(e) => setCustomerName(e.target.value)}
              className="text-sm"
            />
            <Input 
              placeholder="Phone" 
              value={customerPhone}
              onChange={(e) => setCustomerPhone(e.target.value)}
              className="text-sm"
            />
          </div>
        </CardHeader>

        <CardContent className="flex-1 overflow-auto p-0">
          {cart.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-full text-muted-foreground">
              <ShoppingCart className="w-12 h-12 mb-2" />
              <p>Cart is empty</p>
            </div>
          ) : (
            <div className="divide-y">
              {cart.map((item) => (
                <div key={item.id} className="p-3 flex items-center gap-2">
                  <div className="flex-1">
                    <p className="font-medium text-sm">{item.productName}</p>
                    <p className="text-xs text-muted-foreground">
                      {item.unitPrice.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })} x {item.quantity}
                    </p>
                  </div>
                  <div className="flex items-center gap-1">
                    <Button variant="outline" size="icon" className="w-7 h-7" onClick={() => updateQuantity(item.productId, -1)}>
                      <Minus className="w-3 h-3" />
                    </Button>
                    <span className="w-8 text-center text-sm">{item.quantity}</span>
                    <Button variant="outline" size="icon" className="w-7 h-7" onClick={() => updateQuantity(item.productId, 1)}>
                      <Plus className="w-3 h-3" />
                    </Button>
                  </div>
                  <div className="w-20 text-right">
                    <p className="font-medium text-sm">{item.lineTotal.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</p>
                  </div>
                  <Button variant="ghost" size="icon" className="w-7 h-7 text-red-500" onClick={() => removeFromCart(item.productId)}>
                    <Trash2 className="w-4 h-4" />
                  </Button>
                </div>
              ))}
            </div>
          )}
        </CardContent>

        {cart.length > 0 && (
          <div className="p-4 border-t space-y-3">
            <div className="flex justify-between text-sm">
              <span>Subtotal</span>
              <span>{subtotal.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>Tax (5%)</span>
              <span>{tax.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
            </div>
            <div className="flex justify-between text-lg font-bold">
              <span>Total</span>
              <span>{grandTotal.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Payment Method</label>
              <div className="grid grid-cols-4 gap-2">
                {paymentMethods.map((method) => (
                  <Button
                    key={method}
                    variant={selectedPayment === method ? 'default' : 'outline'}
                    size="sm"
                    onClick={() => setSelectedPayment(method)}
                  >
                    {method}
                  </Button>
                ))}
              </div>
            </div>

            {selectedPayment === 'CASH' && (
              <div className="space-y-2">
                <label className="text-sm font-medium">Cash Received</label>
                <Input
                  type="number"
                  value={cashReceived}
                  onChange={(e) => setCashReceived(parseFloat(e.target.value) || 0)}
                  placeholder="0"
                />
                {cashReceived > 0 && (
                  <div className="flex justify-between text-sm">
                    <span>Change</span>
                    <span className="font-medium text-green-600">{change.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
                  </div>
                )}
              </div>
            )}

            <Button 
              className="w-full" 
              size="lg" 
              onClick={handleCheckout}
              disabled={saving || (selectedPayment === 'CASH' && cashReceived < grandTotal)}
            >
              {saving ? <Loader2 className="w-4 h-4 mr-2 animate-spin" /> : <Calculator className="w-4 h-4 mr-2" />}
              Checkout
            </Button>
          </div>
        )}
      </Card>

      {/* Receipt Modal */}
      {showReceipt && lastTransaction && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-sm p-6">
            <div className="text-center mb-4">
              <h2 className="text-2xl font-bold">RECEIPT</h2>
              <p className="text-sm text-muted-foreground">{lastTransaction.receiptNumber}</p>
            </div>
            <div className="space-y-2 border-t border-b py-4">
              <div className="flex justify-between">
                <span>Subtotal</span>
                <span>{(Number(lastTransaction.grandTotal) / 1.05).toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
              </div>
              <div className="flex justify-between">
                <span>Tax</span>
                <span>{(lastTransaction.grandTotal * 0.05).toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
              </div>
              <div className="flex justify-between font-bold text-lg">
                <span>Total</span>
                <span>{lastTransaction.grandTotal.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
              </div>
              <div className="flex justify-between">
                <span>Paid</span>
                <span>{lastTransaction.paidAmount.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
              </div>
              {lastTransaction.changeAmount > 0 && (
                <div className="flex justify-between text-green-600">
                  <span>Change</span>
                  <span>{lastTransaction.changeAmount.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' })}</span>
                </div>
              )}
            </div>
            <p className="text-center text-sm text-muted-foreground mt-4">Thank you for shopping!</p>
            <div className="flex gap-2 mt-4">
              <Button className="flex-1" onClick={() => setShowReceipt(false)}>New Sale</Button>
              <Button variant="outline" onClick={() => window.print()}>Print</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
