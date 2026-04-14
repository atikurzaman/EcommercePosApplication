import apiClient from './client';

export interface PosTransaction {
  id: string;
  receiptNumber: string;
  saleDate: string;
  saleType: string;
  subTotal: number;
  discountAmount: number;
  grandTotal: number;
  paidAmount: number;
  changeAmount: number;
  totalItemQuantity: number;
  status: string;
  cashierName?: string;
  customerName?: string;
}

export interface PosTransactionDetail extends PosTransaction {
  totalTaxAmount: number;
  roundOffAmount: number;
  voidReason?: string;
  notes?: string;
  customerPhone?: string;
  lines: TransactionLine[];
  payments: PaymentTender[];
}

export interface TransactionLine {
  id: string;
  productName: string;
  sku?: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
  taxAmount: number;
  lineTotal: number;
}

export interface PaymentTender {
  id: string;
  paymentMethod: string;
  amount: number;
  cardLastFour?: string;
}

export interface PosTransactionFilter {
  pageIndex?: number;
  pageSize?: number;
  search?: string;
  cashierId?: string;
  status?: string;
  startDate?: string;
  endDate?: string;
}

export const posTransactionApi = {
  getAll: (filter?: PosTransactionFilter) =>
    apiClient.get<{ items: PosTransaction[]; totalCount: number; pageIndex: number; pageSize: number }>('/pos-transactions', {
      params: filter,
    }),

  getById: (id: string) =>
    apiClient.get<PosTransactionDetail>(`/pos-transactions/${id}`),

  create: (data: { cashShiftId: string; posCounterId: string; customerId?: string; customerName?: string; customerPhone?: string; saleType?: string; lines: TransactionLine[]; payments: PaymentTender[]; notes?: string }) =>
    apiClient.post<PosTransaction>('/pos-transactions', data),

  void: (id: string, data: { voidedBy: string; reason: string }) =>
    apiClient.post(`/pos-transactions/void/${id}`, data),
};
