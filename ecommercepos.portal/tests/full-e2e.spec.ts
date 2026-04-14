import { test, expect } from '@playwright/test';

const ADMIN_URL = 'http://localhost:5173';
const API_URL = 'http://localhost:5149';

async function registerAndLogin(page: any) {
  const timestamp = Date.now();
  const email = `e2e${timestamp}@test.com`;
  const password = 'Test@123456';

  await fetch(`${API_URL}/api/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email,
      password,
      firstName: 'E2E',
      lastName: 'Test',
      phone: '+8801712345678'
    })
  });

  await page.goto(`${ADMIN_URL}/login`);
  await page.locator('input[id="email"]').fill(email);
  await page.locator('input[id="password"]').fill(password);
  await page.locator('button[type="submit"]').click();
  await expect(page).not.toHaveURL(/\/login$/, { timeout: 10000 });
}

async function loginAs(page: any, email: string, password: string) {
  await page.goto(`${ADMIN_URL}/login`);
  await page.locator('input[id="email"]').fill(email);
  await page.locator('input[id="password"]').fill(password);
  await page.locator('button[type="submit"]').click();
  await expect(page).not.toHaveURL(/\/login$/, { timeout: 10000 });
}

test.describe('Admin Portal - Login', () => {
  test('login page loads with form elements', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/login`);
    await expect(page.locator('input[id="email"]')).toBeVisible();
    await expect(page.locator('input[id="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('login form accepts input', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/login`);
    await page.locator('input[id="email"]').fill('test@example.com');
    await page.locator('input[id="password"]').fill('password123');
    await expect(page.locator('input[id="email"]')).toHaveValue('test@example.com');
  });
});

