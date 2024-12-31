<script setup lang="ts">
  import { defineEmits, Ref, UnwrapRef, useTemplateRef } from 'vue';
  import { useService } from '../../composables/useService.ts';
  import { EvaluationsServiceKey } from '../../services/evaluationService.ts';
  import GenericForm, { GenericFormComponent } from './GenericForm.vue';

  const form = useTemplateRef<GenericFormComponent>('form');

  export type AddEvaluationFormComponentWrapped = {
    form: Ref<GenericFormComponent | null>;
  };

  export type AddEvaluationFormComponent = UnwrapRef<AddEvaluationFormComponentWrapped>;

  defineExpose<AddEvaluationFormComponentWrapped>({
    form: form,
  });

  const emit = defineEmits<{
    (e: 'evaluation-added'): void;
  }>();

  const evaluationsService = useService(EvaluationsServiceKey);
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
        name: 'description',
        label: 'Description',
        required: false,
        type: 'textarea',
      },
    ]"
    :on-submit="
      async ({ name, description }) => {
        return await evaluationsService.create({
          name: name,
          description: description,
        });
      }
    "
    @form-submitted="emit('evaluation-added')"
    submit-button-label="Add"
  />
</template>
