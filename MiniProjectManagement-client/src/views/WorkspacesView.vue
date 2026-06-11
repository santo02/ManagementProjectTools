<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { FolderClosed, Plus, ArrowRight, MoreVertical, Clock, Loader2 } from 'lucide-vue-next'
import Sidebar from './components/Sidebar.vue' // Impor komponen Sidebar Anda
import { useWorkspaceStore } from '@/stores/workspaceStore'
import { useAuthStore } from '@/stores/authStore'

const workspaceStore = useWorkspaceStore()
const authStore = useAuthStore()
const router = useRouter()

const isCreateModalOpen = ref(false)
const newWorkspaceTitle = ref('')
const isSubmitting = ref(false)

onMounted(async () => {
  await workspaceStore.fetchWorkspace()
})

const handleSelectWorkspace = (workspace: any) => {

  console.log("Memilih workspace:", workspace) // Debug: Pastikan data workspace benar
  // 1. Simpan ID Workspace aktif ke localStorage (Gunakan huruf kecil 'i' pada Id jika dari backend berupa id)
  const wsId = workspace.id || workspace.Id
  localStorage.setItem('activeWorkspaceId', wsId.toString())

  // 2. Set active workspace di Pinia Store agar state-nya sinkron
  const currentUserId = authStore.user?.Id
  workspaceStore.setActiveWorkspace(workspace, currentUserId)
  console.log("Active workspace di store:", workspaceStore.currentWorkspace) // Debug: Pastikan workspace tersimpan di storeTask

  // 3. Pindah ke halaman dashboard papan Kanban
  router.push('/dashboard')
}

