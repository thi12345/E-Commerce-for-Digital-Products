"use client";

import { use } from "react";
import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { CheckCircle, Package, ArrowRight } from "lucide-react";
import { ordersApi } from "@/lib/api";
import { formatPrice } from "@/lib/utils";
import { OrderBadge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";
import { EmptyState } from "@/components/ui/EmptyState";

export default function OrderDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);

  const { data: order, isLoading, isError } = useQuery({
    queryKey: ["orders", id],
    queryFn: () => ordersApi.getById(id),
  });

  if (isLoading) return <LoadingSpinner text="Loading order..." />;
  if (isError || !order) {
    return (
      <EmptyState
        title="Order not found"
        description="This order does not exist or could not be loaded."
        action={
          <Link href="/">
            <Button>Go Home</Button>
          </Link>
        }
      />
    );
  }

  const currency = order.currency ?? "USD";

  return (
    <div className="mx-auto max-w-3xl px-4 py-10 sm:px-6">
      {/* Success banner */}
      <div className="mb-8 flex flex-col items-center text-center gap-3 rounded-2xl bg-emerald-50 p-8 ring-1 ring-emerald-100">
        <CheckCircle className="h-14 w-14 text-emerald-500" />
        <h1 className="text-2xl font-bold text-gray-900">Order Placed!</h1>
        <p className="text-gray-500">
          Thank you for your purchase. Your order has been received.
        </p>
      </div>

      {/* Order meta */}
      <div className="rounded-xl bg-white shadow-sm ring-1 ring-gray-100 overflow-hidden">
        <div className="border-b border-gray-100 px-5 py-4 flex items-center justify-between">
          <div>
            <p className="text-xs text-gray-400 uppercase tracking-wide">
              Order ID
            </p>
            <p className="font-mono text-sm text-gray-700 mt-0.5">
              {order.id}
            </p>
          </div>
          <OrderBadge status={order.status} />
        </div>

        <div className="px-5 py-4 border-b border-gray-100">
          <p className="text-xs text-gray-400 uppercase tracking-wide mb-0.5">
            Customer
          </p>
          <p className="text-sm text-gray-700">{order.customerId}</p>
        </div>

        {/* Items */}
        <div className="px-5 py-4">
          <p className="text-xs text-gray-400 uppercase tracking-wide mb-3">
            Items
          </p>
          <div className="space-y-3">
            {order.items.map((item, idx) => (
              <div key={idx} className="flex items-center gap-3">
                <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-indigo-50">
                  <Package className="h-4 w-4 text-indigo-400" />
                </div>
                <div className="flex flex-1 items-center justify-between min-w-0">
                  <div className="min-w-0">
                    <p className="text-sm font-medium text-gray-900 line-clamp-1">
                      {item.productName}
                    </p>
                    <p className="text-xs text-gray-400">
                      {formatPrice(item.unitPrice, item.currency)} × {item.quantity}
                    </p>
                  </div>
                  <span className="ml-4 shrink-0 text-sm font-semibold text-gray-700">
                    {formatPrice(item.totalPrice, item.currency)}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Total */}
        <div className="border-t border-gray-100 px-5 py-4 flex items-center justify-between">
          <span className="font-semibold text-gray-900">Total</span>
          <span className="text-lg font-bold text-indigo-600">
            {formatPrice(order.totalAmount, currency)}
          </span>
        </div>
      </div>

      {/* Actions */}
      <div className="mt-6 flex flex-col sm:flex-row gap-3">
        <Link href="/products" className="flex-1">
          <Button size="lg" className="w-full">
            Continue Shopping <ArrowRight className="h-4 w-4" />
          </Button>
        </Link>
        <Link href="/" className="flex-1">
          <Button size="lg" variant="outline" className="w-full">
            Go Home
          </Button>
        </Link>
      </div>
    </div>
  );
}
