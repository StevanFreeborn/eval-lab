<script setup lang="ts"></script>

<template>
  <div class="tab-content">
    <p
      v-if="!content"
      class="placeholder"
    >
      Waiting for trace information.
    </p>
    <p v-else>{{ content }}</p>
  </div>
</template>

<style scoped>
  .tab-content {
    display: flex;
    flex-direction: column;
    flex: 1;
  }

  .placeholder {
    align-self: center;
    align-content: center;
    height: 100%;
  }
</style>
