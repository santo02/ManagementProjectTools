import apiClient from "@/services/api";
import { defineStore } from "pinia";
import { ref } from "vue";

export const useWorkspaceStore = defineStore('workspace', () => {
    const workspaces = ref<any[]>([]);
    const currentWorkspace = ref<any | null>(null);
    const isLeader = ref(false);
    const isLoading = ref(false);

    const fetchWorkspace = async () => {
        isLoading.value = true;
        try {
            const response = await apiClient.get('/Workspace/my-workspaces');
            workspaces.value = response.data;
        } catch (error) {
            console.error('Error fetching workspaces:', error);
        } finally {
            isLoading.value = false;
        }
    }

    const setActiveWorkspace = (workspace: any, currentUserId: any) => {
        currentWorkspace.value = workspace;
        isLeader.value = workspace.leaderId === currentUserId;
    }

    const addMemberToWorkspace = async (workspaceId: any, username: string) => {
        try {
            await apiClient.post(`/Workspace/${workspaceId}/member`, { username });
            return { success: true, message: 'Anggota Berhasil Ditambahkan' };
        } catch (error: any) {
            const errorMessage = error.response?.data || 'Error adding member to workspace';
            return { success: false, message: errorMessage };
        }
    }

    const createWorkspace = async (workspaceName: string) => {
        try {
            const response = await apiClient.post('/Workspace', {
                Name: workspaceName
            })

            console.log('Workspace berhasil dibuat di backend:', response.data)

            return response.data
        } catch (error) {
            console.error('Gagal mengirim data workspace ke backend:', error)
            throw error
        }
    }

    const updateWorkspace = async (id: number, newName: string) => {
        try {
            // Sinkron dengan DTO backend .NET (Menggunakan 'Name' kapital)
            const response = await apiClient.put(`/Workspace/${id}`, {
                Name: newName
            })

            // Refresh daftar workspace lokal setelah berhasil diedit
            await fetchWorkspace()
            return response.data
        } catch (error) {
            console.error("Gagal memperbarui workspace:", error)
            throw error
        }
    }

    const deleteWorkspace = async (id: number) => {
        try {
            await apiClient.delete(`/Workspace/${id}`)

            // Refresh list setelah dihapus
            await fetchWorkspace()
        } catch (error) {
            console.error("Gagal menghapus workspace:", error)
            throw error
        }
    }

    return {
        workspaces,
        currentWorkspace,
        isLeader,
        isLoading,
        fetchWorkspace,
        setActiveWorkspace,
        addMemberToWorkspace,
        createWorkspace,
        updateWorkspace,
        deleteWorkspace
    }
})