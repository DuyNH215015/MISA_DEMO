<script setup>
const props = defineProps({
  show: {
    type: Boolean,
    default: false,
  },
  title: {
    type: String,
    default: 'Cảnh báo!',
  },
  message: {
    type: String,
    default: '',
  },
  shiftCode: {
    type: String,
    default: '',
  },
})

const emit = defineEmits(['close'])

const handleClose = () => {
  emit('close')
}
</script>

<template>
  <div v-if="show" class="popup-overlay-2">
    <div class="popup-add">
      <div class="header-waring">
        <div class="header-waring-left">
          <div class="icon-warning"></div>
          <div class="msg-title">{{ props.title }}</div>
        </div>
        <div class="icon-close" @click="handleClose"></div>
      </div>
      <div class="main-waring">
        <template v-if="props.shiftCode">
          Mã ca <strong>{{ props.shiftCode }}</strong> đã tồn tại. Vui lòng kiểm tra lại.
        </template>
        <template v-else>
          {{ props.message }}
        </template>
      </div>
      <div class="footer-waring">
        <div class="btn-2" @click="handleClose">Đóng</div>
      </div>
      <div></div>
    </div>
  </div>
</template>

<style scoped>
.popup-overlay-2 {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.popup-add {
  width: 432px;
  padding: 16px;
  background: #fff;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}
.header-waring {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}
.header-waring-left {
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
}
.msg-title {
  font-size: 20px;
  font-weight: 600;
  color: #111827;
}
.main-waring {
  font-size: 13px;
  font-weight: 400;
  margin-bottom: 16px;
}
.footer-waring {
  display: flex;
  flex-direction: row;
  justify-content: flex-end;
  width: 100%;
  gap: 8px;
  box-sizing: border-box;
}
.footer-waring .btn-1 {
  box-sizing: border-box;
  height: 28px;
  padding: 6px 12px;
  border: 1px solid #d1d5db;
  color: #111827;
  background-color: #fff;
  white-space: nowrap;
  font-weight: 500;
  border-radius: 4px;
  cursor: pointer;
  line-height: 16px;
}
.footer-waring .btn-2 {
  box-sizing: border-box;
  height: 28px;
  padding: 6px 12px;
  border: none;
  color: #fff;
  background-color: #009b71;
  white-space: nowrap;
  font-weight: 500;
  border-radius: 4px;
  cursor: pointer;
  line-height: 16px;
}
</style>
