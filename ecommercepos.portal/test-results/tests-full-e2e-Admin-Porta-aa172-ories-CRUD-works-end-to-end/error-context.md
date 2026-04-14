# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: tests\full-e2e.spec.ts >> Admin Portal - Authenticated CRUD >> categories CRUD works end-to-end
- Location: tests\full-e2e.spec.ts:185:3

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: locator('tr').filter({ hasText: 'E2E Category Updated 1776030592306' }).first()
Expected: visible
Timeout: 5000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" with timeout 5000ms
  - waiting for locator('tr').filter({ hasText: 'E2E Category Updated 1776030592306' }).first()

```

# Page snapshot

```yaml
- generic [ref=e3]:
  - complementary [ref=e5]:
    - generic [ref=e6]:
      - generic [ref=e7]: "N"
      - generic [ref=e8]: NEXUSAdmin Portal
    - generic [ref=e9]:
      - img [ref=e10]
      - textbox "Search..." [ref=e13]
    - navigation [ref=e14]:
      - link "Dashboard" [ref=e15] [cursor=pointer]:
        - /url: /
        - img [ref=e16]
        - generic [ref=e21]: Dashboard
      - generic [ref=e22]:
        - generic [ref=e23] [cursor=pointer]:
          - generic [ref=e24]:
            - img [ref=e25]
            - text: Catalog
          - generic [ref=e29]: ▶
        - generic [ref=e30]:
          - link "Products" [ref=e31] [cursor=pointer]:
            - /url: /products
            - img [ref=e32]
            - generic [ref=e36]: Products
          - link "Categories" [ref=e37] [cursor=pointer]:
            - /url: /categories
            - img [ref=e38]
            - generic [ref=e42]: Categories
          - link "Brands" [ref=e43] [cursor=pointer]:
            - /url: /brands
            - img [ref=e44]
            - generic [ref=e47]: Brands
          - link "Tags" [ref=e48] [cursor=pointer]:
            - /url: /tags
            - img [ref=e49]
            - generic [ref=e51]: Tags
          - link "Collections" [ref=e52] [cursor=pointer]:
            - /url: /collections
            - img [ref=e53]
            - generic [ref=e56]: Collections
      - generic [ref=e58] [cursor=pointer]:
        - generic [ref=e59]:
          - img [ref=e60]
          - text: Sales
        - generic [ref=e64]: ▶
      - generic [ref=e66] [cursor=pointer]:
        - generic [ref=e67]:
          - img [ref=e68]
          - text: POS
        - generic [ref=e71]: ▶
      - generic [ref=e73] [cursor=pointer]:
        - generic [ref=e74]:
          - img [ref=e75]
          - text: Inventory
        - generic [ref=e78]: ▶
      - generic [ref=e80] [cursor=pointer]:
        - generic [ref=e81]:
          - img [ref=e82]
          - text: Customers
        - generic [ref=e87]: ▶
      - generic [ref=e89] [cursor=pointer]:
        - generic [ref=e90]:
          - img [ref=e91]
          - text: Procurement
        - generic [ref=e94]: ▶
      - generic [ref=e96] [cursor=pointer]:
        - generic [ref=e97]:
          - img [ref=e98]
          - text: Status Definitions
        - generic [ref=e101]: ▶
      - generic [ref=e103] [cursor=pointer]:
        - generic [ref=e104]:
          - img [ref=e105]
          - text: Reference Data
        - generic [ref=e109]: ▶
      - generic [ref=e111] [cursor=pointer]:
        - generic [ref=e112]:
          - img [ref=e113]
          - text: Access Control
        - generic [ref=e115]: ▶
      - link "Reports" [ref=e116] [cursor=pointer]:
        - /url: /reports
        - img [ref=e117]
        - generic [ref=e119]: Reports
      - link "Settings" [ref=e120] [cursor=pointer]:
        - /url: /settings
        - img [ref=e121]
        - generic [ref=e124]: Settings
    - generic [ref=e125]:
      - button "Dark Mode" [ref=e126] [cursor=pointer]:
        - img
        - text: Dark Mode
      - button "Logout" [ref=e127]:
        - img
        - text: Logout
  - generic [ref=e128]:
    - banner [ref=e129]:
      - generic [ref=e130]:
        - generic [ref=e131]: NEXUS
        - generic [ref=e132]: /
        - generic [ref=e133]: Categories
      - generic [ref=e134]:
        - img [ref=e135]
        - textbox "Search anything..." [ref=e138]
      - generic [ref=e139]:
        - button "Notifications" [ref=e140] [cursor=pointer]:
          - img [ref=e141]
        - button "Toggle Theme" [ref=e144] [cursor=pointer]:
          - img [ref=e145]
        - link "POS" [ref=e147] [cursor=pointer]:
          - /url: /pos
          - img [ref=e148]
        - link "Settings" [ref=e152] [cursor=pointer]:
          - /url: /settings
          - img [ref=e153]
        - link "Profile" [ref=e156] [cursor=pointer]:
          - /url: /profile
          - img [ref=e157]
        - generic [ref=e161] [cursor=pointer]: ET
    - main [ref=e162]:
      - generic [ref=e163]:
        - generic [ref=e164]:
          - generic [ref=e165]:
            - heading "Categories" [level=1] [ref=e166]
            - paragraph [ref=e167]: Organise your product catalog with categories
          - generic [ref=e168]:
            - generic [ref=e169]:
              - button "Tree" [ref=e170]:
                - img [ref=e171]
                - text: Tree
              - button "List" [ref=e175]:
                - img [ref=e176]
                - text: List
            - button "Add Category" [ref=e179]:
              - img
              - text: Add Category
        - generic [ref=e180]:
          - generic [ref=e181]:
            - generic [ref=e182]:
              - paragraph [ref=e183]: Total Categories
              - img [ref=e184]
            - paragraph [ref=e186]: "10"
          - generic [ref=e187]:
            - generic [ref=e188]:
              - paragraph [ref=e189]: Root Categories
              - img [ref=e190]
            - paragraph [ref=e194]: "10"
          - generic [ref=e195]:
            - generic [ref=e196]:
              - paragraph [ref=e197]: Sub-categories
              - img [ref=e198]
            - paragraph [ref=e200]: "0"
        - generic [ref=e201]:
          - generic [ref=e203]:
            - generic [ref=e204]:
              - generic [ref=e205]:
                - img [ref=e206]
                - textbox "Search categories..." [active] [ref=e209]: E2E Category Updated 1776030592306
              - combobox [ref=e211] [cursor=pointer]:
                - option "All Status" [selected]
                - option "Active"
                - option "Inactive"
            - generic [ref=e212]:
              - img [ref=e213]
              - paragraph [ref=e215]: No categories found
          - generic [ref=e218]:
            - generic [ref=e219]:
              - heading "Edit Category" [level=3] [ref=e221]
              - generic [ref=e222]:
                - button "Delete category" [ref=e223]:
                  - img
                - button [ref=e224]:
                  - img
            - generic [ref=e225]:
              - button "general" [ref=e226]
              - button "seo" [ref=e227]
            - generic [ref=e229]:
              - generic [ref=e230]:
                - generic [ref=e231]:
                  - generic [ref=e232]: Code *
                  - textbox "e.g. CAT-01" [ref=e233]
                - generic [ref=e234]:
                  - generic [ref=e235]: Display Order
                  - spinbutton [ref=e236]: "0"
              - generic [ref=e237]:
                - generic [ref=e238]: Category Name *
                - textbox "e.g. Electronics" [ref=e239]: E2E Category Updated 1776030592306
              - generic [ref=e240]:
                - generic [ref=e241]: Parent Category
                - combobox [ref=e242] [cursor=pointer]:
                  - option "No Parent (Root Category)" [selected]
                  - option
                  - option "E2E Category 1776030289487"
                  - option "E2E Category 1776030340740"
                  - option "E2E Category 1776030454976"
                  - option "E2E Product Category 1776030317453"
                  - option "E2E Product Category 1776030368905"
                  - option "E2E Product Category 1776030485299"
                  - option "Product Test Cat"
                  - option "POS Test Category"
              - generic [ref=e243]:
                - generic [ref=e244]: Description
                - textbox "Brief description of this category..." [ref=e245]
              - generic [ref=e246]:
                - generic [ref=e247]: Image URL
                - generic [ref=e248]:
                  - textbox "https://example.com/image.jpg" [ref=e249]
                  - button [ref=e250]:
                    - img
              - generic [ref=e251]:
                - generic [ref=e252] [cursor=pointer]:
                  - checkbox "Active" [checked] [ref=e253]
                  - generic [ref=e254]: Active
                - generic [ref=e255] [cursor=pointer]:
                  - checkbox "Featured" [ref=e256]
                  - generic [ref=e257]: Featured
            - generic [ref=e258]:
              - generic [ref=e260]:
                - text: "ID:"
                - code [ref=e261]: 1621bac7...
              - generic [ref=e262]:
                - button "Cancel" [ref=e263]
                - button "Save Changes" [ref=e264]
