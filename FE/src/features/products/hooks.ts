import { useQuery } from '@tanstack/react-query'
import { getProducts, getProductById } from './api'

export const useProducts = () =>
  useQuery({ queryKey: ['products'], queryFn: getProducts })

export const useProduct = (id: string) =>
  useQuery({ queryKey: ['products', id], queryFn: () => getProductById(id), enabled: !!id })
