<script setup lang="ts">
import { onMounted, ref } from 'vue'
import Sidebar from '../views/components/Sidebar.vue'
import TaskList from '../views/components/TaskList.vue'
import TaskDetail from '../views/components/TaskDetail.vue'
import { useTaskStore } from '../stores/taskStore.ts'

const taskStore = useTaskStore()

onMounted(async () => {
    // 1. Tarik daftar kolom (TODO, IN PROGRESS, dll) dari database
    await taskStore.fetchBoards()

    // 2. Looping kolom yang didapat, lalu tarik daftar tugasnya satu per satu
    for (const board of taskStore.boards) {
        await taskStore.fetchTasksByBoard(board.id)
    }
})
</script>

<template>
    <div class="flex h-screen bg-[#F8F7FC] font-['Inter']">
        <Sidebar />
        <TaskList />
        <TaskDetail />
    </div>
</template>