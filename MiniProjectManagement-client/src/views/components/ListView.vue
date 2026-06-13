<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import {
    Plus, ListFilter, MoreVertical, Calendar, Clock, Loader2,
    CheckCircle2, AlertCircle, HelpCircle, ChevronLeft, ChevronRight
} from 'lucide-vue-next'
import Sidebar from '../components/Sidebar.vue'
import { useWorkspaceStore } from '@/stores/workspaceStore'
import { useTaskStore } from '@/stores/taskStore'

const workspaceStore = useWorkspaceStore()
const taskStore = useTaskStore()
const router = useRouter()

// State untuk UI & Filter
const isModalOpen = ref(false)
const activeMenuTaskId = ref<number | null>(null)
const selectedPriorityFilter = ref<string>('All')

// State Form Tambah Tugas Baru
const titleInput = ref('')
const descInput = ref('')
const priorityInput = ref('Normal')
const categoryInput = ref('Dev')
const dueDateInput = ref('')
const isSubmitting = ref(false)

onMounted(async () => {
    // Ambil data workspace aktif jika belum ada di store
    const activeWsId = localStorage.getItem('activeWorkspaceId')
    if (activeWsId && !workspaceStore.currentWorkspace) {
        // Pastikan list workspace ter-fetch terlebih dahulu
        await workspaceStore.fetchWorkspace()
        const foundWs = workspaceStore.workspaces.find(w => (w.id || w.Id) === parseInt(activeWsId))
        if (foundWs) {
            workspaceStore.setActiveWorkspace(foundWs, false) // false agar tidak fetch ulang task di sini
        }
    }

    // Tarik data board dan tugas berdasarkan workspace aktif
    if (workspaceStore.currentWorkspace) {
        const wsId = workspaceStore.currentWorkspace.id || workspaceStore.currentWorkspace.Id
        await taskStore.fetchBoardsAndTasks(wsId)
    }
})

// Menggabungkan semua tugas dari berbagai board menjadi satu array flat untuk tabel
const allTasks = computed(() => {
    const tasksArray: any[] = []
    if (!taskStore.boards) return tasksArray

    taskStore.boards.forEach((board: any) => {
        const boardId = board.id || board.Id
        const boardTasks = taskStore.tasksByBoard[boardId] || []

        boardTasks.forEach((task: any) => {
            tasksArray.push({
                ...task,
                boardTitle: board.Title || board.title || 'Kolom'
            })
        })
    })
    return tasksArray
})

// Menghitung metrik ringkasan status di bagian atas
const metrics = computed(() => {
    const total = allTasks.value.length
    const inProgress = allTasks.value.filter(t => t.boardTitle.toLowerCase() === 'in progress').length
    const pending = allTasks.value.filter(t => t.boardTitle.toLowerCase() === 'to do').length
    const done = allTasks.value.filter(t => t.boardTitle.toLowerCase() === 'done').length

    return { total, inProgress, pending, done }
})

// Memfilter tugas berdasarkan pilihan user
const filteredTasks = computed(() => {
    if (selectedPriorityFilter.value === 'All') return allTasks.value
    return allTasks.value.filter(t => (t.priority || t.Priority) === selectedPriorityFilter.value)
})

const toggleTaskMenu = (id: number) => {
    activeMenuTaskId.value = activeMenuTaskId.value === id ? null : id
}

// Fungsi styling penanda prioritas & kategori
const getPriorityClass = (priority: string) => {
    const styles: Record<string, string> = {
        'High': 'bg-rose-50 text-rose-600 border border-rose-100',
        'Normal': 'bg-amber-50 text-amber-600 border border-amber-100',
        'Low': 'bg-sky-50 text-sky-600 border border-sky-100'
    }
    return styles[priority] || 'bg-slate-50 text-slate-500'
}

const getCategoryStyle = (category: string) => {
    const styles: Record<string, string> = {
        'Design': 'bg-pink-50 text-pink-600 border border-pink-100',
        'Dev': 'bg-blue-50 text-blue-600 border border-blue-100',
        'Bug': 'bg-rose-50 text-rose-600 border border-rose-100',
        'Testing': 'bg-amber-50 text-amber-600 border border-amber-100',
        'Konten': 'bg-emerald-50 text-emerald-600 border border-emerald-100'
    }
    return styles[category] || 'bg-slate-50 text-slate-600'
}

