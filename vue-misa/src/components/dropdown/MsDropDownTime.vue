<script setup>
import Dropdown from 'primevue/select'
import { ref, watch } from 'vue'

// nhận v-model từ cha
const props = defineProps({
  modelValue: {
    type: String,
    default: null,
  },
  options: {
    type: Array,
    required: true,
  },
  placeholder: {
    type: String,
    default: 'HH:MM',
  },
})

const emit = defineEmits(['update:modelValue'])

/**
 * selected dùng cho Dropdown
 * nhưng modelValue luôn là string
 */
const selected = ref(null)

// gõ tay
watch(selected, (val) => {
  // 1️⃣ gõ tay → PrimeVue set string
  if (typeof val === 'string') {
    emit('update:modelValue', val)
    return
  }

  // 2️⃣ chọn dropdown → object
  if (val && typeof val === 'object') {
    emit('update:modelValue', val.code)
  }
})

// sync cha → con (chỉ cho dropdown)
watch(
  () => props.modelValue,
  (val) => {
    selected.value = props.options.find((o) => o.code === val || o.name === val) || val
  },
  { immediate: true },
)
</script>
<template>
  <div class="custom">
    <Dropdown
      v-model="selected"
      editable
      :options="options"
      optionLabel="name"
      :placeholder="placeholder"
      dropdownIcon="pi pi-clock"
    />
  </div>
</template>

<style scoped>
.custom {
  display: flex;
  width: 122px;
  height: 28px;
  --p-select-padding-y: 6px;
  --p-select-padding-x: 12px;
}
.custom .p-select-option {
  display: flex;
  justify-content: center;
}
</style>
