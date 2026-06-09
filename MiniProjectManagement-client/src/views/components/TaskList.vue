<script setup lang="ts">
import { Search, Clock, Plus } from 'lucide-vue-next'
import { useTaskStore } from '@/stores/taskStore'
import CreateTaskModal from '../components/CreateTaskModal.vue'
import CreateBoardModal from '../components/CreateBoardModal.vue'
import { ref } from 'vue'

const taskStore = useTaskStore()
const isModalOpen = ref(false)
const isBoardModalOpen = ref(false)

// Fungsi bantuan untuk menentukan warna badge prioritas
const getPriorityColor = (priority: string) => {
    if (priority?.toLowerCase() === 'high') return 'bg-rose-100 text-rose-600'
    if (priority?.toLowerCase() === 'medium') return 'bg-violet-100 text-violet-600'
    return 'bg-green-100 text-green-600'
}

const formatDate = (dateString: string | null) => {
    if (!dateString) return 'No date set'

    const date = new Date(dateString)

    // Menggunakan Intl.DateTimeFormat untuk format yang rapi dan konsisten
    return new Intl.DateTimeFormat('id-ID', {
        day: 'numeric',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true // Menggunakan format AM/PM sesuai desain referensi
    }).format(date)
}
</script>

<template>
    <section class="w-[450px] bg-white border-r border-slate-100 flex flex-col shrink-0">
        <div class="p-6">
            <div class="flex justify-between items-center mb-6">
                <h2 class="text-xl font-bold text-slate-900">My Events</h2>
                <div class="flex gap-3">
                    <button class="p-2 hover:bg-slate-100 rounded-full transition-colors">
                        <Search class="w-5 h-5 text-slate-400" />
                    </button>
                    <button @click="isBoardModalOpen = true"
                        class="p-2 bg-violet-600 hover:bg-violet-700 text-white rounded-full transition-colors shadow-md shadow-violet-200">
                        <Plus class="w-5 h-5" />
                    </button>
                </div>
            </div>


            <div class="flex gap-4 text-sm font-medium text-slate-400 border-b border-slate-100 pb-2">
                <span class="text-slate-900 border-b-2 border-slate-900 pb-2 cursor-pointer">All</span>
                <span class="hover:text-slate-900 cursor-pointer">Remote</span>
                <span class="hover:text-slate-900 cursor-pointer">In Person</span>
            </div>
        </div>

        <div class="flex-1 overflow-y-auto px-6 pb-6 space-y-8">

            <div v-if="taskStore.isLoading" class="text-center text-slate-400 py-10">
                Memuat data tugas...
            </div>

            <div v-else>
                <div v-for="board in taskStore.boards" :key="board.id" class="mb-8">

                    <div class="flex items-center gap-2 mb-4">
                        <h3 class="text-xs font-bold text-slate-400 tracking-wider uppercase">{{ board.title }}</h3>
                        <span class="bg-slate-100 text-slate-500 text-[10px] px-2 py-0.5 rounded-full font-bold">
                            {{ taskStore.tasksByBoard[board.id]?.length || 0 }}
                        </span>
                    </div>

                    <div class="space-y-4">
                        <div v-for="item in taskStore.tasksByBoard[board.id]" :key="item.id"
                            @click="taskStore.selectTask(item)" :class="[
                                'p-5 rounded-[24px] border hover:shadow-md transition-all cursor-pointer bg-white group',
                                taskStore.selectedTask?.id === item.id ? 'border-violet-500 ring-1 ring-violet-500 shadow-sm' : 'border-slate-100'
                            ]">
                            <div class="flex justify-between items-start mb-3 gap-4">
                                <h3
                                    class="font-semibold text-slate-900 leading-tight group-hover:text-violet-600 transition-colors">
                                    {{ item.title }}</h3>

                                <span
                                    :class="['px-3 py-1 rounded-full text-[10px] font-bold whitespace-nowrap', getPriorityColor(item.priority)]">
                                    {{ item.priority }} priority
                                </span>
                            </div>
                            <div class="flex items-center gap-2 text-xs text-slate-400">
                                <Clock class="w-3 h-3" /> {{ formatDate(item.createdAt) }}
                            </div>
                        </div>

                        <div v-if="!taskStore.tasksByBoard[board.id]?.length"
                            class="p-5 rounded-[24px] border border-dashed border-slate-200 text-center text-slate-400 text-sm">
                            Tidak ada tugas
                        </div>
                    </div>
                </div>
                <button @click="isModalOpen = true"
                    class="w-full py-4 mt-2 border-2 border-dashed border-slate-200 rounded-[24px] text-slate-400 font-semibold hover:border-violet-400 hover:text-violet-600 transition-colors flex items-center justify-center gap-2">
                    <Plus class="w-5 h-5" /> Tambah Tugas
                </button>
            </div>
        </div>
        <CreateTaskModal :is-open="isModalOpen" @close="isModalOpen = false" />
        <CreateBoardModal :is-open="isBoardModalOpen" @close="isBoardModalOpen = false" />
    </section>
</template>