import apiClient from './client';

// ========== TYPES ==========

export interface ProductVariant {
  id: string;
  name: string;
  sku?: string;
  barcode?: string;
  costPrice: number;
  priceModifier: number;
  overridePrice?: number;
  weightKg?: number;
  isDefault: boolean;
  isActive: boolean;
  sortOrder: number;
  imageUrl?: string;
  attributes: VariantAttributeInfo[];
}

export interface VariantAttributeInfo {
  attributeTypeId: string;
  attributeTypeName: string;
  optionId: string;
  optionValue: string;
}

export interface AttributeType {
  id: string;
  name: string;
  slug: string;
  uiType: string;
  affectsPrice: boolean;
  affectsSku: boolean;
  affectsImage: boolean;
  affectsStock: boolean;
  isFilterable: boolean;
  sortOrder: number;
  optionCount?: number;
  options?: AttributeOption[];
}

export interface AttributeOption {
  id: string;
  value: string;
  displayValue?: string;
  colorId?: string;
  sortOrder: number;
  isActive: boolean;
}

export interface ProductImage {
  id: string;
  productId: string;
  variantId?: string;
  imageUrl: string;
  altText?: string;
  sortOrder: number;
  isPrimary: boolean;
}

export interface ProductTag {
  tagId: string;
  name: string;
  slug: string;
}

export interface Tag {
  id: string;
  name: string;
  slug: string;
  productCount?: number;
}

export interface ProductSpecValue {
  id: string;
  specId: string;
  specName: string;
  variantId?: string;
  value: string;
}

export interface Specification {
  id: string;
  specName: string;
  sortOrder: number;
}

export interface ProductSupplierLink {
  id: string;
  supplierId: string;
  supplierName: string;
  supplierCode?: string;
  supplierSku?: string;
  unitCost?: number;
  leadTimeDays?: number;
  isPreferred: boolean;
  isActive: boolean;
}

export interface ProductPriceHistory {
  id: string;
  changedByUserId: string;
  changedByName?: string;
  oldCostPrice: number;
  oldSalePrice: number;
  newCostPrice: number;
  newSalePrice: number;
  effectiveFrom: string;
  effectiveTo?: string;
  reason?: string;
  createdAt: string;
}

export interface ProductAttributeLink {
  id: string;
  attributeTypeId: string;
  attributeTypeName: string;
  uiType: string;
  isRequired: boolean;
  sortOrder: number;
  options: AttributeOption[];
}

export interface BundleComponent {
  id: string;
  componentVariantId: string;
  variantName: string;
  productName: string;
  quantity: number;
  isSubstitutable: boolean;
  sortOrder: number;
}

export interface BundleOptionGroup {
  id: string;
  groupName: string;
  isRequired: boolean;
  minSelections: number;
  maxSelections: number;
  quantityPerSelection: number;
  sortOrder: number;
  items: BundleOptionItem[];
}

export interface BundleOptionItem {
  id: string;
  variantId: string;
  variantName: string;
  productName: string;
  priceAdjustment: number;
  isDefault: boolean;
  sortOrder: number;
}

export interface ProductCollection {
  id: string;
  name: string;
  slug: string;
  description?: string;
  imageUrl?: string;
  displayOrder: number;
  isActive: boolean;
  showInHomePage: boolean;
  productCount?: number;
  products?: CollectionProduct[];
}

export interface CollectionProduct {
  id: string;
  productId: string;
  productName: string;
  productCode?: string;
  imageUrl?: string;
  salePrice: number;
  displayOrder: number;
}

// ========== API FUNCTIONS ==========

// Product Variants
export const productVariantApi = {
  getByProduct: (productId: string) => apiClient.get(`/products/${productId}/variants`),
  create: (productId: string, data: any) => apiClient.post(`/products/${productId}/variants`, data),
  update: (productId: string, variantId: string, data: any) => apiClient.put(`/products/${productId}/variants/${variantId}`, data),
  delete: (productId: string, variantId: string) => apiClient.delete(`/products/${productId}/variants/${variantId}`),
};

// Attribute Types & Options
export const attributeTypeApi = {
  getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string }) => apiClient.get('/attribute-types', { params }),
  getById: (id: string) => apiClient.get(`/attribute-types/${id}`),
  create: (data: any) => apiClient.post('/attribute-types', data),
  update: (id: string, data: any) => apiClient.put(`/attribute-types/${id}`, data),
  delete: (id: string) => apiClient.delete(`/attribute-types/${id}`),
  // Options
  getOptions: (typeId: string) => apiClient.get(`/attribute-types/${typeId}/options`),
  createOption: (typeId: string, data: any) => apiClient.post(`/attribute-types/${typeId}/options`, data),
  bulkCreateOptions: (typeId: string, options: any[]) => apiClient.post(`/attribute-types/${typeId}/options/bulk`, { attributeTypeId: typeId, options }),
  updateOption: (typeId: string, optionId: string, data: any) => apiClient.put(`/attribute-types/${typeId}/options/${optionId}`, data),
  deleteOption: (typeId: string, optionId: string) => apiClient.delete(`/attribute-types/${typeId}/options/${optionId}`),
};