```

# Test source

```ts
  120 |   test('products page accessible after login', async ({ page }) => {
  121 |     await page.goto(`${ADMIN_URL}/products`);
  122 |     await page.waitForTimeout(2000);
  123 |     expect(page.url()).not.toContain('/login');
  124 |   });
  125 | 
  126 |   test('categories page accessible after login', async ({ page }) => {
  127 |     await page.goto(`${ADMIN_URL}/categories`);
  128 |     await page.waitForTimeout(2000);
  129 |     expect(page.url()).not.toContain('/login');
  130 |   });
  131 | 
  132 |   test('brands page accessible after login', async ({ page }) => {
  133 |     await page.goto(`${ADMIN_URL}/brands`);
  134 |     await page.waitForTimeout(2000);
  135 |     expect(page.url()).not.toContain('/login');
  136 |   });
  137 | 
  138 |   test('can open add category modal', async ({ page }) => {
  139 |     await page.goto(`${ADMIN_URL}/categories`);
  140 |     await page.waitForTimeout(2000);
  141 |     
  142 |     const addBtn = page.locator('button:has-text("Add Category"), button:has-text("Add")').first();
  143 |     if (await addBtn.isVisible()) {
  144 |       await addBtn.click();
  145 |       await page.waitForTimeout(1000);
  146 |       await expect(page.locator('input, [role="dialog"]').first()).toBeVisible();
  147 |     }
  148 |   });
  149 | 
  150 |   test('can open add brand modal', async ({ page }) => {
  151 |     await page.goto(`${ADMIN_URL}/brands`);
  152 |     await page.waitForTimeout(2000);
  153 |     
  154 |     const addBtn = page.locator('button:has-text("Add Brand"), button:has-text("Add")').first();
  155 |     if (await addBtn.isVisible()) {
  156 |       await addBtn.click();
  157 |       await page.waitForTimeout(1000);
  158 |       await expect(page.locator('input, [role="dialog"]').first()).toBeVisible();
  159 |     }
  160 |   });
  161 | 
  162 |   test('can navigate to all menu items', async ({ page }) => {
  163 |     await page.goto(`${ADMIN_URL}/products`);
  164 |     await page.waitForTimeout(1500);
  165 |     
  166 |     await page.goto(`${ADMIN_URL}/categories`);
  167 |     await page.waitForTimeout(1500);
  168 |     
  169 |     await page.goto(`${ADMIN_URL}/brands`);
  170 |     await page.waitForTimeout(1500);
  171 |     
  172 |     await page.goto(`${ADMIN_URL}/customers`);
  173 |     await page.waitForTimeout(1500);
  174 |     
  175 |     await page.goto(`${ADMIN_URL}/orders`);
  176 |     await page.waitForTimeout(1500);
  177 |     
  178 |     await page.goto(`${ADMIN_URL}/inventory`);
  179 |     await page.waitForTimeout(1500);
  180 |     
  181 |     const currentUrl = page.url();
  182 |     expect(currentUrl).not.toContain('/login');
  183 |   });
  184 | 
  185 |   test('categories CRUD works end-to-end', async ({ page }) => {
  186 |     const stamp = Date.now();
  187 |     const categoryCode = `E2E-CAT-${stamp}`;
  188 |     const categoryName = `E2E Category ${stamp}`;
  189 |     const categoryUpdated = `E2E Category Updated ${stamp}`;
  190 | 
  191 |     await page.goto(`${ADMIN_URL}/categories`);
  192 |     await page.waitForTimeout(1500);
  193 | 
  194 |     await page.getByRole('button', { name: 'List' }).click();
  195 |     await page.waitForTimeout(800);
  196 | 
  197 |     await page.getByRole('button', { name: 'Add Category' }).click();
  198 |     await page.getByPlaceholder('e.g. CAT-01').fill(categoryCode);
  199 |     await page.getByPlaceholder('e.g. Electronics').fill(categoryName);
  200 |     await page.getByRole('button', { name: 'Create Category' }).click();
  201 |     await page.waitForTimeout(1800);
  202 | 
  203 |     await page.getByPlaceholder('Search categories...').fill(categoryName);
  204 |     await page.getByPlaceholder('Search categories...').press('Enter');
  205 |     await page.waitForTimeout(1800);
  206 |     await expect(page.locator('tr', { hasText: categoryName }).first()).toBeVisible();
  207 | 
  208 |     const row = page.locator('tr', { hasText: categoryName }).first();
  209 |     await row.locator('button').nth(1).click();
  210 |     await page.getByPlaceholder('e.g. Electronics').fill(categoryUpdated);
  211 |     await page.getByRole('button', { name: 'Save Changes' }).click();
  212 |     await page.waitForTimeout(1200);
  213 | 
  214 |     await page.getByPlaceholder('Search categories...').fill('');
  215 |     await page.getByPlaceholder('Search categories...').press('Enter');
  216 |     await page.waitForTimeout(700);
  217 |     await page.getByPlaceholder('Search categories...').fill(categoryUpdated);
  218 |     await page.getByPlaceholder('Search categories...').press('Enter');
  219 |     await page.waitForTimeout(1800);
> 220 |     await expect(page.locator('tr', { hasText: categoryUpdated }).first()).toBeVisible();
      |                                                                            ^ Error: expect(locator).toBeVisible() failed
  221 | 
  222 |     const rowAfterUpdate = page.locator('tr', { hasText: categoryUpdated }).first();
  223 |     await rowAfterUpdate.locator('button').nth(2).click();
  224 |     await page.locator('div:has-text("Delete Category")').getByRole('button', { name: 'Delete' }).click();
  225 |     await page.waitForTimeout(1200);
  226 | 
  227 |     await page.getByPlaceholder('Search categories...').fill(categoryUpdated);
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
```