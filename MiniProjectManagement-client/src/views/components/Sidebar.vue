<script setup lang="ts">
import { LayoutGrid, Calendar, Folder, Settings } from 'lucide-vue-next'
</script>

<template>
    <aside class="w-20 bg-white border-r border-slate-100 flex flex-col items-center py-8 gap-10">
        <div class="w-10 h-10 bg-slate-900 rounded-xl flex items-center justify-center text-white font-bold">MP</div>
        <nav class="flex flex-col gap-8">
            <LayoutGrid class="w-6 h-6 text-slate-400 cursor-pointer hover:text-slate-900" />
            <Calendar class="w-6 h-6 text-slate-900 cursor-pointer" />
            <Folder class="w-6 h-6 text-slate-400 cursor-pointer hover:text-slate-900" />
            <Settings class="w-6 h-6 text-slate-400 cursor-pointer hover:text-slate-900" />
        </nav>
    </aside>
</template>