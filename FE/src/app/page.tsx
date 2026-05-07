"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { ArrowRight, Zap, Shield, Download } from "lucide-react";
import { productsApi } from "@/lib/api";
import { ProductCard } from "@/components/products/ProductCard";
import { Button } from "@/components/ui/Button";
import { PageSkeleton } from "@/components/ui/LoadingSpinner";

const features = [
  {
    icon: <Zap className="h-6 w-6 text-indigo-500" />,
    title: "Instant Delivery",
    desc: "Download your product immediately after purchase.",
  },
  {
    icon: <Shield className="h-6 w-6 text-emerald-500" />,
    title: "Secure Checkout",
    desc: "Your payment and personal data are always protected.",
  },
  {
    icon: <Download className="h-6 w-6 text-purple-500" />,
    title: "High Quality",
    desc: "Curated digital products reviewed for quality.",
  },
];

export default function HomePage() {
  const { data: products, isLoading } = useQuery({
    queryKey: ["products"],
    queryFn: productsApi.getAll,
  });

  const featured = products?.filter((p) => p.status === "Active").slice(0, 6);

  return (
    <div>
      {/* Hero */}
      <section className="bg-gradient-to-br from-gray-900 via-indigo-950 to-gray-900 py-20 px-4">
        <div className="mx-auto max-w-3xl text-center">
          <span className="inline-block mb-4 rounded-full bg-indigo-500/20 px-4 py-1 text-sm font-medium text-indigo-300 ring-1 ring-indigo-500/30">
            Digital Products Marketplace
          </span>
          <h1 className="text-4xl font-extrabold tracking-tight text-white sm:text-5xl md:text-6xl">
            Download. Create.{" "}
            <span className="text-indigo-400">Elevate.</span>
          </h1>
          <p className="mt-6 text-lg text-gray-400 max-w-xl mx-auto">
            Discover premium digital products — themes, tools, templates, and
            more. Instant download after purchase.
          </p>
          <div className="mt-8 flex flex-col sm:flex-row gap-4 justify-center">
            <Link href="/products">
              <Button size="lg">
                Browse Products <ArrowRight className="h-4 w-4" />
              </Button>
            </Link>
            <Link href="/cart">
              <Button size="lg" variant="outline" className="text-white border-white/30 hover:bg-white/10">
                View Cart
              </Button>
            </Link>
          </div>
        </div>
      </section>

      {/* Features */}
      <section className="bg-white py-12 px-4">
        <div className="mx-auto max-w-7xl">
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-3">
            {features.map((f) => (
              <div
                key={f.title}
                className="flex items-start gap-4 rounded-xl p-5 ring-1 ring-gray-100"
              >
                <div className="shrink-0 rounded-lg bg-gray-50 p-2">{f.icon}</div>
                <div>
                  <h3 className="font-semibold text-gray-900">{f.title}</h3>
                  <p className="text-sm text-gray-500">{f.desc}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Products */}
      <section className="py-14 px-4">
        <div className="mx-auto max-w-7xl">
          <div className="flex items-center justify-between mb-8">
            <div>
              <h2 className="text-2xl font-bold text-gray-900">
                Featured Products
              </h2>
              <p className="text-sm text-gray-500 mt-1">
                Handpicked digital products ready to download.
              </p>
            </div>
            <Link href="/products" className="text-sm text-indigo-600 font-medium hover:underline flex items-center gap-1">
              View all <ArrowRight className="h-4 w-4" />
            </Link>
          </div>

          {isLoading ? (
            <PageSkeleton />
          ) : featured && featured.length > 0 ? (
            <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {featured.map((p) => (
                <ProductCard key={p.id} product={p} />
              ))}
            </div>
          ) : (
            <div className="py-12 text-center text-gray-400">
              No active products available yet. Check back soon!
            </div>
          )}
        </div>
      </section>
    </div>
  );
}
