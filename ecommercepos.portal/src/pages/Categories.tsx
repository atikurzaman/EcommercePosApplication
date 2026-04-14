import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Plus, Edit, Trash2, Folder, FolderOpen,
  ChevronRight, ChevronDown, Loader2, X,
  Upload, Search, LayoutList, GitBranch,
  Star, RefreshCw,
} from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { categoryApi, type Category, type CategoryTreeItem } from '@/api/categoryApi';

/* ─── types ────────────────────────────────────────────────────────────────── */
interface CategoryFormData {
  categoryCode: string;
  categoryName: string;
  slug: string;
  description: string;
  imageUrl: string;
  parentCategoryId: string;
  displayOrder: number;
  isFeatured: boolean;
  isActive: boolean;
  metaTitle: string;
  metaDescription: string;
}

const emptyForm: CategoryFormData = {
  categoryCode:     '',
  categoryName:     '',
  slug:             '',
  description:      '',
  imageUrl:         '',
  parentCategoryId: '',
  displayOrder:     0,
  isFeatured:       false,
  isActive:         true,
  metaTitle:        '',
  metaDescription:  '',
};

function getCategoryName(cat: Category): string {
  return cat.categoryName || cat.name || '';
}

/* ─── helpers ──────────────────────────────────────────────────────────────── */
function toSlug(name: string): string {
  return name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '');
}

function buildBreadcrumb(
  id: string,
  treeItems: CategoryTreeItem[],
  flatMap: Map<string, string>,
): string[] {
  // Walk the flat map of parentCategoryId relationships
  const crumbs: string[] = [];
  let current: string | undefined = id;
  const visited = new Set<string>();
  while (current && !visited.has(current)) {
    visited.add(current);
    crumbs.unshift(flatMap.get(current) ?? '?');
    // find parent — we need the full flat list for this
    current = undefined; // break — breadcrumb is just the name trail from tree
  }
  return crumbs;
}

/* ─── tree node component ──────────────────────────────────────────────────── */
interface TreeNodeProps {
  item: CategoryTreeItem;
  level: number;
  selectedId: string | null;
  expandedIds: Set<string>;
  onToggleExpand: (id: string) => void;
  onSelect: (item: CategoryTreeItem) => void;
}

function TreeNode({ item, level, selectedId, expandedIds, onToggleExpand, onSelect }: TreeNodeProps) {
  const hasChildren = (item.children?.length ?? 0) > 0;
  const isExpanded  = expandedIds.has(item.id);
  const isSelected  = selectedId === item.id;

  return (
    <div>
      <div
        className={`flex items-center gap-1.5 py-2 pr-3 rounded-lg cursor-pointer group transition-colors ${
          isSelected
            ? 'bg-primary/10 text-primary'
            : 'hover:bg-secondary/60 text-foreground'
        }`}
        style={{ paddingLeft: `${level * 20 + 10}px` }}
        onClick={() => onSelect(item)}
      >
        {/* expand toggle */}
        <button
          type="button"
          className="w-5 h-5 flex items-center justify-center rounded hover:bg-secondary flex-shrink-0"
          onClick={(e) => { e.stopPropagation(); if (hasChildren) onToggleExpand(item.id); }}
        >
          {hasChildren ? (
            isExpanded
              ? <ChevronDown className="w-3.5 h-3.5 text-muted-foreground" />
              : <ChevronRight className="w-3.5 h-3.5 text-muted-foreground" />
          ) : (
            <span className="w-3.5" />
          )}
        </button>

        {/* icon */}
        <div className={`w-7 h-7 rounded-lg flex items-center justify-center flex-shrink-0 ${
          isSelected
            ? 'bg-primary/20'
            : item.imageUrl
            ? ''
            : 'bg-secondary'
        }`}>
          {item.imageUrl ? (
            <img src={item.imageUrl} alt="" className="w-7 h-7 object-cover rounded-lg" />
          ) : isExpanded ? (
            <FolderOpen className={`w-3.5 h-3.5 ${isSelected ? 'text-primary' : 'text-muted-foreground'}`} />
          ) : (
            <Folder className={`w-3.5 h-3.5 ${isSelected ? 'text-primary' : 'text-muted-foreground'}`} />
          )}
        </div>

        {/* label */}
        <span className={`flex-1 text-sm truncate ${isSelected ? 'font-semibold' : 'font-medium'}`}>
          {item.name}
        </span>

        {/* badges */}
        <div className="flex items-center gap-1 flex-shrink-0">
          {!item.isActive && (
            <span className="nx-badge nx-badge-neutral text-xs">off</span>
          )}
          {hasChildren && (
            <span className="text-xs text-muted-foreground tabular-nums">
              {item.children!.length}
            </span>
          )}
        </div>
      </div>

      {/* children */}
      {hasChildren && isExpanded && (
        <div>
          {item.children!.map(child => (
            <TreeNode
              key={child.id}
              item={child}
              level={level + 1}
              selectedId={selectedId}
              expandedIds={expandedIds}
              onToggleExpand={onToggleExpand}
              onSelect={onSelect}
            />
          ))}
        </div>
      )}
    </div>
  );
}