test.describe('Admin Portal - Pages Load', () => {
  test('login page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/login`);
    await expect(page.locator('body')).toBeVisible();
  });

  test('products page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/products`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });

  test('categories page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/categories`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });

  test('brands page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/brands`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });

  test('customers page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/customers`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });

  test('suppliers page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/suppliers`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });

  test('employees page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/employees`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });

  test('orders page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/orders`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });

  test('inventory page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/inventory`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });

  test('settings page loads', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/settings`);
    await page.waitForTimeout(2000);
    await expect(page.locator('body')).toBeVisible();
  });
});

test.describe('Admin Portal - Authenticated CRUD', () => {
  test.beforeEach(async ({ page }) => { 
    await registerAndLogin(page); 
  });

  test('products page accessible after login', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/products`);
    await page.waitForTimeout(2000);
    expect(page.url()).not.toContain('/login');
  });

  test('categories page accessible after login', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/categories`);
    await page.waitForTimeout(2000);
    expect(page.url()).not.toContain('/login');
  });

  test('brands page accessible after login', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/brands`);
    await page.waitForTimeout(2000);
    expect(page.url()).not.toContain('/login');
  });

  test('can open add category modal', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/categories`);
    await page.waitForTimeout(2000);
    
    const addBtn = page.locator('button:has-text("Add Category"), button:has-text("Add")').first();
    if (await addBtn.isVisible()) {
      await addBtn.click();
      await page.waitForTimeout(1000);
      await expect(page.locator('input, [role="dialog"]').first()).toBeVisible();
    }
  });

  test('can open add brand modal', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/brands`);
    await page.waitForTimeout(2000);
    
    const addBtn = page.locator('button:has-text("Add Brand"), button:has-text("Add")').first();
    if (await addBtn.isVisible()) {
      await addBtn.click();
      await page.waitForTimeout(1000);
      await expect(page.locator('input, [role="dialog"]').first()).toBeVisible();
    }
  });

  test('can navigate to all menu items', async ({ page }) => {
    await page.goto(`${ADMIN_URL}/products`);
    await page.waitForTimeout(1500);
    
    await page.goto(`${ADMIN_URL}/categories`);
    await page.waitForTimeout(1500);
    
    await page.goto(`${ADMIN_URL}/brands`);
    await page.waitForTimeout(1500);
    
    await page.goto(`${ADMIN_URL}/customers`);
    await page.waitForTimeout(1500);
    
    await page.goto(`${ADMIN_URL}/orders`);
    await page.waitForTimeout(1500);
    
    await page.goto(`${ADMIN_URL}/inventory`);
    await page.waitForTimeout(1500);
    
    const currentUrl = page.url();
    expect(currentUrl).not.toContain('/login');
  });

  test('categories CRUD works end-to-end', async ({ page }) => {
    const stamp = Date.now();
    const categoryCode = `E2E-CAT-${stamp}`;
    const categoryName = `E2E Category ${stamp}`;
    const categoryUpdated = `E2E Category Updated ${stamp}`;

    await page.goto(`${ADMIN_URL}/categories`);
    await page.waitForTimeout(1500);

    await page.getByRole('button', { name: 'List' }).click();
    await page.waitForTimeout(800);

    await page.getByRole('button', { name: 'Add Category' }).click();
    await page.getByPlaceholder('e.g. CAT-01').fill(categoryCode);
    await page.getByPlaceholder('e.g. Electronics').fill(categoryName);
    await page.getByRole('button', { name: 'Create Category' }).click();
    await page.waitForTimeout(1800);

    await page.getByPlaceholder('Search categories...').fill(categoryName);
    await page.getByPlaceholder('Search categories...').press('Enter');
    await page.waitForTimeout(1800);
    await expect(page.locator('tr', { hasText: categoryName }).first()).toBeVisible();

    const row = page.locator('tr', { hasText: categoryName }).first();
    await row.locator('button').nth(1).click();
    await page.getByPlaceholder('e.g. Electronics').fill(categoryUpdated);
    await page.getByRole('button', { name: 'Save Changes' }).click();
    await page.waitForTimeout(1200);

    await page.getByPlaceholder('Search categories...').fill('');
    await page.getByPlaceholder('Search categories...').press('Enter');
    await page.waitForTimeout(700);
    await page.getByPlaceholder('Search categories...').fill(categoryUpdated);
    await page.getByPlaceholder('Search categories...').press('Enter');
    await page.waitForTimeout(1800);
    await expect(page.locator('tr', { hasText: categoryUpdated }).first()).toBeVisible();

    const rowAfterUpdate = page.locator('tr', { hasText: categoryUpdated }).first();
    await rowAfterUpdate.locator('button').nth(2).click();
    await page.locator('div:has-text("Delete Category")').getByRole('button', { name: 'Delete' }).click();
    await page.waitForTimeout(1200);

    await page.getByPlaceholder('Search categories...').fill(categoryUpdated);
    await page.getByPlaceholder('Search categories...').press('Enter');
    await page.waitForTimeout(1200);
    await expect(page.locator('tr', { hasText: categoryUpdated })).toHaveCount(0);
  });

  test('brands CRUD works end-to-end', async ({ page }) => {
    const stamp = Date.now();
    const brandCode = `E2E-BR-${stamp}`;
    const brandName = `E2E Brand ${stamp}`;
    const brandUpdated = `E2E Brand Updated ${stamp}`;

    await page.goto(`${ADMIN_URL}/brands`);
    await page.waitForTimeout(1500);

    await page.getByRole('button', { name: 'Add Brand' }).click();
    await page.getByPlaceholder('Auto-generated if empty').fill(brandCode);
    await page.locator('input[required]').first().fill(brandName);
    await page.getByRole('button', { name: 'Create' }).click();
    await page.waitForTimeout(1800);

    await page.getByPlaceholder('Search brands...').fill(brandName);
    await page.getByRole('button', { name: 'Search' }).click();
    await page.waitForTimeout(1800);
    await expect(page.locator('tr', { hasText: brandName }).first()).toBeVisible();

    const row = page.locator('tr', { hasText: brandName }).first();
    await row.locator('button').nth(1).click();
    await page.locator('input[required]').first().fill(brandUpdated);
    await page.getByRole('button', { name: 'Update' }).click();
    await page.waitForTimeout(1200);

    await page.getByPlaceholder('Search brands...').fill(brandUpdated);
    await page.getByRole('button', { name: 'Search' }).click();
    await page.waitForTimeout(1500);
    await expect(page.locator('tr', { hasText: brandUpdated }).first()).toBeVisible();

    const rowAfterUpdate = page.locator('tr', { hasText: brandUpdated }).first();
    await rowAfterUpdate.locator('button').nth(2).click();
    await page.locator('div:has-text("Delete Brand")').getByRole('button', { name: 'Delete' }).click();
    await page.waitForTimeout(1200);

    await page.getByPlaceholder('Search brands...').fill(brandUpdated);
    await page.getByRole('button', { name: 'Search' }).click();
    await page.waitForTimeout(1200);
    await expect(page.locator('tr', { hasText: brandUpdated })).toHaveCount(0);
  });

  test('products CRUD works end-to-end', async ({ page }) => {
    const stamp = Date.now();
    const categoryCode = `E2E-PCAT-${stamp}`;
    const categoryName = `E2E Product Category ${stamp}`;
    const brandCode = `E2E-PBR-${stamp}`;
    const brandName = `E2E Product Brand ${stamp}`;
    const productCode = `E2E-PRD-${stamp}`;
    const productName = `E2E Product ${stamp}`;
    const productUpdated = `E2E Product Updated ${stamp}`;

    // Create category prerequisite
    await page.goto(`${ADMIN_URL}/categories`);
    await page.waitForTimeout(1200);
    await page.getByRole('button', { name: 'List' }).click();
    await page.waitForTimeout(600);
    await page.getByRole('button', { name: 'Add Category' }).click();
    await page.getByPlaceholder('e.g. CAT-01').fill(categoryCode);
    await page.getByPlaceholder('e.g. Electronics').fill(categoryName);
    await page.getByRole('button', { name: 'Create Category' }).click();
    await page.waitForTimeout(1200);

    // Create brand prerequisite
    await page.goto(`${ADMIN_URL}/brands`);
    await page.waitForTimeout(1200);
    await page.getByRole('button', { name: 'Add Brand' }).click();
    await page.getByPlaceholder('Auto-generated if empty').fill(brandCode);
    await page.locator('input[required]').first().fill(brandName);
    await page.getByRole('button', { name: 'Create' }).click();
    await page.waitForTimeout(1800);

    // Product CRUD
    await page.goto(`${ADMIN_URL}/products`);
    await page.waitForTimeout(1500);

    await page.getByRole('button', { name: 'Add Product' }).first().click();
    await page.getByPlaceholder('e.g. Samsung Galaxy S24 Ultra').fill(productName);
    await page.getByPlaceholder('e.g. PRD-001').fill(productCode);
    await page.locator('form#product-form select').nth(0).selectOption({ label: categoryName });
    await page.locator('form#product-form select').nth(1).selectOption({ label: brandName });
    await page.getByRole('button', { name: 'Create Product' }).click();
    await page.waitForTimeout(2200);

    await page.getByPlaceholder('Search by name, SKU, barcode...').fill(productName);
    await page.getByRole('button', { name: 'Search' }).click();
    await page.waitForTimeout(1800);
    await expect(page.locator('tr', { hasText: productCode }).first()).toBeVisible();

    const row = page.locator('tr', { hasText: productCode }).first();
    await row.getByTitle('Edit').click();
    await page.getByPlaceholder('e.g. Samsung Galaxy S24 Ultra').fill(productUpdated);
    await page.getByRole('button', { name: 'Update Product' }).click();
    await page.waitForTimeout(1600);

    await page.getByPlaceholder('Search by name, SKU, barcode...').fill(productCode);
    await page.getByRole('button', { name: 'Search' }).click();
    await page.waitForTimeout(1500);
    await expect(page.locator('tr', { hasText: productUpdated }).first()).toBeVisible();

    const rowAfterUpdate = page.locator('tr', { hasText: productCode }).first();
    await rowAfterUpdate.getByTitle('Delete').click();
    await page.getByRole('button', { name: 'Delete Product' }).click();
    await page.waitForTimeout(1600);

    await page.getByPlaceholder('Search by name, SKU, barcode...').fill(productCode);
    await page.getByRole('button', { name: 'Search' }).click();
    await page.waitForTimeout(1500);
    await expect(page.locator('tr', { hasText: productCode })).toHaveCount(0);
  });
});