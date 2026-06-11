import { createRouter, createWebHistory } from 'vue-router'
import WorkspacesView from '@/views/WorkspacesView.vue'
import DashboardView from '@/views/DashboardView.vue'
import LoginView from '@/views/LoginView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/workspaces'
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView 
    },
    {
      path: '/workspaces',
      name: 'workspaces',
      component: WorkspacesView
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView // Pastikan mengarah ke DashboardView Anda
    }
  ]
})
export default router