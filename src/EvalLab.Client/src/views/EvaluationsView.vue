<script setup lang="ts">
  import PencilIcon from '@/components/icons/PencilIcon.vue';
  import TrashCanIcon from '@/components/icons/TrashCanIcon.vue';
  import CirclePlusIcon from '@/components/icons/CirclePlusIcon.vue';
  import { RouterLink } from 'vue-router';
  import { ref } from 'vue';

  const drawerOpen = ref(false);

  const data = ref({
    items: [
      { id: 1, name: 'First evaluation', createdDate: '2022-01-01' },
      { id: 2, name: 'Second evaluation', createdDate: '2022-01-02' },
      { id: 3, name: 'Third evaluation', createdDate: '2022-01-03' },
      { id: 4, name: 'Fourth evaluation', createdDate: '2022-01-04' },
      { id: 5, name: 'Fifth evaluation', createdDate: '2022-01-05' },
      { id: 6, name: 'Sixth evaluation', createdDate: '2022-01-06' },
      { id: 7, name: 'Seventh evaluation', createdDate: '2022-01-07' },
      { id: 8, name: 'Eighth evaluation', createdDate: '2022-01-08' },
      { id: 9, name: 'Ninth evaluation', createdDate: '2022-01-09' },
      { id: 10, name: 'Tenth evaluation', createdDate: '2022-01-10' },
      { id: 1, name: 'First evaluation', createdDate: '2022-01-01' },
      { id: 2, name: 'Second evaluation', createdDate: '2022-01-02' },
      { id: 3, name: 'Third evaluation', createdDate: '2022-01-03' },
      { id: 4, name: 'Fourth evaluation', createdDate: '2022-01-04' },
      { id: 5, name: 'Fifth evaluation', createdDate: '2022-01-05' },
      { id: 6, name: 'Sixth evaluation', createdDate: '2022-01-06' },
      { id: 7, name: 'Seventh evaluation', createdDate: '2022-01-07' },
      { id: 8, name: 'Eighth evaluation', createdDate: '2022-01-08' },
      { id: 9, name: 'Ninth evaluation', createdDate: '2022-01-09' },
      { id: 10, name: 'Tenth evaluation', createdDate: '2022-01-10' },
      { id: 1, name: 'First evaluation', createdDate: '2022-01-01' },
      { id: 2, name: 'Second evaluation', createdDate: '2022-01-02' },
      { id: 3, name: 'Third evaluation', createdDate: '2022-01-03' },
      { id: 4, name: 'Fourth evaluation', createdDate: '2022-01-04' },
      { id: 5, name: 'Fifth evaluation', createdDate: '2022-01-05' },
      { id: 6, name: 'Sixth evaluation', createdDate: '2022-01-06' },
      { id: 7, name: 'Seventh evaluation', createdDate: '2022-01-07' },
      { id: 8, name: 'Eighth evaluation', createdDate: '2022-01-08' },
      { id: 9, name: 'Ninth evaluation', createdDate: '2022-01-09' },
      { id: 10, name: 'Tenth evaluation', createdDate: '2022-01-10' },
    ],
  });

  function convertToTitleCase(str: string) {
    return str.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
  }
</script>

<template>
  <div class="container">
    <div class="heading-container">
      <h2>Evaluations</h2>
      <button
        type="button"
        @click="
          () => {
            console.log('Create new evaluation');
            drawerOpen = !drawerOpen;
          }
        "
      >
        <CirclePlusIcon />
      </button>
    </div>
    <div class="table-container">
      <table>
        <thead>
          <tr>
            <th></th>
            <th
              v-for="(value, key) in data.items[0]"
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
              </RouterLink>
              <button
                type="button"
                @click="() => console.log('Delete', item.id)"
              >
                <TrashCanIcon />
              </button>
            </td>
            <td
              v-for="value in item"
              :key="value"
            >
              {{ value }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  <div :class="{ drawer: true, open: drawerOpen }">
    <h3>Create new evaluation</h3>
  </div>
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

  .drawer {
    position: absolute;
    top: 0;
    right: 0;
    bottom: 0;
    width: 0px;
    background-color: var(--secondary-background-color);
    transition: width 0.5s;
    z-index: 2;
    overflow: hidden;
    border-top-left-radius: 0.5rem;
    border-bottom-left-radius: 0.5rem;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);

    h3 {
      margin: 0.5rem;
    }
  }

  .drawer.open {
    width: 300px;
  }
</style>
