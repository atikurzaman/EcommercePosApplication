import apiClient from './client';

export interface Brand {
  id: string;
  brandCode: string;
  brandName: string;
  slug?: string;
  description?: string;
  logoUrl?: string;
  website?: string;
  countryOfOrigin?: string;
  isFeatured: boolean;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export interface BrandWithCount extends Brand {
  productCount: number;
}

type ApiBrand = {
  id: string;
  brandCode: string;
  name: string;
  description?: string;
  logoUrl?: string;
  website?: string;
  countryOfOrigin?: string;
  isFeatured: boolean;
  isActive: boolean;
  productCount?: number;
};

function toBrandModel(item: ApiBrand): Brand {
  return {
    id: item.id,
    brandCode: item.brandCode,
    brandName: item.name,
    description: item.description,
    logoUrl: item.logoUrl,
    website: item.website,
    countryOfOrigin: item.countryOfOrigin,
    isFeatured: item.isFeatured,
    isActive: item.isActive,
  };
}

function toApiBrandPayload(data: Partial<Brand>) {
  return {
    name: data.brandName,
    brandCode: data.brandCode,
    description: data.description,
    logoUrl: data.logoUrl,
    website: data.website,
    countryOfOrigin: data.countryOfOrigin,
    isFeatured: data.isFeatured ?? false,
    isActive: data.isActive ?? true,
  };
}

export const brandApi = {
  getAll: async (filter?: { pageIndex?: number; pageSize?: number; search?: string }) => {
    const res = await apiClient.get<ApiBrand[]>('/brands', {
      params: filter,
    });
    const apiRes = res.data as any;
    const items = apiRes.items || [];
    return {
      ...res,
      data: {
        items: items.map(toBrandModel),
        totalCount: apiRes.totalCount ?? 0,
        pageIndex: apiRes.pageIndex ?? 0,
        pageSize: apiRes.pageSize ?? items.length,
      },
    };
  },

  getWithCount: async () => {
    const res = await apiClient.get<ApiBrand[]>('/brands/with-count');
    const items = res.data?.items || [];
    return {
      ...res,
      data: {
        items: items.map((item) => ({
          ...toBrandModel(item),
          productCount: (item as any).productCount ?? 0,
        })),
      },
    };
  },

  getById: async (id: string) => {
    const res = await apiClient.get<ApiBrand>(`/brands/${id}`);
    return {
      ...res,
      data: toBrandModel(res.data),
    };
  },

  create: async (data: Partial<Brand>) => {
    const res = await apiClient.post<ApiBrand>('/brands', toApiBrandPayload(data));
    return {
      ...res,
      data: toBrandModel(res.data),
    };
  },

  update: async (id: string, data: Partial<Brand>) => {
    const res = await apiClient.put<ApiBrand>(`/brands/${id}`, toApiBrandPayload(data));
    return {
      ...res,
      data: toBrandModel(res.data),
    };
  },

  delete: (id: string) =>
    apiClient.delete(`/brands/${id}`),

  toggle: (id: string) =>
    apiClient.patch<{ data: { id: string; isActive: boolean } }>(`/brands/${id}/toggle`),
};
