<template>
  <transition name="toast-fade">
    <div v-if="visible" class="toast">
      <div class="icon"></div>
      <div class="message">{{ message }}</div>
      <div class="icon-close" @click="close"></div>
    </div>
  </transition>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  message: {
    type: String,
    default: '',
  },
  duration: {
    type: Number,
    default: 3000,
  },
})

const emit = defineEmits(['close'])

const visible = ref(false)
const close = () => {
  visible.value = false
  emit('close')
}
watch(
  () => props.message,
  (val) => {
    if (val) {
      visible.value = true
      setTimeout(() => {
        visible.value = false
        emit('close')
      }, props.duration)
    }
  },
)
</script>

<style scoped>
.toast {
  display: flex;
  align-items: center;
  flex-direction: row;
  position: fixed;
  top: 10px;
  right: 40%;
  background-color: #fff;
  color: rgb(17, 24, 39);
  padding: 12px 0px;
  min-width: 300px;
  min-height: 48px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 400;
  z-index: 9999;
  box-shadow: 0 4px 16px #0000003d;
  box-sizing: border-box;
  justify-content: space-around;
  border-left: 4px solid #009b71;
}
.toast .icon {
  width: 20px;
  height: 20px;
  min-width: 20px;
  min-height: 20px;
  background-color: #009b71;
  mask-image: url(../../assets/icons/toast.svg);
  mask-position: -98px -168px;
  mask-repeat: no-repeat;
}
.toast .icon-close {
  width: 20px;
  height: 20px;
  min-width: 20px;
  min-height: 20px;
  background-color: #4b5563;
  mask-image: url(../../assets/icons/icon-sidebar3.svg);
  mask-position: -299px -16px;
  mask-repeat: no-repeat;
  cursor: pointer;
}
.toast .message {
  line-height: 24px;
}

.toast-fade-enter-active,
.toast-fade-leave-active {
  transition: all 0.3s ease;
}

.toast-fade-enter-from,
.toast-fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
