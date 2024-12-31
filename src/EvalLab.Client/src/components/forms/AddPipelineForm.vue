<script setup lang="ts">
  import { defineEmits, Ref, UnwrapRef, useTemplateRef } from 'vue';
  import { useService } from '../../composables/useService.ts';
  import { PipelinesServiceKey } from '../../services/pipelineService';
  import GenericForm, { GenericFormComponent } from './GenericForm.vue';

  const form = useTemplateRef<GenericFormComponent>('form');

  export type AddPipelineFormComponentWrapped = {
    form: Ref<GenericFormComponent | null>;
  };

  export type AddPipelineFormComponent = UnwrapRef<AddPipelineFormComponentWrapped>;

  defineExpose<AddPipelineFormComponentWrapped>({
    form: form,
  });

  const emit = defineEmits<{
    (e: 'pipeline-added'): void;
  }>();

  const pipelinesService = useService(PipelinesServiceKey);
</script>

<template>
  <GenericForm
    ref="form"
    :fields="[
      {
        name: 'name',
        label: 'Name',
        required: true,
        type: 'text',
      },
      {
        name: 'endpoint',
        label: 'Endpoint',
        required: true,
        type: 'text',
      },
      {
        name: 'description',
        label: 'Description',
        required: false,
        type: 'textarea',
      },
    ]"
    :on-submit="
      async ({ name, endpoint, description }) => {
        return await pipelinesService.create({
          name: name,
          description: description,
          endpoint: endpoint,
        });
      }
    "
    @form-submitted="emit('pipeline-added')"
    submit-button-label="Add"
  />
</template>
