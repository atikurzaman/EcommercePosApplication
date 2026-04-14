// test-pos.mjs — Warehouses, Counters, Terminals, Cash Shifts, Transactions, Returns, Expenses
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

export async function testPos(auth = {}) {
  console.log('\n🏪 POS TESTS');

  // ─── WAREHOUSES ───
  console.log('\n-- Warehouses --');
  const whList = await f('/warehouses', { headers: auth });
  ok(`GET /warehouses: ${whList.status}`, whList.status === 200);

  const whStats = await f('/warehouses/stats', { headers: auth });
  ok(`GET /warehouses/stats: ${whStats.status}`, whStats.status === 200);

  const whCreate = await f('/warehouses', {
    method: 'POST', headers: auth,
    body: JSON.stringify({
      code: `WH${ts}`.slice(0, 10), name: `Warehouse ${ts}`, siteType: 'WAREHOUSE',
      parentId: null, contactPerson: 'John', managerName: 'Jane',
      addressLine1: '123 Test St', addressLine2: '', city: 'Dhaka', area: 'Gulshan',
      state: 'Dhaka', postalCode: '1000', country: 'BD', phone: '01700000000',
      email: `wh${ts}@test.com`, latitude: null, longitude: null,
      openingTime: '09:00', closingTime: '21:00', taxNumber: '', isDefault: false, isActive: true,
    }),
  });
  const whId = whCreate.data?.data?.id || whCreate.data?.id;
  ok(`POST /warehouses: ${whCreate.status} id=${whId}`, (whCreate.status === 200 || whCreate.status === 201) && whId);

  if (whId) {
    const whGet = await f(`/warehouses/${whId}`, { headers: auth });
    ok(`GET /warehouses/${whId}: ${whGet.status}`, whGet.status === 200);

    const whUpdate = await f(`/warehouses/${whId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({
        code: `WH${ts}`.slice(0, 10), name: `Warehouse ${ts} Upd`, siteType: 'WAREHOUSE',
        parentId: null, contactPerson: 'John Upd', managerName: 'Jane Upd',
        addressLine1: '456 Test St', addressLine2: '', city: 'Dhaka', area: 'Banani',
        state: 'Dhaka', postalCode: '1000', country: 'BD', phone: '01700000001',
        email: `wh${ts}@test.com`, latitude: null, longitude: null,
        openingTime: '08:00', closingTime: '22:00', taxNumber: '', isDefault: false, isActive: true,
      }),
    });
    ok(`PUT /warehouses/${whId}: ${whUpdate.status}`, whUpdate.status === 200 || whUpdate.status === 204);

    const whToggle = await f(`/warehouses/${whId}/toggle-active`, { method: 'POST', headers: auth });
    ok(`POST toggle-active: ${whToggle.status}`, whToggle.status === 200 || whToggle.status === 204);
    await f(`/warehouses/${whId}/toggle-active`, { method: 'POST', headers: auth }); // toggle back

    // ─── POS COUNTERS ───
    console.log('\n-- POS Counters --');
    const ctrList = await f(`/pos-counters?warehouseId=${whId}`, { headers: auth });
    ok(`GET /pos-counters: ${ctrList.status}`, ctrList.status === 200);

    const ctrCreate = await f('/pos-counters', {
      method: 'POST', headers: auth,
      body: JSON.stringify({ counterCode: `CTR${ts}`.slice(0, 10), counterName: `Counter ${ts}`, isActive: true, warehouseId: whId }),
    });
    const ctrId = ctrCreate.data?.data?.id || ctrCreate.data?.id;
    ok(`POST /pos-counters: ${ctrCreate.status} id=${ctrId}`, (ctrCreate.status === 200 || ctrCreate.status === 201));

    if (ctrId) {
      const ctrGet = await f(`/pos-counters/${ctrId}`, { headers: auth });
      ok(`GET /pos-counters/${ctrId}: ${ctrGet.status}`, ctrGet.status === 200);

      const ctrUpdate = await f(`/pos-counters/${ctrId}`, {
        method: 'PUT', headers: auth,
        body: JSON.stringify({ counterCode: `CTR${ts}`.slice(0, 10), counterName: `Counter ${ts} Upd`, isActive: true }),
      });
      ok(`PUT /pos-counters/${ctrId}: ${ctrUpdate.status}`, ctrUpdate.status === 200 || ctrUpdate.status === 204);

      // ─── POS TERMINALS ───
      console.log('\n-- POS Terminals --');
      const termList = await f(`/pos-terminals?counterId=${ctrId}`, { headers: auth });
      ok(`GET /pos-terminals: ${termList.status}`, termList.status === 200);

      const termCreate = await f('/pos-terminals', {
        method: 'POST', headers: auth,
        body: JSON.stringify({
          terminalCode: `TRM${ts}`.slice(0, 10), terminalName: `Terminal ${ts}`,
          machineName: 'TEST-PC', ipaddress: '192.168.1.100', printerName: 'TestPrinter',
          isActive: true, posCounterId: ctrId,
        }),
      });
      const termId = termCreate.data?.data?.id || termCreate.data?.id;
      ok(`POST /pos-terminals: ${termCreate.status} id=${termId}`, (termCreate.status === 200 || termCreate.status === 201));

      if (termId) {
        const termUpdate = await f(`/pos-terminals/${termId}`, {
          method: 'PUT', headers: auth,
          body: JSON.stringify({
            terminalCode: `TRM${ts}`.slice(0, 10), terminalName: `Terminal ${ts} Upd`,
            machineName: 'TEST-PC-2', ipaddress: '192.168.1.101', printerName: 'TestPrinter2',
            isActive: true,
          }),
        });
        ok(`PUT terminal: ${termUpdate.status}`, termUpdate.status === 200 || termUpdate.status === 204);

        const termDel = await f(`/pos-terminals/${termId}`, { method: 'DELETE', headers: auth });
        ok(`DELETE terminal: ${termDel.status}`, termDel.status === 200 || termDel.status === 204);
      }

      const ctrDel = await f(`/pos-counters/${ctrId}`, { method: 'DELETE', headers: auth });
      ok(`DELETE counter: ${ctrDel.status}`, ctrDel.status === 200 || ctrDel.status === 204);
    }
  }

  // ─── EXPENSE CATEGORIES ───
  console.log('\n-- Expense Categories --');
  const expCatList = await f('/expense-categories', { headers: auth });
  ok(`GET /expense-categories: ${expCatList.status}`, expCatList.status === 200);

  const expCatCreate = await f('/expense-categories', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `ExpCat ${ts}`, description: 'E2E test', isActive: true }),
  });
  const expCatId = expCatCreate.data?.data?.id || expCatCreate.data?.id;
  ok(`POST /expense-categories: ${expCatCreate.status} id=${expCatId}`, (expCatCreate.status === 200 || expCatCreate.status === 201));

  if (expCatId) {
    const expCatGet = await f(`/expense-categories/${expCatId}`, { headers: auth });
    ok(`GET /expense-categories/${expCatId}: ${expCatGet.status}`, expCatGet.status === 200);

    const expCatUpdate = await f(`/expense-categories/${expCatId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `ExpCat ${ts} Upd`, description: 'Updated', isActive: true }),
    });
    ok(`PUT expense-category: ${expCatUpdate.status}`, expCatUpdate.status === 200 || expCatUpdate.status === 204);
  }

  // ─── EXPENSES ───
  console.log('\n-- Expenses --');
  const expList = await f('/expenses', { headers: auth });
  ok(`GET /expenses: ${expList.status}`, expList.status === 200);

  // Get a warehouse for expense
  const expWhList = await f('/warehouses', { headers: auth });
  const expWarehouses = expWhList.data?.data || [];
  const expWhId = expWarehouses[0]?.id || whId;

  if (expCatId && expWhId) {
    const expCreate = await f('/expenses', {
      method: 'POST', headers: auth,
      body: JSON.stringify({
        warehouseId: expWhId, expenseCategoryId: expCatId, expenseDate: '2024-06-01',
        description: `Test Expense ${ts}`, amount: 500, methodCode: 'CASH',
        receiptReference: `RCP-${ts}`,
      }),
    });
    const expId = expCreate.data?.data?.id || expCreate.data?.id;
    ok(`POST /expenses: ${expCreate.status} id=${expId}`, (expCreate.status === 200 || expCreate.status === 201));

    if (expId) {
      const expGet = await f(`/expenses/${expId}`, { headers: auth });
      ok(`GET /expenses/${expId}: ${expGet.status}`, expGet.status === 200);

      const expUpdate = await f(`/expenses/${expId}`, {
        method: 'PUT', headers: auth,
        body: JSON.stringify({
          warehouseId: expWhId, expenseCategoryId: expCatId, expenseDate: '2024-06-02',
          description: `Test Expense ${ts} Upd`, amount: 600, methodCode: 'CASH',
          receiptReference: `RCP-${ts}-U`,
        }),
      });
      ok(`PUT /expenses/${expId}: ${expUpdate.status}`, expUpdate.status === 200 || expUpdate.status === 204);

      const expDel = await f(`/expenses/${expId}`, { method: 'DELETE', headers: auth });
      ok(`DELETE /expenses/${expId}: ${expDel.status}`, expDel.status === 200 || expDel.status === 204);
    }
  }

  // Cleanup expense category
  if (expCatId) {
    const expCatDel = await f(`/expense-categories/${expCatId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE expense-category: ${expCatDel.status}`, expCatDel.status === 200 || expCatDel.status === 204);
  }

  // ─── CASH SHIFTS ───
  console.log('\n-- Cash Shifts --');
  const shiftList = await f('/cash-shifts', { headers: auth });
  ok(`GET /cash-shifts: ${shiftList.status}`, shiftList.status === 200);

  const shiftActive = await f('/cash-shifts/active', { headers: auth });
  ok(`GET /cash-shifts/active: ${shiftActive.status}`, shiftActive.status === 200 || shiftActive.status === 404);

  // ─── POS TRANSACTIONS ───
  console.log('\n-- POS Transactions --');
  const txList = await f('/pos-transactions', { headers: auth });
  ok(`GET /pos-transactions: ${txList.status}`, txList.status === 200);

  const txns = txList.data?.data || [];
  if (txns.length > 0) {
    const txId = txns[0].id;
    const txGet = await f(`/pos-transactions/${txId}`, { headers: auth });
    ok(`GET /pos-transactions/${txId}: ${txGet.status}`, txGet.status === 200);
  } else {
    ok('No transactions to test GET by ID', true);
  }

  // ─── POS RETURNS ───
  console.log('\n-- POS Returns --');
  const retList = await f('/pos-returns', { headers: auth });
  ok(`GET /pos-returns: ${retList.status}`, retList.status === 200);

  // ─── CASH DRAWER EVENTS ───
  console.log('\n-- Cash Drawer Events --');
  const shifts = shiftList.data?.data || [];
  if (shifts.length > 0) {
    const shiftId = shifts[0].id;
    const cdeList = await f(`/cash-drawer-events?cashShiftId=${shiftId}`, { headers: auth });
    ok(`GET /cash-drawer-events?cashShiftId=${shiftId}: ${cdeList.status}`, cdeList.status === 200);
  } else {
    ok('No shifts for cash-drawer-events test (skipped)', true);
  }

  // ─── DAY-END SUMMARIES ───
  console.log('\n-- Day-End Summaries --');
  const desList = await f('/day-end-summaries', { headers: auth });
  ok(`GET /day-end-summaries: ${desList.status}`, desList.status === 200);

  // Cleanup warehouse
  if (whId) {
    const whDel = await f(`/warehouses/${whId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE warehouse: ${whDel.status}`, whDel.status === 200 || whDel.status === 204);
  }

  return { whId };
}

if (process.argv[1]?.includes('test-pos')) {
  testPos().then(() => console.log('\nPOS tests done.'));
}
