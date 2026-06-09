<script setup lang="ts">
import { MoreHorizontal, Share2, Maximize2, Clock, Eye, Download, Folder } from 'lucide-vue-next'
import { useTaskStore } from '@/stores/taskStore'

const taskStore = useTaskStore()

// Bantuan format tanggal (sama seperti di TaskList)
const formatDate = (dateString: string | null) => {
    if (!dateString) return 'No date set'
    const date = new Date(dateString)
    return new Intl.DateTimeFormat('en-US', {
        day: 'numeric', month: 'short', year: 'numeric',
        hour: '2-digit', minute: '2-digit', hour12: true
    }).format(date)
}
</script>

<template>
    <main class="flex-1 flex flex-col overflow-y-auto bg-[#F8F7FC]">

        <div v-if="taskStore.selectedTask" class="flex-1 flex flex-col">
            <header class="p-8 flex justify-between items-center">
                <div class="flex gap-4">
                    <button class="p-2 hover:bg-slate-100 rounded-lg transition-colors">
                        <Share2 class="w-5 h-5 text-slate-600" />
                    </button>
                    <button
                        class="p-2 hover:bg-slate-100 rounded-lg text-slate-600 flex items-center gap-2 text-sm font-medium transition-colors">
                        <Maximize2 class="w-4 h-4" /> Expand
                    </button>
                </div>
                <MoreHorizontal class="w-6 h-6 text-slate-400 cursor-pointer hover:text-slate-900" />
            </header>

            <div class="px-12 py-4 max-w-4xl">
                <div class="flex flex-wrap items-center justify-between gap-4 mb-6">
                    <div class="flex items-center gap-3">
                        <div class="w-2 h-2 rounded-full bg-violet-600"></div>
                        <span class="text-sm font-semibold text-slate-400 uppercase tracking-wider">
                            {{ taskStore.selectedTask.priority }} PRIORITY
                        </span>
                    </div>

                    <div class="flex items-center gap-2">
                        <span class="text-xs font-medium text-slate-400 uppercase tracking-wider">Status:</span>
                        <select :value="taskStore.selectedTask.boardId"
                            @change="taskStore.updateTaskBoard(taskStore.selectedTask.id, Number(($event.target as HTMLSelectElement).value))"
                            class="bg-white border border-slate-200 text-slate-700 text-xs font-semibold rounded-xl py-2 px-3 focus:ring-2 focus:ring-violet-500 outline-none transition-all cursor-pointer shadow-sm">
                            <option v-for="board in taskStore.boards" :key="board.id" :value="board.id">
                                {{ board.title }}
                            </option>
                        </select>
                    </div>
                </div>

                <h1 class="text-4xl font-bold text-slate-900 mb-8">{{ taskStore.selectedTask.title }}</h1>


                <div
                    class="bg-violet-600 rounded-[32px] p-6 text-white flex justify-between items-center mb-10 shadow-xl shadow-violet-200">
                    <div class="flex items-center gap-4">
                        <div class="p-3 bg-violet-500/50 rounded-2xl">
                            <Clock class="w-6 h-6" />
                        </div>
                        <div>
                            <p class="text-xs text-violet-200">Created At</p>
                            <p class="text-xl font-bold">{{ formatDate(taskStore.selectedTask.createdAt) }}</p>
                        </div>
                    </div>
                </div>

                <div class="mb-10">
                    <h4 class="font-bold text-slate-900 mb-3">Description</h4>
                    <p class="text-slate-500 leading-relaxed text-sm whitespace-pre-wrap">
                        {{ taskStore.selectedTask.description || 'Tidak ada deskripsi untuk tugas ini.' }}
                    </p>
                </div>

            </div>
        </div>

        <div v-else class="flex-1 flex items-center justify-center">
            <div class="text-center text-slate-400">
                <Folder class="w-16 h-16 mx-auto mb-4 text-slate-300 opacity-50" />
                <p class="text-lg font-medium text-slate-500">Pilih tugas untuk melihat detail</p>
                <p class="text-sm mt-1">Klik salah satu kartu di panel sebelah kiri.</p>
            </div>
        </div>

    </main>
</template>