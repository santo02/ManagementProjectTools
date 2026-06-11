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

    return {
        workspaces,
        currentWorkspace,
        isLeader,
        isLoading,
        fetchWorkspace,
        setActiveWorkspace,
        addMemberToWorkspace,
        createWorkspace
    }
})