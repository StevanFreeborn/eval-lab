<script setup lang="ts">
  import { computed, onMounted, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import PagedDropdown from '../components/controls/PagedDropdown.vue';
  import ThumbsDownIcon from '../components/icons/ThumbsDownIcon.vue';
  import ThumbsUpIcon from '../components/icons/ThumbsUpIcon.vue';
  import RunCard from '../components/RunCard.vue';
  import SlideDrawer from '../components/SlideDrawer.vue';
  import TraceViewer from '../components/TraceViewer.vue';
  import WaitingSpinner from '../components/WaitingSpinner.vue';
  import { useService } from '../composables/useService.ts';
  import { Evaluation, EvaluationsServiceKey, TestResult } from '../services/evaluationService.ts';
  import { PipelinesServiceKey } from '../services/pipelineService.ts';

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

  // basic info section
  // criteria section
  // exact match
  // expected output unstructured
  // expected value to match

  const route = useRoute();
  const evaluationId = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id;
  const evaluation = ref<Evaluation>();
  const isValidEvaluation = computed(
    () =>
      evaluation.value !== undefined &&
      !evaluation.value.input &&
      !evaluation.value.targetPipelineId &&
      evaluation.value.successCriteria.type !== 'null',
  );
  const evaluationTestResult = ref<
    | {
        status: 'idle' | 'loading' | 'failed';
      }
    | {
        status: 'success';
        testResult: TestResult;
      }
  >({ status: 'idle' });
  const drawerOpen = ref(false);

  const evaluationsService = useService(EvaluationsServiceKey);
  const pipelinesService = useService(PipelinesServiceKey);

  function closeDrawer() {
    drawerOpen.value = false;
  }

  function openDrawer() {
    drawerOpen.value = true;
  }

  async function getEvaluation() {
    const getEvaluationResult = await evaluationsService.get(evaluationId);

    if (getEvaluationResult.failed) {
      console.error(getEvaluationResult.error.message);
      return;
    }

    evaluation.value = getEvaluationResult.value;
  }

  onMounted(getEvaluation);

  async function testEvaluation() {
    if (evaluation.value) {
      evaluationTestResult.value = { status: 'loading' };

      const testResult = await evaluationsService.test(evaluation.value);

      if (testResult.failed) {
        console.error(testResult.error.message);
        evaluationTestResult.value = { status: 'failed' };
        return;
      }

      evaluationTestResult.value = { status: 'success', testResult: testResult.value };
      openDrawer();
    }
  }

  async function performEvaluation() {
    evaluationTestResult.value = { status: 'idle' };
    openDrawer();

    // TOOD: Need to show drawer with form to collect following information
    // - Expected proportion of runs to pass
    // - Desired confidence level
    // - Desired margin of error
    // - when these are entered the form should show sample size required
    // - user should then be able to confirm and start evaluation
  }
</script>

<template>
  <div
    v-if="!evaluation"
    class="loading-container"
  >
    <p>Loading...</p>
  </div>
  <div
    v-else
    class="evaluation-container"
  >
    <div class="header">
      <h2>{{ evaluation.name }}</h2>
      <p class="description">{{ evaluation.description }}</p>
    </div>
    <div class="basic-info">
      <div class="details-grid">
        <div class="detail-item">
          <label>ID</label>
          <p class="monospace">{{ evaluation.id }}</p>
        </div>
        <div class="detail-item">
          <label>Created Date</label>
          <p>{{ evaluation.createdDate.toLocaleString() }}</p>
        </div>
        <div class="detail-item">
          <label>Updated Date</label>
          <p>{{ evaluation.updatedDate.toLocaleString() }}</p>
        </div>
      </div>
    </div>
    <div class="evaluation-definition">
      <div>
        <div class="card">
          <h3>Evaluation Input</h3>
          <div class="form-group">
            <label>Input</label>
            <textarea
              v-model="evaluation.input"
              required
              rows="10"
            ></textarea>
          </div>
        </div>
        <div class="card">
          <h3>Pipeline</h3>
          <div class="form-group">
            <label>Selected Pipeline</label>
            <PagedDropdown
              v-model="evaluation.targetPipelineId"
              :get-options="pipelinesService.getAll"
              :map-option="i => ({ id: i.id, name: i.name })"
              sort-by="name"
              sort-order="asc"
              placeholder="Select a pipeline"
              search-placeholder="Search pipelines"
              :required="true"
              :disabled="true"
            />
          </div>
        </div>
        <div class="card">
          <h3>Success Criteria</h3>
          <div class="form-group">
            <label>Evaluation Type</label>
            <select
              v-model="evaluation.successCriteria.type"
              required
            >
              <option value="null">Select Evaluation Type</option>
              <option>Unstructured Exact Match</option>
            </select>
          </div>
          <div
            v-if="evaluation.successCriteria.type === 'Unstructured Exact Match'"
            class="form-group"
          >
            <label>Match Value</label>
            <textarea
              v-model="evaluation.successCriteria.matchValue"
              required
              rows="1"
            ></textarea>
          </div>
        </div>
      </div>
      <div class="actions">
        <button
          type="button"
          @click="testEvaluation"
          :disabled="!isValidEvaluation"
        >
          <WaitingSpinner
            v-if="evaluationTestResult.status === 'loading'"
            height="1rem"
            width="1rem"
          />
          Test Evaluation
        </button>
        <button
          type="button"
          @click="performEvaluation"
          :disabled="!isValidEvaluation"
        >
          Perform Evaluation
        </button>
      </div>
    </div>
  </div>
  <SlideDrawer
    :heading="
      evaluationTestResult.status === 'success' ? 'Evaluation Test Result' : 'Perform Evaluation'
    "
    :drawer-open="drawerOpen"
    @drawer-closed="closeDrawer"
  >
    <div
      v-if="evaluationTestResult.status === 'success'"
      class="test-result-container"
    >
      <div class="test-result-status">
        <div
          v-if="evaluationTestResult.testResult.passed"
          class="success"
        >
          <ThumbsUpIcon />
          Passed
        </div>
        <div
          v-else
          class="failed"
        >
          <ThumbsDownIcon />
          Failed
        </div>
      </div>
      <RunCard
        :run="evaluationTestResult.testResult.run"
        style="background-color: var(--background-color)"
      />
      <TraceViewer :run-id="evaluationTestResult.testResult.run.id" />
    </div>
  </SlideDrawer>
</template>

<style scoped>
  .evaluation-container {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .header {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    border-bottom: 1px solid #e5e7eb;
    padding-bottom: 1rem;
  }

  .header h2 {
    font-size: 1.5rem;
    font-weight: 700;
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

  .evaluation-definition {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .evaluation-definition > div:first-child {
    display: flex;
    gap: 1rem;
    flex-wrap: wrap;
  }

  .card {
    display: flex;
    flex-direction: column;
    flex: 1;
    gap: 1rem;
    background-color: var(--secondary-background-color);
    padding: 1rem;
    border-radius: 0.25rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 0.375rem;
  }

  textarea {
    width: 100%;
    resize: vertical;
    padding: 0.25rem;
    border-radius: 0.25rem;
    min-height: 1.75rem;
  }

  textarea:required {
    border-left: 1px solid red;
  }

  select {
    background-color: var(--background-color);
    color: inherit;
    font-size: inherit;
    font-family: inherit;
    padding: 0.5rem 1rem;
    border-radius: 0.25rem;
    border: none;
  }

  select:required {
    border-left: 1px solid red;
  }

  .actions {
    display: flex;
    gap: 1rem;
  }

  .actions button {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.5rem 1rem;
    border-radius: 0.25rem;
    background-color: var(--secondary-background-color);
  }

  .actions button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .test-result-container {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin: 1rem;
    border-radius: 0.5rem;
  }

  .test-result-status {
    display: flex;
    gap: 0.5rem;
    padding: 0.5rem 1rem;
    border-radius: 0.25rem;
    background-color: var(--secondary-background-color);
    align-items: center;
  }

  .test-result-status div {
    display: flex;
    gap: 0.5rem;
    align-items: center;
    font-size: 1.5rem;
  }

  .test-result-status svg {
    width: 1.5rem;
    height: 1.5rem;
  }

  .test-result-status .success svg {
    color: #2c786c;
  }

  .test-result-status .failed svg {
    color: #e53e3e;
  }
</style>
