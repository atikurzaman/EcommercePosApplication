import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Brands from '@/pages/Brands';
import { brandApi } from '@/api/brandApi';

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

describe('Brands Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders brands page with title', async () => {
    vi.mocked(brandApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Brands />);
    
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /brands/i })).toBeInTheDocument();
    });
  });

  it('shows add brand button', async () => {
    vi.mocked(brandApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Brands />);
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /add brand/i })).toBeInTheDocument();
    });
  });

  it('shows search input', async () => {
    vi.mocked(brandApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Brands />);
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/search/i)).toBeInTheDocument();
    });
  });

  it('opens add modal when button clicked', async () => {
    vi.mocked(brandApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Brands />);
    
    await waitFor(() => {
      const addButton = screen.getByRole('button', { name: /add brand/i });
      fireEvent.click(addButton);
      expect(screen.getByRole('heading', { name: /add brand/i })).toBeInTheDocument();
    });
  });
});
