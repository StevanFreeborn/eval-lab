<script setup lang="ts">
  import { ref, useTemplateRef } from 'vue';
  import AddButton from '../components/buttons/AddButton.vue';
  import AddPipelineForm, {
    AddPipelineFormComponent,
  } from '../components/forms/AddPipelineForm.vue';
  import SlideDrawer from '../components/SlideDrawer.vue';

  const form = useTemplateRef<AddPipelineFormComponent>('form');

  const drawerOpen = ref(false);

  function closeDrawer() {
    drawerOpen.value = false;
  }

  function openDrawer() {
    if (form.value?.form?.fieldRefs) {
      form.value?.form.fieldRefs[0]?.focus();
    }

    drawerOpen.value = true;
  }

  function handleNewPipeline() {
    closeDrawer();
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
  </div>
  <SlideDrawer
    heading="Add Pipeline"
    :drawer-open="drawerOpen"
    @drawer-closed="closeDrawer"
  >
    <AddPipelineForm
      ref="form"
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
