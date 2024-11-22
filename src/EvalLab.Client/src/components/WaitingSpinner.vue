<script setup lang="ts">
  import { defineProps } from 'vue';

  defineProps<{
    height: string;
    width: string;
  }>();
</script>

<template>
  <div
    class="spinner"
    :style="{ height, width }"
  ></div>
</template>

<style scoped>
  .spinner {
    border: 4px solid var(--background-color);
    border-left-color: var(--text-color);
    border-radius: 50%;
    animation: spin 1s linear infinite;
  }

  @keyframes spin {
    to {
      transform: rotate(360deg);
    }
  }
</style>
