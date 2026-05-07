export function LoadingSpinner({ text = "Loading..." }: { text?: string }) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 py-20">
      <svg
        className="h-10 w-10 animate-spin text-indigo-600"
        viewBox="0 0 24 24"
        fill="none"
      >
        <circle
          className="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          strokeWidth="4"
        />
        <path
          className="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"
        />
      </svg>
      <p className="text-sm text-gray-500">{text}</p>
    </div>
  );
}

export function PageSkeleton() {
  return (
    <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      {Array.from({ length: 6 }).map((_, i) => (
        <div key={i} className="animate-pulse rounded-xl bg-white shadow-sm p-5">
          <div className="h-40 rounded-lg bg-gray-200 mb-4" />
          <div className="h-4 rounded bg-gray-200 mb-2 w-3/4" />
          <div className="h-3 rounded bg-gray-200 mb-4 w-1/2" />
          <div className="h-6 rounded bg-gray-200 w-1/3" />
        </div>
      ))}
    </div>
  );
}
