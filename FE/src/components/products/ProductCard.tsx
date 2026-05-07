"use client";

import Link from "next/link";
import { ShoppingCart, Download } from "lucide-react";
import type { Product } from "@/lib/types";
import { formatPrice } from "@/lib/utils";
import { ProductBadge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { useCart } from "@/context/CartContext";

export function ProductCard({ product }: { product: Product }) {
  const { addToCart } = useCart();

  const isActive = product.status === "Active";

  return (
    <div className="group flex flex-col rounded-xl bg-white shadow-sm ring-1 ring-gray-100 hover:shadow-md transition-shadow overflow-hidden">
      {/* Thumbnail */}
      <Link href={`/products/${product.id}`}>
        <div className="relative h-44 bg-gradient-to-br from-indigo-50 to-purple-50 flex items-center justify-center">
          <Download className="h-12 w-12 text-indigo-200 group-hover:text-indigo-300 transition-colors" />
          <div className="absolute top-3 left-3">
            <ProductBadge status={product.status} />
          </div>
        </div>
      </Link>

      {/* Content */}
      <div className="flex flex-col flex-1 gap-2 p-4">
        <Link href={`/products/${product.id}`}>
          <h3 className="font-semibold text-gray-900 line-clamp-1 hover:text-indigo-600 transition-colors">
            {product.name}
          </h3>
        </Link>
        <p className="text-sm text-gray-500 line-clamp-2 flex-1">
          {product.description}
        </p>
        <div className="flex items-center justify-between pt-2">
          <span className="text-lg font-bold text-indigo-600">
            {formatPrice(product.price, product.currency)}
          </span>
          <Button
            size="sm"
            variant={isActive ? "primary" : "ghost"}
            disabled={!isActive}
            onClick={() => addToCart(product)}
          >
            <ShoppingCart className="h-4 w-4" />
            {isActive ? "Add to cart" : "Unavailable"}
          </Button>
        </div>
      </div>
    </div>
  );
}
