import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/wishlist-types', codeField: 'typeCode' });

export default function WishlistTypes() {
  return (
    <LookupManager
      title="Wishlist Types"
      subtitle="Manage wishlist type definitions"
      queryKey="wishlist-types"
      api={api}
      codeField="typeCode"
      nameField="displayName"
      columns={[
        { key: 'typeCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
      ]}
      formFields={[
        { key: 'typeCode', label: 'Type Code', type: 'text', required: true, placeholder: 'e.g., Default' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Default' },
      ]}
    />
  );
}
