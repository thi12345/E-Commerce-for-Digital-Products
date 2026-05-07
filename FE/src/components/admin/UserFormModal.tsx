"use client";

import { useEffect, useState } from "react";
import { X } from "lucide-react";
import { Button } from "@/components/ui/Button";
import type { User, CreateUserPayload, UserRole } from "@/lib/types";

interface UserFormModalProps {
  open: boolean;
  user?: User | null;
  onClose: () => void;
  onSubmit: (payload: CreateUserPayload) => void;
  loading?: boolean;
  error?: string | null;
}

const empty: CreateUserPayload = { name: "", email: "", role: "Customer" };

export function UserFormModal({
  open,
  user,
  onClose,
  onSubmit,
  loading,
  error,
}: UserFormModalProps) {
  const [form, setForm] = useState<CreateUserPayload>(empty);

  useEffect(() => {
    if (user) {
      setForm({ name: user.name, email: user.email, role: user.role });
    } else {
      setForm(empty);
    }
  }, [user, open]);

  if (!open) return null;

  const set = (k: keyof CreateUserPayload, v: string) =>
    setForm((f) => ({ ...f, [k]: v }));

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4">
      <div className="w-full max-w-md rounded-2xl bg-white shadow-xl">
        <div className="flex items-center justify-between border-b border-gray-100 px-6 py-4">
          <h2 className="text-lg font-semibold text-gray-900">
            {user ? "Edit User" : "New User"}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X className="h-5 w-5" />
          </button>
        </div>

        <form
          className="p-6 space-y-4"
          onSubmit={(e) => { e.preventDefault(); onSubmit(form); }}
        >
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Full Name *</label>
            <input
              required
              value={form.name}
              onChange={(e) => set("name", e.target.value)}
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email *</label>
            <input
              required
              type="email"
              value={form.email}
              onChange={(e) => set("email", e.target.value)}
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Role *</label>
            <select
              value={form.role}
              onChange={(e) => set("role", e.target.value as UserRole)}
              className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-indigo-400 focus:outline-none focus:ring-2 focus:ring-indigo-200"
            >
              <option value="Customer">Customer</option>
              <option value="Admin">Admin</option>
            </select>
          </div>

          {error && (
            <p className="text-sm text-red-600 bg-red-50 rounded-lg px-3 py-2">{error}</p>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="ghost" onClick={onClose} disabled={loading}>
              Cancel
            </Button>
            <Button type="submit" loading={loading}>
              {user ? "Save Changes" : "Create User"}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
