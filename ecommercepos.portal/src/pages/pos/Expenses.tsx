import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Filter, Edit, Trash2, ChevronLeft, ChevronRight,
  Loader2, X, DollarSign
} from 'lucide-react';
import { expenseApi, warehouseApiV2, type Expense, type Warehouse } from '@/api/posApi';

const EXPENSE_CATEGORIES = ['Rent', 'Utilities', 'Salary', 'Supplies', 'Transport', 'Maintenance', 'Marketing', 'Other'];
const PAYMENT_METHODS = ['Cash', 'Card', 'Bank Transfer', 'bKash', 'Other'];

interface ExpenseForm {
  warehouseId: string;
  category: string;
  description: string;
  amount: number;
  paymentMethod: string;
  expenseDate: string;
  notes: string;
}

const emptyForm: ExpenseForm = {
  warehouseId: '', category: '', description: '', amount: 0,
  paymentMethod: 'Cash', expenseDate: new Date().toISOString().split('T')[0], notes: '',
};

export default function Expenses() {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(10);

  // Filters
  const [filterWarehouse, setFilterWarehouse] = useState('');
  const [filterCategory, setFilterCategory] = useState('');

  // Modal
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<Expense | null>(null);
  const [formData, setFormData] = useState<ExpenseForm>(emptyForm);
  const [deleteTarget, setDeleteTarget] = useState<Expense | null>(null);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);

  const fetchExpenses = async () => {
    setLoading(true);
    try {
      const res = await expenseApi.getAll({
        pageIndex: currentPage - 1, pageSize,
        search: searchQuery || undefined,
        warehouseId: filterWarehouse || undefined,
        category: filterCategory || undefined,
      });
      const data = res.data as unknown as { items: Expense[]; totalCount: number };
      setExpenses(data?.items || []);
      setTotalCount(data?.totalCount || 0);
    } catch {
      /* ignore */
    } finally {
      setLoading(false);
    }
  };

  const loadWarehouses = async () => {
    try {
      const res = await warehouseApiV2.getAll({ pageSize: 100 });
      setWarehouses((res.data as unknown as { items: Warehouse[] })?.items || []);
    } catch { /* ignore */ }
  };

  useEffect(() => { fetchExpenses(); loadWarehouses(); }, [currentPage]);

  const handleSearch = () => { setCurrentPage(1); fetchExpenses(); };

  const openCreate = () => {
    setEditing(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEdit = (exp: Expense) => {
    setEditing(exp);
    setFormData({
      warehouseId: exp.warehouseId,
      category: exp.category,
      description: exp.description,
      amount: exp.amount,
      paymentMethod: exp.paymentMethod,
      expenseDate: exp.expenseDate ? exp.expenseDate.split('T')[0] : '',
      notes: exp.notes || '',
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      if (editing) {
        await expenseApi.update(editing.id, formData);
      } else {
        await expenseApi.create(formData);
      }
      setShowModal(false);
      fetchExpenses();
    } catch (err) {
      console.error('Save failed:', err);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setSaving(true);
    try {
      await expenseApi.delete(deleteTarget.id);
      setDeleteTarget(null);
      fetchExpenses();
    } catch (err) {
      console.error('Delete failed:', err);
    } finally {
      setSaving(false);
    }
  };

  const fmt = (n: number) => n.toLocaleString('en-BD', { style: 'currency', currency: 'BDT' });
  const totalPages = Math.ceil(totalCount / pageSize);
  const totalAmount = expenses.reduce((s, e) => s + e.amount, 0);

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Expenses</h1>
          <p className="nx-page-subtitle">Track business expenses</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreate}>
            <Plus className="w-4 h-4 mr-2" /> Add Expense
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Expenses</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{fmt(totalAmount)}</div>
          <div className="nx-stat-label">Page Total</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{expenses.length}</div>
          <div className="nx-stat-label">Showing</div>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input
                type="text"
                placeholder="Search expenses..."
                value={searchQuery}
                onChange={e => setSearchQuery(e.target.value)}
                onKeyDown={e => e.key === 'Enter' && handleSearch()}
              />
            </div>
            <div className="nx-table-filters">
              <select className="nx-input nx-select" value={filterWarehouse} onChange={e => setFilterWarehouse(e.target.value)}>
                <option value="">All Warehouses</option>
                {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
              </select>
              <select className="nx-input nx-select" value={filterCategory} onChange={e => setFilterCategory(e.target.value)}>
                <option value="">All Categories</option>
                {EXPENSE_CATEGORIES.map(c => <option key={c} value={c}>{c}</option>)}
              </select>
              <Button variant="outline" size="sm" onClick={handleSearch}>
                <Filter className="w-4 h-4 mr-2" /> Search
              </Button>
            </div>
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
                    <th>Category</th>
                    <th>Description</th>
                    <th>Amount</th>
                    <th>Method</th>
                    <th>Warehouse</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {expenses.map(exp => (
                    <tr key={exp.id}>
                      <td>{exp.expenseDate ? new Date(exp.expenseDate).toLocaleDateString() : '-'}</td>
                      <td><span className="nx-badge nx-badge-default">{exp.category}</span></td>
                      <td>{exp.description}</td>
                      <td className="font-medium">{fmt(exp.amount)}</td>
                      <td>{exp.paymentMethod}</td>
                      <td>{exp.warehouseName}</td>
                      <td>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEdit(exp)}>
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteTarget(exp)}>
                            <Trash2 className="w-4 h-4" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                  {expenses.length === 0 && (
                    <tr><td colSpan={7} className="text-center text-muted-foreground py-8">No expenses found</td></tr>
                  )}
                </tbody>
              </table>
            </div>
            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">Showing {expenses.length} of {totalCount}</p>
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

      {/* Create/Edit Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold flex items-center gap-2">
                <DollarSign className="w-5 h-5" />
                {editing ? 'Edit Expense' : 'Add Expense'}
              </h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}><X className="w-4 h-4" /></Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Warehouse *</label>
                  <select
                    className="nx-input nx-select w-full mt-1"
                    value={formData.warehouseId}
                    onChange={e => setFormData({ ...formData, warehouseId: e.target.value })}
                    required
                  >
                    <option value="">Select warehouse</option>
                    {warehouses.map(w => <option key={w.id} value={w.id}>{w.name}</option>)}
                  </select>
                </div>
                <div>
                  <label className="text-sm font-medium">Category *</label>
                  <select
                    className="nx-input nx-select w-full mt-1"
                    value={formData.category}
                    onChange={e => setFormData({ ...formData, category: e.target.value })}
                    required
                  >
                    <option value="">Select category</option>
                    {EXPENSE_CATEGORIES.map(c => <option key={c} value={c}>{c}</option>)}
                  </select>
                </div>
                <div className="col-span-2">
                  <label className="text-sm font-medium">Description *</label>
                  <Input value={formData.description} onChange={e => setFormData({ ...formData, description: e.target.value })} required className="mt-1" />
                </div>
                <div>
                  <label className="text-sm font-medium">Amount *</label>
                  <Input type="number" value={formData.amount} onChange={e => setFormData({ ...formData, amount: parseFloat(e.target.value) || 0 })} required className="mt-1" />
                </div>
                <div>
                  <label className="text-sm font-medium">Payment Method</label>
                  <select
                    className="nx-input nx-select w-full mt-1"
                    value={formData.paymentMethod}
                    onChange={e => setFormData({ ...formData, paymentMethod: e.target.value })}
                  >
                    {PAYMENT_METHODS.map(m => <option key={m} value={m}>{m}</option>)}
                  </select>
                </div>
                <div>
                  <label className="text-sm font-medium">Date *</label>
                  <Input type="date" value={formData.expenseDate} onChange={e => setFormData({ ...formData, expenseDate: e.target.value })} required className="mt-1" />
                </div>
                <div>
                  <label className="text-sm font-medium">Notes</label>
                  <Input value={formData.notes} onChange={e => setFormData({ ...formData, notes: e.target.value })} className="mt-1" />
                </div>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={saving}>
                  {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editing ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Delete Confirm */}
      {deleteTarget && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Expense</h2>
            <p className="text-muted-foreground mb-6">Are you sure you want to delete this expense: "{deleteTarget.description}"?</p>
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setDeleteTarget(null)}>Cancel</Button>
              <Button variant="destructive" onClick={handleDelete} disabled={saving}>
                {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                Delete
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
