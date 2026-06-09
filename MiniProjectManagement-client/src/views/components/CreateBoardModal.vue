<script setup lang="ts">
import { ref, watch } from 'vue'
import { X, Loader2 } from 'lucide-vue-next'
import { useTaskStore } from '@/stores/taskStore'

const props = defineProps<{ isOpen: boolean }>()
const emit = defineEmits(['close'])

const taskStore = useTaskStore()
const isLoading = ref(false)

// State Form
const title = ref('')
const position = ref(1)

// Reset form dan set default posisi (ke urutan paling akhir) setiap kali modal dibuka
watch(() => props.isOpen, (newVal) => {
    if (newVal) {
        title.value = ''
        position.value = taskStore.boards.length > 0
            ? Math.max(...taskStore.boards.map(b => b.position)) + 1
            : 1
    }
})

const handleSubmit = async () => {
    if (!title.value.trim()) return

    isLoading.value = true
    // Mengirim title dan position ke store
    const success = await taskStore.addBoard(title.value, position.value)
    isLoading.value = false

    if (success) {
        emit('close')
    }
}
</script>

<template>
    <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm" @click="emit('close')"></div>

        <div
            class="relative bg-white w-full max-w-md rounded-[32px] shadow-xl p-8 z-10 animate-in fade-in zoom-in duration-200">
            <div class="flex justify-between items-center mb-6">
                <h2 class="text-2xl font-bold text-slate-900">Tambah Kolom Baru</h2>
                <button @click="emit('close')"
                    class="p-2 hover:bg-slate-100 rounded-full text-slate-400 hover:text-slate-900 transition-colors">
                    <X class="w-5 h-5" />
                </button>
            </div>

            <form @submit.prevent="handleSubmit" class="space-y-6">
                <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">Nama Kolom (Board)</label>
                    <input v-model="title" type="text" required
                        class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-2xl py-4 px-5 focus:ring-2 focus:ring-violet-500 outline-none uppercase text-sm"
                        placeholder="Contoh: QA TESTING" />
                </div>

                <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">Urutan (Position)</label>
                    <input v-model="position" type="number" min="1" required
                        class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-2xl py-4 px-5 focus:ring-2 focus:ring-violet-500 outline-none"
                        placeholder="1" />
                    <p class="text-xs text-slate-400 mt-2">Kolom akan diurutkan dari angka terkecil (kiri) ke terbesar
                        (kanan).</p>
                </div>

                <div class="pt-2 flex gap-3">
                    <button type="button" @click="emit('close')"
                        class="flex-1 py-3.5 rounded-2xl font-semibold text-slate-700 bg-slate-100 hover:bg-slate-200 transition-colors">
                        Batal
                    </button>
                    <button type="submit" :disabled="isLoading"
                        class="flex-1 py-3.5 rounded-2xl font-semibold text-white bg-violet-600 hover:bg-violet-700 disabled:bg-violet-400 shadow-lg shadow-violet-200 transition-all flex justify-center items-center gap-2">
                        <Loader2 v-if="isLoading" class="w-5 h-5 animate-spin" />
                        <span v-else>Simpan Kolom</span>
                    </button>
                </div>
            </form>
        </div>
    </div>
</template>