// test-customers-orders.mjs — Customers, Orders, Employees
const BASE = 'http://localhost:5142/api';
const ts = Date.now();

const f = (url, opts = {}) => {
  const { headers, ...rest } = opts;
  return fetch(`${BASE}${url}`, {
    ...rest,
    headers: { 'Content-Type': 'application/json', ...headers },
  }).then(async r => ({ status: r.status, data: await r.json().catch(() => null) }));
};

const ok = (label, cond) => console.log(cond ? `  ✅ ${label}` : `  ❌ ${label}`);

export async function testCustomersOrders(auth = {}) {
  console.log('\n👥 CUSTOMERS & ORDERS TESTS');

  // ─── CUSTOMERS ───
  console.log('\n-- Customers --');
  const custList = await f('/customers', { headers: auth });
  ok(`GET /customers: ${custList.status}`, custList.status === 200);

  const custStats = await f('/customers/stats', { headers: auth });
  ok(`GET /customers/stats: ${custStats.status}`, custStats.status === 200);

  const custCreate = await f('/customers', {
    method: 'POST', headers: auth,
    body: JSON.stringify({
      firstName: 'Test', lastName: `Customer${ts}`, email: `cust${ts}@test.com`,
      phone: '01700000000', dateOfBirth: '1990-01-01', gender: 'Male',
      addressLine1: '123 Test St', city: 'Dhaka', country: 'BD', postalCode: '1000',
      isActive: true,
    }),
  });
  const custId = custCreate.data?.data?.id || custCreate.data?.id;
  ok(`POST /customers: ${custCreate.status} id=${custId}`, (custCreate.status === 200 || custCreate.status === 201) && custId);

  if (custId) {
    const custGet = await f(`/customers/${custId}`, { headers: auth });
    ok(`GET /customers/${custId}: ${custGet.status}`, custGet.status === 200);

    const custUpdate = await f(`/customers/${custId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({
        firstName: 'Test', lastName: `Customer${ts} Upd`, email: `cust${ts}@test.com`,
        phone: '01700000001', dateOfBirth: '1990-01-01', gender: 'Male',
        addressLine1: '456 Test St', city: 'Dhaka', country: 'BD', postalCode: '1000',
        isActive: true,
      }),
    });
    ok(`PUT /customers/${custId}: ${custUpdate.status}`, custUpdate.status === 200 || custUpdate.status === 204);

    const custToggle = await f(`/customers/${custId}/toggle-active`, { method: 'POST', headers: auth });
    ok(`POST toggle-active: ${custToggle.status}`, custToggle.status === 200 || custToggle.status === 204);

    // Toggle back
    await f(`/customers/${custId}/toggle-active`, { method: 'POST', headers: auth });

    const custAddr = await f(`/customers/addresses/${custId}`, { headers: auth });
    ok(`GET /customers/addresses/${custId}: ${custAddr.status}`, custAddr.status === 200);

    const custDel = await f(`/customers/${custId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE /customers/${custId}: ${custDel.status}`, custDel.status === 200 || custDel.status === 204);
  }

  // ─── EMPLOYEES ───
  console.log('\n-- Employees --');
  const empList = await f('/employees', { headers: auth });
  ok(`GET /employees: ${empList.status}`, empList.status === 200);

  const empDepts = await f('/employees/departments', { headers: auth });
  ok(`GET /employees/departments: ${empDepts.status}`, empDepts.status === 200);

  const empStats = await f('/employees/stats', { headers: auth });
  ok(`GET /employees/stats: ${empStats.status}`, empStats.status === 200);

  const empCreate = await f('/employees', {
    method: 'POST', headers: auth,
    body: JSON.stringify({
      firstName: 'Test', lastName: `Employee${ts}`, email: `emp${ts}@test.com`,
      phone: '01700000000', department: 'IT', position: 'Developer', hireDate: '2024-01-01',
      salary: 50000, isActive: true,
    }),
  });
  const empId = empCreate.data?.data?.id || empCreate.data?.id;
  ok(`POST /employees: ${empCreate.status} id=${empId}`, (empCreate.status === 200 || empCreate.status === 201) && empId);

  if (empId) {
    const empGet = await f(`/employees/${empId}`, { headers: auth });
    ok(`GET /employees/${empId}: ${empGet.status}`, empGet.status === 200);

    const empUpdate = await f(`/employees/${empId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({
        firstName: 'Test', lastName: `Employee${ts} Upd`, email: `emp${ts}@test.com`,
        phone: '01700000001', department: 'IT', position: 'Senior Dev', hireDate: '2024-01-01',
        salary: 60000, isActive: true,
      }),
    });
    ok(`PUT /employees/${empId}: ${empUpdate.status}`, empUpdate.status === 200 || empUpdate.status === 204);

    const empToggle = await f(`/employees/${empId}/toggle-active`, { method: 'POST', headers: auth });
    ok(`POST toggle-active: ${empToggle.status}`, empToggle.status === 200 || empToggle.status === 204);

    const empDel = await f(`/employees/${empId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE /employees/${empId}: ${empDel.status}`, empDel.status === 200 || empDel.status === 204);
  }

  // ─── ORDERS ───
  console.log('\n-- Orders --');
  const orderList = await f('/orders', { headers: auth });
  ok(`GET /orders: ${orderList.status}`, orderList.status === 200);

  const orderStats = await f('/orders/stats', { headers: auth });
  ok(`GET /orders/stats: ${orderStats.status}`, orderStats.status === 200);

  const orders = orderList.data?.data || [];
  if (orders.length > 0) {
    const orderId = orders[0].id;
    const orderGet = await f(`/orders/${orderId}`, { headers: auth });
    ok(`GET /orders/${orderId}: ${orderGet.status}`, orderGet.status === 200);
  } else {
    ok('No orders to test GET by ID', true);
  }

  return { custId };
}

if (process.argv[1]?.includes('test-customers')) {
  testCustomersOrders().then(() => console.log('\nCustomers & Orders tests done.'));
}
