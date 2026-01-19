<script setup>
import RadioButton from 'primevue/radiobutton'
import { computed } from 'vue'

const props = defineProps({
  modelValue: {
    type: [Boolean, String, Number],
    required: true,
  },
  value: {
    type: [Boolean, String, Number],
    required: true,
  },
  label: {
    type: String,
    default: '',
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  inputId: {
    type: String,
    default: () => `radio-${Math.random().toString(36).slice(2)}`,
  },
})

const emit = defineEmits(['update:modelValue'])

const internalValue = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})
</script>

<template>
  <div class="base-radio">
    <RadioButton v-model="internalValue" :value="value" :inputId="inputId" :disabled="disabled" />
    <label v-if="label" :for="inputId">
      {{ label }}
    </label>
  </div>
</template>

<style scoped>
.base-radio {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}
:deep(.p-radiobutton .p-radiobutton-box) {
  width: 16px;
  height: 16px;
}

:deep(.p-radiobutton .p-radiobutton-icon) {
  width: 8px;
  height: 8px;
}
</style>
