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

  type BaseFormFieldDefinition = {
    name: string;
    label: string;
    required?: boolean;
  };

  type TextFieldDefinition = BaseFormFieldDefinition & {
    type: 'text';
  };

  type TextAreaFieldDefinition = BaseFormFieldDefinition & {
    type: 'textarea';
    rows?: number;
  };

  type NumberFieldDefinition = BaseFormFieldDefinition & {
    type: 'number';
    min?: number;
    max?: number;
    step?: number;
  };

  type SelectFieldDefinition = BaseFormFieldDefinition & {
    type: 'select';
    options: { label: string; value: string }[];
  };

  type FormFieldDefinition =
    | TextFieldDefinition
    | TextAreaFieldDefinition
    | NumberFieldDefinition
    | SelectFieldDefinition;

  type FieldNames<T extends readonly FormFieldDefinition[]> = {
    [K in T[number]['name']]: string;
  };

  type Props<T extends readonly FormFieldDefinition[]> = {
    fields: T;
    onSubmit: (data: FieldNames<T>) => Promise<{ failed: boolean; error?: { message: string } }>;
    submitButtonLabel: string;
  };

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

      if (field.type === 'number') {
        const value = parseFloat(formData.value[field.name].value);

        if (Number.isNaN(value)) {
          formData.value[field.name].error = 'Please enter a valid number';
          hasErrors = true;
        }

        if (field.min !== undefined && value < field.min) {
          formData.value[field.name].error =
            `Please enter a number greater than or equal to ${field.min}`;
          hasErrors = true;
        }

        if (field.max !== undefined && value > field.max) {
          formData.value[field.name].error =
            `Please enter a number less than or equal to ${field.max}`;
          hasErrors = true;
        }
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

      <input
        v-else-if="field.type === 'number'"
        :ref="index === 0 ? 'firstField' : undefined"
        v-model="formData[field.name].value"
        type="number"
        :id="field.name"
        :name="field.name"
        :min="field.min"
        :max="field.max"
        :step="field.step"
        :required="field.required"
      />

      <select
        v-else-if="field.type === 'select'"
        :ref="index === 0 ? 'firstField' : undefined"
        v-model="formData[field.name].value"
        :id="field.name"
        :name="field.name"
        :required="field.required"
      >
        <option
          v-for="option in field.options"
          :key="option.value"
          :value="option.value"
        >
          {{ option.label }}
        </option>
      </select>

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

  .error {
    color: red;
    font-size: 0.75rem;
  }
</style>
