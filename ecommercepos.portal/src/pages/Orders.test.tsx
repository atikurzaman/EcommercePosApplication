import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Orders from '@/pages/Orders';
import { orderApi } from '@/api/orderApi';

vi.mock('@/api/orderApi');

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

describe('Orders Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  const mockStats = {
    totalOrders: 0,
    totalRevenue: 0,
    pendingOrders: 0,
    processingOrders: 0,
    shippedOrders: 0,
    deliveredOrders: 0,
    todaysSales: 0,
    recentOrders: []
  };

  it('renders orders page with title', async () => {
    vi.mocked(orderApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(orderApi.getStats).mockResolvedValue({
      data: { data: mockStats }
    } as any);

    renderWithRouter(<Orders />);
    
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Orders' })).toBeInTheDocument();
    });
  });

  it('renders orders table', async () => {
    vi.mocked(orderApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(orderApi.getStats).mockResolvedValue({
      data: { data: mockStats }
    } as any);

    renderWithRouter(<Orders />);
    
    await waitFor(() => {
      expect(screen.getByText('Order #')).toBeInTheDocument();
    });
  });

  it('shows status filter options', async () => {
    vi.mocked(orderApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(orderApi.getStats).mockResolvedValue({
      data: { data: mockStats }
    } as any);

    renderWithRouter(<Orders />);
    
    await waitFor(() => {
      expect(screen.getByRole('combobox')).toBeInTheDocument();
    });
  });
});
