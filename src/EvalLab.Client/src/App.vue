<script setup lang="ts">
  import { ref, onMounted } from 'vue';

  const data = ref<string>('');

  onMounted(async () => {
    const response = await fetch('/api/health');
    data.value = await response.json();
  });
</script>

<template>
  <div>
    <h1>EvalLab</h1>
    <pre>{{ data }}</pre>
  </div>
</template>

<style scoped>
</style>
