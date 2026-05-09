export const API_BASE_URL = "/api";

export const API_ENDPOINTS = {
  products: {
    list: "/products",
    detail: (id: string) => `/products/${id}`,
    activate: (id: string) => `/products/${id}/activate`,
    deactivate: (id: string) => `/products/${id}/deactivate`,
  },
  categories: {
    list: "/categories",
  },
  orders: {
    list: "/orders",
    detail: (id: string) => `/orders/${id}`,
  },
  users: {
    list: "/users",
    detail: (id: string) => `/users/${id}`,
  },
} as const;
