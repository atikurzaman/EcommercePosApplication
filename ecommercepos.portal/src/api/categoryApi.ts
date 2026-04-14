import apiClient from './client';

export interface Category {
  id: string;
  categoryCode?: string;
  categoryName?: string;
  name?: string;
  slug?: string;
  description?: string;
  imageUrl?: string;
  parentCategoryId?: string;
  displayOrder: number;
  isFeatured?: boolean;
  isActive?: boolean;
  metaTitle?: string;
  metaDescription?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface CategoryTreeItem {
  id: string;
  name: string;
  slug?: string;
  parentCategoryId?: string;
  displayOrder: number;
  isActive: boolean;
  imageUrl?: string;
  children?: CategoryTreeItem[];
}

type ApiCategory = {
  id: string;
  name: string;
  slug?: string;
  description?: string;
  imageUrl?: string;
  parentCategoryId?: string;
  displayOrder: number;
  isFeatured?: boolean;
  isActive?: boolean;
  metaTitle?: string;
  metaDescription?: string;
};

function toCategoryModel(item: ApiCategory): Category {
  return {
    id: item.id,
    name: item.name,
    categoryName: item.name,
    slug: item.slug,
    description: item.description,
    imageUrl: item.imageUrl,
    parentCategoryId: item.parentCategoryId,
    displayOrder: item.displayOrder,
    isFeatured: item.isFeatured,
    isActive: item.isActive,
    metaTitle: item.metaTitle,
    metaDescription: item.metaDescription,
  };
}

function toApiCategoryPayload(data: Partial<Category>) {
  const name = data.categoryName ?? data.name ?? '';
  return {
    name,
    slug: data.slug,
    description: data.description,
    imageUrl: data.imageUrl,
    parentCategoryId: data.parentCategoryId || null,
    displayOrder: data.displayOrder ?? 0,
    isFeatured: data.isFeatured ?? false,
    isActive: data.isActive ?? true,
    metaTitle: data.metaTitle,
    metaDescription: data.metaDescription,
  };
}

export const categoryApi = {
  getAll: async (filter?: { pageIndex?: number; pageSize?: number; search?: string; isActive?: boolean }) => {
    const res = await apiClient.get<ApiCategory[]>('/categories', {
      params: filter,
    });
    const apiRes = res.data as any;
    const items = apiRes.items || [];
    return {
      ...res,
      data: {
        items: items.map(toCategoryModel),
        totalCount: apiRes.totalCount ?? 0,
        pageIndex: apiRes.pageIndex ?? 0,
        pageSize: apiRes.pageSize ?? items.length,
      },
    };
  },

  getTree: async () => {
    const res = await apiClient.get<CategoryTreeItem[]>('/categories/tree');
    const items = res.data?.items || [];
    return {
      ...res,
      data: { items: Array.isArray(items) ? items : [] },
    };
  },

  getFlat: async () => {
    const res = await apiClient.get<{ id: string; name: string; parentCategoryId?: string; displayOrder: number }[]>('/categories/flat');
    const items = res.data?.items || [];
    return {
      ...res,
      data: { items: Array.isArray(items) ? items : [] },
    };
  },

  getById: async (id: string) => {
    const res = await apiClient.get<ApiCategory>(`/categories/${id}`);
    return {
      ...res,
      data: toCategoryModel(res.data),
    };
  },

  create: async (data: Partial<Category>) => {
    const res = await apiClient.post<ApiCategory>('/categories', toApiCategoryPayload(data));
    return {
      ...res,
      data: toCategoryModel(res.data),
    };
  },

  update: async (id: string, data: Partial<Category>) => {
    const res = await apiClient.put<ApiCategory>(`/categories/${id}`, toApiCategoryPayload(data));
    return {
      ...res,
      data: toCategoryModel(res.data),
    };
  },

  delete: (id: string) =>
    apiClient.delete(`/categories/${id}`),

  toggle: (id: string) =>
    apiClient.patch<{ data: { id: string; isActive: boolean } }>(`/categories/${id}/toggle`),
};
