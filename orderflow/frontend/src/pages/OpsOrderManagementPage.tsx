import { useEffect, useMemo, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getOrder, updateOrderStatus } from '../api/orderflowApi';
import { ErrorState } from '../components/ErrorState';
import { formatCurrency, formatDateTime } from '../components/formatters';
import { LoadingState } from '../components/LoadingState';
import { StatusBadge } from '../components/StatusBadge';
import { StatusTimeline } from '../components/StatusTimeline';
import type { Order, OrderStatus } from '../types/orderflow';

const allowedTransitions: Record<OrderStatus, OrderStatus[]> = {
  Placed: ['Accepted', 'Cancelled'],
  Accepted: ['Preparing', 'Cancelled'],
  Preparing: ['ReadyForPickup'],
  ReadyForPickup: ['OutForDelivery'],
  OutForDelivery: ['Delivered'],
  Delivered: [],
  Cancelled: []
};

export function OpsOrderManagementPage() {
  const { id } = useParams();
  const orderId = Number(id);
  const [order, setOrder] = useState<Order | null>(null);
  const [selectedStatus, setSelectedStatus] = useState<OrderStatus | ''>('');
  const [note, setNote] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!orderId) return;
    getOrder(orderId)
      .then((data) => {
        setOrder(data);
        setSelectedStatus(allowedTransitions[data.status][0] ?? '');
      })
      .catch(() => setError('Unable to load order management page.'))
      .finally(() => setLoading(false));
  }, [orderId]);

  const nextStatuses = useMemo(() => {
    if (!order) return [];
    return allowedTransitions[order.status];
  }, [order]);

  async function submitStatusUpdate() {
    if (!order || !selectedStatus) return;

    setSaving(true);
    setError(null);

    try {
      const updated = await updateOrderStatus(order.id, {
        newStatus: selectedStatus,
        note: note || `Status changed to ${selectedStatus}`
      });
      setOrder(updated);
      setSelectedStatus(allowedTransitions[updated.status][0] ?? '');
      setNote('');
    } catch {
      setError('Status update failed. The backend rejected this transition.');
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <LoadingState message="Loading order management..." />;
  if (error && !order) return <ErrorState message={error} />;
  if (!order) return <ErrorState message="Order not found." />;

  return (
    <section>
      <div className="page-title-row">
        <div>
          <p className="eyebrow">Order management</p>
          <h1>Manage Order #{order.id}</h1>
          <p className="muted">Created {formatDateTime(order.createdAt)}</p>
        </div>
        <StatusBadge status={order.status} />
      </div>

      {error && <ErrorState message={error} />}

      <div className="order-layout">
        <article className="detail-card">
          <h2>Order Details</h2>
          <p><strong>Customer:</strong> {order.customer.name}</p>
          <p><strong>Restaurant:</strong> {order.restaurant.name}</p>
          <p><strong>Total:</strong> {formatCurrency(order.totalAmount)}</p>
          <div className="line-items compact">
            {order.items.map((item) => (
              <div className="line-item" key={item.id}>
                <span>{item.quantity} × {item.menuItemName}</span>
                <strong>{formatCurrency(item.lineTotal)}</strong>
              </div>
            ))}
          </div>
        </article>

        <article className="detail-card">
          <h2>Update Status</h2>
          {nextStatuses.length === 0 ? (
            <p className="muted">This order is in a terminal state and cannot be updated.</p>
          ) : (
            <div className="status-form">
              <label>
                Next status
                <select value={selectedStatus} onChange={(event) => setSelectedStatus(event.target.value as OrderStatus)}>
                  {nextStatuses.map((status) => (
                    <option value={status} key={status}>{status}</option>
                  ))}
                </select>
              </label>
              <label>
                Note
                <textarea value={note} onChange={(event) => setNote(event.target.value)} placeholder="Restaurant started preparing the order" />
              </label>
              <button className="primary-button full-width" disabled={saving || !selectedStatus} onClick={submitStatusUpdate}>
                {saving ? 'Updating...' : 'Update Status'}
              </button>
            </div>
          )}
        </article>
      </div>

      <article className="detail-card spacing-top">
        <h2>Status Event History</h2>
        <StatusTimeline events={order.statusEvents} />
      </article>
    </section>
  );
}
