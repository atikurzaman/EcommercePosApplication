import { useState, useCallback } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Search, Filter, Package, Edit, Trash2,
  ChevronLeft, ChevronRight, Loader2, X, Upload,
  Image, Link2, Tag, Star, BarChart2, Download,
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { productApi, type Product } from '@/api/productApi';
import { categoryApi } from '@/api/categoryApi';
import { brandApi } from '@/api/brandApi';

/* ─── types ────────────────────────────────────────────────────────────────── */
interface ProductFormData {
  productCode: string;
  productName: string;
  slug: string;
  description: string;
  shortDescription: string;
  sku: string;
  barcode: string;
  categoryId: string;
  brandId: string;
  unitId: string;
  costPrice: number;
  sellPrice: number;
  mrp: number;
  taxRate: number;
  quantity: number;
  reorderLevel: number;
  isTrackInventory: boolean;
  allowBackorder: boolean;
  isActive: boolean;
  isFeatured: boolean;
  metaTitle: string;
  metaDescription: string;
  images: string[];
  tags: string[];
}

const emptyForm: ProductFormData = {
  productCode: '',
  productName: '',
  slug: '',
  description: '',
  shortDescription: '',
  sku: '',
  barcode: '',
  categoryId: '',
  brandId: '',
  unitId: '',
  costPrice: 0,
  sellPrice: 0,
  mrp: 0,
  taxRate: 0,
  quantity: 0,
  reorderLevel: 10,
  isTrackInventory: true,
  allowBackorder: false,
  isActive: true,
  isFeatured: false,
  metaTitle: '',
  metaDescription: '',
  images: [],
  tags: [],
};

type TabId = 'basic' | 'pricing' | 'inventory' | 'media' | 'seo';

const TABS: { id: TabId; label: string; icon: React.ElementType }[] = [
  { id: 'basic',     label: 'Basic Info',  icon: Package  },
  { id: 'pricing',   label: 'Pricing',     icon: Tag      },
  { id: 'inventory', label: 'Inventory',   icon: BarChart2 },
  { id: 'media',     label: 'Media',       icon: Image    },
  { id: 'seo',       label: 'SEO',         icon: Link2    },
];

const PAGE_SIZE = 12;

/* ─── helpers ──────────────────────────────────────────────────────────────── */
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-BD', {
    style: 'currency',
    currency: 'BDT',
    minimumFractionDigits: 0,
  }).format(amount);
}

function toSlug(str: string): string {
  return str.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '');
}

function getStockBadge(qty: number, reorder: number) {
  if (qty === 0)        return { label: 'Out of Stock', cls: 'nx-badge nx-badge-danger' };
  if (qty <= reorder)   return { label: 'Low Stock',    cls: 'nx-badge nx-badge-warning' };
  return                       { label: 'In Stock',     cls: 'nx-badge nx-badge-success' };
}

/* ─── sub-components ───────────────────────────────────────────────────────── */
function ProductThumbnail({ imageUrl, name }: { imageUrl?: string; name: string }) {
  if (imageUrl) {
    return (
      <img
        src={imageUrl}
        alt={name}
        className="w-10 h-10 rounded-lg object-cover bg-secondary border flex-shrink-0"
        onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
      />
    );
  }
  return (
    <div className="w-10 h-10 rounded-lg bg-secondary border flex items-center justify-center flex-shrink-0">
      <Package className="w-4 h-4 text-muted-foreground" />
    </div>
  );
}

function TableSkeleton() {
  return (
    <div className="divide-y divide-border">
      {Array.from({ length: 8 }, (_, i) => (
        <div key={i} className="flex items-center gap-4 px-4 py-3 animate-pulse">
          <div className="w-5 h-5 bg-secondary rounded flex-shrink-0" />
          <div className="w-10 h-10 bg-secondary rounded-lg flex-shrink-0" />
          <div className="flex-1 space-y-1.5">
            <div className="h-3.5 bg-secondary rounded w-2/5" />
            <div className="h-3 bg-secondary rounded w-1/4" />
          </div>
          <div className="h-3 bg-secondary rounded w-20 hidden md:block" />
          <div className="h-3 bg-secondary rounded w-20 hidden lg:block" />
          <div className="h-3 bg-secondary rounded w-16" />
          <div className="h-5 bg-secondary rounded-full w-16" />
          <div className="h-5 bg-secondary rounded-full w-14" />
          <div className="h-7 bg-secondary rounded w-20" />
        </div>
      ))}
    </div>
  );
}

