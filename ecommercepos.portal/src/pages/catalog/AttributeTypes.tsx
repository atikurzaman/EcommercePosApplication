import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Edit, Trash2, ChevronDown, ChevronRight,
  Loader2, X, Settings, ChevronLeft,
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import {
  attributeTypeApi,
  type AttributeType,
  type AttributeOption,
} from '@/api/catalogApi';

const uiTypeColors: Record<string, string> = {
  Swatch: 'nx-badge-info',
  Button: 'nx-badge-success',
  Dropdown: 'nx-badge-warning',
  Text: 'nx-badge-neutral',
};

const uiTypes = ['Dropdown', 'Swatch', 'Button', 'Text'];

interface TypeFormData {
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

const emptyTypeForm: TypeFormData = {
  name: '', slug: '', uiType: 'Dropdown',
  affectsPrice: false, affectsSku: false, affectsImage: false, affectsStock: false,
  isFilterable: false, sortOrder: 0,
};

interface OptionFormData {
  value: string;
  displayValue: string;
  colorId: string;
  sortOrder: number;
  isActive: boolean;
}

const emptyOptionForm: OptionFormData = { value: '', displayValue: '', colorId: '', sortOrder: 0, isActive: true };

export default function AttributeTypes() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [page, setPage] = useState(1);
  const [showTypeModal, setShowTypeModal] = useState(false);
  const [editingType, setEditingType] = useState<AttributeType | null>(null);
  const [typeForm, setTypeForm] = useState<TypeFormData>(emptyTypeForm);
  const [deleteModal, setDeleteModal] = useState<AttributeType | null>(null);
  const [expandedTypes, setExpandedTypes] = useState<Set<string>>(new Set());
  const [showOptionModal, setShowOptionModal] = useState(false);
  const [editingOption, setEditingOption] = useState<AttributeOption | null>(null);
  const [optionForm, setOptionForm] = useState<OptionFormData>(emptyOptionForm);
  const [activeTypeId, setActiveTypeId] = useState('');
  const [bulkInput, setBulkInput] = useState('');
  const [showBulkModal, setShowBulkModal] = useState(false);
  const [bulkTypeId, setBulkTypeId] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['attribute-types', page, searchQuery],
    queryFn: () => attributeTypeApi.getAll({ pageIndex: page - 1, pageSize: 20, search: searchQuery || undefined }),
  });

  const types: AttributeType[] = data?.data?.items || data?.data || [];
  const totalCount = data?.data?.totalCount || data?.data?.length || 0;
  const totalPages = Math.ceil(totalCount / 20);

  // Type mutations
  const createTypeMut = useMutation({
    mutationFn: (d: any) => attributeTypeApi.create(d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['attribute-types'] }); setShowTypeModal(false); toast.success('Attribute type created'); },
    onError: () => toast.error('Failed to create'),
  });
  const updateTypeMut = useMutation({
    mutationFn: ({ id, d }: { id: string; d: any }) => attributeTypeApi.update(id, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['attribute-types'] }); setShowTypeModal(false); toast.success('Attribute type updated'); },
    onError: () => toast.error('Failed to update'),
  });
  const deleteTypeMut = useMutation({
    mutationFn: (id: string) => attributeTypeApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['attribute-types'] }); setDeleteModal(null); toast.success('Attribute type deleted'); },
    onError: () => toast.error('Failed to delete'),
  });

  // Option queries/mutations per expanded type
  const optionsQueries: Record<string, AttributeOption[]> = {};
  const expandedArr = Array.from(expandedTypes);

  // Option mutations
  const createOptionMut = useMutation({
    mutationFn: ({ typeId, d }: { typeId: string; d: any }) => attributeTypeApi.createOption(typeId, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['attribute-options'] }); toast.success('Option added'); setShowOptionModal(false); },
    onError: () => toast.error('Failed to add option'),
  });
  const updateOptionMut = useMutation({
    mutationFn: ({ typeId, optId, d }: { typeId: string; optId: string; d: any }) => attributeTypeApi.updateOption(typeId, optId, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['attribute-options'] }); toast.success('Option updated'); setShowOptionModal(false); },
    onError: () => toast.error('Failed to update option'),
  });
  const deleteOptionMut = useMutation({
    mutationFn: ({ typeId, optId }: { typeId: string; optId: string }) => attributeTypeApi.deleteOption(typeId, optId),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['attribute-options'] }); toast.success('Option deleted'); },
    onError: () => toast.error('Failed to delete option'),
  });
  const bulkCreateMut = useMutation({
    mutationFn: ({ typeId, options }: { typeId: string; options: any[] }) => attributeTypeApi.bulkCreateOptions(typeId, options),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['attribute-options'] }); setShowBulkModal(false); setBulkInput(''); toast.success('Options created'); },
    onError: () => toast.error('Failed to bulk create'),
  });

  const openCreateType = () => { setEditingType(null); setTypeForm(emptyTypeForm); setShowTypeModal(true); };
  const openEditType = (t: AttributeType) => {
    setEditingType(t);
    setTypeForm({ name: t.name, slug: t.slug, uiType: t.uiType, affectsPrice: t.affectsPrice, affectsSku: t.affectsSku, affectsImage: t.affectsImage, affectsStock: t.affectsStock, isFilterable: t.isFilterable, sortOrder: t.sortOrder });
    setShowTypeModal(true);
  };
  const handleTypeSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingType) updateTypeMut.mutate({ id: editingType.id, d: typeForm });
    else createTypeMut.mutate(typeForm);
  };

  const toggleExpand = (id: string) => {
    setExpandedTypes(prev => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id); else next.add(id);
      return next;
    });
  };

  const openCreateOption = (typeId: string) => { setActiveTypeId(typeId); setEditingOption(null); setOptionForm(emptyOptionForm); setShowOptionModal(true); };
  const openEditOption = (typeId: string, opt: AttributeOption) => { setActiveTypeId(typeId); setEditingOption(opt); setOptionForm({ value: opt.value, displayValue: opt.displayValue || '', colorId: opt.colorId || '', sortOrder: opt.sortOrder, isActive: opt.isActive }); setShowOptionModal(true); };
  const handleOptionSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingOption) updateOptionMut.mutate({ typeId: activeTypeId, optId: editingOption.id, d: optionForm });
    else createOptionMut.mutate({ typeId: activeTypeId, d: optionForm });
  };

  const openBulk = (typeId: string) => { setBulkTypeId(typeId); setBulkInput(''); setShowBulkModal(true); };
  const handleBulkCreate = () => {
    const values = bulkInput.split(',').map(v => v.trim()).filter(Boolean);
    if (values.length === 0) return;
    const options = values.map((v, i) => ({ value: v, displayValue: v, sortOrder: i, isActive: true }));
    bulkCreateMut.mutate({ typeId: bulkTypeId, options });
  };

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Attribute Types</h1>
          <p className="nx-page-subtitle">Manage attribute types and their options</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateType}><Plus className="w-4 h-4 mr-2" />Add Attribute Type</Button>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input type="text" placeholder="Search attribute types..." value={searchQuery} onChange={e => setSearchQuery(e.target.value)} onKeyDown={e => e.key === 'Enter' && setPage(1)} />
            </div>
          </div>
        </div>

        {isLoading ? (
          <div className="flex items-center justify-center p-8"><Loader2 className="w-8 h-8 animate-spin" /></div>
        ) : (
          <>
            <div className="nx-table-wrap">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th style={{ width: 40 }}></th>
                    <th>Name</th>
                    <th>UI Type</th>
                    <th>Flags</th>
                    <th>Filterable</th>
                    <th>Options</th>
                    <th>Sort</th>
                    <th style={{ width: 100 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {types.map(t => {
                    const isExpanded = expandedTypes.has(t.id);
                    return (
                      <TypeRow key={t.id} type={t} isExpanded={isExpanded}
                        onToggle={() => toggleExpand(t.id)}
                        onEdit={() => openEditType(t)}
                        onDelete={() => setDeleteModal(t)}
                        onCreateOption={() => openCreateOption(t.id)}
                        onEditOption={(opt) => openEditOption(t.id, opt)}
                        onDeleteOption={(optId) => deleteOptionMut.mutate({ typeId: t.id, optId })}
                        onBulkCreate={() => openBulk(t.id)}
                      />
                    );
                  })}
                  {types.length === 0 && (
                    <tr><td colSpan={8} className="text-center py-8 text-muted-foreground">
                      <Settings className="w-8 h-8 mx-auto mb-2 opacity-50" /><p>No attribute types found</p>
                    </td></tr>
                  )}
                </tbody>
              </table>
            </div>

            {totalPages > 1 && (
              <div className="flex items-center justify-between p-4 border-t">
                <p className="text-sm text-muted-foreground">Page {page} of {totalPages}</p>
                <div className="flex items-center gap-2">
                  <Button variant="outline" size="sm" disabled={page === 1} onClick={() => setPage(p => p - 1)}><ChevronLeft className="w-4 h-4" /></Button>
                  <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}><ChevronDown className="w-4 h-4 -rotate-90" /></Button>
                </div>
              </div>
            )}
          </>
        )}
      </Card>

      {/* Type Modal */}
      {showTypeModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editingType ? 'Edit Attribute Type' : 'Add Attribute Type'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowTypeModal(false)}><X className="w-4 h-4" /></Button>
            </div>
            <form onSubmit={handleTypeSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div><label className="text-sm font-medium">Name *</label><Input value={typeForm.name} onChange={e => setTypeForm({ ...typeForm, name: e.target.value, slug: typeForm.slug || e.target.value.toLowerCase().replace(/\s+/g, '-') })} required /></div>
                <div><label className="text-sm font-medium">Sort Order</label><Input type="number" value={typeForm.sortOrder} onChange={e => setTypeForm({ ...typeForm, sortOrder: parseInt(e.target.value) || 0 })} /></div>
              </div>
              <div><label className="text-sm font-medium">Slug</label><Input value={typeForm.slug} onChange={e => setTypeForm({ ...typeForm, slug: e.target.value })} /></div>
              <div><label className="text-sm font-medium">UI Type</label>
                <select className="nx-input nx-select w-full" value={typeForm.uiType} onChange={e => setTypeForm({ ...typeForm, uiType: e.target.value })}>
                  {uiTypes.map(u => <option key={u} value={u}>{u}</option>)}
                </select>
              </div>
              <div className="grid grid-cols-2 gap-2">
                <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={typeForm.affectsPrice} onChange={e => setTypeForm({ ...typeForm, affectsPrice: e.target.checked })} />Affects Price</label>
                <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={typeForm.affectsSku} onChange={e => setTypeForm({ ...typeForm, affectsSku: e.target.checked })} />Affects SKU</label>
                <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={typeForm.affectsImage} onChange={e => setTypeForm({ ...typeForm, affectsImage: e.target.checked })} />Affects Image</label>
                <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={typeForm.affectsStock} onChange={e => setTypeForm({ ...typeForm, affectsStock: e.target.checked })} />Affects Stock</label>
                <label className="flex items-center gap-2 text-sm col-span-2"><input type="checkbox" className="nx-checkbox" checked={typeForm.isFilterable} onChange={e => setTypeForm({ ...typeForm, isFilterable: e.target.checked })} />Filterable</label>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowTypeModal(false)}>Cancel</Button>
                <Button type="submit" disabled={createTypeMut.isPending || updateTypeMut.isPending}>
                  {(createTypeMut.isPending || updateTypeMut.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingType ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Option Modal */}
      {showOptionModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editingOption ? 'Edit Option' : 'Add Option'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowOptionModal(false)}><X className="w-4 h-4" /></Button>
            </div>
            <form onSubmit={handleOptionSubmit} className="p-4 space-y-4">
              <div><label className="text-sm font-medium">Value *</label><Input value={optionForm.value} onChange={e => setOptionForm({ ...optionForm, value: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Display Value</label><Input value={optionForm.displayValue} onChange={e => setOptionForm({ ...optionForm, displayValue: e.target.value })} /></div>
              <div className="grid grid-cols-2 gap-4">
                <div><label className="text-sm font-medium">Sort Order</label><Input type="number" value={optionForm.sortOrder} onChange={e => setOptionForm({ ...optionForm, sortOrder: parseInt(e.target.value) || 0 })} /></div>
                <div className="flex items-center gap-2 pt-6"><input type="checkbox" className="nx-checkbox" checked={optionForm.isActive} onChange={e => setOptionForm({ ...optionForm, isActive: e.target.checked })} /><label className="text-sm font-medium">Active</label></div>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowOptionModal(false)}>Cancel</Button>
                <Button type="submit" disabled={createOptionMut.isPending || updateOptionMut.isPending}>
                  {(createOptionMut.isPending || updateOptionMut.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingOption ? 'Update' : 'Add'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Bulk Create Modal */}
      {showBulkModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">Bulk Add Options</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowBulkModal(false)}><X className="w-4 h-4" /></Button>
            </div>
            <div className="p-4 space-y-4">
              <div>
                <label className="text-sm font-medium">Values (comma-separated)</label>
                <textarea className="nx-input w-full h-24" value={bulkInput} onChange={e => setBulkInput(e.target.value)} placeholder="Red, Blue, Green, Yellow" />
                <p className="text-xs text-muted-foreground mt-1">{bulkInput.split(',').filter(v => v.trim()).length} options will be created</p>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" onClick={() => setShowBulkModal(false)}>Cancel</Button>
                <Button onClick={handleBulkCreate} disabled={bulkCreateMut.isPending || !bulkInput.trim()}>
                  {bulkCreateMut.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  Create Options
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Delete Modal */}
      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Attribute Type</h2>
            <p className="text-muted-foreground mb-6">
              Are you sure you want to delete "{deleteModal.name}"? This will also delete all its options.
            </p>
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setDeleteModal(null)}>Cancel</Button>
              <Button variant="destructive" onClick={() => deleteTypeMut.mutate(deleteModal.id)} disabled={deleteTypeMut.isPending}>
                {deleteTypeMut.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                Delete
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

// Sub-component for type row with expandable options
function TypeRow({ type, isExpanded, onToggle, onEdit, onDelete, onCreateOption, onEditOption, onDeleteOption, onBulkCreate }: {
  type: AttributeType;
  isExpanded: boolean;
  onToggle: () => void;
  onEdit: () => void;
  onDelete: () => void;
  onCreateOption: () => void;
  onEditOption: (opt: AttributeOption) => void;
  onDeleteOption: (optId: string) => void;
  onBulkCreate: () => void;
}) {
  const { data: optionsData } = useQuery({
    queryKey: ['attribute-options', type.id],
    queryFn: () => attributeTypeApi.getOptions(type.id),
    enabled: isExpanded,
  });
  const options: AttributeOption[] = optionsData?.data?.items || optionsData?.data || [];

  return (
    <>
      <tr className="cursor-pointer" onClick={onToggle}>
        <td>
          {isExpanded ? <ChevronDown className="w-4 h-4" /> : <ChevronRight className="w-4 h-4" />}
        </td>
        <td className="font-medium">{type.name} <span className="text-xs text-muted-foreground">({type.slug})</span></td>
        <td><span className={`nx-badge ${uiTypeColors[type.uiType] || 'nx-badge-neutral'}`}>{type.uiType}</span></td>
        <td>
          <div className="flex gap-1 text-xs">
            {type.affectsPrice && <span className="bg-blue-100 text-blue-700 px-1 rounded">Price</span>}
            {type.affectsSku && <span className="bg-purple-100 text-purple-700 px-1 rounded">SKU</span>}
            {type.affectsImage && <span className="bg-green-100 text-green-700 px-1 rounded">Image</span>}
            {type.affectsStock && <span className="bg-yellow-100 text-yellow-700 px-1 rounded">Stock</span>}
          </div>
        </td>
        <td><span className={`nx-badge ${type.isFilterable ? 'nx-badge-success' : 'nx-badge-neutral'}`}>{type.isFilterable ? 'Yes' : 'No'}</span></td>
        <td>{type.optionCount ?? options.length}</td>
        <td>{type.sortOrder}</td>
        <td>
          <div className="flex items-center gap-1" onClick={e => e.stopPropagation()}>
            <Button variant="ghost" size="icon" className="w-8 h-8" onClick={onEdit}><Edit className="w-4 h-4" /></Button>
            <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={onDelete}><Trash2 className="w-4 h-4" /></Button>
          </div>
        </td>
      </tr>
      {isExpanded && (
        <tr>
          <td colSpan={8} className="bg-secondary/20 p-0">
            <div className="p-4">
              <div className="flex items-center justify-between mb-3">
                <span className="text-sm font-medium">Options for {type.name}</span>
                <div className="flex gap-2">
                  <Button size="sm" variant="outline" onClick={onBulkCreate}>Bulk Add</Button>
                  <Button size="sm" onClick={onCreateOption}><Plus className="w-3 h-3 mr-1" />Add Option</Button>
                </div>
              </div>
              <table className="w-full text-sm">
                <thead>
                  <tr className="text-left text-muted-foreground border-b">
                    <th className="py-2 px-3 font-medium">Value</th>
                    <th className="py-2 px-3 font-medium">Display Value</th>
                    <th className="py-2 px-3 font-medium">Active</th>
                    <th className="py-2 px-3 font-medium">Sort</th>
                    <th className="py-2 px-3 w-20"></th>
                  </tr>
                </thead>
                <tbody>
                  {options.map(opt => (
                    <tr key={opt.id} className="border-b">
                      <td className="py-2 px-3 font-medium">{opt.value}</td>
                      <td className="py-2 px-3">{opt.displayValue || '-'}</td>
                      <td className="py-2 px-3"><span className={`nx-badge text-xs ${opt.isActive ? 'nx-badge-success' : 'nx-badge-danger'}`}>{opt.isActive ? 'Active' : 'Inactive'}</span></td>
                      <td className="py-2 px-3">{opt.sortOrder}</td>
                      <td className="py-2 px-3">
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-6 h-6" onClick={() => onEditOption(opt)}><Edit className="w-3 h-3" /></Button>
                          <Button variant="ghost" size="icon" className="w-6 h-6 text-red-500" onClick={() => onDeleteOption(opt.id)}><Trash2 className="w-3 h-3" /></Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                  {options.length === 0 && (
                    <tr><td colSpan={5} className="text-center text-muted-foreground py-4">No options yet</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </td>
        </tr>
      )}
    </>
  );
}
