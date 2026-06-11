<script setup lang="ts">
import {
    X, Share2, MoreVertical, ChevronDown, Calendar,
    Flag, FileText, Paperclip, MessageSquare
} from 'lucide-vue-next'

// TERIMA PROPS OBJECT TASK DARI DASHBOARDVIEW
const props = defineProps({
    isOpen: {
        type: Boolean,
        required: true
    },
    task: {
        type: Object,
        default: () => null
    }
})

defineEmits(['close'])

// Fungsi pembantu format tanggal standar industri untuk deadline
const formatDate = (dateString: string) => {
    if (!dateString) return 'Tanpa Tenggat Waktu'
    return new Date(dateString).toLocaleDateString('id-ID', {
        day: 'numeric',
        month: 'long',
        year: 'numeric'
    })
}
</script>

<template>
    <div v-if="isOpen" class="fixed inset-0 z-50 flex justify-end">
        <div class="absolute inset-0 bg-slate-900/20 backdrop-blur-sm" @click="$emit('close')"></div>

        <div class="relative bg-white w-full max-w-3xl h-full shadow-2xl flex flex-col font-['Inter'] animate-slide-in">

            <header class="px-8 py-5 border-b border-slate-100 flex justify-between items-center shrink-0">
                <div class="flex items-center gap-2 text-xs font-bold text-slate-400 tracking-wider uppercase">
                    <span>Detail Tugas</span>
                    <span class="text-slate-300">/</span>
                    <span>ID: #{{ task?.Id || task?.id }}</span>
                </div>

                <div class="flex items-center gap-2">
                    <button @click="$emit('close')"
                        class="p-2 text-slate-400 hover:text-rose-600 rounded-xl hover:bg-rose-50 transition-all">
                        <X class="w-5 h-5" />
                    </button>
                </div>
            </header>

            <div class="flex-1 flex overflow-hidden">

                <div class="flex-1 overflow-y-auto p-8 space-y-8 custom-scrollbar">

                    <div>
                        <h2 class="text-2xl font-extrabold text-slate-900 tracking-tight leading-tight">
                            {{ task?.Title || task?.title || 'Tugas Tanpa Judul' }}
                        </h2>
                    </div>

                    <div class="space-y-3">
                        <h4 class="text-sm font-bold text-slate-800 flex items-center gap-2">
                            <FileText class="w-4 h-4 text-slate-400" /> Deskripsi Detail
                        </h4>
                        <div
                            class="bg-slate-50/50 border border-slate-100/80 rounded-2xl p-5 text-sm text-slate-600 leading-relaxed font-medium">
                            <p class="whitespace-pre-wrap">{{ task?.Description || task?.description || 'Tidak ada deskripsi detail untuk tugas ini.' }}</p>
                        </div>
                    </div>

                </div>

                <div class="w-72 border-l border-slate-100 bg-slate-50/40 p-6 space-y-6 shrink-0 h-full">

                    <div class="space-y-1.5">
                        <label class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Status
                            Progres</label>
                        <div
                            class="bg-white border border-slate-100 rounded-xl px-4 py-2.5 text-xs font-bold text-violet-600 shadow-sm flex justify-between items-center">
                            <span>Penyelesaian</span>
                            <span>{{ task?.Progress ?? task?.progress ?? 0 }}%</span>
                        </div>
                    </div>

                    <div class="space-y-1.5">
                        <label class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Tenggat
                            Waktu</label>
                        <div
                            class="flex items-center gap-3 bg-white border border-slate-100 rounded-xl px-4 py-2.5 text-xs font-semibold text-slate-700 shadow-sm">
                            <Calendar class="w-4 h-4 text-slate-400" />
                            <span>{{ formatDate(task?.DueDate || task?.dueDate) }}</span>
                        </div>
                    </div>

                    <div class="space-y-1.5">
                        <label class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Tingkat
                            Prioritas</label>
                        <div
                            class="flex items-center justify-between bg-white border border-slate-100 rounded-xl px-4 py-2.5 shadow-sm">
                            <div class="flex items-center gap-2 text-xs font-semibold text-slate-700">
                                <Flag class="w-4 h-4 text-slate-400" />
                                <span>Prioritas</span>
                            </div>
                            <span :class="['text-[9px] font-extrabold px-3 py-1 rounded-full uppercase tracking-wider',
                                (task?.Priority || task?.priority) === 'High' ? 'bg-rose-50 text-rose-600' : (task?.Priority || task?.priority) === 'Normal' ? 'bg-amber-50 text-amber-600' : 'bg-slate-100 text-slate-600']">
                                {{ task?.Priority || task?.priority || 'Normal' }}
                            </span>
                        </div>
                    </div>

                </div>

            </div>

        </div>
    </div>
</template>

<style scoped>
/* Animasi Slide-In Drawer Elegan */
.animate-slide-in {
    animation: slideIn 0.35s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

@keyframes slideIn {
    from {
        transform: translateX(100%);
    }

    to {
        transform: translateX(0);
    }
}

/* Kustom Scrollbar agar Minimalis */
.custom-scrollbar::-webkit-scrollbar {
    width: 5px;
}

.custom-scrollbar::-webkit-scrollbar-track {
    background: transparent;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
    background: #E2E8F0;
    border-radius: 99px;
}
</style>