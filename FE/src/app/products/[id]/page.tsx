"use client";

import { use } from "react";
import { useQuery } from "@tanstack/react-query";
import { ShoppingCart, ArrowLeft } from "lucide-react";
import Link from "next/link";
import { productsApi } from "@/lib/api";
import {
  discountPercentage,
  effectivePrice,
  formatPrice,
  hasDiscount,
  originalPrice,
} from "@/lib/utils";
import { ProductBadge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";
import { EmptyState } from "@/components/ui/EmptyState";
import { useCart } from "@/context/CartContext";

export default function ProductDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const { addToCart, items } = useCart();

  const { data: product, isLoading, isError } = useQuery({
    queryKey: ["products", id],
    queryFn: () => productsApi.getById(id),
  });

  if (isLoading) return <LoadingSpinner text="Loading product..." />;
  if (isError || !product) {
    return (
      <EmptyState
        title="Product not found"
        description="This product does not exist or could not be loaded."
        action={
          <Link href="/products">
            <Button variant="outline">Back to products</Button>
          </Link>
        }
      />
    );
  }

  const inCart = items.some((i) => i.product.id === product.id);
  const isActive = product.status === "Active";
  const discounted = hasDiscount(product);
  const description = product.aboutProduct ?? product.description;

  return (
    <div className="mx-auto max-w-5xl px-4 py-10 sm:px-6 lg:px-8">
      <Link
        href="/products"
        className="mb-6 inline-flex items-center gap-1 text-sm text-gray-500 hover:text-indigo-600"
      >
        <ArrowLeft className="h-4 w-4" /> Back to Products
      </Link>

      <div className="grid grid-cols-1 gap-10 lg:grid-cols-2">
        {/* Image */}
        <div className="relative h-80 overflow-hidden rounded-2xl bg-gray-100 lg:h-full lg:min-h-80">
          <div className="flex h-full items-center justify-center bg-gradient-to-br from-indigo-50 to-purple-100">
            <ShoppingCart className="h-20 w-20 text-indigo-200" />
          </div>
        </div>

        {/* Info */}
        <div className="flex flex-col gap-5">
          <div className="flex items-start justify-between gap-4">
            <h1 className="text-2xl font-bold text-gray-900 leading-tight">
              {product.name}
            </h1>
            <ProductBadge status={product.status} />
          </div>

          <p className="text-gray-600 leading-relaxed">{description}</p>

          {/* Price block */}
          <div className="rounded-xl bg-gray-50 p-4">
            <p className="text-xs font-medium text-gray-400 uppercase tracking-wide mb-2">
              Price
            </p>
            <div className="flex items-end gap-3 flex-wrap">
              <span className="text-3xl font-bold text-indigo-600">
                {formatPrice(effectivePrice(product), product.currency)}
              </span>
              {discounted && (
                <>
                  <span className="text-lg text-gray-400 line-through">
                    {formatPrice(originalPrice(product), product.currency)}
                  </span>
                  <span className="rounded-full bg-red-100 px-2.5 py-0.5 text-sm font-semibold text-red-600">
                    Save {discountPercentage(product)}%
                  </span>
                </>
              )}
            </div>
          </div>

          {product.productLink && (
            <a
              href={product.productLink}
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-1.5 text-sm text-indigo-600 hover:underline"
            >
              View on store
            </a>
          )}

          <div className="flex flex-col gap-3 sm:flex-row">
            <Button
              size="lg"
              className="flex-1"
              disabled={!isActive}
              onClick={() => addToCart(product)}
            >
              <ShoppingCart className="h-5 w-5" />
              {inCart ? "Add Another" : "Add to Cart"}
            </Button>
            <Link href="/cart" className="flex-1">
              <Button size="lg" variant="outline" className="w-full">
                View Cart
              </Button>
            </Link>
          </div>

          {!isActive && (
            <p className="text-sm text-amber-600 bg-amber-50 rounded-lg px-3 py-2">
              This product is currently {product.status.toLowerCase()} and not
              available for purchase.
            </p>
          )}
        </div>
      </div>
    </div>
  );
}
