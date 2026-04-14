import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { 
  Plus, Search, Edit, Trash2, Settings, ChevronDown, ChevronRight,
  Loader2, X
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { attributeApi, AttributeTypeWithOptions, AttributeOption } from '@/api/attributeApi';

const uiTypeColors: Record<string, string> = {
  Swatch: 'nx-badge-info',
  Button: 'nx-badge-success',
  Dropdown: 'nx-badge-warning',
  Text: 'nx-badge-neutral',
};

interface AttributeTypeFormData {
  name: string;
  slug: string;
  uiType: string;
  affectsPrice: boolean;
  affectsSku: boolean;
  affectsImage: boolean;
  affectsStock: boolean;
  isFilterable: boolean;
  sortOrder: number;
}

const emptyForm: AttributeTypeFormData = {
  name: '',
  slug: '',
  uiType: 'Dropdown',
  affectsPrice: false,
  affectsSku: false,
  affectsImage: false,
  affectsStock: false,
  isFilterable: false,
  sortOrder: 0,
};

const uiTypes = ['Dropdown', 'Swatch', 'Button', 'Text'];

export default function Attributes() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [, setCurrentPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingType, setEditingType] = useState<AttributeTypeWithOptions | null>(null);
  const [formData, setFormData] = useState<AttributeTypeFormData>(emptyForm);
  const [deleteModal, setDeleteModal] = useState<AttributeTypeWithOptions | null>(null);
  const [expandedTypes, setExpandedTypes] = useState<Set<string>>(new Set());
  const [newOptionValues, setNewOptionValues] = useState<Record<string, string>>({});

  const { data, isLoading } = useQuery({
    queryKey: ['attributes-with-options'],
    queryFn: attributeApi.getWithOptions,
  });

  const createMutation = useMutation({
    mutationFn: attributeApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attributes-with-options'] });
      setShowModal(false);
      toast.success('Attribute type created');
    },
    onError: () => toast.error('Failed to create attribute type'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<AttributeTypeFormData> }) => 
      attributeApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attributes-with-options'] });
      setShowModal(false);
      toast.success('Attribute type updated');
    },
    onError: () => toast.error('Failed to update attribute type'),
  });

  const deleteMutation = useMutation({
    mutationFn: attributeApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attributes-with-options'] });
      setDeleteModal(null);
      toast.success('Attribute type deleted');
    },
    onError: () => toast.error('Failed to delete attribute type'),
  });

  const createOptionMutation = useMutation({
    mutationFn: (data: { attributeTypeId: string; value: string; displayValue?: string; sortOrder: number }) => 
      attributeApi.createOption(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attributes-with-options'] });
      setNewOptionValues({});
      toast.success('Option added');
    },
    onError: () => toast.error('Failed to add option'),
  });

  const updateOptionMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<AttributeOption> }) => 
      attributeApi.updateOption(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attributes-with-options'] });
      toast.success('Option updated');
    },
    onError: () => toast.error('Failed to update option'),
  });

  const deleteOptionMutation = useMutation({
    mutationFn: attributeApi.deleteOption,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attributes-with-options'] });
      toast.success('Option deleted');
    },
    onError: () => toast.error('Failed to delete option'),
  });

  const types = data?.data?.items || [];
  const filteredTypes = types.filter(t => 
    t.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    t.slug.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const handleSearch = () => setCurrentPage(1);

  const openCreateModal = () => {
    setEditingType(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (type: AttributeTypeWithOptions) => {
    setEditingType(type);
    setFormData({
      name: type.name,
      slug: type.slug,
      uiType: type.uiType,
      affectsPrice: type.affectsPrice,
      affectsSku: type.affectsSku,
      affectsImage: type.affectsImage,
      affectsStock: type.affectsStock,
      isFilterable: type.isFilterable,
      sortOrder: type.sortOrder,
    });
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingType) {
      updateMutation.mutate({ id: editingType.id, data: formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const toggleExpand = (id: string) => {
    setExpandedTypes(prev => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const handleAddOption = (typeId: string) => {
    const value = newOptionValues[typeId];
    if (!value?.trim()) return;
    createOptionMutation.mutate({
      attributeTypeId: typeId,
      value: value.trim(),
      sortOrder: 0,
    });
  };

  const renderOptionRow = (option: AttributeOption) => (
    <tr key={option.id} className="border-b">
      <td className="py-2 px-4">
        <Input
          value={option.value}
          onChange={(e) => updateOptionMutation.mutate({ 
            id: option.id, 
            data: { ...option, value: e.target.value } 
          })}
          className="h-8 text-sm"
        />
      </td>
      <td className="py-2 px-4">
        <Input
          value={option.displayValue || ''}
          onChange={(e) => updateOptionMutation.mutate({ 
            id: option.id, 
            data: { ...option, displayValue: e.target.value } 
          })}
          placeholder="Display text"
          className="h-8 text-sm"
        />
      </td>
      <td className="py-2 px-4">
        <input
          type="color"
          value={option.displayValue?.startsWith('#') ? option.displayValue : '#000000'}
          onChange={(e) => updateOptionMutation.mutate({ 
            id: option.id, 
            data: { ...option, displayValue: e.target.value } 
          })}
          className="w-8 h-8 rounded cursor-pointer"
        />
      </td>
      <td className="py-2 px-4">
        <button
          onClick={() => updateOptionMutation.mutate({ 
            id: option.id, 
            data: { ...option, isActive: !option.isActive } 
          })}
          className={`text-xs px-2 py-1 rounded ${option.isActive ? 'nx-badge-success' : 'nx-badge-danger'}`}
        >
          {option.isActive ? 'Active' : 'Inactive'}
        </button>
      </td>
      <td className="py-2 px-4">
        <Button 
          variant="ghost" 
          size="icon" 
          className="w-6 h-6 text-red-500"
          onClick={() => deleteOptionMutation.mutate(option.id)}
        >
          <Trash2 className="w-3 h-3" />
        </Button>
      </td>
    </tr>
  );

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Attributes</h1>
          <p className="nx-page-subtitle">Manage product attributes and options</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Attribute
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{types.length}</div>
          <div className="nx-stat-label">Total Attributes</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{types.reduce((sum, t) => sum + (t.options?.length || 0), 0)}</div>
          <div className="nx-stat-label">Total Options</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{types.filter(t => t.uiType === 'Swatch').length}</div>
          <div className="nx-stat-label">Color/Swatch</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{types.filter(t => t.isFilterable).length}</div>
          <div className="nx-stat-label">Filterable</div>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input 
                type="text" 
                placeholder="Search attributes..." 
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
          <div className="divide-y">
            {filteredTypes.map((type) => {
              const isExpanded = expandedTypes.has(type.id);
              return (
                <div key={type.id}>
                  <div className="flex items-center gap-4 p-4 hover:bg-secondary/30">
                    <button onClick={() => toggleExpand(type.id)} className="p-1">
                      {isExpanded ? <ChevronDown className="w-4 h-4" /> : <ChevronRight className="w-4 h-4" />}
                    </button>
                    <div className="flex-1 grid grid-cols-6 gap-4 items-center">
                      <div>
                        <span className="font-medium">{type.name}</span>
                        <span className="text-muted-foreground text-xs ml-2">({type.slug})</span>
                      </div>
                      <span className={`nx-badge ${uiTypeColors[type.uiType] || 'nx-badge-neutral'}`}>
                        {type.uiType}
                      </span>
                      <div className="flex gap-2 text-xs">
                        {type.affectsPrice && <span className="bg-blue-100 text-blue-700 px-1 rounded">Price</span>}
                        {type.affectsSku && <span className="bg-purple-100 text-purple-700 px-1 rounded">SKU</span>}
                        {type.affectsImage && <span className="bg-green-100 text-green-700 px-1 rounded">Image</span>}
                        {type.affectsStock && <span className="bg-yellow-100 text-yellow-700 px-1 rounded">Stock</span>}
                      </div>
                      <span className="text-muted-foreground text-sm">{type.options?.length || 0} options</span>
                      <span className={`nx-badge ${type.isFilterable ? 'nx-badge-success' : 'nx-badge-neutral'}`}>
                        {type.isFilterable ? 'Filterable' : '-'}
                      </span>
                    </div>
                    <div className="flex items-center gap-1">
                      <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEditModal(type)}>
                        <Edit className="w-4 h-4" />
                      </Button>
                      <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteModal(type)}>
                        <Trash2 className="w-4 h-4" />
                      </Button>
                    </div>
                  </div>
                  
                  {isExpanded && (
                    <div className="bg-secondary/20 p-4">
                      <table className="w-full">
                        <thead>
                          <tr className="text-left text-sm text-muted-foreground">
                            <th className="py-2 px-4 font-medium">Value</th>
                            <th className="py-2 px-4 font-medium">Display</th>
                            <th className="py-2 px-4 font-medium">Color</th>
                            <th className="py-2 px-4 font-medium">Status</th>
                            <th className="py-2 px-4 w-20"></th>
                          </tr>
                        </thead>
                        <tbody>
                          {type.options?.map(opt => renderOptionRow(opt))}
                          <tr>
                            <td className="py-2 px-4">
                              <Input
                                placeholder="New option value"
                                value={newOptionValues[type.id] || ''}
                                onChange={(e) => setNewOptionValues({ ...newOptionValues, [type.id]: e.target.value })}
                                onKeyDown={(e) => e.key === 'Enter' && handleAddOption(type.id)}
                                className="h-8"
                              />
                            </td>
                            <td className="py-2 px-4"></td>
                            <td className="py-2 px-4"></td>
                            <td className="py-2 px-4"></td>
                            <td className="py-2 px-4">
                              <Button 
                                size="sm" 
                                variant="outline"
                                onClick={() => handleAddOption(type.id)}
                                disabled={!newOptionValues[type.id]?.trim()}
                              >
                                Add
                              </Button>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              );
            })}
            {filteredTypes.length === 0 && (
              <div className="text-center p-8 text-muted-foreground">
                <Settings className="w-8 h-8 mx-auto mb-2 opacity-50" />
                <p>No attributes found</p>
              </div>
            )}
          </div>
        )}
      </Card>

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editingType ? 'Edit Attribute' : 'Add Attribute'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Name *</label>
                  <Input 
                    value={formData.name} 
                    onChange={(e) => setFormData({
                      ...formData, 
                      name: e.target.value,
                      slug: formData.slug || e.target.value.toLowerCase().replace(/\s+/g, '-')
                    })} 
                    required 
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Sort Order</label>
                  <Input 
                    type="number" 
                    value={formData.sortOrder} 
                    onChange={(e) => setFormData({...formData, sortOrder: parseInt(e.target.value) || 0})} 
                  />
                </div>
              </div>
              <div>
                <label className="text-sm font-medium">Slug</label>
                <Input 
                  value={formData.slug} 
                  onChange={(e) => setFormData({...formData, slug: e.target.value})}
                />
              </div>
              <div>
                <label className="text-sm font-medium">UI Type</label>
                <select 
                  className="nx-input nx-select w-full"
                  value={formData.uiType}
                  onChange={(e) => setFormData({...formData, uiType: e.target.value})}
                >
                  {uiTypes.map(type => (
                    <option key={type} value={type}>{type}</option>
                  ))}
                </select>
              </div>
              <div className="grid grid-cols-2 gap-2">
                <label className="flex items-center gap-2 text-sm">
                  <input 
                    type="checkbox" 
                    checked={formData.affectsPrice}
                    onChange={(e) => setFormData({...formData, affectsPrice: e.target.checked})}
                    className="nx-checkbox"
                  />
                  Affects Price
                </label>
                <label className="flex items-center gap-2 text-sm">
                  <input 
                    type="checkbox" 
                    checked={formData.affectsSku}
                    onChange={(e) => setFormData({...formData, affectsSku: e.target.checked})}
                    className="nx-checkbox"
                  />
                  Affects SKU
                </label>
                <label className="flex items-center gap-2 text-sm">
                  <input 
                    type="checkbox" 
                    checked={formData.affectsImage}
                    onChange={(e) => setFormData({...formData, affectsImage: e.target.checked})}
                    className="nx-checkbox"
                  />
                  Affects Image
                </label>
                <label className="flex items-center gap-2 text-sm">
                  <input 
                    type="checkbox" 
                    checked={formData.affectsStock}
                    onChange={(e) => setFormData({...formData, affectsStock: e.target.checked})}
                    className="nx-checkbox"
                  />
                  Affects Stock
                </label>
                <label className="flex items-center gap-2 text-sm col-span-2">
                  <input 
                    type="checkbox" 
                    checked={formData.isFilterable}
                    onChange={(e) => setFormData({...formData, isFilterable: e.target.checked})}
                    className="nx-checkbox"
                  />
                  Filterable in Storefront
                </label>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                  {(createMutation.isPending || updateMutation.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingType ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Attribute</h2>
            <p className="text-muted-foreground mb-6">
              Are you sure you want to delete "{deleteModal.name}"? This will also delete all {deleteModal.options?.length || 0} options.
            </p>
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