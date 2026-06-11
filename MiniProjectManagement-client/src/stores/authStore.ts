import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/services/api'
import { useRouter } from 'vue-router'

// Buat interface untuk tipe data User agar TypeScript tidak protes
interface UserData {
    Id: number
    Username: string
    Email: string
}

export const useAuthStore = defineStore('auth', () => {
    const token = ref<string | null>(localStorage.getItem('token'))
    const isAuthenticated = ref<boolean>(!!token.value)

    // 1. TAMBAHKAN STATE USER BARU DI SINI
    // Mengambil data user lama dari localStorage jika ada agar saat di-refresh tidak hilang
    const savedUser = localStorage.getItem('user')
    const user = ref<UserData | null>(savedUser ? JSON.parse(savedUser) : null)

    const router = useRouter()

    const login = async (username: string, password: string) => {
        try {
            const response = await apiClient.post('/auth/login', { username, password })

            const receivedToken = response.data.token

            // 2. AMBIL DATA USER DARI RESPONSE BACKEND (Sesuaikan dengan struktur JSON .NET Anda)
            // Asumsi backend Anda mengembalikan objek user, misal: response.data.user = { id: 1, username: 'santo', email: '...' }
            const userData = response.data.user

            token.value = receivedToken
            isAuthenticated.value = true

            // 3. SIMPAN DATA USER KE STATE DAN LOCALSTORAGE (Gunakan format PascalCase untuk mencocokkan dengan DTO .NET)
            if (userData) {
                user.value = {
                    Id: userData.id || userData.Id,
                    Username: userData.username || userData.Username,
                    Email: userData.email || userData.Email
                }
                localStorage.setItem('user', JSON.stringify(user.value))
            }

            localStorage.setItem('token', receivedToken)

            // Arahkan langsung ke halaman pemilihan workspaces yang baru kita buat, bukan langsung ke dashboard
            router.push('/workspaces')

            return true
        } catch (error) {
            console.error('Login failed:', error)
            return false
        }
    }

    const logout = () => {
        token.value = null
        isAuthenticated.value = false
        user.value = null // Ikut di-reset saat logout
        localStorage.removeItem('token')
        localStorage.removeItem('user')
        localStorage.removeItem('activeWorkspaceId') // Hapus juga workspace aktifnya
        router.push('/login')
    }

    // 4. JANGAN LUPA tambahkan 'user' ke dalam return block bawah ini:
    return { token, isAuthenticated, user, login, logout }
})