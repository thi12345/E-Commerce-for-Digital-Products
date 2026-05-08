import { Link } from 'react-router-dom'
import type { Product } from '../../types'

interface Props {
  product: Product
  onAddToCart: (product: Product) => void
}

export default function ProductCard({ product, onAddToCart }: Props) {
  return (
    <div className="bg-white border border-gray-200 rounded-lg shadow-sm flex flex-col overflow-hidden transition-all duration-200 hover:shadow-md hover:-translate-y-0.5">
      <Link to={`/products/${product.id}`}>
        <div className="aspect-video overflow-hidden bg-indigo-50 flex items-center justify-center">
          {product.imageUrl
            ? <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
            : <span className="text-4xl">📦</span>
          }
        </div>
        <div className="p-4 flex-1">
          {product.category && (
            <span className="inline-block px-2.5 py-0.5 bg-indigo-50 text-indigo-600 text-xs font-semibold rounded-full mb-2">
              {product.category}
            </span>
          )}
          <h3 className="font-bold text-gray-900 mb-1">{product.name}</h3>
          <p className="text-sm text-gray-500 line-clamp-2">{product.description}</p>
        </div>
      </Link>
      <div className="px-4 py-3 border-t border-gray-200 flex items-center justify-between gap-2">
        <span className="text-lg font-bold text-indigo-600">${product.price.toFixed(2)}</span>
        <button
          className="px-4 py-1.5 bg-indigo-500 hover:bg-indigo-600 text-white text-sm font-semibold rounded-lg transition-colors cursor-pointer"
          onClick={() => onAddToCart(product)}
        >
          Add to Cart
        </button>
      </div>
    </div>
  )
}
