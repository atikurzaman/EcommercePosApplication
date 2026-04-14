// test-lookups.mjs — All lookup tables (code-based PKs)
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

async function testCodeLookup(endpoint, codeField, createBody, updateBody) {
  console.log(`\n-- ${endpoint} --`);
  const list = await f(`/${endpoint}`);
  ok(`GET /${endpoint}: ${list.status}`, list.status === 200);

  const create = await f(`/${endpoint}`, {
    method: 'POST',
    body: JSON.stringify(createBody),
  });
  const code = create.data?.data?.[codeField] || create.data?.[codeField] || createBody[codeField];
  ok(`POST /${endpoint}: ${create.status} code=${code}`, create.status === 200 || create.status === 201);

  if (code) {
    const get = await f(`/${endpoint}/${code}`);
    ok(`GET /${endpoint}/${code}: ${get.status}`, get.status === 200);

    const update = await f(`/${endpoint}/${code}`, {
      method: 'PUT',
      body: JSON.stringify(updateBody),
    });
    ok(`PUT /${endpoint}/${code}: ${update.status}`, update.status === 200 || update.status === 204);

    const del = await f(`/${endpoint}/${code}`, { method: 'DELETE' });
    ok(`DELETE /${endpoint}/${code}: ${del.status}`, del.status === 200 || del.status === 204);
  }
}

async function testGuidLookup(endpoint, createBody, updateBody) {
  console.log(`\n-- ${endpoint} --`);
  const list = await f(`/${endpoint}`);
  ok(`GET /${endpoint}: ${list.status}`, list.status === 200);

  const create = await f(`/${endpoint}`, {
    method: 'POST',
    body: JSON.stringify(createBody),
  });
  const id = create.data?.data?.id || create.data?.id;
  ok(`POST /${endpoint}: ${create.status} id=${id}`, create.status === 200 || create.status === 201);

  if (id) {
    const get = await f(`/${endpoint}/${id}`);
    ok(`GET /${endpoint}/${id}: ${get.status}`, get.status === 200);

    const update = await f(`/${endpoint}/${id}`, {
      method: 'PUT',
      body: JSON.stringify(updateBody),
    });
    ok(`PUT /${endpoint}/${id}: ${update.status}`, update.status === 200 || update.status === 204);

    const del = await f(`/${endpoint}/${id}`, { method: 'DELETE' });
    ok(`DELETE /${endpoint}/${id}: ${del.status}`, del.status === 200 || del.status === 204);
  }
}

