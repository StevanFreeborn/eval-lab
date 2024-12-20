import { createApp } from 'vue';
import { evaluationsService, EvaluationsServiceKey } from '../src/services/evaluationService';
import App from './App.vue';
import { router } from './router';
import { pipelinesService, PipelinesServiceKey } from './services/pipelineService';
import './style.css';

createApp(App)
  .provide(EvaluationsServiceKey, evaluationsService)
  .provide(PipelinesServiceKey, pipelinesService)
  .use(router)
  .mount('#app');
