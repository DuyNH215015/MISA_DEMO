<script setup>
import Select from 'primevue/select'

const props = defineProps({
  modelValue: {
    type: [String, Number, Object, null],
    default: null,
  },
  options: {
    type: Array,
    required: true,
  },
  placeholder: {
    type: String,
    default: '',
  },
  optionLabel: {
    type: String,
    default: 'label',
  },
  optionValue: {
    type: String,
    default: 'value',
  },
})

const emit = defineEmits(['update:modelValue'])

const onChange = (value) => {
  emit('update:modelValue', value)
}
</script>

<template>
  <Select
    :modelValue="modelValue"
    :options="options"
    :optionLabel="optionLabel"
    :optionValue="optionValue"
    :placeholder="placeholder"
    checkmark
    class="w-full"
    @update:modelValue="onChange"
  />
</template>
