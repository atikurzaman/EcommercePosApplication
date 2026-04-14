import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/discount-types', codeField: 'typeCode' });

export default function DiscountTypes() {
  return (
    <LookupManager
      title="Discount Types"
      subtitle="Manage discount type definitions"
      queryKey="discount-types"
      api={api}
      codeField="typeCode"
      nameField="displayName"
      columns={[
        { key: 'typeCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
      ]}
      formFields={[
        { key: 'typeCode', label: 'Type Code', type: 'text', required: true, placeholder: 'e.g., Percentage' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Percentage' },
      ]}
    />
  );
}
