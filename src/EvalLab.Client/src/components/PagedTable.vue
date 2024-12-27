<script setup lang="ts" generic="T">
  import { Page } from '../services/shared';
  import { convertToTitleCase, createDebouncedHandler } from '../shared/utils';
  import ChevronLeft from './icons/ChevronLeft.vue';
  import ChevronRight from './icons/ChevronRight.vue';
  import PencilIcon from './icons/PencilIcon.vue';
  import TrashCanIcon from './icons/TrashCanIcon.vue';
  import WaitingSpinner from './WaitingSpinner.vue';

  // TODO: Add search filter

  export type TableData<T> =
    | {
        status: 'initial' | 'loading' | 'error';
      }
    | {
        status: 'success';
        page: Page<T>;
      };

  type Props = {
    data: TableData<T>;
    getItemKey: (item: T) => string;
    buildEditLink: (item: T) => string;
    deleteItemHandler: (item: T) => void;
  };

  const props = defineProps<Props>();

  const emit = defineEmits<{
    (e: 'previous-page' | 'next-page'): void;
    (e: 'goto-page', pageNumber: number): void;
  }>();

  function handlePreviousPage() {
    emit('previous-page');
  }

  function handleNextPage() {
    emit('next-page');
  }

  function isValidInput(input: HTMLInputElement) {
    return (
      !input.validity.badInput &&
      !input.validity.rangeOverflow &&
      !input.validity.rangeUnderflow &&
      input.value !== ''
    );
  }

  const handlePageInput = createDebouncedHandler(async (e: Event) => {
    if (isValidInput(e.target as HTMLInputElement)) {
      const pageNumber = parseInt((e.target as HTMLInputElement).value);

      emit('goto-page', pageNumber);
    }
  }, 500);

  function handleInputBlur(e: FocusEvent) {
    const target = e.target as HTMLInputElement;
    const isInvalid = !isValidInput(target);

    if (isInvalid && props.data.status === 'success') {
      target.value = props.data.page.pageNumber.toString();
    }
  }
</script>

<template>
  <div class="table-container">
    <WaitingSpinner
      v-if="data.status === 'loading'"
      height="3rem"
      width="3rem"
    />
    <div v-else-if="data.status === 'error'">Failed to load evaluations</div>
    <div v-else-if="data.status === 'success'">
      <div v-if="data.page.items.length === 0">No evaluations found</div>
      <div v-else>
        <div class="pager">
          <button
            :disabled="data.page.pageNumber === 1"
            type="button"
            @click="handlePreviousPage"
          >
            <ChevronLeft />
            <span class="sr-only">Previous Page</span>
          </button>
          <div class="page-number-container">
            <label
              class="sr-only"
              for="currentPageNumber"
            >
              Current Page
            </label>
            <input
              id="currentPageNumber"
              name="currentPageNumber"
              :value="data.page.pageNumber"
              type="number"
              required
              :disabled="data.page.totalPages === 1"
              min="1"
              :max="data.page.totalPages"
              @input="handlePageInput"
              @blur="handleInputBlur"
            />
            <span>of</span>
            <span>{{ data.page.totalPages }}</span>
          </div>
          <button
            :disabled="data.page.pageNumber === data.page.totalPages"
            type="button"
            @click="handleNextPage"
          >
            <ChevronRight />
            <span class="sr-only">Next Page</span>
          </button>
        </div>
        <table>
          <thead>
            <tr>
              <th></th>
              <th
                v-for="key in Object.keys(data.page.items[0] as object)"
                :key="key"
              >
                {{ convertToTitleCase(key) }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="item in data.page.items as T[]"
              :key="props.getItemKey(item)"
            >
              <td>
                <div class="actions-container">
                  <RouterLink :to="props.buildEditLink(item)">
                    <PencilIcon />
                    <span class="sr-only">Edit</span>
                  </RouterLink>
                  <button
                    type="button"
                    @click="() => props.deleteItemHandler(item)"
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
  </div>
</template>

<style scoped>
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

  .table-container div:first-child {
    width: 100%;
  }

  .pager {
    display: flex;
    justify-content: center;
    gap: 0.5rem;
    max-width: fit-content;
    background-color: var(--secondary-background-color);
    padding: 0.5rem;
    margin: 0.5rem 0;
    border-radius: 0.25rem;
  }

  .pager button {
    background-color: var(--background-color);
    border: none;
    padding: 0.25rem 0.25rem;
    border-radius: 0.25rem;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 0.25rem;
  }

  .pager button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .pager button svg {
    width: 1rem;
    height: 1rem;
  }

  .page-number-container {
    display: flex;
    gap: 0.25rem;
    align-items: center;
  }

  .pager input {
    width: fit-content;
    text-align: center;
    border: none;
    background-color: var(--background-color);
    border-radius: 0.25rem;
    padding: 0.125rem;
  }

  .pager input:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .pager input::-webkit-outer-spin-button,
  .pager input::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
  }

  .pager input[type='number'] {
    -moz-appearance: textfield;
    appearance: textfield;
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
