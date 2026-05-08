export interface Product {
  id: string
  name: string
  description: string
  price: number
  imageUrl?: string
  category?: string
}

export interface CartItem {
  product: Product
  quantity: number
}

export interface Order {
  id: string
  items: CartItem[]
  totalAmount: number
  status: 'pending' | 'completed' | 'cancelled'
  createdAt: string
}
