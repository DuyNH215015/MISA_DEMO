<script setup>
defineProps({
  modelValue: {
    type: String,
    default: '',
  },
  type: {
    type: String,
    default: 'text',
  },
  maxlength: {
    type: [String, Number],
    default: 255,
  },
  placeholder: {
    type: String,
    default: '',
  },
  name: {
    type: String,
    default: '',
  },
})
</script>

<template>
  <div class="streaming-field-container">
    <input
      :type="type"
      :maxlength="maxlength"
      :placeholder="placeholder"
      :name="name"
      class="text-input"
      :value="modelValue"
      @input="$emit('update:modelValue', $event.target.value)"
    />
  </div>
</template>
<style>
.streaming-field-container {
  width: 100%;
  height: auto;
}
.text-input:focus,
.text-input:hover {
  border-color: #6aa7ff; /* xanh dương nhạt */
  box-shadow: 0 0 0 2px rgba(106, 167, 255, 0.25); /* glow nhẹ */
}
.text-input {
  display: flex;
  text-align: left;
  justify-content: flex-start;
  height: 32px !important;
  line-height: 32px !important;
  font-family: Inter;
  box-sizing: border-box;
  padding: 0 16px;
  padding-top: 2px;
  width: 100%;
  box-sizing: border-box;
  border-radius: 3px;
  outline: none;
  border: 1px solid rgb(221, 221, 228);
  transition:
    border-color 0.2s ease,
    box-shadow 0.2s ease;
}
</style>
