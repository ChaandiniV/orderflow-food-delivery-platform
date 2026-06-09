import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { MapPin, Star } from 'lucide-react';
import { getRestaurants } from '../api/orderflowApi';
import { ErrorState } from '../components/ErrorState';
import { LoadingState } from '../components/LoadingState';
import type { Restaurant } from '../types/orderflow';

export function RestaurantListPage() {
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getRestaurants()
      .then(setRestaurants)
      .catch(() => setError('Unable to load restaurants. Please confirm the backend API is running.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingState message="Loading restaurants..." />;
  if (error) return <ErrorState message={error} />;

  return (
    <section>
      <div className="hero-card">
        <div>
          <p className="eyebrow">Food delivery workflow MVP</p>
          <h1>OrderFlow</h1>
          <p className="hero-subtitle">Food Delivery Order Management Platform</p>
        </div>
        <div className="hero-metric">
          <strong>{restaurants.length}</strong>
          <span>active restaurants</span>
        </div>
      </div>

      <div className="section-heading">
        <h2>Restaurants</h2>
        <p>Browse active restaurants and place demo customer orders.</p>
      </div>

      <div className="restaurant-grid">
        {restaurants.map((restaurant) => (
          <article className="restaurant-card" key={restaurant.id}>
            <div className="restaurant-avatar">{restaurant.name.slice(0, 2).toUpperCase()}</div>
            <div className="restaurant-card-body">
              <h3>{restaurant.name}</h3>
              <p>{restaurant.cuisine}</p>
              <div className="meta-row">
                <span><MapPin size={16} /> {restaurant.area}</span>
                <span><Star size={16} /> {restaurant.rating.toFixed(1)}</span>
              </div>
              <Link className="primary-button full-width" to={`/restaurants/${restaurant.id}`}>
                View Menu
              </Link>
            </div>
          </article>
        ))}
      </div>
    </section>
  );
}
