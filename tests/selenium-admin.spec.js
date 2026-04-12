const { Builder, By, until, WebDriver } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

const BASE_URL = 'http://localhost:5173';
const API_URL = 'http://localhost:5149/api';

let driver;

async function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

async function login() {
  const timestamp = Date.now();
  const email = `test${timestamp}@example.com`;
  const password = 'Test@123456';
  
  await fetch(`${API_URL}/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email,
      password,
      firstName: 'Test',
      lastName: 'User',
      phone: '+8801712345678'
    })
  });
  
  await driver.get(`${BASE_URL}/login`);
  await driver.wait(until.elementLocated(By.css('input#email')), 10000);
  await driver.findElement(By.css('input#email')).sendKeys(email);
  await driver.findElement(By.css('input#password')).sendKeys(password);
  await driver.findElement(By.css('button[type="submit"]')).click();
  
  await sleep(3000);
  console.log('  ✓ Logged in\n');
}

async function testPage(url, name, checkData = null) {
  try {
    await driver.get(url);
    await driver.wait(until.elementLocated(By.css('body')), 10000);
    await sleep(2000);
    
    const currentUrl = await driver.getCurrentUrl();
    if (currentUrl.includes('/login')) {
      console.log(`  ${name}: ✗ (not authenticated)`);
      return false;
    }
    
    const bodyText = await driver.findElement(By.css('body')).getText();
    const hasContent = bodyText.length > 10;
    const hasError = bodyText.includes('error') || bodyText.includes('Error');
    const has404 = bodyText.includes('404') || bodyText.includes('Page not found');
    
    if (checkData && bodyText.includes(checkData)) {
      console.log(`  ${name}: ✓ (data: ${checkData})`);
      return true;
    }
    
    console.log(`  ${name}:`, hasContent && !hasError && !has404 ? '✓' : '✗');
    return hasContent && !hasError && !has404;
  } catch (e) {
    console.log(`  ${name}: ✗ (${e.message})`);
    return false;
  }
}

async function runAllTests() {
  console.log('Starting Selenium E2E Tests (Authenticated)');
  console.log('='.repeat(50));
  
  try {
    driver = new Builder()
      .forBrowser('chrome')
      .setChromeOptions(new chrome.Options().addArguments('--headless', '--no-sandbox', '--disable-dev-shm-usage'))
      .build();
    
    await driver.manage().setTimeouts({ implicit: 10000, pageLoad: 30000 });
    
    console.log('Logging in...');
    await login();
    
    console.log('\n--- Testing Pages ---');
    await testPage(`${BASE_URL}/`, 'Dashboard');
    await testPage(`${BASE_URL}/products`, 'Products');
    await testPage(`${BASE_URL}/categories`, 'Categories', 'Product Test Cat');
    await testPage(`${BASE_URL}/brands`, 'Brands');
    await testPage(`${BASE_URL}/customers`, 'Customers');
    await testPage(`${BASE_URL}/orders`, 'Orders');
    await testPage(`${BASE_URL}/suppliers`, 'Suppliers');
    await testPage(`${BASE_URL}/employees`, 'Employees');
    await testPage(`${BASE_URL}/inventory/stock`, 'Stock Items');
    await testPage(`${BASE_URL}/tags`, 'Tags');
    await testPage(`${BASE_URL}/units`, 'Units');
    await testPage(`${BASE_URL}/attributes`, 'Attributes');
    await testPage(`${BASE_URL}/pos`, 'POS Terminal');
    await testPage(`${BASE_URL}/settings/order-statuses`, 'Order Statuses');
    await testPage(`${BASE_URL}/settings/payment-statuses`, 'Payment Statuses');
    await testPage(`${BASE_URL}/inventory/movements`, 'Stock Movements');
    await testPage(`${BASE_URL}/inventory/transfers`, 'Stock Transfers');
    await testPage(`${BASE_URL}/inventory/adjustments`, 'Inventory Adjustments');
    
    console.log('\n' + '='.repeat(50));
    console.log('All tests completed!');
    console.log('='.repeat(50));
    
  } catch (error) {
    console.error('Test error:', error.message);
  } finally {
    if (driver) {
      await driver.quit();
    }
  }
}

runAllTests();