<script setup lang="ts">
  import { RouterLink, RouterView } from 'vue-router';
  import FlaskIcon from '../components/icons/FlaskIcon.vue';
  import MicroscopeIcon from '../components/icons/MicroscopeIcon.vue';
  import PipelineIcon from '../components/icons/PipelineIcon.vue';
  import RulerIcon from '../components/icons/RulerIcon.vue';
</script>

<template>
  <div class="brand">
    <h1>EvalLab</h1>
    <FlaskIcon />
  </div>
  <div class="layout-container">
    <aside>
      <nav>
        <ul>
          <li>
            <RouterLink to="/">
              <MicroscopeIcon />
              Bench
            </RouterLink>
          </li>
          <li>
            <RouterLink to="/evaluations">
              <RulerIcon />
              Evaluations
            </RouterLink>
          </li>
          <li>
            <RouterLink to="/pipelines">
              <PipelineIcon />
              Pipelines
            </RouterLink>
          </li>
        </ul>
      </nav>
    </aside>
    <main>
      <div class="main-content">
        <RouterView />
      </div>
    </main>
  </div>
</template>

<style scoped>
  .brand {
    display: flex;
    align-items: center;
    padding: 0.5rem;
    gap: 0.25rem;
    background: linear-gradient(
      90deg,
      var(--secondary-background-color) 40%,
      var(--background-color) 100%
    );
    width: max-content;

    svg {
      width: 2rem;
      height: 2rem;
    }
  }

  .layout-container {
    display: flex;
    height: 100%;
    flex: 1;
    min-height: 0;
    gap: 1rem;
    position: relative;
    overflow: hidden;
  }

  aside {
    width: 75px;
    background-color: var(--secondary-background-color);
  }

  nav {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    ul {
      display: flex;
      flex-direction: column;
    }

    a {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
      padding: 0.5rem;
      transition: background-color 0.2s;
      font-size: 0.75rem;
      border-left: 3px solid transparent;
      border-right: 3px solid transparent;
    }

    a:hover {
      background-color: var(--background-color);
    }

    .router-link-exact-active {
      border-left-color: var(--text-color);
    }

    svg {
      width: 1.5rem;
      height: 1.5rem;
    }
  }

  main {
    display: flex;
    flex-direction: column;
    flex: 1;
    padding: 1rem;
    max-width: 100%;
    overflow: auto;
  }

  .main-content {
    display: flex;
    flex-direction: column;
    flex: 1;
    height: 0;
  }
</style>
