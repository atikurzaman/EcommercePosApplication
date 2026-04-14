import LookupManager from '@/components/LookupManager';
import { createLookupApi } from '@/api/lookupApi';

const api = createLookupApi({ endpoint: '/colors', codeField: 'id' });

export default function Colors() {
  return (
    <LookupManager
      title="Colors"
      subtitle="Manage product color options"
      queryKey="colors"
      api={api}
      codeField="id"
      nameField="name"
      columns={[
        { key: 'name', label: 'Name' },
        {
          key: 'hexCode',
          label: 'Color',
          render: (v) =>
            v ? (
              <div className="flex items-center gap-2">
                <div
                  className="w-5 h-5 rounded border"
                  style={{ backgroundColor: v }}
                />
                <code className="text-sm">{v}</code>
              </div>
            ) : (
              '-'
            ),
        },
        { key: 'isActive', label: 'Active', render: (v) => (v ? 'Yes' : 'No') },
      ]}
      formFields={[
        { key: 'name', label: 'Name', type: 'text', required: true, placeholder: 'e.g., Red' },
        { key: 'hexCode', label: 'Hex Code', type: 'text', required: true, placeholder: 'e.g., #FF0000' },
        { key: 'isActive', label: 'Is Active', type: 'checkbox' },
      ]}
    />
  );
}
