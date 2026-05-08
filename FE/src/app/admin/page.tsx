"use client";

import { useQuery } from "@tanstack/react-query";
import { Package, Users, ShoppingBag, TrendingUp } from "lucide-react";
import Link from "next/link";
import { productsApi, usersApi } from "@/lib/api";

export default function AdminDashboard() {
  const { data: products } = useQuery({ queryKey: ["products", "admin"], queryFn: productsApi.getAll });
  const { data: users } = useQuery({ queryKey: ["users"], queryFn: usersApi.getAll });

  const activeProducts = products?.filter((p) => p.status === "Active").length ?? 0;
  const draftProducts = products?.filter((p) => p.status === "Draft").length ?? 0;
  const adminUsers = users?.filter((u) => u.role === "Admin").length ?? 0;

  const stats = [
    {
      label: "Total Products",
      value: products?.length ?? "—",
      sub: `${activeProducts} active · ${draftProducts} draft`,
      icon: Package,
      color: "bg-indigo-50 text-indigo-600",
      href: "/admin/products",
    },
    {
      label: "Total Users",
      value: users?.length ?? "—",
      sub: `${adminUsers} admin · ${(users?.length ?? 0) - adminUsers} customers`,
      icon: Users,
      color: "bg-emerald-50 text-emerald-600",
      href: "/admin/users",
    },
    {
      label: "Active Listings",
      value: activeProducts,
      sub: "Products visible to customers",
      icon: TrendingUp,
      color: "bg-purple-50 text-purple-600",
      href: "/admin/products",
    },
    {
      label: "Draft Products",
      value: draftProducts,
      sub: "Waiting for activation",
      icon: ShoppingBag,
      color: "bg-amber-50 text-amber-600",
      href: "/admin/products",
    },
  ];

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-sm text-gray-500 mt-1">Overview of your digital store</p>
      </div>

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map((s) => (
          <Link
            key={s.label}
            href={s.href}
            className="group rounded-xl bg-white p-5 shadow-sm ring-1 ring-gray-100 hover:shadow-md transition-shadow"
          >
            <div className="flex items-start justify-between">
              <div>
                <p className="text-sm text-gray-500">{s.label}</p>
                <p className="mt-1 text-3xl font-bold text-gray-900">{s.value}</p>
                <p className="mt-1 text-xs text-gray-400">{s.sub}</p>
              </div>
              <div className={`rounded-xl p-2.5 ${s.color}`}>
                <s.icon className="h-5 w-5" />
              </div>
            </div>
          </Link>
        ))}
      </div>

      {/* Quick links */}
      <div className="mt-10 grid grid-cols-1 gap-5 sm:grid-cols-2">
        <div className="rounded-xl bg-white p-6 shadow-sm ring-1 ring-gray-100">
          <div className="flex items-center justify-between mb-4">
            <h2 className="font-semibold text-gray-900">Quick Actions</h2>
          </div>
          <div className="space-y-2">
            <Link href="/admin/products" className="flex items-center gap-3 rounded-lg p-3 hover:bg-gray-50 transition-colors">
              <Package className="h-5 w-5 text-indigo-500" />
              <div>
                <p className="text-sm font-medium text-gray-800">Manage Products</p>
                <p className="text-xs text-gray-400">Create, edit, activate products</p>
              </div>
            </Link>
            <Link href="/admin/users" className="flex items-center gap-3 rounded-lg p-3 hover:bg-gray-50 transition-colors">
              <Users className="h-5 w-5 text-emerald-500" />
              <div>
                <p className="text-sm font-medium text-gray-800">Manage Users</p>
                <p className="text-xs text-gray-400">Create, edit, delete users</p>
              </div>
            </Link>
          </div>
        </div>

        <div className="rounded-xl bg-white p-6 shadow-sm ring-1 ring-gray-100">
          <h2 className="font-semibold text-gray-900 mb-4">Recent Products</h2>
          {products?.slice(0, 4).map((p) => (
            <div key={p.id} className="flex items-center justify-between py-2 border-b border-gray-50 last:border-0">
              <span className="text-sm text-gray-700 truncate flex-1 mr-2">{p.name}</span>
              <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${
                p.status === "Active" ? "bg-emerald-100 text-emerald-700"
                : p.status === "Draft" ? "bg-amber-100 text-amber-700"
                : "bg-gray-100 text-gray-500"
              }`}>{p.status}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
