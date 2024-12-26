<script setup lang="ts">
  import { defineProps } from 'vue';
  import { Run } from '../../services/pipelineService';

  defineProps<{ run: Run | null }>();
</script>

<template>
  <div class="tab-content">
    <p
      v-if="!run"
      class="placeholder"
    >
      Run input to get response from pipeline
    </p>
    <p v-else>{{ run.output }}</p>
  </div>
</template>

<style scoped>
  .tab-content {
    display: flex;
    flex-direction: column;
    flex: 1;
  }

  .placeholder {
    align-self: center;
    align-content: center;
    height: 100%;
  }
</style>
