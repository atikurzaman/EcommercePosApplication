const { execSync } = require('child_process');
const path = require('path');

const tests = [
  'auth.test.js',
  'catalog.test.js',
  'rbac.test.js',
  'customers.test.js',
  'pos.test.js',
  'inventory.test.js',
  'lookups.test.js',
];

console.log('╔══════════════════════════════════════════════╗');
console.log('║   EcommercePos — Selenium E2E Test Suite     ║');
console.log('╚══════════════════════════════════════════════╝');
console.log(`\nRunning ${tests.length} test suites sequentially...\n`);

const results = {};
const start = Date.now();

for (const test of tests) {
  const name = test.replace('.test.js', '');
  try {
    execSync(`node ${path.join(__dirname, test)}`, { stdio: 'inherit', timeout: 180000 });
    results[name] = '✅';
  } catch {
    results[name] = '❌';
  }
}

const elapsed = ((Date.now() - start) / 1000).toFixed(1);
console.log('\n╔══════════════════════════════════════════════╗');
console.log('║              TEST SUMMARY                    ║');
console.log('╠══════════════════════════════════════════════╣');
for (const [suite, status] of Object.entries(results)) {
  console.log(`║  ${status} ${suite.padEnd(40)}║`);
}
console.log('╠══════════════════════════════════════════════╣');
console.log(`║  Completed in ${elapsed}s`.padEnd(47) + '║');
console.log('╚══════════════════════════════════════════════╝');
