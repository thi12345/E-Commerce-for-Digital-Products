"use client";

import { useEffect, useState } from "react";
import { X } from "lucide-react";
import { Button } from "@/components/ui/Button";
import type { Product, CreateProductPayload } from "@/lib/types";

interface ProductFormModalProps {
  open: boolean;
  product?: Product | null;
  onClose: () => void;
  onSubmit: (payload: CreateProductPayload) => void;
  loading?: boolean;
  error?: string | null;
}

const empty: CreateProductPayload = {
  name: "",
  description: "",
  price: 0,
  currency: "USD",
  downloadUrl: "",
};

export function ProductFormModal({
  open,
  product,
  onClose,
  onSubmit,
  loading,
  error,
}: ProductFormModalProps) {
  const [form, setForm] = useState<CreateProductPayload>(empty);

  useEffect(() => {
    if (product) {
      setForm({
        name: product.name,
        description: product.description,
        price: product.price,
        currency: product.currency,
        downloadUrl: product.downloadUrl,
      });
    } else {
      setForm(empty);
    }
  }, [product, open]);

  if (!open) return null;

  const set = (k: keyof CreateProductPayload, v: string | number) =>
    setForm((f) => ({ ...f, [k]: v }));

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4">
      <div className="w-full max-w-lg rounded-2xl bg-white shadow-xl">
        {/* Header */}
        <div className="flex items-center justify-between border-b border-gray-100 px-6 py-4">
          <h2 className="text-lg font-semibold text-gray-900">
            {product ? "Edit Product" : "New Product"}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* Form */}
        <form
          className="p-6 space-y-4"
          onSubmit={(e) => { e.preventDefault(); onSubmit(form); }}
        >
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Name *</label>
            <input
              required
              value={form.name}
              onChange={(e) => set("name", e.target.value)}
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
            <textarea
              rows={3}
              value={form.description}
              onChange={(e) => set("description", e.target.value)}
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200 resize-none"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Price *</label>
              <input
                required
                type="number"
                min={0}
                step="0.01"
                value={form.price}
                onChange={(e) => set("price", parseFloat(e.target.value) || 0)}
                className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Currency *</label>
              <select
                value={form.currency}
                onChange={(e) => set("currency", e.target.value)}
                className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
              >
                <option>USD</option>
                <option>EUR</option>
                <option>VND</option>
              </select>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Download URL *</label>
            <input
              required
              type="url"
              value={form.downloadUrl}
              onChange={(e) => set("downloadUrl", e.target.value)}
              placeholder="https://example.com/file.zip"
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
            />
          </div>

          {error && (
            <p className="text-sm text-red-600 bg-red-50 rounded-lg px-3 py-2">{error}</p>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="ghost" onClick={onClose} disabled={loading}>
              Cancel
            </Button>
            <Button type="submit" loading={loading}>
              {product ? "Save Changes" : "Create Product"}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
