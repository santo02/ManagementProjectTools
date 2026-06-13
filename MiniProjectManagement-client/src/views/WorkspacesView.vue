<script setup lang="ts">
import { onMounted, ref, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { FolderClosed, Plus, ArrowRight, MoreVertical, Clock, Loader2, Pencil, Trash2, Check, X } from 'lucide-vue-next'
import Sidebar from './components/Sidebar.vue' // Impor komponen Sidebar Anda
import { useWorkspaceStore } from '@/stores/workspaceStore'
import { useAuthStore } from '@/stores/authStore'

const workspaceStore = useWorkspaceStore()
const authStore = useAuthStore()
const router = useRouter()

const isCreateModalOpen = ref(false)
const newWorkspaceTitle = ref('')
const isSubmitting = ref(false)
const editingWorkspaceId = ref<number | null>(null)
const editNameInput = ref('')
const activeMenuId = ref<number | null>(null)

const toggleMenu = (id: number) => {
  activeMenuId.value = activeMenuId.value === id ? null : id
}
const closeAllMenus = () => {
  activeMenuId.value = null
}

onMounted(async () => {
  window.addEventListener('click', closeAllMenus)
  await workspaceStore.fetchWorkspace()
})
onUnmounted(() => {
  window.removeEventListener('click', closeAllMenus) // Bersihkan event listener
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

const startEdit = (workspace: any) => {
  editingWorkspaceId.value = workspace.id || workspace.Id
  editNameInput.value = workspace.name || workspace.Name
}

const cancelEdit = () => {
  editingWorkspaceId.value = null
  editNameInput.value = ''
}
const handleUpdateWorkspace = async (id: number) => {
  if (!editNameInput.value.trim()) return
  try {
    await workspaceStore.updateWorkspace(id, editNameInput.value.trim())
    editingWorkspaceId.value = null
  } catch (err) {
    alert("Gagal mengubah nama project")
  }
}

const handleDeleteWorkspace = async (id: number, name: string) => {
  // Standar industri: Berikan konfirmasi pop-up asli browser demi keamanan data
  if (confirm(`Apakah Anda yakin ingin menghapus project "${name}" secara permanen?\nSemua data kolom dan tugas di dalamnya akan ikut terhapus.`)) {
    try {
      await workspaceStore.deleteWorkspace(id)
    } catch (err) {
      alert("Gagal menghapus project.")
    }
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
            <span class="text-violet-600 border-b-2 border-violet-600 pb-1 cursor-pointer"  @click="router.push('/workspaces')">Overview</span>
            <span class="hover:text-slate-600 cursor-pointer" @click="router.push('/dashboard')">Board</span>
            <span class="hover:text-slate-600 cursor-pointer"  @click="router.push('/list')">List</span>
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
          <div v-for="ws in workspaceStore.workspaces" :key="ws.id || ws.Id"
            class="bg-white border border-slate-100 rounded-[24px] p-6 shadow-sm hover:shadow-md transition-all relative flex flex-col justify-between min-h-[180px]">

            <div class="flex justify-between items-start gap-4 mb-4">

              <div class="flex gap-3 w-full min-w-0">
                <div
                  class="w-12 h-12 rounded-xl bg-violet-50 text-violet-600 flex items-center justify-center shrink-0">
                  <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z" />
                  </svg>
                </div>

                <div class="flex-1 min-w-0">
                  <div v-if="editingWorkspaceId === (ws.id || ws.Id)" class="flex items-center gap-1.5 w-full">
                    <input v-model="editNameInput" @click.stop @keyup.enter="handleUpdateWorkspace(ws.id || ws.Id)"
                      type="text"
                      class="w-full bg-slate-50 border border-slate-200 rounded-lg px-2.5 py-1.5 text-xs font-bold text-slate-800 outline-none ring-2 ring-violet-500"
                      autofocus />
                    <button @click.stop="handleUpdateWorkspace(ws.id || ws.Id)"
                      class="p-1.5 bg-violet-600 text-white rounded-lg hover:bg-violet-700 transition-all shrink-0">
                      <Check class="w-3 h-3" />
                    </button>
                    <button @click.stop="cancelEdit"
                      class="p-1.5 bg-slate-100 text-slate-400 rounded-lg hover:bg-slate-200 transition-all shrink-0">
                      <X class="w-3 h-3" />
                    </button>
                  </div>

                  <div v-else>
                    <h3 class="text-base font-extrabold text-slate-800 tracking-tight line-clamp-1 mb-1">
                      {{ ws.name || ws.Name }}
                    </h3>
                    <p class="text-[11px] text-slate-400 font-medium flex items-center gap-1">
                      <Check class="w-3 h-3 text-slate-300" />
                      Leader: <span class="text-slate-600 font-semibold capitalize">{{ ws.creatorName }}</span>
                    </p>
                  </div>
                </div>
              </div>

              <div class="relative shrink-0 flex items-center gap-2">
                <span
                  class="text-[9px] font-extrabold px-2.5 py-1 rounded-full uppercase tracking-wider bg-amber-50 text-amber-600 hidden sm:block">
                  Urgent
                </span>
                <button @click.stop="toggleMenu(ws.id || ws.Id)"
                  class="p-1 text-slate-400 hover:text-slate-700 hover:bg-slate-50 rounded-lg transition-all">
                  <MoreVertical class="w-5 h-5" />
                </button>

                <Transition name="pop">
                  <div v-if="activeMenuId === (ws.id || ws.Id)"
                    class="absolute right-0 top-8 mt-1 w-36 bg-white border border-slate-100 rounded-xl shadow-lg z-20 py-1 overflow-hidden">
                    <button @click.stop="startEdit(ws); activeMenuId = null"
                      class="w-full text-left px-4 py-2.5 text-xs font-bold text-slate-600 hover:bg-slate-50 flex items-center gap-2">
                      <Pencil class="w-3.5 h-3.5 text-amber-500" /> Edit Proyek
                    </button>
                    <button @click.stop="handleDeleteWorkspace(ws.id || ws.Id, ws.name || ws.Name); activeMenuId = null"
                      class="w-full text-left px-4 py-2.5 text-xs font-bold text-rose-600 hover:bg-rose-50 flex items-center gap-2 border-t border-slate-50">
                      <Trash2 class="w-3.5 h-3.5" /> Hapus Proyek
                    </button>
                  </div>
                </Transition>
              </div>
            </div>

            <div class="mt-2 pt-4 border-t border-slate-100 flex justify-between items-end">
              <div class="flex gap-8 text-[10px] font-semibold">
                <div class="space-y-1.5">
                  <span class="text-slate-400 block font-bold uppercase tracking-wider text-[9px]">Status</span>
                  <div class="flex items-center gap-1.5 text-slate-700">
                    <span class="w-1.5 h-1.5 rounded-full bg-amber-500"></span>
                    <span>In Progress</span>
                  </div>
                </div>
                <div class="space-y-1.5">
                  <span class="text-slate-400 block font-bold uppercase tracking-wider text-[9px]">Deadline</span>
                  <span class="text-slate-700 block">12 Okt 2026</span>
                </div>
              </div>

              <button @click="handleSelectWorkspace(ws)"
                class="text-xs font-bold text-violet-600 hover:text-violet-700 transition-all pb-0.5 flex items-center gap-1">
                Buka Project <span>&rarr;</span>
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
<style scoped>
.pop-enter-active,
.pop-leave-active {
  transition: transform 0.15s ease-out, opacity 0.15s ease-out;
}

.pop-enter-from,
.pop-leave-to {
  transform: scale(0.95) translateY(-5px);
  opacity: 0;
}
</style>