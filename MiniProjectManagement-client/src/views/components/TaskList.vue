<script setup lang="ts">
import { Plus, MessageSquare, Paperclip, MoreHorizontal, Calendar, CheckSquare, Loader2 } from 'lucide-vue-next'
import { ref } from 'vue'
import { useTaskStore } from '@/stores/taskStore'

const emit = defineEmits(['select-task'])
const taskStore = useTaskStore()


const props = defineProps({
    boardId: {
        type: Number,
        required: true
    },
    title: {
        type: String,
        required: true
    },
    tasks: {
        type: Array as () => any[],
        default: () => []
    }
})


// State form modal
const isModalOpen = ref(false)
const titleInput = ref('')
const descInput = ref('')
const priorityInput = ref('Normal')
const dueDateInput = ref('')
const isSubmitting = ref(false)
const categoryInput = ref('Dev')

const handleAddTask = async () => {
    if (!titleInput.value.trim()) return
    isSubmitting.value = true
    try {
        await taskStore.createTask({
            title: titleInput.value,
            description: descInput.value,
            priority: priorityInput.value,
            category: categoryInput.value,
            dueDate: dueDateInput.value ? new Date(dueDateInput.value).toISOString() : null,
            boardId: props.boardId
        })
        // Reset Form
        titleInput.value = ''
        descInput.value = ''
        categoryInput.value = 'Dev'
        priorityInput.value = 'Normal'
        dueDateInput.value = ''
        isModalOpen.value = false
    } catch (err) { console.error(err) } finally { isSubmitting.value = false }
}

// Fungsi pembantu warna badge prioritas dinamis dari .NET string
const getPriorityClass = (priority: string) => {
    const styles: Record<string, string> = {
        'High': 'bg-red-50 text-red-600 border border-red-100',
        'Normal': 'bg-amber-50 text-amber-600 border border-amber-100',
        'Low': 'bg-sky-50 text-sky-600 border border-sky-100'
    };
    return styles[priority] || 'bg-slate-50 text-slate-500 border border-slate-100';
}

const onDragStart = (event: DragEvent, task: any, fromBoardId: number) => {
    if (event.dataTransfer) {
        event.dataTransfer.effectAllowed = 'move';
        // Simpan ID tugas dan ID kolom asal ke dalam transfer data browser
        event.dataTransfer.setData('taskId', (task.id || task.Id).toString());
        event.dataTransfer.setData('fromBoardId', fromBoardId.toString());
    }
}

const onDrop = async (event: DragEvent, toBoardId: number) => {
    if (event.dataTransfer) {
        const taskId = parseInt(event.dataTransfer.getData('taskId'));
        const fromBoardId = parseInt(event.dataTransfer.getData('fromBoardId'));

        // Jika digeser ke kolom yang sama, abaikan
        if (fromBoardId === toBoardId) return;

        // Panggil fungsi store untuk eksekusi perpindahan instan
        await taskStore.moveTask(taskId, fromBoardId, toBoardId);
    }
}

