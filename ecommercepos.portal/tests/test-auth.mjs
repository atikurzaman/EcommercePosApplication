// test-auth.mjs — Auth & Registration endpoint tests
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

export async function testAuth() {
  console.log('\n🔐 AUTH TESTS');

  // Register
  console.log('\n-- Register --');
  const reg = await f('/auth/register', {
    method: 'POST',
    body: JSON.stringify({
      email: `test${ts}@test.com`,
      password: 'Test@12345',
      firstName: 'Test',
      lastName: `User${ts}`,
      phone: '01700000000',
    }),
  });
  ok(`Register: ${reg.status}`, reg.status === 200 || reg.status === 201);

  // Login
  console.log('\n-- Login --');
  const login = await f('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email: `test${ts}@test.com`, password: 'Test@12345' }),
  });
  const token = login.data?.data?.accessToken || login.data?.data?.token || login.data?.accessToken || login.data?.token;
  ok(`Login: ${login.status}`, login.status === 200 && token);

  if (!token) {
    console.log('  ⚠️  No token — skipping authenticated tests');
    return { token: null };
  }

  const auth = { Authorization: `Bearer ${token}` };

  // Get current user
  console.log('\n-- Current User --');
  const me = await f('/auth/me', { headers: auth });
  const userId = me.data?.data?.id || me.data?.id;
  ok(`GET /auth/me: ${me.status}`, me.status === 200 && userId);

  // Change password
  console.log('\n-- Change Password --');
  const cp = await f('/auth/change-password', {
    method: 'POST',
    headers: auth,
    body: JSON.stringify({ currentPassword: 'Test@12345', newPassword: 'Test@12345' }),
  });
  ok(`Change password: ${cp.status}`, cp.status === 200 || cp.status === 204);

  return { token, userId, auth };
}

// Run standalone
if (process.argv[1]?.includes('test-auth')) {
  testAuth().then(r => console.log('\nDone. Token:', r.token ? 'obtained' : 'MISSING'));
}
