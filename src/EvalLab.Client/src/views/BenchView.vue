<script setup lang="ts">
  import { ref, nextTick } from 'vue';
  import PlayIcon from '../components/icons/PlayIcon.vue';
  import ResponseTab from '../components/tabs/ResponseTab.vue';
  import TraceTab from '../components/tabs/TraceTab.vue';
  import WaitingSpinner from '../components/WaitingSpinner.vue';

  const input = ref('');
  const isRunning = ref(false);
  const response = ref('');

  // TODO: Actually send input to pipeline via API and get response
  async function handleSubmit() {
    if (!input.value) {
      alert('Please provide an input');
      return;
    }

    isRunning.value = true;
    console.log('Running input:', input.value);

    await new Promise(resolve => setTimeout(resolve, 2000));

    console.log('Input ran successfully');
    isRunning.value = false;
    response.value = 'This is the response from the pipeline';
  }

  const tabs = [
    {
      id: 'response',
      label: 'Response',
      panelId: 'response-panel',
      component: ResponseTab,
    },
    {
      id: 'trace',
      label: 'Trace',
      panelId: 'trace-panel',
      component: TraceTab,
    },
  ];

  const activeTab = ref(0);

  function handleTabClick(index: number) {
    activeTab.value = index;

    nextTick(() => {
      const tabButton = document.getElementById(`${tabs[index].id}`);
      tabButton?.focus();
    });
  }

  function handleRightKeyDown() {
    const nextTabIndex = (activeTab.value + 1) % tabs.length;
    handleTabClick(nextTabIndex);
  }

  function handleLeftKeyDown() {
    const nextTabIndex = (activeTab.value - 1 + tabs.length) % tabs.length;
    handleTabClick(nextTabIndex);
  }

  function isActiveTab(index: number) {
    return activeTab.value === index;
  }
</script>

<template>
  <h2>Bench</h2>
  <div class="bench-container">
    <div class="input-container">
      <h3>Input</h3>
      <form
        @submit.prevent="handleSubmit"
        novalidate
      >
        <textarea
          v-model="input"
          id="input"
          name="input"
          rows="10"
          placeholder="Enter your input here"
          required
        ></textarea>
        <button
          class="run-button"
          type="submit"
          :disabled="!input || isRunning"
        >
          <WaitingSpinner
            v-if="isRunning"
            height="1rem"
            width="1rem"
          />
          <PlayIcon v-else />
          Run
        </button>
      </form>
    </div>
    <div class="output-container">
      <h3 id="output-tab-list">Outputs</h3>
      <div
        role="tablist"
        aria-labelledby="output-tab-list"
      >
        <button
          v-for="(tab, index) in tabs"
          :key="tab.id"
          :id="tab.id"
          type="button"
          role="tab"
          :aria-selected="isActiveTab(index)"
          :aria-controls="tab.panelId"
          :tabindex="isActiveTab(index) ? 0 : -1"
          @click="handleTabClick(index)"
          @keydown.right.prevent="handleRightKeyDown"
          @keydown.left.prevent="handleLeftKeyDown"
        >
          {{ tab.label }}
        </button>
      </div>
      <div
        v-for="(tab, index) in tabs"
        :key="tab.id"
        :id="tab.panelId"
        role="tabpanel"
        :aria-labelledby="tab.id"
        :hidden="!isActiveTab(index)"
        :class="{ hidden: !isActiveTab(index) }"
      >
        <component
          :is="tab.component"
          v-bind="{
            content: response,
          }"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
  h2 {
    margin-bottom: 1rem;
  }

  .bench-container {
    display: flex;
    gap: 1rem;
    flex: 1;
  }

  .input-container {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    background-color: var(--secondary-background-color);
    padding: 1rem;
    border-radius: 0.25rem;
  }

  .output-container {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  form {
    display: flex;
    flex-direction: column;
    flex: 1;
    gap: 0.5rem;
  }

  textarea {
    width: 100%;
    resize: none;
    padding: 0.25rem;
    border-radius: 0.25rem;
    flex: 1;
  }

  textarea:required {
    border-left: 1px solid red;
  }

  .run-button {
    display: flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.5rem;
    border: 1px solid var(--text-color);
    border-radius: 0.25rem;
    align-self: flex-start;
  }

  .run-button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .run-button > svg {
    height: 1rem;
    width: 1rem;
  }

  [role='tablist'] {
    display: flex;
    gap: 1rem;
  }

  [role='tab'] {
    width: 100%;
    padding: 0.5rem;
    background-color: var(--background-color);
    border: 1px solid var(--secondary-background-color);
    cursor: pointer;
    border-radius: 0.25rem;
  }

  [role='tab'][aria-selected='true'] {
    background-color: var(--secondary-background-color);
    border-color: transparent;
  }

  [role='tabpanel'] {
    display: flex;
    flex-direction: column;
    flex: 1;
    padding: 1rem;
    background-color: var(--secondary-background-color);
    border-radius: 0.25rem;
  }

  .hidden {
    display: none;
  }
</style>
