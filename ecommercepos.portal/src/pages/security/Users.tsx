import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Search, Edit, UserCog,
  ChevronLeft, ChevronRight as ChevronRightIcon, Loader2, X, Power
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { userApi, roleApi, type User } from '@/api/rbacApi';

const statusColors: Record<string, string> = {
  true: 'nx-badge-success',
  false: 'nx-badge-danger',
};

interface UserFormData {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  isActive: boolean;
  preferredLanguage: string;
  timeZone: string;
}

const emptyForm: UserFormData = {
  firstName: '',
  lastName: '',
  phoneNumber: '',
  isActive: true,
  preferredLanguage: 'en',
  timeZone: 'UTC',
};

export default function Users() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [formData, setFormData] = useState<UserFormData>(emptyForm);
  const [selectedRoleIds, setSelectedRoleIds] = useState<string[]>([]);

  const { data, isLoading } = useQuery({
    queryKey: ['users', currentPage, searchQuery],
    queryFn: () => userApi.getAll({
      pageIndex: currentPage - 1,
      pageSize: 10,
      search: searchQuery || undefined,
    }),
  });

  const { data: rolesData } = useQuery({
    queryKey: ['roles-list'],
    queryFn: () => roleApi.getAll({ pageSize: 100 }),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<User> }) =>
      userApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      toast.success('User updated');
    },
    onError: () => toast.error('Failed to update user'),
  });

  const assignRolesMutation = useMutation({
    mutationFn: ({ userId, roleIds }: { userId: string; roleIds: string[] }) =>
      userApi.assignRoles(userId, roleIds),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      setShowModal(false);
      toast.success('User updated successfully');
    },
    onError: () => toast.error('Failed to assign roles'),
  });

  const toggleActiveMutation = useMutation({
    mutationFn: userApi.toggleActive,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      toast.success('User status toggled');
    },
    onError: () => toast.error('Failed to toggle user status'),
  });

  const users: User[] = data?.data?.items || data?.data?.data || [];
  const totalCount = data?.data?.totalCount || data?.data?.pagination?.totalCount || 0;
  const totalPages = Math.ceil(totalCount / 10) || 1;
  const allRoles = rolesData?.data?.items || rolesData?.data?.data || [];

  const handleSearch = () => setCurrentPage(1);

  const getUserRoleNames = (user: User): string[] => {
    if (!user.roles || user.roles.length === 0) return [];
    if (typeof user.roles[0] === 'string') return user.roles as string[];
    return (user.roles as { roleId: string; roleName: string }[]).map(r => r.roleName);
  };

  const getUserRoleIds = (user: User): string[] => {
    if (!user.roles || user.roles.length === 0) return [];
    if (typeof user.roles[0] === 'string') {
      // Match role names to IDs from allRoles
      return allRoles
        .filter((r: { id: string; name: string }) => (user.roles as string[]).includes(r.name))
        .map((r: { id: string }) => r.id);
    }
    return (user.roles as { roleId: string; roleName: string }[]).map(r => r.roleId);
  };

  const openEditModal = (user: User) => {
    setEditingUser(user);
    setFormData({
      firstName: user.firstName || '',
      lastName: user.lastName || '',
      phoneNumber: user.phoneNumber || '',
      isActive: user.isActive ?? true,
      preferredLanguage: user.preferredLanguage || 'en',
      timeZone: user.timeZone || 'UTC',
    });
    setSelectedRoleIds(getUserRoleIds(user));
    setShowModal(true);
  };

  const handleRoleToggle = (roleId: string) => {
    setSelectedRoleIds(prev =>
      prev.includes(roleId) ? prev.filter(id => id !== roleId) : [...prev, roleId]
    );
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingUser) return;
    updateMutation.mutate(
      { id: editingUser.id, data: formData },
      {
        onSuccess: () => {
          assignRolesMutation.mutate({ userId: editingUser.id, roleIds: selectedRoleIds });
        },
      }
    );
  };

  const formatDate = (dateStr?: string) => {
    if (!dateStr) return '-';
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric', month: 'short', day: 'numeric',
      hour: '2-digit', minute: '2-digit',
    });
  };

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Users</h1>
          <p className="nx-page-subtitle">Manage user accounts and role assignments</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Users</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value success">{users.filter(u => u.isActive).length}</div>
          <div className="nx-stat-label">Active</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{users.filter(u => !u.isActive).length}</div>
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
                placeholder="Search users..."
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
                    <th>Email</th>
                    <th>Roles</th>
                    <th>Active</th>
                    <th>Last Login</th>
                    <th style={{ width: 100 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map((user) => (
                    <tr key={user.id}>
                      <td>
                        <div className="flex items-center gap-2">
                          <UserCog className="w-4 h-4 text-muted-foreground" />
                          <span className="font-medium">
                            {[user.firstName, user.lastName].filter(Boolean).join(' ') || user.userName}
                          </span>
                        </div>
                      </td>
                      <td className="text-muted-foreground text-sm">{user.email}</td>
                      <td>
                        <div className="flex flex-wrap gap-1">
                          {getUserRoleNames(user).map((role) => (
                            <span key={role} className="nx-badge">{role}</span>
                          ))}
                          {getUserRoleNames(user).length === 0 && (
                            <span className="text-muted-foreground text-sm">No roles</span>
                          )}
                        </div>
                      </td>
                      <td>
                        <span className={`nx-badge ${statusColors[String(user.isActive)]}`}>
                          {user.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td className="text-muted-foreground text-sm">{formatDate(user.lastLoginAt)}</td>
                      <td>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEditModal(user)} title="Edit user">
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            className={`w-8 h-8 ${user.isActive ? 'text-orange-500' : 'text-green-500'}`}
                            onClick={() => toggleActiveMutation.mutate(user.id)}
                            title={user.isActive ? 'Deactivate' : 'Activate'}
                          >
                            <Power className="w-4 h-4" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                  {users.length === 0 && (
                    <tr>
                      <td colSpan={6} className="text-center text-muted-foreground py-8">No users found</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">Showing {users.length} of {totalCount}</p>
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

      {showModal && editingUser && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">Edit User</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">First Name</label>
                  <Input
                    value={formData.firstName}
                    onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                    placeholder="First name"
                  />
                </div>
                <div>
                  <label className="text-sm font-medium">Last Name</label>
                  <Input
                    value={formData.lastName}
                    onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                    placeholder="Last name"
                  />
                </div>
              </div>
              <div>
                <label className="text-sm font-medium">Phone Number</label>
                <Input
                  value={formData.phoneNumber}
                  onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })}
                  placeholder="Phone number"
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium">Language</label>
                  <select
                    className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm"
                    value={formData.preferredLanguage}
                    onChange={(e) => setFormData({ ...formData, preferredLanguage: e.target.value })}
                  >
                    <option value="en">English</option>
                    <option value="bn">Bengali</option>
                    <option value="ar">Arabic</option>
                    <option value="es">Spanish</option>
                    <option value="fr">French</option>
                  </select>
                </div>
                <div>
                  <label className="text-sm font-medium">Time Zone</label>
                  <select
                    className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm"
                    value={formData.timeZone}
                    onChange={(e) => setFormData({ ...formData, timeZone: e.target.value })}
                  >
                    <option value="UTC">UTC</option>
                    <option value="Asia/Dhaka">Asia/Dhaka</option>
                    <option value="America/New_York">America/New_York</option>
                    <option value="America/Chicago">America/Chicago</option>
                    <option value="America/Los_Angeles">America/Los_Angeles</option>
                    <option value="Europe/London">Europe/London</option>
                    <option value="Asia/Tokyo">Asia/Tokyo</option>
                  </select>
                </div>
              </div>
              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="userIsActive"
                  checked={formData.isActive}
                  onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                  className="nx-checkbox"
                />
                <label htmlFor="userIsActive" className="text-sm font-medium">Active</label>
              </div>

              <div className="border-t pt-4">
                <h3 className="text-sm font-semibold mb-3">Role Assignment</h3>
                <div className="space-y-2">
                  {allRoles.map((role: { id: string; name: string; description?: string }) => (
                    <div key={role.id} className="flex items-center gap-2">
                      <input
                        type="checkbox"
                        id={`role-${role.id}`}
                        checked={selectedRoleIds.includes(role.id)}
                        onChange={() => handleRoleToggle(role.id)}
                        className="nx-checkbox"
                      />
                      <label htmlFor={`role-${role.id}`} className="text-sm">
                        <span className="font-medium">{role.name}</span>
                        {role.description && (
                          <span className="text-muted-foreground ml-2">- {role.description}</span>
                        )}
                      </label>
                    </div>
                  ))}
                  {allRoles.length === 0 && (
                    <p className="text-sm text-muted-foreground">No roles available</p>
                  )}
                </div>
              </div>

              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={updateMutation.isPending || assignRolesMutation.isPending}>
                  {(updateMutation.isPending || assignRolesMutation.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  Save Changes
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
