export type Restaurant = {
  id: number;
  name: string;
  cuisine: string;
  area: string;
  rating: number;
  isActive: boolean;
};

export type RestaurantDetail = Restaurant & {
  menuItems: MenuItem[];
};

export type MenuItem = {
  id: number;
  restaurantId: number;
  name: string;
  description: string;
  price: number;
  category: string;
  isAvailable: boolean;
};

export type Customer = {
  id: number;
  name: string;
  phone: string;
  email: string;
  address: string;
};

export type CreateCustomer = Omit<Customer, 'id'>;

export type CartItem = {
  menuItem: MenuItem;
  quantity: number;
};

export type OrderItem = {
  id: number;
  menuItemId: number;
  menuItemName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
};

export type OrderStatus =
  | 'Placed'
  | 'Accepted'
  | 'Preparing'
  | 'ReadyForPickup'
  | 'OutForDelivery'
  | 'Delivered'
  | 'Cancelled';

export type OrderStatusEvent = {
  id: number;
  orderId: number;
  oldStatus: OrderStatus | null;
  newStatus: OrderStatus;
  note: string;
  createdAt: string;
};

export type Order = {
  id: number;
  customer: Customer;
  restaurant: Restaurant;
  status: OrderStatus;
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
  items: OrderItem[];
  statusEvents: OrderStatusEvent[];
};

export type OrderSummary = {
  id: number;
  customerName: string;
  restaurantName: string;
  status: OrderStatus;
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
};

export type CreateOrderRequest = {
  customer: CreateCustomer;
  restaurantId: number;
  items: Array<{
    menuItemId: number;
    quantity: number;
  }>;
};

export type UpdateOrderStatusRequest = {
  newStatus: OrderStatus;
  note?: string;
};
