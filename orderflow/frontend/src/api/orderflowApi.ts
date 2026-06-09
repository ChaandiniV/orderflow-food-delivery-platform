import axios from 'axios';
import type {
  CreateOrderRequest,
  MenuItem,
  Order,
  OrderStatusEvent,
  OrderSummary,
  Restaurant,
  RestaurantDetail,
  UpdateOrderStatusRequest
} from '../types/orderflow';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080/api/';

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

export async function getRestaurants(): Promise<Restaurant[]> {
  const response = await api.get<Restaurant[]>('restaurants');
  return response.data;
}

export async function getRestaurant(id: number): Promise<RestaurantDetail> {
  const response = await api.get<RestaurantDetail>(`restaurants/${id}`);
  return response.data;
}

export async function getRestaurantMenu(id: number): Promise<MenuItem[]> {
  const response = await api.get<MenuItem[]>(`restaurants/${id}/menu`);
  return response.data;
}

export async function createOrder(request: CreateOrderRequest): Promise<Order> {
  const response = await api.post<Order>('orders', request);
  return response.data;
}

export async function getOrder(id: number): Promise<Order> {
  const response = await api.get<Order>(`orders/${id}`);
  return response.data;
}

export async function getOrders(): Promise<OrderSummary[]> {
  const response = await api.get<OrderSummary[]>('orders');
  return response.data;
}

export async function updateOrderStatus(id: number, request: UpdateOrderStatusRequest): Promise<Order> {
  const response = await api.patch<Order>(`orders/${id}/status`, request);
  return response.data;
}

export async function getOrderEvents(id: number): Promise<OrderStatusEvent[]> {
  const response = await api.get<OrderStatusEvent[]>(`orders/${id}/events`);
  return response.data;
}
