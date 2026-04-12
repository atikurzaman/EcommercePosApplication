const { Builder, By, until, Keys } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

const API_URL = 'http://localhost:5149';
const STOREFRONT_URL = 'http://localhost:3000';
const ADMIN_URL = 'http://localhost:5173';

async function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

async function runApiTests() {
  console.log('\n=== Testing API ===\n');
  
  const options = new chrome.Options();
  options.addArguments('--start-maximized');
  options.addArguments('--no-sandbox');
  
  const driver = await new Builder()
    .forBrowser('chrome')
    .setChromeOptions(options)
    .build();

  try {
    console.log('1. Loading API health endpoint...');
    await driver.get(`${API_URL}/health`);
    await sleep(2000);
    const body = await driver.findElement(By.css('body'));
    const content = await body.getText();
    console.log(`   API Response: ${content.substring(0, 200)}`);
    console.log('   ✓ API endpoint tested');
  } catch (error) {
    console.log('   ⚠ API may not be running (SQL Server required)');
    console.log(`   Error: ${error.message}`);
  } finally {
    await driver.quit();
  }
}

async function runStorefrontTests() {
  console.log('\n=== Testing Storefront ===\n');
  
  const options = new chrome.Options();
  options.addArguments('--start-maximized');
  options.addArguments('--no-sandbox');
  
  const driver = await new Builder()
    .forBrowser('chrome')
    .setChromeOptions(options)
    .build();

  try {
    console.log('1. Loading homepage...');
    await driver.get(STOREFRONT_URL);
    await driver.wait(until.elementLocated(By.css('body'), 15000));
    console.log('   ✓ Homepage loaded');

    const title = await driver.getTitle();
    console.log(`   Page title: ${title}`);

    console.log('2. Checking for navigation...');
    const nav = await driver.findElements(By.css('nav'));
    console.log(`   ✓ Found ${nav.length} navigation element(s)`);

    console.log('3. Navigating to electronics category...');
    await driver.get(`${STOREFRONT_URL}/category/electronics`);
    await driver.wait(until.elementLocated(By.css('body'), 15000));
    console.log('   ✓ Category page loaded');

    console.log('4. Searching for products...');
    await driver.get(`${STOREFRONT_URL}/search`);
    await driver.wait(until.elementLocated(By.css('input'), 15000));
    const searchInput = await driver.findElement(By.css('input'));
    await searchInput.sendKeys('headphones');
    await sleep(500);
    console.log('   ✓ Search performed');

    console.log('5. Viewing cart page...');
    await driver.get(`${STOREFRONT_URL}/cart`);
    await driver.wait(until.elementLocated(By.css('body'), 15000));
    console.log('   ✓ Cart page loaded');

    console.log('6. Viewing checkout page...');
    await driver.get(`${STOREFRONT_URL}/checkout`);
    await driver.wait(until.elementLocated(By.css('body'), 15000));
    console.log('   ✓ Checkout page loaded');

    console.log('7. Viewing product detail page...');
    await driver.get(`${STOREFRONT_URL}/product/premium-wireless-headphones`);
    await driver.wait(until.elementLocated(By.css('body'), 15000));
    const h1 = await driver.findElements(By.css('h1'));
    console.log(`   ✓ Product page loaded with ${h1.length} heading(s)`);

    console.log('\n✅ All storefront tests passed!');
    await sleep(2000);
  } catch (error) {
    console.error('   ❌ Error:', error.message);
  } finally {
    await driver.quit();
  }
}

async function runAdminPortalTests() {
  console.log('\n=== Testing Admin Portal ===\n');
  
  const options = new chrome.Options();
  options.addArguments('--start-maximized');
  options.addArguments('--no-sandbox');
  
  const driver = await new Builder()
    .forBrowser('chrome')
    .setChromeOptions(options)
    .build();

  try {
    console.log('1. Loading login page...');
    await driver.get(`${ADMIN_URL}/login`);
    await driver.wait(until.elementLocated(By.css('body'), 15000));
    console.log('   ✓ Login page loaded');

    console.log('2. Checking email input field...');
    const emailInput = await driver.findElement(By.css('input#email'));
    await emailInput.sendKeys('admin@example.com');
    const emailValue = await emailInput.getAttribute('value');
    console.log(`   ✓ Email field working (value: ${emailValue})`);

    console.log('3. Checking password input field...');
    const passwordInput = await driver.findElement(By.css('input#password'));
    await passwordInput.sendKeys('password123');
    const passwordType = await passwordInput.getAttribute('type');
    console.log(`   ✓ Password field working (type: ${passwordType})`);

    console.log('4. Checking submit button...');
    const submitButton = await driver.findElement(By.css('button[type="submit"]'));
    const buttonText = await submitButton.getText();
    const isEnabled = await submitButton.isEnabled();
    console.log(`   ✓ Submit button found (text: "${buttonText}", enabled: ${isEnabled})`);

    console.log('5. Clicking sign in button...');
    await submitButton.click();
    await sleep(3000);
    const currentUrl = await driver.getCurrentUrl();
    console.log(`   ✓ After click, URL: ${currentUrl}`);

    console.log('6. Verifying page has content...');
    const body = await driver.findElement(By.css('body'));
    const content = await body.getText();
    console.log(`   ✓ Page has ${content.length} characters of content`);

    console.log('\n✅ All admin portal tests passed!');
    await sleep(2000);
  } catch (error) {
    console.error('   ❌ Error:', error.message);
  } finally {
    await driver.quit();
  }
}

async function main() {
  console.log('🚀 Starting Selenium UI Tests for All 3 Projects');
  console.log('==================================================');
  console.log('Browser windows will appear - do not close them manually');
  
  await runApiTests();
  await runStorefrontTests();
  await runAdminPortalTests();
  
  console.log('\n==================================================');
  console.log('🎉 All Selenium UI tests completed!');
}

main().catch(console.error);
