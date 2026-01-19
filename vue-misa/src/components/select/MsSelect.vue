<script setup>
defineProps({
  modelValue: {
    type: [String, Number],
    default: '',
  },
  options: {
    type: Array,
    required: true,
    // [{ label: 'Nam', value: 'male' }]
  },
  placeholder: {
    type: String,
    default: 'Chọn',
  },
  name: {
    type: String,
    default: '',
  },
})

const emit = defineEmits(['update:modelValue'])

function onChange(e) {
  emit('update:modelValue', e.target.value)
}
</script>

<template>
  <div class="select-form">
    <select class="select-input" :name="name" :value="modelValue" @change="onChange">
      <option value="" disabled hidden>
        {{ placeholder }}
      </option>

      <option v-for="item in options" :key="item.value" :value="item.value">
        {{ item.label }}
      </option>
    </select>
    <div class="icon-down-placeholder"></div>
  </div>
</template>
