import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getOrders } from '../api/orderflowApi';
import { ErrorState } from '../components/ErrorState';
import { formatCurrency, formatDateTime } from '../components/formatters';
import { LoadingState } from '../components/LoadingState';
import { StatusBadge } from '../components/StatusBadge';
import type { OrderSummary } from '../types/orderflow';

export function OpsOrdersPage() {
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getOrders()
      .then(setOrders)
      .catch(() => setError('Unable to load operations dashboard.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingState message="Loading orders..." />;
  if (error) return <ErrorState message={error} />;

  return (
    <section>
      <div className="page-title-row">
        <div>
          <p className="eyebrow">Restaurant operations</p>
          <h1>Incoming Orders</h1>
          <p className="muted">View recent orders and manage status updates.</p>
        </div>
      </div>

      <div className="table-card">
        <table>
          <thead>
            <tr>
              <th>Order ID</th>
              <th>Customer</th>
              <th>Restaurant</th>
              <th>Total</th>
              <th>Status</th>
              <th>Created</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {orders.map((order) => (
              <tr key={order.id}>
                <td>#{order.id}</td>
                <td>{order.customerName}</td>
                <td>{order.restaurantName}</td>
                <td>{formatCurrency(order.totalAmount)}</td>
                <td><StatusBadge status={order.status} /></td>
                <td>{formatDateTime(order.createdAt)}</td>
                <td><Link className="table-link" to={`/ops/orders/${order.id}`}>View / Update</Link></td>
              </tr>
            ))}
          </tbody>
        </table>
        {orders.length === 0 && <p className="empty-state">No orders yet. Place an order from the customer flow first.</p>}
      </div>
    </section>
  );
}