// Product Images
export const productImageApi = {
  getByProduct: (productId: string, variantId?: string) => apiClient.get(`/products/${productId}/images`, { params: { variantId } }),
  add: (productId: string, data: any) => apiClient.post(`/products/${productId}/images`, data),
  update: (productId: string, imageId: string, data: any) => apiClient.put(`/products/${productId}/images/${imageId}`, data),
  delete: (productId: string, imageId: string) => apiClient.delete(`/products/${productId}/images/${imageId}`),
  reorder: (productId: string, orders: { imageId: string; sortOrder: number }[]) => apiClient.put(`/products/${productId}/images/reorder`, { productId, orders }),
};

// Tags
export const tagApi = {
  getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string }) => apiClient.get('/tags', { params }),
  create: (data: { name: string; slug?: string }) => apiClient.post('/tags', data),
  update: (id: string, data: { name: string; slug?: string }) => apiClient.put(`/tags/${id}`, data),
  delete: (id: string) => apiClient.delete(`/tags/${id}`),
};

// Product Tags
export const productTagApi = {
  getByProduct: (productId: string) => apiClient.get(`/products/${productId}/tags`),
  manage: (productId: string, tagIds: string[]) => apiClient.put(`/products/${productId}/tags`, { tagIds }),
};

// Specifications
export const specificationApi = {
  getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string }) => apiClient.get('/specifications', { params }),
  create: (data: { specName: string; sortOrder: number }) => apiClient.post('/specifications', data),
};

// Product Specifications
export const productSpecApi = {
  getByProduct: (productId: string) => apiClient.get(`/products/${productId}/specifications`),
  manage: (productId: string, values: { specId: string; value: string }[]) => apiClient.put(`/products/${productId}/specifications`, { productId, values }),
};

// Product Supplier Links
export const productSupplierApi = {
  getByProduct: (productId: string) => apiClient.get(`/products/${productId}/suppliers`),
  add: (productId: string, data: any) => apiClient.post(`/products/${productId}/suppliers`, data),
  update: (productId: string, linkId: string, data: any) => apiClient.put(`/products/${productId}/suppliers/${linkId}`, data),
  delete: (productId: string, linkId: string) => apiClient.delete(`/products/${productId}/suppliers/${linkId}`),
};

// Product Price History
export const productPriceHistoryApi = {
  getByProduct: (productId: string, params?: { pageIndex?: number; pageSize?: number }) => apiClient.get(`/products/${productId}/price-history`, { params }),
};

// Product Attribute Links
export const productAttributeApi = {
  getByProduct: (productId: string) => apiClient.get(`/products/${productId}/attributes`),
  manage: (productId: string, links: { attributeTypeId: string; isRequired: boolean; sortOrder: number }[]) => apiClient.put(`/products/${productId}/attributes`, { productId, links }),
};

// Bundle
export const bundleApi = {
  getComponents: (productId: string) => apiClient.get(`/products/${productId}/bundle/components`),
  manageComponents: (productId: string, components: any[]) => apiClient.put(`/products/${productId}/bundle/components`, { bundleProductId: productId, components }),
  getOptionGroups: (productId: string) => apiClient.get(`/products/${productId}/bundle/option-groups`),
  createOptionGroup: (productId: string, data: any) => apiClient.post(`/products/${productId}/bundle/option-groups`, data),
  updateOptionGroup: (productId: string, groupId: string, data: any) => apiClient.put(`/products/${productId}/bundle/option-groups/${groupId}`, data),
  deleteOptionGroup: (productId: string, groupId: string) => apiClient.delete(`/products/${productId}/bundle/option-groups/${groupId}`),
};

// Collections
export const collectionApi = {
  getAll: (params?: { pageIndex?: number; pageSize?: number; search?: string }) => apiClient.get('/product-collections', { params }),
  getById: (id: string) => apiClient.get(`/product-collections/${id}`),
  create: (data: any) => apiClient.post('/product-collections', data),
  update: (id: string, data: any) => apiClient.put(`/product-collections/${id}`, data),
  delete: (id: string) => apiClient.delete(`/product-collections/${id}`),
  manageItems: (id: string, items: { productId: string; displayOrder: number }[]) => apiClient.put(`/product-collections/${id}/items`, { collectionId: id, items }),
};
