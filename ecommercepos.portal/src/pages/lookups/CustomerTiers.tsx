import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/customer-tiers', codeField: 'tierCode' });

export default function CustomerTiers() {
  return (
    <LookupManager
      title="Customer Tiers"
      subtitle="Manage customer loyalty tiers"
      queryKey="customer-tiers"
      api={api}
      codeField="tierCode"
      nameField="displayName"
      columns={[
        { key: 'tierCode', label: 'Code' },
        { key: 'displayName', label: 'Name' },
        { key: 'minLifetimeSpend', label: 'Min Spend' },
        { key: 'discountPct', label: 'Discount %' },
        { key: 'pointsMultiplier', label: 'Points Mult.' },
        { key: 'sortOrder', label: 'Sort Order' },
      ]}
      formFields={[
        { key: 'tierCode', label: 'Tier Code', type: 'text', required: true, placeholder: 'e.g., Gold' },
        { key: 'displayName', label: 'Display Name', type: 'text', required: true, placeholder: 'e.g., Gold Tier' },
        { key: 'minLifetimeSpend', label: 'Min Lifetime Spend', type: 'number', required: true, placeholder: '0' },
        { key: 'discountPct', label: 'Discount Percentage', type: 'number', required: true, placeholder: '0' },
        { key: 'pointsMultiplier', label: 'Points Multiplier', type: 'number', required: true, placeholder: '1.0' },
        { key: 'sortOrder', label: 'Sort Order', type: 'number', required: true, placeholder: '0' },
      ]}
    />
  );
}
