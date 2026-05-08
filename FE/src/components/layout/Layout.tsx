import { Outlet } from 'react-router-dom'
import Header from './Header'
import Footer from './Footer'

export default function Layout() {
  return (
    <div className="flex flex-col min-h-screen bg-gray-50">
      <Header />
      <main className="flex-1 py-8">
        <div className="max-w-6xl mx-auto px-6">
          <Outlet />
        </div>
      </main>
      <Footer />
    </div>
  )
}
