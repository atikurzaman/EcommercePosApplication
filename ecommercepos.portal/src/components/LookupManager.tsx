import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Edit, Trash2,
  ChevronLeft, ChevronRight as ChevronRightIcon, Loader2, X, Database
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { createLookupApi } from '@/api/lookupApi';

export interface ColumnDef {
  key: string;
  label: string;
  render?: (value: any, row: any) => React.ReactNode;
}

export interface FormFieldDef {
  key: string;
  label: string;
  type: 'text' | 'number' | 'checkbox' | 'select';
  required?: boolean;
  placeholder?: string;
  options?: { value: string; label: string }[];
}

export interface LookupManagerProps {
  title: string;
  subtitle: string;
  queryKey: string;
  api: ReturnType<typeof createLookupApi>;
  codeField: string;
  nameField: string;
  columns: ColumnDef[];
  formFields: FormFieldDef[];
}

export default function LookupManager({
  title,
  subtitle,
  queryKey,
  api,
  codeField,
  nameField,
  columns,
  formFields,
}: LookupManagerProps) {
  const queryClientHook = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingItem, setEditingItem] = useState<Record<string, any> | null>(null);
  const [formData, setFormData] = useState<Record<string, any>>({});
  const [deleteModal, setDeleteModal] = useState<Record<string, any> | null>(null);

  const buildEmptyForm = (): Record<string, any> => {
    const empty: Record<string, any> = {};
    for (const field of formFields) {
      if (field.type === 'checkbox') {
        empty[field.key] = false;
      } else if (field.type === 'number') {
        empty[field.key] = '';
      } else {
        empty[field.key] = '';
      }
    }
    return empty;
  };

  const { data, isLoading } = useQuery({
    queryKey: [queryKey, currentPage, searchQuery],
    queryFn: () =>
      api.getAll({
        pageIndex: currentPage - 1,
        pageSize: 10,
        search: searchQuery || undefined,
      }),
  });

  const createMutation = useMutation({
    mutationFn: (payload: Record<string, any>) => api.create(payload),
    onSuccess: () => {
      queryClientHook.invalidateQueries({ queryKey: [queryKey] });
      setShowModal(false);
      toast.success(`${title.replace(/s$/, '')} created`);
    },
    onError: () => toast.error(`Failed to create ${title.replace(/s$/, '').toLowerCase()}`),
  });

  const updateMutation = useMutation({
    mutationFn: ({ code, payload }: { code: string; payload: Record<string, any> }) =>
      api.update(code, payload),
    onSuccess: () => {
      queryClientHook.invalidateQueries({ queryKey: [queryKey] });
      setShowModal(false);
      toast.success(`${title.replace(/s$/, '')} updated`);
    },
    onError: () => toast.error(`Failed to update ${title.replace(/s$/, '').toLowerCase()}`),
  });

  const deleteMutation = useMutation({
    mutationFn: (code: string) => api.delete(code),
    onSuccess: () => {
      queryClientHook.invalidateQueries({ queryKey: [queryKey] });
      setDeleteModal(null);
      toast.success(`${title.replace(/s$/, '')} deleted`);
    },
    onError: () => toast.error(`Failed to delete ${title.replace(/s$/, '').toLowerCase()}`),
  });

  const items = data?.data?.data || [];
  const pagination = data?.data?.pagination;
  const totalCount = pagination?.totalCount || items.length;
  const totalPages = pagination?.totalPages || 1;

  const handleSearch = () => setCurrentPage(1);

  const openCreateModal = () => {
    setEditingItem(null);
    setFormData(buildEmptyForm());
    setShowModal(true);
  };

  const openEditModal = (item: Record<string, any>) => {
    setEditingItem(item);
    const data: Record<string, any> = {};
    for (const field of formFields) {
      if (field.type === 'checkbox') {
        data[field.key] = item[field.key] ?? false;
      } else if (field.type === 'number') {
        data[field.key] = item[field.key] ?? '';
      } else {
        data[field.key] = item[field.key] ?? '';
      }
    }
    setFormData(data);
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const payload: Record<string, any> = { ...formData };
    for (const field of formFields) {
      if (field.type === 'number' && payload[field.key] !== '') {
        payload[field.key] = Number(payload[field.key]);
      }
    }
    if (editingItem) {
      updateMutation.mutate({ code: String(editingItem[codeField]), payload });
    } else {
      createMutation.mutate(payload);
    }
  };

  const updateField = (key: string, value: any) => {
    setFormData((prev) => ({ ...prev, [key]: value }));
  };

  const singularTitle = title.replace(/ies$/, 'y').replace(/ses$/, 's').replace(/s$/, '');

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">{title}</h1>
          <p className="nx-page-subtitle">{subtitle}</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add {singularTitle}
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total {title}</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{items.length}</div>
          <div className="nx-stat-label">Showing on Page</div>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input
                type="text"
                placeholder={`Search ${title.toLowerCase()}...`}
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
                    {columns.map((col) => (
                      <th key={col.key}>{col.label}</th>
                    ))}
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {items.length === 0 ? (
                    <tr>
                      <td colSpan={columns.length + 1} className="text-center text-muted-foreground py-8">
                        <Database className="w-8 h-8 mx-auto mb-2 opacity-30" />
                        No {title.toLowerCase()} found
                      </td>
                    </tr>
                  ) : (
                    items.map((item: Record<string, any>, idx: number) => (
                      <tr key={item[codeField] ?? idx}>
                        {columns.map((col) => (
                          <td key={col.key}>
                            {col.render
                              ? col.render(item[col.key], item)
                              : item[col.key] ?? '-'}
                          </td>
                        ))}
                        <td>
                          <div className="flex items-center gap-1">
                            <Button
                              variant="ghost"
                              size="icon"
                              className="w-8 h-8"
                              onClick={() => openEditModal(item)}
                            >
                              <Edit className="w-4 h-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="w-8 h-8 text-red-500"
                              onClick={() => setDeleteModal(item)}
                            >
                              <Trash2 className="w-4 h-4" />
                            </Button>
                          </div>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>

            <div className="flex items-center justify-between p-4 border-t">
              <p className="text-sm text-muted-foreground">
                Showing {items.length} of {totalCount}
              </p>
              <div className="flex items-center gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={currentPage === 1}
                  onClick={() => setCurrentPage((p) => p - 1)}
                >
                  <ChevronLeft className="w-4 h-4" />
                </Button>
                <span className="text-sm">
                  Page {currentPage} of {totalPages}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={currentPage >= totalPages}
                  onClick={() => setCurrentPage((p) => p + 1)}
                >
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
              <h2 className="text-lg font-semibold">
                {editingItem ? `Edit ${singularTitle}` : `Add ${singularTitle}`}
              </h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              {formFields.map((field) => {
                if (field.type === 'checkbox') {
                  return (
                    <div key={field.key} className="flex items-center gap-2">
                      <input
                        type="checkbox"
                        id={field.key}
                        checked={!!formData[field.key]}
                        onChange={(e) => updateField(field.key, e.target.checked)}
                        className="nx-checkbox"
                      />
                      <label htmlFor={field.key} className="text-sm font-medium">
                        {field.label}
                      </label>
                    </div>
                  );
                }

                if (field.type === 'select') {
                  return (
                    <div key={field.key}>
                      <label className="text-sm font-medium">
                        {field.label} {field.required && '*'}
                      </label>
                      <select
                        value={formData[field.key] ?? ''}
                        onChange={(e) => updateField(field.key, e.target.value)}
                        required={field.required}
                        className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                      >
                        <option value="">Select...</option>
                        {field.options?.map((opt) => (
                          <option key={opt.value} value={opt.value}>
                            {opt.label}
                          </option>
                        ))}
                      </select>
                    </div>
                  );
                }

                return (
                  <div key={field.key}>
                    <label className="text-sm font-medium">
                      {field.label} {field.required && '*'}
                    </label>
                    <Input
                      type={field.type}
                      value={formData[field.key] ?? ''}
                      onChange={(e) => updateField(field.key, e.target.value)}
                      placeholder={field.placeholder}
                      required={field.required}
                      step={field.type === 'number' ? 'any' : undefined}
                    />
                  </div>
                );
              })}
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>
                  Cancel
                </Button>
                <Button
                  type="submit"
                  disabled={createMutation.isPending || updateMutation.isPending}
                >
                  {(createMutation.isPending || updateMutation.isPending) && (
                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                  )}
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
            <h2 className="text-lg font-semibold mb-4">Delete {singularTitle}</h2>
            <p className="text-muted-foreground mb-6">
              Are you sure you want to delete &quot;{deleteModal[nameField] || deleteModal[codeField]}&quot;?
            </p>
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setDeleteModal(null)}>
                Cancel
              </Button>
              <Button
                variant="destructive"
                onClick={() => deleteMutation.mutate(String(deleteModal[codeField]))}
                disabled={deleteMutation.isPending}
              >
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
