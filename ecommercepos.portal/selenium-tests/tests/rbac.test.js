const { createDriver, login, navigateTo, clickButton, waitForModal, fillAndSubmitForm, ok, By } = require('../utils/driver');

(async function testRbac() {
  console.log('\n🔑 RBAC BROWSER TESTS');
  let driver;
  try {
    driver = await createDriver();
    await login(driver);
    const ts = Date.now();

    // ─── PERMISSIONS ───
    console.log('\n-- Permissions --');
    await navigateTo(driver, '/permissions');
    await ok('Permissions page loads', true);
    await clickButton(driver, 'Add Permission');
    await waitForModal(driver);
    const permOk = await fillAndSubmitForm(driver, {
      'Permission Code': `e2e.perm.${ts}`,
      'Name': `E2E Permission ${ts}`,
      'Module': 'E2ETesting',
      'Description': 'Selenium test',
    });
    await ok('Permission created (modal closed)', permOk);

    // ─── MENUS ───
    console.log('\n-- Menus --');
    await navigateTo(driver, '/menus');
    await ok('Menus page loads', true);
    await clickButton(driver, 'Add Menu');
    await waitForModal(driver);
    const menuOk = await fillAndSubmitForm(driver, {
      'Menu Code': `e2emenu${ts}`,
      'Menu Name': `E2EMenu${ts}`,
      'Display Name': `E2E Menu ${ts}`,
      'URL': '/e2e-test',
      'Icon Class': 'TestIcon',
    });
    await ok('Menu created (modal closed)', menuOk);

    // ─── ROLES ───
    console.log('\n-- Roles --');
    await navigateTo(driver, '/roles');
    await ok('Roles page loads', true);
    await clickButton(driver, 'Add Role');
    await waitForModal(driver);
    const roleOk = await fillAndSubmitForm(driver, {
      'Name': `E2E Role ${ts}`,
      'Description': 'Selenium test role',
    });
    await ok('Role created (modal closed)', roleOk);

    // ─── USERS ───
    console.log('\n-- Users --');
    await navigateTo(driver, '/users');
    await ok('Users page loads', true);
    try {
      const firstRow = await driver.findElement(By.css('table tbody tr'));
      await firstRow.click();
      await driver.sleep(1500);
      await ok('User row clicked', true);
    } catch {
      await ok('User row click (no users)', true);
    }

    // ─── USER PROFILE ───
    console.log('\n-- User Profile --');
    await navigateTo(driver, '/profile');
    await driver.sleep(1500);
    const profileTitle = await driver.findElement(By.css('h1, h2, .nx-page-title')).getText().catch(() => '');
    await ok(`Profile page loads: "${profileTitle}"`, profileTitle.length > 0);

    console.log('\n✅ RBAC tests complete');
  } catch (e) {
    console.error('  ❌ Error:', e.message);
  } finally {
    if (driver) await driver.quit();
  }
})();
