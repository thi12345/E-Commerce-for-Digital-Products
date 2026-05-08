import { Link, NavLink } from 'react-router-dom'
import { useCart } from '../../features/cart/cartContext'

export default function Header() {
  const { totalItems } = useCart()

  return (
    <header className="bg-white border-b border-gray-200 shadow-sm sticky top-0 z-10">
      <div className="max-w-6xl mx-auto px-6 flex items-center gap-8 h-15">
        <Link to="/" className="text-xl font-bold text-indigo-500">
          DigitalShop
        </Link>
        <nav className="flex gap-6 flex-1">
          <NavLink
            to="/" end
            className={({ isActive }) =>
              isActive ? 'font-medium text-indigo-500' : 'font-medium text-gray-500 hover:text-indigo-500 transition-colors'
            }
          >
            Home
          </NavLink>
          <NavLink
            to="/products"
            className={({ isActive }) =>
              isActive ? 'font-medium text-indigo-500' : 'font-medium text-gray-500 hover:text-indigo-500 transition-colors'
            }
          >
            Products
          </NavLink>
        </nav>
        <Link to="/cart" className="relative text-2xl">
          🛒
          {totalItems > 0 && (
            <span className="absolute -top-1.5 -right-2 bg-indigo-500 text-white text-xs font-bold w-5 h-5 rounded-full flex items-center justify-center">
              {totalItems}
            </span>
          )}
        </Link>
      </div>
    </header>
  )
}
