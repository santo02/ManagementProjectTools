import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/services/api'
import { useRouter } from 'vue-router'

export const useAuthStore = defineStore('auth', () => {
    const token = ref<string | null>(localStorage.getItem('token'))
    const isAuthenticated = ref<boolean>(!!token.value)
    const router = useRouter()

    const login = async (username: string, password: string) => {
        try {
            // Memanggil endpoint .NET: POST /api/auth/login (sesuaikan dengan nama endpoint Anda)
            const response = await apiClient.post('/auth/login', { username, password })
            
            // Mengambil token dari response
            const receivedToken = response.data.token

            // Menyimpan token ke memory dan Local Storage
            token.value = receivedToken
            isAuthenticated.value = true
            localStorage.setItem('token', receivedToken)

            // Mengarahkan ke halaman Dashboard setelah sukses
            router.push('/dashboard')

            return true
        } catch (error) {
            console.error('Login failed:', error)
            return false
        }
    }

    const logout = () => {
        token.value = null
        isAuthenticated.value = false
        localStorage.removeItem('token')
        router.push('/login')
    }

    return { token, isAuthenticated, login, logout }
})