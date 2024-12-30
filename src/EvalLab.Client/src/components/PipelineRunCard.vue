<script setup lang="ts">
  import { PipelineRun } from '../services/pipelineRunService.ts';

  defineProps<{
    run: PipelineRun;
  }>();
</script>

<template>
  <div class="run-card">
    <div class="run-header">
      <div class="detail-item">
        <label>Run ID</label>
        <p class="monospace">{{ run.id }}</p>
      </div>
      <div class="detail-item">
        <label>Created Date</label>
        <p>{{ run.createdDate.toLocaleString() }}</p>
      </div>
    </div>
    <div class="run-content">
      <div class="io-section">
        <label>Input</label>
        <div class="io-content">{{ run.input }}</div>
      </div>
      <div class="io-section">
        <label>Output</label>
        <div class="io-content">{{ run.output }}</div>
      </div>
    </div>
    <slot></slot>
  </div>
</template>

<style scoped>
  .run-card {
    padding: 1rem;
    border-radius: 0.5rem;
  }

  .run-header {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
    margin-bottom: 1rem;
  }

  .detail-item {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .detail-item label {
    font-size: 0.875rem;
    font-weight: bold;
  }

  .run-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .io-section {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .io-section label {
    font-size: 0.875rem;
    font-weight: bold;
  }

  .io-content {
    background-color: var(--secondary-background-color);
    padding: 0.75rem;
    border-radius: 0.375rem;
  }
</style>
