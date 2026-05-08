"use client";

import Link from "next/link";
import Image from "next/image";
import { ShoppingCart } from "lucide-react";
import type { Product } from "@/lib/types";
import {
  discountPercentage,
  effectivePrice,
  formatPrice,
  hasDiscount,
  originalPrice,
} from "@/lib/utils";
import { ProductBadge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { useCart } from "@/context/CartContext";

export function ProductCard({ product }: { product: Product }) {
  const { addToCart } = useCart();
  const isActive = product.status === "Active";
  const discounted = hasDiscount(product);
  const description = product.aboutProduct ?? product.description;

  return (
    <div className="group flex flex-col rounded-xl bg-white shadow-sm ring-1 ring-gray-100 hover:shadow-md transition-shadow overflow-hidden">
      {/* Thumbnail */}
      <Link href={`/products/${product.id}`} className="relative block h-44 bg-gray-100 overflow-hidden">
        {product.imgLink ? (
          <Image
            src={product.imgLink}
            alt={product.name}
            fill
            className="object-cover group-hover:scale-105 transition-transform duration-300"
            unoptimized
          />
        ) : (
          <div className="h-full flex items-center justify-center bg-gradient-to-br from-indigo-50 to-purple-50">
            <ShoppingCart className="h-12 w-12 text-indigo-200" />
          </div>
        )}

        {/* Badges overlay */}
        <div className="absolute top-2 left-2 flex flex-col gap-1">
          <ProductBadge status={product.status} />
          {discounted && (
            <span className="inline-block rounded-full bg-red-500 px-2 py-0.5 text-xs font-bold text-white">
              -{discountPercentage(product)}%
            </span>
          )}
        </div>
      </Link>

      {/* Content */}
      <div className="flex flex-col flex-1 gap-2 p-4">
        <Link href={`/products/${product.id}`}>
          <h3 className="font-semibold text-gray-900 line-clamp-2 text-sm leading-snug hover:text-indigo-600 transition-colors">
            {product.name}
          </h3>
        </Link>

        <p className="text-xs text-gray-500 line-clamp-2">{description}</p>

        {/* Price */}
        <div className="mt-auto pt-2 flex items-end justify-between gap-2">
          <div className="flex flex-col">
            <span className="text-base font-bold text-indigo-600">
              {formatPrice(effectivePrice(product), product.currency)}
            </span>
            {discounted && (
              <span className="text-xs text-gray-400 line-through">
                {formatPrice(originalPrice(product), product.currency)}
              </span>
            )}
          </div>

          <Button
            size="sm"
            variant={isActive ? "primary" : "ghost"}
            disabled={!isActive}
            onClick={() => addToCart(product)}
            className="shrink-0"
          >
            <ShoppingCart className="h-3.5 w-3.5" />
          </Button>
        </div>
      </div>
    </div>
  );
}
