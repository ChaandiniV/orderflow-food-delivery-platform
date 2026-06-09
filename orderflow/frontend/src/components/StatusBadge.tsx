import type { OrderStatus } from '../types/orderflow';

const statusClassMap: Record<OrderStatus, string> = {
  Placed: 'badge badge-blue',
  Accepted: 'badge badge-purple',
  Preparing: 'badge badge-orange',
  ReadyForPickup: 'badge badge-amber',
  OutForDelivery: 'badge badge-teal',
  Delivered: 'badge badge-green',
  Cancelled: 'badge badge-red'
};

export function StatusBadge({ status }: { status: OrderStatus }) {
  return <span className={statusClassMap[status]}>{status}</span>;
}
