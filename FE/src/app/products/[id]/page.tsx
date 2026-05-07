"use client";

import { use } from "react";
import { useQuery } from "@tanstack/react-query";
import { ShoppingCart, Download, ArrowLeft, Link as LinkIcon } from "lucide-react";
import Link from "next/link";
import { productsApi } from "@/lib/api";
import { formatPrice } from "@/lib/utils";
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

  return (
    <div className="mx-auto max-w-5xl px-4 py-10 sm:px-6 lg:px-8">
      {/* Breadcrumb */}
      <Link
        href="/products"
        className="mb-6 inline-flex items-center gap-1 text-sm text-gray-500 hover:text-indigo-600"
      >
        <ArrowLeft className="h-4 w-4" /> Back to Products
      </Link>

      <div className="grid grid-cols-1 gap-10 lg:grid-cols-2">
        {/* Product image / icon */}
        <div className="flex h-80 items-center justify-center rounded-2xl bg-gradient-to-br from-indigo-50 to-purple-100 lg:h-full lg:min-h-64">
          <Download className="h-20 w-20 text-indigo-300" />
        </div>

        {/* Product info */}
        <div className="flex flex-col gap-5">
          <div className="flex items-start justify-between gap-4">
            <h1 className="text-3xl font-bold text-gray-900 leading-tight">
              {product.name}
            </h1>
            <ProductBadge status={product.status} />
          </div>

          <p className="text-gray-600 leading-relaxed">{product.description}</p>

          <div className="rounded-xl bg-gray-50 p-4">
            <p className="text-sm text-gray-500">Price</p>
            <p className="text-3xl font-bold text-indigo-600 mt-1">
              {formatPrice(product.price, product.currency)}
            </p>
          </div>

          {product.downloadUrl && (
            <div className="flex items-center gap-2 rounded-lg border border-dashed border-gray-200 px-3 py-2 text-sm text-gray-500">
              <LinkIcon className="h-4 w-4 shrink-0" />
              <span className="truncate">{product.downloadUrl}</span>
            </div>
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
