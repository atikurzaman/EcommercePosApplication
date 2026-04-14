import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/return-statuses', codeField: 'statusCode' });

export default function ReturnStatuses() {
  return (
    <LookupManager
      title="Return Statuses"
      subtitle="Manage return request states"
      queryKey="return-statuses"
      api={api}
      codeField="statusCode"
      nameField="displayName"
      columns={[
        { key: 'statusCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
        { key: 'sortOrder', label: 'Sort Order' },
      ]}
      formFields={[
        { key: 'statusCode', label: 'Status Code', type: 'text', required: true, placeholder: 'e.g., Requested' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Requested' },
        { key: 'sortOrder', label: 'Sort Order', type: 'number', required: true, placeholder: '0' },
      ]}
    />
  );
}
