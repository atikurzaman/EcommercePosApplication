import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Edit, Trash2, ShieldCheck, ArrowLeft,
  ChevronLeft, ChevronRight as ChevronRightIcon, Loader2, X, Save
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { roleApi, type Role, type RoleDetail, type RolePermissionItem, type RoleMenuItem } from '@/api/rbacApi';

const statusColors: Record<string, string> = {
  true: 'nx-badge-success',
  false: 'nx-badge-danger',
};

interface RoleFormData {
  name: string;
  description: string;
  isActive: boolean;
}

const emptyForm: RoleFormData = {
  name: '',
  description: '',
  isActive: true,
};

export default function Roles() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingRole, setEditingRole] = useState<Role | null>(null);
  const [formData, setFormData] = useState<RoleFormData>(emptyForm);
  const [deleteModal, setDeleteModal] = useState<Role | null>(null);
  const [selectedRoleId, setSelectedRoleId] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'permissions' | 'menus'>('permissions');
  const [permissionEdits, setPermissionEdits] = useState<Record<string, boolean>>({});
  const [menuEdits, setMenuEdits] = useState<Record<string, { canView: boolean; canAdd: boolean; canEdit: boolean; canDelete: boolean; canApprove: boolean }>>({});

  const { data, isLoading } = useQuery({
    queryKey: ['roles', currentPage, searchQuery],
    queryFn: () => roleApi.getAll({
      pageIndex: currentPage - 1,
      pageSize: 10,
      search: searchQuery || undefined,
    }),
  });

  const { data: roleDetailData, isLoading: isLoadingDetail } = useQuery({
    queryKey: ['role-detail', selectedRoleId],
    queryFn: () => roleApi.getById(selectedRoleId!),
    enabled: !!selectedRoleId,
  });

  const createMutation = useMutation({
    mutationFn: roleApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      setShowModal(false);
      toast.success('Role created');
    },
    onError: () => toast.error('Failed to create role'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<Role> }) =>
      roleApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      setShowModal(false);
      toast.success('Role updated');
    },
    onError: () => toast.error('Failed to update role'),
  });

  const deleteMutation = useMutation({
    mutationFn: roleApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      setDeleteModal(null);
      toast.success('Role deleted');
    },
    onError: () => toast.error('Failed to delete role'),
  });

  const assignPermissionsMutation = useMutation({
    mutationFn: ({ roleId, permissions }: { roleId: string; permissions: { permissionId: string; isGranted: boolean }[] }) =>
      roleApi.assignPermissions(roleId, permissions),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['role-detail', selectedRoleId] });
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      toast.success('Permissions saved');
    },
    onError: () => toast.error('Failed to save permissions'),
  });

  const assignMenusMutation = useMutation({
    mutationFn: ({ roleId, menus }: { roleId: string; menus: { menuId: string; canView: boolean; canAdd: boolean; canEdit: boolean; canDelete: boolean; canApprove: boolean }[] }) =>
      roleApi.assignMenus(roleId, menus),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['role-detail', selectedRoleId] });
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      toast.success('Menu permissions saved');
    },
    onError: () => toast.error('Failed to save menu permissions'),
  });

  const roles: Role[] = data?.data?.items || data?.data?.data || [];
  const totalCount = data?.data?.totalCount || data?.data?.pagination?.totalCount || 0;
  const totalPages = Math.ceil(totalCount / 10) || 1;

  const roleDetail: RoleDetail | null = roleDetailData?.data?.data || roleDetailData?.data || null;
  const rolePermissions: RolePermissionItem[] = roleDetail?.permissions || [];
  const roleMenus: RoleMenuItem[] = roleDetail?.menus || [];

  // Group permissions by module
  const permissionsByModule: Record<string, RolePermissionItem[]> = {};
  rolePermissions.forEach((p) => {
    const mod = p.module || 'General';
    if (!permissionsByModule[mod]) permissionsByModule[mod] = [];
    permissionsByModule[mod].push(p);
  });

  const handleSearch = () => setCurrentPage(1);

  const openCreateModal = () => {
    setEditingRole(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (role: Role) => {
    setEditingRole(role);
    setFormData({
      name: role.name || '',
      description: role.description || '',
      isActive: role.isActive ?? true,
    });
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingRole) {
      updateMutation.mutate({ id: editingRole.id, data: formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const openRoleDetail = (role: Role) => {
    setSelectedRoleId(role.id);
    setPermissionEdits({});
    setMenuEdits({});
    setActiveTab('permissions');
  };

  const getPermissionGranted = (permissionId: string, original: boolean): boolean => {
    if (permissionId in permissionEdits) return permissionEdits[permissionId];
    return original;
  };

  const togglePermission = (permissionId: string, current: boolean) => {
    setPermissionEdits(prev => ({ ...prev, [permissionId]: !current }));
  };

  const getMenuAccess = (menuId: string, field: keyof typeof menuEdits[string], original: boolean): boolean => {
    if (menuId in menuEdits && field in menuEdits[menuId]) return menuEdits[menuId][field];
    return original;
  };

  const toggleMenuAccess = (menu: RoleMenuItem, field: keyof typeof menuEdits[string]) => {
    const current = getMenuAccess(menu.menuId, field, menu[field]);
    setMenuEdits(prev => ({
      ...prev,
      [menu.menuId]: {
        canView: getMenuAccess(menu.menuId, 'canView', menu.canView),
        canAdd: getMenuAccess(menu.menuId, 'canAdd', menu.canAdd),
        canEdit: getMenuAccess(menu.menuId, 'canEdit', menu.canEdit),
        canDelete: getMenuAccess(menu.menuId, 'canDelete', menu.canDelete),
        canApprove: getMenuAccess(menu.menuId, 'canApprove', menu.canApprove),
        [field]: !current,
      },
    }));
  };

  const savePermissions = () => {
    if (!selectedRoleId) return;
    const permissions = rolePermissions.map(p => ({
      permissionId: p.permissionId,
      isGranted: getPermissionGranted(p.permissionId, p.isGranted),
    }));
    assignPermissionsMutation.mutate({ roleId: selectedRoleId, permissions });
  };

  const saveMenus = () => {
    if (!selectedRoleId) return;
    const menus = roleMenus.map(m => ({
      menuId: m.menuId,
      canView: getMenuAccess(m.menuId, 'canView', m.canView),
      canAdd: getMenuAccess(m.menuId, 'canAdd', m.canAdd),
      canEdit: getMenuAccess(m.menuId, 'canEdit', m.canEdit),
      canDelete: getMenuAccess(m.menuId, 'canDelete', m.canDelete),
      canApprove: getMenuAccess(m.menuId, 'canApprove', m.canApprove),
    }));
    assignMenusMutation.mutate({ roleId: selectedRoleId, menus });
  };

  // Detail view for a selected role
  if (selectedRoleId) {
    return (
      <div className="space-y-6">
        <div className="nx-page-header">
          <div className="flex items-center gap-3">
            <Button variant="ghost" size="icon" onClick={() => setSelectedRoleId(null)}>
              <ArrowLeft className="w-4 h-4" />
            </Button>
            <div>
              <h1 className="nx-page-title">Role: {roleDetail?.name || '...'}</h1>
              <p className="nx-page-subtitle">{roleDetail?.description || 'Configure permissions and menu access'}</p>
            </div>
          </div>
        </div>

        {isLoadingDetail ? (
          <div className="flex items-center justify-center p-8">
            <Loader2 className="w-8 h-8 animate-spin" />
          </div>
        ) : (
          <>
            <div className="flex gap-2 border-b">
              <button
                className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${activeTab === 'permissions' ? 'border-primary text-primary' : 'border-transparent text-muted-foreground hover:text-foreground'}`}
                onClick={() => setActiveTab('permissions')}
              >
                Permissions ({rolePermissions.length})
              </button>
              <button
                className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${activeTab === 'menus' ? 'border-primary text-primary' : 'border-transparent text-muted-foreground hover:text-foreground'}`}
                onClick={() => setActiveTab('menus')}
              >
                Menus ({roleMenus.length})
              </button>
            </div>

            {activeTab === 'permissions' && (
              <Card>
                <div className="p-4 border-b flex items-center justify-between">
                  <h3 className="font-semibold">Permission Matrix</h3>
                  <Button size="sm" onClick={savePermissions} disabled={assignPermissionsMutation.isPending}>
                    {assignPermissionsMutation.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                    <Save className="w-4 h-4 mr-2" />
                    Save Permissions
                  </Button>
                </div>
                <div className="p-4 space-y-6">
                  {Object.keys(permissionsByModule).length === 0 && (
                    <p className="text-muted-foreground text-center py-8">No permissions available. Create permissions first.</p>
                  )}
                  {Object.entries(permissionsByModule).map(([module, perms]) => (
                    <div key={module}>
                      <h4 className="text-sm font-semibold text-muted-foreground uppercase tracking-wider mb-3">{module}</h4>
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-2">
                        {perms.map((perm) => {
                          const granted = getPermissionGranted(perm.permissionId, perm.isGranted);
                          return (
                            <div
                              key={perm.permissionId}
                              className={`flex items-center gap-3 p-3 rounded-lg border cursor-pointer transition-colors ${granted ? 'bg-green-50 border-green-200 dark:bg-green-950/20 dark:border-green-800' : 'bg-background border-border hover:bg-muted/50'}`}
                              onClick={() => togglePermission(perm.permissionId, granted)}
                            >
                              <input
                                type="checkbox"
                                checked={granted}
                                onChange={() => togglePermission(perm.permissionId, granted)}
                                className="nx-checkbox"
                              />
                              <div className="min-w-0">
                                <p className="text-sm font-medium truncate">{perm.name}</p>
                                <p className="text-xs text-muted-foreground truncate">{perm.permissionCode}</p>
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    </div>
                  ))}
                </div>
              </Card>
            )}

            {activeTab === 'menus' && (
              <Card>
                <div className="p-4 border-b flex items-center justify-between">
                  <h3 className="font-semibold">Menu Access Matrix</h3>
                  <Button size="sm" onClick={saveMenus} disabled={assignMenusMutation.isPending}>
                    {assignMenusMutation.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                    <Save className="w-4 h-4 mr-2" />
                    Save Menu Access
                  </Button>
                </div>
                <div className="nx-table-wrap">
                  <table className="nx-table">
                    <thead>
                      <tr>
                        <th>Menu</th>
                        <th className="text-center" style={{ width: 80 }}>View</th>
                        <th className="text-center" style={{ width: 80 }}>Add</th>
                        <th className="text-center" style={{ width: 80 }}>Edit</th>
                        <th className="text-center" style={{ width: 80 }}>Delete</th>
                        <th className="text-center" style={{ width: 80 }}>Approve</th>
                      </tr>
                    </thead>
                    <tbody>
                      {roleMenus.length === 0 && (
                        <tr>
                          <td colSpan={6} className="text-center text-muted-foreground py-8">No menus available. Create menus first.</td>
                        </tr>
                      )}
                      {roleMenus.map((menu) => (
                        <tr key={menu.menuId}>
                          <td>
                            <div>
                              <p className="font-medium">{menu.displayName}</p>
                              <p className="text-xs text-muted-foreground">{menu.menuCode}</p>
                            </div>
                          </td>
                          <td className="text-center">
                            <input
                              type="checkbox"
                              checked={getMenuAccess(menu.menuId, 'canView', menu.canView)}
                              onChange={() => toggleMenuAccess(menu, 'canView')}
                              className="nx-checkbox"
                            />
                          </td>
                          <td className="text-center">
                            <input
                              type="checkbox"
                              checked={getMenuAccess(menu.menuId, 'canAdd', menu.canAdd)}
                              onChange={() => toggleMenuAccess(menu, 'canAdd')}
                              className="nx-checkbox"
                            />
                          </td>
                          <td className="text-center">
                            <input
                              type="checkbox"
                              checked={getMenuAccess(menu.menuId, 'canEdit', menu.canEdit)}
                              onChange={() => toggleMenuAccess(menu, 'canEdit')}
                              className="nx-checkbox"
                            />
                          </td>
                          <td className="text-center">
                            <input
                              type="checkbox"
                              checked={getMenuAccess(menu.menuId, 'canDelete', menu.canDelete)}
                              onChange={() => toggleMenuAccess(menu, 'canDelete')}
                              className="nx-checkbox"
                            />
                          </td>
                          <td className="text-center">
                            <input
                              type="checkbox"
                              checked={getMenuAccess(menu.menuId, 'canApprove', menu.canApprove)}
                              onChange={() => toggleMenuAccess(menu, 'canApprove')}
                              className="nx-checkbox"
                            />
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </Card>
            )}
          </>
        )}
      </div>
    );
  }

  // List view
  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Roles</h1>
          <p className="nx-page-subtitle">Manage roles and their permissions</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Role
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Roles</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value success">{roles.filter(r => r.isActive).length}</div>
          <div className="nx-stat-label">Active</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{roles.filter(r => !r.isActive).length}</div>
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
                placeholder="Search roles..."
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
                    <th>Name</th>
                    <th>Description</th>
                    <th>Status</th>
                    <th style={{ width: 120 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {roles.map((role) => (
                    <tr key={role.id} className="cursor-pointer" onClick={() => openRoleDetail(role)}>
                      <td>
                        <div className="flex items-center gap-2">
                          <ShieldCheck className="w-4 h-4 text-muted-foreground" />
                          <span className="font-medium">{role.name}</span>
                        </div>
                      </td>
                      <td className="text-muted-foreground text-sm">{role.description || '-'}</td>
                      <td>
                        <span className={`nx-badge ${statusColors[String(role.isActive)]}`}>
                          {role.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td>
                        <div className="flex items-center gap-1" onClick={(e) => e.stopPropagation()}>
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openRoleDetail(role)} title="Configure permissions">
                            <ShieldCheck className="w-4 h-4" />
                          </Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEditModal(role)} title="Edit role">
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteModal(role)} title="Delete role">
                            <Trash2 className="w-4 h-4" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                  {roles.length === 0 && (
                    <tr>
                      <td colSpan={4} className="text-center text-muted-foreground py-8">No roles found</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">Showing {roles.length} of {totalCount}</p>
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
          <div className="bg-background rounded-lg w-full max-w-md">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editingRole ? 'Edit Role' : 'Add Role'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div>
                <label className="text-sm font-medium">Name *</label>
                <Input
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  placeholder="e.g., Admin, Manager"
                  required
                />
              </div>
              <div>
                <label className="text-sm font-medium">Description</label>
                <Input
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  placeholder="Optional description"
                />
              </div>
              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="roleIsActive"
                  checked={formData.isActive}
                  onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                  className="nx-checkbox"
                />
                <label htmlFor="roleIsActive" className="text-sm font-medium">Active</label>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                  {(createMutation.isPending || updateMutation.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingRole ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Role</h2>
            <p className="text-muted-foreground mb-6">Are you sure you want to delete &quot;{deleteModal.name}&quot;?</p>
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
