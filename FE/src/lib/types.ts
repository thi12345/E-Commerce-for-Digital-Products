export type ProductStatus = "Draft" | "Active" | "Inactive";
export type OrderStatus = "Pending" | "Paid" | "Completed" | "Cancelled";

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  actualPrice?: number;
  discountedPrice?: number;
  discountPercentage?: number;
  aboutProduct?: string;
  imgLink?: string;
  productLink?: string;
  rating?: number;
  ratingCount?: number;
  currency: string;
  downloadUrl: string;
  status: ProductStatus;
  createdAt: string;
}

export interface OrderItem {
  productId: string;
  productName: string;
  unitPrice: number;
  currency: string;
  quantity: number;
  totalPrice: number;
}

export interface Order {
  id: string;
  customerId: string;
  status: OrderStatus;
  totalAmount: number;
  currency: string;
  items: OrderItem[];
  createdAt: string;
}

export interface CartItem {
  product: Product;
  quantity: number;
}

export interface CreateProductPayload {
  name: string;
  description: string;
  price: number;
  actualPrice?: number;
  discountedPrice?: number;
  discountPercentage?: number;
  aboutProduct?: string;
  imgLink?: string;
  productLink?: string;
  currency: string;
  downloadUrl: string;
}

export interface UpdateProductPayload extends CreateProductPayload {
  id?: string;
}

export type ProductSortBy =
  | "Default"
  | "PriceAsc"
  | "PriceDesc";

export interface PlaceOrderPayload {
  customerId: string;
  items: { productId: string; quantity: number }[];
}

export type UserRole = "Customer" | "Admin";

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  createdAt: string;
}

export interface CreateUserPayload {
  name: string;
  email: string;
  role: UserRole;
}

export interface UpdateUserPayload {
  name: string;
  email: string;
  role: UserRole;
}
