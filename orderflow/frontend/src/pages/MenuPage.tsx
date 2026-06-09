import { useEffect, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Plus, Trash2 } from 'lucide-react';
import { createOrder, getRestaurant, getRestaurantMenu } from '../api/orderflowApi';
import { ErrorState } from '../components/ErrorState';
import { formatCurrency } from '../components/formatters';
import { LoadingState } from '../components/LoadingState';
import type { CartItem, CreateCustomer, MenuItem, RestaurantDetail } from '../types/orderflow';

const defaultCustomer: CreateCustomer = {
  name: 'Aisha Khan',
  phone: '+971501112233',
  email: 'aisha@example.com',
  address: 'Dubai Marina, Dubai'
};

export function MenuPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const restaurantId = Number(id);
  const [restaurant, setRestaurant] = useState<RestaurantDetail | null>(null);
  const [menu, setMenu] = useState<MenuItem[]>([]);
  const [cart, setCart] = useState<CartItem[]>([]);
  const [customer, setCustomer] = useState<CreateCustomer>(defaultCustomer);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!restaurantId) return;

    Promise.all([getRestaurant(restaurantId), getRestaurantMenu(restaurantId)])
      .then(([restaurantData, menuData]) => {
        setRestaurant(restaurantData);
        setMenu(menuData);
      })
      .catch(() => setError('Unable to load menu. Please confirm the backend API is running.'))
      .finally(() => setLoading(false));
  }, [restaurantId]);

  const total = useMemo(
    () => cart.reduce((sum, item) => sum + item.menuItem.price * item.quantity, 0),
    [cart]
  );

  function addToCart(menuItem: MenuItem) {
    setCart((current) => {
      const existing = current.find((item) => item.menuItem.id === menuItem.id);
      if (existing) {
        return current.map((item) =>
          item.menuItem.id === menuItem.id ? { ...item, quantity: item.quantity + 1 } : item
        );
      }
      return [...current, { menuItem, quantity: 1 }];
    });
  }

  function removeFromCart(menuItemId: number) {
    setCart((current) => current.filter((item) => item.menuItem.id !== menuItemId));
  }

  async function placeOrder() {
    if (cart.length === 0) {
      setError('Add at least one item before placing an order.');
      return;
    }

    setSubmitting(true);
    setError(null);

    try {
      const order = await createOrder({
        customer,
        restaurantId,
        items: cart.map((item) => ({ menuItemId: item.menuItem.id, quantity: item.quantity }))
      });
      navigate(`/orders/${order.id}`);
    } catch {
      setError('Order could not be placed. Check backend validation and try again.');
    } finally {
      setSubmitting(false);
    }
  }

  if (loading) return <LoadingState message="Loading menu..." />;
  if (!restaurant || error && menu.length === 0) return <ErrorState message={error ?? 'Restaurant not found.'} />;

  return (
    <section>
      <div className="page-title-row">
        <div>
          <p className="eyebrow">Customer ordering</p>
          <h1>{restaurant.name}</h1>
          <p className="muted">{restaurant.cuisine} · {restaurant.area} · Rating {restaurant.rating.toFixed(1)}</p>
        </div>
      </div>

      {error && <ErrorState message={error} />}

      <div className="menu-layout">
        <div className="menu-list">
          {menu.map((item) => (
            <article className="menu-card" key={item.id}>
              <div>
                <span className="category-pill">{item.category}</span>
                <h3>{item.name}</h3>
                <p>{item.description}</p>
                <strong>{formatCurrency(item.price)}</strong>
              </div>
              <button className="icon-button" onClick={() => addToCart(item)} aria-label={`Add ${item.name} to cart`}>
                <Plus size={18} />
              </button>
            </article>
          ))}
        </div>

        <aside className="cart-card">
          <h2>Cart Summary</h2>
          {cart.length === 0 ? (
            <p className="muted">Your cart is empty.</p>
          ) : (
            <div className="cart-items">
              {cart.map((item) => (
                <div className="cart-row" key={item.menuItem.id}>
                  <div>
                    <strong>{item.menuItem.name}</strong>
                    <p>{item.quantity} × {formatCurrency(item.menuItem.price)}</p>
                  </div>
                  <button className="ghost-icon" onClick={() => removeFromCart(item.menuItem.id)} aria-label="Remove item">
                    <Trash2 size={16} />
                  </button>
                </div>
              ))}
            </div>
          )}

          <div className="customer-form">
            <h3>Customer</h3>
            <input value={customer.name} onChange={(event) => setCustomer({ ...customer, name: event.target.value })} placeholder="Name" />
            <input value={customer.phone} onChange={(event) => setCustomer({ ...customer, phone: event.target.value })} placeholder="Phone" />
            <input value={customer.email} onChange={(event) => setCustomer({ ...customer, email: event.target.value })} placeholder="Email" />
            <textarea value={customer.address} onChange={(event) => setCustomer({ ...customer, address: event.target.value })} placeholder="Address" />
          </div>

          <div className="total-row">
            <span>Total</span>
            <strong>{formatCurrency(total)}</strong>
          </div>
          <button className="primary-button full-width" onClick={placeOrder} disabled={submitting || cart.length === 0}>
            {submitting ? 'Placing Order...' : 'Place Order'}
          </button>
        </aside>
      </div>
    </section>
  );
}
