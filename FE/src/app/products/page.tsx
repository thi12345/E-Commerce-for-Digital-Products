"use client";

import { useEffect, useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  Search,
  ArrowUpDown,
  ChevronLeft,
  ChevronRight,
  SlidersHorizontal,
  Star,
} from "lucide-react";
import { categoriesApi, productsApi } from "@/lib/api";
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

function ratingRangeLabel(rating: number) {
  return `${rating}.0 - 5.0`;
}

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
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [sort, setSort] = useState<ProductSortBy>("Default");
  const [page, setPage] = useState(1);
  const [selectedCategories, setSelectedCategories] = useState<string[]>([]);
  const [minDiscount, setMinDiscount] = useState(0);
  const [minRating, setMinRating] = useState(0);

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      setDebouncedSearch(search);
    }, 400);

    return () => window.clearTimeout(timeoutId);
  }, [search]);

  const productQueryParams = useMemo(
    () => ({
      name: debouncedSearch.trim() || undefined,
      minDiscountPercentage: minDiscount > 0 ? minDiscount : undefined,
      minRating: minRating > 0 ? minRating : undefined,
      sortBy: sort,
      pageSize: 100,
    }),
    [debouncedSearch, minDiscount, minRating, sort]
  );

  const { data: products, isLoading, isError } = useQuery({
    queryKey: ["products", productQueryParams, selectedCategories],
    queryFn: async () => {
      if (selectedCategories.length === 0) {
        return productsApi.getFiltered(productQueryParams);
      }

      const groups = await Promise.all(
        selectedCategories.map((categoryId) =>
          productsApi.getFiltered({ ...productQueryParams, categoryId })
        )
      );

      return Array.from(
        new Map(groups.flat().map((product) => [product.id, product])).values()
      );
    },
  });

  const { data: categories } = useQuery({
    queryKey: ["categories"],
    queryFn: categoriesApi.getAll,
  });

  const filtered = useMemo(() => {
    const next = [...(products ?? [])];

    if (sort === "PriceAsc") {
      next.sort((a, b) => effectivePrice(a) - effectivePrice(b));
    }

    if (sort === "PriceDesc") {
      next.sort((a, b) => effectivePrice(b) - effectivePrice(a));
    }

    return next;
  }, [products, sort]);

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

  const toggleCategory = (categoryId: string) => {
    setSelectedCategories((current) =>
      current.includes(categoryId)
        ? current.filter((id) => id !== categoryId)
        : [...current, categoryId]
    );
    setPage(1);
  };

  const resetFilters = () => {
    setSelectedCategories([]);
    setMinDiscount(0);
    setMinRating(0);
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

      <div className="grid gap-6 lg:grid-cols-[260px_1fr]">
        <aside className="h-fit rounded-xl bg-white p-4 shadow-sm ring-1 ring-gray-100 lg:sticky lg:top-20">
          <div className="mb-4 flex items-center justify-between gap-3">
            <div className="flex items-center gap-2">
              <SlidersHorizontal className="h-4 w-4 text-gray-400" />
              <h2 className="font-semibold text-gray-900">Filters</h2>
            </div>
            <button
              onClick={resetFilters}
              className="text-xs font-medium text-indigo-600 hover:text-indigo-700"
            >
              Reset
            </button>
          </div>

          <div className="space-y-6">
            <section>
              <p className="mb-3 text-xs font-semibold uppercase tracking-wide text-gray-400">
                Category
              </p>
              <div className="space-y-2">
                {(categories ?? []).map((category) => (
                  <label
                    key={category.id}
                    className="flex cursor-pointer items-center gap-2 text-sm text-gray-600"
                  >
                    <input
                      type="checkbox"
                      checked={selectedCategories.includes(category.id)}
                      onChange={() => toggleCategory(category.id)}
                      className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                    />
                    <span className="line-clamp-1">{category.name}</span>
                  </label>
                ))}
                {categories?.length === 0 && (
                  <p className="text-sm text-gray-400">No categories</p>
                )}
              </div>
            </section>

            <section>
              <div className="mb-3 flex items-center justify-between gap-3">
                <p className="text-xs font-semibold uppercase tracking-wide text-gray-400">
                  Discount
                </p>
                <span className="text-xs font-semibold text-indigo-600">
                  {minDiscount}%+
                </span>
              </div>
              <input
                type="range"
                min={0}
                max={100}
                step={5}
                value={minDiscount}
                onChange={(e) => {
                  setMinDiscount(Number(e.target.value));
                  setPage(1);
                }}
                className="w-full accent-indigo-600"
              />
              <div className="mt-1 flex justify-between text-xs text-gray-400">
                <span>0%</span>
                <span>100%</span>
              </div>
            </section>

            <section>
              <div className="mb-3 flex items-center justify-between gap-3">
                <p className="text-xs font-semibold uppercase tracking-wide text-gray-400">
                  Review
                </p>
                {minRating > 0 && (
                  <span className="text-xs font-semibold text-amber-600">
                    {ratingRangeLabel(minRating)}
                  </span>
                )}
              </div>
              <div className="grid grid-cols-4 gap-2">
                {[1, 2, 3, 4].map((rating) => (
                  <button
                    key={rating}
                    type="button"
                    onClick={() => {
                      setMinRating(minRating === rating ? 0 : rating);
                      setPage(1);
                    }}
                    className={`flex h-9 items-center justify-center gap-0.5 rounded-lg text-xs font-semibold ring-1 transition-colors ${
                      minRating === rating
                        ? "bg-amber-50 text-amber-600 ring-amber-200"
                        : "bg-white text-gray-500 ring-gray-200 hover:bg-gray-50"
                    }`}
                    title={`${ratingRangeLabel(rating)} stars`}
                  >
                    {rating}
                    <Star className="h-3 w-3 fill-current" />
                  </button>
                ))}
              </div>
            </section>
          </div>
        </aside>

        <section>
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
        </section>
      </div>
    </div>
  );
}
