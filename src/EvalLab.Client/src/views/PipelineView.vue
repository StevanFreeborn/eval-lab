<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import ChartBarIcon from '../components/icons/ChartBarIcon.vue';
  import TrashCanIcon from '../components/icons/TrashCanIcon.vue';
  import PipelineRunCard from '../components/PipelineRunCard.vue';
  import SlideDrawer from '../components/SlideDrawer.vue';
  import TraceViewer from '../components/TraceViewer.vue';
  import { useService } from '../composables/useService.ts';
  import { PipelineRun, PipelineRunsServiceKey } from '../services/pipelineRunService.ts';
  import { Pipeline, PipelinesServiceKey } from '../services/pipelineService.ts';

  // TODO: Pagination/filtering for runs
  // TODO: Support updating pipeline

  const route = useRoute();
  const pipelineId = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id;
  const pipeline = ref<Pipeline>();
  const pipelineRuns = ref<PipelineRun[]>([]);
  const selectedPipelineRun = ref<PipelineRun | null>(null);
  const drawerOpen = ref(false);

  const pipelinesService = useService(PipelinesServiceKey);
  const pipelineRunsService = useService(PipelineRunsServiceKey);

  async function getPipeline() {
    const getPipelineResult = await pipelinesService.get(pipelineId);

    if (getPipelineResult.failed) {
      console.error(getPipelineResult.error.message);
      return;
    }

    pipeline.value = getPipelineResult.value;
  }

  onMounted(getPipeline);

  async function getPipelineRuns() {
    const getPipelineRunsResult = await pipelineRunsService.getAll({
      additionalParams: { pipelineId },
    });

    if (getPipelineRunsResult.failed) {
      console.error(getPipelineRunsResult.error.message);
      return;
    }

    pipelineRuns.value = getPipelineRunsResult.value.items;
  }

  onMounted(getPipelineRuns);

  async function handleDeleteRunClick(runId: string) {
    const isSure = confirm('Are you sure you want to delete this run?');

    if (!isSure) {
      return;
    }

    const deleteRunResult = await pipelineRunsService.delete(runId);

    if (deleteRunResult.failed) {
      console.error(deleteRunResult.error.message);
      return;
    }

    getPipelineRuns();
  }

  function openDrawer() {
    drawerOpen.value = true;
  }

  function closeDrawer() {
    drawerOpen.value = false;
  }

  function handleTraceClick(pipelineRun: PipelineRun) {
    selectedPipelineRun.value = pipelineRun;
    openDrawer();
  }
</script>

<template>
  <div class="container">
    <div v-if="!pipeline">Loading...</div>
    <div
      v-else
      class="content"
    >
      <div class="header">
        <h2>{{ pipeline.name }}</h2>
        <p class="description">{{ pipeline.description }}</p>
      </div>

      <div class="details-grid">
        <div class="detail-item">
          <label>ID</label>
          <p class="monospace">{{ pipeline.id }}</p>
        </div>
        <div class="detail-item">
          <label>Created Date</label>
          <p>{{ pipeline.createdDate.toLocaleString() }}</p>
        </div>
        <div class="detail-item">
          <label>Updated Date</label>
          <p>{{ pipeline.updatedDate.toLocaleString() }}</p>
        </div>
        <div class="detail-item">
          <label>Endpoint</label>
          <p class="monospace">{{ pipeline.endpoint }}</p>
        </div>
      </div>

      <div
        class="runs-section"
        ref="runsSection"
      >
        <h3>Runs ({{ pipelineRuns.length }})</h3>
        <ul class="runs-list">
          <li
            v-for="pipelineRun in pipelineRuns"
            :key="pipelineRun.id"
            class="run-item"
          >
            <PipelineRunCard :run="pipelineRun">
              <div class="run-actions">
                <button
                  type="button"
                  @click="() => handleTraceClick(pipelineRun)"
                >
                  <ChartBarIcon />
                  <span class="sr-only">View Trace</span>
                </button>
                <button
                  type="button"
                  @click="() => handleDeleteRunClick(pipelineRun.id)"
                >
                  <TrashCanIcon />
                  <span class="sr-only">Delete Run</span>
                </button>
              </div></PipelineRunCard
            >
          </li>
        </ul>
      </div>
    </div>
  </div>
  <SlideDrawer
    :heading="`Trace for Run ${selectedPipelineRun?.id}`"
    :drawer-open="drawerOpen"
    @drawer-closed="closeDrawer"
  >
    <div style="padding: 1rem; width: 100%">
      <TraceViewer
        v-if="selectedPipelineRun"
        :run-id="selectedPipelineRun.id"
      />
      <div v-else>No run selected.</div>
    </div>
  </SlideDrawer>
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

  .header h2 {
    font-size: 1.5rem;
    font-weight: 700;
    margin-bottom: 0.5rem;
  }

  .details-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
  }

  .details-grid .detail-item:first-child {
    grid-column: -1 / 1;
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

  .runs-section h3 {
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
  }

  .run-actions {
    display: flex;
    gap: 0.5rem;
    margin-top: 1rem;
  }

  .run-actions button {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.5rem;
    border: 2px solid var(--secondary-background-color);
    border-radius: 0.375rem;
    font-size: 0.875rem;
    font-weight: bold;
    cursor: pointer;
  }

  .run-actions button svg {
    width: 1rem;
    height: 1rem;
  }
</style>
