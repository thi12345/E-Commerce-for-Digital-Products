import type { Product, ProductStatus, OrderStatus, ProductVariant } from "./types";

export function formatPrice(amount: number, currency: string = "USD"): string {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
  }).format(amount);
}

export function selectedVariant(product: Product): ProductVariant | undefined {
  return (
    product.variants?.find((variant) => variant.isDefault) ??
    product.variants?.slice().sort((a, b) => a.discountedPrice - b.discountedPrice)[0]
  );
}

export function originalPrice(product: Product): number {
  return selectedVariant(product)?.actualPrice ?? product.actualPrice ?? product.price ?? 0;
}

export function effectivePrice(product: Product): number {
  const variant = selectedVariant(product);
  if (variant) {
    return variant.discountedPrice > 0 ? variant.discountedPrice : variant.actualPrice;
  }

  return product.discountedPrice && product.discountedPrice > 0
    ? product.discountedPrice
    : product.price ?? product.actualPrice ?? 0;
}

export function discountPercentage(product: Product): number {
  const variant = selectedVariant(product);
  if (variant?.discountPercentage && variant.discountPercentage > 0) {
    return variant.discountPercentage;
  }

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

export function productCurrency(product: Product): string {
  return selectedVariant(product)?.currency ?? product.currency ?? "USD";
}

export function productLink(product: Product): string | undefined {
  return selectedVariant(product)?.productLink ?? product.productLink;
}

export function productStock(product: Product): number | undefined {
  return selectedVariant(product)?.stock;
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