export async function testLookups(auth = {}) {
  console.log('\n📋 LOOKUP TESTS');

  // Colors (Guid PK)
  await testGuidLookup('colors',
    { name: `TestColor ${ts}`, hexCode: '#FF0000', isActive: true },
    { name: `TestColor ${ts} Upd`, hexCode: '#00FF00', isActive: true },
  );

  // Currencies (code PK)
  await testCodeLookup('currencies', 'currencyCode',
    { currencyCode: `T${ts}`.slice(0, 3).toUpperCase(), name: `TestCur ${ts}`, symbol: '¤', exchangeRate: 1.0, decimalPlaces: 2, isBaseCurrency: false, isActive: true },
    { currencyCode: `T${ts}`.slice(0, 3).toUpperCase(), name: `TestCur ${ts} Upd`, symbol: '¤', exchangeRate: 1.1, decimalPlaces: 2, isBaseCurrency: false, isActive: true },
  );

  // Customer Tiers
  await testCodeLookup('customer-tiers', 'tierCode',
    { tierCode: `TT${ts}`.slice(0, 8), displayName: `Tier ${ts}`, minLifetimeSpend: 0, discountPct: 5, pointsMultiplier: 1, sortOrder: 99 },
    { tierCode: `TT${ts}`.slice(0, 8), displayName: `Tier ${ts} Upd`, minLifetimeSpend: 100, discountPct: 10, pointsMultiplier: 2, sortOrder: 98 },
  );

  // Payment Methods
  await testCodeLookup('payment-methods', 'methodCode',
    { methodCode: `PM${ts}`.slice(0, 10), displayName: `Pay ${ts}`, isOnline: false, isActive: true, sortOrder: 99 },
    { methodCode: `PM${ts}`.slice(0, 10), displayName: `Pay ${ts} Upd`, isOnline: true, isActive: true, sortOrder: 98 },
  );

  // Order Statuses
  await testCodeLookup('order-statuses', 'statusCode',
    { statusCode: `OS${ts}`.slice(0, 10), displayName: `OrdStat ${ts}`, description: 'E2E', sortOrder: 99, isTerminal: false },
    { statusCode: `OS${ts}`.slice(0, 10), displayName: `OrdStat ${ts} Upd`, description: 'Updated', sortOrder: 98, isTerminal: true },
  );

  // Payment Statuses
  await testCodeLookup('payment-statuses', 'statusCode',
    { statusCode: `PS${ts}`.slice(0, 10), displayName: `PayStat ${ts}` },
    { statusCode: `PS${ts}`.slice(0, 10), displayName: `PayStat ${ts} Upd` },
  );

  // Shipment Statuses
  await testCodeLookup('shipment-statuses', 'statusCode',
    { statusCode: `SS${ts}`.slice(0, 10), displayName: `ShipStat ${ts}`, sortOrder: 99 },
    { statusCode: `SS${ts}`.slice(0, 10), displayName: `ShipStat ${ts} Upd`, sortOrder: 98 },
  );

  // Return Statuses
  await testCodeLookup('return-statuses', 'statusCode',
    { statusCode: `RS${ts}`.slice(0, 10), displayName: `RetStat ${ts}`, sortOrder: 99 },
    { statusCode: `RS${ts}`.slice(0, 10), displayName: `RetStat ${ts} Upd`, sortOrder: 98 },
  );

  // Discount Types
  await testCodeLookup('discount-types', 'typeCode',
    { typeCode: `DT${ts}`.slice(0, 10), displayName: `Disc ${ts}` },
    { typeCode: `DT${ts}`.slice(0, 10), displayName: `Disc ${ts} Upd` },
  );

  // Product Conditions
  await testCodeLookup('product-conditions', 'conditionCode',
    { conditionCode: `PC${ts}`.slice(0, 10), displayName: `Cond ${ts}` },
    { conditionCode: `PC${ts}`.slice(0, 10), displayName: `Cond ${ts} Upd` },
  );

  // Stock Movement Types
  await testCodeLookup('stock-movement-types', 'typeCode',
    { typeCode: `SM${ts}`.slice(0, 10), displayName: `StMov ${ts}`, isInbound: true },
    { typeCode: `SM${ts}`.slice(0, 10), displayName: `StMov ${ts} Upd`, isInbound: false },
  );

  // Wishlist Types
  await testCodeLookup('wishlist-types', 'typeCode',
    { typeCode: `WL${ts}`.slice(0, 10), displayName: `Wish ${ts}` },
    { typeCode: `WL${ts}`.slice(0, 10), displayName: `Wish ${ts} Upd` },
  );

  // Tax Rates (Guid PK)
  console.log(`\n-- tax-rates --`);
  const trList = await f('/tax-rates');
  ok(`GET /tax-rates: ${trList.status}`, trList.status === 200);

  const trCreate = await f('/tax-rates', {
    method: 'POST',
    body: JSON.stringify({
      taxName: `Tax ${ts}`, rate: 15, taxCode: `TX${ts}`.slice(0, 10), description: 'E2E',
      isActive: true, taxType: 'VAT', isPercentage: true, isInclusive: false,
      isDefault: false, country: 'BD', applyToShipping: false, priority: 1,
      effectiveFrom: null, effectiveTo: null,
    }),
  });
  const trId = trCreate.data?.data?.id || trCreate.data?.id;
  ok(`POST /tax-rates: ${trCreate.status} id=${trId}`, (trCreate.status === 200 || trCreate.status === 201) && trId);

  if (trId) {
    const trGet = await f(`/tax-rates/${trId}`);
    ok(`GET /tax-rates/${trId}: ${trGet.status}`, trGet.status === 200);

    const trUpdate = await f(`/tax-rates/${trId}`, {
      method: 'PUT',
      body: JSON.stringify({
        taxName: `Tax ${ts} Upd`, rate: 10, taxCode: `TX${ts}`.slice(0, 10), description: 'Updated',
        isActive: true, taxType: 'VAT', isPercentage: true, isInclusive: true,
        isDefault: false, country: 'BD', applyToShipping: true, priority: 2,
        effectiveFrom: null, effectiveTo: null,
      }),
    });
    ok(`PUT /tax-rates/${trId}: ${trUpdate.status}`, trUpdate.status === 200 || trUpdate.status === 204);

    const trDel = await f(`/tax-rates/${trId}`, { method: 'DELETE' });
    ok(`DELETE /tax-rates/${trId}: ${trDel.status}`, trDel.status === 200 || trDel.status === 204);
  }
}

if (process.argv[1]?.includes('test-lookups')) {
  testLookups().then(() => console.log('\nLookup tests done.'));
}
