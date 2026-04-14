import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { 
  Plus, Search, Filter, Edit, Trash2, Truck, 
  ChevronLeft, ChevronRight, Loader2, X, Mail, Phone
} from 'lucide-react';
import { supplierApi, type Supplier } from '@/api/supplierApi';

const statusColors: Record<string, string> = {
  true: 'nx-badge-success',
  false: 'nx-badge-danger',
};

interface SupplierFormData {
  supplierCode: string;
  supplierName: string;
  contactPerson: string;
  email: string;
  phone: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  country: string;
  taxId: string;
  isActive: boolean;
}

const emptyForm: SupplierFormData = {
  supplierCode: '',
  supplierName: '',
  contactPerson: '',
  email: '',
  phone: '',
  addressLine1: '',
  addressLine2: '',
  city: '',
  country: '',
  taxId: '',
  isActive: true,
};

export default function Suppliers() {
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState('all');
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(10);
  
  const [showModal, setShowModal] = useState(false);
  const [editingSupplier, setEditingSupplier] = useState<Supplier | null>(null);
  const [formData, setFormData] = useState<SupplierFormData>(emptyForm);
  const [deleteModal, setDeleteModal] = useState<Supplier | null>(null);

  const fetchSuppliers = async () => {
    setLoading(true);
    try {
      const response = await supplierApi.getAll({ 
        pageIndex: currentPage - 1, 
        pageSize,
        search: searchQuery || undefined 
      });
      if (response.data?.items) {
        setSuppliers(response.data.items);
        setTotalCount(response.data.totalCount);
      }
    } catch (error) {
      console.error('Error fetching suppliers:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSuppliers();
  }, [currentPage, statusFilter]);

  const handleSearch = () => {
    setCurrentPage(1);
    fetchSuppliers();
  };

  const handlePageChange = (page: number) => setCurrentPage(page);

  const openCreateModal = () => {
    setEditingSupplier(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (supplier: Supplier) => {
    setEditingSupplier(supplier);
    setFormData({
      supplierCode: supplier.supplierCode || '',
      supplierName: supplier.supplierName || '',
      contactPerson: supplier.contactPerson || '',
      email: supplier.email || '',
      phone: supplier.phone || '',
      addressLine1: supplier.addressLine1 || '',
      addressLine2: supplier.addressLine2 || '',
      city: supplier.city || '',
      country: supplier.country || '',
      taxId: supplier.taxId || '',
      isActive: supplier.isActive ?? true,
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      if (editingSupplier) {
        await supplierApi.update(editingSupplier.id, formData);
      } else {
        await supplierApi.create(formData);
      }
      setShowModal(false);
      fetchSuppliers();
    } catch (error) {
      console.error('Error saving supplier:', error);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteModal) return;
    setSaving(true);
    try {
      await supplierApi.delete(deleteModal.id);
      setDeleteModal(null);
      fetchSuppliers();
    } catch (error) {
      console.error('Error deleting supplier:', error);
    } finally {
      setSaving(false);
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);
  const activeCount = suppliers.filter(s => s.isActive).length;

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Suppliers</h1>
          <p className="nx-page-subtitle">Manage supplier database</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Supplier
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Suppliers</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value success">{activeCount}</div>
          <div className="nx-stat-label">Active</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount - activeCount}</div>
          <div className="nx-stat-label">Inactive</div>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input 
                type="text" 
                placeholder="Search suppliers..." 
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
              />
            </div>
            <div className="nx-table-filters">
              <select className="nx-input nx-select" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
                <option value="all">All Status</option>
                <option value="true">Active</option>
                <option value="false">Inactive</option>
              </select>
              <Button variant="outline" size="sm" onClick={handleSearch}>
                <Filter className="w-4 h-4 mr-2" />
                Search
              </Button>
            </div>
          </div>
        </div>

        {loading ? (
          <div className="flex items-center justify-center p-8">
            <Loader2 className="w-8 h-8 animate-spin" />
          </div>
        ) : (
          <>
            <div className="nx-table-wrap">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Code</th>
                    <th>Supplier</th>
                    <th>Contact</th>
                    <th>City</th>
                    <th>Status</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {suppliers.map((supplier) => (
                    <tr key={supplier.id}>
                      <td><code className="text-xs bg-secondary px-2 py-1 rounded">{supplier.supplierCode}</code></td>
                      <td>
                        <div className="flex items-center gap-3">
                          <div className="w-8 h-8 bg-secondary rounded-lg flex items-center justify-center">
                            <Truck className="w-4 h-4 text-muted-foreground" />
                          </div>
                          <div>
                            <p className="font-medium">{supplier.supplierName}</p>
                            <p className="text-xs text-muted-foreground">{supplier.contactPerson}</p>
                          </div>
                        </div>
                      </td>
                      <td>
                        <div className="text-sm">
                          {supplier.phone && <p className="flex items-center gap-1"><Phone className="w-3 h-3" /> {supplier.phone}</p>}
                          {supplier.email && <p className="flex items-center gap-1"><Mail className="w-3 h-3" /> {supplier.email}</p>}
                        </div>
                      </td>
                      <td>{supplier.city || '-'}</td>
                      <td>
                        <span className={`nx-badge ${statusColors[String(supplier.isActive)]}`}>
                          {supplier.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEditModal(supplier)}>
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteModal(supplier)}>
                            <Trash2 className="w-4 h-4" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">Showing {suppliers.length} of {totalCount}</p>
              <div className="flex items-center gap-2">
                <Button variant="outline" size="sm" disabled={currentPage === 1} onClick={() => handlePageChange(currentPage - 1)}>
                  <ChevronLeft className="w-4 h-4" />
                </Button>
                <span className="text-sm">Page {currentPage} of {totalPages || 1}</span>
                <Button variant="outline" size="sm" disabled={currentPage >= totalPages} onClick={() => handlePageChange(currentPage + 1)}>
                  <ChevronRight className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </>
        )}
      </Card>

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-2xl max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editingSupplier ? 'Edit Supplier' : 'Add Supplier'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Supplier Code *</label>
                  <Input value={formData.supplierCode} onChange={(e) => setFormData({...formData, supplierCode: e.target.value})} required />
                </div>
                <div>
                  <label className="text-sm font-medium">Supplier Name *</label>
                  <Input value={formData.supplierName} onChange={(e) => setFormData({...formData, supplierName: e.target.value})} required />
                </div>
                <div>
                  <label className="text-sm font-medium">Contact Person</label>
                  <Input value={formData.contactPerson} onChange={(e) => setFormData({...formData, contactPerson: e.target.value})} />
                </div>
                <div>
                  <label className="text-sm font-medium">Phone</label>
                  <Input value={formData.phone} onChange={(e) => setFormData({...formData, phone: e.target.value})} />
                </div>
                <div>
                  <label className="text-sm font-medium">Email</label>
                  <Input type="email" value={formData.email} onChange={(e) => setFormData({...formData, email: e.target.value})} />
                </div>
                <div>
                  <label className="text-sm font-medium">City</label>
                  <Input value={formData.city} onChange={(e) => setFormData({...formData, city: e.target.value})} />
                </div>
                <div>
                  <label className="text-sm font-medium">Country</label>
                  <Input value={formData.country} onChange={(e) => setFormData({...formData, country: e.target.value})} />
                </div>
                <div>
                  <label className="text-sm font-medium">Tax ID</label>
                  <Input value={formData.taxId} onChange={(e) => setFormData({...formData, taxId: e.target.value})} />
                </div>
              </div>
              <div>
                <label className="text-sm font-medium">Address</label>
                <Input value={formData.addressLine1} onChange={(e) => setFormData({...formData, addressLine1: e.target.value})} placeholder="Address Line 1" />
                <Input className="mt-2" value={formData.addressLine2} onChange={(e) => setFormData({...formData, addressLine2: e.target.value})} placeholder="Address Line 2" />
              </div>
              <div className="flex items-center gap-2">
                <input type="checkbox" id="isActive" checked={formData.isActive} onChange={(e) => setFormData({...formData, isActive: e.target.checked})} className="nx-checkbox" />
                <label htmlFor="isActive" className="text-sm font-medium">Active</label>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={saving}>
                  {saving && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingSupplier ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Supplier</h2>
            <p className="text-muted-foreground mb-6">Are you sure you want to delete "{deleteModal.supplierName}"?</p>
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setDeleteModal(null)}>Cancel</Button>
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
