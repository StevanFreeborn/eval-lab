<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import { useService } from '../composables/useService.ts';
  import { EvaluationsServiceKey } from '../services/evaluationService.ts';

  // TODO: Evaluations need further defining
  // user needs to select the target pipeline of the evaluation
  // user needs to select the criteria for the evaluation
  // - exact match
  // - partial match
  // - cosine similarity
  // - LLM assisted => could possibly be stored "graders" or "evaluators"
  // user needs to provide the expected output
  // user needs to provide the output type i.e. structured or unstructured
  // user needs to provide number of runs to evaluate

  const route = useRoute();
  const evaluationId = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id;
  const data = ref<string>('');

  const evaluationsService = useService(EvaluationsServiceKey);

  onMounted(async () => {
    const result = await evaluationsService.get(evaluationId);

    if (result.failed) {
      console.error(result.error.message);
      return;
    }

    data.value = JSON.stringify(result.value, null, 2);
    console.log(result);
  });
</script>

<template>
  <pre>{{ data }}</pre>
</template>

<style scoped></style>
