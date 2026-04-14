import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/payment-methods', codeField: 'methodCode' });

export default function PaymentMethods() {
  return (
    <LookupManager
      title="Payment Methods"
      subtitle="Manage available payment methods"
      queryKey="payment-methods"
      api={api}
      codeField="methodCode"
      nameField="displayName"
      columns={[
        { key: 'methodCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
        { key: 'isOnline', label: 'Online', render: (v) => (v ? 'Yes' : 'No') },
        { key: 'isActive', label: 'Active', render: (v) => (v ? 'Yes' : 'No') },
        { key: 'sortOrder', label: 'Sort Order' },
      ]}
      formFields={[
        { key: 'methodCode', label: 'Method Code', type: 'text', required: true, placeholder: 'e.g., CreditCard' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Credit Card' },
        { key: 'sortOrder', label: 'Sort Order', type: 'number', placeholder: '0' },
        { key: 'isOnline', label: 'Is Online', type: 'checkbox' },
        { key: 'isActive', label: 'Is Active', type: 'checkbox' },
      ]}
    />
  );
}