/* ─── breadcrumb component ─────────────────────────────────────────────────── */
function CategoryBreadcrumb({
  categoryId,
  treeItems,
}: {
  categoryId: string;
  treeItems: CategoryTreeItem[];
}) {
  // Build breadcrumb by traversing tree
  const crumbs: string[] = [];
  function findPath(items: CategoryTreeItem[], targetId: string): boolean {
    for (const item of items) {
      if (item.id === targetId) {
        crumbs.push(item.name);
        return true;
      }
      if (item.children?.length) {
        if (findPath(item.children, targetId)) {
          crumbs.unshift(item.name);
          return true;
        }
      }
    }
    return false;
  }
  findPath(treeItems, categoryId);

  if (crumbs.length <= 1) return null;
  return (
    <div className="flex items-center gap-1 text-xs text-muted-foreground mb-1">
      {crumbs.map((crumb, i) => (
        <span key={i} className="flex items-center gap-1">
          {i > 0 && <ChevronRight className="w-3 h-3" />}
          <span className={i === crumbs.length - 1 ? 'text-foreground font-medium' : ''}>
            {crumb}
          </span>
        </span>
      ))}
    </div>
  );
}

/* ─── main component ───────────────────────────────────────────────────────── */
export default function Categories() {
  const queryClient = useQueryClient();

  // ── view state
  const [viewMode, setViewMode]           = useState<'tree' | 'list'>('tree');
  const [expandedIds, setExpandedIds]     = useState<Set<string>>(new Set());
  const [selectedTreeItem, setSelectedTreeItem] = useState<CategoryTreeItem | null>(null);
  const [treeSearch, setTreeSearch]       = useState('');

  // ── form/panel state
  const [panelMode, setPanelMode]         = useState<'empty' | 'create' | 'edit'>('empty');
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const [formData, setFormData]           = useState<CategoryFormData>(emptyForm);
  const [activeFormTab, setActiveFormTab] = useState<'general' | 'seo'>('general');

  // ── list view state
  const [searchQuery, setSearchQuery]     = useState('');
  const [searchInput, setSearchInput]     = useState('');
  const [currentPage, setCurrentPage]     = useState(1);
  const [statusFilter, setStatusFilter]   = useState('all');
  const [deleteModal, setDeleteModal]     = useState<Category | null>(null);

  /* ── queries ──────────────────────────────────────────────────────────── */
  const { data: treeData, isLoading: treeLoading } = useQuery({
    queryKey: ['category-tree'],
    queryFn: categoryApi.getTree,
    staleTime: 120_000,
  });

  const { data: flatData } = useQuery({
    queryKey: ['category-flat'],
    queryFn: categoryApi.getFlat,
    staleTime: 120_000,
  });

  const { data: listData, isLoading: listLoading } = useQuery({
    queryKey: ['categories', 'list', currentPage, statusFilter, searchQuery],
    queryFn: () => categoryApi.getAll({
      pageIndex: currentPage - 1,
      pageSize: 15,
      search: searchQuery || undefined,
      isActive: statusFilter !== 'all' ? statusFilter === 'true' : undefined,
    }),
    enabled: viewMode === 'list',
  });

  /* ── mutations ──────────────────────────────────────────────────────── */
  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ['categories'] });
    queryClient.invalidateQueries({ queryKey: ['category-tree'] });
    queryClient.invalidateQueries({ queryKey: ['category-flat'] });
  };

  const createMutation = useMutation({
    mutationFn: categoryApi.create,
    onSuccess: () => {
      invalidate();
      setPanelMode('empty');
      toast.success('Category created');
    },
    onError: () => toast.error('Failed to create category'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<Category> }) =>
      categoryApi.update(id, data),
    onSuccess: () => {
      invalidate();
      toast.success('Category updated');
    },
    onError: () => toast.error('Failed to update category'),
  });

  const deleteMutation = useMutation({
    mutationFn: categoryApi.delete,
    onSuccess: () => {
      invalidate();
      setDeleteModal(null);
      if (panelMode === 'edit') setPanelMode('empty');
      toast.success('Category deleted');
    },
    onError: () => toast.error('Failed to delete category'),
  });

  const toggleMutation = useMutation({
    mutationFn: categoryApi.toggle,
    onSuccess: () => { invalidate(); },
  });

  /* ── derived ──────────────────────────────────────────────────────────── */
  const treeItems  = (treeData?.data as any)?.items    ?? [];
  const flatItems  = (flatData?.data as any)?.items    ?? [];
  const listItems  = (listData?.data as any)?.items    ?? [];
  const totalCount = (listData?.data as any)?.totalCount ?? 0;
  const totalPages = Math.ceil(totalCount / 15);

  // filtered tree search
  const filteredTree = treeSearch
    ? treeItems.filter((item: CategoryTreeItem) =>
        item.name.toLowerCase().includes(treeSearch.toLowerCase())
      )
    : treeItems;

  const activeCount = listItems.filter((c: Category) => c.isActive).length;

  /* ── tree stats (count nodes recursively) ─────────────────────────────── */
  function countNodes(items: CategoryTreeItem[]): number {
    return items.reduce((acc, item) => acc + 1 + countNodes(item.children ?? []), 0);
  }
  const totalTreeCount = countNodes(treeItems);

  /* ── handlers ────────────────────────────────────────────────────────── */
  const toggleExpand = (id: string) => {
    setExpandedIds(prev => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const expandAll = () => {
    const allIds = new Set<string>();
    function collect(items: CategoryTreeItem[]) {
      items.forEach(item => { allIds.add(item.id); collect(item.children ?? []); });
    }
    collect(treeItems);
    setExpandedIds(allIds);
  };

  const collapseAll = () => setExpandedIds(new Set());

  const handleSelectTreeItem = (item: CategoryTreeItem) => {
    setSelectedTreeItem(item);
    // load full category for editing
    categoryApi.getById(item.id).then(res => {
      const cat = (res.data as any)?.data ?? res.data;
      if (cat) {
        setEditingCategory(cat);
        setFormData({
          categoryCode:     cat.categoryCode ?? '',
          categoryName:     getCategoryName(cat),
          slug:             cat.slug ?? '',
          description:      cat.description ?? '',
          imageUrl:         cat.imageUrl ?? '',
          parentCategoryId: cat.parentCategoryId ?? '',
          displayOrder:     cat.displayOrder ?? 0,
          isFeatured:       cat.isFeatured ?? false,
          isActive:         cat.isActive ?? true,
          metaTitle:        cat.metaTitle ?? '',
          metaDescription:  cat.metaDescription ?? '',
        });
        setPanelMode('edit');
        setActiveFormTab('general');
      }
    }).catch(() => toast.error('Could not load category details'));
  };

  const openCreatePanel = (parentId?: string) => {
    setEditingCategory(null);
    setSelectedTreeItem(null);
    setFormData({ ...emptyForm, parentCategoryId: parentId ?? '' });
    setPanelMode('create');
    setActiveFormTab('general');
  };

  const openEditModal = (category: Category) => {
    setEditingCategory(category);
    setFormData({
      categoryCode:     category.categoryCode ?? '',
      categoryName:     category.categoryName ?? '',
      slug:             category.slug ?? '',
      description:      category.description ?? '',
      imageUrl:         category.imageUrl ?? '',
      parentCategoryId: category.parentCategoryId ?? '',
      displayOrder:     category.displayOrder ?? 0,
      isFeatured:       category.isFeatured ?? false,
      isActive:         category.isActive ?? true,
      metaTitle:        category.metaTitle ?? '',
      metaDescription:  category.metaDescription ?? '',
    });
    setPanelMode('edit');
    setActiveFormTab('general');
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const payload = { ...formData, parentCategoryId: formData.parentCategoryId || undefined };
    if (panelMode === 'edit' && editingCategory) {
      updateMutation.mutate({ id: editingCategory.id, data: payload });
    } else {
      createMutation.mutate(payload);
    }
  };

  /* ── form panel ────────────────────────────────────────────────────────── */
  const FormPanel = () => (
    <div className="flex flex-col h-full">
      {/* Panel header */}
      <div className="flex items-center justify-between px-5 py-4 border-b">
        <div>
          <h3 className="font-semibold text-base">
            {panelMode === 'create' ? 'New Category' : 'Edit Category'}
          </h3>
          {panelMode === 'edit' && editingCategory && (
            <CategoryBreadcrumb categoryId={editingCategory.id} treeItems={treeItems} />
          )}
        </div>
        <div className="flex items-center gap-2">
          {panelMode === 'edit' && editingCategory && (
            <Button
              variant="ghost"
              size="icon"
              className="w-8 h-8 text-red-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20"
              onClick={() => setDeleteModal(editingCategory)}
              title="Delete category"
            >
              <Trash2 className="w-4 h-4" />
            </Button>
          )}
          <Button
            variant="ghost"
            size="icon"
            className="w-8 h-8"
            onClick={() => setPanelMode('empty')}
          >
            <X className="w-4 h-4" />
          </Button>
        </div>
      </div>

      {/* Form tabs */}
      <div className="flex border-b px-5">
        {(['general', 'seo'] as const).map(tab => (
          <button
            key={tab}
            type="button"
            onClick={() => setActiveFormTab(tab)}
            className={`py-3 px-1 mr-5 text-sm font-medium border-b-2 transition-colors capitalize ${
              activeFormTab === tab
                ? 'border-primary text-primary'
                : 'border-transparent text-muted-foreground hover:text-foreground'
            }`}
          >
            {tab}
          </button>
        ))}
      </div>

      {/* Form body */}
      <form id="category-form" onSubmit={handleSubmit} className="flex-1 overflow-y-auto p-5">
        {activeFormTab === 'general' ? (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1.5">
                  Code <span className="text-red-500">*</span>
                </label>
                <Input
                  value={formData.categoryCode}
                  onChange={(e) => setFormData(f => ({ ...f, categoryCode: e.target.value }))}
                  placeholder="e.g. CAT-01"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1.5">Display Order</label>
                <Input
                  type="number"
                  min={0}
                  value={formData.displayOrder}
                  onChange={(e) => setFormData(f => ({ ...f, displayOrder: parseInt(e.target.value) || 0 }))}
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1.5">
                Category Name <span className="text-red-500">*</span>
              </label>
              <Input
                value={formData.categoryName}
                onChange={(e) => {
                  const name = e.target.value;
                  setFormData(f => ({
                    ...f,
                    categoryName: name,
                    slug: f.slug || toSlug(name),
                    metaTitle: f.metaTitle || name,
                  }));
                }}
                placeholder="e.g. Electronics"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1.5">Parent Category</label>
              <select
                className="nx-input nx-select w-full"
                value={formData.parentCategoryId}
                onChange={(e) => setFormData(f => ({ ...f, parentCategoryId: e.target.value }))}
              >
                <option value="">No Parent (Root Category)</option>
                {flatItems
                  .filter((c: any) => c.id !== editingCategory?.id)
                  .map((cat: any) => (
                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                  ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1.5">Description</label>
              <textarea
                className="nx-input w-full h-20 resize-none"
                value={formData.description}
                onChange={(e) => setFormData(f => ({ ...f, description: e.target.value }))}
                placeholder="Brief description of this category..."
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1.5">Image URL</label>
              <div className="flex gap-2">
                <Input
                  value={formData.imageUrl}
                  onChange={(e) => setFormData(f => ({ ...f, imageUrl: e.target.value }))}
                  placeholder="https://example.com/image.jpg"
                />
                <Button type="button" variant="outline" size="icon" className="flex-shrink-0">
                  <Upload className="w-4 h-4" />
                </Button>
              </div>
              {formData.imageUrl && (
                <div className="mt-2 flex items-center gap-3">
                  <img
                    src={formData.imageUrl}
                    alt="Preview"
                    className="w-14 h-14 object-cover rounded-xl border"
                    onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
                  />
                  <button
                    type="button"
                    className="text-xs text-red-500 hover:underline"
                    onClick={() => setFormData(f => ({ ...f, imageUrl: '' }))}
                  >
                    Remove image
                  </button>
                </div>
              )}
            </div>

            {/* Toggles */}
            <div className="flex items-center gap-6 pt-1">
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
        ) : (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1.5">Slug</label>
              <Input
                value={formData.slug}
                onChange={(e) => setFormData(f => ({ ...f, slug: e.target.value }))}
                placeholder="auto-generated-from-name"
              />
              <p className="text-xs text-muted-foreground mt-1">
                Used in URLs: /categories/{formData.slug || 'your-slug'}
              </p>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1.5">Meta Title</label>
              <Input
                value={formData.metaTitle}
                onChange={(e) => setFormData(f => ({ ...f, metaTitle: e.target.value }))}
                placeholder={formData.categoryName || 'SEO page title'}
              />
              <p className={`text-xs mt-1 ${formData.metaTitle.length > 60 ? 'text-red-500' : 'text-muted-foreground'}`}>
                {formData.metaTitle.length}/60 characters
              </p>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1.5">Meta Description</label>
              <textarea
                className="nx-input w-full h-24 resize-none"
                value={formData.metaDescription}
                onChange={(e) => setFormData(f => ({ ...f, metaDescription: e.target.value }))}
                placeholder="Description for search engines..."
              />
              <p className={`text-xs mt-1 ${formData.metaDescription.length > 160 ? 'text-red-500' : 'text-muted-foreground'}`}>
                {formData.metaDescription.length}/160 characters
              </p>
            </div>
          </div>
        )}
      </form>

      {/* Form footer */}
      <div className="flex items-center justify-between px-5 py-4 border-t bg-secondary/20 gap-3">
        <div className="text-xs text-muted-foreground">
          {panelMode === 'edit' && editingCategory && (
            <span>ID: <code className="font-mono">{editingCategory.id.slice(0, 8)}...</code></span>
          )}
        </div>
        <div className="flex items-center gap-3">
          <Button type="button" variant="outline" size="sm" onClick={() => setPanelMode('empty')}>
            Cancel
          </Button>
          <Button
            type="submit"
            form="category-form"
            size="sm"
            disabled={createMutation.isPending || updateMutation.isPending}
          >
            {(createMutation.isPending || updateMutation.isPending) && (
              <Loader2 className="w-4 h-4 mr-2 animate-spin" />
            )}
            {panelMode === 'edit' ? 'Save Changes' : 'Create Category'}
          </Button>
        </div>
      </div>
    </div>
  );

  /* ── render ─────────────────────────────────────────────────────────────── */
  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">Categories</h1>
          <p className="nx-page-subtitle">Organise your product catalog with categories</p>
        </div>
        <div className="nx-page-actions">
          <div className="flex items-center gap-1 bg-secondary rounded-lg p-1">
            <button
              type="button"
              onClick={() => setViewMode('tree')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-md text-sm font-medium transition-colors ${
                viewMode === 'tree'
                  ? 'bg-background shadow-sm text-foreground'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              <GitBranch className="w-3.5 h-3.5" />
              Tree
            </button>
            <button
              type="button"
              onClick={() => setViewMode('list')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-md text-sm font-medium transition-colors ${
                viewMode === 'list'
                  ? 'bg-background shadow-sm text-foreground'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              <LayoutList className="w-3.5 h-3.5" />
              List
            </button>
          </div>
          <Button size="sm" onClick={() => openCreatePanel()}>
            <Plus className="w-4 h-4 mr-2" />
            Add Category
          </Button>
        </div>
      </div>

      {/* Stats row */}
      <div className="grid grid-cols-3 gap-4">
        <div className="nx-stat-card">
          <div className="flex items-center justify-between mb-2">
            <p className="text-sm font-medium text-muted-foreground">Total Categories</p>
            <Folder className="w-4 h-4 text-primary/60" />
          </div>
          <p className="text-2xl font-bold">{totalTreeCount}</p>
        </div>
        <div className="nx-stat-card">
          <div className="flex items-center justify-between mb-2">
            <p className="text-sm font-medium text-muted-foreground">Root Categories</p>
            <GitBranch className="w-4 h-4 text-primary/60" />
          </div>
          <p className="text-2xl font-bold">{treeItems.length}</p>
        </div>
        <div className="nx-stat-card">
          <div className="flex items-center justify-between mb-2">
            <p className="text-sm font-medium text-muted-foreground">Sub-categories</p>
            <ChevronRight className="w-4 h-4 text-primary/60" />
          </div>
          <p className="text-2xl font-bold">{Math.max(0, totalTreeCount - treeItems.length)}</p>
        </div>
      </div>

      {/* ── Tree View ─────────────────────────────────────────────────────── */}
      {viewMode === 'tree' && (
        <div className="grid grid-cols-1 lg:grid-cols-5 gap-6" style={{ minHeight: '600px' }}>
          {/* Left: Tree panel */}
          <div className="lg:col-span-2">
            <Card className="h-full flex flex-col">
              {/* Tree toolbar */}
              <div className="p-3 border-b space-y-2">
                <div className="nx-table-search">
                  <Search className="w-4 h-4 flex-shrink-0" />
                  <input
                    type="text"
                    placeholder="Search categories..."
                    value={treeSearch}
                    onChange={(e) => setTreeSearch(e.target.value)}
                    className="bg-transparent border-none outline-none text-sm w-full"
                  />
                </div>
                <div className="flex items-center gap-2 text-xs">
                  <button
                    type="button"
                    onClick={expandAll}
                    className="text-primary hover:underline"
                  >
                    Expand All
                  </button>
                  <span className="text-border">·</span>
                  <button
                    type="button"
                    onClick={collapseAll}
                    className="text-primary hover:underline"
                  >
                    Collapse All
                  </button>
                  <span className="text-border ml-auto">·</span>
                  <button
                    type="button"
                    onClick={() => openCreatePanel()}
                    className="text-primary hover:underline"
                  >
                    + New
                  </button>
                </div>
              </div>

              {/* Tree content */}
              <div className="flex-1 overflow-y-auto p-2">
                {treeLoading ? (
                  <div className="space-y-2 p-2">
                    {[1, 2, 3, 4, 5].map(i => (
                      <div key={i} className="animate-pulse flex items-center gap-2 p-2">
                        <div className="w-5 h-5 bg-secondary rounded" />
                        <div className="w-7 h-7 bg-secondary rounded-lg" />
                        <div className="h-3.5 bg-secondary rounded flex-1" />
                      </div>
                    ))}
                  </div>
                ) : filteredTree.length === 0 ? (
                  <div className="flex flex-col items-center justify-center py-12 text-muted-foreground">
                    <Folder className="w-10 h-10 mb-3 opacity-30" />
                    <p className="text-sm">No categories found</p>
                    <Button
                      size="sm"
                      variant="outline"
                      className="mt-3"
                      onClick={() => openCreatePanel()}
                    >
                      <Plus className="w-4 h-4 mr-1.5" />
                      Create first category
                    </Button>
                  </div>
                ) : (
                  filteredTree.map((item: CategoryTreeItem) => (
                    <TreeNode
                      key={item.id}
                      item={item}
                      level={0}
                      selectedId={selectedTreeItem?.id ?? null}
                      expandedIds={expandedIds}
                      onToggleExpand={toggleExpand}
                      onSelect={handleSelectTreeItem}
                    />
                  ))
                )}
              </div>
            </Card>
          </div>

          {/* Right: Detail / Edit panel */}
          <div className="lg:col-span-3">
            <Card className="h-full flex flex-col">
              {panelMode === 'empty' ? (
                <div className="flex flex-col items-center justify-center flex-1 py-16 text-muted-foreground">
                  <div className="w-16 h-16 rounded-2xl bg-secondary flex items-center justify-center mb-4">
                    <FolderOpen className="w-8 h-8 opacity-40" />
                  </div>
                  <h3 className="text-base font-semibold text-foreground mb-1">
                    Select a Category
                  </h3>
                  <p className="text-sm text-center max-w-xs mb-6">
                    Click any category in the tree to view and edit its details, or create a new one.
                  </p>
                  <Button size="sm" onClick={() => openCreatePanel()}>
                    <Plus className="w-4 h-4 mr-2" />
                    Create Category
                  </Button>
                </div>
              ) : (
                <FormPanel />
              )}
            </Card>
          </div>
        </div>
      )}

      {/* ── List View ─────────────────────────────────────────────────────── */}
      {viewMode === 'list' && (
        <div className="grid grid-cols-1 lg:grid-cols-5 gap-6" style={{ minHeight: '600px' }}>
          {/* List table */}
          <div className={panelMode !== 'empty' ? 'lg:col-span-3' : 'lg:col-span-5'}>
            <Card>
              {/* List toolbar */}
              <div className="p-4 border-b flex flex-col sm:flex-row gap-3">
                <div className="nx-table-search flex-1">
                  <Search className="w-4 h-4" />
                  <input
                    type="text"
                    placeholder="Search categories..."
                    value={searchInput}
                    onChange={(e) => setSearchInput(e.target.value)}
                    onKeyDown={(e) => { if (e.key === 'Enter') { setSearchQuery(searchInput); setCurrentPage(1); } }}
                    className="bg-transparent border-none outline-none text-sm w-full"
                  />
                </div>
                <div className="flex items-center gap-2">
                  <select
                    className="nx-input nx-select text-sm h-9"
                    value={statusFilter}
                    onChange={(e) => { setStatusFilter(e.target.value); setCurrentPage(1); }}
                  >
                    <option value="all">All Status</option>
                    <option value="true">Active</option>
                    <option value="false">Inactive</option>
                  </select>
                </div>
              </div>

              {listLoading ? (
                <div className="space-y-0 divide-y divide-border">
                  {[1,2,3,4,5,6,7].map(i => (
                    <div key={i} className="animate-pulse flex items-center gap-4 px-4 py-3">
                      <div className="w-9 h-9 bg-secondary rounded-xl flex-shrink-0" />
                      <div className="flex-1 space-y-1.5">
                        <div className="h-3.5 bg-secondary rounded w-1/3" />
                        <div className="h-3 bg-secondary rounded w-1/4" />
                      </div>
                      <div className="h-5 bg-secondary rounded-full w-14" />
                      <div className="h-7 bg-secondary rounded w-16" />
                    </div>
                  ))}
                </div>
              ) : listItems.length === 0 ? (
                <div className="flex flex-col items-center justify-center py-12 text-muted-foreground">
                  <Folder className="w-10 h-10 mb-3 opacity-30" />
                  <p className="text-sm">No categories found</p>
                </div>
              ) : (
                <>
                  <div className="nx-table-wrap">
                    <table className="nx-table">
                      <thead>
                        <tr>
                          <th>Category</th>
                          <th>Code</th>
                          <th>Order</th>
                          <th>Status</th>
                          <th className="w-24">Actions</th>
                        </tr>
                      </thead>
                      <tbody>
                        {listItems.map((category: Category) => {
                          const parentName = flatItems.find((c: any) => c.id === category.parentCategoryId)?.name;
                          return (
                            <tr key={category.id} className="hover:bg-secondary/30 transition-colors">
                              <td>
                                <div className="flex items-center gap-3">
                                  <div className="w-9 h-9 rounded-xl bg-secondary flex items-center justify-center flex-shrink-0 overflow-hidden">
                                    {category.imageUrl ? (
                                      <img src={category.imageUrl} alt="" className="w-full h-full object-cover" />
                                    ) : (
                                      <Folder className="w-4 h-4 text-muted-foreground" />
                                    )}
                                  </div>
                                  <div className="min-w-0">
                                    <div className="flex items-center gap-1.5">
                                      <p className="font-medium text-sm">{category.categoryName || category.name}</p>
                                      {category.isFeatured && <Star className="w-3.5 h-3.5 text-yellow-500" fill="currentColor" />}
                                    </div>
                                    {parentName && (
                                      <p className="text-xs text-muted-foreground truncate">
                                        under {parentName}
                                      </p>
                                    )}
                                  </div>
                                </div>
                              </td>
                              <td>
                                <code className="text-xs bg-secondary px-2 py-1 rounded">
                                  {category.categoryCode}
                                </code>
                              </td>
                              <td className="text-sm text-muted-foreground">{category.displayOrder}</td>
                              <td>
                                <button
                                  onClick={() => toggleMutation.mutate(category.id)}
                                  className={`nx-badge cursor-pointer hover:opacity-80 transition-opacity ${
                                    category.isActive ? 'nx-badge-success' : 'nx-badge-danger'
                                  }`}
                                >
                                  {category.isActive ? 'Active' : 'Inactive'}
                                </button>
                              </td>
                              <td>
                                <div className="flex items-center gap-1">
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    className="w-8 h-8"
                                    onClick={() => openEditModal(category)}
                                  >
                                    <Edit className="w-3.5 h-3.5" />
                                  </Button>
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    className="w-8 h-8 text-red-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20"
                                    onClick={() => setDeleteModal(category)}
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
                      Showing {listItems.length} of {totalCount}
                    </p>
                    <div className="flex items-center gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        disabled={currentPage === 1}
                        onClick={() => setCurrentPage(p => p - 1)}
                      >
                        <ChevronRight className="w-4 h-4 rotate-180" />
                      </Button>
                      <span className="text-sm font-medium tabular-nums">
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
          </div>

          {/* Slide-in edit panel */}
          {panelMode !== 'empty' && (
            <div className="lg:col-span-2">
              <Card className="sticky top-6 flex flex-col" style={{ maxHeight: 'calc(100vh - 160px)' }}>
                <FormPanel />
              </Card>
            </div>
          )}
        </div>
      )}

      {/* ── Delete Modal ─────────────────────────────────────────────────── */}
      {deleteModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-background rounded-2xl w-full max-w-md p-6 shadow-2xl">
            <div className="flex items-start gap-4 mb-5">
              <div className="w-10 h-10 rounded-full bg-red-100 dark:bg-red-900/30 flex items-center justify-center flex-shrink-0">
                <Trash2 className="w-5 h-5 text-red-600" />
              </div>
              <div>
                <h2 className="text-base font-semibold mb-1">Delete Category</h2>
                <p className="text-sm text-muted-foreground">
                  Are you sure you want to delete{' '}
                  <span className="font-medium text-foreground">
                    "{deleteModal.categoryName || deleteModal.name}"
                  </span>?
                  Sub-categories may be affected. This cannot be undone.
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
                Delete
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
