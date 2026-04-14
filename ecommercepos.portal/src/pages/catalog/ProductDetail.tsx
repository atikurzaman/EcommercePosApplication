import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  ArrowLeft, Save, Loader2, Plus, Trash2, X, Star, GripVertical,
  Search, Package,
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { productApi, type Product } from '@/api/productApi';
import { categoryApi } from '@/api/categoryApi';
import { brandApi } from '@/api/brandApi';
import {
  productVariantApi, productImageApi, productTagApi, tagApi,
  productSpecApi, specificationApi, productSupplierApi,
  productPriceHistoryApi, productAttributeApi, attributeTypeApi,
  type ProductVariant, type ProductImage, type ProductTag, type Tag,
  type ProductSpecValue, type Specification, type ProductSupplierLink,
  type ProductPriceHistory, type ProductAttributeLink, type AttributeType,
  type AttributeOption,
} from '@/api/catalogApi';
import { supplierApi, type Supplier } from '@/api/supplierApi';

function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-BD', { style: 'currency', currency: 'BDT' }).format(amount);
}

type TabId = 'details' | 'variants' | 'images' | 'specifications' | 'tags' | 'suppliers' | 'price-history' | 'attributes';

const tabs: TabId[] = ['details', 'variants', 'images', 'specifications', 'tags', 'suppliers', 'price-history', 'attributes'];

