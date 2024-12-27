<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import { useService } from '../composables/useService.ts';
  import { EvaluationsServiceKey } from '../services/evaluationService.ts';

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
