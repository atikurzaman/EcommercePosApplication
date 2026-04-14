const { Builder, By, Key, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

const BASE_URL = 'http://localhost:5173';
const TIMEOUT = 10000;

async function createDriver() {
  const options = new chrome.Options();
  options.addArguments('--no-sandbox', '--disable-dev-shm-usage');
  // options.addArguments('--headless=new'); // uncomment for headless

  const driver = await new Builder()
    .forBrowser('chrome')
    .setChromeOptions(options)
    .build();

  await driver.manage().setTimeouts({ implicit: 5000, pageLoad: 30000 });
  await driver.manage().window().setRect({ width: 1400, height: 900 });
  return driver;
}

async function login(driver, email = null, password = 'Test@12345') {
  // Register a fresh user, then login
  const ts = Date.now();
  const testEmail = email || `e2e${ts}@test.com`;

  // Register via API
  await fetch('http://localhost:5142/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email: testEmail, password, firstName: 'E2E', lastName: `User${ts}`, phone: '01700000000',
    }),
  });

  await driver.get(`${BASE_URL}/login`);
  await driver.wait(until.elementLocated(By.id('email')), TIMEOUT);

  await driver.findElement(By.id('email')).sendKeys(testEmail);
  await driver.findElement(By.id('password')).sendKeys(password);
  await driver.findElement(By.css('button[type="submit"]')).click();

  // Wait for URL to change away from /login
  await driver.wait(async () => {
    const url = await driver.getCurrentUrl();
    return !url.includes('/login');
  }, TIMEOUT);
  await driver.sleep(1500);
  console.log('  ✅ Logged in as', testEmail);
  return testEmail;
}

async function navigateTo(driver, path) {
  await driver.get(`${BASE_URL}${path}`);
  await driver.sleep(1000);
  await driver.wait(until.elementLocated(By.css('.nx-page-header, .nx-page-title, table, form')), TIMEOUT);
}

async function clickButton(driver, text) {
  const buttons = await driver.findElements(By.css('button'));
  for (const btn of buttons) {
    const btnText = await btn.getText();
    if (btnText.includes(text)) {
      await btn.click();
      return true;
    }
  }
  return false;
}

async function waitForModal(driver) {
  await driver.wait(until.elementLocated(By.css('.fixed.inset-0, [role="dialog"]')), TIMEOUT);
  await driver.sleep(500);
}

async function fillInput(driver, name, value) {
  try {
    // Try by name attribute first
    let input = await driver.findElement(By.name(name)).catch(() => null);
    // Then try by id
    if (!input) input = await driver.findElement(By.id(name)).catch(() => null);
    // Then try by placeholder
    if (!input) {
      const inputs = await driver.findElements(By.css('input, textarea, select'));
      for (const el of inputs) {
        const ph = await el.getAttribute('placeholder');
        const lbl = await el.getAttribute('aria-label');
        if (ph?.toLowerCase().includes(name.toLowerCase()) || lbl?.toLowerCase().includes(name.toLowerCase())) {
          input = el;
          break;
        }
      }
    }
    if (!input) return false;

    const tag = await input.getTagName();
    if (tag === 'select') {
      await input.click();
      await driver.sleep(200);
      const options = await input.findElements(By.css('option'));
      for (const opt of options) {
        const optText = await opt.getText();
        if (optText.includes(value)) {
          await opt.click();
          break;
        }
      }
    } else {
      await input.clear();
      await input.sendKeys(value);
    }
    return true;
  } catch {
    return false;
  }
}

async function fillFormByLabel(driver, label, value) {
  try {
    const labels = await driver.findElements(By.css('label'));
    for (const lbl of labels) {
      const text = await lbl.getText();
      if (text.replace('*', '').trim().toLowerCase() === label.toLowerCase()) {
        const forAttr = await lbl.getAttribute('for');
        let input;
        if (forAttr) {
          input = await driver.findElement(By.id(forAttr)).catch(() => null);
        }
        if (!input) {
          // Try sibling or child input
          const parent = await lbl.findElement(By.xpath('..'));
          input = await parent.findElement(By.css('input, textarea, select')).catch(() => null);
        }
        if (!input) {
          // Try next sibling
          input = await lbl.findElement(By.xpath('following-sibling::input | following-sibling::textarea | following-sibling::select | following-sibling::div//input')).catch(() => null);
        }
        if (input) {
          const tag = await input.getTagName();
          const type = await input.getAttribute('type');
          if (type === 'checkbox') {
            const checked = await input.isSelected();
            if ((value && !checked) || (!value && checked)) {
              await input.click();
            }
          } else if (tag === 'select') {
            await input.click();
            await driver.sleep(200);
            const opts = await input.findElements(By.css('option'));
            for (const o of opts) {
              if ((await o.getText()).includes(value)) { await o.click(); break; }
            }
          } else {
            // Use JS to set value and trigger React's onChange
            await driver.executeScript(`
              const el = arguments[0];
              const nativeInputValueSetter = Object.getOwnPropertyDescriptor(
                window.HTMLInputElement.prototype, 'value'
              )?.set || Object.getOwnPropertyDescriptor(
                window.HTMLTextAreaElement.prototype, 'value'
              )?.set;
              if (nativeInputValueSetter) {
                nativeInputValueSetter.call(el, arguments[1]);
              } else {
                el.value = arguments[1];
              }
              el.dispatchEvent(new Event('input', { bubbles: true }));
              el.dispatchEvent(new Event('change', { bubbles: true }));
            `, input, String(value));
          }
          return true;
        }
      }
    }
    return false;
  } catch {
    return false;
  }
}

async function getTableRowCount(driver) {
  try {
    const rows = await driver.findElements(By.css('table tbody tr'));
    return rows.length;
  } catch {
    return 0;
  }
}

async function isModalClosed(driver) {
  await driver.sleep(500);
  const modals = await driver.findElements(By.css('.fixed.inset-0'));
  return modals.length === 0;
}

async function apiGet(path) {
  try {
    const r = await fetch(`http://localhost:5142/api${path}`);
    return await r.json();
  } catch { return null; }
}

async function fillAndSubmitForm(driver, fields, submitText = 'Create') {
  for (const [label, value] of Object.entries(fields)) {
    const filled = await fillFormByLabel(driver, label, value);
    if (!filled) console.log(`    ⚠️  Could not fill "${label}"`);
  }
  await clickButton(driver, submitText);
  await driver.sleep(2000);
  return await isModalClosed(driver);
}

async function ok(label, cond) {
  console.log(cond ? `  ✅ ${label}` : `  ❌ ${label}`);
  return cond;
}

module.exports = {
  createDriver, login, navigateTo, clickButton, waitForModal,
  fillInput, fillFormByLabel, getTableRowCount, isModalClosed,
  apiGet, fillAndSubmitForm, ok,
  BASE_URL, TIMEOUT, By, Key, until,
};