const handleAddTask = async () => {
    if (!titleInput.value.trim() || !taskStore.boards?.[0]) return
    isSubmitting.value = true

    // Masukkan otomatis ke kolom pertama (To Do / default)
    const firstBoardId = taskStore.boards[0].id || taskStore.boards[0].Id

    try {
        await taskStore.createTask({
            title: titleInput.value,
            description: descInput.value,
            priority: priorityInput.value,
            category: categoryInput.value,
            dueDate: dueDateInput.value ? new Date(dueDateInput.value).toISOString() : null,
            boardId: firstBoardId
        })

        // Reset formulir modal
        titleInput.value = ''
        descInput.value = ''
        priorityInput.value = 'Normal'
        categoryInput.value = 'Dev'
        dueDateInput.value = ''
        isModalOpen.value = false

        // Tarik ulang agar data tabel sinkron sempurna
        const wsId = workspaceStore.currentWorkspace.id || workspaceStore.currentWorkspace.Id
        await taskStore.fetchBoardsAndTasks(wsId)
    } catch (err) {
        console.error(err)
    } finally {
        isSubmitting.value = false
    }
}
</script>

<template>
    <div class="flex h-screen bg-[#F8F7FC] font-['Inter'] overflow-hidden" @click="activeMenuTaskId = null">

        <Sidebar :show-new-project-button="false" />

        <div class="flex-1 flex flex-col overflow-hidden">

            <div class="px-10 py-5 bg-white border-b border-slate-50 flex justify-between items-center shrink-0">
                <div class="flex items-center gap-6">
                    <span class="text-sm font-bold text-slate-800">Project Management</span>
                    <div class="flex gap-4 text-xs font-semibold text-slate-400 border-l border-slate-200 pl-6">
                        <span class="hover:text-slate-600 cursor-pointer" @click="router.push('/workspaces')">Overview</span>
                        <span class="hover:text-slate-600 cursor-pointer"
                            @click="router.push('/dashboard')">Board</span>
                        <span class="text-violet-600 border-b-2 border-violet-600 pb-1 cursor-pointer">List</span>
                        <span class="hover:text-slate-600 cursor-pointer">Timeline</span>
                    </div>
                </div>
            </div>

            <div class="p-10 space-y-6 flex-1 overflow-y-auto">

                <div class="flex justify-between items-start">
                    <div>
                        <span class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">Workspace / {{
                            workspaceStore.currentWorkspace?.name || 'Proyek' }}</span>
                        <h2 class="text-2xl font-black text-slate-900 tracking-tight mt-0.5">Daftar Semua Tugas</h2>
                        <div class="flex items-center gap-4 text-xs text-slate-400 mt-2 font-medium">
                            <span
                                class="bg-violet-100 text-violet-700 px-2 py-0.5 rounded-full text-[10px] font-bold">{{
                                metrics.total }} Total Tugas</span>
                            <span>•</span>
                            <span class="text-emerald-600 font-semibold">{{ metrics.done }} Selesai</span>
                        </div>
                    </div>
                    <button @click="isModalOpen = true"
                        class="flex items-center gap-2 bg-violet-600 hover:bg-violet-700 text-white font-bold text-xs px-4 py-2.5 rounded-xl transition-all shadow-md shadow-violet-100">
                        <Plus class="w-4 h-4" /> Tambah Tugas
                    </button>
                </div>

                <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div
                        class="bg-white border border-slate-100 rounded-2xl p-5 shadow-sm flex justify-between items-center">
                        <div>
                            <span class="text-xs font-bold text-slate-400 block uppercase tracking-wider">Sedang
                                Berjalan</span>
                            <h3 class="text-3xl font-black text-slate-800 mt-1">{{ metrics.inProgress }}</h3>
                        </div>
                        <div class="w-10 h-10 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center">
                            <Loader2 class="w-5 h-5 animate-spin" />
                        </div>
                    </div>

                    <div
                        class="bg-white border border-slate-100 rounded-2xl p-5 shadow-sm flex justify-between items-center">
                        <div>
                            <span class="text-xs font-bold text-slate-400 block uppercase tracking-wider">Antrean
                                Kerja</span>
                            <h3 class="text-3xl font-black text-slate-800 mt-1">{{ metrics.pending }}</h3>
                        </div>
                        <div class="w-10 h-10 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center">
                            <AlertCircle class="w-5 h-5" />
                        </div>
                    </div>

                    <div
                        class="bg-white border border-slate-100 rounded-2xl p-5 shadow-sm flex justify-between items-center">
                        <div>
                            <span class="text-xs font-bold text-slate-400 block uppercase tracking-wider">Telah
                                Selesai</span>
                            <h3 class="text-3xl font-black text-slate-800 mt-1">{{ metrics.done }}</h3>
                        </div>
                        <div
                            class="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
                            <CheckCircle2 class="w-5 h-5" />
                        </div>
                    </div>
                </div>

                <div class="bg-white border border-slate-100 rounded-3xl shadow-sm overflow-hidden flex flex-col">

                    <div class="px-6 py-4 border-b border-slate-50 flex justify-between items-center bg-slate-50/30">
                        <div class="flex items-center gap-2">
                            <ListFilter class="w-4 h-4 text-slate-400" />
                            <span class="text-xs font-bold text-slate-600">Filter Prioritas:</span>
                            <select v-model="selectedPriorityFilter"
                                class="bg-white border border-slate-200 rounded-lg text-xs font-semibold px-2.5 py-1 text-slate-700 outline-none">
                                <option value="All">Semua Tingkat</option>
                                <option value="Low">Low</option>
                                <option value="Normal">Normal</option>
                                <option value="High">High</option>
                            </select>
                        </div>
                    </div>

                    <div class="overflow-x-auto">
                        <table class="w-full text-left border-collapse">
                            <thead>
                                <tr
                                    class="border-b border-slate-100 text-[11px] font-extrabold text-slate-400 uppercase tracking-wider bg-slate-50/20">
                                    <th class="py-4 px-6">Nama Tugas</th>
                                    <th class="py-4 px-4">Kategori</th>
                                    <th class="py-4 px-4">Status Alur</th>
                                    <th class="py-4 px-4">Tenggat Waktu</th>
                                    <th class="py-4 px-4">Prioritas</th>
                                    <th class="py-4 px-6 text-right">Aksi</th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-slate-50 text-xs font-medium text-slate-700">
                                <tr v-for="task in filteredTasks" :key="task.id || task.Id"
                                    class="hover:bg-slate-50/50 transition-all group">

                                    <td class="py-4 px-6 min-w-[240px]">
                                        <div
                                            class="font-bold text-slate-800 line-clamp-1 group-hover:text-violet-600 transition-colors">
                                            {{ task.title || task.Title }}
                                        </div>
                                        <div class="text-[11px] text-slate-400 line-clamp-1 font-medium mt-0.5">
                                            {{ task.description || task.Description || 'Tidak ada deskripsi singkat.' }}
                                        </div>
                                    </td>

                                    <td class="py-4 px-4">
                                        <span
                                            :class="['text-[10px] font-extrabold px-2.5 py-0.5 rounded-md uppercase tracking-wide', getCategoryStyle(task.category || task.Category)]">
                                            {{ task.category || task.Category || 'Dev' }}
                                        </span>
                                    </td>

                                    <td class="py-4 px-4">
                                        <span
                                            class="bg-violet-50 text-violet-700 px-2.5 py-0.5 rounded-full font-bold text-[10px]">
                                            {{ task.boardTitle }}
                                        </span>
                                    </td>

                                    <td class="py-4 px-4 text-slate-500 font-semibold">
                                        <div class="flex items-center gap-1.5">
                                            <Calendar class="w-3.5 h-3.5 text-slate-300" />
                                            <span>
                                                {{ task.dueDate || task.DueDate ? new Date(task.dueDate ||
                                                    task.DueDate).toLocaleDateString('id-ID', {
                                                        day: 'numeric', month:
                                                'short', year: 'numeric' }) : '-' }}
                                            </span>
                                        </div>
                                    </td>

                                    <td class="py-4 px-4">
                                        <span
                                            :class="['text-[10px] font-bold px-2 py-0.5 rounded-md uppercase', getPriorityClass(task.priority || task.Priority)]">
                                            {{ task.priority || task.Priority }}
                                        </span>
                                    </td>

                                    <td class="py-4 px-6 text-right relative">
                                        <button @click.stop="toggleTaskMenu(task.id || task.Id)"
                                            class="p-1 text-slate-400 hover:text-slate-700 hover:bg-slate-100 rounded-md transition-all inline-block">
                                            <MoreVertical class="w-4 h-4" />
                                        </button>

                                        <div v-if="activeMenuTaskId === (task.id || task.Id)"
                                            class="absolute right-6 top-12 w-28 bg-white border border-slate-100 rounded-xl shadow-xl z-30 py-1 overflow-hidden text-left">
                                            <button
                                                class="w-full px-3 py-2 text-[11px] font-bold text-slate-600 hover:bg-slate-50 block">
                                                Detail Tugas
                                            </button>
                                        </div>
                                    </td>

                                </tr>

                                <tr v-if="filteredTasks.length === 0">
                                    <td colspan="6" class="text-center py-12 text-slate-400 font-semibold">
                                        Tidak ada tugas yang terdaftar dengan kriteria ini.
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div
                        class="px-6 py-4 border-t border-slate-50 bg-slate-50/20 flex justify-between items-center shrink-0">
                        <span class="text-[11px] text-slate-400 font-bold">Menampilkan {{ filteredTasks.length }}
                            Tugas</span>
                        <div class="flex gap-1.5">
                            <button
                                class="p-1.5 border border-slate-200 rounded-lg bg-white text-slate-400 hover:bg-slate-50 transition-all">
                                <ChevronLeft class="w-3.5 h-3.5" />
                            </button>
                            <button
                                class="p-1.5 border border-slate-200 rounded-lg bg-white text-slate-400 hover:bg-slate-50 transition-all">
                                <ChevronRight class="w-3.5 h-3.5" />
                            </button>
                        </div>
                    </div>

                </div>

            </div>
        </div>

        <div v-if="isModalOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4">
            <div class="absolute inset-0 bg-slate-900/30 backdrop-blur-sm" @click="isModalOpen = false"></div>
            <div class="relative bg-white w-full max-w-md rounded-[24px] p-6 z-10 shadow-xl space-y-4">
                <h3 class="text-base font-bold text-slate-900">Tambah Tugas Baru</h3>
                <form @submit.prevent="handleAddTask" class="space-y-3">
                    <input v-model="titleInput" type="text" placeholder="Judul Tugas" required
                        class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-xl py-2.5 px-4 text-xs focus:ring-2 focus:ring-violet-500 outline-none" />

                    <textarea v-model="descInput" placeholder="Deskripsi Singkat" rows="3"
                        class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-xl py-2.5 px-4 text-xs focus:ring-2 focus:ring-violet-500 outline-none resize-none"></textarea>

                    <div class="grid grid-cols-2 gap-3">
                        <div>
                            <label
                                class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block mb-1">Kategori</label>
                            <select v-model="categoryInput"
                                class="w-full bg-slate-50 ring-1 ring-slate-200 rounded-xl py-2 px-3 text-xs outline-none font-bold text-slate-700">
                                <option value="Dev">Dev</option>
                                <option value="Design">Design</option>
                                <option value="Bug">Bug</option>
                                <option value="Testing">Testing</option>
                                <option value="Konten">Konten</option>
                            </select>
                        </div>
                        <div>
                            <label
                                class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block mb-1">Prioritas</label>
                            <select v-model="priorityInput"
                                class="w-full bg-slate-50 ring-1 ring-slate-200 rounded-xl py-2 px-3 text-xs outline-none font-bold text-slate-700">
                                <option value="Low">Low</option>
                                <option value="Normal">Normal</option>
                                <option value="High">High</option>
                            </select>
                        </div>
                    </div>

                    <div>
                        <label
                            class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block mb-1">Deadline</label>
                        <input v-model="dueDateInput" type="date"
                            class="w-full bg-slate-50 ring-1 ring-slate-200 rounded-xl py-2 px-3 text-xs outline-none font-bold text-slate-700" />
                    </div>

                    <div class="flex gap-2 pt-2">
                        <button type="button" @click="isModalOpen = false"
                            class="flex-1 py-2 rounded-xl text-xs font-bold text-slate-500 bg-slate-100">Batal</button>
                        <button type="submit" :disabled="isSubmitting"
                            class="flex-1 py-2 rounded-xl text-xs font-bold text-white bg-violet-600 flex justify-center items-center shadow-md">
                            <Loader2 v-if="isSubmitting" class="w-4 h-4 animate-spin mr-1" /> Simpan Tugas
                        </button>
                    </div>
                </form>
            </div>
        </div>

    </div>
</template>