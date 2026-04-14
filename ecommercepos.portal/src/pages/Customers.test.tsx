import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Customers from '@/pages/Customers';
import { customerApi } from '@/api/customerApi';

vi.mock('@/api/customerApi');

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

describe('Customers Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  const mockStats = {
    totalCustomers: 0,
    newCustomersThisMonth: 0,
    loyaltyMembers: 0,
    totalLoyaltyPoints: 0,
    newCustomersToday: 0,
    topSpender: null,
    recentActivity: []
  };

  it('renders customers page with title', async () => {
    vi.mocked(customerApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(customerApi.getStats).mockResolvedValue({
      data: { data: mockStats }
    } as any);

    renderWithRouter(<Customers />);
    
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Customers' })).toBeInTheDocument();
    });
  });

  it('renders customers table', async () => {
    vi.mocked(customerApi.getAll).mockResolvedValue({
      data: {
        items: [{
          id: '1', customerCode: 'CUS-001', customerType: 'Retail',
          phone: '+8801711000000', loyaltyPoints: 0, isActive: true,
          registrationDate: new Date().toISOString(),
        }],
        totalCount: 1, pageIndex: 0, pageSize: 10,
      }
    } as any);
    vi.mocked(customerApi.getStats).mockResolvedValue({
      data: { data: mockStats }
    } as any);

    renderWithRouter(<Customers />);

    await waitFor(() => {
      expect(screen.getByText('Phone')).toBeInTheDocument();
    });
  });

  it('opens create modal when add button clicked', async () => {
    vi.mocked(customerApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(customerApi.getStats).mockResolvedValue({
      data: { data: mockStats }
    } as any);

    renderWithRouter(<Customers />);
    
    await waitFor(() => {
      const addButtons = screen.getAllByRole('button', { name: /add customer/i });
      fireEvent.click(addButtons[0]);
      expect(screen.getByRole('heading', { name: /add new customer/i })).toBeInTheDocument();
    });
  });

  it('shows search input', async () => {
    vi.mocked(customerApi.getAll).mockResolvedValue({
      data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 10 }
    } as any);
    vi.mocked(customerApi.getStats).mockResolvedValue({
      data: { data: mockStats }
    } as any);

    renderWithRouter(<Customers />);
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/search/i)).toBeInTheDocument();
    });
  });
});
