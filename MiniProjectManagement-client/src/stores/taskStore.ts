import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/services/api'

export const useTaskStore = defineStore('task', () => {
    // Tambahan baru: State untuk menyimpan daftar Board
    const boards = ref<any[]>([])
    const tasksByBoard = ref<Record<number, any[]>>({})
    const isLoading = ref(false)
    const selectedTask = ref<any | null>(null) // Menyimpan tugas yang sedang diklik

    const selectTask = (task: any) => {
        selectedTask.value = task
    }

    // Fungsi baru: Mengambil daftar board dan mengurutkannya berdasarkan Position
    const fetchBoards = async (workspaceId: number) => {
        try {
            const response = await apiClient.get(`/Board/workspace/${workspaceId}`)
            boards.value = response.data.sort((a: any, b: any) => a.position - b.position)
        } catch (error) {
            console.error('Gagal mengambil data board:', error)
        }
    }

    // Fungsi lama (tidak banyak berubah)
    const fetchTasksByBoard = async (boardId: number) => {
        isLoading.value = true
        try {
            const response = await apiClient.get(`/TaskItem/board/${boardId}`)
            tasksByBoard.value[boardId] = response.data
        } catch (error) {
            console.error(`Gagal mengambil data tugas untuk board ${boardId}:`, error)
        } finally {
            isLoading.value = false
        }
    }

    const fetchBoardsAndTasks = async (workspaceId: number) => {
        isLoading.value = true
        try {
            const boardRes = await apiClient.get(`/Board/workspace/${workspaceId}`)
            boards.value = boardRes.data

            console.log("Boards yang diambil:", boards.value) 

            tasksByBoard.value = {}
            const taskPromises = boards.value.map(board => fetchTasksByBoard(board.id))

            await Promise.all(taskPromises)
        } catch (error) {
            console.error('Gagal memuat papan Kanban secara keseluruhan:', error)
        } finally {
            isLoading.value = false
        }
    }
    // Fungsi baru: Menambahkan tugas ke database
    const createTask = async (payload: { title: string; description: string; priority: string; category: string; dueDate: string | null; boardId: number }) => {
        try {
            await apiClient.post('/TaskItem', payload)
            await fetchTasksByBoard(payload.boardId)
        } catch (error) {
            console.error('Gagal membuat tugas baru:', error)
            throw error
        }
    }

    // Fungsi baru: Menambahkan Board/Kolom baru
    const addBoard = async (name: string, workspaceId: number) => {
        try {
            await apiClient.post('/Board', {
                Name: name,
                WorkspaceId: workspaceId
            });

            // Refresh data boards dan tasks agar kolom baru langsung muncul di layar
            await fetchBoardsAndTasks(workspaceId);
        } catch (error) {
            console.error("Gagal menambahkan kolom baru:", error);
            throw error;
        }
    }

    //update Task
    const updateTaskBoard = async (taskId: number, newBoardId: number) => {
        try {
            // 1. Ambil data tugas lama yang sedang aktif dari state untuk mempertahankan nilai title & description
            if (!selectedTask.value) return false

            const updatedData = {
                id: taskId,
                title: selectedTask.value.title,
                description: selectedTask.value.description,
                priority: selectedTask.value.priority,
                createdAt: selectedTask.value.createdAt,
                boardId: newBoardId, // Ganti dengan Board ID yang baru
                assigneeId: selectedTask.value.assigneeId
            }

            // 2. Kirim perubahan ke backend .NET (Sesuaikan jika endpoint PUT Anda berbeda)
            await apiClient.put(`/TaskItem/${taskId}`, updatedData)

            // 3. Ambil data lama board asal untuk kita refresh di UI
            const oldBoardId = selectedTask.value.boardId

            // 4. Update data di memori lokal agar tidak perlu refresh halaman penuh
            selectedTask.value.boardId = newBoardId

            // 5. Tarik ulang data tugas untuk kedua board terkait (board lama dan board baru)
            await fetchTasksByBoard(oldBoardId)
            await fetchTasksByBoard(newBoardId)

            return true
        } catch (error) {
            console.error('Gagal memindahkan tugas:', error)
            return false
        }
    }

    const moveTask = async (taskId: number, fromBoardId: number, toBoardId: number) => {
        // 1. Ambil array kolom asal dengan aman. Jika belum ada array-nya, default-kan ke array kosong.
        const sourceList = tasksByBoard.value[fromBoardId] || [];

        const taskIndex = sourceList.findIndex(t => (t.id || t.Id) === taskId);

        if (taskIndex !== -1) {
            // Jalankan splice pada objek array aman yang sudah divalidasi
            const [movedTask] = sourceList.splice(taskIndex, 1);

            // Perbarui properti BoardId penunjuk kolom baru
            if (movedTask) {
                if ('BoardId' in movedTask) movedTask.BoardId = toBoardId;
                if ('boardId' in movedTask) movedTask.boardId = toBoardId;

                // Amankan kolom tujuan, jika belum ada array-nya, buat baru
                if (!tasksByBoard.value[toBoardId]) {
                    tasksByBoard.value[toBoardId] = [];
                }

                // Dorong masuk kartu ke kolom baru
                tasksByBoard.value[toBoardId].push(movedTask);
            }
        }

        // 2. Tembak database .NET di latar belakang
        try {
            await apiClient.put(`/TaskItem/${taskId}/move`, {
                NewBoardId: toBoardId
            });
        } catch (error) {
            console.error("Gagal menyimpan posisi tugas di database:", error);
            // Jika gagal, tarik ulang datanya sebagai rollback aman
            await fetchTasksByBoard(fromBoardId);
            await fetchTasksByBoard(toBoardId);
        }
    }

    return {
        boards,
        tasksByBoard,
        isLoading,
        selectedTask,
        fetchBoards,
        fetchTasksByBoard,
        fetchBoardsAndTasks,
        selectTask,
        createTask,
        addBoard,
        updateTaskBoard,
        moveTask
    }
})