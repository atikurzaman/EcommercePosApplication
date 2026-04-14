import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/currencies', codeField: 'currencyCode' });

export default function Currencies() {
  return (
    <LookupManager
      title="Currencies"
      subtitle="Manage currency definitions and exchange rates"
      queryKey="currencies"
      api={api}
      codeField="currencyCode"
      nameField="name"
      columns={[
        { key: 'currencyCode', label: 'Code' },
        { key: 'name', label: 'Name' },
        { key: 'symbol', label: 'Symbol' },
        { key: 'exchangeRate', label: 'Exchange Rate' },
        { key: 'decimalPlaces', label: 'Decimals' },
        { key: 'isBaseCurrency', label: 'Base', render: (v) => (v ? 'Yes' : 'No') },
        { key: 'isActive', label: 'Active', render: (v) => (v ? 'Yes' : 'No') },
      ]}
      formFields={[
        { key: 'currencyCode', label: 'Currency Code', type: 'text', required: true, placeholder: 'e.g., USD' },
        { key: 'name', label: 'Name', type: 'text', required: true, placeholder: 'e.g., US Dollar' },
        { key: 'symbol', label: 'Symbol', type: 'text', required: true, placeholder: 'e.g., $' },
        { key: 'exchangeRate', label: 'Exchange Rate', type: 'number', required: true, placeholder: '1.0' },
        { key: 'decimalPlaces', label: 'Decimal Places', type: 'number', required: true, placeholder: '2' },
        { key: 'isBaseCurrency', label: 'Is Base Currency', type: 'checkbox' },
        { key: 'isActive', label: 'Is Active', type: 'checkbox' },
      ]}
    />
  );
}
