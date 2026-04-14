import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/payment-statuses', codeField: 'statusCode' });

export default function PaymentStatuses() {
  return (
    <LookupManager
      title="Payment Statuses"
      subtitle="Manage payment status definitions"
      queryKey="payment-statuses"
      api={api}
      codeField="statusCode"
      nameField="displayName"
      columns={[
        { key: 'statusCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
      ]}
      formFields={[
        { key: 'statusCode', label: 'Status Code', type: 'text', required: true, placeholder: 'e.g., Paid' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Paid' },
      ]}
    />
  );
}
