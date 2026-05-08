import { useParams, Link } from 'react-router-dom'
import { useProduct } from '../features/products/hooks'
import { useCart } from '../features/cart/cartContext'

export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: product, isLoading, isError } = useProduct(id!)
  const { addItem } = useCart()

  if (isLoading) return <p className="text-gray-500">Loading...</p>

  if (isError || !product) return (
    <div className="space-y-4">
      <p className="text-red-500">Product not found.</p>
      <Link to="/products" className="inline-flex px-4 py-2 border border-indigo-500 text-indigo-500 rounded-lg font-semibold hover:bg-indigo-500 hover:text-white transition-colors">
        Back to Products
      </Link>
    </div>
  )

  return (
    <div>
      <Link to="/products" className="text-sm text-gray-500 hover:text-indigo-500 transition-colors inline-block mb-6">
        ← Back to Products
      </Link>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-12">
        <div className="rounded-lg overflow-hidden border border-gray-200 bg-indigo-50 flex items-center justify-center min-h-64">
          {product.imageUrl
            ? <img src={product.imageUrl} alt={product.name} className="w-full block" />
            : <span className="text-8xl">📦</span>
          }
        </div>
        <div className="flex flex-col gap-4">
          {product.category && (
            <span className="self-start px-2.5 py-0.5 bg-indigo-50 text-indigo-600 text-xs font-semibold rounded-full">
              {product.category}
            </span>
          )}
          <h1 className="text-3xl font-extrabold text-gray-900">{product.name}</h1>
          <p className="text-gray-500 leading-relaxed">{product.description}</p>
          <p className="text-4xl font-extrabold text-indigo-600">${product.price.toFixed(2)}</p>
          <button
            className="self-start px-8 py-3 bg-indigo-500 hover:bg-indigo-600 text-white font-semibold rounded-lg text-base transition-colors cursor-pointer"
            onClick={() => addItem(product)}
          >
            Add to Cart
          </button>
        </div>
      </div>
    </div>
  )
}
