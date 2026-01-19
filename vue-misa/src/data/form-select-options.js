export const selectOptions = {
  shiftCode: [
    { label: 'Chứa', value: 'contains' },
    { label: 'Không chứa', value: 'notcontains' },
    { label: 'Bắt đầu với', value: 'startswith' },
    { label: 'Kết thúc với', value: 'endswith' },
    { label: 'Bằng', value: 'endswith' },
  ],
  shiftName: [
    { label: 'Chứa', value: 'contains' },
    { label: 'Không chứa', value: 'notcontains' },
    { label: 'Bắt đầu với', value: 'startswith' },
    { label: 'Kết thúc với', value: 'endswith' },
    { label: 'Bằng', value: 'endswith' },
  ],
  workingTime: [
    { label: 'Bằng', value: 'contains' },
    { label: 'Khác', value: 'notcontains' },
    { label: 'Nhỏ hơn', value: 'startswith' },
    { label: 'Nhỏ hơn hoặc bằng', value: 'endswith' },
    { label: 'Lớn hơn', value: 'endswith' },
  ],
  breakingTime: [
    { label: 'Bằng', value: 'contains' },
    { label: 'Khác', value: 'notcontains' },
    { label: 'Nhỏ hơn', value: 'startswith' },
    { label: 'Nhỏ hơn hoặc bằng', value: 'endswith' },
    { label: 'Lớn hơn', value: 'endswith' },
  ],
  inactive: [
    { label: 'Đang sử dụng', value: true },
    { label: 'Ngừng sử dụng', value: false },
  ],
  createdBy: [
    { label: 'Chứa', value: 'contains' },
    { label: 'Không chứa', value: 'notcontains' },
    { label: 'Bắt đầu với', value: 'startswith' },
    { label: 'Kết thúc với', value: 'endswith' },
    { label: 'Bằng', value: 'endswith' },
  ],
  createdDate: [
    { label: 'Bằng', value: 'contains' },
    { label: 'Khác', value: 'notcontains' },
    { label: 'Nhỏ hơn', value: 'startswith' },
    { label: 'Nhỏ hơn hoặc bằng', value: 'endswith' },
    { label: 'Lớn hơn', value: 'endswith' },
  ],
  modifiedBy: [
    { label: 'Chứa', value: 'contains' },
    { label: 'Không chứa', value: 'notcontains' },
    { label: 'Bắt đầu với', value: 'startswith' },
    { label: 'Kết thúc với', value: 'endswith' },
    { label: 'Bằng', value: 'endswith' },
  ],
  modifiedDate: [
    { label: 'Bằng', value: 'contains' },
    { label: 'Khác', value: 'notcontains' },
    { label: 'Nhỏ hơn', value: 'startswith' },
    { label: 'Nhỏ hơn hoặc bằng', value: 'endswith' },
    { label: 'Lớn hơn', value: 'endswith' },
  ],
}
export const filterConfig = {
  shiftCode: {
    type: 'text',
  },
  shiftName: {
    type: 'text',
  },
  workingTime: {
    type: 'text',
  },
  breakingTime: {
    type: 'text',
  },
  inactive: {
    type: null,
  },
  createdBy: {
    type: 'text',
  },
  createdDate: {
    type: 'text',
  },
  modifiedBy: {
    type: 'text',
  },
  modifiedDate: {
    type: 'text',
  },
}
export const pageSizeOptions = [
  { label: '10', value: 10 },
  { label: '20', value: 20 },
  { label: '30', value: 30 },
  { label: '50', value: 50 },
  { label: '100', value: 100 },
]
