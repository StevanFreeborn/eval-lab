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
          path: 'evaluations/:id',
          component: () => import('./views/EvaluationView.vue'),
        },
        {
          path: 'evaluation-runs/:id',
          component: () => import('./views/EvaluationRunView.vue'),
        },
        {
          path: 'pipelines',
          component: () => import('./views/PipelinesView.vue'),
        },
        {
          path: 'pipelines/:id',
          component: () => import('./views/PipelineView.vue'),
        },
      ],
    },
  ],
});
