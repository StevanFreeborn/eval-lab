<script setup lang="ts" generic="T">
  import {
    computed,
    defineModel,
    nextTick,
    onMounted,
    onUnmounted,
    ref,
    useTemplateRef,
    watch,
  } from 'vue';
  import { GetAllParams, Page, Result } from '../../services/shared.ts';
  import { createDebouncedHandler } from '../../shared/utils';
  import WaitingSpinner from '../WaitingSpinner.vue';

  // TODO: Revisit accessibility of this component

  export type Option = {
    id: string;
    name: string;
  };

  const selectedOptionId = defineModel<string>();

  type Props = {
    getOptions: (params: GetAllParams) => Promise<Result<Page<T>>>;
    mapOption: (item: T) => Option;
    placeholder: string;
    searchPlaceholder: string;
    required?: boolean;
    disabled?: boolean;
    pageSize?: number;
    sortBy?: keyof T;
    sortOrder?: 'asc' | 'desc';
  };

  const props = defineProps<Props>();

  const pages = ref<Page<T>[]>([]);
  const currentPage = computed(() => pages.value.at(-1));
  const options = computed(() =>
    pages.value?.flatMap(p => p.items.flatMap(i => props.mapOption(i as T))),
  );
  const selectedOption = computed(
    () =>
      options.value.find(o => o.id === selectedOptionId.value) ?? {
        id: '',
        name: props.placeholder,
      },
  );
  const nameFilter = ref<string>('');
  const loading = ref<boolean>(false);

  const hasMore = computed(() => {
    if (currentPage.value === undefined) {
      return false;
    }

    return currentPage.value.pageNumber < currentPage.value.totalPages;
  });

  async function getNextPage() {
    const timeout = setTimeout(() => {
      loading.value = true;
    }, 500);

    try {
      const page = currentPage.value ? currentPage.value.pageNumber : 0;
      const nextPage = page + 1;
      const result = await props.getOptions({
        pageNumber: nextPage,
        pageSize: props.pageSize,
        sortBy: props.sortBy?.toString(),
        sortOrder: props.sortOrder,
        additionalParams: { name: nameFilter.value },
      });

      if (result.failed) {
        console.error(result.error.message);
        return;
      }

      pages.value.push(result.value as never);
    } finally {
      clearTimeout(timeout);
      loading.value = false;
    }
  }

  onMounted(getNextPage);

  const selectedOptionRef = useTemplateRef<HTMLButtonElement | null>('selectedOptionRef');
  const filterInputRef = useTemplateRef<HTMLInputElement | null>('filter');
  const dropdownRef = useTemplateRef<HTMLDivElement | null>('dropdownRef');
  const menuOpen = ref(false);

  function handleClickOutsideMenu(e: MouseEvent) {
    if (dropdownRef.value && menuOpen.value && !dropdownRef.value.contains(e.target as Node)) {
      menuOpen.value = false;
    }
  }

  onMounted(() => document.addEventListener('click', handleClickOutsideMenu));
  onUnmounted(() => document.removeEventListener('click', handleClickOutsideMenu));

  watch(menuOpen, async value => {
    if (value) {
      await nextTick();
      filterInputRef.value?.focus();
    }
  });

  function handleOptionClick(option: Option) {
    selectedOptionId.value = option ? option.id : '';
    menuOpen.value = false;
    selectedOptionRef.value?.focus();
  }

  function handleSelectedOptionClick() {
    menuOpen.value = !menuOpen.value;
  }

  const handleFilterInput = createDebouncedHandler(async (value: string) => {
    pages.value = [];
    nameFilter.value = value;
    await getNextPage();
  });
</script>

<template>
  <div
    ref="dropdownRef"
    class="dropdown"
  >
    <button
      ref="selectedOptionRef"
      type="button"
      :class="{ 'selected-option': true, required }"
      @click="handleSelectedOptionClick"
      :disabled="props.disabled"
    >
      {{ !selectedOption?.id ? placeholder : selectedOption.name }}
    </button>
    <div :class="{ menu: true, open: menuOpen }">
      <label
        for="filter"
        class="sr-only"
      >
        Filter
      </label>
      <input
        ref="filter"
        class="filter"
        type="text"
        name="filter"
        id="filter"
        :placeholder="searchPlaceholder"
        @input="e => handleFilterInput((e.target as HTMLInputElement).value)"
      />
      <button
        :class="{
          option: true,
          selected: selectedOption?.id == '',
        }"
        type="button"
        @click="
          () =>
            handleOptionClick({
              id: '',
              name: placeholder,
            })
        "
      >
        {{ placeholder }}
      </button>
      <WaitingSpinner
        v-if="loading && !currentPage"
        height="3rem"
        width="3rem"
        style="align-self: center"
      />
      <div
        class="displayed-options"
        v-else
      >
        <ul>
          <li
            v-for="option in options"
            :key="option.id"
          >
            <button
              :class="{
                option: true,
                selected: selectedOption?.id == option.id,
              }"
              type="button"
              @click="() => handleOptionClick(option)"
            >
              {{ option.name }}
            </button>
          </li>
        </ul>
        <button
          v-if="hasMore"
          class="load-more-button"
          type="button"
          @click="getNextPage"
        >
          <WaitingSpinner
            v-if="loading"
            height="1rem"
            width="1rem"
          />
          Load More
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
  .dropdown {
    position: relative;
    width: 100%;
  }

  .selected-option,
  .option {
    text-align: left;
    width: 100%;
    background-color: var(--background-color);
    padding: 0.5rem 1rem;
    border-radius: 0.25rem;
  }

  .option {
    border: 1px solid transparent;
  }

  .option.selected {
    background-color: var(--secondary-background-color);
    border: 1px solid var(--background-color);
  }

  .selected-option:focus {
    outline: -webkit-focus-ring-color auto 1px;
  }

  .required {
    border-left: 1px solid red;
  }

  .menu {
    display: none;
    position: absolute;
    z-index: 1;
    max-height: 380px;
  }

  .menu.open {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    top: 120%;
    left: 0;
    width: 100%;
    background-color: var(--secondary-background-color);
    border-radius: 0.25rem;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
    padding: 0.5rem;
  }

  .filter {
    width: 100%;
    padding: 0.25rem;
    border-radius: 0.25rem;
  }

  .displayed-options {
    display: flex;
    flex-direction: column;
    overflow-y: auto;
  }

  ul {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .load-more-button {
    padding: 0.25rem 0.5rem;
    border: 1px solid var(--text-color);
    border-radius: 999px;
    width: max-content;
    display: flex;
    align-items: center;
    gap: 0.25rem;
    align-self: center;
    margin-top: -0.25rem;
  }
</style>