export default function ProductDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [activeTab, setActiveTab] = useState<TabId>('details');

  const { data: productData, isLoading } = useQuery({
    queryKey: ['product', id],
    queryFn: () => productApi.getById(id!),
    enabled: !!id,
  });

  const product: Product | undefined = productData?.data;

  if (isLoading) {
    return (
      <div className="flex items-center justify-center p-16">
        <Loader2 className="w-8 h-8 animate-spin" />
      </div>
    );
  }

  if (!product) {
    return (
      <div className="text-center p-16">
        <p className="text-muted-foreground">Product not found</p>
        <Button variant="outline" className="mt-4" onClick={() => navigate('/products')}>Back to Products</Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => navigate('/products')}>
            <ArrowLeft className="w-5 h-5" />
          </Button>
          <div>
            <h1 className="nx-page-title">{product.productName}</h1>
            <p className="nx-page-subtitle">
              {product.productCode} &middot; {formatCurrency(product.sellPrice)} &middot;{' '}
              <span className={`nx-badge ${product.isActive ? 'nx-badge-success' : 'nx-badge-danger'}`}>
                {product.isActive ? 'Active' : 'Inactive'}
              </span>
            </p>
          </div>
        </div>
      </div>

      <div className="flex gap-1 border-b mb-4">
        {tabs.map(tab => (
          <button key={tab} onClick={() => setActiveTab(tab)}
            className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px ${activeTab === tab ? 'border-primary text-primary' : 'border-transparent text-muted-foreground hover:text-foreground'}`}>
            {tab.charAt(0).toUpperCase() + tab.slice(1).replace('-', ' ')}
          </button>
        ))}
      </div>

      {activeTab === 'details' && <DetailsTab product={product} />}
      {activeTab === 'variants' && <VariantsTab productId={id!} />}
      {activeTab === 'images' && <ImagesTab productId={id!} />}
      {activeTab === 'specifications' && <SpecificationsTab productId={id!} />}
      {activeTab === 'tags' && <TagsTab productId={id!} />}
      {activeTab === 'suppliers' && <SuppliersTab productId={id!} />}
      {activeTab === 'price-history' && <PriceHistoryTab productId={id!} />}
      {activeTab === 'attributes' && <AttributesTab productId={id!} />}
    </div>
  );
}

// ==================== DETAILS TAB ====================
function DetailsTab({ product }: { product: Product }) {
  const queryClient = useQueryClient();
  const [form, setForm] = useState({
    productName: product.productName || '',
    productCode: product.productCode || '',
    description: product.description || '',
    sku: product.sku || '',
    barcode: product.barcode || '',
    categoryId: product.categoryId || '',
    brandId: product.brandId || '',
    costPrice: product.costPrice || 0,
    sellPrice: product.sellPrice || 0,
    mrp: product.mrp || 0,
    isActive: product.isActive,
  });

  const { data: categoriesData } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoryApi.getAll({ pageSize: 100 }),
  });
  const { data: brandsData } = useQuery({
    queryKey: ['brands'],
    queryFn: () => brandApi.getAll({ pageSize: 100 }),
  });

  const categories = categoriesData?.data?.items || [];
  const brands = brandsData?.data?.items || [];

  const updateMutation = useMutation({
    mutationFn: (data: Partial<Product>) => productApi.update(product.id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['product', product.id] });
      toast.success('Product updated');
    },
    onError: () => toast.error('Failed to update product'),
  });

  const handleSave = () => updateMutation.mutate(form);

  return (
    <Card className="p-6">
      <div className="grid grid-cols-2 gap-4">
        <div className="col-span-2">
          <label className="text-sm font-medium">Product Name</label>
          <Input value={form.productName} onChange={e => setForm({ ...form, productName: e.target.value })} />
        </div>
        <div>
          <label className="text-sm font-medium">Product Code</label>
          <Input value={form.productCode} onChange={e => setForm({ ...form, productCode: e.target.value })} />
        </div>
        <div>
          <label className="text-sm font-medium">SKU</label>
          <Input value={form.sku} onChange={e => setForm({ ...form, sku: e.target.value })} />
        </div>
        <div>
          <label className="text-sm font-medium">Barcode</label>
          <Input value={form.barcode} onChange={e => setForm({ ...form, barcode: e.target.value })} />
        </div>
        <div>
          <label className="text-sm font-medium">Category</label>
          <select className="nx-input nx-select w-full" value={form.categoryId} onChange={e => setForm({ ...form, categoryId: e.target.value })}>
            <option value="">Select Category</option>
            {categories.map((c: any) => <option key={c.id} value={c.id}>{c.categoryName}</option>)}
          </select>
        </div>
        <div>
          <label className="text-sm font-medium">Brand</label>
          <select className="nx-input nx-select w-full" value={form.brandId} onChange={e => setForm({ ...form, brandId: e.target.value })}>
            <option value="">Select Brand</option>
            {brands.map((b: any) => <option key={b.id} value={b.id}>{b.brandName}</option>)}
          </select>
        </div>
        <div>
          <label className="text-sm font-medium">Cost Price</label>
          <Input type="number" value={form.costPrice} onChange={e => setForm({ ...form, costPrice: parseFloat(e.target.value) || 0 })} />
        </div>
        <div>
          <label className="text-sm font-medium">Sell Price</label>
          <Input type="number" value={form.sellPrice} onChange={e => setForm({ ...form, sellPrice: parseFloat(e.target.value) || 0 })} />
        </div>
        <div>
          <label className="text-sm font-medium">MRP</label>
          <Input type="number" value={form.mrp} onChange={e => setForm({ ...form, mrp: parseFloat(e.target.value) || 0 })} />
        </div>
        <div className="flex items-center gap-2 pt-6">
          <input type="checkbox" className="nx-checkbox" checked={form.isActive} onChange={e => setForm({ ...form, isActive: e.target.checked })} />
          <label className="text-sm font-medium">Active</label>
        </div>
        <div className="col-span-2">
          <label className="text-sm font-medium">Description</label>
          <textarea className="nx-input w-full h-24" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} />
        </div>
        <div className="col-span-2 flex justify-end">
          <Button onClick={handleSave} disabled={updateMutation.isPending}>
            {updateMutation.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
            <Save className="w-4 h-4 mr-2" /> Save Changes
          </Button>
        </div>
      </div>
    </Card>
  );
}

// ==================== VARIANTS TAB ====================
function VariantsTab({ productId }: { productId: string }) {
  const queryClient = useQueryClient();
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<ProductVariant | null>(null);
  const [form, setForm] = useState({ name: '', sku: '', barcode: '', costPrice: 0, priceModifier: 0, overridePrice: undefined as number | undefined, isDefault: false, isActive: true, sortOrder: 0 });

  const { data, isLoading } = useQuery({
    queryKey: ['product-variants', productId],
    queryFn: () => productVariantApi.getByProduct(productId),
  });
  const variants: ProductVariant[] = data?.data?.items || data?.data || [];

  const createMut = useMutation({
    mutationFn: (d: any) => productVariantApi.create(productId, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-variants', productId] }); setShowModal(false); toast.success('Variant created'); },
    onError: () => toast.error('Failed to create variant'),
  });
  const updateMut = useMutation({
    mutationFn: ({ vid, d }: { vid: string; d: any }) => productVariantApi.update(productId, vid, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-variants', productId] }); setShowModal(false); toast.success('Variant updated'); },
    onError: () => toast.error('Failed to update variant'),
  });
  const deleteMut = useMutation({
    mutationFn: (vid: string) => productVariantApi.delete(productId, vid),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-variants', productId] }); toast.success('Variant deleted'); },
    onError: () => toast.error('Failed to delete variant'),
  });

  const openCreate = () => { setEditing(null); setForm({ name: '', sku: '', barcode: '', costPrice: 0, priceModifier: 0, overridePrice: undefined, isDefault: false, isActive: true, sortOrder: 0 }); setShowModal(true); };
  const openEdit = (v: ProductVariant) => { setEditing(v); setForm({ name: v.name, sku: v.sku || '', barcode: v.barcode || '', costPrice: v.costPrice, priceModifier: v.priceModifier, overridePrice: v.overridePrice, isDefault: v.isDefault, isActive: v.isActive, sortOrder: v.sortOrder }); setShowModal(true); };
  const handleSubmit = (e: React.FormEvent) => { e.preventDefault(); if (editing) updateMut.mutate({ vid: editing.id, d: form }); else createMut.mutate(form); };

  return (
    <Card>
      <div className="flex items-center justify-between p-4 border-b">
        <h3 className="font-semibold">Variants ({variants.length})</h3>
        <Button size="sm" onClick={openCreate}><Plus className="w-4 h-4 mr-2" />Add Variant</Button>
      </div>
      {isLoading ? <div className="flex justify-center p-8"><Loader2 className="w-6 h-6 animate-spin" /></div> : (
        <div className="nx-table-wrap">
          <table className="nx-table">
            <thead><tr><th>Name</th><th>SKU</th><th>Cost</th><th>Price Mod</th><th>Default</th><th>Status</th><th>Attributes</th><th style={{ width: 80 }}>Actions</th></tr></thead>
            <tbody>
              {variants.map(v => (
                <tr key={v.id}>
                  <td className="font-medium">{v.name}</td>
                  <td><code className="text-xs bg-secondary px-2 py-1 rounded">{v.sku || '-'}</code></td>
                  <td>{formatCurrency(v.costPrice)}</td>
                  <td>{v.priceModifier ? formatCurrency(v.priceModifier) : '-'}</td>
                  <td>{v.isDefault ? <span className="nx-badge nx-badge-info">Default</span> : '-'}</td>
                  <td><span className={`nx-badge ${v.isActive ? 'nx-badge-success' : 'nx-badge-danger'}`}>{v.isActive ? 'Active' : 'Inactive'}</span></td>
                  <td><div className="flex gap-1 flex-wrap">{v.attributes?.map(a => <span key={a.optionId} className="nx-badge nx-badge-neutral text-xs">{a.attributeTypeName}: {a.optionValue}</span>)}</div></td>
                  <td>
                    <div className="flex items-center gap-1">
                      <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEdit(v)}><Save className="w-4 h-4" /></Button>
                      <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => deleteMut.mutate(v.id)}><Trash2 className="w-4 h-4" /></Button>
                    </div>
                  </td>
                </tr>
              ))}
              {variants.length === 0 && <tr><td colSpan={8} className="text-center text-muted-foreground py-8">No variants yet</td></tr>}
            </tbody>
          </table>
        </div>
      )}
      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editing ? 'Edit Variant' : 'Add Variant'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}><X className="w-4 h-4" /></Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="col-span-2"><label className="text-sm font-medium">Name *</label><Input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required /></div>
                <div><label className="text-sm font-medium">SKU</label><Input value={form.sku} onChange={e => setForm({ ...form, sku: e.target.value })} /></div>
                <div><label className="text-sm font-medium">Barcode</label><Input value={form.barcode} onChange={e => setForm({ ...form, barcode: e.target.value })} /></div>
                <div><label className="text-sm font-medium">Cost Price</label><Input type="number" value={form.costPrice} onChange={e => setForm({ ...form, costPrice: parseFloat(e.target.value) || 0 })} /></div>
                <div><label className="text-sm font-medium">Price Modifier</label><Input type="number" value={form.priceModifier} onChange={e => setForm({ ...form, priceModifier: parseFloat(e.target.value) || 0 })} /></div>
                <div><label className="text-sm font-medium">Sort Order</label><Input type="number" value={form.sortOrder} onChange={e => setForm({ ...form, sortOrder: parseInt(e.target.value) || 0 })} /></div>
                <div className="flex items-center gap-4 pt-6">
                  <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={form.isDefault} onChange={e => setForm({ ...form, isDefault: e.target.checked })} />Default</label>
                  <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={form.isActive} onChange={e => setForm({ ...form, isActive: e.target.checked })} />Active</label>
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
    </Card>
  );
}

// ==================== IMAGES TAB ====================
function ImagesTab({ productId }: { productId: string }) {
  const queryClient = useQueryClient();
  const [showAdd, setShowAdd] = useState(false);
  const [url, setUrl] = useState('');
  const [altText, setAltText] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['product-images', productId],
    queryFn: () => productImageApi.getByProduct(productId),
  });
  const images: ProductImage[] = data?.data?.items || data?.data || [];

  const addMut = useMutation({
    mutationFn: (d: any) => productImageApi.add(productId, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-images', productId] }); setShowAdd(false); setUrl(''); setAltText(''); toast.success('Image added'); },
    onError: () => toast.error('Failed to add image'),
  });
  const updateMut = useMutation({
    mutationFn: ({ imgId, d }: { imgId: string; d: any }) => productImageApi.update(productId, imgId, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-images', productId] }); toast.success('Image updated'); },
    onError: () => toast.error('Failed to update image'),
  });
  const deleteMut = useMutation({
    mutationFn: (imgId: string) => productImageApi.delete(productId, imgId),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-images', productId] }); toast.success('Image deleted'); },
    onError: () => toast.error('Failed to delete image'),
  });
  const reorderMut = useMutation({
    mutationFn: (orders: { imageId: string; sortOrder: number }[]) => productImageApi.reorder(productId, orders),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-images', productId] }); toast.success('Reordered'); },
    onError: () => toast.error('Failed to reorder'),
  });

  const handleAdd = () => { if (!url.trim()) return; addMut.mutate({ imageUrl: url, altText, sortOrder: images.length, isPrimary: images.length === 0 }); };
  const setPrimary = (img: ProductImage) => { updateMut.mutate({ imgId: img.id, d: { ...img, isPrimary: true } }); };

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-semibold">Images ({images.length})</h3>
        <Button size="sm" onClick={() => setShowAdd(!showAdd)}><Plus className="w-4 h-4 mr-2" />Add Image</Button>
      </div>
      {showAdd && (
        <div className="flex gap-2 mb-4 p-4 bg-secondary/30 rounded-lg">
          <Input placeholder="Image URL" value={url} onChange={e => setUrl(e.target.value)} className="flex-1" />
          <Input placeholder="Alt text" value={altText} onChange={e => setAltText(e.target.value)} className="w-48" />
          <Button onClick={handleAdd} disabled={addMut.isPending}>{addMut.isPending ? <Loader2 className="w-4 h-4 animate-spin" /> : 'Add'}</Button>
          <Button variant="ghost" onClick={() => setShowAdd(false)}><X className="w-4 h-4" /></Button>
        </div>
      )}
      {isLoading ? <div className="flex justify-center p-8"><Loader2 className="w-6 h-6 animate-spin" /></div> : (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {images.map((img, i) => (
            <div key={img.id} className={`relative group border rounded-lg overflow-hidden ${img.isPrimary ? 'ring-2 ring-primary' : ''}`}>
              <div className="aspect-square bg-secondary flex items-center justify-center">
                <img src={img.imageUrl} alt={img.altText || ''} className="w-full h-full object-cover" onError={e => { (e.target as HTMLImageElement).style.display = 'none'; }} />
              </div>
              <div className="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center gap-2">
                {!img.isPrimary && <Button size="sm" variant="secondary" onClick={() => setPrimary(img)}><Star className="w-4 h-4 mr-1" />Primary</Button>}
                <Button size="sm" variant="destructive" onClick={() => deleteMut.mutate(img.id)}><Trash2 className="w-4 h-4" /></Button>
              </div>
              {img.isPrimary && <div className="absolute top-2 left-2"><span className="nx-badge nx-badge-info text-xs">Primary</span></div>}
              <div className="p-2 text-xs text-muted-foreground truncate">{img.altText || `Image ${i + 1}`}</div>
            </div>
          ))}
          {images.length === 0 && <div className="col-span-4 text-center text-muted-foreground py-8">No images yet</div>}
        </div>
      )}
    </Card>
  );
}

// ==================== SPECIFICATIONS TAB ====================
function SpecificationsTab({ productId }: { productId: string }) {
  const queryClient = useQueryClient();
  const [newSpec, setNewSpec] = useState({ specId: '', value: '' });

  const { data: specsData, isLoading } = useQuery({
    queryKey: ['product-specs', productId],
    queryFn: () => productSpecApi.getByProduct(productId),
  });
  const { data: allSpecsData } = useQuery({
    queryKey: ['specifications'],
    queryFn: () => specificationApi.getAll({ pageSize: 100 }),
  });
  const specs: ProductSpecValue[] = specsData?.data?.items || specsData?.data || [];
  const allSpecs: Specification[] = allSpecsData?.data?.items || allSpecsData?.data || [];

  const manageMut = useMutation({
    mutationFn: (values: { specId: string; value: string }[]) => productSpecApi.manage(productId, values),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-specs', productId] }); toast.success('Specifications saved'); },
    onError: () => toast.error('Failed to save specifications'),
  });

  const [localSpecs, setLocalSpecs] = useState<{ specId: string; specName: string; value: string }[]>([]);
  const isInitialized = localSpecs.length > 0 || specs.length === 0;

  if (specs.length > 0 && localSpecs.length === 0) {
    // Intentionally not using useEffect - this sets initial state once
    const mapped = specs.map(s => ({ specId: s.specId, specName: s.specName, value: s.value }));
    if (mapped.length > 0) setLocalSpecs(mapped);
  }

  const handleAdd = () => {
    if (!newSpec.specId || !newSpec.value.trim()) return;
    const spec = allSpecs.find(s => s.id === newSpec.specId);
    setLocalSpecs([...localSpecs, { specId: newSpec.specId, specName: spec?.specName || '', value: newSpec.value }]);
    setNewSpec({ specId: '', value: '' });
  };

  const handleRemove = (idx: number) => setLocalSpecs(localSpecs.filter((_, i) => i !== idx));
  const handleSave = () => manageMut.mutate(localSpecs.map(s => ({ specId: s.specId, value: s.value })));

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-semibold">Specifications</h3>
        <Button size="sm" onClick={handleSave} disabled={manageMut.isPending}>
          {manageMut.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
          <Save className="w-4 h-4 mr-2" />Save
        </Button>
      </div>
      {isLoading ? <div className="flex justify-center p-8"><Loader2 className="w-6 h-6 animate-spin" /></div> : (
        <>
          <div className="nx-table-wrap mb-4">
            <table className="nx-table">
              <thead><tr><th>Specification</th><th>Value</th><th style={{ width: 60 }}></th></tr></thead>
              <tbody>
                {localSpecs.map((s, i) => (
                  <tr key={i}>
                    <td className="font-medium">{s.specName}</td>
                    <td><Input value={s.value} onChange={e => { const next = [...localSpecs]; next[i] = { ...next[i], value: e.target.value }; setLocalSpecs(next); }} className="h-8" /></td>
                    <td><Button variant="ghost" size="icon" className="w-7 h-7 text-red-500" onClick={() => handleRemove(i)}><Trash2 className="w-3 h-3" /></Button></td>
                  </tr>
                ))}
                {localSpecs.length === 0 && <tr><td colSpan={3} className="text-center text-muted-foreground py-4">No specifications</td></tr>}
              </tbody>
            </table>
          </div>
          <div className="flex gap-2 p-3 bg-secondary/30 rounded-lg">
            <select className="nx-input nx-select flex-1" value={newSpec.specId} onChange={e => setNewSpec({ ...newSpec, specId: e.target.value })}>
              <option value="">Select specification...</option>
              {allSpecs.filter(s => !localSpecs.some(ls => ls.specId === s.id)).map(s => <option key={s.id} value={s.id}>{s.specName}</option>)}
            </select>
            <Input placeholder="Value" value={newSpec.value} onChange={e => setNewSpec({ ...newSpec, value: e.target.value })} className="flex-1" />
            <Button variant="outline" onClick={handleAdd} disabled={!newSpec.specId || !newSpec.value.trim()}><Plus className="w-4 h-4" /></Button>
          </div>
        </>
      )}
    </Card>
  );
}

// ==================== TAGS TAB ====================
function TagsTab({ productId }: { productId: string }) {
  const queryClient = useQueryClient();
  const [search, setSearch] = useState('');

  const { data: prodTagsData, isLoading } = useQuery({
    queryKey: ['product-tags', productId],
    queryFn: () => productTagApi.getByProduct(productId),
  });
  const { data: allTagsData } = useQuery({
    queryKey: ['tags-all'],
    queryFn: () => tagApi.getAll({ pageSize: 200 }),
  });

  const productTags: ProductTag[] = prodTagsData?.data?.items || prodTagsData?.data || [];
  const allTags: Tag[] = allTagsData?.data?.items || allTagsData?.data || [];
  const assignedIds = new Set(productTags.map(t => t.tagId));

  const manageMut = useMutation({
    mutationFn: (tagIds: string[]) => productTagApi.manage(productId, tagIds),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-tags', productId] }); toast.success('Tags updated'); },
    onError: () => toast.error('Failed to update tags'),
  });

  const toggle = (tagId: string) => {
    const current = productTags.map(t => t.tagId);
    const next = assignedIds.has(tagId) ? current.filter(id => id !== tagId) : [...current, tagId];
    manageMut.mutate(next);
  };

  const filtered = allTags.filter(t => t.name.toLowerCase().includes(search.toLowerCase()));

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-semibold">Tags ({productTags.length} assigned)</h3>
      </div>
      <div className="mb-4">
        <div className="nx-table-search mb-3">
          <Search className="w-4 h-4" />
          <input type="text" placeholder="Search tags..." value={search} onChange={e => setSearch(e.target.value)} />
        </div>
        <div className="flex flex-wrap gap-2 mb-4">
          {productTags.map(t => (
            <span key={t.tagId} className="nx-badge nx-badge-info flex items-center gap-1 cursor-pointer" onClick={() => toggle(t.tagId)}>
              {t.name} <X className="w-3 h-3" />
            </span>
          ))}
          {productTags.length === 0 && <span className="text-sm text-muted-foreground">No tags assigned</span>}
        </div>
      </div>
      {isLoading ? <div className="flex justify-center p-4"><Loader2 className="w-6 h-6 animate-spin" /></div> : (
        <div className="border rounded-lg max-h-64 overflow-y-auto">
          {filtered.map(t => (
            <label key={t.id} className="flex items-center gap-3 px-4 py-2 hover:bg-secondary/30 cursor-pointer border-b last:border-b-0">
              <input type="checkbox" className="nx-checkbox" checked={assignedIds.has(t.id)} onChange={() => toggle(t.id)} />
              <span className="text-sm">{t.name}</span>
              <span className="text-xs text-muted-foreground ml-auto">{t.slug}</span>
            </label>
          ))}
          {filtered.length === 0 && <div className="text-center text-muted-foreground py-4">No tags found</div>}
        </div>
      )}
    </Card>
  );
}

// ==================== SUPPLIERS TAB ====================
function SuppliersTab({ productId }: { productId: string }) {
  const queryClient = useQueryClient();
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState<ProductSupplierLink | null>(null);
  const [form, setForm] = useState({ supplierId: '', supplierSku: '', unitCost: 0, leadTimeDays: 0, isPreferred: false, isActive: true });

  const { data, isLoading } = useQuery({
    queryKey: ['product-suppliers', productId],
    queryFn: () => productSupplierApi.getByProduct(productId),
  });
  const { data: suppliersData } = useQuery({
    queryKey: ['suppliers-all'],
    queryFn: () => supplierApi.getAll({ pageSize: 100 }),
  });

  const links: ProductSupplierLink[] = data?.data?.items || data?.data || [];
  const suppliersArray = suppliersData?.data;
  const allSuppliers: Supplier[] = Array.isArray(suppliersArray) ? suppliersArray : (suppliersArray as any)?.items || [];

  const addMut = useMutation({
    mutationFn: (d: any) => productSupplierApi.add(productId, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-suppliers', productId] }); setShowModal(false); toast.success('Supplier link added'); },
    onError: () => toast.error('Failed to add supplier'),
  });
  const updateMut = useMutation({
    mutationFn: ({ lid, d }: { lid: string; d: any }) => productSupplierApi.update(productId, lid, d),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-suppliers', productId] }); setShowModal(false); toast.success('Supplier link updated'); },
    onError: () => toast.error('Failed to update supplier'),
  });
  const deleteMut = useMutation({
    mutationFn: (lid: string) => productSupplierApi.delete(productId, lid),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-suppliers', productId] }); toast.success('Supplier link removed'); },
    onError: () => toast.error('Failed to remove supplier'),
  });

  const openCreate = () => { setEditing(null); setForm({ supplierId: '', supplierSku: '', unitCost: 0, leadTimeDays: 0, isPreferred: false, isActive: true }); setShowModal(true); };
  const openEdit = (l: ProductSupplierLink) => { setEditing(l); setForm({ supplierId: l.supplierId, supplierSku: l.supplierSku || '', unitCost: l.unitCost || 0, leadTimeDays: l.leadTimeDays || 0, isPreferred: l.isPreferred, isActive: l.isActive }); setShowModal(true); };
  const handleSubmit = (e: React.FormEvent) => { e.preventDefault(); if (editing) updateMut.mutate({ lid: editing.id, d: form }); else addMut.mutate(form); };

  return (
    <Card>
      <div className="flex items-center justify-between p-4 border-b">
        <h3 className="font-semibold">Suppliers ({links.length})</h3>
        <Button size="sm" onClick={openCreate}><Plus className="w-4 h-4 mr-2" />Add Supplier</Button>
      </div>
      {isLoading ? <div className="flex justify-center p-8"><Loader2 className="w-6 h-6 animate-spin" /></div> : (
        <div className="nx-table-wrap">
          <table className="nx-table">
            <thead><tr><th>Supplier</th><th>SKU</th><th>Unit Cost</th><th>Lead Time</th><th>Preferred</th><th>Status</th><th style={{ width: 80 }}>Actions</th></tr></thead>
            <tbody>
              {links.map(l => (
                <tr key={l.id}>
                  <td className="font-medium">{l.supplierName}</td>
                  <td><code className="text-xs bg-secondary px-2 py-1 rounded">{l.supplierSku || '-'}</code></td>
                  <td>{l.unitCost ? formatCurrency(l.unitCost) : '-'}</td>
                  <td>{l.leadTimeDays ? `${l.leadTimeDays} days` : '-'}</td>
                  <td>{l.isPreferred ? <span className="nx-badge nx-badge-info">Preferred</span> : '-'}</td>
                  <td><span className={`nx-badge ${l.isActive ? 'nx-badge-success' : 'nx-badge-danger'}`}>{l.isActive ? 'Active' : 'Inactive'}</span></td>
                  <td>
                    <div className="flex items-center gap-1">
                      <Button variant="ghost" size="icon" className="w-8 h-8" onClick={() => openEdit(l)}><Save className="w-4 h-4" /></Button>
                      <Button variant="ghost" size="icon" className="w-8 h-8 text-red-500" onClick={() => deleteMut.mutate(l.id)}><Trash2 className="w-4 h-4" /></Button>
                    </div>
                  </td>
                </tr>
              ))}
              {links.length === 0 && <tr><td colSpan={7} className="text-center text-muted-foreground py-8">No supplier links</td></tr>}
            </tbody>
          </table>
        </div>
      )}
      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-background rounded-lg w-full max-w-lg">
            <div className="flex items-center justify-between p-4 border-b">
              <h2 className="text-lg font-semibold">{editing ? 'Edit Supplier Link' : 'Add Supplier Link'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}><X className="w-4 h-4" /></Button>
            </div>
            <form onSubmit={handleSubmit} className="p-4 space-y-4">
              <div><label className="text-sm font-medium">Supplier *</label>
                <select className="nx-input nx-select w-full" value={form.supplierId} onChange={e => setForm({ ...form, supplierId: e.target.value })} required>
                  <option value="">Select supplier...</option>
                  {allSuppliers.map(s => <option key={s.id} value={s.id}>{s.supplierName}</option>)}
                </select>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div><label className="text-sm font-medium">Supplier SKU</label><Input value={form.supplierSku} onChange={e => setForm({ ...form, supplierSku: e.target.value })} /></div>
                <div><label className="text-sm font-medium">Unit Cost</label><Input type="number" value={form.unitCost} onChange={e => setForm({ ...form, unitCost: parseFloat(e.target.value) || 0 })} /></div>
                <div><label className="text-sm font-medium">Lead Time (days)</label><Input type="number" value={form.leadTimeDays} onChange={e => setForm({ ...form, leadTimeDays: parseInt(e.target.value) || 0 })} /></div>
                <div className="flex items-center gap-4 pt-6">
                  <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={form.isPreferred} onChange={e => setForm({ ...form, isPreferred: e.target.checked })} />Preferred</label>
                  <label className="flex items-center gap-2 text-sm"><input type="checkbox" className="nx-checkbox" checked={form.isActive} onChange={e => setForm({ ...form, isActive: e.target.checked })} />Active</label>
                </div>
              </div>
              <div className="flex justify-end gap-2 pt-4 border-t">
                <Button variant="outline" type="button" onClick={() => setShowModal(false)}>Cancel</Button>
                <Button type="submit" disabled={addMut.isPending || updateMut.isPending}>
                  {(addMut.isPending || updateMut.isPending) && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                  {editing ? 'Update' : 'Add'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </Card>
  );
}

// ==================== PRICE HISTORY TAB ====================
function PriceHistoryTab({ productId }: { productId: string }) {
  const [page, setPage] = useState(1);
  const { data, isLoading } = useQuery({
    queryKey: ['product-price-history', productId, page],
    queryFn: () => productPriceHistoryApi.getByProduct(productId, { pageIndex: page - 1, pageSize: 20 }),
  });
  const items: ProductPriceHistory[] = data?.data?.items || data?.data || [];

  return (
    <Card>
      <div className="p-4 border-b"><h3 className="font-semibold">Price History</h3></div>
      {isLoading ? <div className="flex justify-center p-8"><Loader2 className="w-6 h-6 animate-spin" /></div> : (
        <div className="nx-table-wrap">
          <table className="nx-table">
            <thead><tr><th>Date</th><th>Changed By</th><th>Old Cost</th><th>New Cost</th><th>Old Sale</th><th>New Sale</th><th>Reason</th></tr></thead>
            <tbody>
              {items.map(h => (
                <tr key={h.id}>
                  <td className="text-sm">{new Date(h.createdAt).toLocaleDateString()}</td>
                  <td>{h.changedByName || '-'}</td>
                  <td>{formatCurrency(h.oldCostPrice)}</td>
                  <td className="font-medium">{formatCurrency(h.newCostPrice)}</td>
                  <td>{formatCurrency(h.oldSalePrice)}</td>
                  <td className="font-medium">{formatCurrency(h.newSalePrice)}</td>
                  <td className="text-sm text-muted-foreground">{h.reason || '-'}</td>
                </tr>
              ))}
              {items.length === 0 && <tr><td colSpan={7} className="text-center text-muted-foreground py-8">No price history</td></tr>}
            </tbody>
          </table>
        </div>
      )}
    </Card>
  );
}

// ==================== ATTRIBUTES TAB ====================
function AttributesTab({ productId }: { productId: string }) {
  const queryClient = useQueryClient();

  const { data: linksData, isLoading } = useQuery({
    queryKey: ['product-attributes', productId],
    queryFn: () => productAttributeApi.getByProduct(productId),
  });
  const { data: allTypesData } = useQuery({
    queryKey: ['attribute-types-all'],
    queryFn: () => attributeTypeApi.getAll({ pageSize: 100 }),
  });

  const links: ProductAttributeLink[] = linksData?.data?.items || linksData?.data || [];
  const allTypes: AttributeType[] = allTypesData?.data?.items || allTypesData?.data || [];
  const linkedIds = new Set(links.map(l => l.attributeTypeId));

  const manageMut = useMutation({
    mutationFn: (data: { attributeTypeId: string; isRequired: boolean; sortOrder: number }[]) => productAttributeApi.manage(productId, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['product-attributes', productId] }); toast.success('Attributes updated'); },
    onError: () => toast.error('Failed to update attributes'),
  });

  const toggle = (typeId: string) => {
    const current = links.map(l => ({ attributeTypeId: l.attributeTypeId, isRequired: l.isRequired, sortOrder: l.sortOrder }));
    const next = linkedIds.has(typeId) ? current.filter(c => c.attributeTypeId !== typeId) : [...current, { attributeTypeId: typeId, isRequired: false, sortOrder: current.length }];
    manageMut.mutate(next);
  };

  const toggleRequired = (typeId: string) => {
    const next = links.map(l => ({
      attributeTypeId: l.attributeTypeId,
      isRequired: l.attributeTypeId === typeId ? !l.isRequired : l.isRequired,
      sortOrder: l.sortOrder,
    }));
    manageMut.mutate(next);
  };

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-semibold">Linked Attributes ({links.length})</h3>
      </div>
      {isLoading ? <div className="flex justify-center p-4"><Loader2 className="w-6 h-6 animate-spin" /></div> : (
        <>
          {links.length > 0 && (
            <div className="nx-table-wrap mb-4">
              <table className="nx-table">
                <thead><tr><th>Attribute</th><th>UI Type</th><th>Required</th><th>Options</th><th style={{ width: 60 }}></th></tr></thead>
                <tbody>
                  {links.map(l => (
                    <tr key={l.id}>
                      <td className="font-medium">{l.attributeTypeName}</td>
                      <td><span className="nx-badge nx-badge-neutral">{l.uiType}</span></td>
                      <td>
                        <button onClick={() => toggleRequired(l.attributeTypeId)} className={`nx-badge ${l.isRequired ? 'nx-badge-warning' : 'nx-badge-neutral'}`}>
                          {l.isRequired ? 'Required' : 'Optional'}
                        </button>
                      </td>
                      <td><div className="flex gap-1 flex-wrap">{l.options?.map(o => <span key={o.id} className="nx-badge nx-badge-neutral text-xs">{o.value}</span>)}</div></td>
                      <td><Button variant="ghost" size="icon" className="w-7 h-7 text-red-500" onClick={() => toggle(l.attributeTypeId)}><Trash2 className="w-3 h-3" /></Button></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
          <div className="border rounded-lg">
            <div className="p-3 bg-secondary/30 border-b text-sm font-medium">Available Attribute Types</div>
            <div className="max-h-64 overflow-y-auto">
              {allTypes.filter(t => !linkedIds.has(t.id)).map(t => (
                <div key={t.id} className="flex items-center justify-between px-4 py-2 hover:bg-secondary/30 border-b last:border-b-0">
                  <div>
                    <span className="text-sm font-medium">{t.name}</span>
                    <span className="text-xs text-muted-foreground ml-2">({t.uiType})</span>
                  </div>
                  <Button size="sm" variant="outline" onClick={() => toggle(t.id)}><Plus className="w-3 h-3 mr-1" />Add</Button>
                </div>
              ))}
              {allTypes.filter(t => !linkedIds.has(t.id)).length === 0 && <div className="text-center text-muted-foreground py-4">All attributes linked</div>}
            </div>
          </div>
        </>
      )}
    </Card>
  );
}
