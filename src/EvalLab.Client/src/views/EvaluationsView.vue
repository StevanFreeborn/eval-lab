<script setup lang="ts">
  import PencilIcon from '../components/icons/PencilIcon.vue';
  import TrashCanIcon from '../components/icons/TrashCanIcon.vue';
  import CirclePlusIcon from '../components/icons/CirclePlusIcon.vue';
  import AddEvaluationForm from '../components/forms/AddEvaluationForm.vue';
  import { RouterLink } from 'vue-router';
  import { ref, onMounted } from 'vue';
  import SlideDrawer from '../components/SlideDrawer.vue';
  import { Evaluation, EvaluationsServiceKey } from '../services/evaluationService.ts';
  import { useService } from '../composables/useService.ts';
  import { convertToTitleCase } from '../shared/utils.ts';
  import WaitingSpinner from '../components/WaitingSpinner.vue';

  const drawerOpen = ref(false);

  const data = ref<{
    items: Evaluation[];
    status: 'loading' | 'error' | 'success';
  }>({
    items: [],
    status: 'loading',
  });

  const evaluationsService = useService(EvaluationsServiceKey);

  async function getEvaluations() {
    const getEvaluationsResult = await evaluationsService.getEvaluations();

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
    drawerOpen.value = true;
  }

  function closeDrawer() {
    drawerOpen.value = false;
  }

  function handleNewEvaluation() {
    getEvaluations();
    closeDrawer();
  }
</script>

<template>
  <div class="container">
    <div class="heading-container">
      <h2>Evaluations</h2>
      <button
        type="button"
        @click="openDrawer"
      >
        <CirclePlusIcon />
        <span class="sr-only">Add Evaluation</span>
      </button>
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
            <td class="actions-container">
              <RouterLink :to="`/evaluations/${item.id}/edit`">
                <PencilIcon />
                <span class="sr-only">Edit</span>
              </RouterLink>
              <button
                type="button"
                @click="() => console.log('Delete', item.id)"
              >
                <TrashCanIcon />
                <span class="sr-only">Delete</span>
              </button>
            </td>
            <td
              v-for="(value, name) in item"
              :key="name"
            >
              {{ value }}
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
    <AddEvaluationForm @evaluation-added="handleNewEvaluation" />
  </SlideDrawer>
</template>

<style scoped>
  .container {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    overflow: hidden;
  }

  .heading-container {
    display: flex;
    align-items: center;
    gap: 0.5rem;

    button {
      background-color: var(--secondary-background-color);
      border: 1px solid var(--text-color);
      border-radius: 0.25rem;
      padding: 0.25rem;
      cursor: pointer;
    }

    svg {
      display: block;
      width: 1rem;
      height: 1rem;
    }
  }

  .table-container {
    display: flex;
    flex-direction: column;
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
