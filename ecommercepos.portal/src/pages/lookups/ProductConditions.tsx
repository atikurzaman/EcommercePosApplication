import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/product-conditions', codeField: 'conditionCode' });

export default function ProductConditions() {
  return (
    <LookupManager
      title="Product Conditions"
      subtitle="Manage product condition definitions"
      queryKey="product-conditions"
      api={api}
      codeField="conditionCode"
      nameField="displayName"
      columns={[
        { key: 'conditionCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
      ]}
      formFields={[
        { key: 'conditionCode', label: 'Condition Code', type: 'text', required: true, placeholder: 'e.g., New' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., New' },
      ]}
    />
  );
}
