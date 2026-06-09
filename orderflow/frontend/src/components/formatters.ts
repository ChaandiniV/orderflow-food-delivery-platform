export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-AE', {
    style: 'currency',
    currency: 'AED'
  }).format(amount);
}

export function formatDateTime(value: string): string {
  return new Date(value).toLocaleString();
}
