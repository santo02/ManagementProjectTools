<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import {
    Layers, LayoutGrid, ListFilter, Plus, ArrowLeft,
    Search, SlidersHorizontal, Loader2, Calendar,
    ListTodo, BarChart3, UserPlus, Check, X as CloseIcon
} from 'lucide-vue-next'
import Sidebar from './components/Sidebar.vue'
import TaskList from './components/TaskList.vue'
import TaskDetail from './components/TaskDetail.vue'
import { useWorkspaceStore } from '@/stores/workspaceStore'
import { useTaskStore } from '@/stores/taskStore'
import { useAuthStore } from '@/stores/authStore'

const workspaceStore = useWorkspaceStore()
const taskStore = useTaskStore()
const authStore = useAuthStore()
const router = useRouter()

const isDetailOpen = ref(false)
const selectedTask = ref<any>(null)
const isAddingBoard = ref(false)
const newBoardName = ref('')

const handleOpenDetail = (task: any) => {
    selectedTask.value = task
    isDetailOpen.value = true
}

const loadBoardData = async () => {
    if (workspaceStore.currentWorkspace?.id) {
        await taskStore.fetchBoardsAndTasks(workspaceStore.currentWorkspace.id)
    }
}

onMounted(async () => {
    try {
        if (workspaceStore.workspaces.length === 0) {
            await workspaceStore.fetchWorkspace()
        }
        const savedId = localStorage.getItem('activeWorkspaceId')
        if (savedId) {
            const activeWs = workspaceStore.workspaces.find(w => (w.id || w.Id) === parseInt(savedId))
            if (activeWs) {
                const currentUserId = authStore.user?.Id || 1
                workspaceStore.setActiveWorkspace(activeWs, currentUserId)
                await loadBoardData()
                return
            }
        }
        if (!workspaceStore.currentWorkspace) {
            router.push('/workspaces')
        } else {
            await loadBoardData()
        }
    } catch (error) {
        console.error("Gagal melakukan inisialisasi Dashboard:", error)
        router.push('/workspaces')
    }
})

watch(() => workspaceStore.currentWorkspace, async () => {
    await loadBoardData()
})

// Fungsi pembantu format tanggal standar industri
const formatDate = (dateString: string) => {
    if (!dateString || dateString.startsWith('0001')) return 'Baru Dibuat'
    return new Date(dateString).toLocaleDateString('id-ID', {
        day: 'numeric',
        month: 'long',
        year: 'numeric'
    })
}

// Fungsi hitung statistik kalkulasi progres proyek secara riil
const getTotalTasksCount = () => {
    return Object.values(taskStore.tasksByBoard).reduce((acc, curr) => acc + curr.length, 0)
}

const handleCreateBoard = async () => {
    if (!newBoardName.value.trim() || !workspaceStore.currentWorkspace?.id) return

    try {
        await taskStore.addBoard(newBoardName.value.trim(), workspaceStore.currentWorkspace.id)
        newBoardName.value = '' // Reset input
        isAddingBoard.value = false // Tutup form inline
    } catch (error) {
        alert("Gagal membuat kolom baru")
    }
}
</script>

