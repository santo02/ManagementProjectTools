<script setup lang="ts">
import { ref } from 'vue'
import { X, Loader2 } from 'lucide-vue-next'
import { useTaskStore } from '@/stores/taskStore'

const props = defineProps<{ isOpen: boolean }>()
const emit = defineEmits(['close'])

const taskStore = useTaskStore()
const isLoading = ref(false)

// State Form
const title = ref('')
const description = ref('')
const priority = ref('Low')
// Set default boardId ke board pertama yang ada di database (biasanya TODO)
const boardId = ref(taskStore.boards.length > 0 ? taskStore.boards[0].id : 1)

const handleSubmit = async () => {
    isLoading.value = true

    const success = await taskStore.addTask({
        title: title.value,
        description: description.value,
        priority: priority.value,
        boardId: Number(boardId.value)
    })

    isLoading.value = false

    if (success) {
        // Reset form
        title.value = ''
        description.value = ''
        priority.value = 'Low'
        // Tutup modal
        emit('close')
    }
}
</script>

<template>
    <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm" @click="emit('close')"></div>

        <div
            class="relative bg-white w-full max-w-lg rounded-[32px] shadow-xl p-8 z-10 animate-in fade-in zoom-in duration-200">
            <div class="flex justify-between items-center mb-6">
                <h2 class="text-2xl font-bold text-slate-900">Buat Tugas Baru</h2>
                <button @click="emit('close')"
                    class="p-2 hover:bg-slate-100 rounded-full text-slate-400 hover:text-slate-900 transition-colors">
                    <X class="w-5 h-5" />
                </button>
            </div>

            <form @submit.prevent="handleSubmit" class="space-y-5">
                <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">Judul Tugas</label>
                    <input v-model="title" type="text" required
                        class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-2xl py-3 px-4 focus:ring-2 focus:ring-violet-500 outline-none"
                        placeholder="Contoh: Perbaiki bug di halaman login" />
                </div>

                <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">Deskripsi</label>
                    <textarea v-model="description" rows="3"
                        class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-2xl py-3 px-4 focus:ring-2 focus:ring-violet-500 outline-none resize-none"
                        placeholder="Tambahkan detail tugas di sini..."></textarea>
                </div>

                <div class="flex gap-4">
                    <div class="flex-1">
                        <label class="block text-sm font-medium text-slate-700 mb-2">Prioritas</label>
                        <select v-model="priority"
                            class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-2xl py-3 px-4 focus:ring-2 focus:ring-violet-500 outline-none appearance-none">
                            <option value="Low">Low Priority</option>
                            <option value="Medium">Medium Priority</option>
                            <option value="High">High Priority</option>
                        </select>
                    </div>

                    <div class="flex-1">
                        <label class="block text-sm font-medium text-slate-700 mb-2">Kolom (Status)</label>
                        <select v-model="boardId"
                            class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-2xl py-3 px-4 focus:ring-2 focus:ring-violet-500 outline-none appearance-none">
                            <option v-for="board in taskStore.boards" :key="board.id" :value="board.id">
                                {{ board.title }}
                            </option>
                        </select>
                    </div>
                </div>

                <div class="pt-4 flex gap-3">
                    <button type="button" @click="emit('close')"
                        class="flex-1 py-3.5 rounded-2xl font-semibold text-slate-700 bg-slate-100 hover:bg-slate-200 transition-colors">
                        Batal
                    </button>
                    <button type="submit" :disabled="isLoading"
                        class="flex-1 py-3.5 rounded-2xl font-semibold text-white bg-violet-600 hover:bg-violet-700 disabled:bg-violet-400 shadow-lg shadow-violet-200 transition-all flex justify-center items-center gap-2">
                        <Loader2 v-if="isLoading" class="w-5 h-5 animate-spin" />
                        <span v-else>Simpan Tugas</span>
                    </button>
                </div>
            </form>
        </div>
    </div>
</template>