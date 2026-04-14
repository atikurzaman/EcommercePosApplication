import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/stock-movement-types', codeField: 'typeCode' });

export default function StockMovementTypes() {
  return (
    <LookupManager
      title="Stock Movement Types"
      subtitle="Manage stock movement type definitions"
      queryKey="stock-movement-types"
      api={api}
      codeField="typeCode"
      nameField="displayName"
      columns={[
        { key: 'typeCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
        { key: 'isInbound', label: 'Inbound', render: (v) => (v ? 'Yes' : 'No') },
      ]}
      formFields={[
        { key: 'typeCode', label: 'Type Code', type: 'text', required: true, placeholder: 'e.g., Purchase' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Purchase Receipt' },
        { key: 'isInbound', label: 'Is Inbound', type: 'checkbox' },
      ]}
    />
  );
}
