import { useState } from 'react'
import { useProducts } from '../features/products/hooks'
import { useCart } from '../features/cart/cartContext'
import ProductCard from '../features/products/ProductCard'

export default function ProductsPage() {
  const { data: products, isLoading, isError } = useProducts()
  const { addItem } = useCart()
  const [search, setSearch] = useState('')

  const filtered = products?.filter(p =>
    p.name.toLowerCase().includes(search.toLowerCase()) ||
    p.description.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div>
      <div className="flex items-center justify-between flex-wrap gap-4 mb-6">
        <h1 className="text-2xl font-bold text-gray-900">All Products</h1>
        <input
          type="search"
          placeholder="Search products..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          className="px-4 py-2 border border-gray-200 rounded-lg text-sm w-64 outline-none focus:border-indigo-500 transition-colors"
        />
      </div>

      {isLoading && <p className="text-gray-500">Loading products...</p>}
      {isError && <p className="text-red-500">Failed to load products. Is the API running?</p>}

      {filtered && (
        <>
          <p className="text-sm text-gray-500 mb-4">
            {filtered.length} product{filtered.length !== 1 ? 's' : ''} found
          </p>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {filtered.map(p => (
              <ProductCard key={p.id} product={p} onAddToCart={addItem} />
            ))}
          </div>
          {filtered.length === 0 && (
            <p className="text-gray-500 mt-4">No products match your search.</p>
          )}
        </>
      )}
    </div>
  )
}
