# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: tests\full-e2e.spec.ts >> Admin Portal - Authenticated CRUD >> products CRUD works end-to-end
- Location: tests\full-e2e.spec.ts:275:3

# Error details

```
Test timeout of 30000ms exceeded.
```

```
Error: locator.fill: Test timeout of 30000ms exceeded.
Call log:
  - waiting for getByPlaceholder('Search by name, SKU, barcode...')

```

# Test source

```ts
  228 |     await page.getByPlaceholder('Search categories...').press('Enter');
  229 |     await page.waitForTimeout(1200);
  230 |     await expect(page.locator('tr', { hasText: categoryUpdated })).toHaveCount(0);
  231 |   });
  232 | 
  233 |   test('brands CRUD works end-to-end', async ({ page }) => {
  234 |     const stamp = Date.now();
  235 |     const brandCode = `E2E-BR-${stamp}`;
  236 |     const brandName = `E2E Brand ${stamp}`;
  237 |     const brandUpdated = `E2E Brand Updated ${stamp}`;
  238 | 
  239 |     await page.goto(`${ADMIN_URL}/brands`);
  240 |     await page.waitForTimeout(1500);
  241 | 
  242 |     await page.getByRole('button', { name: 'Add Brand' }).click();
  243 |     await page.getByPlaceholder('Auto-generated if empty').fill(brandCode);
  244 |     await page.locator('input[required]').first().fill(brandName);
  245 |     await page.getByRole('button', { name: 'Create' }).click();
  246 |     await page.waitForTimeout(1800);
  247 | 
  248 |     await page.getByPlaceholder('Search brands...').fill(brandName);
  249 |     await page.getByRole('button', { name: 'Search' }).click();
  250 |     await page.waitForTimeout(1800);
  251 |     await expect(page.locator('tr', { hasText: brandName }).first()).toBeVisible();
  252 | 
  253 |     const row = page.locator('tr', { hasText: brandName }).first();
  254 |     await row.locator('button').nth(1).click();
  255 |     await page.locator('input[required]').first().fill(brandUpdated);
  256 |     await page.getByRole('button', { name: 'Update' }).click();
  257 |     await page.waitForTimeout(1200);
  258 | 
  259 |     await page.getByPlaceholder('Search brands...').fill(brandUpdated);
  260 |     await page.getByRole('button', { name: 'Search' }).click();
  261 |     await page.waitForTimeout(1500);
  262 |     await expect(page.locator('tr', { hasText: brandUpdated }).first()).toBeVisible();
  263 | 
  264 |     const rowAfterUpdate = page.locator('tr', { hasText: brandUpdated }).first();
  265 |     await rowAfterUpdate.locator('button').nth(2).click();
  266 |     await page.locator('div:has-text("Delete Brand")').getByRole('button', { name: 'Delete' }).click();
  267 |     await page.waitForTimeout(1200);
  268 | 
  269 |     await page.getByPlaceholder('Search brands...').fill(brandUpdated);
  270 |     await page.getByRole('button', { name: 'Search' }).click();
  271 |     await page.waitForTimeout(1200);
  272 |     await expect(page.locator('tr', { hasText: brandUpdated })).toHaveCount(0);
  273 |   });
  274 | 
  275 |   test('products CRUD works end-to-end', async ({ page }) => {
  276 |     const stamp = Date.now();
  277 |     const categoryCode = `E2E-PCAT-${stamp}`;
  278 |     const categoryName = `E2E Product Category ${stamp}`;
  279 |     const brandCode = `E2E-PBR-${stamp}`;
  280 |     const brandName = `E2E Product Brand ${stamp}`;
  281 |     const productCode = `E2E-PRD-${stamp}`;
  282 |     const productName = `E2E Product ${stamp}`;
  283 |     const productUpdated = `E2E Product Updated ${stamp}`;
  284 | 
  285 |     // Create category prerequisite
  286 |     await page.goto(`${ADMIN_URL}/categories`);
  287 |     await page.waitForTimeout(1200);
  288 |     await page.getByRole('button', { name: 'List' }).click();
  289 |     await page.waitForTimeout(600);
  290 |     await page.getByRole('button', { name: 'Add Category' }).click();
  291 |     await page.getByPlaceholder('e.g. CAT-01').fill(categoryCode);
  292 |     await page.getByPlaceholder('e.g. Electronics').fill(categoryName);
  293 |     await page.getByRole('button', { name: 'Create Category' }).click();
  294 |     await page.waitForTimeout(1200);
  295 | 
  296 |     // Create brand prerequisite
  297 |     await page.goto(`${ADMIN_URL}/brands`);
  298 |     await page.waitForTimeout(1200);
  299 |     await page.getByRole('button', { name: 'Add Brand' }).click();
  300 |     await page.getByPlaceholder('Auto-generated if empty').fill(brandCode);
  301 |     await page.locator('input[required]').first().fill(brandName);
  302 |     await page.getByRole('button', { name: 'Create' }).click();
  303 |     await page.waitForTimeout(1800);
  304 | 
  305 |     // Product CRUD
  306 |     await page.goto(`${ADMIN_URL}/products`);
  307 |     await page.waitForTimeout(1500);
  308 | 
  309 |     await page.getByRole('button', { name: 'Add Product' }).first().click();
  310 |     await page.getByPlaceholder('e.g. Samsung Galaxy S24 Ultra').fill(productName);
  311 |     await page.getByPlaceholder('e.g. PRD-001').fill(productCode);
  312 |     await page.locator('form#product-form select').nth(0).selectOption({ label: categoryName });
  313 |     await page.locator('form#product-form select').nth(1).selectOption({ label: brandName });
  314 |     await page.getByRole('button', { name: 'Create Product' }).click();
  315 |     await page.waitForTimeout(2200);
  316 | 
  317 |     await page.getByPlaceholder('Search by name, SKU, barcode...').fill(productName);
  318 |     await page.getByRole('button', { name: 'Search' }).click();
  319 |     await page.waitForTimeout(1800);
  320 |     await expect(page.locator('tr', { hasText: productCode }).first()).toBeVisible();
  321 | 
  322 |     const row = page.locator('tr', { hasText: productCode }).first();
  323 |     await row.getByTitle('Edit').click();
  324 |     await page.getByPlaceholder('e.g. Samsung Galaxy S24 Ultra').fill(productUpdated);
  325 |     await page.getByRole('button', { name: 'Update Product' }).click();
  326 |     await page.waitForTimeout(1600);
  327 | 
> 328 |     await page.getByPlaceholder('Search by name, SKU, barcode...').fill(productCode);
      |                                                                    ^ Error: locator.fill: Test timeout of 30000ms exceeded.
  329 |     await page.getByRole('button', { name: 'Search' }).click();
  330 |     await page.waitForTimeout(1500);
  331 |     await expect(page.locator('tr', { hasText: productUpdated }).first()).toBeVisible();
  332 | 
  333 |     const rowAfterUpdate = page.locator('tr', { hasText: productCode }).first();
  334 |     await rowAfterUpdate.getByTitle('Delete').click();
  335 |     await page.getByRole('button', { name: 'Delete Product' }).click();
  336 |     await page.waitForTimeout(1600);
  337 | 
  338 |     await page.getByPlaceholder('Search by name, SKU, barcode...').fill(productCode);
  339 |     await page.getByRole('button', { name: 'Search' }).click();
  340 |     await page.waitForTimeout(1500);
  341 |     await expect(page.locator('tr', { hasText: productCode })).toHaveCount(0);
  342 |   });
  343 | });
```