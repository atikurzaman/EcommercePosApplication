import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Dashboard from '@/pages/Dashboard';

vi.mock('@/api/productApi', () => ({
  productApi: { getAll: vi.fn().mockResolvedValue({ data: { items: [], totalCount: 0 } }) },
}));
vi.mock('@/api/customerApi', () => ({
  customerApi: { getAll: vi.fn().mockResolvedValue({ data: { items: [], totalCount: 0 } }), getStats: vi.fn().mockResolvedValue({ data: { data: {} } }) },
}));
vi.mock('@/api/inventoryApi', () => ({
  inventoryApi: { getLowStock: vi.fn().mockResolvedValue({ data: { items: [], totalCount: 0 } }) },
}));
vi.mock('@/api/posTransactionApi', () => ({
  posTransactionApi: { getAll: vi.fn().mockResolvedValue({ data: { items: [], totalCount: 0 } }) },
}));

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

describe('Dashboard Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders dashboard page with title', async () => {
    renderWithRouter(<Dashboard />);
    
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /dashboard/i })).toBeInTheDocument();
    });
  });

  it('displays welcome message', async () => {
    renderWithRouter(<Dashboard />);
    
    await waitFor(() => {
      expect(screen.getByText(/welcome back/i)).toBeInTheDocument();
    });
  });

  it('displays stats cards', async () => {
    renderWithRouter(<Dashboard />);

    await waitFor(() => {
      expect(screen.getByText(/today's revenue/i)).toBeInTheDocument();
      expect(screen.getByText(/total products/i)).toBeInTheDocument();
    });
  });

  it('shows recent orders section', async () => {
    renderWithRouter(<Dashboard />);

    await waitFor(() => {
      expect(screen.getByText(/recent transactions/i)).toBeInTheDocument();
    });
  });

  it('shows export report button', async () => {
    renderWithRouter(<Dashboard />);
    
    await waitFor(() => {
      expect(screen.getByText(/export report/i)).toBeInTheDocument();
    });
  });

  it('shows new order button', async () => {
    renderWithRouter(<Dashboard />);
    
    await waitFor(() => {
      expect(screen.getAllByText(/new order/i).length).toBeGreaterThan(0);
    });
  });
});
