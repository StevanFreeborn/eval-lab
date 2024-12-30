import { createApp } from 'vue';
import { evaluationsService, EvaluationsServiceKey } from '../src/services/evaluationService';
import App from './App.vue';
import { router } from './router';
import { evaluationRunService, EvaluationRunsServiceKey } from './services/evaluationRunService';
import { pipelineRunsService, PipelineRunsServiceKey } from './services/pipelineRunService';
import { pipelinesService, PipelinesServiceKey } from './services/pipelineService';
import { tracesService, TracesServiceKey } from './services/traceService';
import './style.css';

createApp(App)
  .provide(EvaluationsServiceKey, evaluationsService)
  .provide(EvaluationRunsServiceKey, evaluationRunService)
  .provide(PipelinesServiceKey, pipelinesService)
  .provide(PipelineRunsServiceKey, pipelineRunsService)
  .provide(TracesServiceKey, tracesService)
  .use(router)
  .mount('#app');
