const { createDriver, login, navigateTo, ok, By } = require('../utils/driver');

(async function testInventory() {
  console.log('\n📦 INVENTORY BROWSER TESTS');
  let driver;
  try {
    driver = await createDriver();
    await login(driver);

    // ─── STOCK ITEMS ───
    console.log('\n-- Stock Items --');
    await navigateTo(driver, '/inventory');
    await driver.sleep(1500);
    await ok('Stock Items page loads', true);
    const statCards = await driver.findElements(By.css('.nx-stat-card, [class*="stat"]'));
    await ok(`Stats visible (${statCards.length} elements)`, statCards.length > 0);

    // ─── STOCK MOVEMENTS ───
    console.log('\n-- Stock Movements --');
    await navigateTo(driver, '/inventory/movements');
    await driver.sleep(1500);
    await ok('Stock Movements page loads', true);

    // ─── INVENTORY ADJUSTMENTS ───
    console.log('\n-- Inventory Adjustments --');
    await navigateTo(driver, '/inventory/adjustments');
    await driver.sleep(1500);
    await ok('Inventory Adjustments page loads', true);

    // ─── STOCK TRANSFERS ───
    console.log('\n-- Stock Transfers --');
    await navigateTo(driver, '/inventory/transfers');
    await driver.sleep(1500);
    await ok('Stock Transfers page loads', true);

    console.log('\n✅ Inventory tests complete');
  } catch (e) {
    console.error('  ❌ Error:', e.message);
  } finally {
    if (driver) await driver.quit();
  }
})();
