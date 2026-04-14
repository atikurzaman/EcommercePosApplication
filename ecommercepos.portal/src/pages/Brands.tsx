import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { 
  Plus, Search, Filter, Edit, Trash2, Award, 
  ChevronLeft, ChevronRight as ChevronRightIcon, Loader2, X, Upload, ExternalLink
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { brandApi, Brand } from '@/api/brandApi';

const statusColors: Record<string, string> = {
  true: 'nx-badge-success',
  false: 'nx-badge-danger',
};

interface BrandFormData {
  brandCode: string;
  brandName: string;
  description: string;
  logoUrl: string;
  website: string;
  countryOfOrigin: string;
  isFeatured: boolean;
  isActive: boolean;
}

const emptyForm: BrandFormData = {
  brandCode: '',
  brandName: '',
  description: '',
  logoUrl: '',
  website: '',
  countryOfOrigin: '',
  isFeatured: false,
  isActive: true,
};

export default function Brands() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState('all');
  const [showModal, setShowModal] = useState(false);
  const [editingBrand, setEditingBrand] = useState<Brand | null>(null);
  const [formData, setFormData] = useState<BrandFormData>(emptyForm);
  const [deleteModal, setDeleteModal] = useState<Brand | null>(null);

  const { data: brandsData, isLoading } = useQuery({
    queryKey: ['brands', currentPage, statusFilter, searchQuery],
    queryFn: () => brandApi.getAll({ 
      pageIndex: currentPage - 1, 
      pageSize: 10,
      search: searchQuery || undefined
    }),
  });

  const { data: brandsWithCountData } = useQuery({
    queryKey: ['brands-with-count'],
    queryFn: brandApi.getWithCount,
  });

  const createMutation = useMutation({
    mutationFn: brandApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['brands'] });
      queryClient.invalidateQueries({ queryKey: ['brands-with-count'] });
      setShowModal(false);
      toast.success('Brand created successfully');
    },
    onError: () => toast.error('Failed to create brand'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<Brand> }) => 
      brandApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['brands'] });
      queryClient.invalidateQueries({ queryKey: ['brands-with-count'] });
      setShowModal(false);
      toast.success('Brand updated successfully');
    },
    onError: () => toast.error('Failed to update brand'),
  });

  const deleteMutation = useMutation({
    mutationFn: brandApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['brands'] });
      queryClient.invalidateQueries({ queryKey: ['brands-with-count'] });
      setDeleteModal(null);
      toast.success('Brand deleted successfully');
    },
    onError: () => toast.error('Failed to delete brand'),
  });

  const toggleMutation = useMutation({
    mutationFn: brandApi.toggle,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['brands'] });
      queryClient.invalidateQueries({ queryKey: ['brands-with-count'] });
    },
  });

  const brands = brandsData?.data?.items || [];
  const totalCount = brandsData?.data?.totalCount || 0;
  const brandsWithCount = brandsWithCountData?.data?.items || [];
  
  const totalPages = Math.ceil(totalCount / 10);
  const activeCount = brands.filter(b => b.isActive).length;

  const handleSearch = () => {
    setCurrentPage(1);
  };

  const openCreateModal = () => {
    setEditingBrand(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (brand: Brand) => {
    setEditingBrand(brand);
    setFormData({
      brandCode: brand.brandCode || '',
      brandName: brand.brandName || '',
      description: brand.description || '',
      logoUrl: brand.logoUrl || '',
      website: brand.website || '',
      countryOfOrigin: brand.countryOfOrigin || '',
      isFeatured: brand.isFeatured || false,
      isActive: brand.isActive ?? true,
    });
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingBrand) {
      updateMutation.mutate({ id: editingBrand.id, data: formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const getProductCount = (brandId: string) => {
    const bwc = brandsWithCount.find(b => b.id === brandId);
    return bwc?.productCount || 0;
  };

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Brands</h1>
          <p className="nx-page-subtitle">Manage product brands</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Brand
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Brands</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value success">{activeCount}</div>
          <div className="nx-stat-label">Active</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{brandsWithCount.reduce((sum, b) => sum + b.productCount, 0)}</div>
          <div className="nx-stat-label">Total Products</div>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input 
                type="text" 
                placeholder="Search brands..." 
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

        {isLoading ? (
          <div className="flex items-center justify-center p-8">
            <Loader2 className="w-8 h-8 animate-spin" />
          </div>
        ) : (
          <>
            <div className="nx-table-wrap">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th>Logo</th>
                    <th>Brand Name</th>
                    <th>Products</th>
                    <th>Website</th>
                    <th>Status</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {brands.map((brand) => (
                    <tr key={brand.id}>
                      <td>
                        <div className="w-10 h-10 bg-secondary rounded-lg flex items-center justify-center overflow-hidden">
                          {brand.logoUrl ? (
                            <img src={brand.logoUrl} alt={brand.brandName} className="w-full h-full object-contain p-1" />
                          ) : (
                            <Award className="w-5 h-5 text-muted-foreground" />
                          )}
                        </div>
                      </td>
                      <td>
                        <div className="flex items-center gap-2">
                          <span className="font-medium">{brand.brandName}</span>
                          {brand.isFeatured && <span className="nx-badge-info text-xs">Featured</span>}
                        </div>
                      </td>
                      <td>
                        <span className="text-muted-foreground">{getProductCount(brand.id)}</span>
                      </td>
                      <td>
                        {brand.website ? (
                          <a 
                            href={brand.website} 
                            target="_blank" 
                            rel="noopener noreferrer"
                            className="text-primary hover:underline flex items-center gap-1 text-sm"
                          >
                            <ExternalLink className="w-3 h-3" />
                            Visit
                          </a>
                        ) : <span className="text-muted-foreground">-</span>}
                      </td>
                      <td>
                        <button 
                          onClick={() => toggleMutation.mutate(brand.id)}
                          className={`nx-badge cursor-pointer hover:opacity-80 ${statusColors[String(brand.isActive)]}`}
                        >
                          {brand.isActive ? 'Active' : 'Inactive'}
                        </button>
                      </td>
                      <td>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEditModal(brand)}>
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteModal(brand)}>
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
              <p className="text-sm text-muted-foreground">Showing {brands.length} of {totalCount}</p>
              <div className="flex items-center gap-2">
                <Button variant="outline" size="sm" disabled={currentPage === 1} onClick={() => setCurrentPage(p => p - 1)}>
                  <ChevronLeft className="w-4 h-4" />
                </Button>
                <span className="text-sm">Page {currentPage} of {totalPages || 1}</span>
                <Button variant="outline" size="sm" disabled={currentPage >= totalPages} onClick={() => setCurrentPage(p => p + 1)}>
                  <ChevronRightIcon className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </>
        )}
      </Card>

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editingBrand ? 'Edit Brand' : 'Add Brand'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Brand Code</label>
                  <Input 
                    value={formData.brandCode} 
                    onChange={(e) => setFormData({...formData, brandCode: e.target.value})}
                    placeholder="Auto-generated if empty"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Country of Origin</label>
                  <Input 
                    value={formData.countryOfOrigin} 
                    onChange={(e) => setFormData({...formData, countryOfOrigin: e.target.value})}
                    placeholder="e.g., Bangladesh"
                  />
                </div>
              </div>
              <div>
                <label className="text-sm font-medium">Brand Name *</label>
                <Input 
                  value={formData.brandName} 
                  onChange={(e) => setFormData({...formData, brandName: e.target.value})}
                  required 
                />
              </div>
              <div>
                <label className="text-sm font-medium">Logo URL</label>
                <div className="flex gap-2">
                  <Input 
                    value={formData.logoUrl} 
                    onChange={(e) => setFormData({...formData, logoUrl: e.target.value})}
                    placeholder="https://..."
                  />
                  <Button variant="outline" size="sm" type="button">
                    <Upload className="w-4 h-4" />
                  </Button>
                </div>
                {formData.logoUrl && (
                  <img src={formData.logoUrl} alt="Preview" className="mt-2 w-16 h-16 object-contain rounded-lg border" />
                )}
              </div>
              <div>
                <label className="text-sm font-medium">Website</label>
                <Input 
                  value={formData.website} 
                  onChange={(e) => setFormData({...formData, website: e.target.value})}
                  placeholder="https://example.com"
                />
              </div>
              <div>
                <label className="text-sm font-medium">Description</label>
                <textarea 
                  className="nx-input w-full h-20" 
                  value={formData.description} 
                  onChange={(e) => setFormData({...formData, description: e.target.value})} 
                />
              </div>
              <div className="flex gap-4">
                <div className="flex items-center gap-2">
                  <input 
                    type="checkbox" 
                    id="isFeatured"
                    checked={formData.isFeatured}
                    onChange={(e) => setFormData({...formData, isFeatured: e.target.checked})}
                    className="nx-checkbox"
                  />
                  <label htmlFor="isFeatured" className="text-sm font-medium">Featured</label>
                </div>
                <div className="flex items-center gap-2">
                  <input 
                    type="checkbox" 
                    id="isActive"
                    checked={formData.isActive}
                    onChange={(e) => setFormData({...formData, isActive: e.target.checked})}
                    className="nx-checkbox"
                  />
                  <label htmlFor="isActive" className="text-sm font-medium">Active</label>
                </div>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button 
                  type="submit" 
                  disabled={createMutation.isPending || updateMutation.isPending}
                >
                  {(createMutation.isPending || updateMutation.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingBrand ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Brand</h2>
            <p className="text-muted-foreground mb-6">Are you sure you want to delete "{deleteModal.brandName}"?</p>
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setDeleteModal(null)}>Cancel</Button>
              <Button variant="destructive" onClick={() => deleteMutation.mutate(deleteModal.id)} disabled={deleteMutation.isPending}>
                {deleteMutation.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                Delete
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}