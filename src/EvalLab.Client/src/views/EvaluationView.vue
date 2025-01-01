<script setup lang="ts">
  import { computed, onMounted, ref } from 'vue';
  import { useRoute } from 'vue-router';
  import PagedDropdown from '../components/controls/PagedDropdown.vue';
  import TextEditor from '../components/controls/TextEditor.vue';
  import GenericForm from '../components/forms/GenericForm.vue';
  import ThumbsDownIcon from '../components/icons/ThumbsDownIcon.vue';
  import ThumbsUpIcon from '../components/icons/ThumbsUpIcon.vue';
  import PagedTable, { TableData } from '../components/PagedTable.vue';
  import PipelineRunCard from '../components/PipelineRunCard.vue';
  import SlideDrawer from '../components/SlideDrawer.vue';
  import TraceViewer from '../components/TraceViewer.vue';
  import WaitingSpinner from '../components/WaitingSpinner.vue';
  import { useService } from '../composables/useService.ts';
  import { EvaluationRun, EvaluationRunsServiceKey } from '../services/evaluationRunService.ts';
  import { Evaluation, EvaluationsServiceKey, TestRun } from '../services/evaluationService.ts';
  import { PipelinesServiceKey } from '../services/pipelineService.ts';

  // TODO: Separate table logic into component

  const route = useRoute();
  const evaluationId = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id;
  const evaluation = ref<Evaluation>();
  const isValidEvaluation = computed(
    () =>
      evaluation.value !== undefined &&
      evaluation.value.targetPipelineId &&
      evaluation.value.successCriteria.type !== 'null',
  );
  const testInput = ref('');
  const evaluationTestResult = ref<
    | {
        status: 'idle' | 'entering-input' | 'loading' | 'failed';
      }
    | {
        status: 'success';
        testResult: TestRun;
      }
  >({ status: 'idle' });
  const drawerOpen = ref(false);
  const evaluationRunData = ref<TableData<EvaluationRun>>({ status: 'initial' });
  const evaluationRunDataPage = computed(() =>
    evaluationRunData.value.status === 'success' ? evaluationRunData.value.page.pageNumber : 1,
  );
  const isEditable = computed(
    () =>
      evaluationRunData.value.status === 'success' && evaluationRunData.value.page.totalItems === 0,
  );

  const evaluationsService = useService(EvaluationsServiceKey);
  const evaluationRunsService = useService(EvaluationRunsServiceKey);
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

  async function getEvaluationRuns(pageNumber?: number, pageSize?: number) {
    const timeout = setTimeout(() => {
      evaluationRunData.value.status = 'loading';
    }, 500);

    try {
      const getEvaluationRunsResult = await evaluationRunsService.getAll({
        pageNumber,
        pageSize,
        additionalParams: { evaluationId },
      });

      if (getEvaluationRunsResult.failed) {
        console.error(getEvaluationRunsResult.error.message);
        evaluationRunData.value.status = 'error';
        return;
      }

      evaluationRunData.value = { status: 'success', page: getEvaluationRunsResult.value };
      return getEvaluationRunsResult.value;
    } finally {
      clearTimeout(timeout);
    }
  }

  onMounted(getEvaluationRuns);

  function handlePreviousPage() {
    if (evaluationRunData.value.status !== 'success') {
      return;
    }

    getEvaluationRuns(evaluationRunData.value.page.pageNumber - 1);
  }

  function handleNextPage() {
    if (evaluationRunData.value.status !== 'success') {
      return;
    }

    getEvaluationRuns(evaluationRunData.value.page.pageNumber + 1);
  }

  function handleGotoPage(pageNumber: number) {
    if (evaluationRunData.value.status !== 'success') {
      return;
    }

    getEvaluationRuns(pageNumber);
  }

  async function handleTestEvaluationClick() {
    evaluationTestResult.value = { status: 'entering-input' };
    openDrawer();
  }

  async function handleTestClick() {
    if (!evaluation.value) {
      return;
    }

    evaluationTestResult.value = { status: 'loading' };

    const testResult = await evaluationsService.test({
      input: testInput.value,
      evaluation: evaluation.value,
    });

    if (testResult.failed) {
      console.error(testResult.error.message);
      evaluationTestResult.value = { status: 'failed' };
      return;
    }

    testInput.value = '';
    evaluationTestResult.value = { status: 'success', testResult: testResult.value };
  }

  async function handlePerformEvaluationClick() {
    evaluationTestResult.value = { status: 'idle' };
    openDrawer();
  }

  async function handleNewEvaluationRun() {
    closeDrawer();
    getEvaluationRuns();
  }

  async function deleteEvaluationRun(id: string) {
    const isSure = confirm('Are you sure you want to delete this evaluation run?');

    if (!isSure) {
      return;
    }

    const deleteEvaluationRunResult = await evaluationRunsService.delete(id);

    if (deleteEvaluationRunResult.failed) {
      console.error(deleteEvaluationRunResult.error.message);
      return;
    }

    getEvaluationRuns(evaluationRunDataPage.value);
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
              :disabled="isEditable === false"
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
              :disabled="isEditable === false"
            >
              <option value="null">Select Evaluation Type</option>
              <option>Unstructured Exact Match</option>
              <option>Unstructured Partial Match</option>
              <option>JSON Match</option>
            </select>
          </div>
          <div
            v-if="
              evaluation.successCriteria.type === 'Unstructured Exact Match' ||
              evaluation.successCriteria.type === 'Unstructured Partial Match'
            "
            class="form-group"
          >
            <label>Match Value</label>
            <textarea
              v-model="evaluation.successCriteria.matchValue"
              required
              rows="1"
              :disabled="isEditable === false"
            ></textarea>
          </div>
          <div
            v-else-if="evaluation.successCriteria.type === 'JSON Match'"
            class="form-group"
          >
            <label>JSON Schema</label>
            <TextEditor
              v-model="evaluation.successCriteria.schema"
              :disabled="isEditable === false"
            />
          </div>
        </div>
      </div>
      <div class="actions">
        <button
          type="button"
          @click="handleTestEvaluationClick"
          :disabled="!isValidEvaluation"
        >
          Test Evaluation
        </button>
        <button
          type="button"
          @click="handlePerformEvaluationClick"
          :disabled="!isValidEvaluation"
        >
          Perform Evaluation
        </button>
      </div>
    </div>
    <div class="evaluation-runs">
      <PagedTable
        :data="evaluationRunData"
        :columns="[
          'name',
          'status',
          'successRate',
          'expectedProportion',
          'confidenceLevel',
          'marginOfError',
          'sampleSize',
          'createdDate',
          'updatedDate',
        ]"
        :get-item-key="evaluationRun => evaluationRun.id"
        :build-edit-link="evaluationRun => `/evaluation-runs/${evaluationRun.id}`"
        :delete-item-handler="evaluationRun => deleteEvaluationRun(evaluationRun.id)"
        @previous-page="handlePreviousPage"
        @goto-page="handleGotoPage"
        @next-page="handleNextPage"
      />
    </div>
  </div>
  <SlideDrawer
    :heading="evaluationTestResult.status === 'success' ? 'Test Evaluation' : 'Perform Evaluation'"
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
      <PipelineRunCard
        :run="evaluationTestResult.testResult.pipelineRun"
        style="background-color: var(--background-color)"
      />
      <TraceViewer :run-id="evaluationTestResult.testResult.pipelineRun.id" />
    </div>
    <div
      class="test-input-container"
      v-else-if="
        evaluationTestResult.status === 'entering-input' ||
        evaluationTestResult.status === 'loading'
      "
    >
      <label for="testInput">Test Input</label>
      <textarea
        id="testInput"
        name="testInput"
        v-model="testInput"
        rows="10"
        required
      ></textarea>
      <button
        type="button"
        :disabled="!testInput"
        @click="handleTestClick"
      >
        <WaitingSpinner
          v-if="evaluationTestResult.status === 'loading'"
          height="1rem"
          width="1rem"
        />
        Test
      </button>
    </div>
    <div v-else>
      <GenericForm
        :fields="[
          {
            name: 'input',
            label: 'Input',
            type: 'textarea',
            required: true,
            rows: 10,
          },
          {
            name: 'expectedProportion',
            label: 'Expected Proportion of Runs to Pass (%)',
            type: 'number',
            required: true,
            min: 0,
            max: 100,
            step: 1,
            default: '50',
          },
          {
            name: 'confidenceLevel',
            label: 'Desired Confidence Level (%)',
            type: 'select',
            required: true,
            options: [
              { value: '80', label: '80' },
              { value: '85', label: '85' },
              { value: '90', label: '90' },
              { value: '95', label: '95' },
              { value: '99', label: '99' },
            ],
            default: '95',
          },
          {
            name: 'marginOfError',
            label: 'Desired Margin of Error (%)',
            type: 'number',
            required: true,
            min: 0,
            max: 100,
            step: 1,
            default: '5',
          },
        ]"
        :on-submit="
          async fields => {
            if (!evaluation) {
              return { failed: true, error: new Error('An evaluation is required') };
            }

            return await evaluationRunsService.create({
              input: fields.input,
              expectedProportion: parseInt(fields.expectedProportion),
              confidenceLevel: parseInt(fields.confidenceLevel),
              marginOfError: parseInt(fields.marginOfError),
              evaluation: evaluation,
            });
          }
        "
        @form-submitted="handleNewEvaluationRun"
        submit-button-label="Run Evaluation"
      />
    </div>
  </SlideDrawer>
</template>

<style scoped>
  .evaluation-container {
    display: flex;
    flex-direction: column;
    gap: 2rem;
    padding-bottom: 1rem;
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
    flex-direction: column;
    gap: 1rem;
    width: 100%;
  }

  .card {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    background-color: var(--secondary-background-color);
    padding: 1rem;
    border-radius: 0.25rem;
    width: 100%;
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

  .test-input-container {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    padding: 1rem;
  }

  .test-input-container label {
    font-weight: bold;
  }

  .test-input-container button {
    padding: 0.25rem 0.5rem;
    border: 1px solid var(--text-color);
    border-radius: 0.25rem;
    width: max-content;
    display: flex;
    align-items: center;
    gap: 0.25rem;
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
