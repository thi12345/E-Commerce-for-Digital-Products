import { Link } from 'react-router-dom'
import { useCart } from '../features/cart/cartContext'

export default function CartPage() {
  const { items, removeItem, updateQuantity, clearCart, totalPrice } = useCart()

  if (items.length === 0) {
    return (
      <div className="flex flex-col items-center gap-4 py-24 text-center">
        <p className="text-xl text-gray-500">🛒 Your cart is empty.</p>
        <Link
          to="/products"
          className="px-6 py-2.5 bg-indigo-500 hover:bg-indigo-600 text-white font-semibold rounded-lg transition-colors"
        >
          Browse Products
        </Link>
      </div>
    )
  }

  return (
    <div>
      <div className="flex items-center justify-between flex-wrap gap-4 mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Your Cart</h1>
        <button
          onClick={clearCart}
          className="px-4 py-2 border border-gray-200 text-gray-500 hover:bg-gray-100 font-semibold rounded-lg text-sm transition-colors cursor-pointer"
        >
          Clear Cart
        </button>
      </div>

      <div className="flex flex-col gap-3 mb-8">
        {items.map(({ product, quantity }) => (
          <div key={product.id} className="bg-white border border-gray-200 rounded-lg px-5 py-4 flex items-center gap-6 flex-wrap">
            <div className="flex-1 min-w-30">
              <Link to={`/products/${product.id}`} className="font-semibold text-gray-900 hover:text-indigo-500 transition-colors">
                {product.name}
              </Link>
              <p className="text-xs text-gray-500 mt-0.5">${product.price.toFixed(2)} each</p>
            </div>
            <div className="flex items-center gap-2">
              <button
                onClick={() => updateQuantity(product.id, quantity - 1)}
                className="w-7 h-7 rounded-full border border-gray-200 flex items-center justify-center hover:bg-gray-100 font-bold cursor-pointer transition-colors"
              >
                −
              </button>
              <span className="w-8 text-center font-semibold">{quantity}</span>
              <button
                onClick={() => updateQuantity(product.id, quantity + 1)}
                className="w-7 h-7 rounded-full border border-gray-200 flex items-center justify-center hover:bg-gray-100 font-bold cursor-pointer transition-colors"
              >
                +
              </button>
            </div>
            <span className="font-bold text-gray-900 min-w-16 text-right">
              ${(product.price * quantity).toFixed(2)}
            </span>
            <button
              onClick={() => removeItem(product.id)}
              className="text-sm text-gray-400 hover:text-red-500 transition-colors cursor-pointer"
            >
              Remove
            </button>
          </div>
        ))}
      </div>

      <div className="flex justify-end items-center gap-8 bg-white border border-gray-200 rounded-lg p-6 mb-6">
        <span className="text-gray-500 text-lg">Total</span>
        <span className="text-3xl font-extrabold text-indigo-600">${totalPrice.toFixed(2)}</span>
      </div>

      <div className="flex gap-3 justify-end">
        <Link
          to="/products"
          className="px-6 py-2.5 border-2 border-indigo-500 text-indigo-500 hover:bg-indigo-500 hover:text-white font-semibold rounded-lg transition-colors"
        >
          Continue Shopping
        </Link>
        <button
          onClick={() => alert('Checkout coming soon!')}
          className="px-8 py-2.5 bg-indigo-500 hover:bg-indigo-600 text-white font-semibold rounded-lg text-base transition-colors cursor-pointer"
        >
          Checkout
        </button>
      </div>
    </div>
  )
}
