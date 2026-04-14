// test-shipping.mjs — Shipping Methods, Delivery Zones, Pickup Points
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

export async function testShipping(auth = {}) {
  console.log('\n🚚 SHIPPING TESTS');

  // ─── SHIPPING METHODS ───
  console.log('\n-- Shipping Methods --');
  const smList = await f('/shipping-methods', { headers: auth });
  ok(`GET /shipping-methods: ${smList.status}`, smList.status === 200);

  const smCreate = await f('/shipping-methods', {
    method: 'POST', headers: auth,
    body: JSON.stringify({
      name: `Ship ${ts}`, description: 'E2E', carrierName: 'TestCarrier',
      baseCost: 50, costPerKg: 5, estimatedDaysMin: 1, estimatedDaysMax: 3,
      isActive: true, isFreeShipping: false, freeShippingThreshold: 1000, displayOrder: 99,
    }),
  });
  const smId = smCreate.data?.data?.id || smCreate.data?.id;
  ok(`POST /shipping-methods: ${smCreate.status} id=${smId}`, (smCreate.status === 200 || smCreate.status === 201));

  if (smId) {
    const smGet = await f(`/shipping-methods/${smId}`, { headers: auth });
    ok(`GET /shipping-methods/${smId}: ${smGet.status}`, smGet.status === 200);

    const smUpdate = await f(`/shipping-methods/${smId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({
        name: `Ship ${ts} Upd`, description: 'Updated', carrierName: 'TestCarrier2',
        baseCost: 60, costPerKg: 6, estimatedDaysMin: 2, estimatedDaysMax: 5,
        isActive: true, isFreeShipping: false, freeShippingThreshold: 2000, displayOrder: 98,
      }),
    });
    ok(`PUT shipping-method: ${smUpdate.status}`, smUpdate.status === 200 || smUpdate.status === 204);

    const smDel = await f(`/shipping-methods/${smId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE shipping-method: ${smDel.status}`, smDel.status === 200 || smDel.status === 204);
  }

  // ─── DELIVERY ZONES ───
  console.log('\n-- Delivery Zones --');
  const dzList = await f('/delivery-zones', { headers: auth });
  ok(`GET /delivery-zones: ${dzList.status}`, dzList.status === 200);

  const dzCreate = await f('/delivery-zones', {
    method: 'POST', headers: auth,
    body: JSON.stringify({
      name: `Zone ${ts}`, description: 'E2E', isActive: true,
      baseDeliveryCost: 30, freeDeliveryThreshold: 500, minDeliveryDays: 1, maxDeliveryDays: 2,
    }),
  });
  const dzId = dzCreate.data?.data?.id || dzCreate.data?.id;
  ok(`POST /delivery-zones: ${dzCreate.status} id=${dzId}`, (dzCreate.status === 200 || dzCreate.status === 201));

  if (dzId) {
    const dzGet = await f(`/delivery-zones/${dzId}`, { headers: auth });
    ok(`GET /delivery-zones/${dzId}: ${dzGet.status}`, dzGet.status === 200);

    const dzUpdate = await f(`/delivery-zones/${dzId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({
        name: `Zone ${ts} Upd`, description: 'Updated', isActive: true,
        baseDeliveryCost: 40, freeDeliveryThreshold: 800, minDeliveryDays: 2, maxDeliveryDays: 4,
      }),
    });
    ok(`PUT delivery-zone: ${dzUpdate.status}`, dzUpdate.status === 200 || dzUpdate.status === 204);

    const dzDel = await f(`/delivery-zones/${dzId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE delivery-zone: ${dzDel.status}`, dzDel.status === 200 || dzDel.status === 204);
  }

  // ─── PICKUP POINTS ───
  console.log('\n-- Pickup Points --');
  const ppList = await f('/pickup-points', { headers: auth });
  ok(`GET /pickup-points: ${ppList.status}`, ppList.status === 200);

  // Get a warehouse for pickup point
  const whList = await f('/warehouses', { headers: auth });
  const warehouses = whList.data?.data || [];
  const whId = warehouses[0]?.id;

  if (whId) {
    const ppCreate = await f('/pickup-points', {
      method: 'POST', headers: auth,
      body: JSON.stringify({
        warehouseId: whId, name: `Pickup ${ts}`, addressLine1: '123 Test',
        city: 'Dhaka', postalCode: '1000', phone: '01700000000',
        latitude: null, longitude: null, openingTime: '09:00', closingTime: '21:00', isActive: true,
      }),
    });
    const ppId = ppCreate.data?.data?.id || ppCreate.data?.id;
    ok(`POST /pickup-points: ${ppCreate.status} id=${ppId}`, (ppCreate.status === 200 || ppCreate.status === 201));

    if (ppId) {
      const ppGet = await f(`/pickup-points/${ppId}`, { headers: auth });
      ok(`GET /pickup-points/${ppId}: ${ppGet.status}`, ppGet.status === 200);

      const ppUpdate = await f(`/pickup-points/${ppId}`, {
        method: 'PUT', headers: auth,
        body: JSON.stringify({
          warehouseId: whId, name: `Pickup ${ts} Upd`, addressLine1: '456 Test',
          city: 'Dhaka', postalCode: '1000', phone: '01700000001',
          latitude: null, longitude: null, openingTime: '08:00', closingTime: '22:00', isActive: true,
        }),
      });
      ok(`PUT pickup-point: ${ppUpdate.status}`, ppUpdate.status === 200 || ppUpdate.status === 204);

      const ppDel = await f(`/pickup-points/${ppId}`, { method: 'DELETE', headers: auth });
      ok(`DELETE pickup-point: ${ppDel.status}`, ppDel.status === 200 || ppDel.status === 204);
    }
  } else {
    ok('No warehouse for pickup point test', false);
  }
}

if (process.argv[1]?.includes('test-shipping')) {
  testShipping().then(() => console.log('\nShipping tests done.'));
}
