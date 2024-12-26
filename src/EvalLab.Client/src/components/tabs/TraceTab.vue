<script setup lang="ts">
  import { defineProps } from 'vue';
  import { Run } from '../../services/pipelineService';
  import TraceViewer from '../TraceViewer.vue';

  defineProps<{ run: Run | null }>();
</script>

<template>
  <div class="tab-content">
    <p
      v-if="!run"
      class="placeholder"
    >
      Waiting for trace information.
    </p>
    <TraceViewer
      v-else
      :run-id="run.id"
    />
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
