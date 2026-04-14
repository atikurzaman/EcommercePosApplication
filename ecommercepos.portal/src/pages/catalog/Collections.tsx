import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Edit, Trash2, Loader2, X, ChevronLeft, ChevronRight,
  Layers, Home, ArrowLeft, Package,
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import {
  collectionApi,
  type ProductCollection,
  type CollectionProduct,
} from '@/api/catalogApi';
import { productApi, type Product } from '@/api/productApi';

interface CollectionFormData {
  name: string;
  slug: string;
  description: string;
  imageUrl: string;
  displayOrder: number;
  isActive: boolean;
  showInHomePage: boolean;
}

const emptyForm: CollectionFormData = {
  name: '', slug: '', description: '', imageUrl: '',
  displayOrder: 0, isActive: true, showInHomePage: false,
};

export default function Collections() {
  const queryClient = useQueryClient();
  const [searchQuery, setSearchQuery] = useState('');
  const [page, setPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<ProductCollection | null>(null);
  const [form, setForm] = useState<CollectionFormData>(emptyForm);
  const [deleteModal, setDeleteModal] = useState<ProductCollection | null>(null);
  const [selectedCollection, setSelectedCollection] = useState<ProductCollection | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['collections', page, searchQuery],
    queryFn: () => collectionApi.getAll({ pageIndex: page - 1, pageSize: 20, search: searchQuery || undefined }),
  });

  const collections: ProductCollection[] = data?.data?.items || data?.data || [];
  const totalCount = data?.data?.totalCount || data?.data?.length || 0;
  const totalPages = Math.ceil(totalCount / 20);

  const createMut = useMutation({
    mutationFn: (d: any) => collectionApi.create(d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['collections'] }); setShowModal(false); toast.success('Collection created'); },
    onError: () => toast.error('Failed to create collection'),
  });
  const updateMut = useMutation({
    mutationFn: ({ id, d }: { id: string; d: any }) => collectionApi.update(id, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['collections'] }); setShowModal(false); toast.success('Collection updated'); },
    onError: () => toast.error('Failed to update collection'),
  });
  const deleteMut = useMutation({
    mutationFn: (id: string) => collectionApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['collections'] }); setDeleteModal(null); toast.success('Collection deleted'); },
    onError: () => toast.error('Failed to delete collection'),
  });

  const openCreate = () => { setEditing(null); setForm(emptyForm); setShowModal(true); };
  const openEdit = (c: ProductCollection) => {
    setEditing(c);
    setForm({ name: c.name, slug: c.slug, description: c.description || '', imageUrl: c.imageUrl || '', displayOrder: c.displayOrder, isActive: c.isActive, showInHomePage: c.showInHomePage });
    setShowModal(true);
  };
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editing) updateMut.mutate({ id: editing.id, d: form });
    else createMut.mutate(form);
  };

  if (selectedCollection) {
    return <CollectionProducts collection={selectedCollection} onBack={() => { setSelectedCollection(null); queryClient.invalidateQueries({ queryKey: ['collections'] }); }} />;
  }

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Collections</h1>
          <p className="nx-page-subtitle">Manage product collections</p>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={openCreate}><Plus className="w-4 h-4 mr-2" />Add Collection</Button>
        </div>
      </div>

      <Card>
        <div className="p-4 border-b">
          <div className="nx-table-toolbar">
            <div className="nx-table-search">
              <Search className="w-4 h-4" />
              <input type="text" placeholder="Search collections..." value={searchQuery} onChange={e => setSearchQuery(e.target.value)} onKeyDown={e => e.key === 'Enter' && setPage(1)} />
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
                    <th>Name</th>
                    <th>Slug</th>
                    <th style={{ textAlign: 'right' }}>Products</th>
                    <th>Active</th>
                    <th>Home Page</th>
                    <th>Order</th>
                    <th style={{ width: 120 }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {collections.map(c => (
                    <tr key={c.id} className="cursor-pointer" onClick={() => setSelectedCollection(c)}>
                      <td className="font-medium">{c.name}</td>
                      <td><code className="text-xs bg-secondary px-2 py-1 rounded">{c.slug}</code></td>
                      <td style={{ textAlign: 'right' }}>{c.productCount ?? 0}</td>
                      <td><span className={`nx-badge ${c.isActive ? 'nx-badge-success' : 'nx-badge-danger'}`}>{c.isActive ? 'Active' : 'Inactive'}</span></td>
                      <td>{c.showInHomePage ? <Home className="w-4 h-4 text-primary" /> : <span className="text-muted-foreground">-</span>}</td>
                      <td>{c.displayOrder}</td>
                      <td>
                        <div className="flex items-center gap-1" onClick={e => e.stopPropagation()}>
                          <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEdit(c)}><Edit className="w-4 h-4" /></Button>
                          <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => setDeleteModal(c)}><Trash2 className="w-4 h-4" /></Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                  {collections.length === 0 && (
                    <tr><td colSpan={7} className="text-center py-8 text-muted-foreground">
                      <Layers className="w-8 h-8 mx-auto mb-2 opacity-50" /><p>No collections found</p>
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
                  <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}><ChevronRight className="w-4 h-4" /></Button>
                </div>
              </div>
            )}
          </>
        )}
      </Card>

      {/* Create/Edit Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editing ? 'Edit Collection' : 'Add Collection'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}><X className="w-4 h-4" /></Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div><label className="text-sm font-medium">Name *</label><Input value={form.name} onChange={e => setForm({ ...form, name: e.target.value, slug: form.slug || e.target.value.toLowerCase().replace(/\s+/g, '-') })} required /></div>
              <div><label className="text-sm font-medium">Slug</label><Input value={form.slug} onChange={e => setForm({ ...form, slug: e.target.value })} /></div>
              <div><label className="text-sm font-medium">Description</label><textarea className="nx-input w-full h-20" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} /></div>
              <div><label className="text-sm font-medium">Image URL</label><Input value={form.imageUrl} onChange={e => setForm({ ...form, imageUrl: e.target.value })} /></div>
              <div className="grid grid-cols-2 gap-4">
                <div><label className="text-sm font-medium">Display Order</label><Input type="number" value={form.displayOrder} onChange={e => setForm({ ...form, displayOrder: parseInt(e.target.value) || 0 })} /></div>
                <div className="flex flex-col gap-2 pt-6">
                  <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={form.isActive} onChange={e => setForm({ ...form, isActive: e.target.checked })} />Active</label>
                  <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={form.showInHomePage} onChange={e => setForm({ ...form, showInHomePage: e.target.checked })} />Show on Home Page</label>
                </div>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={createMut.isPending || updateMut.isPending}>
                  {(createMut.isPending || updateMut.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editing ? 'Update' : 'Create'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Delete Modal */}
      {deleteModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">Delete Collection</h2>
            <p className="text-muted-foreground mb-6">Are you sure you want to delete "{deleteModal.name}"?</p>
            <div className="flex justify-end gap-2">
              <Button variant="outline" onClick={() => setDeleteModal(null)}>Cancel</Button>
              <Button variant="destructive" onClick={() => deleteMut.mutate(deleteModal.id)} disabled={deleteMut.isPending}>
                {deleteMut.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                Delete
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

// ==================== COLLECTION PRODUCTS SUB-VIEW ====================
function CollectionProducts({ collection, onBack }: { collection: ProductCollection; onBack: () => void }) {
  const queryClient = useQueryClient();
  const [productSearch, setProductSearch] = useState('');
  const [showAddProducts, setShowAddProducts] = useState(false);

  const { data: collectionData, isLoading } = useQuery({
    queryKey: ['collection-detail', collection.id],
    queryFn: () => collectionApi.getById(collection.id),
  });
  const products: CollectionProduct[] = collectionData?.data?.products || [];

  const { data: searchData } = useQuery({
    queryKey: ['products-search', productSearch],
    queryFn: () => productApi.getAll({ pageSize: 20, search: productSearch || undefined }),
    enabled: showAddProducts && productSearch.length > 0,
  });
  const searchResults: Product[] = searchData?.data?.items || [];
  const existingProductIds = new Set(products.map(p => p.productId));

  const manageItemsMut = useMutation({
    mutationFn: (items: { productId: string; displayOrder: number }[]) => collectionApi.manageItems(collection.id, items),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['collection-detail', collection.id] }); toast.success('Products updated'); },
    onError: () => toast.error('Failed to update products'),
  });

  const addProduct = (productId: string) => {
    const items = [...products.map(p => ({ productId: p.productId, displayOrder: p.displayOrder })), { productId, displayOrder: products.length }];
    manageItemsMut.mutate(items);
  };

  const removeProduct = (productId: string) => {
    const items = products.filter(p => p.productId !== productId).map((p, i) => ({ productId: p.productId, displayOrder: i }));
    manageItemsMut.mutate(items);
  };

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={onBack}><ArrowLeft className="w-5 h-5" /></Button>
          <div>
            <h1 className="nx-page-title">{collection.name}</h1>
            <p className="nx-page-subtitle">{products.length} products in this collection</p>
          </div>
        </div>
        <div className="nx-page-actions">
          <Button size="sm" onClick={() => setShowAddProducts(!showAddProducts)}>
            <Plus className="w-4 h-4 mr-2" />{showAddProducts ? 'Close' : 'Add Products'}
          </Button>
        </div>
      </div>

      {showAddProducts && (
        <Card className="p-4">
          <div className="nx-table-search mb-3">
            <Search className="w-4 h-4" />
            <input type="text" placeholder="Search products to add..." value={productSearch} onChange={e => setProductSearch(e.target.value)} />
          </div>
          <div className="max-h-64 overflow-y-auto border rounded-lg">
            {searchResults.filter(p => !existingProductIds.has(p.id)).map(p => (
              <div key={p.id} className="flex items-center justify-between px-4 py-2 hover:bg-secondary/30 border-b last:border-b-0">
                <div className="flex items-center gap-3">
                  <Package className="w-4 h-4 text-muted-foreground" />
                  <span className="text-sm font-medium">{p.productName}</span>
                  <code className="text-xs bg-secondary px-2 py-0.5 rounded">{p.productCode}</code>
                </div>
                <Button size="sm" variant="outline" onClick={() => addProduct(p.id)} disabled={manageItemsMut.isPending}><Plus className="w-3 h-3 mr-1" />Add</Button>
              </div>
            ))}
            {productSearch && searchResults.filter(p => !existingProductIds.has(p.id)).length === 0 && (
              <div className="text-center text-muted-foreground py-4">No products found</div>
            )}
            {!productSearch && <div className="text-center text-muted-foreground py-4">Type to search products</div>}
          </div>
        </Card>
      )}

      <Card>
        {isLoading ? (
          <div className="flex items-center justify-center p-8"><Loader2 className="w-8 h-8 animate-spin" /></div>
        ) : (
          <div className="nx-table-wrap">
            <table className="nx-table">
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Code</th>
                  <th style={{ textAlign: 'right' }}>Price</th>
                  <th>Order</th>
                  <th style={{ width: 60 }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {products.map(p => (
                  <tr key={p.id}>
                    <td className="font-medium">{p.productName}</td>
                    <td><code className="text-xs bg-secondary px-2 py-1 rounded">{p.productCode || '-'}</code></td>
                    <td style={{ textAlign: 'right' }}>{new Intl.NumberFormat('en-BD', { style: 'currency', currency: 'BDT' }).format(p.salePrice)}</td>
                    <td>{p.displayOrder}</td>
                    <td>
                      <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => removeProduct(p.productId)}>
                        <Trash2 className="w-4 h-4" />
                      </Button>
                    </td>
                  </tr>
                ))}
                {products.length === 0 && (
                  <tr><td colSpan={5} className="text-center text-muted-foreground py-8">No products in this collection</td></tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </Card>
    </div>
  );
}
