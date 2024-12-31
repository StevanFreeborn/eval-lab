import globals from 'globals';
import pluginJs from '@eslint/js';
import tseslint from 'typescript-eslint';
import pluginVue from 'eslint-plugin-vue';
import eslintConfigPrettier from 'eslint-config-prettier';

/** @type {import('eslint').Linter.Config[]} */
export default [
  { files: ['**/*.{js,mjs,cjs,ts,vue}'], ignores: ['**/node_modules/**'] },
  { languageOptions: { globals: { ...globals.browser, ...globals.node } } },
  pluginJs.configs.recommended,
  ...tseslint.configs.strict,
  ...pluginVue.configs['flat/strongly-recommended'],
  eslintConfigPrettier,
  { files: ['**/*.vue'], languageOptions: { parserOptions: { parser: tseslint.parser } } },
];
