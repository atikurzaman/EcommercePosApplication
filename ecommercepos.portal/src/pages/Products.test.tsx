import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Products from '@/pages/Products';
import { productApi } from '@/api/productApi';
import { categoryApi } from '@/api/categoryApi';
import { brandApi } from '@/api/brandApi';

vi.mock('@/api/productApi');
vi.mock('@/api/categoryApi');
vi.mock('@/api/brandApi');

const createTestQueryClient = () => new QueryClient({
  defaultOptions: { queries: { retry: false }, mutations: { retry: false } },
});

const renderWithRouter = (component: React.ReactElement) => {
  const queryClient = createTestQueryClient();
  return render(
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthProvider>
          {component}
        </AuthProvider>
      </BrowserRouter>
    </QueryClientProvider>
  );
};

describe('Products Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders products page with title', async () => {
    vi.mocked(productApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(categoryApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(brandApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Products />);
    
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Products' })).toBeInTheDocument();
    });
  });

  it('displays products table when data loads', async () => {
    const mockProducts = [
      { id: '1', productCode: 'PRD-001', productName: 'Test Product', sku: 'SKU-001', category: null, brand: null, costPrice: 100, sellPrice: 150, quantity: 50, isActive: true }
    ];
    
    vi.mocked(productApi.getAll).mockResolvedValue({
      data: { items: mockProducts, totalCount: 1, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(categoryApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(brandApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Products />);
    
    await waitFor(() => {
      expect(screen.getByText('Test Product')).toBeInTheDocument();
    });
  });

  it('opens create modal when add button clicked', async () => {
    vi.mocked(productApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(categoryApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(brandApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Products />);

    await waitFor(() => {
      const addButtons = screen.getAllByRole('button', { name: /add product/i });
      fireEvent.click(addButtons[0]);
      expect(screen.getByRole('heading', { name: /add new product/i })).toBeInTheDocument();
    });
  });

  it('shows empty table when no products', async () => {
    vi.mocked(productApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(categoryApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(brandApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Products />);

    await waitFor(() => {
      expect(screen.getByText(/no products found/i)).toBeInTheDocument();
    });
  });
});
