<script setup>
import { reactive, watch, onMounted, onBeforeUnmount, ref, computed, nextTick } from 'vue'
import BaseTimeDropdown from '@/components/dropdown/MsDropDownTime.vue'
import { timeOptions } from '@/data/form-select-time'
import ToastWarning from '@/components/toast/ToastWarning.vue'
import ToastWarningValidate from '@/components/toast/ToastWarningValidate.vue'
import { validateForm, toMinutes } from '@/utils/validate-shift'
import { formatTimeToHM } from '@/utils/format'
const props = defineProps({
  cloneData: {
    type: Object,
    default: null,
  },
})
const emit = defineEmits(['close', 'success', 'saveandadd'])
// const startTime = ref(null)
/* ================= FORM ================= */
const form = reactive({
  ShiftCode: '',
  ShiftName: '',
  BeginShiftTime: null,
  EndShiftTime: null,
  BeginBreakTime: null,
  EndBreakTime: null,
  Description: '',
})
/* ================= DIRTY CHECK ================= */
/**
 * Gán dữ liệu ban đầu từ form vào initialForm
 * thực hiện kiểm tra isDirty đầu vào form và initialForm đầu ra trả về bool
 * snapshotForm arrow funtion thực hiện gán form cho initialForm từ lúc onMounted
 * Create by: NHDuy (13/1/2026)
 */
const initialForm = ref(null)
const snapshotForm = () => {
  initialForm.value = JSON.parse(JSON.stringify(form))
}
const isDirty = () => {
  if (!initialForm.value) return false
  return JSON.stringify(form) !== JSON.stringify(initialForm.value)
}
/* ================= WARNING POPUP ================= */
/**
 * Popup Waring khi người dùng thực hiện đóng popup khi có dữ liệu thay đổi
 * isClosing biến quản lí emit(close) true thì cho phép emit false thì đóng
 * safeClose arrowfuntion quản lí emit("close")
 * Create by: NHDuy (13/1/2026)
 */
const PopupWaring = ref(false)
/* ================= SAFE CLOSE ================= */
const isClosing = ref(false)
const safeClose = async () => {
  if (isClosing.value) return
  isClosing.value = true
  await nextTick()
  emit('close')
}
/* ================= CLOSE LOGIC ================= */
// đóng popup
const close = () => {
  if (!isDirty()) {
    safeClose()
    return
  }
  PopupWaring.value = true
}
const closePopupWaring = () => {
  PopupWaring.value = false
}
const handleConfirmWaring = async () => {
  PopupWaring.value = false
  await nextTick()
  safeClose()
}
/* ================= SAVE ================= */
/**
 * funtion save đầu vào là form đã lọc bỏ working time và breakingtime
 * Create by: NHDuy (13/1/2026)
 */
const save = () => {
  emit('success', { ...form })
}
/**
 * funtion saveandadd đầu vào là form đã lọc bỏ working time và breakingtime
 * emit lại saveandadd và reset lại form
 * Create by: NHDuy (13/1/2026)
 */
const saveAndAdd = () => {
  emit('saveandadd', { ...form })
  resetForm()
  snapshotForm()
}
/* ================= RESET ================= */
const resetForm = () => {
  Object.assign(form, {
    ShiftCode: '',
    ShiftName: '',
    BeginShiftTime: null,
    EndShiftTime: null,
    BeginBreakTime: null,
    EndBreakTime: null,
    Description: '',
  })
}
/* ================= CLONE DATA ================= */
watch(
  () => props.cloneData,
  (val) => {
    if (!val) {
      resetForm()
      snapshotForm()
      return
    }
    Object.assign(form, {
      ShiftCode: '', // luôn rỗng
      ShiftName: val.shiftName,
      BeginShiftTime: formatTimeToHM(val.beginShiftTime),
      EndShiftTime: formatTimeToHM(val.endShiftTime),
      BeginBreakTime: formatTimeToHM(val.beginBreakTime),
      EndBreakTime: formatTimeToHM(val.endBreakTime),
      Description: val.description,
    })
    snapshotForm()
  },
  { immediate: true },
)
/* ================= HOTKEY ================= */
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
onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
})
onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleKeydown)
})

/* ================= WorkingTime & ShiftTime ================= */
const workingTimeDisplay = ref(0)
const breakingTimeDisplay = ref(0)
const MINUTES_PER_DAY = 24 * 60
function calcMinutes(start, end) {
  let minutes = end - start
  if (minutes < 0) minutes += MINUTES_PER_DAY
  return minutes
}
watch(
  () => [form.BeginShiftTime, form.EndShiftTime, form.BeginBreakTime, form.EndBreakTime],
  ([beginShift, endShift, beginBreak, endBreak]) => {
    if (!beginShift || !endShift) {
      workingTimeDisplay.value = 0
      return
    }
    const shiftStart = toMinutes(beginShift)
    const shiftEnd = toMinutes(endShift)
    // ❌ trùng giờ → invalid
    if (shiftStart === shiftEnd) {
      workingTimeDisplay.value = 0
      return
    }
    const shiftMinutes = calcMinutes(shiftStart, shiftEnd)
    let breakMinutes = 0
    if (beginBreak && endBreak) {
      const breakStart = toMinutes(beginBreak)
      const breakEnd = toMinutes(endBreak)
      breakMinutes = calcMinutes(breakStart, breakEnd)
    }
    const workingMinutes = Math.max(0, shiftMinutes - breakMinutes)
    workingTimeDisplay.value = Number((workingMinutes / 60).toFixed(2))
  },
  { immediate: true },
)
watch(
  () => [form.BeginBreakTime, form.EndBreakTime],
  ([beginBreak, endBreak]) => {
    if (!beginBreak || !endBreak) {
      breakingTimeDisplay.value = 0
      return
    }
    const start = toMinutes(beginBreak)
    const end = toMinutes(endBreak)
    if (start === end) {
      breakingTimeDisplay.value = 0
      return
    }
    const breakMinutes = calcMinutes(start, end)
    breakingTimeDisplay.value = Number((breakMinutes / 60).toFixed(2))
  },
  { immediate: true },
)
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
    <div class="popup-overlay" @click.self="close">
      <div class="popup-add">
        <div class="popup-header">
          <div class="popup-header-left">Thêm ca làm việc</div>
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
              <div class="right-2">{{ workingTimeDisplay.toFixed(2) }}</div>
            </div>
            <div class="ms-input">
              <div class="left">Kết thúc nghỉ giữa ca (giờ)</div>
              <div class="right-2">{{ breakingTimeDisplay.toFixed(2) }}</div>
            </div>
          </div>
          <div class="ms-input-3">
            <div class="left">Mô tả</div>
            <div class="right">
              <textarea name="mô tả" class="input" rows="3" v-model="form.Description"></textarea>
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
  z-index: 999;
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
