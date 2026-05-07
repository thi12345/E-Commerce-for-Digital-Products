import type { ProductStatus, OrderStatus } from "./types";

export function formatPrice(amount: number, currency: string = "USD"): string {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
  }).format(amount);
};
  
export const statusColors: Record<ProductStatus, string> = {
  Active: "bg-emerald-100 text-emerald-700",
  Draft: "bg-amber-100 text-amber-700",
  Inactive: "bg-gray-100 text-gray-500",
};

export const orderStatusColors: Record<OrderStatus, string> = {
  Pending: "bg-amber-100 text-amber-700",
  Paid: "bg-blue-100 text-blue-700",
  Completed: "bg-emerald-100 text-emerald-700",
  Cancelled: "bg-red-100 text-red-600",
};
