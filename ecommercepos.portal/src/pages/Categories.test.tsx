import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor, within } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from '@/hooks/useAuth';
import Categories from '@/pages/Categories';
import { categoryApi } from '@/api/categoryApi';

vi.mock('@/api/categoryApi');

const createTestQueryClient = () =>
  new QueryClient({
    defaultOptions: { queries: { retry: false }, mutations: { retry: false } },
  });

const renderCategories = () => {
  const queryClient = createTestQueryClient();
  return render(
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthProvider>
          <Categories />
        </AuthProvider>
      </BrowserRouter>
    </QueryClientProvider>
  );
};

// ── shared fixtures ──────────────────────────────────────────────────────────

const mockTreeItem = {
  id: 'cat-tree-1',
  name: 'Electronics',
  slug: 'electronics',
  parentCategoryId: null,
  displayOrder: 1,
  isActive: true,
  imageUrl: null,
  children: [],
};

const mockCategory = {
  id: 'cat-1',
  categoryCode: 'CAT-001',
  categoryName: 'Electronics',
  name: 'Electronics',
  slug: 'electronics',
  description: 'Electronic products',
  imageUrl: '',
  parentCategoryId: null,
  displayOrder: 1,
  isFeatured: false,
  isActive: true,
  metaTitle: 'Electronics',
  metaDescription: '',
};

function setupDefaultMocks() {
  vi.mocked(categoryApi.getTree).mockResolvedValue({
    data: { items: [mockTreeItem] },
  } as any);
  vi.mocked(categoryApi.getFlat).mockResolvedValue({
    data: { items: [] },
  } as any);
  vi.mocked(categoryApi.getAll).mockResolvedValue({
    data: { items: [], totalCount: 0, pageIndex: 0, pageSize: 15 },
  } as any);
  vi.mocked(categoryApi.getById).mockResolvedValue({
    data: { data: mockCategory },
  } as any);
  vi.mocked(categoryApi.create).mockResolvedValue({
    data: { data: mockCategory },
  } as any);
  vi.mocked(categoryApi.update).mockResolvedValue({
    data: { data: mockCategory },
  } as any);
  vi.mocked(categoryApi.delete).mockResolvedValue({} as any);
  vi.mocked(categoryApi.toggle).mockResolvedValue({} as any);
}

// Helper: switch to list view with data and wait for table
async function setupListView(items = [mockCategory]) {
  vi.mocked(categoryApi.getAll).mockResolvedValue({
    data: { items, totalCount: items.length, pageIndex: 0, pageSize: 15 },
  } as any);
  renderCategories();
  await waitFor(() => screen.getByRole('button', { name: /add category/i }));
  fireEvent.click(screen.getByRole('button', { name: /list/i }));
  if (items.length > 0) {
    await waitFor(() => screen.getByText(items[0].categoryCode!));
  } else {
    await waitFor(() => screen.getByText(/no categories found/i));
  }
}

// ── tests ────────────────────────────────────────────────────────────────────

