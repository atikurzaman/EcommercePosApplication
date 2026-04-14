import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/shipment-statuses', codeField: 'statusCode' });

export default function ShipmentStatuses() {
  return (
    <LookupManager
      title="Shipment Statuses"
      subtitle="Manage shipment tracking states"
      queryKey="shipment-statuses"
      api={api}
      codeField="statusCode"
      nameField="displayName"
      columns={[
        { key: 'statusCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
        { key: 'sortOrder', label: 'Sort Order' },
      ]}
      formFields={[
        { key: 'statusCode', label: 'Status Code', type: 'text', required: true, placeholder: 'e.g., Shipped' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Shipped' },
        { key: 'sortOrder', label: 'Sort Order', type: 'number', required: true, placeholder: '0' },
      ]}
    />
  );
}
