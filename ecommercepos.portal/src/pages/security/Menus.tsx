import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Edit, Trash2, LayoutList,
  ChevronLeft, ChevronRight as ChevronRightIcon, Loader2, X
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { menuApi, type Menu } from '@/api/rbacApi';

const statusColors: Record<string, string> = {
  true: 'nx-badge-success',
  false: 'nx-badge-danger',
};

interface MenuFormData {
  menuCode: string;
  menuName: string;
  displayName: string;
  menuUrl: string;
  iconClass: string;
  displayOrder: number;
  menuLevel: number;
  permissionCode: string;
  parentMenuId: string;
  isActive: boolean;
  isVisible: boolean;
  isExternalLink: boolean;
  openInNewTab: boolean;
  description: string;
}

const emptyForm: MenuFormData = {
  menuCode: '',
  menuName: '',
  displayName: '',
  menuUrl: '',
  iconClass: '',
  displayOrder: 0,
  menuLevel: 1,
  permissionCode: '',
  parentMenuId: '',
  isActive: true,
  isVisible: true,
  isExternalLink: false,
  openInNewTab: false,
  description: '',
};

export default function Menus() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingItem, setEditingItem] = useState<Menu | null>(null);
  const [formData, setFormData] = useState<MenuFormData>(emptyForm);
  const [deleteModal, setDeleteModal] = useState<Menu | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['menus', currentPage, searchQuery],
    queryFn: () => menuApi.getAll({
      pageIndex: currentPage - 1,
      pageSize: 10,
      search: searchQuery || undefined,
    }),
  });

  const { data: allMenusData } = useQuery({
    queryKey: ['menus-all'],
    queryFn: () => menuApi.getAll({ pageSize: 500 }),
  });

  const createMutation = useMutation({
    mutationFn: menuApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['menus'] });
      setShowModal(false);
      toast.success('Menu created');
    },
    onError: () => toast.error('Failed to create menu'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<Menu> }) =>
      menuApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['menus'] });
      setShowModal(false);
      toast.success('Menu updated');
    },
    onError: () => toast.error('Failed to update menu'),
  });

  const deleteMutation = useMutation({
    mutationFn: menuApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['menus'] });
      setDeleteModal(null);
      toast.success('Menu deleted');
    },
    onError: () => toast.error('Failed to delete menu'),
  });

  const items: Menu[] = data?.data?.items || data?.data?.data || [];
  const totalCount = data?.data?.totalCount || data?.data?.pagination?.totalCount || 0;
  const totalPages = Math.ceil(totalCount / 10) || 1;
  const allMenus: Menu[] = allMenusData?.data?.items || allMenusData?.data?.data || [];

  const handleSearch = () => setCurrentPage(1);

  const openCreateModal = () => {
    setEditingItem(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (item: Menu) => {
    setEditingItem(item);
    setFormData({
      menuCode: item.menuCode || '',
      menuName: item.menuName || '',
      displayName: item.displayName || '',
      menuUrl: item.menuUrl || '',
      iconClass: item.iconClass || '',
      displayOrder: item.displayOrder ?? 0,
      menuLevel: item.menuLevel ?? 1,
      permissionCode: item.permissionCode || '',
      parentMenuId: item.parentMenuId || '',
      isActive: item.isActive ?? true,
      isVisible: item.isVisible ?? true,
      isExternalLink: item.isExternalLink ?? false,
      openInNewTab: item.openInNewTab ?? false,
      description: item.description || '',
    });
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const payload = {
      ...formData,
      parentMenuId: formData.parentMenuId || undefined,
      permissionCode: formData.permissionCode || undefined,
    };
    if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, data: payload });
    } else {
      createMutation.mutate(payload);
    }
  };

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Menus</h1>
          <p className="nx-page-subtitle">Manage navigation menus</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Menu
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Menus</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value success">{items.filter(i => i.isActive).length}</div>
          <div className="nx-stat-label">Active</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{items.filter(i => i.isVisible).length}</div>
          <div className="nx-stat-label">Visible</div>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input
                type="text"
                placeholder="Search menus..."
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
                    <th>Code</th>
                    <th>Display Name</th>
                    <th>URL</th>
                    <th>Icon</th>
                    <th>Level</th>
                    <th>Order</th>
                    <th>Status</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((item) => (
                    <tr key={item.id}>
                      <td>
                        <div className="flex items-center gap-2">
                          <LayoutList className="w-4 h-4 text-muted-foreground" />
                          <code className="text-sm font-medium">{item.menuCode}</code>
                        </div>
                      </td>
                      <td className="font-medium">{item.displayName}</td>
                      <td className="text-muted-foreground text-sm">{item.menuUrl || '-'}</td>
                      <td className="text-muted-foreground text-sm">{item.iconClass || '-'}</td>
                      <td>{item.menuLevel}</td>
                      <td>{item.displayOrder}</td>
                      <td>
                        <span className={`nx-badge ${statusColors[String(item.isActive)]}`}>
                          {item.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEditModal(item)}>
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteModal(item)}>
                            <Trash2 className="w-4 h-4" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                  {items.length === 0 && (
                    <tr>
                      <td colSpan={8} className="text-center text-muted-foreground py-8">No menus found</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">Showing {items.length} of {totalCount}</p>
              <div className="flex items-center gap-2">
                <Button variant="outline" size="sm" disabled={currentPage === 1} onClick={() => setCurrentPage(p => p - 1)}>
                  <ChevronLeft className="w-4 h-4" />
                </Button>
                <span className="text-sm">Page {currentPage} of {totalPages}</span>
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
          <div className="bg-background rounded-lg w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editingItem ? 'Edit Menu' : 'Add Menu'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Menu Code *</label>
                  <Input
                    value={formData.menuCode}
                    onChange={(e) => setFormData({ ...formData, menuCode: e.target.value })}
                    placeholder="e.g., products"
                    required
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Menu Name *</label>
                  <Input
                    value={formData.menuName}
                    onChange={(e) => setFormData({ ...formData, menuName: e.target.value })}
                    placeholder="e.g., Products"
                    required
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Display Name *</label>
                  <Input
                    value={formData.displayName}
                    onChange={(e) => setFormData({ ...formData, displayName: e.target.value })}
                    placeholder="e.g., Products"
                    required
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">URL</label>
                  <Input
                    value={formData.menuUrl}
                    onChange={(e) => setFormData({ ...formData, menuUrl: e.target.value })}
                    placeholder="e.g., /products"
                  />
                </div>
              </div>
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <label className="text-sm font-medium">Icon Class</label>
                  <Input
                    value={formData.iconClass}
                    onChange={(e) => setFormData({ ...formData, iconClass: e.target.value })}
                    placeholder="e.g., Package"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Level</label>
                  <Input
                    type="number"
                    value={formData.menuLevel}
                    onChange={(e) => setFormData({ ...formData, menuLevel: Number(e.target.value) })}
                    min={1}
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Order</label>
                  <Input
                    type="number"
                    value={formData.displayOrder}
                    onChange={(e) => setFormData({ ...formData, displayOrder: Number(e.target.value) })}
                    min={0}
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Permission Code</label>
                  <Input
                    value={formData.permissionCode}
                    onChange={(e) => setFormData({ ...formData, permissionCode: e.target.value })}
                    placeholder="Optional"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Parent Menu</label>
                  <select
                    className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm"
                    value={formData.parentMenuId}
                    onChange={(e) => setFormData({ ...formData, parentMenuId: e.target.value })}
                  >
                    <option value="">None (Root)</option>
                    {allMenus
                      .filter((m) => m.id !== editingItem?.id)
                      .map((m) => (
                        <option key={m.id} value={m.id}>{m.displayName}</option>
                      ))}
                  </select>
                </div>
              </div>
              <div>
                <label className="text-sm font-medium">Description</label>
                <Input
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  placeholder="Optional description"
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    id="menuIsActive"
                    checked={formData.isActive}
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                    className="nx-checkbox"
                  />
                  <label htmlFor="menuIsActive" className="text-sm font-medium">Active</label>
                </div>
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    id="menuIsVisible"
                    checked={formData.isVisible}
                    onChange={(e) => setFormData({ ...formData, isVisible: e.target.checked })}
                    className="nx-checkbox"
                  />
                  <label htmlFor="menuIsVisible" className="text-sm font-medium">Visible</label>
                </div>
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    id="menuIsExternal"
                    checked={formData.isExternalLink}
                    onChange={(e) => setFormData({ ...formData, isExternalLink: e.target.checked })}
                    className="nx-checkbox"
                  />
                  <label htmlFor="menuIsExternal" className="text-sm font-medium">External Link</label>
                </div>
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    id="menuNewTab"
                    checked={formData.openInNewTab}
                    onChange={(e) => setFormData({ ...formData, openInNewTab: e.target.checked })}
                    className="nx-checkbox"
                  />
                  <label htmlFor="menuNewTab" className="text-sm font-medium">Open in New Tab</label>
                </div>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                  {(createMutation.isPending || updateMutation.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingItem ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Menu</h2>
            <p className="text-muted-foreground mb-6">Are you sure you want to delete &quot;{deleteModal.displayName}&quot;?</p>
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
