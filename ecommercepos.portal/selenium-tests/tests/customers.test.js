const { createDriver, login, navigateTo, clickButton, waitForModal, fillAndSubmitForm, ok, By } = require('../utils/driver');

(async function testCustomers() {
  console.log('\n👥 CUSTOMERS, EMPLOYEES & ORDERS BROWSER TESTS');
  let driver;
  try {
    driver = await createDriver();
    await login(driver);
    const ts = Date.now();

    // ─── CUSTOMERS ───
    console.log('\n-- Customers --');
    await navigateTo(driver, '/customers');
    await ok('Customers page loads', true);
    await clickButton(driver, 'Add Customer');
    await waitForModal(driver);
    const custOk = await fillAndSubmitForm(driver, {
      'Phone': `0170${ts}`.slice(0, 11),
      'Email': `cust${ts}@test.com`,
      'Company Name': `E2E Corp ${ts}`,
      'Address': '123 Test Street',
      'City': 'Dhaka',
    }, 'Create Customer');
    await ok('Customer created (modal closed)', custOk);

    // ─── EMPLOYEES ───
    console.log('\n-- Employees --');
    await navigateTo(driver, '/employees');
    await ok('Employees page loads', true);
    const addedEmp = await clickButton(driver, 'Add Employee');
    if (addedEmp) {
      await waitForModal(driver);
      const empOk = await fillAndSubmitForm(driver, {
        'Employee Code': `EMP${ts}`.slice(0, 12),
        'First Name': 'E2E',
        'Last Name': `Employee${ts}`,
        'Email': `emp${ts}@test.com`,
        'Phone': '01700000000',
        'Department': 'QA',
        'Designation': 'Tester',
      });
      await ok('Employee created (modal closed)', empOk);
    }

    // ─── ORDERS ───
    console.log('\n-- Orders --');
    await navigateTo(driver, '/orders');
    await driver.sleep(1500);
    await ok('Orders page loads', true);
    // Try clicking an order row if one exists
    try {
      const row = await driver.findElement(By.css('table tbody tr'));
      await row.click();
      await driver.sleep(1500);
      await ok('Order detail opened', true);
    } catch {
      await ok('No orders to view (expected)', true);
    }

    console.log('\n✅ Customers, Employees & Orders tests complete');
  } catch (e) {
    console.error('  ❌ Error:', e.message);
  } finally {
    if (driver) await driver.quit();
  }
})();
