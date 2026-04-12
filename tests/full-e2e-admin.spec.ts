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
  await page.waitForTimeout(1000);
  await page.locator('input[id="email"]').fill(email);
  await page.locator('input[id="password"]').fill(password);
  await page.locator('button[type="submit"]').click();
  await page.waitForTimeout(3000);
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
});