// test-rbac.mjs — Roles, Permissions, Menus, Users
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

export async function testRbac(auth = {}) {
  console.log('\n🔑 RBAC TESTS');

  // ─── PERMISSIONS ───
  console.log('\n-- Permissions --');
  const permList = await f('/permissions', { headers: auth });
  ok(`GET /permissions: ${permList.status}`, permList.status === 200);

  const permModules = await f('/permissions/modules', { headers: auth });
  ok(`GET /permissions/modules: ${permModules.status}`, permModules.status === 200);

  const permCreate = await f('/permissions', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ permissionCode: `TEST_PERM_${ts}`, name: `Test Perm ${ts}`, module: 'Testing', description: 'E2E test', isActive: true }),
  });
  const permId = permCreate.data?.data?.id || permCreate.data?.id;
  ok(`POST /permissions: ${permCreate.status} id=${permId}`, (permCreate.status === 200 || permCreate.status === 201) && permId);

  if (permId) {
    const permGet = await f(`/permissions/${permId}`, { headers: auth });
    ok(`GET /permissions/${permId}: ${permGet.status}`, permGet.status === 200);

    const permUpdate = await f(`/permissions/${permId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ permissionCode: `TEST_PERM_${ts}`, name: `Test Perm ${ts} Upd`, module: 'Testing', description: 'Updated', isActive: true }),
    });
    ok(`PUT /permissions/${permId}: ${permUpdate.status}`, permUpdate.status === 200 || permUpdate.status === 204);
  }

  // ─── MENUS ───
  console.log('\n-- Menus --');
  const menuList = await f('/menus', { headers: auth });
  ok(`GET /menus: ${menuList.status}`, menuList.status === 200);

  const menuTree = await f('/menus/tree', { headers: auth });
  ok(`GET /menus/tree: ${menuTree.status}`, menuTree.status === 200);

  const menuCreate = await f('/menus', {
    method: 'POST', headers: auth,
    body: JSON.stringify({
      menuCode: `MENU_${ts}`, menuName: `TestMenu${ts}`, displayName: `Test Menu ${ts}`,
      menuUrl: '/test', iconClass: 'icon-test', displayOrder: 99, menuLevel: 1,
      permissionCode: null, parentMenuId: null, isActive: true, isVisible: true,
      isExternalLink: false, openInNewTab: false, description: 'E2E test',
    }),
  });
  const menuId = menuCreate.data?.data?.id || menuCreate.data?.id;
  ok(`POST /menus: ${menuCreate.status} id=${menuId}`, (menuCreate.status === 200 || menuCreate.status === 201) && menuId);

  if (menuId) {
    const menuGet = await f(`/menus/${menuId}`, { headers: auth });
    ok(`GET /menus/${menuId}: ${menuGet.status}`, menuGet.status === 200);

    const menuUpdate = await f(`/menus/${menuId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({
        menuCode: `MENU_${ts}`, menuName: `TestMenu${ts}Upd`, displayName: `Test Menu ${ts} Upd`,
        menuUrl: '/test-upd', iconClass: 'icon-test', displayOrder: 98, menuLevel: 1,
        permissionCode: null, parentMenuId: null, isActive: true, isVisible: true,
        isExternalLink: false, openInNewTab: false, description: 'Updated',
      }),
    });
    ok(`PUT /menus/${menuId}: ${menuUpdate.status}`, menuUpdate.status === 200 || menuUpdate.status === 204);
  }

  // ─── ROLES ───
  console.log('\n-- Roles --');
  const roleList = await f('/roles', { headers: auth });
  ok(`GET /roles: ${roleList.status}`, roleList.status === 200);

  const roleCreate = await f('/roles', {
    method: 'POST', headers: auth,
    body: JSON.stringify({ name: `TestRole ${ts}`, description: 'E2E test role', isActive: true }),
  });
  const roleId = roleCreate.data?.data?.id || roleCreate.data?.id;
  ok(`POST /roles: ${roleCreate.status} id=${roleId}`, (roleCreate.status === 200 || roleCreate.status === 201) && roleId);

  if (roleId) {
    const roleGet = await f(`/roles/${roleId}`, { headers: auth });
    ok(`GET /roles/${roleId}: ${roleGet.status}`, roleGet.status === 200);

    const roleUpdate = await f(`/roles/${roleId}`, {
      method: 'PUT', headers: auth,
      body: JSON.stringify({ name: `TestRole ${ts} Upd`, description: 'Updated', isActive: true }),
    });
    ok(`PUT /roles/${roleId}: ${roleUpdate.status}`, roleUpdate.status === 200 || roleUpdate.status === 204);

    // Assign permissions to role
    if (permId) {
      const rolePerm = await f(`/roles/${roleId}/permissions`, {
        method: 'PUT', headers: auth,
        body: JSON.stringify([{ permissionId: permId, isGranted: true }]),
      });
      ok(`PUT /roles/${roleId}/permissions: ${rolePerm.status}`, rolePerm.status === 200 || rolePerm.status === 204);
    }

    // Assign menus to role
    if (menuId) {
      const roleMenu = await f(`/roles/${roleId}/menus`, {
        method: 'PUT', headers: auth,
        body: JSON.stringify([{ menuId: menuId, isGranted: true }]),
      });
      ok(`PUT /roles/${roleId}/menus: ${roleMenu.status}`, roleMenu.status === 200 || roleMenu.status === 204);
    }
  }

  // ─── USERS ───
  console.log('\n-- Users --');
  const userList = await f('/users', { headers: auth });
  ok(`GET /users: ${userList.status}`, userList.status === 200);

  const users = userList.data?.data || [];
  const firstUser = users[0];
  const userId = firstUser?.id;

  if (userId) {
    const userGet = await f(`/users/${userId}`, { headers: auth });
    ok(`GET /users/${userId}: ${userGet.status}`, userGet.status === 200);

    const userMenus = await f(`/users/${userId}/menus`, { headers: auth });
    ok(`GET /users/${userId}/menus: ${userMenus.status}`, userMenus.status === 200);

    const userPerms = await f(`/users/${userId}/permissions`, { headers: auth });
    ok(`GET /users/${userId}/permissions: ${userPerms.status}`, userPerms.status === 200);

    // Assign roles to user
    if (roleId) {
      const userRoles = await f(`/users/${userId}/roles`, {
        method: 'PUT', headers: auth,
        body: JSON.stringify({ roleIds: [roleId] }),
      });
      ok(`PUT /users/${userId}/roles: ${userRoles.status}`, userRoles.status === 200 || userRoles.status === 204);
    }

    const userToggle = await f(`/users/${userId}/toggle-active`, { method: 'POST', headers: auth });
    ok(`POST /users/${userId}/toggle-active: ${userToggle.status}`, userToggle.status === 200 || userToggle.status === 204);

    // Toggle back
    await f(`/users/${userId}/toggle-active`, { method: 'POST', headers: auth });
  }

  // ─── CLEANUP ───
  console.log('\n-- Cleanup --');
  if (roleId) {
    const roleDel = await f(`/roles/${roleId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE role: ${roleDel.status}`, roleDel.status === 200 || roleDel.status === 204);
  }
  if (menuId) {
    const menuDel = await f(`/menus/${menuId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE menu: ${menuDel.status}`, menuDel.status === 200 || menuDel.status === 204);
  }
  if (permId) {
    const permDel = await f(`/permissions/${permId}`, { method: 'DELETE', headers: auth });
    ok(`DELETE permission: ${permDel.status}`, permDel.status === 200 || permDel.status === 204);
  }

  return { roleId, permId, menuId };
}

if (process.argv[1]?.includes('test-rbac')) {
  testRbac().then(() => console.log('\nRBAC tests done.'));
}
