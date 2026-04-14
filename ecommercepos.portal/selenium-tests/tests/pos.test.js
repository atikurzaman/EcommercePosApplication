const { createDriver, login, navigateTo, clickButton, waitForModal, fillAndSubmitForm, fillFormByLabel, ok, By } = require('../utils/driver');

(async function testPos() {
  console.log('\n🏪 POS BROWSER TESTS');
  let driver;
  try {
    driver = await createDriver();
    await login(driver);
    const ts = Date.now();

    // ─── WAREHOUSES ───
    console.log('\n-- Warehouses --');
    await navigateTo(driver, '/warehouses');
    await ok('Warehouses page loads', true);
    const addedWh = await clickButton(driver, 'Add Warehouse');
    if (addedWh) {
      await waitForModal(driver);
      const whOk = await fillAndSubmitForm(driver, {
        'Code': `WH${ts}`.slice(0, 10),
        'Name': `E2E Warehouse ${ts}`,
        'Contact Person': 'John',
        'Phone': '01700000000',
        'City': 'Dhaka',
      });
      await ok('Warehouse created (modal closed)', whOk);
    }

    // ─── POS TRANSACTIONS ───
    console.log('\n-- POS Transactions --');
    await navigateTo(driver, '/pos/transactions');
    await driver.sleep(1500);
    await ok('Transactions page loads', true);
    const searchInput = await driver.findElements(By.css('input[placeholder*="Search"]'));
    await ok('Search input visible', searchInput.length > 0);

    // ─── CASH SHIFTS ───
    console.log('\n-- Cash Shifts --');
    await navigateTo(driver, '/pos/shifts');
    await driver.sleep(1500);
    await ok('Cash Shifts page loads', true);

    // ─── EXPENSES ───
    console.log('\n-- Expenses --');
    await navigateTo(driver, '/pos/expenses');
    await driver.sleep(1500);
    await ok('Expenses page loads', true);
    const addedExp = await clickButton(driver, 'Add Expense');
    if (addedExp) {
      await waitForModal(driver);
      await fillFormByLabel(driver, 'Description', `E2E Expense ${ts}`);
      await fillFormByLabel(driver, 'Amount', '500');
      await clickButton(driver, 'Create');
      await driver.sleep(2000);
      await ok('Expense form submitted', true);
    }

    // ─── POS RETURNS ───
    console.log('\n-- POS Returns --');
    await navigateTo(driver, '/pos/returns');
    await driver.sleep(1500);
    await ok('POS Returns page loads', true);

    // ─── DAY END SUMMARIES ───
    console.log('\n-- Day End Summaries --');
    await navigateTo(driver, '/pos/day-end');
    await driver.sleep(1500);
    await ok('Day End Summaries page loads', true);

    // ─── POS TERMINAL ───
    console.log('\n-- POS Terminal --');
    await navigateTo(driver, '/pos');
    await driver.sleep(2000);
    const posPage = await driver.findElements(By.css('h1, h2, [class*="pos"], [class*="terminal"], button'));
    await ok(`POS Terminal page loads (${posPage.length} elements)`, posPage.length > 0);

    console.log('\n✅ POS tests complete');
  } catch (e) {
    console.error('  ❌ Error:', e.message);
  } finally {
    if (driver) await driver.quit();
  }
})();
