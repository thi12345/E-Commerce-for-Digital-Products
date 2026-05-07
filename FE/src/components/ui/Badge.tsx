import type { ProductStatus, OrderStatus } from "@/lib/types";
import { statusColors, orderStatusColors } from "@/lib/utils";

export function ProductBadge({ status }: { status: ProductStatus }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${statusColors[status]}`}
    >
      {status}
    </span>
  );
}

export function OrderBadge({ status }: { status: OrderStatus }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${orderStatusColors[status]}`}
    >
      {status}
    </span>
  );
}
