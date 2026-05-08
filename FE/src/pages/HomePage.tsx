import { Link } from 'react-router-dom'
import { useProducts } from '../features/products/hooks'
import { useCart } from '../features/cart/cartContext'
import ProductCard from '../features/products/ProductCard'

export default function HomePage() {
  const { data: products, isLoading } = useProducts()
  const { addItem } = useCart()
  const featured = products?.slice(0, 4)

  return (
    <div>
      <section className="text-center py-20 px-4 bg-gradient-to-br from-indigo-50 to-purple-50 rounded-2xl mb-12">
        <h1 className="text-4xl font-extrabold text-gray-900 mb-3">
          Discover Premium Digital Products
        </h1>
        <p className="text-lg text-gray-500 mb-8">
          Software, templates, courses and more — instant download.
        </p>
        <Link
          to="/products"
          className="inline-flex items-center px-8 py-3 bg-indigo-500 hover:bg-indigo-600 text-white font-semibold rounded-lg text-base transition-colors"
        >
          Browse All Products
        </Link>
      </section>

      <section>
        <h2 className="text-2xl font-bold text-gray-900 mb-6">Featured Products</h2>
        {isLoading && <p className="text-gray-500">Loading...</p>}
        {featured && featured.length > 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            {featured.map(p => (
              <ProductCard key={p.id} product={p} onAddToCart={addItem} />
            ))}
          </div>
        )}
        {featured?.length === 0 && (
          <p className="text-gray-500">No products available yet.</p>
        )}
      </section>
    </div>
  )
}
