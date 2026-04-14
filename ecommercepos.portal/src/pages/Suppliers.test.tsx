import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Suppliers from '@/pages/Suppliers';
import { supplierApi } from '@/api/supplierApi';

vi.mock('@/api/supplierApi');

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

describe('Suppliers Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders suppliers page with title', async () => {
    vi.mocked(supplierApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Suppliers />);
    
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /suppliers/i })).toBeInTheDocument();
    });
  });

  it('shows add supplier button', async () => {
    vi.mocked(supplierApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Suppliers />);
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /add supplier/i })).toBeInTheDocument();
    });
  });

  it('shows search input', async () => {
    vi.mocked(supplierApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Suppliers />);
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/search/i)).toBeInTheDocument();
    });
  });
});
