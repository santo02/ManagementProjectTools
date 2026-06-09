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
    const fetchBoards = async () => {
        try {
            // Pastikan endpoint ini sesuai dengan C# Anda (misal: /Board atau /Boards)
            const response = await apiClient.get('/Board')
            // Mengurutkan array agar TODO tampil pertama, lalu IN PROGRESS, dst
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
    // Fungsi baru: Menambahkan tugas ke database
    const addTask = async (taskData: { title: string, description: string, priority: string, boardId: number }) => {
        try {
            // Menembak endpoint POST ke .NET
            await apiClient.post('/TaskItem', taskData)

            // Setelah sukses menyimpan, kita 'refresh' (tarik ulang) data khusus untuk kolom tersebut
            await fetchTasksByBoard(taskData.boardId)
            return true // Mengembalikan nilai true jika sukses
        } catch (error) {
            console.error('Gagal menambahkan tugas:', error)
            return false
        }
    }

    // Fungsi baru: Menambahkan Board/Kolom baru
    const addBoard = async (title: string, position: number) => {
        try {

            // Asumsi default WorkspaceId adalah 1 (sesuaikan jika Anda sudah mengatur login multi-tenant)
            const boardData = {
                title: title,
                position: position,
                workspaceId: 1
            }

            await apiClient.post('/Board', boardData) // Pastikan endpoint ini sesuai dengan C# Anda

            // Tarik ulang daftar board dari database agar UI langsung ter-update
            await fetchBoards()
            return true
        } catch (error) {
            console.error('Gagal menambahkan board:', error)
            return false
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

    return {
        boards,
        tasksByBoard,
        isLoading,
        selectedTask,
        fetchBoards,
        fetchTasksByBoard,
        selectTask,  
        addTask,
        addBoard,
        updateTaskBoard
    }
})