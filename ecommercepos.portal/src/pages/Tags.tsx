import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { 
  Plus, Search, Edit, Trash2, Tag as TagIcon,
  ChevronLeft, ChevronRight as ChevronRightIcon, Loader2, X
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { tagApi, Tag } from '@/api/tagApi';

interface TagFormData {
  name: string;
  slug: string;
}

const emptyForm: TagFormData = {
  name: '',
  slug: '',
};

export default function Tags() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingTag, setEditingTag] = useState<Tag | null>(null);
  const [formData, setFormData] = useState<TagFormData>(emptyForm);
  const [deleteModal, setDeleteModal] = useState<Tag | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['tags', currentPage, searchQuery],
    queryFn: () => tagApi.getAll({ 
      pageIndex: currentPage - 1, 
      pageSize: 10,
      search: searchQuery || undefined
    }),
  });

  const { data: tagsWithCountData } = useQuery({
    queryKey: ['tags-with-count'],
    queryFn: tagApi.getWithCount,
  });

  const createMutation = useMutation({
    mutationFn: tagApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tags'] });
      queryClient.invalidateQueries({ queryKey: ['tags-with-count'] });
      setShowModal(false);
      toast.success('Tag created');
    },
    onError: () => toast.error('Failed to create tag'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: { name: string; slug?: string } }) => 
      tagApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tags'] });
      queryClient.invalidateQueries({ queryKey: ['tags-with-count'] });
      setShowModal(false);
      toast.success('Tag updated');
    },
    onError: () => toast.error('Failed to update tag'),
  });

  const deleteMutation = useMutation({
    mutationFn: tagApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tags'] });
      queryClient.invalidateQueries({ queryKey: ['tags-with-count'] });
      setDeleteModal(null);
      toast.success('Tag deleted');
    },
    onError: () => toast.error('Failed to delete tag'),
  });

  const tags = data?.data?.items || [];
  const totalCount = data?.data?.totalCount || 0;
  const tagsWithCount = tagsWithCountData?.data?.items || [];
  const totalPages = Math.ceil(totalCount / 10);

  const handleSearch = () => setCurrentPage(1);

  const getProductCount = (tagId: string) => {
    const t = tagsWithCount.find(x => x.id === tagId);
    return t?.productCount || 0;
  };

  const openCreateModal = () => {
    setEditingTag(null);
    setFormData(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (tag: Tag) => {
    setEditingTag(tag);
    setFormData({
      name: tag.name,
      slug: tag.slug,
    });
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const slug = formData.slug || formData.name.toLowerCase().replace(/\s+/g, '-').replace(/-+/g, '-');
    if (editingTag) {
      updateMutation.mutate({ id: editingTag.id, data: { name: formData.name, slug } });
    } else {
      createMutation.mutate({ name: formData.name, slug });
    }
  };

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Tags</h1>
          <p className="nx-page-subtitle">Manage product tags</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Tag
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="nx-stat-value">{totalCount}</div>
          <div className="nx-stat-label">Total Tags</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{tagsWithCount.reduce((sum, t) => sum + t.productCount, 0)}</div>
          <div className="nx-stat-label">Total Products</div>
        </div>
        <div className="nx-stat-card">
          <div className="nx-stat-value">{tagsWithCount.filter(t => t.productCount > 0).length}</div>
          <div className="nx-stat-label">Used Tags</div>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input 
                type="text" 
                placeholder="Search tags..." 
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
                    <th>Slug</th>
                    <th>Products</th>
                    <th style={{ width: 80 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {tags.map((tag) => (
                    <tr key={tag.id}>
                      <td>
                        <div className="flex items-center gap-2">
                          <TagIcon className="w-4 h-4 text-muted-foreground" />
                          <span className="font-medium">{tag.name}</span>
                        </div>
                      </td>
                      <td><code className="text-xs text-muted-foreground">{tag.slug}</code></td>
                      <td>
                        <span className="nx-badge">{getProductCount(tag.id)} products</span>
                      </td>
                      <td>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEditModal(tag)}>
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteModal(tag)}>
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
              <p className="text-sm text-muted-foreground">Showing {tags.length} of {totalCount}</p>
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
              <h2 className="text-lg font-semibold">{editingTag ? 'Edit Tag' : 'Add Tag'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div>
                <label className="text-sm font-medium">Name *</label>
                <Input 
                  value={formData.name} 
                  onChange={(e) => setFormData({
                    ...formData, 
                    name: e.target.value,
                    slug: formData.slug || e.target.value.toLowerCase().replace(/\s+/g, '-').replace(/-+/g, '-')
                  })} 
                  required 
                />
              </div>
              <div>
                <label className="text-sm font-medium">Slug</label>
                <Input 
                  value={formData.slug} 
                  onChange={(e) => setFormData({...formData, slug: e.target.value})}
                  placeholder="auto-generated"
                />
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                  {(createMutation.isPending || updateMutation.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editingTag ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Tag</h2>
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