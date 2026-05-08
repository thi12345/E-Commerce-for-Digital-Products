import Link from "next/link";
import { Button } from "@/components/ui/Button";

export default function NotFound() {
  return (
    <div className="flex flex-col items-center justify-center gap-6 py-32 text-center px-4">
      <p className="text-8xl font-black text-indigo-100">404</p>
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Page not found</h1>
        <p className="mt-2 text-gray-500">
          The page you&apos;re looking for doesn&apos;t exist.
        </p>
      </div>
      <Link href="/">
        <Button size="lg">Go Home</Button>
      </Link>
    </div>
  );
}
