<script setup lang="ts">
  import { json } from '@codemirror/lang-json';
  import { Compartment, EditorState } from '@codemirror/state';
  import { EditorView, basicSetup } from 'codemirror';
  import { dracula } from 'thememirror';
  import { onMounted, ref, watch } from 'vue';

  const editorText = defineModel<string>();

  const props = defineProps<{
    disabled: boolean;
  }>();

  const editorContainer = ref(null);

  const isDisabled = new Compartment();
  const isEditable = new Compartment();

  let editorView: EditorView | null = null;

  onMounted(() => {
    if (editorContainer.value === null) {
      return;
    }

    const onUpdate = EditorView.updateListener.of(update => {
      if (update.docChanged && editorView !== null) {
        editorText.value = editorView.state.doc.toString();
      }
    });

    editorView = new EditorView({
      state: EditorState.create({
        doc: editorText.value,
        extensions: [
          json(),
          onUpdate,
          basicSetup,
          dracula,
          isDisabled.of(EditorState.readOnly.of(props.disabled)),
          isEditable.of(EditorView.editable.of(!props.disabled)),
        ],
      }),
      parent: editorContainer.value,
    });
  });

  watch(
    () => editorText.value,
    newValue => {
      if (editorView !== null && newValue !== editorView.state.doc.toString()) {
        editorView.dispatch({
          changes: { from: 0, to: editorView.state.doc.length, insert: newValue },
        });
      }
    },
  );

  watch(
    () => props.disabled,
    newValue => {
      if (editorView !== null) {
        editorView.dispatch({
          effects: [
            isDisabled.reconfigure(EditorState.readOnly.of(newValue)),
            isEditable.reconfigure(EditorView.editable.of(!newValue)),
          ],
        });
      }
    },
  );
</script>

<template>
  <div ref="editorContainer"></div>
</template>

<style>
  div[ref='editorContainer'] {
    height: 100%;
  }
</style>
