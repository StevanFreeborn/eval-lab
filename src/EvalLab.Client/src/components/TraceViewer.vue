<script setup lang="ts">
  import { computed, onMounted, ref, watch } from 'vue';
  import { useService } from '../composables/useService';
  import { Span, Trace, TracesServiceKey } from '../services/traceService';
  import WaitingSpinner from './WaitingSpinner.vue';

  const props = defineProps<{
    runId: string;
  }>();

  const trace = ref<Trace | null>(null);
  const tracesService = useService(TracesServiceKey);

  async function getTrace() {
    trace.value = null;

    let traceResult = await tracesService.get(props.runId);

    while (traceResult.failed) {
      console.error(traceResult.error.message);
      await new Promise(resolve => setTimeout(resolve, 5000));
      traceResult = await tracesService.get(props.runId);
    }

    trace.value = traceResult.value;
  }

  onMounted(getTrace);

  watch(() => props.runId, getTrace);

  function processSpans(spans: Span[]) {
    const depthMap = new Map<string, number>();
    const rootSpans = spans.filter(span => !span.parentId);
    rootSpans.forEach(span => depthMap.set(span.id, 0));

    function calculateDepth(span: Span): number {
      if (depthMap.has(span.id)) {
        // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
        return depthMap.get(span.id)!;
      }

      if (!span.parentId) {
        depthMap.set(span.id, 0);
        return 0;
      }

      const parentSpan = spans.find(s => s.id === span.parentId);
      if (!parentSpan) {
        depthMap.set(span.id, 0);
        return 0;
      }

      const parentDepth = calculateDepth(parentSpan);
      const depth = parentDepth + 1;
      depthMap.set(span.id, depth);
      return depth;
    }

    const traceStart = Math.min(...spans.map(span => span.start));
    const traceEnd = Math.max(...spans.map(span => span.end));
    const traceDuration = traceEnd - traceStart;

    return spans.map(span => ({
      ...span,
      depth: calculateDepth(span),
      startPercent: ((span.start - traceStart) / traceDuration) * 100,
      widthPercent: (span.duration / traceDuration) * 100,
    }));
  }

  const processedSpans = computed(() => {
    if (!trace.value) {
      return [];
    }

    const spans = trace.value.spans;
    const sortedSpans = [...spans].sort((a, b) => a.start - b.start);
    return processSpans(sortedSpans);
  });

  function formatDuration(duration: number) {
    if (duration < 1_000) {
      return `${duration}μs`;
    } else if (duration < 1_000_000) {
      return `${(duration / 1_000).toFixed(2)}ms`;
    } else {
      return `${(duration / 1_000_000).toFixed(2)}s`;
    }
  }
</script>

<template>
  <div
    class="loading-container"
    v-if="!trace"
  >
    <WaitingSpinner
      height="3rem"
      width="3rem"
    />
    <p>Waiting on trace...</p>
  </div>
  <div v-else>
    <div class="trace-header">
      <div class="trace-info">
        <span class="trace-name">{{ trace.name }}</span>
        <span class="trace-timestamp">{{ trace.createdDate.toLocaleString() }}</span>
        <span class="trace-duration">{{ formatDuration(trace.duration) }}</span>
      </div>
    </div>
    <div>
      <div class="spans-container">
        <div
          v-for="span in processedSpans"
          :key="span.id"
          class="span-item"
          :style="{}"
        >
          <div class="span-bar-container">
            <div
              class="span-bar"
              :style="{
                left: `${span.startPercent}%`,
                width: `${span.widthPercent}%`,
              }"
            >
              <div class="span-content">
                <span
                  class="span-name"
                  :title="span.name"
                  >{{ span.name }}</span
                >
                <span class="span-duration">{{ formatDuration(span.duration) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- TODO: Attributes viewer -->
  </div>
</template>

<style scoped>
  .loading-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 1rem;
    height: 100%;
    width: 100%;
  }

  .trace-header {
    padding: 0.75rem;
    background-color: var(--background-color);
    border-radius: 0.25rem;
  }

  .trace-info {
    display: flex;
    gap: 1rem;
    align-items: center;
  }

  .trace-name {
    font-weight: 600;
  }

  .trace-duration,
  .trace-timestamp {
    font-size: 0.85rem;
  }

  .spans-container {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    margin: 0.625rem 0;
    position: relative;
  }

  .span-item {
    position: relative;
    margin: 0.25rem 0;
    min-height: 1.5rem;
  }

  .span-bar-container {
    position: relative;
    width: 100%;
    height: 100%;
  }

  .span-bar {
    background-color: var(--background-color);
    position: absolute;
    height: 1.5rem;
    padding: 0.25rem 0.5rem;
    border-radius: 0.125rem;
    min-width: 100px;
    cursor: pointer;
  }

  .span-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 0.85rem;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .span-name {
    margin-right: 0.5rem;
    overflow: hidden;
    text-overflow: ellipsis;
  }
</style>
