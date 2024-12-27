<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import { useService } from '../composables/useService.ts';
  import { Pipeline, PipelinesServiceKey } from '../services/pipelineService.ts';
  import { Run, RunsServiceKey } from '../services/runService.ts';

  // TODO: Runs will be queried separately and will be paginated
  // TODO: Add actions for each run: trace view and delete
  // TODO: Should runs be a table instead of a list?

  const route = useRoute();
  const pipelineId = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id;
  const pipeline = ref<Pipeline>();
  const runs = ref<Run[]>([]);

  const pipelinesService = useService(PipelinesServiceKey);
  const runsService = useService(RunsServiceKey);

  onMounted(async () => {
    const getPipelineResult = await pipelinesService.get(pipelineId);

    if (getPipelineResult.failed) {
      console.error(getPipelineResult.error.message);
      return;
    }

    pipeline.value = getPipelineResult.value;
  });

  onMounted(async () => {
    const getRunsResult = await runsService.getAll({ additionalParams: { pipelineId } });

    if (getRunsResult.failed) {
      console.error(getRunsResult.error.message);
      return;
    }

    runs.value = getRunsResult.value.items;
  });
</script>

<template>
  <div class="container">
    <div v-if="!pipeline">Loading...</div>
    <div
      v-else
      class="content"
    >
      <div class="header">
        <h1>{{ pipeline.name }}</h1>
        <p class="description">{{ pipeline.description }}</p>
      </div>

      <div class="details-grid">
        <div class="detail-item">
          <label>ID</label>
          <p class="monospace">{{ pipeline.id }}</p>
        </div>
        <div class="detail-item">
          <label>Endpoint</label>
          <p class="monospace">{{ pipeline.endpoint }}</p>
        </div>
        <div class="detail-item">
          <label>Created Date</label>
          <p>{{ pipeline.createdDate.toLocaleString() }}</p>
        </div>
        <div class="detail-item">
          <label>Updated Date</label>
          <p>{{ pipeline.updatedDate.toLocaleString() }}</p>
        </div>
      </div>

      <div class="runs-section">
        <h2>Runs ({{ runs.length }})</h2>
        <div class="runs-list">
          <div
            v-for="run in runs"
            :key="run.id"
            class="run-item"
          >
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
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
  .container {
    display: flex;
    flex-direction: column;
    width: 100%;
    min-height: 0;
  }

  .content {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
    min-height: 0;
  }

  .header {
    border-bottom: 1px solid #e5e7eb;
    padding-bottom: 1rem;
  }

  .header h1 {
    font-size: 1.5rem;
    font-weight: 700;
    margin-bottom: 0.5rem;
  }

  .details-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
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

  .monospace {
    font-family: monospace;
    font-size: 0.875rem;
  }

  .runs-section {
    display: flex;
    flex-direction: column;
    min-height: 0;
    margin-top: 2rem;
  }

  .runs-section h2 {
    font-size: 1.125rem;
    font-weight: bold;
    margin-bottom: 1rem;
  }

  .runs-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    padding: 1rem;
    overflow: auto;
  }

  .run-item {
    border: 2px solid var(--secondary-background-color);
    border-radius: 0.5rem;
    padding: 1rem;
  }

  .run-header {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
    margin-bottom: 1rem;
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
