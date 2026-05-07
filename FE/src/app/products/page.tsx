"use client";

import { useState, useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { Search, SlidersHorizontal } from "lucide-react";
import { productsApi } from "@/lib/api";
import { ProductCard } from "@/components/products/ProductCard";
import { PageSkeleton } from "@/components/ui/LoadingSpinner";
import { EmptyState } from "@/components/ui/EmptyState";
import type { ProductStatus } from "@/lib/types";

const STATUS_FILTERS: { label: string; value: ProductStatus | "All" }[] = [
  { label: "All", value: "All" },
  { label: "Active", value: "Active" },
  { label: "Draft", value: "Draft" },
  { label: "Inactive", value: "Inactive" },
];

export default function ProductsPage() {
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState<ProductStatus | "All">("All");

  const { data: products, isLoading, isError } = useQuery({
    queryKey: ["products"],
    queryFn: productsApi.getAll,
  });

  const filtered = useMemo(() => {
    if (!products) return [];
    return products.filter((p) => {
      const matchesSearch =
        p.name.toLowerCase().includes(search.toLowerCase()) ||
        p.description.toLowerCase().includes(search.toLowerCase());
      const matchesStatus = status === "All" || p.status === status;
      return matchesSearch && matchesStatus;
    });
  }, [products, search, status]);

  return (
    <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      {/* Page header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">All Products</h1>
        <p className="mt-1 text-gray-500">
          {products?.length ?? 0} products available
        </p>
      </div>

      {/* Filters */}
      <div className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        {/* Search */}
        <div className="relative w-full sm:max-w-xs">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            placeholder="Search products..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full rounded-lg border border-gray-200 bg-white py-2 pl-9 pr-4 text-sm placeholder:text-gray-400 focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
          />
        </div>

        {/* Status filter */}
        <div className="flex items-center gap-2">
          <SlidersHorizontal className="h-4 w-4 text-gray-400" />
          <div className="flex rounded-lg overflow-hidden ring-1 ring-gray-200 bg-white">
            {STATUS_FILTERS.map((f) => (
              <button
                key={f.value}
                onClick={() => setStatus(f.value)}
                className={`px-3 py-1.5 text-sm font-medium transition-colors ${
                  status === f.value
                    ? "bg-indigo-600 text-white"
                    : "text-gray-600 hover:bg-gray-50"
                }`}
              >
                {f.label}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Results */}
      {isLoading ? (
        <PageSkeleton />
      ) : isError ? (
        <EmptyState
          title="Failed to load products"
          description="Could not connect to the server. Make sure the backend is running."
        />
      ) : filtered.length === 0 ? (
        <EmptyState
          title="No products found"
          description={search ? `No results for "${search}"` : "No products in this category yet."}
        />
      ) : (
        <>
          <p className="mb-4 text-sm text-gray-500">{filtered.length} result(s)</p>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {filtered.map((p) => (
              <ProductCard key={p.id} product={p} />
            ))}
          </div>
        </>
      )}
    </div>
  );
}
