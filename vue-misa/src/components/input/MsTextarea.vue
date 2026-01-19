<template>
  <div class="streaming-field-container-2">
    <textarea
      class="text-input-2"
      :name="name"
      :placeholder="placeholder"
      :maxlength="maxlength"
      :rows="rows"
      :value="modelValue"
      @input="$emit('update:modelValue', $event.target.value)"
    ></textarea>
  </div>
</template>

<script setup>
defineProps({
  modelValue: {
    type: String,
    default: '',
  },
  name: {
    type: String,
    default: '',
  },
  placeholder: {
    type: String,
    default: '',
  },
  maxlength: {
    type: [String, Number],
    default: 255,
  },
  rows: {
    type: [String, Number],
    default: 4,
  },
})
</script>
<style>
.text-input-2 {
  width: 100%;
  min-height: 64px; /* 4 dòng ~ 24px x 4 */
  padding: 8px 12px;
  font-size: 14px;
  font-family: Inter;
  line-height: 24px;
  border-radius: 3px;
  border: 1px solid #dddde4;
  box-sizing: border-box;
  resize: none; /* ❌ không cho kéo */
  outline: none;
}

.streaming-field-container-2 {
  width: 100%;
  height: auto;
}
</style>
