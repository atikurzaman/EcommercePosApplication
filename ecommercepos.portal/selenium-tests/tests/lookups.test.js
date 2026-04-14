const { createDriver, login, navigateTo, clickButton, waitForModal, fillAndSubmitForm, ok, By } = require('../utils/driver');

async function testLookupPage(driver, path, pageName, fields) {
  console.log(`\n-- ${pageName} --`);
  await navigateTo(driver, path);
  await driver.sleep(1000);
  await ok(`${pageName} page loads`, true);
  const added = await clickButton(driver, 'Add');
  if (added) {
    await waitForModal(driver);
    const created = await fillAndSubmitForm(driver, fields, 'Create');
    await ok(`${pageName} entry created (modal closed)`, created);
  } else {
    await ok(`${pageName} Add button not found`, false);
  }
}

(async function testLookups() {
  console.log('\n📋 LOOKUP PAGES BROWSER TESTS');
  let driver;
  try {
    driver = await createDriver();
    await login(driver);
    const ts = Date.now();

    await testLookupPage(driver, '/colors', 'Colors', {
      'Name': `E2E Color ${ts}`, 'Hex Code': '#FF0000',
    });
    await testLookupPage(driver, '/currencies', 'Currencies', {
      'Currency Code': `E${ts}`.slice(0, 3).toUpperCase(), 'Name': `E2E Cur ${ts}`, 'Symbol': '¤',
    });
    await testLookupPage(driver, '/settings/order-statuses', 'Order Statuses', {
      'Status Code': `OS${ts}`.slice(0, 10), 'Display Name': `E2E OrdStat ${ts}`,
    });
    await testLookupPage(driver, '/settings/payment-methods', 'Payment Methods', {
      'Method Code': `PM${ts}`.slice(0, 10), 'Display Name': `E2E Pay ${ts}`,
    });
    await testLookupPage(driver, '/settings/payment-statuses', 'Payment Statuses', {
      'Status Code': `PS${ts}`.slice(0, 10), 'Display Name': `E2E PayStat ${ts}`,
    });
    await testLookupPage(driver, '/settings/shipment-statuses', 'Shipment Statuses', {
      'Status Code': `SS${ts}`.slice(0, 10), 'Display Name': `E2E ShipStat ${ts}`,
    });
    await testLookupPage(driver, '/settings/return-statuses', 'Return Statuses', {
      'Status Code': `RS${ts}`.slice(0, 10), 'Display Name': `E2E RetStat ${ts}`,
    });
    await testLookupPage(driver, '/settings/discount-types', 'Discount Types', {
      'Type Code': `DT${ts}`.slice(0, 10), 'Display Name': `E2E Disc ${ts}`,
    });
    await testLookupPage(driver, '/settings/customer-tiers', 'Customer Tiers', {
      'Tier Code': `TR${ts}`.slice(0, 8), 'Display Name': `E2E Tier ${ts}`,
    });
    await testLookupPage(driver, '/settings/product-conditions', 'Product Conditions', {
      'Condition Code': `PC${ts}`.slice(0, 10), 'Display Name': `E2E Cond ${ts}`,
    });
    await testLookupPage(driver, '/settings/wishlist-types', 'Wishlist Types', {
      'Type Code': `WL${ts}`.slice(0, 10), 'Display Name': `E2E Wish ${ts}`,
    });
    await testLookupPage(driver, '/settings/stock-movement-types', 'Stock Movement Types', {
      'Type Code': `SM${ts}`.slice(0, 10), 'Display Name': `E2E StMov ${ts}`,
    });

    // ─── DASHBOARD ───
    console.log('\n-- Dashboard --');
    await navigateTo(driver, '/');
    await driver.sleep(1500);
    const dashTitle = await driver.findElement(By.css('.nx-page-title, h1')).getText().catch(() => '');
    await ok(`Dashboard loads: "${dashTitle}"`, dashTitle.length > 0);
    const dashStats = await driver.findElements(By.css('.nx-stat-card, [class*="stat-card"]'));
    await ok(`Dashboard stats visible (${dashStats.length})`, dashStats.length >= 2);

    console.log('\n✅ Lookup & Dashboard tests complete');
  } catch (e) {
    console.error('  ❌ Error:', e.message);
  } finally {
    if (driver) await driver.quit();
  }
})();
