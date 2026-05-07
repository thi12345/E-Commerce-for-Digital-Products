import type { ReactNode } from "react";
import Link from "next/link";
import { LayoutDashboard, Package, Users, Zap, ArrowLeft } from "lucide-react";

const navItems = [
  { href: "/admin", label: "Dashboard", icon: LayoutDashboard, exact: true },
  { href: "/admin/products", label: "Products", icon: Package },
  { href: "/admin/users", label: "Users", icon: Users },
];

export default function AdminLayout({ children }: { children: ReactNode }) {
  return (
    <div className="flex min-h-screen bg-gray-50">
      {/* Sidebar */}
      <aside className="hidden w-56 shrink-0 flex-col bg-gray-900 md:flex">
        {/* Logo */}
        <div className="flex h-16 items-center gap-2 px-5 border-b border-gray-700">
          <Zap className="h-5 w-5 text-indigo-400" />
          <span className="font-bold text-white text-sm">
            Digital<span className="text-indigo-400">Shop</span>{" "}
            <span className="text-gray-400 font-normal">Admin</span>
          </span>
        </div>

        {/* Nav */}
        <nav className="flex-1 px-3 py-4 space-y-1">
          {navItems.map(({ href, label, icon: Icon }) => (
            <Link
              key={href}
              href={href}
              className="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium text-gray-300 hover:bg-gray-800 hover:text-white transition-colors"
            >
              <Icon className="h-4 w-4" />
              {label}
            </Link>
          ))}
        </nav>

        {/* Back to shop */}
        <div className="border-t border-gray-700 p-3">
          <Link
            href="/"
            className="flex items-center gap-2 rounded-lg px-3 py-2 text-sm text-gray-400 hover:text-white transition-colors"
          >
            <ArrowLeft className="h-4 w-4" />
            Back to Shop
          </Link>
        </div>
      </aside>

      {/* Main content */}
      <div className="flex flex-1 flex-col min-w-0">
        {/* Top bar (mobile) */}
        <div className="flex h-14 items-center justify-between border-b border-gray-200 bg-white px-4 md:hidden">
          <Link href="/admin" className="flex items-center gap-2">
            <Zap className="h-5 w-5 text-indigo-500" />
            <span className="font-bold text-gray-900 text-sm">Admin Panel</span>
          </Link>
          <div className="flex gap-1">
            {navItems.map(({ href, label, icon: Icon }) => (
              <Link
                key={href}
                href={href}
                className="flex items-center gap-1 rounded-lg px-2 py-1 text-xs font-medium text-gray-600 hover:bg-gray-100"
              >
                <Icon className="h-3.5 w-3.5" />
                {label}
              </Link>
            ))}
          </div>
        </div>

        <main className="flex-1 p-6">{children}</main>
      </div>
    </div>
  );
}
