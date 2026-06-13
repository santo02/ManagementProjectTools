<template>
  <div class="flex flex-col items-center justify-center h-screen bg-[#F8F7FC] font-['Inter']">
    <div class="flex flex-col items-center text-center max-w-lg">
      <div class="relative mb-8">
        <div
          class="w-40 h-40 bg-white rounded-[40px] shadow-sm flex items-center justify-center border border-slate-100">
          <svg class="w-20 h-20 text-violet-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"
              d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>
        <div class="absolute -top-3 -right-3 bg-violet-600 p-3 rounded-2xl text-white shadow-lg">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0zM10 7v3m0 0v3m0-3h3m-3 0H7" />
          </svg>
        </div>
      </div>

      <h1 class="text-6xl font-black text-slate-900 mb-4">404</h1>
      <h2 class="text-2xl font-bold text-slate-800 mb-3">Halaman Tidak Ditemukan</h2>
      <p class="text-slate-500 font-medium mb-10 leading-relaxed px-6">
        Ups! Sepertinya halaman yang Anda cari telah dipindahkan atau tidak ada di ruang kerja ini. Mari kita kembali ke
        jalur yang benar.
      </p>

      <div class="flex gap-4 w-full justify-center">
        <button @click="$router.push('/dashboard')"
          class="px-8 py-3 bg-violet-700 hover:bg-violet-800 text-white font-bold rounded-xl transition-all shadow-lg shadow-violet-200 flex items-center gap-2">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" />
          </svg>
          Kembali ke Dashboard
        </button>
      </div>

      <!-- <div class="flex gap-3 mt-12">
        <div class="flex items-center gap-2 bg-white px-4 py-2 rounded-full border border-slate-200 shadow-sm">
          <div class="w-2 h-2 rounded-full bg-amber-700"></div>
          <span class="text-xs font-bold text-slate-600">Proses Sinkronisasi</span>
        </div>
        <div class="flex items-center gap-2 bg-white px-4 py-2 rounded-full border border-slate-200 shadow-sm">
          <div class="w-2 h-2 rounded-full bg-violet-600"></div>
          <span class="text-xs font-bold text-slate-600">Akses Stabil</span>
        </div>
      </div> -->
    </div>
  </div>
</template>