describe('Categories Page — CRUD E2E', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setupDefaultMocks();
  });

  // ─ READ — Page Layout ──────────────────────────────────────────────────────

  describe('Read — Page Layout', () => {
    it('renders page heading', async () => {
      renderCategories();
      await waitFor(() =>
        expect(screen.getByRole('heading', { name: 'Categories', level: 1 })).toBeInTheDocument()
      );
    });

    it('shows "Add Category" button', async () => {
      renderCategories();
      await waitFor(() =>
        expect(screen.getByRole('button', { name: /add category/i })).toBeInTheDocument()
      );
    });

    it('shows Tree and List view toggle buttons', async () => {
      renderCategories();
      await waitFor(() => {
        expect(screen.getByRole('button', { name: /tree/i })).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /list/i })).toBeInTheDocument();
      });
    });

    it('shows search input in tree view by default', async () => {
      renderCategories();
      await waitFor(() =>
        expect(screen.getByPlaceholderText(/search categories/i)).toBeInTheDocument()
      );
    });

    it('renders tree items from getTree response', async () => {
      renderCategories();
      await waitFor(() => expect(screen.getByText('Electronics')).toBeInTheDocument());
    });

    it('calls getTree and getFlat on mount', async () => {
      renderCategories();
      await waitFor(() => {
        expect(vi.mocked(categoryApi.getTree)).toHaveBeenCalled();
        expect(vi.mocked(categoryApi.getFlat)).toHaveBeenCalled();
      });
    });
  });

  // ─ READ — List View ────────────────────────────────────────────────────────

  describe('Read — List View', () => {
    it('calls getAll when switching to list mode', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /list/i }));
      await waitFor(() => expect(vi.mocked(categoryApi.getAll)).toHaveBeenCalled());
    });

    it('renders category data in list table', async () => {
      await setupListView();
      expect(screen.getByText('CAT-001')).toBeInTheDocument();
      expect(screen.getByText('Electronics')).toBeInTheDocument();
    });

    it('shows table column headers', async () => {
      await setupListView();
      expect(screen.getByText('Category')).toBeInTheDocument();
      expect(screen.getByText('Code')).toBeInTheDocument();
      expect(screen.getByText('Status')).toBeInTheDocument();
    });

    it('shows Active badge for active categories', async () => {
      await setupListView();
      expect(screen.getByText('Active')).toBeInTheDocument();
    });

    it('shows empty state when no categories', async () => {
      await setupListView([]);
      expect(screen.getByText(/no categories found/i)).toBeInTheDocument();
    });
  });

  // ─ CREATE ─────────────────────────────────────────────────────────────────

  describe('Create', () => {
    it('opens form panel with heading "New Category" when Add button clicked', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));

      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => expect(screen.getByText('New Category')).toBeInTheDocument());
    });

    it('shows required Code and Category Name fields in create panel', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => {
        expect(screen.getByPlaceholderText('e.g. CAT-01')).toBeInTheDocument();
        expect(screen.getByPlaceholderText('e.g. Electronics')).toBeInTheDocument();
      });
    });

    it('shows "Create Category" submit button in create mode', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() =>
        expect(screen.getByRole('button', { name: /create category/i })).toBeInTheDocument()
      );
    });

    it('calls categoryApi.create with correct payload on form submit', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => screen.getByPlaceholderText('e.g. CAT-01'));

      fireEvent.change(screen.getByPlaceholderText('e.g. CAT-01'), {
        target: { value: 'CAT-TEST' },
      });
      fireEvent.change(screen.getByPlaceholderText('e.g. Electronics'), {
        target: { value: 'Test Category' },
      });

      fireEvent.click(screen.getByRole('button', { name: /create category/i }));

      await waitFor(() =>
        expect(vi.mocked(categoryApi.create)).toHaveBeenCalledWith(
          expect.objectContaining({
            categoryCode: 'CAT-TEST',
            categoryName: 'Test Category',
            isActive: true,
          })
        )
      );
    });

    it('closes panel after successful creation', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => screen.getByPlaceholderText('e.g. CAT-01'));
      fireEvent.change(screen.getByPlaceholderText('e.g. CAT-01'), {
        target: { value: 'CAT-X' },
      });
      fireEvent.change(screen.getByPlaceholderText('e.g. Electronics'), {
        target: { value: 'Cat X' },
      });
      fireEvent.click(screen.getByRole('button', { name: /create category/i }));

      await waitFor(() =>
        expect(screen.queryByText('New Category')).not.toBeInTheDocument()
      );
    });

    it('closes panel when Cancel button is clicked', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => screen.getByText('New Category'));
      fireEvent.click(screen.getByRole('button', { name: /cancel/i }));

      await waitFor(() =>
        expect(screen.queryByText('New Category')).not.toBeInTheDocument()
      );
    });

    it('code input starts empty in create mode', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => {
        const codeInput = screen.getByPlaceholderText('e.g. CAT-01') as HTMLInputElement;
        expect(codeInput.value).toBe('');
      });
    });

    it('Active checkbox is checked by default in create mode', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => {
        const activeCheckbox = screen.getByRole('checkbox', { name: /active/i }) as HTMLInputElement;
        expect(activeCheckbox.checked).toBe(true);
      });
    });
  });

  // ─ UPDATE ─────────────────────────────────────────────────────────────────

  describe('Update', () => {
    it('opens edit panel with heading "Edit Category" from list row', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      // Row buttons: [0] = status toggle, [1] = edit icon, [2] = delete icon
      const buttons = within(row).getAllByRole('button');
      fireEvent.click(buttons[1]);

      await waitFor(() =>
        expect(screen.getByText('Edit Category')).toBeInTheDocument()
      );
    });

    it('pre-fills form with existing category Code', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      fireEvent.click(within(row).getAllByRole('button')[1]);

      await waitFor(() => {
        const codeInput = screen.getByPlaceholderText('e.g. CAT-01') as HTMLInputElement;
        expect(codeInput.value).toBe('CAT-001');
      });
    });

    it('pre-fills form with existing Category Name', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      fireEvent.click(within(row).getAllByRole('button')[1]);

      await waitFor(() => {
        const nameInput = screen.getByPlaceholderText('e.g. Electronics') as HTMLInputElement;
        expect(nameInput.value).toBe('Electronics');
      });
    });

    it('shows "Save Changes" submit button in edit mode', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      fireEvent.click(within(row).getAllByRole('button')[1]);

      await waitFor(() =>
        expect(screen.getByRole('button', { name: /save changes/i })).toBeInTheDocument()
      );
    });

    it('calls categoryApi.update with modified name on save', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      fireEvent.click(within(row).getAllByRole('button')[1]);

      await waitFor(() => screen.getByText('Edit Category'));

      const nameInput = screen.getByPlaceholderText('e.g. Electronics');
      fireEvent.change(nameInput, { target: { value: 'Electronics Updated' } });

      fireEvent.click(screen.getByRole('button', { name: /save changes/i }));

      await waitFor(() =>
        expect(vi.mocked(categoryApi.update)).toHaveBeenCalledWith(
          'cat-1',
          expect.objectContaining({ categoryName: 'Electronics Updated' })
        )
      );
    });

    it('opens edit panel from tree node click and calls getById', async () => {
      renderCategories();
      await waitFor(() => screen.getByText('Electronics'));

      // Click the tree node label
      fireEvent.click(screen.getByText('Electronics'));

      await waitFor(() =>
        expect(vi.mocked(categoryApi.getById)).toHaveBeenCalledWith('cat-tree-1')
      );

      await waitFor(() =>
        expect(screen.getByText('Edit Category')).toBeInTheDocument()
      );
    });

    it('pre-fills form from getById response when editing from tree', async () => {
      renderCategories();
      await waitFor(() => screen.getByText('Electronics'));

      fireEvent.click(screen.getByText('Electronics'));

      await waitFor(() => {
        const codeInput = screen.getByPlaceholderText('e.g. CAT-01') as HTMLInputElement;
        expect(codeInput.value).toBe('CAT-001');
      });
    });
  });

  // ─ DELETE ─────────────────────────────────────────────────────────────────

  describe('Delete', () => {
    it('opens delete confirmation modal from list row delete button', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      // [2] = delete button (icon only, red)
      fireEvent.click(within(row).getAllByRole('button')[2]);

      await waitFor(() =>
        expect(screen.getByText('Delete Category')).toBeInTheDocument()
      );
    });

    it('shows the category name in the delete confirmation message', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      fireEvent.click(within(row).getAllByRole('button')[2]);

      await waitFor(() => {
        expect(screen.getByText('Delete Category')).toBeInTheDocument();
        expect(screen.getByText(/"Electronics"/)).toBeInTheDocument();
      });
    });

    it('calls categoryApi.delete when the Delete confirmation button is clicked', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      fireEvent.click(within(row).getAllByRole('button')[2]);

      await waitFor(() => screen.getByText('Delete Category'));

      // Modal has "Cancel" and "Delete" buttons; click the destructive Delete button
      const deleteButtons = screen.getAllByRole('button', { name: /^delete$/i });
      fireEvent.click(deleteButtons[deleteButtons.length - 1]);

      await waitFor(() =>
        expect(vi.mocked(categoryApi.delete)).toHaveBeenCalledWith('cat-1')
      );
    });

    it('closes delete modal when Cancel is clicked', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      fireEvent.click(within(row).getAllByRole('button')[2]);

      await waitFor(() => screen.getByText('Delete Category'));
      fireEvent.click(screen.getByRole('button', { name: /cancel/i }));

      await waitFor(() =>
        expect(screen.queryByText('Delete Category')).not.toBeInTheDocument()
      );
    });

    it('does NOT call categoryApi.delete when Cancel is clicked', async () => {
      await setupListView();

      const row = screen.getByText('CAT-001').closest('tr')!;
      fireEvent.click(within(row).getAllByRole('button')[2]);

      await waitFor(() => screen.getByText('Delete Category'));
      fireEvent.click(screen.getByRole('button', { name: /cancel/i }));

      expect(vi.mocked(categoryApi.delete)).not.toHaveBeenCalled();
    });
  });

  // ─ TOGGLE STATUS ──────────────────────────────────────────────────────────

  describe('Toggle Status', () => {
    it('calls categoryApi.toggle with category id when Active badge is clicked', async () => {
      await setupListView();

      // Status toggle button shows "Active" text
      fireEvent.click(screen.getByText('Active'));

      await waitFor(() =>
        expect(vi.mocked(categoryApi.toggle)).toHaveBeenCalledWith('cat-1')
      );
    });

    it('calls categoryApi.toggle with category id when Inactive badge is clicked', async () => {
      const inactiveCategory = { ...mockCategory, id: 'cat-2', categoryCode: 'CAT-002', isActive: false };
      await setupListView([inactiveCategory]);

      fireEvent.click(screen.getByText('Inactive'));

      await waitFor(() =>
        expect(vi.mocked(categoryApi.toggle)).toHaveBeenCalledWith('cat-2')
      );
    });
  });

  // ─ VIEW SWITCHING ──────────────────────────────────────────────────────────

  describe('View Switching', () => {
    it('tree view is shown by default', async () => {
      renderCategories();
      await waitFor(() =>
        expect(screen.getByPlaceholderText(/search categories/i)).toBeInTheDocument()
      );
    });

    it('switching to list view renders list table', async () => {
      vi.mocked(categoryApi.getAll).mockResolvedValue({
        data: { items: [mockCategory], totalCount: 1, pageIndex: 0, pageSize: 15 },
      } as any);
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));

      fireEvent.click(screen.getByRole('button', { name: /list/i }));

      await waitFor(() => expect(screen.getByText('CAT-001')).toBeInTheDocument());
    });

    it('switching back to tree view shows tree search again', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));

      fireEvent.click(screen.getByRole('button', { name: /list/i }));
      fireEvent.click(screen.getByRole('button', { name: /tree/i }));

      await waitFor(() =>
        expect(screen.getByPlaceholderText(/search categories/i)).toBeInTheDocument()
      );
    });

    it('tree view shows Expand All and Collapse All links', async () => {
      renderCategories();
      await waitFor(() => {
        expect(screen.getByText(/expand all/i)).toBeInTheDocument();
        expect(screen.getByText(/collapse all/i)).toBeInTheDocument();
      });
    });
  });

  // ─ SEO TAB ────────────────────────────────────────────────────────────────

  describe('SEO Tab', () => {
    it('shows SEO fields (Slug, Meta Title, Meta Description) when SEO tab is clicked', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => screen.getByText('New Category'));

      fireEvent.click(screen.getByText('seo'));

      await waitFor(() => {
        expect(screen.getByPlaceholderText('auto-generated-from-name')).toBeInTheDocument();
        expect(screen.getByText(/meta title/i)).toBeInTheDocument();
        expect(screen.getByText(/meta description/i)).toBeInTheDocument();
      });
    });

    it('general tab is active by default in create mode', async () => {
      renderCategories();
      await waitFor(() => screen.getByRole('button', { name: /add category/i }));
      fireEvent.click(screen.getByRole('button', { name: /add category/i }));

      await waitFor(() => {
        // General tab fields are visible
        expect(screen.getByPlaceholderText('e.g. CAT-01')).toBeInTheDocument();
        // SEO slug field is NOT visible
        expect(screen.queryByPlaceholderText('auto-generated-from-name')).not.toBeInTheDocument();
      });
    });
  });
});