function EmptyState({ onAdd }: { onAdd: () => void }) {
  return (
    <div className="flex flex-col items-center justify-center py-20 text-muted-foreground">
      <div className="w-20 h-20 rounded-2xl bg-secondary flex items-center justify-center mb-4">
        <Package className="w-10 h-10 opacity-40" />
      </div>
      <h3 className="text-base font-semibold text-foreground mb-1">No products found</h3>
      <p className="text-sm mb-6 text-center max-w-xs">
        Get started by adding your first product or try adjusting the search filters.
      </p>
      <Button size="sm" onClick={onAdd}>
        <Plus className="w-4 h-4 mr-2" />
        Add Product
      </Button>
    </div>
  );
}

/* ─── main component ───────────────────────────────────────────────────────── */
export default function Products() {
  const queryClient = useQueryClient();

  // ── filter state
  const [searchQuery, setSearchQuery] = useState('');
  const [searchInput, setSearchInput]   = useState('');
  const [currentPage, setCurrentPage]   = useState(1);
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [selectedBrand, setSelectedBrand]       = useState('all');
  const [selectedStatus, setSelectedStatus]     = useState('all');

  // ── modal state
  const [showModal, setShowModal]       = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [formData, setFormData]         = useState<ProductFormData>(emptyForm);
  const [deleteModal, setDeleteModal]   = useState<Product | null>(null);
  const [activeTab, setActiveTab]       = useState<TabId>('basic');
  const [tagInput, setTagInput]         = useState('');

  /* ── data queries ───────────────────────────────────────────────────────── */
  const { data: productsData, isLoading } = useQuery({
    queryKey: ['products', currentPage, selectedCategory, selectedBrand, selectedStatus, searchQuery],
    queryFn: () => productApi.getAll({
      pageIndex: currentPage - 1,
      pageSize: PAGE_SIZE,
      search: searchQuery || undefined,
      categoryId: selectedCategory !== 'all' ? selectedCategory : undefined,
      brandId: selectedBrand !== 'all' ? selectedBrand : undefined,
      isActive: selectedStatus !== 'all' ? selectedStatus === 'true' : undefined,
    }),
  });

  const { data: categoriesData } = useQuery({
    queryKey: ['categories', 'all'],
    queryFn: () => categoryApi.getAll({ pageSize: 200 }),
    staleTime: 300_000,
  });

  const { data: brandsData } = useQuery({
    queryKey: ['brands', 'all'],
    queryFn: () => brandApi.getAll({ pageSize: 200 }),
    staleTime: 300_000,
  });

  /* ── mutations ──────────────────────────────────────────────────────────── */
  const createMutation = useMutation({
    mutationFn: productApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      setShowModal(false);
      toast.success('Product created successfully');
    },
    onError: () => toast.error('Failed to create product'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<ProductFormData> }) =>
      productApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      setShowModal(false);
      toast.success('Product updated successfully');
    },
    onError: () => toast.error('Failed to update product'),
  });

  const deleteMutation = useMutation({
    mutationFn: productApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      setDeleteModal(null);
      toast.success('Product deleted');
    },
    onError: () => toast.error('Failed to delete product'),
  });

  /* ── derived ────────────────────────────────────────────────────────────── */
  const products    = (productsData?.data as any)?.items     ?? [];
  const totalCount  = (productsData?.data as any)?.totalCount ?? 0;
  const categories  = (categoriesData?.data as any)?.items   ?? [];
  const brands      = (brandsData?.data as any)?.items       ?? [];
  const totalPages  = Math.ceil(totalCount / PAGE_SIZE);

  // build lookup maps for name resolution
  const categoryMap = new Map<string, string>(
    categories.map((c: any) => [c.id, c.categoryName])
  );
  const brandMap = new Map<string, string>(
    brands.map((b: any) => [b.id, b.brandName])
  );

  const activeCount   = products.filter((p: Product) => p.isActive).length;
  const featuredCount = products.filter((p: any) => p.isFeatured).length;
  const outOfStock    = products.filter((p: Product) => (p.quantity ?? 0) === 0).length;

  /* ── handlers ───────────────────────────────────────────────────────────── */
  const applySearch = useCallback(() => {
    setSearchQuery(searchInput);
    setCurrentPage(1);
  }, [searchInput]);

  const openCreateModal = () => {
    setEditingProduct(null);
    setFormData(emptyForm);
    setActiveTab('basic');
    setShowModal(true);
  };

  const openEditModal = (product: Product) => {
    setEditingProduct(product);
    setFormData({
      productCode:       product.productCode ?? '',
      productName:       product.productName ?? '',
      slug:              toSlug(product.productName ?? ''),
      description:       product.description ?? '',
      shortDescription:  '',
      sku:               product.sku ?? '',
      barcode:           product.barcode ?? '',
      categoryId:        product.categoryId ?? '',
      brandId:           product.brandId ?? '',
      unitId:            product.unitId ?? '',
      costPrice:         product.costPrice ?? 0,
      sellPrice:         product.sellPrice ?? 0,
      mrp:               product.mrp ?? 0,
      taxRate:           0,
      quantity:          product.quantity ?? 0,
      reorderLevel:      product.reorderLevel ?? 10,
      isTrackInventory:  true,
      allowBackorder:    false,
      isActive:          product.isActive ?? true,
      isFeatured:        (product as any).isFeatured ?? false,
      metaTitle:         product.productName ?? '',
      metaDescription:   product.description ?? '',
      images:            product.imageUrl ? [product.imageUrl] : [],
      tags:              [],
    });
    setActiveTab('basic');
    setShowModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingProduct) {
      updateMutation.mutate({ id: editingProduct.id, data: formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const addTag = () => {
    const tag = tagInput.trim();
    if (tag && !formData.tags.includes(tag)) {
      setFormData(f => ({ ...f, tags: [...f.tags, tag] }));
      setTagInput('');
    }
  };

  const removeTag = (tag: string) => {
    setFormData(f => ({ ...f, tags: f.tags.filter(t => t !== tag) }));
  };

  /* ── form tab content ───────────────────────────────────────────────────── */
  const renderTabContent = () => {
    switch (activeTab) {
      case 'basic':
        return (
          <div className="grid grid-cols-2 gap-4">
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-1">Product Name <span className="text-red-500">*</span></label>
              <Input
                value={formData.productName}
                onChange={(e) =>
                  setFormData(f => ({
                    ...f,
                    productName: e.target.value,
                    slug: toSlug(e.target.value),
                    metaTitle: f.metaTitle || e.target.value,
                  }))
                }
                placeholder="e.g. Samsung Galaxy S24 Ultra"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Product Code <span className="text-red-500">*</span></label>
              <Input
                value={formData.productCode}
                onChange={(e) => setFormData(f => ({ ...f, productCode: e.target.value }))}
                placeholder="e.g. PRD-001"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Slug</label>
              <Input
                value={formData.slug}
                onChange={(e) => setFormData(f => ({ ...f, slug: e.target.value }))}
                placeholder="auto-generated"
              />
            </div>
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-1">Short Description</label>
              <Input
                value={formData.shortDescription}
                onChange={(e) => setFormData(f => ({ ...f, shortDescription: e.target.value }))}
                placeholder="Brief summary shown in product cards"
              />
            </div>
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-1">Full Description</label>
              <textarea
                className="nx-input w-full h-28 resize-none"
                value={formData.description}
                onChange={(e) => setFormData(f => ({ ...f, description: e.target.value }))}
                placeholder="Detailed product description..."
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Category</label>
              <select
                className="nx-input nx-select w-full"
                value={formData.categoryId}
                onChange={(e) => setFormData(f => ({ ...f, categoryId: e.target.value }))}
              >
                <option value="">Select Category</option>
                {categories.map((cat: any) => (
                  <option key={cat.id} value={cat.id}>{cat.categoryName}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Brand</label>
              <select
                className="nx-input nx-select w-full"
                value={formData.brandId}
                onChange={(e) => setFormData(f => ({ ...f, brandId: e.target.value }))}
              >
                <option value="">Select Brand</option>
                {brands.map((brand: any) => (
                  <option key={brand.id} value={brand.id}>{brand.brandName}</option>
                ))}
              </select>
            </div>
            <div className="col-span-2 flex items-center gap-6 pt-1">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  checked={formData.isActive}
                  onChange={(e) => setFormData(f => ({ ...f, isActive: e.target.checked }))}
                  className="nx-checkbox"
                />
                <span className="text-sm font-medium">Active</span>
              </label>
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  checked={formData.isFeatured}
                  onChange={(e) => setFormData(f => ({ ...f, isFeatured: e.target.checked }))}
                  className="nx-checkbox"
                />
                <span className="text-sm font-medium">Featured</span>
              </label>
            </div>
          </div>
        );

      case 'pricing':
        return (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Cost Price (৳)</label>
                <Input
                  type="number"
                  min={0}
                  step="0.01"
                  value={formData.costPrice}
                  onChange={(e) => setFormData(f => ({ ...f, costPrice: parseFloat(e.target.value) || 0 }))}
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Sell Price (৳) <span className="text-red-500">*</span></label>
                <Input
                  type="number"
                  min={0}
                  step="0.01"
                  value={formData.sellPrice}
                  onChange={(e) => setFormData(f => ({ ...f, sellPrice: parseFloat(e.target.value) || 0 }))}
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">MRP / List Price (৳)</label>
                <Input
                  type="number"
                  min={0}
                  step="0.01"
                  value={formData.mrp}
                  onChange={(e) => setFormData(f => ({ ...f, mrp: parseFloat(e.target.value) || 0 }))}
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Tax Rate (%)</label>
                <Input
                  type="number"
                  min={0}
                  max={100}
                  step="0.01"
                  value={formData.taxRate}
                  onChange={(e) => setFormData(f => ({ ...f, taxRate: parseFloat(e.target.value) || 0 }))}
                />
              </div>
            </div>
            {/* Pricing summary */}
            <div className="rounded-xl border bg-secondary/40 p-4">
              <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-3">Pricing Summary</p>
              <div className="grid grid-cols-3 gap-4 text-center">
                <div>
                  <p className="text-xs text-muted-foreground mb-1">Cost</p>
                  <p className="text-base font-bold">{formatCurrency(formData.costPrice)}</p>
                </div>
                <div>
                  <p className="text-xs text-muted-foreground mb-1">Sell Price</p>
                  <p className="text-base font-bold">{formatCurrency(formData.sellPrice)}</p>
                </div>
                <div>
                  <p className="text-xs text-muted-foreground mb-1">Profit</p>
                  <p className={`text-base font-bold ${formData.sellPrice - formData.costPrice >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                    {formatCurrency(formData.sellPrice - formData.costPrice)}
                  </p>
                </div>
              </div>
              {formData.costPrice > 0 && formData.sellPrice > 0 && (
                <p className="text-center text-xs text-muted-foreground mt-2">
                  Margin: {(((formData.sellPrice - formData.costPrice) / formData.sellPrice) * 100).toFixed(1)}%
                </p>
              )}
            </div>
          </div>
        );

      case 'inventory':
        return (
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">SKU</label>
              <Input
                value={formData.sku}
                onChange={(e) => setFormData(f => ({ ...f, sku: e.target.value }))}
                placeholder="Stock Keeping Unit"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Barcode</label>
              <Input
                value={formData.barcode}
                onChange={(e) => setFormData(f => ({ ...f, barcode: e.target.value }))}
                placeholder="EAN / UPC"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Opening Stock</label>
              <Input
                type="number"
                min={0}
                value={formData.quantity}
                onChange={(e) => setFormData(f => ({ ...f, quantity: parseInt(e.target.value) || 0 }))}
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Reorder Level</label>
              <Input
                type="number"
                min={0}
                value={formData.reorderLevel}
                onChange={(e) => setFormData(f => ({ ...f, reorderLevel: parseInt(e.target.value) || 0 }))}
              />
            </div>
            <div className="col-span-2 space-y-3 pt-1">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  checked={formData.isTrackInventory}
                  onChange={(e) => setFormData(f => ({ ...f, isTrackInventory: e.target.checked }))}
                  className="nx-checkbox"
                />
                <span className="text-sm font-medium">Track Inventory</span>
              </label>
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  checked={formData.allowBackorder}
                  onChange={(e) => setFormData(f => ({ ...f, allowBackorder: e.target.checked }))}
                  className="nx-checkbox"
                />
                <span className="text-sm font-medium">Allow Backorder</span>
              </label>
            </div>
          </div>
        );

      case 'media':
        return (
          <div className="space-y-4">
            <div className="border-2 border-dashed border-border rounded-xl p-10 text-center hover:border-primary/50 hover:bg-secondary/30 cursor-pointer transition-all group">
              <Upload className="w-8 h-8 mx-auto mb-3 text-muted-foreground group-hover:text-primary transition-colors" />
              <p className="text-sm font-medium mb-1">Drop images here or click to upload</p>
              <p className="text-xs text-muted-foreground">Supports JPG, PNG, WebP up to 5 MB</p>
            </div>
            {formData.images.length > 0 && (
              <div className="grid grid-cols-4 gap-3">
                {formData.images.map((img, i) => (
                  <div key={i} className="relative aspect-square rounded-xl overflow-hidden border bg-secondary">
                    <img src={img} alt="" className="w-full h-full object-cover" />
                    <button
                      type="button"
                      className="absolute top-1.5 right-1.5 w-6 h-6 rounded-full bg-red-500 text-white flex items-center justify-center hover:bg-red-600 transition-colors"
                      onClick={() => setFormData(f => ({ ...f, images: f.images.filter((_, idx) => idx !== i) }))}
                    >
                      <X className="w-3 h-3" />
                    </button>
                  </div>
                ))}
              </div>
            )}
          </div>
        );

      case 'seo':
        return (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Meta Title</label>
              <Input
                value={formData.metaTitle}
                onChange={(e) => setFormData(f => ({ ...f, metaTitle: e.target.value }))}
                placeholder={formData.productName || 'SEO page title'}
              />
              <p className={`text-xs mt-1 ${formData.metaTitle.length > 60 ? 'text-red-500' : 'text-muted-foreground'}`}>
                {formData.metaTitle.length}/60 characters
              </p>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Meta Description</label>
              <textarea
                className="nx-input w-full h-24 resize-none"
                value={formData.metaDescription}
                onChange={(e) => setFormData(f => ({ ...f, metaDescription: e.target.value }))}
                placeholder="SEO description for search engines..."
              />
              <p className={`text-xs mt-1 ${formData.metaDescription.length > 160 ? 'text-red-500' : 'text-muted-foreground'}`}>
                {formData.metaDescription.length}/160 characters
              </p>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Tags</label>
              <div className="flex gap-2 mb-2">
                <Input
                  value={tagInput}
                  onChange={(e) => setTagInput(e.target.value)}
                  onKeyDown={(e) => { if (e.key === 'Enter') { e.preventDefault(); addTag(); } }}
                  placeholder="Type a tag and press Enter"
                />
                <Button type="button" variant="outline" size="sm" onClick={addTag}>Add</Button>
              </div>
              {formData.tags.length > 0 && (
                <div className="flex flex-wrap gap-2">
                  {formData.tags.map((tag) => (
                    <span key={tag} className="inline-flex items-center gap-1 nx-badge nx-badge-info">
                      {tag}
                      <button type="button" onClick={() => removeTag(tag)}>
                        <X className="w-3 h-3" />
                      </button>
                    </span>
                  ))}
                </div>
              )}
            </div>
          </div>
        );
    }
  };

  /* ── render ─────────────────────────────────────────────────────────────── */
  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Products</h1>
          <p className="nx-page-subtitle">Manage your product catalog</p>
        </div>
        <div className="nx-page-actions">
          <Button variant="outline" size="sm">
            <Download className="w-4 h-4 mr-2" />
            Export
          </Button>
          <Button size="sm" onClick={openCreateModal}>
            <Plus className="w-4 h-4 mr-2" />
            Add Product
          </Button>
        </div>
      </div>

      {/* Stats Row */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <div className="nx-stat-card">
          <div className="flex items-center justify-between mb-2">
            <p className="text-sm text-muted-foreground font-medium">Total Products</p>
            <Package className="w-4 h-4 text-purple-500" />
          </div>
          <p className="text-2xl font-bold">{totalCount.toLocaleString()}</p>
        </div>
        <div className="nx-stat-card">
          <div className="flex items-center justify-between mb-2">
            <p className="text-sm text-muted-foreground font-medium">Active</p>
            <span className="w-2 h-2 rounded-full bg-green-500" />
          </div>
          <p className="text-2xl font-bold text-green-600">{activeCount}</p>
          <p className="text-xs text-muted-foreground mt-1">on current page</p>
        </div>
        <div className="nx-stat-card">
          <div className="flex items-center justify-between mb-2">
            <p className="text-sm text-muted-foreground font-medium">Featured</p>
            <Star className="w-4 h-4 text-yellow-500" />
          </div>
          <p className="text-2xl font-bold text-yellow-600">{featuredCount}</p>
          <p className="text-xs text-muted-foreground mt-1">on current page</p>
        </div>
        <div className="nx-stat-card">
          <div className="flex items-center justify-between mb-2">
            <p className="text-sm text-muted-foreground font-medium">Out of Stock</p>
            <span className="w-2 h-2 rounded-full bg-red-500" />
          </div>
          <p className="text-2xl font-bold text-red-600">{outOfStock}</p>
          <p className="text-xs text-muted-foreground mt-1">on current page</p>
        </div>
      </div>

      {/* Table Card */}
      <Card>
        {/* Toolbar */}
        <div className="p-4 border-b">
          <div className="flex flex-col sm:flex-row gap-3">
            {/* Search */}
            <div className="nx-table-search flex-1 min-w-0">
              <Search className="w-4 h-4 flex-shrink-0" />
              <input
                type="text"
                placeholder="Search by name, SKU, barcode..."
                value={searchInput}
                onChange={(e) => setSearchInput(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && applySearch()}
                className="bg-transparent border-none outline-none text-sm w-full"
              />
            </div>
            {/* Filters */}
            <div className="flex items-center gap-2 flex-shrink-0">
              <select
                className="nx-input nx-select text-sm h-9 pl-3 pr-7"
                value={selectedCategory}
                onChange={(e) => { setSelectedCategory(e.target.value); setCurrentPage(1); }}
              >
                <option value="all">All Categories</option>
                {categories.map((cat: any) => (
                  <option key={cat.id} value={cat.id}>{cat.categoryName}</option>
                ))}
              </select>
              <select
                className="nx-input nx-select text-sm h-9 pl-3 pr-7"
                value={selectedBrand}
                onChange={(e) => { setSelectedBrand(e.target.value); setCurrentPage(1); }}
              >
                <option value="all">All Brands</option>
                {brands.map((b: any) => (
                  <option key={b.id} value={b.id}>{b.brandName}</option>
                ))}
              </select>
              <select
                className="nx-input nx-select text-sm h-9 pl-3 pr-7"
                value={selectedStatus}
                onChange={(e) => { setSelectedStatus(e.target.value); setCurrentPage(1); }}
              >
                <option value="all">All Status</option>
                <option value="true">Active</option>
                <option value="false">Inactive</option>
              </select>
              <Button variant="outline" size="sm" onClick={applySearch}>
                <Filter className="w-4 h-4 mr-1.5" />
                Search
              </Button>
            </div>
          </div>
        </div>

        {/* Table */}
        {isLoading ? (
          <TableSkeleton />
        ) : products.length === 0 ? (
          <EmptyState onAdd={openCreateModal} />
        ) : (
          <>
            <div className="nx-table-wrap overflow-x-auto">
              <table className="nx-table">
                <thead>
                  <tr>
                    <th className="w-10">
                      <input type="checkbox" className="nx-checkbox" />
                    </th>
                    <th>Product</th>
                    <th>Category</th>
                    <th>Brand</th>
                    <th className="text-right">Price</th>
                    <th className="text-right">Cost</th>
                    <th className="text-center">Stock</th>
                    <th>Status</th>
                    <th className="w-28">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {products.map((product: Product & { isFeatured?: boolean }) => {
                    const stockBadge = getStockBadge(product.quantity ?? 0, product.reorderLevel ?? 10);
                    return (
                      <tr key={product.id}>
                        <td>
                          <input type="checkbox" className="nx-checkbox" onClick={(e) => e.stopPropagation()} />
                        </td>
                        <td>
                          <div className="flex items-center gap-3">
                            <ProductThumbnail imageUrl={product.imageUrl} name={product.productName} />
                            <div className="min-w-0">
                              <div className="flex items-center gap-1.5">
                                <p className="font-medium text-sm truncate max-w-[180px]">{product.productName}</p>
                                {product.isFeatured && (
                                  <Star className="w-3.5 h-3.5 text-yellow-500 flex-shrink-0" fill="currentColor" />
                                )}
                              </div>
                              <code className="text-xs text-muted-foreground bg-secondary px-1.5 py-0.5 rounded">
                                {product.sku || product.productCode}
                              </code>
                            </div>
                          </div>
                        </td>
                        <td>
                          <span className="text-sm text-muted-foreground">
                            {product.categoryId ? (categoryMap.get(product.categoryId) ?? '—') : '—'}
                          </span>
                        </td>
                        <td>
                          <span className="text-sm text-muted-foreground">
                            {product.brandId ? (brandMap.get(product.brandId) ?? '—') : '—'}
                          </span>
                        </td>
                        <td className="text-right">
                          <span className="font-semibold text-sm tabular-nums">
                            {formatCurrency(product.sellPrice ?? 0)}
                          </span>
                        </td>
                        <td className="text-right">
                          <span className="text-sm text-muted-foreground tabular-nums">
                            {formatCurrency(product.costPrice ?? 0)}
                          </span>
                        </td>
                        <td className="text-center">
                          <div className="flex flex-col items-center gap-1">
                            <span className={`font-semibold text-sm tabular-nums ${
                              (product.quantity ?? 0) === 0 ? 'text-red-600' :
                              (product.quantity ?? 0) <= (product.reorderLevel ?? 10) ? 'text-orange-600' :
                              'text-foreground'
                            }`}>
                              {product.quantity ?? 0}
                            </span>
                            <span className={stockBadge.cls}>{stockBadge.label}</span>
                          </div>
                        </td>
                        <td>
                          <span className={`nx-badge ${product.isActive ? 'nx-badge-success' : 'nx-badge-danger'}`}>
                            {product.isActive ? 'Active' : 'Inactive'}
                          </span>
                        </td>
                        <td>
                          <div className="flex items-center gap-1">
                            <Button
                              variant="ghost"
                              size="icon"
                              className="w-8 h-8 text-muted-foreground hover:text-foreground"
                              onClick={() => openEditModal(product)}
                              title="Edit"
                            >
                              <Edit className="w-3.5 h-3.5" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="w-8 h-8 text-red-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20"
                              onClick={() => setDeleteModal(product)}
                              title="Delete"
                            >
                              <Trash2 className="w-3.5 h-3.5" />
                            </Button>
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            <div className="flex items-center justify-between px-4 py-3 border-t">
              <p className="text-sm text-muted-foreground">
                Showing <span className="font-medium">{(currentPage - 1) * PAGE_SIZE + 1}</span>–
                <span className="font-medium">{Math.min(currentPage * PAGE_SIZE, totalCount)}</span> of{' '}
                <span className="font-medium">{totalCount}</span> products
              </p>
              <div className="flex items-center gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={currentPage === 1}
                  onClick={() => setCurrentPage(p => p - 1)}
                >
                  <ChevronLeft className="w-4 h-4" />
                </Button>
                <span className="text-sm font-medium tabular-nums px-2">
                  {currentPage} / {totalPages || 1}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={currentPage >= totalPages}
                  onClick={() => setCurrentPage(p => p + 1)}
                >
                  <ChevronRight className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </>
        )}
      </Card>

      {/* ── Create / Edit Modal ──────────────────────────────────────────── */}
      {showModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl w-full max-w-3xl max-h-[92vh] overflow-hidden flex flex-col shadow-2xl">
            {/* Modal Header */}
            <div className="flex items-center justify-between px-6 py-4 border-b">
              <div>
                <h2 className="text-lg font-semibold">
                  {editingProduct ? 'Edit Product' : 'Add New Product'}
                </h2>
                <p className="text-xs text-muted-foreground mt-0.5">
                  {editingProduct
                    ? `Editing: ${editingProduct.productName}`
                    : 'Fill in the product details below'}
                </p>
              </div>
              <Button variant="ghost" size="icon" onClick={() => setShowModal(false)}>
                <X className="w-4 h-4" />
              </Button>
            </div>

            {/* Tabs */}
            <div className="flex border-b overflow-x-auto">
              {TABS.map((tab) => {
                const Icon = tab.icon;
                return (
                  <button
                    key={tab.id}
                    type="button"
                    onClick={() => setActiveTab(tab.id)}
                    className={`flex items-center gap-2 px-4 py-3 text-sm font-medium whitespace-nowrap border-b-2 transition-colors ${
                      activeTab === tab.id
                        ? 'border-primary text-primary'
                        : 'border-transparent text-muted-foreground hover:text-foreground hover:border-border'
                    }`}
                  >
                    <Icon className="w-4 h-4" />
                    {tab.label}
                  </button>
                );
              })}
            </div>

            {/* Form content */}
            <form id="product-form" onSubmit={handleSubmit} className="flex-1 overflow-y-auto p-6">
              {renderTabContent()}
            </form>

            {/* Modal Footer */}
            <div className="flex items-center justify-end gap-3 px-6 py-4 border-t bg-secondary/20">
              <Button variant="outline" type="button" onClick={() => setShowModal(false)}>
                Cancel
              </Button>
              <Button
                type="submit"
                form="product-form"
                disabled={createMutation.isPending || updateMutation.isPending}
              >
                {(createMutation.isPending || updateMutation.isPending) && (
                  <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                )}
                {editingProduct ? 'Update Product' : 'Create Product'}
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* ── Delete Confirmation Modal ──────────────────────────────────── */}
      {deleteModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl w-full max-w-md p-6 shadow-2xl">
            <div className="flex items-start gap-4 mb-5">
              <div className="w-10 h-10 rounded-full bg-red-100 dark:bg-red-900/30 flex items-center justify-center flex-shrink-0 mt-0.5">
                <Trash2 className="w-5 h-5 text-red-600" />
              </div>
              <div>
                <h2 className="text-base font-semibold mb-1">Delete Product</h2>
                <p className="text-sm text-muted-foreground">
                  Are you sure you want to delete{' '}
                  <span className="font-medium text-foreground">"{deleteModal.productName}"</span>?
                  This action cannot be undone.
                </p>
              </div>
            </div>
            <div className="flex justify-end gap-3">
              <Button variant="outline" onClick={() => setDeleteModal(null)}>Cancel</Button>
              <Button
                variant="destructive"
                onClick={() => deleteMutation.mutate(deleteModal.id)}
                disabled={deleteMutation.isPending}
              >
                {deleteMutation.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                Delete Product
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
