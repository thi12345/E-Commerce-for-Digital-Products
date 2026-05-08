import { Zap } from "lucide-react";
import Link from "next/link";

export function Footer() {
  return (
    <footer className="mt-auto border-t border-gray-200 bg-white">
      <div className="mx-auto w-full px-4 py-8 sm:px-6 lg:px-8">
        <div className="flex flex-col items-center justify-center gap-4 sm:flex-row">
          <Link href="/" className="flex items-center gap-2 text-gray-700">
            <Zap className="h-5 w-5 text-indigo-500" />
            <span className="font-bold">
              Digital<span className="text-indigo-500">Shop</span>
            </span>
          </Link>
          <p className="text-sm text-gray-400">
            &copy; {new Date().getFullYear()} DigitalShop. All rights reserved.
          </p>
        </div>
      </div>
    </footer>
  );
}
