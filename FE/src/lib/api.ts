import axios from "axios";
import type {
  Product,
  Order,
  User,
  CreateProductPayload,
  UpdateProductPayload,
  PlaceOrderPayload,
  CreateUserPayload,
  UpdateUserPayload,
} from "./types";

const http = axios.create({
  baseURL: "/api",
  headers: { "Content-Type": "application/json" },
});

type ProductListResponse = Product[] | {
  data?: Product[];
  items?: Product[];
  products?: Product[];
};

function normalizeProducts(data: ProductListResponse): Product[] {
  if (Array.isArray(data)) return data;

  return data.data ?? data.items ?? data.products ?? [];
}

export const productsApi = {
  getAll: () =>
    http.get<ProductListResponse>("/products").then((r) => normalizeProducts(r.data)),
  getById: (id: string) => http.get<Product>(`/products/${id}`).then((r) => r.data),
  create: (payload: CreateProductPayload) =>
    http.post<Product>("/products", payload).then((r) => r.data),
  update: (id: string, payload: UpdateProductPayload) =>
    http.put<Product>(`/products/${id}`, payload).then((r) => r.data),
  activate: (id: string) =>
    http.patch<Product>(`/products/${id}/activate`).then((r) => r.data),
  deactivate: (id: string) =>
    http.patch<Product>(`/products/${id}/deactivate`).then((r) => r.data),
  delete: (id: string) => http.delete(`/products/${id}`),
};

export const ordersApi = {
  getById: (id: string) => http.get<Order>(`/orders/${id}`).then((r) => r.data),
  place: (payload: PlaceOrderPayload) =>
    http.post<Order>("/orders", payload).then((r) => r.data),
};

export const usersApi = {
  getAll: () => http.get<User[]>("/users").then((r) => r.data),
  getById: (id: string) => http.get<User>(`/users/${id}`).then((r) => r.data),
  create: (payload: CreateUserPayload) =>
    http.post<User>("/users", payload).then((r) => r.data),
  update: (id: string, payload: UpdateUserPayload) =>
    http.put<User>(`/users/${id}`, payload).then((r) => r.data),
  delete: (id: string) => http.delete(`/users/${id}`),
};
