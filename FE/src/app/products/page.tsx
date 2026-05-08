"use client";

import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  Search,
  ArrowUpDown,
  ChevronLeft,
  ChevronRight,
} from "lucide-react";
import { productsApi } from "@/lib/api";
import { ProductCard } from "@/components/products/ProductCard";
import { PageSkeleton } from "@/components/ui/LoadingSpinner";
import { EmptyState } from "@/components/ui/EmptyState";
import type { ProductSortBy } from "@/lib/types";
import { effectivePrice } from "@/lib/utils";

const PAGE_SIZE = 30;

const SORT_OPTIONS: { label: string; value: ProductSortBy }[] = [
  { label: "Default", value: "Default" },
  { label: "Price: Low to High", value: "PriceAsc" },
  { label: "Price: High to Low", value: "PriceDesc" },
];

function Pagination({
  current,
  total,
  onChange,
}: {
  current: number;
  total: number;
  onChange: (p: number) => void;
}) {
  if (total <= 1) return null;

  return (
    <div className="mt-8 flex items-center justify-center gap-2">
      <button
        onClick={() => onChange(current - 1)}
        disabled={current === 1}
        className="flex h-8 w-8 items-center justify-center rounded-lg border border-gray-200 text-gray-500 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-40"
      >
        <ChevronLeft className="h-4 w-4" />
      </button>
      <span className="text-sm text-gray-500">
        Page {current} of {total}
      </span>
      <button
        onClick={() => onChange(current + 1)}
        disabled={current === total}
        className="flex h-8 w-8 items-center justify-center rounded-lg border border-gray-200 text-gray-500 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-40"
      >
        <ChevronRight className="h-4 w-4" />
      </button>
    </div>
  );
}

export default function ProductsPage() {
  const [search, setSearch] = useState("");
  const [sort, setSort] = useState<ProductSortBy>("Default");
  const [page, setPage] = useState(1);

  const { data: products, isLoading, isError } = useQuery({
    queryKey: ["products"],
    queryFn: productsApi.getAll,
  });

  const filtered = useMemo(() => {
    const query = search.trim().toLowerCase();
    const next = (products ?? []).filter((product) => {
      if (!query) return true;
      return (
        product.name.toLowerCase().includes(query) ||
        product.description.toLowerCase().includes(query) ||
        (product.aboutProduct?.toLowerCase().includes(query) ?? false)
      );
    });

    if (sort === "PriceAsc") {
      next.sort((a, b) => effectivePrice(a) - effectivePrice(b));
    }

    if (sort === "PriceDesc") {
      next.sort((a, b) => effectivePrice(b) - effectivePrice(a));
    }

    return next;
  }, [products, search, sort]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
  const currentPage = Math.min(page, totalPages);
  const visibleProducts = filtered.slice(
    (currentPage - 1) * PAGE_SIZE,
    currentPage * PAGE_SIZE
  );

  const handleSearch = (value: string) => {
    setSearch(value);
    setPage(1);
  };

  const handleSort = (value: ProductSortBy) => {
    setSort(value);
    setPage(1);
  };

  return (
    <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">All Products</h1>
        <p className="mt-0.5 text-sm text-gray-500">
          {products ? `${filtered.length} products available` : "Loading..."}
        </p>
      </div>

      <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center">
        <div className="relative flex-1 sm:max-w-sm">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            placeholder="Search products..."
            value={search}
            onChange={(e) => handleSearch(e.target.value)}
            className="w-full rounded-lg border border-gray-200 bg-white py-2 pl-9 pr-4 text-sm placeholder:text-gray-400 focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
          />
        </div>

        <div className="flex items-center gap-2 sm:ml-auto">
          <ArrowUpDown className="h-4 w-4 shrink-0 text-gray-400" />
          <select
            value={sort}
            onChange={(e) => handleSort(e.target.value as ProductSortBy)}
            className="rounded-lg border border-gray-200 bg-white py-2 pl-3 pr-8 text-sm text-gray-700 focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
          >
            {SORT_OPTIONS.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </div>
      </div>

      {isLoading ? (
        <PageSkeleton />
      ) : isError ? (
        <EmptyState title="Failed to load products" description="Could not connect to the server." />
      ) : visibleProducts.length === 0 ? (
        <EmptyState
          title="No products found"
          description={search ? `No results for "${search}"` : "No products are available yet."}
        />
      ) : (
        <>
          <p className="mb-4 text-sm text-gray-500">
            Showing {(currentPage - 1) * PAGE_SIZE + 1}-
            {Math.min(currentPage * PAGE_SIZE, filtered.length)} of {filtered.length} products
          </p>
          <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 xl:grid-cols-3">
            {visibleProducts.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
          <Pagination current={currentPage} total={totalPages} onChange={setPage} />
        </>
      )}
    </div>
  );
}
