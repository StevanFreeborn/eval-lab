<script setup lang="ts">
  import { computed, onMounted, ref, useTemplateRef } from 'vue';
  import PagedTable, { TableData } from '../components/PagedTable.vue';
  import SlideDrawer from '../components/SlideDrawer.vue';
  import AddButton from '../components/buttons/AddButton.vue';
  import AddEvaluationForm, {
    AddEvaluationFormComponent,
  } from '../components/forms/AddEvaluationForm.vue';
  import { useService } from '../composables/useService.ts';
  import { Evaluation, EvaluationsServiceKey } from '../services/evaluationService.ts';

  const drawerOpen = ref(false);

  const addForm = useTemplateRef<AddEvaluationFormComponent>('addForm');

  const data = ref<TableData<Evaluation>>({ status: 'initial' });
  const currentPage = computed(() =>
    data.value.status === 'success' ? data.value.page.pageNumber : 1,
  );

  const evaluationsService = useService(EvaluationsServiceKey);

  async function getEvaluations(pageNumber?: number, pageSize?: number) {
    const timeout = setTimeout(() => {
      data.value.status = 'loading';
    }, 500);

    try {
      const getEvaluationsResult = await evaluationsService.getAll({ pageNumber, pageSize });

      if (getEvaluationsResult.failed) {
        console.error(getEvaluationsResult.error.message);
        data.value.status = 'error';
        return;
      }

      data.value = { status: 'success', page: getEvaluationsResult.value };
      return getEvaluationsResult.value;
    } finally {
      clearTimeout(timeout);
    }
  }

  onMounted(getEvaluations);

  function openDrawer() {
    if (addForm.value?.form?.fieldRefs) {
      addForm.value?.form?.fieldRefs[0]?.focus();
    }

    drawerOpen.value = true;
  }

  function closeDrawer() {
    drawerOpen.value = false;
  }

  function handleNewEvaluation() {
    getEvaluations();
    closeDrawer();
  }

  async function deleteEvaluation(id: string) {
    const isSure = confirm('Are you sure you want to delete this evaluation?');

    if (!isSure) {
      return;
    }

    const deleteEvaluationResult = await evaluationsService.delete(id);

    if (deleteEvaluationResult.failed) {
      console.error(deleteEvaluationResult.error.message);
      return;
    }

    getEvaluations(currentPage.value);
  }

  function handlePreviousPage() {
    if (data.value.status !== 'success') {
      return;
    }

    getEvaluations(data.value.page.pageNumber - 1);
  }

  function handleNextPage() {
    if (data.value.status !== 'success') {
      return;
    }

    getEvaluations(data.value.page.pageNumber + 1);
  }

  function handleGotoPage(pageNumber: number) {
    if (data.value.status !== 'success') {
      return;
    }

    getEvaluations(pageNumber);
  }
</script>

<template>
  <div class="container">
    <div class="heading-container">
      <h2>Evaluations</h2>
      <AddButton
        label="Add Evaluation"
        type="button"
        @clicked="openDrawer"
      />
    </div>
    <PagedTable
      :data="data"
      :columns="['id', 'name', 'description', 'createdDate', 'updatedDate']"
      :get-item-key="evaluation => evaluation.id"
      :build-edit-link="evaluation => `/evaluations/${evaluation.id}`"
      :delete-item-handler="evaluation => deleteEvaluation(evaluation.id)"
      @previous-page="handlePreviousPage"
      @goto-page="handleGotoPage"
      @next-page="handleNextPage"
    />
  </div>
  <SlideDrawer
    heading="Add Evaluation"
    :drawer-open="drawerOpen"
    @drawer-closed="closeDrawer"
  >
    <AddEvaluationForm
      ref="addForm"
      @evaluation-added="handleNewEvaluation"
    />
  </SlideDrawer>
</template>

<style scoped>
  .container {
    display: flex;
    flex-direction: column;
    flex: 1;
    gap: 1rem;
    overflow: hidden;
  }

  .heading-container {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }

  .table-container {
    display: flex;
    flex-direction: column;
    flex: 1;
    align-items: center;
    --border: 1px solid var(--text-color);
    overflow-y: auto;
    scrollbar-width: thin;
    scrollbar-color: var(--text-color) var(--background-color);
  }

  table {
    width: 100%;
    border-collapse: separate;
    border-spacing: 0;
  }

  thead th {
    position: sticky;
    top: 0;
    z-index: 1;
  }

  th,
  td {
    border-left: var(--border);
    padding: 0.5rem;
  }

  th {
    background-color: var(--secondary-background-color);
    text-align: left;
    border-top: var(--border);
    border-bottom: var(--border);
  }

  th:first-child {
    max-width: 1rem;
  }

  th:last-child {
    border-right: var(--border);
  }

  td:last-child {
    border-right: var(--border);
  }

  tr:nth-child(even) {
    background-color: var(--secondary-background-color);
  }

  tr:nth-child(odd) {
    background-color: var(--background-color);
  }

  tr:last-child td {
    border-bottom: var(--border);
  }

  tr:last-child td:first-child {
    border-bottom-left-radius: 0.25rem;
  }

  tr:last-child td:last-child {
    border-bottom-right-radius: 0.25rem;
  }

  th:first-child {
    width: 1%;
  }

  .actions-container {
    display: flex;
    gap: 0.5rem;

    svg {
      width: 1rem;
      height: 1rem;
    }
  }
</style>
