import axios from "axios";
import type {
  Category,
  Product,
  Order,
  User,
  CreateProductPayload,
  UpdateProductPayload,
  PlaceOrderPayload,
  CreateUserPayload,
  UpdateUserPayload,
  ProductSortBy,
} from "./types";
import { API_BASE_URL, API_ENDPOINTS } from "./api-constants";

const http = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
});

type ProductListResponse = Product[] | {
  data?: Product[];
  items?: Product[];
  products?: Product[];
};

export interface GetProductsParams {
  name?: string;
  categoryId?: string;
  minRating?: number;
  minDiscountPercentage?: number;
  sortBy?: ProductSortBy;
  page?: number;
  pageSize?: number;
}

function normalizeProducts(data: ProductListResponse): Product[] {
  if (Array.isArray(data)) return data;

  return data.data ?? data.items ?? data.products ?? [];
}

function toQueryString(params?: GetProductsParams): string {
  const query = new URLSearchParams();

  Object.entries(params ?? {}).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      query.set(key, String(value));
    }
  });

  const next = query.toString();
  return next ? `?${next}` : "";
}

function toProductRequest(payload: CreateProductPayload) {
  const actualPrice = payload.actualPrice ?? payload.price ?? 0;
  const discountedPrice = payload.discountedPrice ?? actualPrice;

  return {
    name: payload.name,
    aboutProduct: payload.aboutProduct ?? payload.description ?? "",
    imgLink: payload.imgLink ?? "",
    variants:
      payload.variants && payload.variants.length > 0
        ? payload.variants
        : [
            {
              name: payload.variantName || "Default",
              actualPrice,
              discountedPrice,
              discountPercentage: payload.discountPercentage ?? 0,
              currency: payload.currency,
              productLink: payload.productLink ?? "",
              downloadUrl: payload.downloadUrl,
              stock: payload.stock ?? 0,
              isDefault: true,
              options: [],
            },
          ],
  };
}

export const productsApi = {
  getAll: () =>
    http.get<ProductListResponse>(API_ENDPOINTS.products.list).then((r) => normalizeProducts(r.data)),
  getFiltered: (params?: GetProductsParams) =>
    http.get<ProductListResponse>(`${API_ENDPOINTS.products.list}${toQueryString(params)}`).then((r) => normalizeProducts(r.data)),
  getById: (id: string) => http.get<Product>(API_ENDPOINTS.products.detail(id)).then((r) => r.data),
  create: (payload: CreateProductPayload) =>
    http.post<Product>(API_ENDPOINTS.products.list, toProductRequest(payload)).then((r) => r.data),
  update: (id: string, payload: UpdateProductPayload) =>
    http.put<Product>(API_ENDPOINTS.products.detail(id), toProductRequest(payload)).then((r) => r.data),
  activate: (id: string) =>
    http.patch<Product>(API_ENDPOINTS.products.activate(id)).then((r) => r.data),
  deactivate: (id: string) =>
    http.patch<Product>(API_ENDPOINTS.products.deactivate(id)).then((r) => r.data),
  delete: (id: string) => http.delete(API_ENDPOINTS.products.detail(id)),
};

export const categoriesApi = {
  getAll: () => http.get<Category[]>(API_ENDPOINTS.categories.list).then((r) => r.data),
};

export const ordersApi = {
  getById: (id: string) => http.get<Order>(API_ENDPOINTS.orders.detail(id)).then((r) => r.data),
  place: (payload: PlaceOrderPayload) =>
    http.post<Order>(API_ENDPOINTS.orders.list, payload).then((r) => r.data),
};

export const usersApi = {
  getAll: () => http.get<User[]>(API_ENDPOINTS.users.list).then((r) => r.data),
  getById: (id: string) => http.get<User>(API_ENDPOINTS.users.detail(id)).then((r) => r.data),
  create: (payload: CreateUserPayload) =>
    http.post<User>(API_ENDPOINTS.users.list, payload).then((r) => r.data),
  update: (id: string, payload: UpdateUserPayload) =>
    http.put<User>(API_ENDPOINTS.users.detail(id), payload).then((r) => r.data),
  delete: (id: string) => http.delete(API_ENDPOINTS.users.detail(id)),
};
