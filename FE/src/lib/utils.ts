import type { Product, ProductStatus, OrderStatus } from "./types";

export function formatPrice(amount: number, currency: string = "USD"): string {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
  }).format(amount);
}

export function originalPrice(product: Product): number {
  return product.actualPrice ?? product.price;
}

export function effectivePrice(product: Product): number {
  return product.discountedPrice && product.discountedPrice > 0
    ? product.discountedPrice
    : product.price;
}

export function discountPercentage(product: Product): number {
  if (product.discountPercentage && product.discountPercentage > 0) {
    return product.discountPercentage;
  }

  const original = originalPrice(product);
  const effective = effectivePrice(product);
  if (original <= 0 || effective >= original) return 0;

  return Math.round(((original - effective) / original) * 100);
}

export function hasDiscount(product: Product): boolean {
  return discountPercentage(product) > 0 && effectivePrice(product) < originalPrice(product);
}

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
