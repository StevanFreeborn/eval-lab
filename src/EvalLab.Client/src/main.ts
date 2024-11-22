import { createApp } from 'vue';
import './style.css';
import App from './App.vue';
import { router } from './router';
import { EvaluationsServiceKey, evaluationsService } from '../src/services/evaluationService';

createApp(App).provide(EvaluationsServiceKey, evaluationsService).use(router).mount('#app');
