<script setup lang="ts">
  import { defineEmits, ref, Ref, UnwrapRef, useTemplateRef } from 'vue';
  import WaitingSpinner from '../WaitingSpinner.vue';

  type FormField = {
    value: string;
    error: string;
  };

  type FormData = {
    [key: string]: FormField;
  };

  interface FormFieldDefinition {
    name: string;
    label: string;
    type: 'text' | 'textarea';
    required?: boolean;
    rows?: number;
  }

  type FieldNames<T extends readonly FormFieldDefinition[]> = {
    [K in T[number]['name']]: string;
  };

  interface Props<T extends readonly FormFieldDefinition[]> {
    fields: T;
    onSubmit: (data: FieldNames<T>) => Promise<{ failed: boolean; error?: { message: string } }>;
    submitButtonLabel: string;
  }

  const props = defineProps<Props<readonly FormFieldDefinition[]>>();
  const firstField = useTemplateRef<HTMLElement[]>('firstField');

  type GenericFormComponentWrapped = {
    fieldRefs: Ref<HTMLElement[] | null>;
  };

  export type GenericFormComponent = UnwrapRef<GenericFormComponentWrapped>;

  defineExpose<GenericFormComponentWrapped>({
    fieldRefs: firstField,
  });

  const emit = defineEmits<{
    (e: 'form-submitted'): void;
  }>();

  const errors = ref<string[]>([]);

  const formData = ref<FormData>(
    (() => {
      const data: FormData = {};
      props.fields.forEach(field => {
        data[field.name] = {
          value: '',
          error: '',
        };
      });
      return data;
    })(),
  );

  function clearFormErrors() {
    errors.value = [];
    props.fields.forEach(field => {
      formData.value[field.name].error = '';
    });
  }

  function clearFormValues() {
    props.fields.forEach(field => {
      formData.value[field.name].value = '';
    });
  }

  const isSubmitting = ref(false);

  async function handleSubmit() {
    clearFormErrors();

    let hasErrors = false;

    for (const field of props.fields) {
      if (field.required && !formData.value[field.name].value) {
        formData.value[field.name].error = `${field.label.toLowerCase()} is required`;
        hasErrors = true;
      }
    }

    if (hasErrors) {
      return;
    }

    isSubmitting.value = true;

    const payload = props.fields.reduce(
      (acc, field) => {
        acc[field.name] = formData.value[field.name].value;
        return acc;
      },
      {} as FieldNames<typeof props.fields>,
    );

    const result = await props.onSubmit(payload);

    isSubmitting.value = false;

    if (result.failed) {
      errors.value.push(result.error?.message ?? 'An unknown error occurred');
      return;
    }

    clearFormValues();
    emit('form-submitted');
  }
</script>

<template>
  <form
    @submit.prevent="handleSubmit"
    novalidate
  >
    <div
      v-for="(field, index) in fields"
      :key="field.name"
      class="form-group"
    >
      <label :for="field.name">{{ field.label }}</label>

      <input
        v-if="field.type === 'text'"
        :ref="index === 0 ? 'firstField' : undefined"
        v-model="formData[field.name].value"
        type="text"
        :id="field.name"
        :name="field.name"
        :required="field.required"
      />

      <textarea
        v-else-if="field.type === 'textarea'"
        :ref="index === 0 ? 'firstField' : undefined"
        v-model="formData[field.name].value"
        :id="field.name"
        :name="field.name"
        :rows="field.rows || 10"
        :required="field.required"
      ></textarea>

      <p class="error">{{ formData[field.name].error }}</p>
    </div>

    <button type="submit">
      <WaitingSpinner
        v-if="isSubmitting"
        height="1rem"
        width="1rem"
      />
      {{ submitButtonLabel }}
    </button>

    <ul>
      <li
        class="error"
        v-for="error in errors"
        :key="error"
      >
        {{ error }}
      </li>
    </ul>
  </form>
</template>

<style scoped>
  form {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    padding: 0.75rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  label {
    font-weight: bold;
  }

  input,
  textarea {
    padding: 0.25rem;
    border-radius: 0.25rem;
  }

  textarea {
    resize: vertical;
  }

  button {
    padding: 0.25rem 0.5rem;
    border: 1px solid var(--text-color);
    border-radius: 0.25rem;
    cursor: pointer;
    width: max-content;
    display: flex;
    align-items: center;
    gap: 0.25rem;
  }

  input:required {
    border-left: 1px solid red;
  }

  .error {
    color: red;
    font-size: 0.75rem;
  }
</style>
