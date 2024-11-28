import { createRouter, createWebHistory } from 'vue-router';

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: () => import('./components/MainLayout.vue'),
      children: [
        {
          path: '',
          component: () => import('./views/BenchView.vue'),
        },
        {
          path: 'evaluations',
          component: () => import('./views/EvaluationsView.vue'),
        },
        {
          path: 'pipelines',
          component: () => import('./views/PipelinesView.vue'),
        },
      ],
    },
  ],
});