const handleCreateWorkspace = async () => {
  if (!newWorkspaceTitle.value.trim()) return
  isSubmitting.value = true
  try {
    await workspaceStore.createWorkspace(newWorkspaceTitle.value)
    newWorkspaceTitle.value = ''
    isCreateModalOpen.value = false
    await workspaceStore.fetchWorkspace()
  } catch (error) {
    console.error(error)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="flex h-screen bg-[#F8F7FC] font-['Inter'] overflow-hidden">

    <Sidebar :show-new-project-button="true" @open-create-modal="isCreateModalOpen = true" />

    <div class="flex-1 flex flex-col overflow-y-auto">
      <div class="px-10 py-5 bg-white border-b border-slate-50 flex justify-between items-center shrink-0">
        <div class="flex items-center gap-6">
          <span class="text-sm font-bold text-slate-800">Project Management</span>
          <div class="flex gap-4 text-xs font-semibold text-slate-400 border-l border-slate-200 pl-6">
            <span class="text-violet-600 border-b-2 border-violet-600 pb-1 cursor-pointer">Overview</span>
            <span class="hover:text-slate-600 cursor-pointer" @click="router.push('/dashboard')">Board</span>
            <span class="hover:text-slate-600 cursor-pointer">List</span>
            <span class="hover:text-slate-600 cursor-pointer">Timeline</span>
          </div>
        </div>
        <div class="w-72">
          <input type="text" placeholder="Cari project..."
            class="w-full bg-slate-50 border-none rounded-xl py-2 px-4 text-xs outline-none ring-1 ring-slate-100" />
        </div>
      </div>

      <div class="p-10 space-y-8 flex-1">
        <div class="flex justify-between items-center">
          <div>
            <h2 class="text-2xl font-extrabold text-slate-900 tracking-tight">Pilih Workspace</h2>
            <p class="text-xs text-slate-400 mt-1 font-medium">Selamat datang kembali! Lanjutkan pekerjaan Anda di
              project yang tersedia.</p>
          </div>
          <div class="flex -space-x-2 overflow-hidden">
            <img class="inline-block h-8 w-8 rounded-full ring-2 ring-white object-cover"
              src="https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=100" alt="" />
            <img class="inline-block h-8 w-8 rounded-full ring-2 ring-white object-cover"
              src="https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=100" alt="" />
            <div
              class="inline-block h-8 w-8 rounded-full bg-violet-100 text-violet-600 text-xs font-bold flex items-center justify-center ring-2 ring-white">
              +5</div>
          </div>
        </div>

        <div v-if="workspaceStore.isLoading" class="flex flex-col items-center justify-center py-12 text-slate-400">
          <Loader2 class="w-8 h-8 animate-spin text-violet-600 mb-2" />
        </div>

        <div v-else class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div v-for="(ws, index) in workspaceStore.workspaces" :key="ws.id"
            class="bg-white border border-slate-100 rounded-[24px] p-6 shadow-sm flex flex-col justify-between h-64 relative group">
            <div>
              <div class="flex justify-between items-start mb-4">
                <div class="p-3 bg-violet-50 text-violet-600 rounded-xl">
                  <FolderClosed class="w-6 h-6" />
                </div>
                <span
                  :class="['text-[10px] font-bold px-3 py-1 rounded-full uppercase tracking-wider', index === 0 ? 'bg-orange-50 text-orange-600' : 'bg-slate-100 text-slate-600']">
                  {{ index === 0 ? 'Urgent' : 'In Progress' }}
                </span>
              </div>
              <h3 class="text-lg font-bold text-slate-900 line-clamp-1 mb-1">{{ ws.name }}</h3>
              <div class="flex items-center gap-1.5 text-xs font-medium text-slate-400">
                <Clock class="w-3.5 h-3.5" />
                <span>Leader ID: <strong class="text-slate-600">{{ ws.leaderId }}</strong></span>
              </div>
            </div>

            <div class="space-y-1.5 my-2">
              <div class="flex justify-between text-xs font-bold"><span
                  class="text-slate-400 text-[10px]">Progres</span><span class="text-violet-600">65%</span></div>
              <div class="w-full bg-slate-100 h-2 rounded-full overflow-hidden">
                <div class="bg-violet-600 h-full w-[65%] rounded-full"></div>
              </div>
            </div>

            <div class="border-t border-slate-50 pt-4 flex justify-between items-center">
              <div><span class="block text-[10px] uppercase font-bold text-slate-300">Deadline</span><span
                  class="text-xs font-semibold text-slate-600">12 Okt 2026</span></div>
              <button @click="handleSelectWorkspace(ws)"
                class="flex items-center gap-1.5 text-xs font-bold text-violet-600 hover:text-violet-700">
                <span>Buka Project</span>
                <ArrowRight class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div @click="isCreateModalOpen = true"
            class="bg-white border-2 border-dashed border-slate-200 rounded-[24px] p-6 flex flex-col items-center justify-center text-center cursor-pointer hover:border-violet-400 transition-all h-44">
            <Plus class="w-6 h-6 text-slate-400 mb-2" />
            <h4 class="text-sm font-bold text-slate-800">Buat Project Baru</h4>
          </div>

          <div
            class="lg:col-span-2 bg-white border border-slate-100 rounded-[24px] p-6 shadow-sm flex flex-col justify-between h-44">
            <div class="flex justify-between items-center">
              <h4 class="text-sm font-bold text-slate-800">Ringkasan Aktivitas</h4>
            </div>
            <div class="grid grid-cols-3 gap-4 items-end">
              <div><span class="text-[10px] font-bold text-slate-400 block">Total Task</span>
                <h3 class="text-2xl font-black text-slate-800">42</h3>
              </div>
              <div class="border-l border-slate-100 pl-4"><span class="text-[10px] font-bold text-slate-400 block">Waktu
                  Kerja</span>
                <h3 class="text-2xl font-black text-slate-800">156j</h3>
              </div>
              <div class="border-l border-slate-100 pl-4"><span
                  class="text-[10px] font-bold text-slate-400 block">Efisiensi</span>
                <h3 class="text-2xl font-black text-slate-800">94%</h3>
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>

    <div v-if="isCreateModalOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-slate-900/30 backdrop-blur-sm" @click="isCreateModalOpen = false"></div>
      <div class="relative bg-white w-full max-w-sm rounded-[24px] p-6 z-10 shadow-xl">
        <h3 class="text-lg font-bold text-slate-900 mb-4">Buat Project Baru</h3>
        <form @submit.prevent="handleCreateWorkspace" class="space-y-4">
          <input v-model="newWorkspaceTitle" type="text" required
            class="w-full bg-slate-50 border-none ring-1 ring-slate-200 rounded-xl py-3 px-4 text-sm"
            placeholder="Nama Project" />
          <div class="flex gap-2">
            <button type="button" @click="isCreateModalOpen = false"
              class="flex-1 py-2.5 rounded-xl text-xs bg-slate-100">Batal</button>
            <button type="submit" :disabled="isSubmitting"
              class="flex-1 py-2.5 rounded-xl text-xs bg-violet-600 text-white">Buat</button>
          </div>
        </form>
      </div>
    </div>

  </div>
</template>