"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { ShoppingCart, Zap, Menu, X, LogIn, UserPlus } from "lucide-react";
import { useState } from "react";
import { useCart } from "@/context/CartContext";

const navLinks = [
  { href: "/", label: "Home" },
  { href: "/products", label: "Products" },
  { href: "/admin", label: "Admin" },
];

export function Header() {
  const { totalItems } = useCart();
  const pathname = usePathname();
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <header className="sticky top-0 z-50 bg-gray-900 shadow-lg">
      <div className="mx-auto w-full px-4 sm:px-6 lg:px-8">
        <div className="flex h-16 items-center justify-between">
          {/* Logo */}
          <Link href="/" className="flex items-center gap-2 text-white">
            <Zap className="h-6 w-6 text-indigo-400" />
            <span className="text-lg font-bold tracking-tight">
              Digital<span className="text-indigo-400">Shop</span>
            </span>
          </Link>

          {/* Desktop nav */}
          <nav className="hidden md:flex items-center gap-6">
            {navLinks.map((l) => (
              <Link
                key={l.href}
                href={l.href}
                className={`text-sm font-medium transition-colors ${
                  pathname === l.href
                    ? "text-indigo-400"
                    : "text-gray-300 hover:text-white"
                }`}
              >
                {l.label}
              </Link>
            ))}
          </nav>

          {/* Auth + Cart + mobile toggle */}
          <div className="flex items-center gap-2">
            {/* Cart */}
            <Link
              href="/cart"
              className="relative flex items-center gap-1.5 rounded-lg bg-indigo-600 px-3 py-1.5 text-sm font-medium text-white hover:bg-indigo-700 transition-colors"
            >
              <ShoppingCart className="h-4 w-4" />
              <span className="hidden sm:inline">Cart</span>
              {totalItems > 0 && (
                <span className="absolute -right-2 -top-2 flex h-5 w-5 items-center justify-center rounded-full bg-red-500 text-xs font-bold text-white">
                  {totalItems > 9 ? "9+" : totalItems}
                </span>
              )}
            </Link>
            {/* Login */}
            <Link
              href="/login"
              className={`hidden sm:flex items-center gap-1.5 rounded-lg px-3 py-1.5 text-sm font-medium transition-colors ${
                pathname === "/login"
                  ? "text-indigo-400"
                  : "text-gray-300 hover:text-white hover:bg-gray-800"
              }`}
            >
              <LogIn className="h-4 w-4" />
              Login
            </Link>

            {/* Register */}
            <Link
              href="/register"
              className={`hidden sm:flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-sm font-medium transition-colors ${
                pathname === "/register"
                  ? "border-indigo-400 text-indigo-400"
                  : "border-gray-600 text-gray-300 hover:border-indigo-400 hover:text-indigo-400"
              }`}
            >
              <UserPlus className="h-4 w-4" />
              Register
            </Link>





            {/* Mobile menu button */}
            <button
              className="md:hidden text-gray-300 hover:text-white"
              onClick={() => setMobileOpen((v) => !v)}
            >
              {mobileOpen ? <X className="h-6 w-6" /> : <Menu className="h-6 w-6" />}
            </button>
          </div>
        </div>
      </div>

      {/* Mobile nav */}
      {mobileOpen && (
        <div className="md:hidden border-t border-gray-700 bg-gray-900 px-4 pb-4 pt-2">
          {navLinks.map((l) => (
            <Link
              key={l.href}
              href={l.href}
              onClick={() => setMobileOpen(false)}
              className={`block py-2 text-sm font-medium ${
                pathname === l.href
                  ? "text-indigo-400"
                  : "text-gray-300 hover:text-white"
              }`}
            >
              {l.label}
            </Link>
          ))}
          <div className="mt-3 flex gap-2 border-t border-gray-700 pt-3">
            <Link
              href="/login"
              onClick={() => setMobileOpen(false)}
              className="flex flex-1 items-center justify-center gap-1.5 rounded-lg px-3 py-2 text-sm font-medium text-gray-300 hover:bg-gray-800 hover:text-white transition-colors"
            >
              <LogIn className="h-4 w-4" /> Login
            </Link>
            <Link
              href="/register"
              onClick={() => setMobileOpen(false)}
              className="flex flex-1 items-center justify-center gap-1.5 rounded-lg border border-gray-600 px-3 py-2 text-sm font-medium text-gray-300 hover:border-indigo-400 hover:text-indigo-400 transition-colors"
            >
              <UserPlus className="h-4 w-4" /> Register
            </Link>
          </div>
        </div>
      )}
    </header>
  );
}
