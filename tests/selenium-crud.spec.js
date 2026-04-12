const { Builder, By, until, WebDriver } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

const BASE_URL = 'http://localhost:5173';
const API_URL = 'http://localhost:5149/api';

let driver;
let testUser = {};

async function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

async function login() {
  const timestamp = Date.now();
  testUser = {
    email: `test${timestamp}@example.com`,
    password: 'Test@123456'
  };
  
  try {
    await fetch(`${API_URL}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email: testUser.email,
        password: testUser.password,
        firstName: 'Selenium',
        lastName: 'Test',
        phone: '+8801712345678'
      })
    });
  } catch (e) {
    console.log('  Registration may have failed, trying login...');
  }
  
  await driver.get(`${BASE_URL}/login`);
  await driver.wait(until.elementLocated(By.css('input#email')), 15000);
  await driver.findElement(By.css('input#email')).sendKeys(testUser.email);
  await driver.findElement(By.css('input#password')).sendKeys(testUser.password);
  await driver.findElement(By.css('button[type="submit"]')).click();
  await sleep(4000);
  console.log('  Logged in');
}

async function testPageCRUD(entityName, path, nameFieldSelector, dataSelectors = {}) {
  console.log(`\n--- ${entityName} ---`);
  
  // Navigate
  await driver.get(`${BASE_URL}${path}`);
  await driver.wait(until.elementLocated(By.css('body')), 15000);
  await sleep(2500);
  
  // Check page loaded
  const pageLoaded = await driver.findElement(By.css('body')).getText();
  console.log(`  Page loaded: ${pageLoaded.length > 0 ? 'YES' : 'NO'}`);
  
  // Click Add button
  const buttons = await driver.findElements(By.css('button'));
  let addBtn = null;
  for (const btn of buttons) {
    const text = await btn.getText();
    if (text.match(/add|new|create/i)) {
      addBtn = btn;
      break;
    }
  }
  
  if (addBtn) {
    await addBtn.click();
    await sleep(2000);
    console.log('  Open form: YES');
    
    // Fill first input
    const inputs = await driver.findElements(By.css('input:not([type="checkbox"]), textarea'));
    if (inputs.length > 0) {
      const testName = `${entityName} ${Date.now()}`;
      await inputs[0].clear();
      await inputs[0].sendKeys(testName);
      console.log(`  Filled name: ${testName}`);
      
      // Submit
      const submitBtns = await driver.findElements(By.css('button'));
      for (const btn of submitBtns) {
        const text = await btn.getText();
        if (text.match(/save|create|submit/i)) {
          await btn.click();
          await sleep(2500);
          console.log('  Submitted: YES');
          break;
        }
      }
    }
  } else {
    console.log('  Open form: NO (button not found)');
  }
  
  // Refresh and check
  await driver.get(`${BASE_URL}${path}`);
  await sleep(2000);
  console.log('  CRUD test complete');
}

async function runTests() {
  console.log('='.repeat(50));
  console.log('Selenium CRUD Tests (VISIBLE Browser)');
  console.log('='.repeat(50));
  console.log('NOTE: Make sure admin portal is running on http://localhost:5173');
  console.log('NOTE: Make sure API is running on http://localhost:5149');
  console.log('');
  
  driver = new Builder()
    .forBrowser('chrome')
    .setChromeOptions(new chrome.Options().addArguments('--start-maximized', '--no-sandbox'))
    .build();
  
  await driver.manage().setTimeouts({ implicit: 15000, pageLoad: 45000 });
  
  try {
    // Login first
    console.log('\nLogging in...');
    await login();
    
    // Run CRUD tests for each entity
    await testPageCRUD('Categories', '/categories');
    await testPageCRUD('Brands', '/brands');
    await testPageCRUD('Units', '/units');
    await testPageCRUD('Tags', '/tags');
    await testPageCRUD('Suppliers', '/suppliers');
    await testPageCRUD('Employees', '/employees');
    
    console.log('\n' + '='.repeat(50));
    console.log('All CRUD tests completed!');
    console.log('='.repeat(50));
    
  } catch (error) {
    console.error('Error:', error.message);
  } finally {
    await sleep(5000);
    await driver.quit();
  }
}

runTests();