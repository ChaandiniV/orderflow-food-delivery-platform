import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ErrorState } from '../components/ErrorState';
import { formatCurrency, formatDateTime } from '../components/formatters';
import { LoadingState } from '../components/LoadingState';
import { StatusBadge } from '../components/StatusBadge';
import { StatusTimeline } from '../components/StatusTimeline';
import { getOrder } from '../api/orderflowApi';
import type { Order } from '../types/orderflow';

export function OrderTrackingPage() {
  const { id } = useParams();
  const orderId = Number(id);
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!orderId) return;
    getOrder(orderId)
      .then(setOrder)
      .catch(() => setError('Unable to load order details.'))
      .finally(() => setLoading(false));
  }, [orderId]);

  if (loading) return <LoadingState message="Loading order..." />;
  if (error || !order) return <ErrorState message={error ?? 'Order not found.'} />;

  return (
    <section>
      <div className="page-title-row">
        <div>
          <p className="eyebrow">Order confirmation</p>
          <h1>Order #{order.id}</h1>
          <p className="muted">Created {formatDateTime(order.createdAt)}</p>
        </div>
        <StatusBadge status={order.status} />
      </div>

      <div className="detail-grid">
        <article className="detail-card">
          <h2>Customer</h2>
          <p><strong>{order.customer.name}</strong></p>
          <p>{order.customer.phone}</p>
          <p>{order.customer.email}</p>
          <p>{order.customer.address}</p>
        </article>

        <article className="detail-card">
          <h2>Restaurant</h2>
          <p><strong>{order.restaurant.name}</strong></p>
          <p>{order.restaurant.cuisine}</p>
          <p>{order.restaurant.area}</p>
          <p>Rating {order.restaurant.rating.toFixed(1)}</p>
        </article>
      </div>

      <div className="order-layout">
        <article className="detail-card">
          <h2>Ordered Items</h2>
          <div className="line-items">
            {order.items.map((item) => (
              <div className="line-item" key={item.id}>
                <div>
                  <strong>{item.menuItemName}</strong>
                  <p>{item.quantity} × {formatCurrency(item.unitPrice)}</p>
                </div>
                <strong>{formatCurrency(item.lineTotal)}</strong>
              </div>
            ))}
          </div>
          <div className="total-row large">
            <span>Total</span>
            <strong>{formatCurrency(order.totalAmount)}</strong>
          </div>
        </article>

        <article className="detail-card">
          <h2>Status Timeline</h2>
          <StatusTimeline events={order.statusEvents} />
          <Link className="secondary-button full-width" to={`/ops/orders/${order.id}`}>Open in Operations</Link>
        </article>
      </div>
    </section>
  );
}
