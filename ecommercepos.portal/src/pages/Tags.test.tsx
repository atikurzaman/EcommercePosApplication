import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Tags from '@/pages/Tags';
import { tagApi } from '@/api/tagApi';

vi.mock('@/api/tagApi');

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

describe('Tags Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders tags page with title', async () => {
    vi.mocked(tagApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Tags />);
    
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /tags/i })).toBeInTheDocument();
    });
  });

  it('shows add tag button', async () => {
    vi.mocked(tagApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);

    renderWithRouter(<Tags />);
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /add tag/i })).toBeInTheDocument();
    });
  });
});
