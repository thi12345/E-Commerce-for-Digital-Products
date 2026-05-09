"use client";

import { useEffect, useState } from "react";
import { X } from "lucide-react";
import { Button } from "@/components/ui/Button";
import type { Product, CreateProductPayload } from "@/lib/types";
import { selectedVariant } from "@/lib/utils";

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
  actualPrice: 0,
  discountedPrice: 0,
  discountPercentage: 0,
  aboutProduct: "",
  imgLink: "",
  productLink: "",
  currency: "USD",
  downloadUrl: "",
  stock: 0,
  variantName: "Default",
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
      const variant = selectedVariant(product);
      setForm({
        name: product.name,
        description: product.description ?? product.aboutProduct ?? "",
        price: variant?.discountedPrice ?? product.price ?? product.actualPrice ?? 0,
        actualPrice: variant?.actualPrice ?? product.actualPrice ?? product.price ?? 0,
        discountedPrice: variant?.discountedPrice ?? product.discountedPrice ?? product.price ?? 0,
        discountPercentage: variant?.discountPercentage ?? product.discountPercentage ?? 0,
        aboutProduct: product.aboutProduct ?? product.description ?? "",
        imgLink: product.imgLink ?? "",
        productLink: variant?.productLink ?? product.productLink ?? "",
        currency: variant?.currency ?? product.currency ?? "USD",
        downloadUrl: variant?.downloadUrl ?? product.downloadUrl ?? "",
        stock: variant?.stock ?? 0,
        variantName: variant?.name ?? "Default",
      });
    } else {
      setForm(empty);
    }
  }, [product, open]);

  if (!open) return null;

  const setNum = (k: keyof CreateProductPayload, v: string) =>
    setForm((f) => ({ ...f, [k]: parseFloat(v) || 0 }));
  const setStr = (k: keyof CreateProductPayload, v: string) =>
    setForm((f) => ({ ...f, [k]: v }));

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4 py-8 overflow-auto">
      <div className="w-full max-w-lg rounded-2xl bg-white shadow-xl my-auto">
        <div className="flex items-center justify-between border-b border-gray-100 px-6 py-4">
          <h2 className="text-lg font-semibold text-gray-900">
            {product ? "Edit Product" : "New Product"}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X className="h-5 w-5" />
          </button>
        </div>

        <form
          className="p-6 space-y-4 max-h-[75vh] overflow-y-auto"
          onSubmit={(e) => { e.preventDefault(); onSubmit(form); }}
        >
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Name *</label>
            <input required value={form.name} onChange={(e) => setStr("name", e.target.value)}
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">About Product</label>
            <textarea
              rows={3}
              value={form.aboutProduct ?? form.description}
              onChange={(e) => {
                setForm((f) => ({ ...f, aboutProduct: e.target.value, description: e.target.value }));
              }}
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200 resize-none" />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Actual Price *</label>
              <input required type="number" min={0} step="0.01" value={form.actualPrice ?? form.price}
                onChange={(e) => {
                  const value = parseFloat(e.target.value) || 0;
                  setForm((f) => ({ ...f, actualPrice: value, price: value }));
                }}
                className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Discounted Price</label>
              <input type="number" min={0} step="0.01" value={form.discountedPrice ?? 0}
                onChange={(e) => {
                  const value = parseFloat(e.target.value) || 0;
                  setForm((f) => ({ ...f, discountedPrice: value, price: value > 0 ? value : f.actualPrice ?? f.price }));
                }}
                className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Variant Name</label>
            <input value={form.variantName ?? ""} onChange={(e) => setStr("variantName", e.target.value)}
              placeholder="Default"
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Discount %</label>
              <input type="number" min={0} max={100} step="1" value={form.discountPercentage ?? 0}
                onChange={(e) => setNum("discountPercentage", e.target.value)}
                className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Currency *</label>
              <select value={form.currency} onChange={(e) => setStr("currency", e.target.value)}
                className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200">
                <option>USD</option>
                <option>EUR</option>
                <option>INR</option>
                <option>VND</option>
              </select>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Image URL</label>
            <input type="url" value={form.imgLink ?? ""} onChange={(e) => setStr("imgLink", e.target.value)}
              placeholder="https://example.com/image.jpg"
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Product Link</label>
            <input type="url" value={form.productLink ?? ""} onChange={(e) => setStr("productLink", e.target.value)}
              placeholder="https://store.com/product"
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Stock</label>
            <input type="number" min={0} step="1" value={form.stock ?? 0}
              onChange={(e) => setNum("stock", e.target.value)}
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Download URL *</label>
            <input required type="url" value={form.downloadUrl} onChange={(e) => setStr("downloadUrl", e.target.value)}
              placeholder="https://example.com/file.zip"
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200" />
          </div>

          {error && (
            <p className="text-sm text-red-600 bg-red-50 rounded-lg px-3 py-2">{error}</p>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="ghost" onClick={onClose} disabled={loading}>Cancel</Button>
            <Button type="submit" loading={loading}>
              {product ? "Save Changes" : "Create Product"}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
