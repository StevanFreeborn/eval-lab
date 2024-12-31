<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import { useService } from '../composables/useService.ts';
  import { EvaluationRun, EvaluationRunsServiceKey } from '../services/evaluationRunService.ts';

  const route = useRoute();
  const evaluationRunId = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id;
  const evaluationRun = ref<EvaluationRun>();

  const evaluationRunsService = useService(EvaluationRunsServiceKey);

  onMounted(async () => {
    var getEvaluationRunResult = await evaluationRunsService.get(evaluationRunId);

    if (getEvaluationRunResult.failed) {
      console.error(getEvaluationRunResult.error);
      return;
    }

    evaluationRun.value = getEvaluationRunResult.value;
  });
</script>

<template>
  <div>
    <pre>{{ evaluationRun }}</pre>
  </div>
</template>

<style scoped></style>