<template>
    <div class="flex h-screen bg-[#F8F7FC] font-['Inter'] overflow-hidden">

        <Sidebar :show-new-project-button="false" />

        <div class="flex-1 flex flex-col overflow-hidden">

            <header
                class="px-8 py-5 bg-white border-b border-slate-100 flex justify-between items-center shrink-0 shadow-[0_1px_2px_rgba(0,0,0,0,02)]">
                <div class="flex items-center gap-4">
                    <button @click="router.push('/workspaces')"
                        class="p-2.5 bg-slate-50 hover:bg-slate-100 text-slate-400 hover:text-slate-600 rounded-xl transition-all border border-slate-100">
                        <ArrowLeft class="w-4 h-4" />
                    </button>
                    <div>
                        <div class="flex items-center gap-2.5">
                            <h2 class="text-xl font-extrabold text-slate-800 tracking-tight">
                                {{ workspaceStore.currentWorkspace?.name || workspaceStore.currentWorkspace?.Name ||
                                    'Memuat Proyek...' }}
                            </h2>
                            <span
                                class="text-[10px] bg-violet-100 text-violet-700 font-extrabold px-2.5 py-0.5 rounded-full uppercase tracking-wider">
                                {{ workspaceStore.isLeader ? 'Leader' : 'Member' }}
                            </span>
                        </div>
                        <p class="text-xs text-slate-400 font-medium flex items-center gap-1.5 mt-0.5">
                            <Calendar class="w-3.5 h-3.5 text-slate-300" />
                            Dibuat pada: <span class="text-slate-500 font-semibold">{{
                                formatDate(workspaceStore.currentWorkspace?.createdAt ||
                                    workspaceStore.currentWorkspace?.CreatedAt) }}</span>
                        </p>
                    </div>
                </div>

                <div class="flex items-center gap-3">
                    <div class="relative w-64 hidden sm:block">
                        <Search class="w-3.5 h-3.5 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
                        <input type="text" placeholder="Cari tugas di papan..."
                            class="w-full bg-slate-50 border-none rounded-xl py-2 pl-9 pr-4 text-xs outline-none ring-1 ring-slate-100 focus:ring-2 focus:ring-violet-500 transition-all font-medium text-slate-600" />
                    </div>
                    <button
                        class="p-2 bg-slate-50 text-slate-500 border border-slate-100 rounded-xl hover:bg-slate-100 transition-all">
                        <SlidersHorizontal class="w-4 h-4" />
                    </button>
                    <button
                        class="flex items-center gap-1.5 bg-violet-600 hover:bg-violet-700 text-white text-xs font-bold px-4 py-2.5 rounded-xl transition-all shadow-sm shadow-violet-100">
                        <UserPlus class="w-3.5 h-3.5" /> Undang Tim
                    </button>
                </div>
            </header>

            <div
                class="px-8 py-3.5 bg-white border-b border-slate-100 flex flex-wrap justify-between items-center gap-4 shrink-0">
                <div class="flex gap-5 text-xs font-bold text-slate-400">
                    <span class="hover:text-slate-600 cursor-pointer flex items-center gap-1.5 py-1">
                        <BarChart3 class="w-3.5 h-3.5" /> Overview
                    </span>
                    <span
                        class="text-violet-600 border-b-2 border-violet-600 pb-3 cursor-pointer flex items-center gap-1.5">
                        <LayoutGrid class="w-3.5 h-3.5" /> Papan Kanban
                    </span>
                    <span class="hover:text-slate-600 cursor-pointer flex items-center gap-1.5 py-1">
                        <ListFilter class="w-3.5 h-3.5" /> Daftar List
                    </span>
                </div>

                <div
                    class="flex items-center gap-6 bg-slate-50/60 border border-slate-100 px-5 py-2 rounded-2xl text-[11px] font-bold text-slate-500">
                    <div class="flex items-center gap-1.5">
                        <ListTodo class="w-4 h-4 text-violet-500" />
                        Total Tugas: <span class="text-slate-800 text-xs font-extrabold">{{ getTotalTasksCount()
                            }}</span>
                    </div>
                    <div class="h-4 w-[1px] bg-slate-200"></div>
                    <div class="flex items-center gap-2">
                        <span class="text-slate-400">Tim Proyek:</span>
                        <div class="flex -space-x-1.5 overflow-hidden">
                            <img class="inline-block h-5.5 w-5.5 rounded-full ring-2 ring-white object-cover"
                                src="https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=80" />
                            <img class="inline-block h-5.5 w-5.5 rounded-full ring-2 ring-white object-cover"
                                src="https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=80" />
                            <div
                                class="inline-block h-5.5 w-5.5 rounded-full bg-slate-200 text-slate-600 text-[9px] font-extrabold flex items-center justify-center ring-2 ring-white">
                                +2</div>
                        </div>
                    </div>
                </div>
            </div>

            <main class="flex-1 overflow-x-auto p-8 flex gap-6 items-start custom-scrollbar">

                <div v-if="taskStore.isLoading"
                    class="flex-1 flex flex-col items-center justify-center py-20 text-slate-400">
                    <Loader2 class="w-8 h-8 animate-spin text-violet-600 mb-2" />
                    <p class="text-xs font-medium">Sinkronisasi data database...</p>
                </div>

                <template v-else>
                    <template v-if="taskStore.boards && taskStore.boards.length > 0">

                        <TaskList v-for="board in taskStore.boards" :key="board.id" :board-id="board.id"
                            :title="board.Title || board.title || 'Kolom Tugas'"
                            :tasks="taskStore.tasksByBoard[board.id] || []" @select-task="handleOpenDetail" />

                        <div
                            class="w-72 shrink-0 bg-slate-100/50 hover:bg-slate-100 border border-dashed border-slate-200 rounded-2xl p-4 transition-all duration-200">
                            <button v-if="!isAddingBoard" @click="isAddingBoard = true"
                                class="w-full flex items-center justify-center gap-2 text-slate-500 hover:text-slate-800 text-xs font-bold py-2 transition-all">
                                <Plus class="w-4 h-4" /> Tambah Kolom Baru
                            </button>

                            <div v-else class="space-y-3">
                                <input v-model="newBoardName" @keyup.enter="handleCreateBoard" type="text"
                                    placeholder="Masukkan nama kolom (misal: Testing)..."
                                    class="w-full bg-white border border-slate-200 rounded-xl px-3 py-2 text-xs font-semibold text-slate-700 outline-none ring-1 ring-transparent focus:ring-violet-500 focus:border-violet-500 transition-all"
                                    autofocus />
                                <div class="flex items-center gap-2">
                                    <button @click="handleCreateBoard"
                                        class="flex items-center gap-1 bg-violet-600 hover:bg-violet-700 text-white text-[11px] font-bold px-3 py-1.5 rounded-lg transition-all shadow-sm">
                                        <Check class="w-3.5 h-3.5" /> Simpan
                                    </button>
                                    <button @click="isAddingBoard = false; newBoardName = ''"
                                        class="p-1.5 bg-white text-slate-400 hover:text-slate-600 border border-slate-200 rounded-lg hover:bg-slate-50 transition-all">
                                        <CloseIcon class="w-3.5 h-3.5" />
                                    </button>
                                </div>
                            </div>
                        </div>

                    </template>

                    <template v-else-if="!isAddingBoard">
                        <div class="flex-1 flex items-center justify-center py-16">
                            <div
                                class="text-center bg-white border border-slate-100 rounded-[32px] p-12 max-w-sm shadow-sm">
                                <div
                                    class="w-12 h-12 rounded-2xl bg-violet-50 text-violet-600 flex items-center justify-center mb-4 mx-auto">
                                    <Layers class="w-6 h-6" />
                                </div>
                                <h3 class="text-sm font-bold text-slate-800">Papan Kerja Kosong</h3>
                                <p class="text-xs text-slate-400 font-medium mt-1 leading-relaxed">
                                    Proyek baru ini belum memiliki kolom alur kerja bawaan.
                                </p>
                                <button @click="isAddingBoard = true"
                                    class="mt-4 bg-violet-600 hover:bg-violet-700 text-white text-xs font-bold px-4 py-2.5 rounded-xl transition-all shadow-sm">
                                    + Buat Kolom Pertama
                                </button>
                            </div>
                        </div>
                    </template>

                    <template v-else>
                        <div class="flex-1 flex items-center justify-center py-16">
                            <div
                                class="bg-white border border-slate-100 rounded-[32px] p-8 w-full max-w-sm shadow-md space-y-4">
                                <h3 class="text-sm font-bold text-slate-800">Nama Kolom Pertama</h3>
                                <input v-model="newBoardName" @keyup.enter="handleCreateBoard" type="text"
                                    placeholder="Masukkan nama kolom (misal: To Do)..."
                                    class="w-full bg-slate-50 border border-slate-200 rounded-xl px-4 py-2.5 text-xs font-semibold text-slate-700 outline-none focus:ring-2 focus:ring-violet-500 transition-all"
                                    autofocus />
                                <div class="flex items-center gap-2 justify-end">
                                    <button @click="isAddingBoard = false; newBoardName = ''"
                                        class="px-4 py-2 text-xs font-bold text-slate-500 bg-slate-100 rounded-xl hover:bg-slate-200 transition-all">
                                        Batal
                                    </button>
                                    <button @click="handleCreateBoard"
                                        class="flex items-center gap-1 bg-violet-600 hover:bg-violet-700 text-white text-xs font-bold px-4 py-2 rounded-xl transition-all shadow-sm">
                                        <Check class="w-3.5 h-3.5" /> Simpan Kolom
                                    </button>
                                </div>
                            </div>
                        </div>
                    </template>
                </template>

            </main>
        </div>

        <TaskDetail :is-open="isDetailOpen" :task="selectedTask" @close="isDetailOpen = false" />
    </div>
</template>