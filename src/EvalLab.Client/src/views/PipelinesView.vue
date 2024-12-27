<script setup lang="ts">
  import { onMounted, ref, useTemplateRef } from 'vue';
  import AddButton from '../components/buttons/AddButton.vue';
  import AddPipelineForm, {
    AddPipelineFormComponent,
  } from '../components/forms/AddPipelineForm.vue';
  import PagedTable, { TableData } from '../components/PagedTable.vue';
  import SlideDrawer from '../components/SlideDrawer.vue';
  import { useService } from '../composables/useService.ts';
  import { Pipeline, PipelinesServiceKey } from '../services/pipelineService.ts';

  const drawerOpen = ref(false);

  const addForm = useTemplateRef<AddPipelineFormComponent>('addForm');

  const data = ref<TableData<Pipeline>>({ status: 'initial' });

  const pipelinesService = useService(PipelinesServiceKey);

  async function getPipelines(pageNumber?: number, pageSize?: number) {
    const timeout = setTimeout(() => {
      data.value.status = 'loading';
    }, 500);

    try {
      const getPipelinesResult = await pipelinesService.getAll(pageNumber, pageSize);

      if (getPipelinesResult.failed) {
        console.error(getPipelinesResult.error.message);
        data.value.status = 'error';
        return;
      }

      data.value = { status: 'success', page: getPipelinesResult.value };
      return getPipelinesResult.value;
    } finally {
      clearTimeout(timeout);
    }
  }

  onMounted(getPipelines);

  function closeDrawer() {
    drawerOpen.value = false;
  }

  function openDrawer() {
    if (addForm.value?.form?.fieldRefs) {
      addForm.value?.form.fieldRefs[0]?.focus();
    }

    drawerOpen.value = true;
  }

  function handleNewPipeline() {
    getPipelines();
    closeDrawer();
  }

  async function deletePipeline(id: string) {
    const isSure = confirm('Are you sure you want to delete this pipeline?');

    if (!isSure) {
      return;
    }

    const deletePipelineResult = await pipelinesService.delete(id);

    if (deletePipelineResult.failed) {
      console.error(deletePipelineResult.error.message);
      return;
    }

    getPipelines();
  }

  function handlePreviousPage() {
    if (data.value.status !== 'success') {
      return;
    }

    getPipelines(data.value.page.pageNumber - 1);
  }

  function handleNextPage() {
    if (data.value.status !== 'success') {
      return;
    }

    getPipelines(data.value.page.pageNumber + 1);
  }

  function handleGotoPage(pageNumber: number) {
    if (data.value.status !== 'success') {
      return;
    }

    getPipelines(pageNumber);
  }
</script>

<template>
  <div class="container">
    <div class="heading-container">
      <h2>Pipelines</h2>
      <AddButton
        label="Add Pipeline"
        type="button"
        @clicked="openDrawer"
      />
    </div>
    <PagedTable
      :data="data"
      :get-item-key="pipeline => pipeline.id"
      :build-edit-link="pipeline => `/pipelines/${pipeline.id}`"
      :delete-item-handler="pipeline => deletePipeline(pipeline.id)"
      @previous-page="handlePreviousPage"
      @goto-page="handleGotoPage"
      @next-page="handleNextPage"
    />
  </div>
  <SlideDrawer
    heading="Add Pipeline"
    :drawer-open="drawerOpen"
    @drawer-closed="closeDrawer"
  >
    <AddPipelineForm
      ref="addForm"
      @pipeline-added="handleNewPipeline"
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
</style>
