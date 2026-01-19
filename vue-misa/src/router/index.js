import { createRouter, createWebHistory } from 'vue-router'
import Shift from '@/views/Shift/Shift.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '',
      component: Shift,
    },
  ],
})

export default router
