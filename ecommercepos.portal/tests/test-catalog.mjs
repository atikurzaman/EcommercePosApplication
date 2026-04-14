// test-catalog.mjs — Products, Categories, Brands, Collections, Attributes, Tags, Units, Suppliers, Specifications
const BASE = 'http://localhost:5142/api';
const ts = Date.now();

const f = (url, opts = {}) => {
  const { headers, ...rest } = opts;
  return fetch(`${BASE}${url}`, {
    ...rest,
    headers: { 'Content-Type': 'application/json', ...headers },
  }).then(async r => ({ status: r.status, data: await r.json().catch(() => null) }));
};

const ok = (label, cond) => console.log(cond ? `  ✅ ${label}` : `  ❌ ${label}`);

export async function testCatalog(auth = {}) {
  console.log('\n📦 CATALOG TESTS');

  // ─── CATEGORIES ───
  console.log('\n-- Categories --');
  const catList = await f('/categories', { headers: auth });
  ok(`GET /categories: ${catList.status}`, catList.status === 200);

  const catTree = await f('/categories/tree', { headers: auth });
  ok(`GET /categories/tree: ${catTree.status}`, catTree.status === 200);

  const catFlat = await f('/categories/flat', { headers: auth });
  ok(`GET /categories/flat: ${catFlat.status}`, catFlat.status === 200);

  const catCreate = await f('/categories', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `Test Cat ${ts}`, slug: `test-cat-${ts}`, description: 'E2E test', imageUrl: '', parentCategoryId: null, displayOrder: 0, isFeatured: false, isActive: true }),
  });
  const catId = catCreate.data?.data?.id || catCreate.data?.id;
  ok(`POST /categories: ${catCreate.status} id=${catId}`, (catCreate.status === 200 || catCreate.status === 201) && catId);

  if (catId) {
    const catGet = await f(`/categories/${catId}`, { headers: auth });
    ok(`GET /categories/${catId}: ${catGet.status}`, catGet.status === 200);

    const catUpdate = await f(`/categories/${catId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `Test Cat ${ts} Updated`, slug: `test-cat-${ts}-upd`, description: 'Updated', imageUrl: '', parentCategoryId: null, displayOrder: 1, isFeatured: true, isActive: true }),
    });
    ok(`PUT /categories/${catId}: ${catUpdate.status}`, catUpdate.status === 200 || catUpdate.status === 204);

    const catToggle = await f(`/categories/${catId}/toggle`, { method: 'PATCH', headers: auth });
    ok(`PATCH /categories/${catId}/toggle: ${catToggle.status}`, catToggle.status === 200 || catToggle.status === 204);

    const catDel = await f(`/categories/${catId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE /categories/${catId}: ${catDel.status}`, catDel.status === 200 || catDel.status === 204);
  }

  // ─── BRANDS ───
  console.log('\n-- Brands --');
  const brandList = await f('/brands', { headers: auth });
  ok(`GET /brands: ${brandList.status}`, brandList.status === 200);

  const brandCount = await f('/brands/with-count', { headers: auth });
  ok(`GET /brands/with-count: ${brandCount.status}`, brandCount.status === 200);

  const brandCreate = await f('/brands', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `Test Brand ${ts}`, description: 'E2E test', logoUrl: '', websiteUrl: '', isActive: true }),
  });
  const brandId = brandCreate.data?.data?.id || brandCreate.data?.id;
  ok(`POST /brands: ${brandCreate.status} id=${brandId}`, (brandCreate.status === 200 || brandCreate.status === 201) && brandId);

  if (brandId) {
    const brandGet = await f(`/brands/${brandId}`, { headers: auth });
    ok(`GET /brands/${brandId}: ${brandGet.status}`, brandGet.status === 200);

    const brandUpdate = await f(`/brands/${brandId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `Test Brand ${ts} Updated`, description: 'Updated', logoUrl: '', websiteUrl: '', isActive: true }),
    });
    ok(`PUT /brands/${brandId}: ${brandUpdate.status}`, brandUpdate.status === 200 || brandUpdate.status === 204);

    const brandToggle = await f(`/brands/${brandId}/toggle`, { method: 'PATCH', headers: auth });
    ok(`PATCH /brands/${brandId}/toggle: ${brandToggle.status}`, brandToggle.status === 200 || brandToggle.status === 204);

    const brandDel = await f(`/brands/${brandId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE /brands/${brandId}: ${brandDel.status}`, brandDel.status === 200 || brandDel.status === 204);
  }

  // ─── TAGS ───
  console.log('\n-- Tags --');
  const tagList = await f('/tags', { headers: auth });
  ok(`GET /tags: ${tagList.status}`, tagList.status === 200);

  const tagCreate = await f('/tags', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `Tag ${ts}`, slug: `tag-${ts}` }),
  });
  const tagId = tagCreate.data?.data?.id || tagCreate.data?.id;
  ok(`POST /tags: ${tagCreate.status} id=${tagId}`, (tagCreate.status === 200 || tagCreate.status === 201) && tagId);

  if (tagId) {
    const tagUpdate = await f(`/tags/${tagId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `Tag ${ts} Upd`, slug: `tag-${ts}-upd` }),
    });
    ok(`PUT /tags/${tagId}: ${tagUpdate.status}`, tagUpdate.status === 200 || tagUpdate.status === 204);

    const tagDel = await f(`/tags/${tagId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE /tags/${tagId}: ${tagDel.status}`, tagDel.status === 200 || tagDel.status === 204);
  }

  // ─── UNITS ───
  console.log('\n-- Units --');
  const unitList = await f('/units', { headers: auth });
  ok(`GET /units: ${unitList.status}`, unitList.status === 200);

  const unitCreate = await f('/units', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `Unit ${ts}`, shortName: `u${ts}`, description: 'E2E test', baseUnitId: null, conversionFactor: null, isActive: true }),
  });
  const unitId = unitCreate.data?.data?.id || unitCreate.data?.id;
  ok(`POST /units: ${unitCreate.status} id=${unitId}`, (unitCreate.status === 200 || unitCreate.status === 201) && unitId);

  if (unitId) {
    const unitGet = await f(`/units/${unitId}`, { headers: auth });
    ok(`GET /units/${unitId}: ${unitGet.status}`, unitGet.status === 200);

    const unitUpdate = await f(`/units/${unitId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `Unit ${ts} Upd`, shortName: `u${ts}u`, description: 'Updated', baseUnitId: null, conversionFactor: null, isActive: true }),
    });
    ok(`PUT /units/${unitId}: ${unitUpdate.status}`, unitUpdate.status === 200 || unitUpdate.status === 204);

    const unitDel = await f(`/units/${unitId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE /units/${unitId}: ${unitDel.status}`, unitDel.status === 200 || unitDel.status === 204);
  }

  // ─── SUPPLIERS ───
  console.log('\n-- Suppliers --');
  const supList = await f('/suppliers', { headers: auth });
  ok(`GET /suppliers: ${supList.status}`, supList.status === 200);

  const supCreate = await f('/suppliers', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `Supplier ${ts}`, contactPerson: 'John', email: `sup${ts}@test.com`, phone: '01700000000', address: '123 Test St', city: 'Dhaka', country: 'BD', isActive: true }),
  });
  const supId = supCreate.data?.data?.id || supCreate.data?.id;
  ok(`POST /suppliers: ${supCreate.status} id=${supId}`, (supCreate.status === 200 || supCreate.status === 201) && supId);

  if (supId) {
    const supGet = await f(`/suppliers/${supId}`, { headers: auth });
    ok(`GET /suppliers/${supId}: ${supGet.status}`, supGet.status === 200);

    const supUpdate = await f(`/suppliers/${supId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `Supplier ${ts} Upd`, contactPerson: 'Jane', email: `sup${ts}@test.com`, phone: '01700000001', address: '456 Test St', city: 'Dhaka', country: 'BD', isActive: true }),
    });
    ok(`PUT /suppliers/${supId}: ${supUpdate.status}`, supUpdate.status === 200 || supUpdate.status === 204);

    const supDel = await f(`/suppliers/${supId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE /suppliers/${supId}: ${supDel.status}`, supDel.status === 200 || supDel.status === 204);
  }

  // ─── ATTRIBUTE TYPES ───
  console.log('\n-- Attribute Types --');
  const attrList = await f('/attribute-types', { headers: auth });
  ok(`GET /attribute-types: ${attrList.status}`, attrList.status === 200);

  const attrCreate = await f('/attribute-types', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `Attr ${ts}`, displayName: `Attribute ${ts}`, dataType: 'Text', isRequired: false, isFilterable: true, isActive: true }),
  });
  const attrId = attrCreate.data?.data?.id || attrCreate.data?.id;
  ok(`POST /attribute-types: ${attrCreate.status} id=${attrId}`, (attrCreate.status === 200 || attrCreate.status === 201) && attrId);

  if (attrId) {
    const attrGet = await f(`/attribute-types/${attrId}`, { headers: auth });
    ok(`GET /attribute-types/${attrId}: ${attrGet.status}`, attrGet.status === 200);

    const attrUpdate = await f(`/attribute-types/${attrId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `Attr ${ts} Upd`, slug: `attr-${ts}-upd`, uiType: 'Dropdown', affectsPrice: false, affectsSku: false, affectsImage: false, affectsStock: false, isFilterable: true, sortOrder: 1 }),
    });
    ok(`PUT /attribute-types/${attrId}: ${attrUpdate.status}`, attrUpdate.status === 200 || attrUpdate.status === 204);

    // Attribute options
    console.log('\n-- Attribute Options --');
    const optCreate = await f(`/attribute-types/${attrId}/options`, {
      method: 'POST', headers: auth,
      body: JSON.stringify({ value: `Opt ${ts}`, label: `Option ${ts}`, sortOrder: 1, isActive: true }),
    });
    const optId = optCreate.data?.data?.id || optCreate.data?.id;
    ok(`POST options: ${optCreate.status} id=${optId}`, (optCreate.status === 200 || optCreate.status === 201) && optId);

    const optBulk = await f(`/attribute-types/${attrId}/options/bulk`, {
      method: 'POST', headers: auth,
      body: JSON.stringify([
        { value: `BulkA ${ts}`, label: `Bulk A ${ts}`, sortOrder: 2, isActive: true },
        { value: `BulkB ${ts}`, label: `Bulk B ${ts}`, sortOrder: 3, isActive: true },
      ]),
    });
    ok(`POST options/bulk: ${optBulk.status}`, optBulk.status === 200 || optBulk.status === 201);

    const optList = await f(`/attribute-types/${attrId}/options`, { headers: auth });
    ok(`GET options: ${optList.status}`, optList.status === 200);

    if (optId) {
      const optUpdate = await f(`/attribute-types/${attrId}/options/${optId}`, {
        method: 'PUT', headers: auth,
        body: JSON.stringify({ value: `Opt ${ts} Upd`, label: `Option ${ts} Upd`, sortOrder: 1, isActive: true }),
      });
      ok(`PUT option: ${optUpdate.status}`, optUpdate.status === 200 || optUpdate.status === 204);

      const optDel = await f(`/attribute-types/${attrId}/options/${optId}`, { method: 'DELETE', headers: auth });
      ok(`DELETE option: ${optDel.status}`, optDel.status === 200 || optDel.status === 204);
    }

    const attrDel = await f(`/attribute-types/${attrId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE /attribute-types/${attrId}: ${attrDel.status}`, attrDel.status === 200 || attrDel.status === 204);
  }

  // ─── SPECIFICATIONS ───
  console.log('\n-- Specifications --');
  const specList = await f('/specifications', { headers: auth });
  ok(`GET /specifications: ${specList.status}`, specList.status === 200);

  const specCreate = await f('/specifications', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ specName: `Spec ${ts}`, sortOrder: 1 }),
  });
  const specId = specCreate.data?.data?.id || specCreate.data?.id;
  ok(`POST /specifications: ${specCreate.status} id=${specId}`, (specCreate.status === 200 || specCreate.status === 201) && specId);

  // ─── PRODUCTS ───
  console.log('\n-- Products --');
  const prodList = await f('/products', { headers: auth });
  ok(`GET /products: ${prodList.status}`, prodList.status === 200);

  const prodStats = await f('/products/stats', { headers: auth });
  ok(`GET /products/stats: ${prodStats.status}`, prodStats.status === 200);

  const prodTypes = await f('/products/types', { headers: auth });
  ok(`GET /products/types: ${prodTypes.status}`, prodTypes.status === 200);

  // Need a category for product creation
  const tempCat = await f('/categories', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `ProdCat ${ts}`, slug: `prodcat-${ts}`, description: '', imageUrl: '', parentCategoryId: null, displayOrder: 0, isFeatured: false, isActive: true }),
  });
  const tempCatId = tempCat.data?.data?.id || tempCat.data?.id;

  if (tempCatId) {
    const prodCreate = await f('/products', {
      method: 'POST', headers: auth,
      body: JSON.stringify({
        name: `Test Product ${ts}`, sku: `SKU-${ts}`, barcode: `BAR-${ts}`,
        shortDescription: 'E2E test product', description: 'Full description',
        productType: 'Standard', costPrice: 100, salePrice: 150, originalPrice: 200,
        isTaxInclusive: false, weightKg: 0.5, isFeatured: false, isActive: true,
        categoryId: tempCatId, brandId: null, unitId: null,
      }),
    });
    const prodId = prodCreate.data?.data?.id || prodCreate.data?.id;
    ok(`POST /products: ${prodCreate.status} id=${prodId}`, (prodCreate.status === 200 || prodCreate.status === 201) && prodId);

    if (prodId) {
      const prodGet = await f(`/products/${prodId}`, { headers: auth });
      ok(`GET /products/${prodId}: ${prodGet.status}`, prodGet.status === 200);

      const prodUpdate = await f(`/products/${prodId}`, {
        method: 'PUT', headers: auth,
        body: JSON.stringify({
          name: `Test Product ${ts} Upd`, sku: `SKU-${ts}`, barcode: `BAR-${ts}`,
          shortDescription: 'Updated', description: 'Updated desc',
          productType: 'Standard', costPrice: 110, salePrice: 160, originalPrice: 200,
          isTaxInclusive: false, weightKg: 0.5, isFeatured: true, isActive: true,
          categoryId: tempCatId, brandId: null, unitId: null,
        }),
      });
      ok(`PUT /products/${prodId}: ${prodUpdate.status}`, prodUpdate.status === 200 || prodUpdate.status === 204);

      const prodToggle = await f(`/products/${prodId}/toggle-featured`, { method: 'POST', headers: auth });
      ok(`POST toggle-featured: ${prodToggle.status}`, prodToggle.status === 200 || prodToggle.status === 204);

      // Product sub-resources
      console.log('\n-- Product Variants --');
      const varList = await f(`/products/${prodId}/variants`, { headers: auth });
      ok(`GET variants: ${varList.status}`, varList.status === 200);

      const varCreate = await f(`/products/${prodId}/variants`, {
        method: 'POST', headers: auth,
        body: JSON.stringify({ sku: `VAR-${ts}`, name: `Variant ${ts}`, priceAdjustment: 10, stockQuantity: 50, isActive: true }),
      });
      const varId = varCreate.data?.data?.id || varCreate.data?.id;
      ok(`POST variant: ${varCreate.status} id=${varId}`, (varCreate.status === 200 || varCreate.status === 201));

      if (varId) {
        const varUpdate = await f(`/products/${prodId}/variants/${varId}`, {
          method: 'PUT', headers: auth,
          body: JSON.stringify({ sku: `VAR-${ts}-U`, name: `Variant ${ts} Upd`, priceAdjustment: 15, stockQuantity: 60, isActive: true }),
        });
        ok(`PUT variant: ${varUpdate.status}`, varUpdate.status === 200 || varUpdate.status === 204);

        const varDel = await f(`/products/${prodId}/variants/${varId}`, { method: 'DELETE', headers: auth });
        ok(`DELETE variant: ${varDel.status}`, varDel.status === 200 || varDel.status === 204);
      }

      console.log('\n-- Product Images --');
      const imgList = await f(`/products/${prodId}/images`, { headers: auth });
      ok(`GET images: ${imgList.status}`, imgList.status === 200);

      const imgCreate = await f(`/products/${prodId}/images`, {
        method: 'POST', headers: auth,
        body: JSON.stringify({ imageUrl: 'https://example.com/img.jpg', altText: 'Test', sortOrder: 1, isPrimary: true, variantId: null }),
      });
      const imgId = imgCreate.data?.data?.id || imgCreate.data?.id;
      ok(`POST image: ${imgCreate.status} id=${imgId}`, (imgCreate.status === 200 || imgCreate.status === 201));

      if (imgId) {
        const imgUpdate = await f(`/products/${prodId}/images/${imgId}`, {
          method: 'PUT', headers: auth,
          body: JSON.stringify({ altText: 'Updated', sortOrder: 2, isPrimary: true }),
        });
        ok(`PUT image: ${imgUpdate.status}`, imgUpdate.status === 200 || imgUpdate.status === 204);

        const imgDel = await f(`/products/${prodId}/images/${imgId}`, { method: 'DELETE', headers: auth });
        ok(`DELETE image: ${imgDel.status}`, imgDel.status === 200 || imgDel.status === 204);
      }

      console.log('\n-- Product Tags --');
      const pTagList = await f(`/products/${prodId}/tags`, { headers: auth });
      ok(`GET product tags: ${pTagList.status}`, pTagList.status === 200);

      if (tagId) {
        const pTagManage = await f(`/products/${prodId}/tags`, {
          method: 'PUT', headers: auth,
          body: JSON.stringify([tagId]),
        });
        ok(`PUT manage tags: ${pTagManage.status}`, pTagManage.status === 200 || pTagManage.status === 204);
      }

      console.log('\n-- Product Specifications --');
      const pSpecList = await f(`/products/${prodId}/specifications`, { headers: auth });
      ok(`GET product specs: ${pSpecList.status}`, pSpecList.status === 200);

      console.log('\n-- Product Attributes --');
      const pAttrList = await f(`/products/${prodId}/attributes`, { headers: auth });
      ok(`GET product attributes: ${pAttrList.status}`, pAttrList.status === 200);

      console.log('\n-- Product Suppliers --');
      const pSupList = await f(`/products/${prodId}/suppliers`, { headers: auth });
      ok(`GET product suppliers: ${pSupList.status}`, pSupList.status === 200);

      if (supId) {
        const pSupAdd = await f(`/products/${prodId}/suppliers`, {
          method: 'POST', headers: auth,
          body: JSON.stringify({ supplierId: supId, supplierSku: `SSSKU-${ts}`, unitCost: 80, leadTimeDays: 7, isPreferred: true, isActive: true }),
        });
        const pSupLinkId = pSupAdd.data?.data?.id || pSupAdd.data?.id;
        ok(`POST product supplier: ${pSupAdd.status}`, pSupAdd.status === 200 || pSupAdd.status === 201);

        if (pSupLinkId) {
          const pSupUpd = await f(`/products/${prodId}/suppliers/${pSupLinkId}`, {
            method: 'PUT', headers: auth,
            body: JSON.stringify({ supplierSku: `SSSKU-${ts}-U`, unitCost: 85, leadTimeDays: 5, isPreferred: true, isActive: true }),
          });
          ok(`PUT product supplier: ${pSupUpd.status}`, pSupUpd.status === 200 || pSupUpd.status === 204);

          const pSupDel = await f(`/products/${prodId}/suppliers/${pSupLinkId}`, { method: 'DELETE', headers: auth });
          ok(`DELETE product supplier: ${pSupDel.status}`, pSupDel.status === 200 || pSupDel.status === 204);
        }
      }

      console.log('\n-- Product Price History --');
      const priceHist = await f(`/products/${prodId}/price-history`, { headers: auth });
      ok(`GET price history: ${priceHist.status}`, priceHist.status === 200);

      // Cleanup product
      const prodDel = await f(`/products/${prodId}`, { method: 'DELETE', headers: auth });
      ok(`DELETE /products/${prodId}: ${prodDel.status}`, prodDel.status === 200 || prodDel.status === 204);
    }

    // Cleanup temp category
    await f(`/categories/${tempCatId}`, { method: 'DELETE', headers: auth });
  }

  // ─── PRODUCT COLLECTIONS ───
  console.log('\n-- Product Collections --');
  const collList = await f('/product-collections', { headers: auth });
  ok(`GET /product-collections: ${collList.status}`, collList.status === 200);

  const collCreate = await f('/product-collections', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `Collection ${ts}`, displayOrder: 1, isActive: true, showInHomePage: false }),
  });
  const collId = collCreate.data?.data?.id || collCreate.data?.id;
  ok(`POST /product-collections: ${collCreate.status} id=${collId}`, (collCreate.status === 200 || collCreate.status === 201) && collId);

  if (collId) {
    const collGet = await f(`/product-collections/${collId}`, { headers: auth });
    ok(`GET /product-collections/${collId}: ${collGet.status}`, collGet.status === 200);

    const collUpdate = await f(`/product-collections/${collId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `Collection ${ts} Upd`, displayOrder: 2, isActive: true, showInHomePage: true }),
    });
    ok(`PUT collection: ${collUpdate.status}`, collUpdate.status === 200 || collUpdate.status === 204);

    const collItems = await f(`/product-collections/${collId}/items`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify([]),
    });
    ok(`PUT collection items: ${collItems.status}`, collItems.status === 200 || collItems.status === 204);

    const collDel = await f(`/product-collections/${collId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE collection: ${collDel.status}`, collDel.status === 200 || collDel.status === 204);
  }

  // Collections (storefront)
  console.log('\n-- Collections (Storefront) --');
  const sfColl = await f('/collections', { headers: auth });
  ok(`GET /collections: ${sfColl.status}`, sfColl.status === 200);

  const sfCollHome = await f('/collections/home', { headers: auth });
  ok(`GET /collections/home: ${sfCollHome.status}`, sfCollHome.status === 200);

  return { catId, brandId, tagId, unitId, supId };
}

if (process.argv[1]?.includes('test-catalog')) {
  testCatalog().then(() => console.log('\nCatalog tests done.'));
}
