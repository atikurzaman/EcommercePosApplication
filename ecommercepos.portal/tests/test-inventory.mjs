// test-inventory.mjs — Stock Items, Movements, Transfers, Adjustments, Reorder Rules
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

export async function testInventory(auth = {}) {
  console.log('\n📦 INVENTORY TESTS');

  // ─── STOCK ITEMS ───
  console.log('\n-- Stock Items --');
  const siList = await f('/stock-items', { headers: auth });
  ok(`GET /stock-items: ${siList.status}`, siList.status === 200);

  const siLow = await f('/stock-items/low-stock', { headers: auth });
  ok(`GET /stock-items/low-stock: ${siLow.status}`, siLow.status === 200);

  const stockItems = siList.data?.data || [];
  if (stockItems.length > 0) {
    const siId = stockItems[0].id;
    const siGet = await f(`/stock-items/${siId}`, { headers: auth });
    ok(`GET /stock-items/${siId}: ${siGet.status}`, siGet.status === 200);

    const siReorder = await f(`/stock-items/${siId}/reorder-level`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify(10),
    });
    ok(`PUT reorder-level: ${siReorder.status}`, siReorder.status === 200 || siReorder.status === 204);
  } else {
    ok('No stock items to test', true);
  }

  // ─── STOCK MOVEMENTS ───
  console.log('\n-- Stock Movements --');
  const smList = await f('/stock-movements', { headers: auth });
  ok(`GET /stock-movements: ${smList.status}`, smList.status === 200);

  const smTypes = await f('/stock-movements/types', { headers: auth });
  ok(`GET /stock-movements/types: ${smTypes.status}`, smTypes.status === 200);

  // ─── STOCK TRANSFERS ───
  console.log('\n-- Stock Transfers --');
  const stList = await f('/stock-transfers', { headers: auth });
  ok(`GET /stock-transfers: ${stList.status}`, stList.status === 200);

  const transfers = stList.data?.data || [];
  if (transfers.length > 0) {
    const stId = transfers[0].id;
    const stGet = await f(`/stock-transfers/${stId}`, { headers: auth });
    ok(`GET /stock-transfers/${stId}: ${stGet.status}`, stGet.status === 200);
  } else {
    ok('No transfers to test GET by ID', true);
  }

  // ─── INVENTORY ADJUSTMENTS ───
  console.log('\n-- Inventory Adjustments --');
  const iaList = await f('/inventory-adjustments', { headers: auth });
  ok(`GET /inventory-adjustments: ${iaList.status}`, iaList.status === 200);

  const adjustments = iaList.data?.data || [];
  if (adjustments.length > 0) {
    const iaId = adjustments[0].id;
    const iaGet = await f(`/inventory-adjustments/${iaId}`, { headers: auth });
    ok(`GET /inventory-adjustments/${iaId}: ${iaGet.status}`, iaGet.status === 200);
  } else {
    ok('No adjustments to test GET by ID', true);
  }

  // ─── REORDER RULES ───
  console.log('\n-- Reorder Rules --');
  const rrList = await f('/reorder-rules', { headers: auth });
  ok(`GET /reorder-rules: ${rrList.status}`, rrList.status === 200);

  const rules = rrList.data?.data || [];
  if (rules.length > 0) {
    const rrId = rules[0].id;
    const rrGet = await f(`/reorder-rules/${rrId}`, { headers: auth });
    ok(`GET /reorder-rules/${rrId}: ${rrGet.status}`, rrGet.status === 200);
  } else {
    ok('No reorder rules to test', true);
  }
}

if (process.argv[1]?.includes('test-inventory')) {
  testInventory().then(() => console.log('\nInventory tests done.'));
}
