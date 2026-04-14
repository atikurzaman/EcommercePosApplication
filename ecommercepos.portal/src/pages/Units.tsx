import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { 
  Plus, Search, Edit, Trash2, Scale,
  ChevronLeft, ChevronRight as ChevronRightIcon, Loader2, X
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { unitApi, Unit } from '@/api/unitApi';

const statusColors: Record<string, string> = {
  true: 'nx-badge-success',
  false: 'nx-badge-danger',
};

interface UnitFormData {
  shortName: string;
  name: string;
  description: string;
  isActive: boolean;
}

const emptyForm: UnitFormData = {
  shortName: '',
  name: '',
  description: '',
  isActive: true,
};

export default function Units() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingUnit, setEditingUnit] = useState<Unit | null>(null);
  const [formData, setFormData] = useState<UnitFormData>(emptyForm);
  const [deleteModal, setDeleteModal] = useState<Unit | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['units', currentPage, searchQuery],
    queryFn: () => unitApi.getAll({ 
      pageIndex: currentPage - 1, 
      pageSize: 10,
      search: searchQuery || undefined
    }),
  });

  const createMutation = useMutation({
    mutationFn: unitApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['units'] });
      setShowModal(false);
      toast.success('Unit created');
    },
    onError: () => toast.error('Failed to create unit'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<Unit> }) => 
      unitApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['units'] });
      setShowModal(false);
      toast.success('Unit updated');
    },
    onError: () => toast.error('Failed to update unit'),
  });

  const deleteMutation = useMutation({
    mutationFn: unitApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['units'] });
      setDeleteModal(null);
      toast.success('Unit deleted');
    },
    onError: () => toast.error('Failed to delete unit'),
  });

  const units = data?.data?.items || [];
  const totalCount = data?.data?.totalCount || 0;
  const totalPages = Math.ceil(totalCount / 10);
  const activeCount = units.filter(u => u.isActive).length;

  const handleSearch = () => setCurrentPage(1);

  const openCreateModal = () => {
    setEditingUnit(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (unit: Unit) => {
    setEditingUnit(unit);
    setFormData({
      shortName: unit.shortName || '',
      name: unit.name || '',
      description: unit.description || '',
      isActive: unit.isActive ?? true,
    });
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingUnit) {
      updateMutation.mutate({ id: editingUnit.id, data: formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Units of Measure</h1>
          <p className="nx-page-subtitle">Manage product units</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Unit
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Units</div>
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
                placeholder="Search units..." 
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
              />
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
                    <th>Abbreviation</th>
                    <th>Name</th>
                    <th>Description</th>
                    <th>Status</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {units.map((unit) => (
                    <tr key={unit.id}>
                      <td>
                        <div className="flex items-center gap-2">
                          <Scale className="w-4 h-4 text-muted-foreground" />
                          <code className="text-sm font-medium">{unit.shortName}</code>
                        </div>
                      </td>
                      <td className="font-medium">{unit.name}</td>
                      <td className="text-muted-foreground text-sm">{unit.description || '-'}</td>
                      <td>
                        <span className={`nx-badge ${statusColors[String(unit.isActive)]}`}>
                          {unit.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEditModal(unit)}>
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteModal(unit)}>
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
              <p className="text-sm text-muted-foreground">Showing {units.length} of {totalCount}</p>
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
          <div className="bg-background rounded-lg w-full max-w-md">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editingUnit ? 'Edit Unit' : 'Add Unit'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Abbreviation *</label>
                  <Input 
                    value={formData.shortName} 
                    onChange={(e) => setFormData({...formData, shortName: e.target.value})}
                    placeholder="e.g., kg, m, L"
                    required 
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Name *</label>
                  <Input 
                    value={formData.name} 
                    onChange={(e) => setFormData({...formData, name: e.target.value})}
                    placeholder="e.g., Kilogram"
                    required 
                  />
                </div>
              </div>
              <div>
                <label className="text-sm font-medium">Description</label>
                <Input 
                  value={formData.description} 
                  onChange={(e) => setFormData({...formData, description: e.target.value})}
                  placeholder="Optional description"
                />
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
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                  {(createMutation.isPending || updateMutation.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingUnit ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Unit</h2>
            <p className="text-muted-foreground mb-6">Are you sure you want to delete "{deleteModal.name}"?</p>
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