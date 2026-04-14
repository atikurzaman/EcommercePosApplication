const { createDriver, login, ok, navigateTo, BASE_URL, By, until, TIMEOUT } = require('../utils/driver');

(async function testAuth() {
  console.log('\n🔐 AUTH BROWSER TESTS');
  let driver;
  try {
    driver = await createDriver();

    // ─── Login Page Loads ───
    console.log('\n-- Login Page --');
    await driver.get(`${BASE_URL}/login`);
    await driver.wait(until.elementLocated(By.id('email')), TIMEOUT);
    const emailField = await driver.findElement(By.id('email'));
    const passField = await driver.findElement(By.id('password'));
    const submitBtn = await driver.findElement(By.css('button[type="submit"]'));
    await ok('Login page loads with email/password/submit', emailField && passField && submitBtn);

    // ─── Invalid Login ───
    console.log('\n-- Invalid Login --');
    await emailField.sendKeys('bad@email.com');
    await passField.sendKeys('wrongpass');
    await submitBtn.click();
    await driver.sleep(2000);
    const currentUrl = await driver.getCurrentUrl();
    await ok('Invalid login stays on login page', currentUrl.includes('/login'));

    // ─── Valid Login ───
    console.log('\n-- Valid Login --');
    const testEmail = await login(driver);
    const dashUrl = await driver.getCurrentUrl();
    await ok('Valid login redirects to dashboard', !dashUrl.includes('/login'));

    // ─── Dashboard Loads ───
    console.log('\n-- Dashboard --');
    const pageTitle = await driver.findElement(By.css('.nx-page-title, h1')).getText();
    await ok(`Dashboard title visible: "${pageTitle}"`, pageTitle.length > 0);

    // ─── Sidebar Navigation ───
    console.log('\n-- Sidebar Nav --');
    const sidebarLinks = await driver.findElements(By.css('nav a, aside a'));
    await ok(`Sidebar has ${sidebarLinks.length} links`, sidebarLinks.length > 5);

    console.log('\n✅ Auth tests complete');
  } catch (e) {
    console.error('  ❌ Error:', e.message);
  } finally {
    if (driver) await driver.quit();
  }
})();
