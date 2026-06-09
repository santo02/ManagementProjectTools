import axios from 'axios'

// Ganti URL ini dengan URL dari proyek .NET Anda
const apiClient = axios.create({
    baseURL: 'http://localhost:5134/api',
    headers: {
        'Content-Type': 'application/json'
    }
})

// Interceptor: Otomatis menyisipkan Token JWT ke setiap request (jika ada)
apiClient.interceptors.request.use((config) => {
    const token = localStorage.getItem('token')
    if (token) {
        config.headers.Authorization = `Bearer ${token}`
    }
    return config
}, (error) => {
    return Promise.reject(error)
})

export default apiClient