// test-all.mjs — Master test runner for all UI pages and forms
// Usage: node test-all.mjs
//
// Safe data extraction pattern used throughout:
//   const id = response.data?.data?.id || response.data?.id;
// This handles both { data: { id } } and { data: { data: { id } } } response shapes.

import { testAuth } from './test-auth.mjs';
import { testCatalog } from './test-catalog.mjs';
import { testRbac } from './test-rbac.mjs';
import { testCustomersOrders } from './test-customers-orders.mjs';
import { testPos } from './test-pos.mjs';
import { testInventory } from './test-inventory.mjs';
import { testLookups } from './test-lookups.mjs';
import { testShipping } from './test-shipping.mjs';

const BASE = 'http://localhost:5142/api';

async function run() {
  console.log('╔══════════════════════════════════════════════╗');
  console.log('║   EcommercePos — Full API Test Suite         ║');
  console.log('║   Testing all UI forms & pages               ║');
  console.log('╚══════════════════════════════════════════════╝');
  console.log(`\nBase URL: ${BASE}`);
  console.log(`Timestamp: ${new Date().toISOString()}\n`);

  // Check API is alive
  try {
    const health = await fetch(`${BASE}/categories`);
    if (!health.ok) throw new Error(`Status ${health.status}`);
  } catch (e) {
    console.error(`❌ API not reachable at ${BASE}: ${e.message}`);
    console.error('Start the API first: dotnet run --project EcommercePos.Api');
    process.exit(1);
  }
  console.log('✅ API is reachable\n');

  const results = {};
  const startTime = Date.now();

  // 1. Auth — get token for authenticated tests
  try {
    const authResult = await testAuth();
    results.auth = '✅';
    var auth = authResult.auth || {};
  } catch (e) {
    console.error('  ❌ Auth tests failed:', e.message);
    results.auth = '❌';
    var auth = {};
  }

  // 2. Catalog (Products, Categories, Brands, Tags, Units, Suppliers, Attributes, Collections)
  try {
    await testCatalog(auth);
    results.catalog = '✅';
  } catch (e) {
    console.error('  ❌ Catalog tests failed:', e.message);
    results.catalog = '❌';
  }

  // 3. RBAC (Roles, Permissions, Menus, Users)
  try {
    await testRbac(auth);
    results.rbac = '✅';
  } catch (e) {
    console.error('  ❌ RBAC tests failed:', e.message);
    results.rbac = '❌';
  }

  // 4. Customers & Orders
  try {
    await testCustomersOrders(auth);
    results.customersOrders = '✅';
  } catch (e) {
    console.error('  ❌ Customers/Orders tests failed:', e.message);
    results.customersOrders = '❌';
  }

  // 5. POS (Warehouses, Counters, Terminals, Shifts, Transactions, Expenses)
  try {
    await testPos(auth);
    results.pos = '✅';
  } catch (e) {
    console.error('  ❌ POS tests failed:', e.message);
    results.pos = '❌';
  }

  // 6. Inventory (Stock Items, Movements, Transfers, Adjustments)
  try {
    await testInventory(auth);
    results.inventory = '✅';
  } catch (e) {
    console.error('  ❌ Inventory tests failed:', e.message);
    results.inventory = '❌';
  }

  // 7. Lookups (Colors, Currencies, Tiers, Payment Methods, Statuses, etc.)
  try {
    await testLookups(auth);
    results.lookups = '✅';
  } catch (e) {
    console.error('  ❌ Lookup tests failed:', e.message);
    results.lookups = '❌';
  }

  // 8. Shipping (Methods, Delivery Zones, Pickup Points)
  try {
    await testShipping(auth);
    results.shipping = '✅';
  } catch (e) {
    console.error('  ❌ Shipping tests failed:', e.message);
    results.shipping = '❌';
  }

  // Summary
  const elapsed = ((Date.now() - startTime) / 1000).toFixed(1);
  console.log('\n╔══════════════════════════════════════════════╗');
  console.log('║              TEST SUMMARY                    ║');
  console.log('╠══════════════════════════════════════════════╣');
  for (const [suite, status] of Object.entries(results)) {
    console.log(`║  ${status} ${suite.padEnd(40)}║`);
  }
  console.log('╠══════════════════════════════════════════════╣');
  console.log(`║  Completed in ${elapsed}s`.padEnd(47) + '║');
  console.log('╚══════════════════════════════════════════════╝');
}

run().catch(e => {
  console.error('Fatal error:', e);
  process.exit(1);
});
