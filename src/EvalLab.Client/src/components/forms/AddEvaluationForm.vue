<script setup lang="ts">
  import { ref, defineEmits } from 'vue';
  import { EvaluationsServiceKey } from '../../services/evaluationService.ts';
  import { useService } from '../../composables/useService.ts';
  import WaitingSpinner from '../WaitingSpinner.vue';

  const emit = defineEmits<{
    (e: 'evaluation-added'): void;
  }>();

  type NewEvaluationFormData = {
    name: {
      value: string;
      error: string;
    };
    description: {
      value: string;
      error: string;
    };
    errors: string[];
  };

  const newEvaluation = ref<NewEvaluationFormData>({
    name: {
      value: '',
      error: '',
    },
    description: {
      value: '',
      error: '',
    },
    errors: [],
  });

  const evaluationsService = useService(EvaluationsServiceKey);

  const isSubmitting = ref(false);

  async function handleSubmit() {
    newEvaluation.value.errors = [];
    newEvaluation.value.name.error = '';
    newEvaluation.value.description.error = '';

    if (!newEvaluation.value.name.value) {
      newEvaluation.value.name.error = 'Please enter a name for the evaluation';
      return;
    }

    isSubmitting.value = true;

    const addEvaluationResult = await evaluationsService.createEvaluation({
      name: newEvaluation.value.name.value,
      description: newEvaluation.value.description.value,
    });

    isSubmitting.value = false;

    if (addEvaluationResult.failed) {
      newEvaluation.value.errors.push('Failed to create evaluation');
      console.error(addEvaluationResult.error.message);
      return;
    }

    newEvaluation.value.name.value = '';
    newEvaluation.value.description.value = '';
    emit('evaluation-added');
  }
</script>

<template>
  <form
    @submit.prevent="handleSubmit"
    novalidate
    class="add-evaluation-form"
  >
    <div class="form-group">
      <label for="name">Name</label>
      <input
        v-model="newEvaluation.name.value"
        type="text"
        id="name"
        name="name"
        required
        autofocus
      />
      <p class="error">{{ newEvaluation.name.error }}</p>
    </div>
    <div class="form-group">
      <label for="description">Description</label>
      <textarea
        v-model="newEvaluation.description.value"
        id="description"
        name="description"
        rows="10"
      ></textarea>
      <p class="error">{{ newEvaluation.description.error }}</p>
    </div>
    <button type="submit">
      <WaitingSpinner
        v-if="isSubmitting"
        height="1rem"
        width="1rem"
      />
      Add
    </button>
    <ul>
      <li
        class="error"
        v-for="error in newEvaluation.errors"
        :key="error"
      >
        {{ error }}
      </li>
    </ul>
  </form>
</template>

<style scoped>
  .add-evaluation-form {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    padding: 0.5rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    padding: 0.5rem;
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
    margin: 0.5rem;
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
