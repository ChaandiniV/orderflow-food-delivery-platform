import { Link, NavLink, Outlet } from 'react-router-dom';
import { ShoppingBag } from 'lucide-react';

export function AppShell() {
  return (
    <div className="app-shell">
      <header className="topbar">
        <Link to="/" className="brand">
          <span className="brand-icon"><ShoppingBag size={22} /></span>
          <span>OrderFlow</span>
        </Link>
        <nav className="nav-links">
          <NavLink to="/" className={({ isActive }) => (isActive ? 'active' : '')}>Restaurants</NavLink>
          <NavLink to="/ops/orders" className={({ isActive }) => (isActive ? 'active' : '')}>Operations</NavLink>
        </nav>
      </header>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
