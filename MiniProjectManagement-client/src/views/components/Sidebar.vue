<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import { useWorkspaceStore } from '@/stores/workspaceStore'
import {
    LayoutDashboard, FolderClosed, CheckSquare, Calendar, Settings, Plus
} from 'lucide-vue-next'

const authStore = useAuthStore()
const workspaceStore = useWorkspaceStore()

defineProps({
    showNewProjectButton: {
        type: Boolean,
        default: false
    }
})

defineEmits(['open-create-modal'])
</script>
<template>
    <aside class="w-64 bg-white border-r border-slate-100 flex flex-col justify-between shrink-0 h-screen">
        <div class="p-6">
            <div class="mb-8">
                <h1 class="text-2xl font-black text-violet-600 tracking-tight">Workspace</h1>
                <p class="text-xs text-slate-400 font-medium">Manage your project</p>
            </div>
            <nav class="space-y-2">
                <RouterLink to="/workspaces"
                    class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-50 font-medium rounded-2xl transition-all"
                    active-class="bg-violet-50 text-violet-600 font-semibold">
                    <LayoutDashboard class="w-5 h-5" />
                    <span class="text-sm">Dashboard</span>
                </RouterLink>
                <RouterLink to="/dashboard" class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-50
                font-medium rounded-2xl transition-all" active-class="bg-violet-50 text-violet-600 font-semibold">
                    <FolderClosed class="w-5 h-5" />
                    <span class="text-sm">Projects</span>
                </RouterLink>
                <a href="#"
                    class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-50 font-medium rounded-2xl transition-all">
                    <CheckSquare class="w-5 h-5" />
                    <span class="text-sm">Tasks</span>
                </a>
                <a href="#"
                    class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-50 font-medium rounded-2xl transition-all">
                    <Calendar class="w-5 h-5" />
                    <span class="text-sm">Calendar</span>
                </a>
                <a href="#"
                    class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-50 font-medium rounded-2xl transition-all">
                    <Settings class="w-5 h-5" />
                    <span class="text-sm">Settings</span>
                </a>
            </nav>
        </div>

        <div class="p-4 border-t border-slate-50 space-y-4">
            <button v-if="showNewProjectButton" @click="$emit('open-create-modal')"
                class="w-full flex items-center justify-center gap-2 bg-violet-600 hover:bg-violet-700 text-white font-semibold py-3 px-4 rounded-xl transition-all shadow-md shadow-violet-100">
                <Plus class="w-4 h-4" /> New Project
            </button>

            <div class="flex items-center gap-3 px-2 py-2">
                <img src="https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=100&auto=format&fit=crop"
                    class="w-10 h-10 rounded-full object-cover border-2 border-slate-100" alt="Avatar" />
                <div>
                    <h4 class="text-sm font-bold text-slate-800">{{ authStore.user?.Username || 'Alex Rivera' }}</h4>
                    <p class="text-[11px] font-medium text-slate-400 uppercase tracking-wider">Administrator</p>
                </div>
            </div>
        </div>

    </aside>

</template>