"use client";

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Plus, Pencil, Trash2, Zap, ZapOff, Search } from "lucide-react";
import { productsApi } from "@/lib/api";
import { effectivePrice, formatPrice, hasDiscount, originalPrice, productCurrency } from "@/lib/utils";
import { ProductBadge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";
import { ProductFormModal } from "@/components/admin/ProductFormModal";
import { ConfirmDialog } from "@/components/admin/ConfirmDialog";
import type { Product, CreateProductPayload } from "@/lib/types";

export default function AdminProductsPage() {
  const qc = useQueryClient();
  const { data: products, isLoading } = useQuery({
    queryKey: ["products", "admin-list"],
    queryFn: productsApi.getAll,
  });

  const [search, setSearch] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Product | null>(null);
  const [deleting, setDeleting] = useState<Product | null>(null);
  const [formError, setFormError] = useState<string | null>(null);

  const invalidate = () => qc.invalidateQueries({ queryKey: ["products"] });


  const createMutation = useMutation({
    mutationFn: productsApi.create,
    onSuccess: () => { invalidate(); setModalOpen(false); setFormError(null); },
    onError: (e: Error) => setFormError(e.message),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: CreateProductPayload }) =>
      productsApi.update(id, payload),
    onSuccess: () => { invalidate(); setModalOpen(false); setEditing(null); setFormError(null); },
    onError: (e: Error) => setFormError(e.message),
  });

  const activateMutation = useMutation({
    mutationFn: productsApi.activate,
    onSuccess: invalidate,
  });

  const deactivateMutation = useMutation({
    mutationFn: productsApi.deactivate,
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: productsApi.delete,
    onSuccess: () => { invalidate(); setDeleting(null); },
  });

  const filtered = (products ?? []).filter(
    (p) =>
      p.name.toLowerCase().includes(search.toLowerCase()) ||
      (p.description?.toLowerCase().includes(search.toLowerCase()) ?? false) ||
      (p.aboutProduct?.toLowerCase().includes(search.toLowerCase()) ?? false) ||
      p.status.toLowerCase().includes(search.toLowerCase())
  );

  const openCreate = () => { setEditing(null); setFormError(null); setModalOpen(true); };
  const openEdit = (p: Product) => { setEditing(p); setFormError(null); setModalOpen(true); };

  const handleSubmit = (payload: CreateProductPayload) => {
    if (editing) {
      updateMutation.mutate({ id: editing.id, payload });
    } else {
      createMutation.mutate(payload);
    }
  };

  return (
    <div>
      <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Products</h1>
          <p className="text-sm text-gray-500 mt-0.5">{products?.length ?? 0} total</p>
        </div>
        <Button onClick={openCreate}>
          <Plus className="h-4 w-4" /> New Product
        </Button>
      </div>

      {/* Search */}
      <div className="relative mb-5 max-w-xs">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
        <input
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search products…"
          className="w-full rounded-lg border border-gray-200 bg-white py-2 pl-9 pr-4 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
        />
      </div>

      {isLoading ? (
        <LoadingSpinner />
      ) : (
        <div className="rounded-xl bg-white shadow-sm ring-1 ring-gray-100 overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-gray-100 bg-gray-50 text-left text-xs font-semibold text-gray-500 uppercase tracking-wide">
                <th className="px-5 py-3">Name</th>
                <th className="px-5 py-3 hidden sm:table-cell">Price</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3 hidden lg:table-cell">Created</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {filtered.map((p) => (
                <tr key={p.id} className="hover:bg-gray-50/50 transition-colors">
                  <td className="px-5 py-3">
                    <p className="font-medium text-gray-900 line-clamp-1">{p.name}</p>
                    <p className="text-xs text-gray-400 line-clamp-1 mt-0.5">
                      {p.aboutProduct ?? p.description}
                    </p>
                  </td>
                  <td className="px-5 py-3 hidden sm:table-cell">
                    <span className="font-medium text-indigo-600">
                      {formatPrice(effectivePrice(p), productCurrency(p))}
                    </span>
                    {hasDiscount(p) && (
                      <span className="ml-1.5 text-xs text-gray-400 line-through">
                        {formatPrice(originalPrice(p), productCurrency(p))}
                      </span>
                    )}
                  </td>
                  <td className="px-5 py-3">
                    <ProductBadge status={p.status} />
                  </td>
                  <td className="px-5 py-3 hidden lg:table-cell text-gray-400 text-xs">
                    {new Date(p.createdAt).toLocaleDateString()}
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex items-center justify-end gap-1">
                      {p.status !== "Active" ? (
                        <button
                          title="Activate"
                          onClick={() => activateMutation.mutate(p.id)}
                          className="p-1.5 rounded-lg text-gray-400 hover:text-emerald-600 hover:bg-emerald-50 transition-colors"
                        >
                          <Zap className="h-4 w-4" />
                        </button>
                      ) : (
                        <button
                          title="Deactivate"
                          onClick={() => deactivateMutation.mutate(p.id)}
                          className="p-1.5 rounded-lg text-gray-400 hover:text-amber-600 hover:bg-amber-50 transition-colors"
                        >
                          <ZapOff className="h-4 w-4" />
                        </button>
                      )}
                      <button
                        title="Edit"
                        onClick={() => openEdit(p)}
                        className="p-1.5 rounded-lg text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 transition-colors"
                      >
                        <Pencil className="h-4 w-4" />
                      </button>
                      <button
                        title="Delete"
                        onClick={() => setDeleting(p)}
                        className="p-1.5 rounded-lg text-gray-400 hover:text-red-600 hover:bg-red-50 transition-colors"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
              {filtered.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-5 py-12 text-center text-gray-400">
                    No products found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      <ProductFormModal
        open={modalOpen}
        product={editing}
        onClose={() => { setModalOpen(false); setEditing(null); }}
        onSubmit={handleSubmit}
        loading={createMutation.isPending || updateMutation.isPending}
        error={formError}
      />

      <ConfirmDialog
        open={!!deleting}
        title="Delete Product"
        description={`Are you sure you want to delete "${deleting?.name}"? This action cannot be undone.`}
        onConfirm={() => deleting && deleteMutation.mutate(deleting.id)}
        onCancel={() => setDeleting(null)}
        loading={deleteMutation.isPending}
      />
    </div>
  );
}
