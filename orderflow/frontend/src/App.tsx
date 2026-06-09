import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { AppShell } from './components/AppShell';
import { MenuPage } from './pages/MenuPage';
import { OpsOrderManagementPage } from './pages/OpsOrderManagementPage';
import { OpsOrdersPage } from './pages/OpsOrdersPage';
import { OrderTrackingPage } from './pages/OrderTrackingPage';
import { RestaurantListPage } from './pages/RestaurantListPage';

const router = createBrowserRouter([
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <RestaurantListPage /> },
      { path: 'restaurants/:id', element: <MenuPage /> },
      { path: 'orders/:id', element: <OrderTrackingPage /> },
      { path: 'ops/orders', element: <OpsOrdersPage /> },
      { path: 'ops/orders/:id', element: <OpsOrderManagementPage /> }
    ]
  }
]);

export function App() {
  return <RouterProvider router={router} />;
}
