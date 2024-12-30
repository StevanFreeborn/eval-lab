<script setup lang="ts">
  import { defineProps } from 'vue';
  import CircleXMarkIcon from './icons/CircleXMarkIcon.vue';

  const props = defineProps<{
    heading: string;
    drawerOpen: boolean;
  }>();

  const emit = defineEmits<{
    (e: 'drawer-closed'): void;
  }>();

  function closeDrawer() {
    emit('drawer-closed');
  }
</script>

<template>
  <div :class="{ drawer: true, open: drawerOpen }">
    <div class="drawer-header">
      <h3>{{ props.heading }}</h3>
      <button
        class="close-button"
        type="button"
        @click="closeDrawer"
      >
        <span class="sr-only">Close</span>
        <CircleXMarkIcon />
      </button>
    </div>
    <slot></slot>
  </div>
</template>

<style scoped>
  .drawer {
    position: absolute;
    top: 0;
    right: 0;
    bottom: 0;
    width: 0px;
    background-color: var(--secondary-background-color);
    transition: width 0.5s;
    z-index: 2;
    overflow-y: auto;
    overflow-x: hidden;
    scrollbar-width: thin;
    scrollbar-color: var(--text-color) var(--background-color);
    border-top-left-radius: 0.5rem;
    border-bottom-left-radius: 0.5rem;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
  }

  .drawer.open {
    min-width: 350px;
    max-width: 100%;
    width: fit-content;
  }

  .drawer-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  h3 {
    margin: 0.5rem;
    white-space: nowrap;
  }

  .close-button {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0.25rem;
    margin: 0.5rem;

    & svg {
      width: 1rem;
      height: 1rem;
    }
  }
</style>