const getCategoryStyle = (category: string) => {
    const styles: Record<string, string> = {
        'Design': 'bg-pink-100 text-pink-700',
        'Dev': 'bg-blue-100 text-blue-700',
        'Bug': 'bg-rose-100 text-rose-700',
        'Testing': 'bg-amber-100 text-amber-700',
        'Konten': 'bg-emerald-100 text-emerald-700'
    };
    return styles[category] || 'bg-slate-100 text-slate-600';
};
</script>
<template>
    <div class="w-80 flex flex-col shrink-0 max-h-full">
        <div class="flex justify-between items-center mb-4 px-1">
            <div class="flex items-center gap-2">
                <h3 class="text-sm font-bold text-slate-800 tracking-tight">{{ title }}</h3>
                <span class="text-[11px] font-bold px-2 py-0.5 rounded-full bg-violet-100 text-violet-700">
                    {{ tasks.length }}
                </span>
            </div>
            <button class="p-1 text-slate-400 rounded-lg hover:bg-white transition-all">
                <MoreHorizontal class="w-4 h-4" />
            </button>
        </div>

        <div class="space-y-3.5 overflow-y-auto pr-1 flex-1 pb-4" @dragover.prevent @drop="onDrop($event, boardId)">

            <template v-if="tasks && tasks.length > 0">
                <div v-for="task in tasks" :key="task.id" @click="emit('select-task', task)" draggable="true"
                    @dragstart="onDragStart($event, task, boardId)"
                    class="bg-white border border-slate-100/80 rounded-[20px] p-5 shadow-sm hover:shadow-md transition-all cursor-pointer group active:scale-95 duration-150">

                    <div class="flex items-center gap-2 mb-3">
                        <span
                            :class="['text-[9px] font-extrabold uppercase tracking-wider px-2 py-0.5 rounded-md', getCategoryStyle(task.category)]">
                            {{ task.category || 'General' }}
                        </span>
                        <span
                            :class="['text-[9px] font-bold px-2 py-0.5 rounded-md uppercase tracking-wider', getPriorityClass(task.priority)]">
                            {{ task.priority }}
                        </span>
                    </div>

                    <h4
                        class="text-sm font-bold text-slate-800 line-clamp-1 mb-1 group-hover:text-violet-600 transition-colors">
                        {{ task.title }}
                    </h4>
                    <p class="text-xs text-slate-400 font-medium line-clamp-2 mb-4 leading-relaxed">
                        {{ task.description || 'No description.' }}
                    </p>
                </div>
            </template>

            <template v-else>
                <div
                    class="bg-slate-50/40 border border-dashed border-slate-200/60 rounded-[20px] py-8 px-4 flex flex-col items-center justify-center text-center">
                    <div class="p-2.5 bg-white rounded-xl shadow-sm text-slate-300 mb-2">
                        <CheckSquare class="w-5 h-5" />
                    </div>
                    <p class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">Belum ada tugas</p>
                    <p class="text-[10px] text-slate-400 font-medium mt-0.5 max-w-[180px]">Klik tombol di bawah untuk
                        memulai tugas pertama di kolom ini.</p>
                </div>
            </template>

            <button @click="isModalOpen = true"
                class="w-full border border-dashed border-slate-200 hover:border-violet-300 text-slate-400 hover:text-violet-600 bg-white/50 hover:bg-white rounded-[16px] py-3 text-xs font-bold flex items-center justify-center gap-2 transition-all mt-2">
                <Plus class="w-4 h-4" /> Add New Task
            </button>
        </div>

        <div v-if="isModalOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4">
            <div class="absolute inset-0 bg-slate-900/30 backdrop-blur-sm" @click="isModalOpen = false"></div>
            <div class="relative bg-white w-full max-w-md rounded-[24px] p-6 z-10 shadow-xl space-y-4">
                <h3 class="text-base font-bold text-slate-900">Tambah Tugas Baru ke "{{ title }}"</h3>
                <form @submit.prevent="handleAddTask" class="space-y-3">
                    <input v-model="titleInput" type="text" placeholder="Judul Tugas" required
                        class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-xl py-2.5 px-4 text-xs focus:ring-2 focus:ring-violet-500 outline-none" />

                    <textarea v-model="descInput" placeholder="Deskripsi Singkat" rows="3"
                        class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-xl py-2.5 px-4 text-xs focus:ring-2 focus:ring-violet-500 outline-none resize-none"></textarea>

                    <div class="grid grid-cols-2 gap-3">
                        <div>
                            <label
                                class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block mb-1">Prioritas</label>
                            <select v-model="priorityInput"
                                class="w-full bg-slate-50 ring-1 ring-slate-200 rounded-xl py-2 px-3 text-xs outline-none">
                                <option value="Low">Low</option>
                                <option value="Normal">Normal</option>
                                <option value="High">High</option>
                            </select>
                        </div>
                        <div>
                            <label
                                class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block mb-1">Deadline</label>
                            <input v-model="dueDateInput" type="date"
                                class="w-full bg-slate-50 ring-1 ring-slate-200 rounded-xl py-2 px-3 text-xs outline-none" />
                        </div>
                    </div>
                    <div>
                        <label
                            class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block mb-1">Kategori</label>
                        <select v-model="categoryInput"
                            class="w-full bg-slate-50 ring-1 ring-slate-200 rounded-xl py-2 px-3 text-xs outline-none">
                            <option value="Dev">Dev</option>
                            <option value="Design">Design</option>
                            <option value="Bug">Bug</option>
                            <option value="Testing">Testing</option>
                            <option value="Konten">Konten</option>
                        </select>
                    </div>

                    <div class="flex gap-2 pt-2">
                        <button type="button" @click="isModalOpen = false"
                            class="flex-1 py-2 rounded-xl text-xs font-bold text-slate-500 bg-slate-100">Batal</button>
                        <button type="submit" :disabled="isSubmitting"
                            class="flex-1 py-2 rounded-xl text-xs font-bold text-white bg-violet-600 flex justify-center items-center">
                            <Loader2 v-if="isSubmitting" class="w-4 h-4 animate-spin mr-1" /> Simpan Tugas
                        </button>
                    </div>
                </form>
            </div>
        </div>

    </div>
</template>
