import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Units from '@/pages/Units';
import { unitApi } from '@/api/unitApi';

vi.mock('@/api/unitApi');

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

describe('Units Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders units page with title', async () => {
    vi.mocked(unitApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Units />);
    
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /units/i })).toBeInTheDocument();
    });
  });

  it('shows add unit button', async () => {
    vi.mocked(unitApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Units />);
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /add unit/i })).toBeInTheDocument();
    });
  });
});
