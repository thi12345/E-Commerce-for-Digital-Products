"use client";

import Link from "next/link";
import { useState, useEffect } from "react";
import { useMutation } from "@tanstack/react-query";
import { Trash2, ShoppingBag, Minus, Plus, ArrowLeft } from "lucide-react";
import { useCart } from "@/context/CartContext";
import { ordersApi } from "@/lib/api";
import { effectivePrice, formatPrice, hasDiscount, originalPrice, productCurrency } from "@/lib/utils";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { useRouter } from "next/navigation";

export default function CartPage() {
  const { items, totalItems, totalPrice, currency, updateQuantity, removeFromCart, clearCart } =
    useCart();
  const router = useRouter();
  const [customerId, setCustomerId] = useState("");

  useEffect(() => {
    const stored = localStorage.getItem("customerId");
    if (stored) {
      setCustomerId(stored);
    } else {
      const id = crypto.randomUUID();
      localStorage.setItem("customerId", id);
      setCustomerId(id);
    }
  }, []);

  const { mutate: placeOrder, isPending, isError, error } = useMutation({
    mutationFn: () =>
      ordersApi.place({
        customerId,
        items: items.map((i) => ({ productId: i.product.id, quantity: i.quantity })),
      }),
    onSuccess: (order) => {
      clearCart();
      router.push(`/orders/${order.id}`);
    },
  });

  if (items.length === 0) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-10">
        <EmptyState
          icon={<ShoppingBag className="h-16 w-16" />}
          title="Your cart is empty"
          description="Add some digital products to get started."
          action={
            <Link href="/products">
              <Button>Browse Products</Button>
            </Link>
          }
        />
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-5xl px-4 py-10 sm:px-6 lg:px-8">
      <Link
        href="/products"
        className="mb-6 inline-flex items-center gap-1 text-sm text-gray-500 hover:text-indigo-600"
      >
        <ArrowLeft className="h-4 w-4" /> Continue Shopping
      </Link>

      <h1 className="mb-8 text-3xl font-bold text-gray-900">
        Shopping Cart{" "}
        <span className="text-gray-400 text-xl font-normal">({totalItems} items)</span>
      </h1>

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-3">
        {/* Cart items */}
        <div className="lg:col-span-2 space-y-4">
          {items.map(({ product, quantity }) => (
            <div
              key={product.id}
              className="flex items-start gap-4 rounded-xl bg-white p-4 shadow-sm ring-1 ring-gray-100"
            >
              {/* Icon */}
              <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-lg bg-indigo-50">
                <ShoppingBag className="h-7 w-7 text-indigo-400" />
              </div>

              {/* Info */}
              <div className="flex flex-1 flex-col gap-1 min-w-0">
                <Link
                  href={`/products/${product.id}`}
                  className="font-semibold text-gray-900 hover:text-indigo-600 line-clamp-1"
                >
                  {product.name}
                </Link>
                <p className="text-xs text-gray-400 line-clamp-1">
                  {product.aboutProduct ?? product.description}
                </p>
                <div className="mt-1 flex items-center gap-2">
                  <p className="text-sm font-bold text-indigo-600">
                    {formatPrice(effectivePrice(product), productCurrency(product))}
                  </p>
                  {hasDiscount(product) && (
                    <p className="text-xs text-gray-400 line-through">
                      {formatPrice(originalPrice(product), productCurrency(product))}
                    </p>
                  )}
                </div>
              </div>

              {/* Qty + Remove */}
              <div className="flex flex-col items-end gap-3 shrink-0">
                <button
                  onClick={() => removeFromCart(product.id)}
                  className="text-gray-300 hover:text-red-500 transition-colors"
                >
                  <Trash2 className="h-4 w-4" />
                </button>
                <div className="flex items-center rounded-lg ring-1 ring-gray-200 overflow-hidden">
                  <button
                    className="px-2 py-1 hover:bg-gray-100 transition-colors"
                    onClick={() => updateQuantity(product.id, quantity - 1)}
                  >
                    <Minus className="h-3 w-3" />
                  </button>
                  <span className="px-3 text-sm font-medium">{quantity}</span>
                  <button
                    className="px-2 py-1 hover:bg-gray-100 transition-colors"
                    onClick={() => updateQuantity(product.id, quantity + 1)}
                  >
                    <Plus className="h-3 w-3" />
                  </button>
                </div>
                <p className="text-xs text-gray-400">
                  = {formatPrice(effectivePrice(product) * quantity, productCurrency(product))}
                </p>
              </div>
            </div>
          ))}
        </div>

        {/* Order summary */}
        <div className="lg:col-span-1">
          <div className="sticky top-20 rounded-xl bg-white p-6 shadow-sm ring-1 ring-gray-100">
            <h2 className="mb-4 text-lg font-semibold text-gray-900">
              Order Summary
            </h2>

            <div className="space-y-3 text-sm">
              {items.map(({ product, quantity }) => (
                <div key={product.id} className="flex justify-between text-gray-600">
                  <span className="line-clamp-1 flex-1 mr-2">{product.name} × {quantity}</span>
                  <span className="shrink-0">
                    {formatPrice(effectivePrice(product) * quantity, productCurrency(product))}
                  </span>
                </div>
              ))}
            </div>

            <div className="my-4 border-t border-gray-100" />

            <div className="flex justify-between text-lg font-bold text-gray-900">
              <span>Total</span>
              <span className="text-indigo-600">
                {formatPrice(totalPrice, currency)}
              </span>
            </div>

            <div className="mt-4">
              <label className="block text-xs font-medium text-gray-500 mb-1">
                Customer ID
              </label>
              <input
                type="text"
                value={customerId}
                onChange={(e) => setCustomerId(e.target.value)}
                className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
              />
            </div>

            {isError && (
              <p className="mt-3 text-xs text-red-600 bg-red-50 rounded-lg px-3 py-2">
                {(error as Error)?.message ?? "Failed to place order. Try again."}
              </p>
            )}

            <Button
              className="mt-4 w-full"
              size="lg"
              loading={isPending}
              onClick={() => placeOrder()}
              disabled={!customerId.trim()}
            >
              Place Order
            </Button>

            <button
              onClick={clearCart}
              className="mt-2 w-full text-xs text-gray-400 hover:text-red-500 transition-colors py-1"
            >
              Clear Cart
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
