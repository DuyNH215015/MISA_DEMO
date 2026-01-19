<script setup>
import { reactive, watch, onMounted, onBeforeUnmount, ref, computed, nextTick } from 'vue'
import BaseTimeDropdown from '@/components/dropdown/MsDropDownTime.vue'
import BaseRadio from '@/components/radiobutton/RadioButton.vue'
import ToastWarning from '@/components/toast/ToastWarning.vue'
import { timeOptions } from '@/data/form-select-time'
import { validateForm } from '@/utils/validate-shift'
import { formatTimeToHM } from '@/utils/format'
const emit = defineEmits(['close', 'updateShift', 'saveandadd'])
const props = defineProps({
  shiftEditvalue: {
    type: Object,
    required: true,
  },
})
/* ================= FORM ================= */
const form = reactive({
  ShiftCode: '',
  ShiftName: '',
  BeginShiftTime: null,
  EndShiftTime: null,
  BeginBreakTime: null,
  EndBreakTime: null,
  WorkingTime: null,
  BreakingTime: null,
  Description: '',
  Inactive: null,
})
/* ================= CloneFrom (initialForm) ================= */
const initialForm = ref(null) // clone dữ liệu form trước khi edit để kiểm tra trạng thái sửa đôi
watch(
  () => props.shiftEditvalue,
  (val) => {
    if (!val) return

    const mappedData = {
      ShiftCode: val.shiftCode,
      ShiftName: val.shiftName,
      BeginShiftTime: formatTimeToHM(val.beginShiftTime),
      EndShiftTime: formatTimeToHM(val.endShiftTime),
      BeginBreakTime: formatTimeToHM(val.beginBreakTime),
      EndBreakTime: formatTimeToHM(val.endBreakTime),
      WorkingTime: val.workingTime,
      BreakingTime: val.breakingTime,
      Description: val.description,
      Inactive: val.inactive,
    }
    Object.assign(form, mappedData)
    initialForm.value = JSON.parse(JSON.stringify(mappedData))
  },
  { immediate: true },
)
/* ================= Close ================= */
// đóng và nếu có thay đổi thì hiện popup nếu ko có thay đổi thì ko hiện
// biến quản lí close popupedit
const isClosing = ref(false)
// xử lí chăn double click khi waring đang hiện
const safeClose = async () => {
  if (isClosing.value) return
  isClosing.value = true
  //  Đợi Vue render xong (DOM ổn định)
  await nextTick()
  emit('close')
}
const close = () => {
  if (!isDirty()) {
    //nếu popup chưa sửa gì isDirty =fasle thực gọi safeClose() đóng popup
    safeClose()
    return
  }
  // nếu đã sửa isDirty = true thực hiện mở cảnh báo
  PopupWaring.value = true
}
/* ================= isDirty check ================= */
// kiểm tra xem người dùng đã thực hiện sửa đổi chưa
const isDirty = () => {
  if (!initialForm.value) return false
  return JSON.stringify(form) !== JSON.stringify(initialForm.value)
}

/* ================= WARNING POPUP ================= */
// xử lí sự kiện thông thông báo thoái edit nếu đã nhập dữ liệu
// biến để mở popup cảnh báo
const PopupWaring = ref(false)
// tắt cảnh báo và tiếp tục
const closePopupWaring = () => {
  PopupWaring.value = false
}
// tắt cách báo và xác nhận ko lưu
const handleConfirmWaring = async () => {
  PopupWaring.value = false
  await nextTick()
  safeClose()
}

/* ================= Save & SaveAndAdd ================= */
// lưu popupedit
const save = () => {
  const { WorkingTime, BreakingTime, ...payload } = { ...form }

  emit('updateShift', {
    id: props.shiftEditvalue.shiftId,
    formData: payload,
  })
}
// lưu lại edit và thêm mới
const saveAndAdd = () => {
  const { WorkingTime, BreakingTime, ...payload } = { ...form }

  emit('saveandadd', {
    id: props.shiftEditvalue.shiftId,
    formData: payload,
  })
}

/* ================= Hot Key ================= */
// xử lí sự kiện lưu và thêm bằng phím tắt
const handleKeydown = (e) => {
  // Ctrl + Shift + S
  if (e.ctrlKey && e.shiftKey && e.code === 'KeyS') {
    e.preventDefault()
    saveAndAdd()
    return
  }
  // Ctrl + S
  if (e.ctrlKey && !e.shiftKey && e.code === 'KeyS') {
    e.preventDefault()
    save()
    return
  }
  // ESC
  if (e.code === 'Escape') {
    close()
  }
}

/* ================= LIFECYCLE ================= */
// gắn và tắt sự kiện trước và sau khi khởi tạo và xóa DOM
onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
})
onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleKeydown)
})
</script>

