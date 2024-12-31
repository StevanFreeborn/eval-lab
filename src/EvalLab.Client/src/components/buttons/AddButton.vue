<script setup lang="ts">
  import CirclePlusIcon from '../icons/CirclePlusIcon.vue';

  const props = defineProps<{
    label: string;
    type: HTMLButtonElement['type'];
  }>();

  const emit = defineEmits(['clicked']);
</script>

<template>
  <button
    :type="props.type"
    @click="emit('clicked')"
  >
    <CirclePlusIcon />
    <span class="sr-only">{{ props.label }}</span>
  </button>
</template>

<style scoped>
  button {
    background-color: var(--secondary-background-color);
    border: 1px solid var(--text-color);
    border-radius: 0.25rem;
    padding: 0.25rem;
  }

  svg {
    display: block;
    width: 1rem;
    height: 1rem;
  }
</style>
