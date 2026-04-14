import apiClient from './client';

export interface Product {
  id: string;
  productCode: string;
  productName: string;
  description?: string;
  sku?: string;
  barcode?: string;
  categoryId?: string;
  brandId?: string;
  unitId?: string;
  costPrice: number;
  sellPrice: number;
  mrp: number;
  quantity: number;
  reorderLevel: number;
  isActive: boolean;
  imageUrl?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface ProductFilter {
  pageIndex?: number;
  pageSize?: number;
  search?: string;
  categoryId?: string;
  brandId?: string;
  isActive?: boolean;
}

type ApiProduct = {
  id: string;
  productCode: string;
  name: string;
  shortDescription?: string;
  description?: string;
  sku?: string;
  barcode?: string;
  categoryId?: string;
  brandId?: string;
  unitId?: string;
  costPrice: number;
  salePrice: number;
  originalPrice?: number;
  quantity?: number;
  reorderLevel?: number;
  isFeatured?: boolean;
  isActive: boolean;
};

function toProductModel(item: ApiProduct): Product {
  return {
    id: item.id,
    productCode: item.productCode,
    productName: item.name,
    description: item.description,
    sku: item.sku,
    barcode: item.barcode,
    categoryId: item.categoryId,
    brandId: item.brandId,
    unitId: item.unitId,
    costPrice: item.costPrice,
    sellPrice: item.salePrice,
    mrp: item.originalPrice ?? item.salePrice,
    quantity: item.quantity ?? 0,
    reorderLevel: item.reorderLevel ?? 0,
    isActive: item.isActive,
  };
}

function toApiProductPayload(data: Partial<Product>) {
  return {
    productCode: data.productCode,
    name: data.productName,
    shortDescription: data.description,
    description: data.description,
    productType: 'STANDARD',
    costPrice: data.costPrice ?? 0,
    salePrice: data.sellPrice ?? 0,
    originalPrice: data.mrp ?? data.sellPrice ?? 0,
    isTaxInclusive: false,
    isFeatured: false,
    isActive: data.isActive ?? true,
    categoryId: data.categoryId,
    brandId: data.brandId || null,
    unitId: data.unitId || null,
    sku: data.sku || null,
    barcode: data.barcode || null,
  };
}

export const productApi = {
  getAll: async (filter?: ProductFilter) => {
    const res = await apiClient.get<ApiProduct[]>('/products', {
      params: filter,
    });
    const apiRes = res.data as any;
    const items = apiRes.items || [];
    return {
      ...res,
      data: {
        items: items.map(toProductModel),
        totalCount: apiRes.totalCount ?? 0,
        pageIndex: apiRes.pageIndex ?? 0,
        pageSize: apiRes.pageSize ?? items.length,
      },
    };
  },

  getById: async (id: string) => {
    const res = await apiClient.get<ApiProduct>(`/products/${id}`);
    return {
      ...res,
      data: toProductModel(res.data),
    };
  },

  create: async (data: Partial<Product>) => {
    const res = await apiClient.post<ApiProduct>('/products', toApiProductPayload(data));
    return {
      ...res,
      data: toProductModel(res.data),
    };
  },

  update: async (id: string, data: Partial<Product>) => {
    const res = await apiClient.put<ApiProduct>(`/products/${id}`, toApiProductPayload(data));
    return {
      ...res,
      data: toProductModel(res.data),
    };
  },

  delete: (id: string) =>
    apiClient.delete(`/products/${id}`),
};
