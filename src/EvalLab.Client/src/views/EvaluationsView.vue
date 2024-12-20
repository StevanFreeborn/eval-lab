<script setup lang="ts">
  import { onMounted, ref, useTemplateRef } from 'vue';
  import { RouterLink } from 'vue-router';
  import SlideDrawer from '../components/SlideDrawer.vue';
  import WaitingSpinner from '../components/WaitingSpinner.vue';
  import AddButton from '../components/buttons/AddButton.vue';
  import AddEvaluationForm, {
    AddEvaluationFormComponent,
  } from '../components/forms/AddEvaluationForm.vue';
  import PencilIcon from '../components/icons/PencilIcon.vue';
  import TrashCanIcon from '../components/icons/TrashCanIcon.vue';
  import { useService } from '../composables/useService.ts';
  import { Evaluation, EvaluationsServiceKey } from '../services/evaluationService.ts';
  import { convertToTitleCase } from '../shared/utils.ts';

  const drawerOpen = ref(false);

  const addForm = useTemplateRef<AddEvaluationFormComponent>('addForm');

  const data = ref<{
    items: Evaluation[];
    status: 'loading' | 'error' | 'success';
  }>({
    items: [],
    status: 'loading',
  });

  const evaluationsService = useService(EvaluationsServiceKey);

  async function getEvaluations() {
    const getEvaluationsResult = await evaluationsService.getAll();

    if (getEvaluationsResult.failed) {
      console.error(getEvaluationsResult.error.message);
      data.value.status = 'error';
      return;
    }

    data.value.items = getEvaluationsResult.value.items;
    data.value.status = 'success';
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

    getEvaluations();
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
    <div class="table-container">
      <WaitingSpinner
        v-if="data.status === 'loading'"
        height="3rem"
        width="3rem"
      />
      <div v-else-if="data.status === 'error'">Failed to load evaluations</div>
      <div v-else-if="data.items.length === 0">No evaluations found</div>
      <table v-else>
        <thead>
          <tr>
            <th></th>
            <th
              v-for="(_, key) in data.items[0]"
              :key="key"
            >
              {{ convertToTitleCase(key) }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="item in data.items"
            :key="item.id"
          >
            <td>
              <div class="actions-container">
                <RouterLink :to="`/evaluations/${item.id}/edit`">
                  <PencilIcon />
                  <span class="sr-only">Edit</span>
                </RouterLink>
                <button
                  type="button"
                  @click="() => deleteEvaluation(item.id)"
                >
                  <TrashCanIcon />
                  <span class="sr-only">Delete</span>
                </button>
              </div>
            </td>
            <td
              v-for="(value, name) in item"
              :key="name"
            >
              {{ value instanceof Date ? value.toLocaleDateString() : value }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
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
