<script setup lang="ts">
import { ref } from 'vue'
import { LogIn, Lock, Mail, AlertCircle, Loader2, User } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/authStore'

const authStore = useAuthStore()

// State untuk form
const username = ref('')
const password = ref('')

// State untuk UI feedback
const isLoading = ref(false)
const errorMessage = ref('')

const handleLogin = async () => {
    // Reset pesan error dan mulai loading
    errorMessage.value = ''
    isLoading.value = true

    // Panggil fungsi login dari Pinia store
    const success = await authStore.login(username.value, password.value)
    if (!success) {
        errorMessage.value = 'Username atau password salah. Silakan coba lagi.'
    }

    isLoading.value = false
}
</script>

<template>
    <div class="min-h-screen bg-[#F8F7FC] flex items-center justify-center p-6 font-['Inter']">

        <div class="bg-white w-full max-w-md rounded-[32px] shadow-sm border border-slate-100 p-10">

            <div class="flex justify-center mb-8">
                <div class="bg-violet-100 p-4 rounded-2xl">
                    <LogIn class="w-8 h-8 text-violet-600" />
                </div>
            </div>

            <div class="text-center mb-8">
                <h1 class="text-2xl font-bold text-slate-900">Selamat Datang</h1>
                <p class="text-slate-500 mt-2">Masuk untuk mengelola proyek Anda</p>
            </div>

            <div v-if="errorMessage"
                class="mb-6 p-4 bg-red-50 border border-red-100 rounded-2xl flex items-center gap-3 text-red-600 text-sm">
                <AlertCircle class="w-5 h-5 flex-shrink-0" />
                <p>{{ errorMessage }}</p>
            </div>

            <form @submit.prevent="handleLogin" class="space-y-6">
                <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">Username</label>
                    <div class="relative">
                        <User class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                        <input v-model="username" required
                            class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-2xl py-4 pl-12 pr-4 focus:ring-2 focus:ring-violet-500 transition-all outline-none"
                            placeholder="nama@perusahaan.com" />
                    </div>
                </div>

                <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">Password</label>
                    <div class="relative">
                        <Lock class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                        <input v-model="password" type="password" required
                            class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-2xl py-4 pl-12 pr-4 focus:ring-2 focus:ring-violet-500 transition-all outline-none"
                            placeholder="••••••••" />
                    </div>
                </div>

                <button type="submit" :disabled="isLoading"
                    class="w-full bg-violet-600 hover:bg-violet-700 disabled:bg-violet-400 text-white font-semibold py-4 rounded-2xl shadow-lg shadow-violet-200 transition-all active:scale-[0.98] flex justify-center items-center gap-2">
                    <Loader2 v-if="isLoading" class="w-5 h-5 animate-spin" />
                    <span v-else>Masuk ke Dashboard</span>
                </button>
            </form>

            <div class="mt-8 text-center">
                <p class="text-sm text-slate-500">
                    Belum punya akun?
                    <a href="#" class="text-violet-600 font-semibold hover:underline">Daftar Sekarang</a>
                </p>
            </div>
        </div>
    </div>
</template>