<template>
  <div>
    <ToastWarning
      :show="PopupWaring"
      title="Thoát và không lưu?"
      message="Nếu bạn thoát, các dữ liệu đang nhập liệu sẽ không được lưu lại."
      @close="closePopupWaring"
      @confirm="handleConfirmWaring"
    />
    <div class="popup-overlay" @click.self="!PopupWaring && close()">
      <!-- popup cảnh báo thoát edit  -->

      <div class="popup-add">
        <div class="popup-header">
          <div class="popup-header-left">Sửa ca làm việc</div>
          <div class="popup-header-right">
            <div class="icon-help"></div>
            <div class="icon-close" @click="close"></div>
          </div>
        </div>
        <div class="popup-main">
          <div class="ms-input">
            <div class="left">Mã ca <span style="color: #e54848">*</span></div>
            <div class="right">
              <input type="text" class="input" maxlength="20" v-model="form.ShiftCode" />
            </div>
          </div>
          <div class="ms-input">
            <div class="left">Tên ca <span style="color: #e54848">*</span></div>
            <div class="right">
              <input type="text" class="input" maxlength="50" v-model="form.ShiftName" />
            </div>
          </div>
          <div class="ms-input-2">
            <div class="ms-input">
              <div class="left">Giờ vào ca <span style="color: #e54848">*</span></div>
              <div class="right">
                <BaseTimeDropdown v-model="form.BeginShiftTime" :options="timeOptions" />
              </div>
            </div>
            <div class="ms-input">
              <div class="left">Giờ hết ca <span style="color: #e54848">*</span></div>
              <div class="right">
                <BaseTimeDropdown v-model="form.EndShiftTime" :options="timeOptions" />
              </div>
            </div>
          </div>
          <div class="ms-input-2">
            <div class="ms-input">
              <div class="left">Bắt đầu nghỉ giữa ca</div>
              <div class="right">
                <BaseTimeDropdown v-model="form.BeginBreakTime" :options="timeOptions" />
              </div>
            </div>
            <div class="ms-input">
              <div class="left">Kết thúc nghỉ giữa ca</div>
              <div class="right">
                <BaseTimeDropdown v-model="form.EndBreakTime" :options="timeOptions" />
              </div>
            </div>
          </div>
          <div class="ms-input-2">
            <div class="ms-input">
              <div class="left">Thời gian làm việc (giờ)</div>
              <div class="right-2">{{ form.WorkingTime.toFixed(2) }}</div>
            </div>
            <div class="ms-input">
              <div class="left">Kết thúc nghỉ giữa ca (giờ)</div>
              <div class="right-2">{{ form.BreakingTime.toFixed(2) }}</div>
            </div>
          </div>
          <div class="ms-input-3">
            <div class="left">Mô tả</div>
            <div class="right">
              <textarea name="mô tả" class="input" rows="3" v-model="form.Description"></textarea>
            </div>
          </div>
          <div class="ms-input-4">
            <div class="text">Trạng thái</div>
            <div class="radio">
              <BaseRadio v-model="form.Inactive" :value="true" label="Đang hoạt động" />
              <BaseRadio v-model="form.Inactive" :value="false" label="Ngừng hoạt động" />
            </div>
          </div>
        </div>
        <div class="popup-footer">
          <div class="btn-1" @click="close">Hủy</div>
          <div class="btn-1" @click="saveAndAdd">Lưu và thêm</div>
          <div class="btn-2" @click="save">Lưu</div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.popup-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 800;
}
.popup-add {
  width: 680px;
  background: #fff;
  border-radius: 8px;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
}
.popup-header {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  width: 100%;
  height: 68px;
}
.popup-header-left {
  font-size: 24px;
  font-weight: 700;
  white-space: nowrap;
  color: #111827;
  line-height: 1;
}
.popup-header-right {
  display: flex;
  flex-direction: row;
  gap: 8px;
}
.popup-main {
  padding: 20px;
  display: flex;
  flex-direction: column;
  border-bottom: 1px solid #d5dfe2;
}
.ms-input {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}
.ms-input-2 {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  gap: 45px;
  margin-bottom: 4px;
}
.ms-input-3 {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: flex-start;
  gap: 45px;
  margin-bottom: 4px;
}
.ms-input-4 {
  display: flex;
  flex-direction: row;
  margin-top: 15px;
}
.ms-input-4 .text {
  font-weight: 600;
  color: #262626;
  width: 175px;
}
.ms-input-4 .radio {
  display: flex;
  gap: 20px;
}
.ms-input .left {
  font-size: 13px;
  font-weight: 500;
  color: #262626;
}
.ms-input .right {
  display: flex;
  justify-content: flex-end;
}
.right .input {
  padding: 5px 12px;
  border-radius: 4px;
  border: 1px solid #d1d5db;
  width: 465px;
}
.right .input:focus-visible {
  outline: none;
  border-color: rgb(255, 0, 0);
}
.right .p-select {
  height: 32px;
}
.right-2 {
  width: 122px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  background-color: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding: 5px 12px;
  color: #4b5563;
  font-size: 13px;
  font-weight: 400;
}
.popup-footer {
  display: flex;
  flex-direction: row;
  justify-content: flex-end;
  height: 52px;
  width: 100%;
  padding: 12px 20px;
  gap: 8px;
}
.popup-footer .btn-1 {
  padding: 6px 12px;
  border: 1px solid #d1d5db;
  color: #111827;
  background-color: #fff;
  white-space: nowrap;
  font-weight: 500;
  border-radius: 4px;
  cursor: pointer;
}
.popup-footer .btn-2 {
  padding: 6px 12px;
  border: none;
  color: #fff;
  background-color: #009b71;
  white-space: nowrap;
  font-weight: 500;
  border-radius: 4px;
  cursor: pointer;
}
</style>
