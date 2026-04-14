import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/order-statuses', codeField: 'statusCode' });

export default function OrderStatuses() {
  return (
    <LookupManager
      title="Order Statuses"
      subtitle="Manage order lifecycle states"
      queryKey="order-statuses"
      api={api}
      codeField="statusCode"
      nameField="displayName"
      columns={[
        { key: 'statusCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
        { key: 'description', label: 'Description' },
        { key: 'sortOrder', label: 'Sort Order' },
        { key: 'isTerminal', label: 'Terminal', render: (v) => (v ? 'Yes' : 'No') },
      ]}
      formFields={[
        { key: 'statusCode', label: 'Status Code', type: 'text', required: true, placeholder: 'e.g., Pending' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Pending' },
        { key: 'description', label: 'Description', type: 'text', placeholder: 'Optional description' },
        { key: 'sortOrder', label: 'Sort Order', type: 'number', required: true, placeholder: '0' },
        { key: 'isTerminal', label: 'Is Terminal', type: 'checkbox' },
      ]}
    />
  );
}
