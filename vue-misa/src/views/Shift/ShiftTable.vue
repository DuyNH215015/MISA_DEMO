<script setup>
import BaseTable from '@/components/table/MsTable.vue'
import { shiftColumn } from '@/common/constant/table-shift'
import { Status } from '@/common/enum/Iactive'
import { defineEmits, defineProps, onBeforeUnmount, onMounted, ref, computed } from 'vue'
import ToastWarningDelete from '@/components/toast/ToastWarningDelete.vue'
const props = defineProps({
  shifts: {
    type: Array,
    default: () => [],
  },
  currentPage: Number,
  pageSize: Number,
  searchText: { type: String, default: '' },
  clearSelection: Boolean,
  filters: {
    type: Array,
    default: () => [],
  },
})

const emit = defineEmits([
  'selectedIds',
  'sort-change',
  'edit-row',
  'apply-filter',
  'clone',
  'change-status',
  'delete-row',
])
/* ================= Handler SORT Filter and SelectIds ================= */
const handleSortChange = ({ SortBy, SortDir }) => {
  emit('sort-change', { SortBy, SortDir }) // emit tiếp lên Shift.vue
}
const handeSelect = (selectedIds) => {
  emit('selectedIds', selectedIds)
}
function handleEdit(id) {
  emit('edit-row', id)
}
const handleFilter = (value) => {
  emit('apply-filter', value) // emit tiếp lên Shift.vue
}
/* ================= PopupMore ================= */
// xử lí sự kiện more
const showRowPopup = ref(false)
const activeRow = ref(null)
const rowPopupPosition = ref({ x: 0, y: 0 })
const rowPopupRef = ref(null)
function openRowAction(row, event) {
  activeRow.value = row
  showRowPopup.value = true

  const rect = event.target.getBoundingClientRect()
  rowPopupPosition.value = {
    x: rect.right - 120, // width popup
    y: rect.bottom + 8,
  }
}
function handleClickOutside(e) {
  if (!rowPopupRef.value) return
  if (!rowPopupRef.value.contains(e.target)) {
    showRowPopup.value = false
    activeRow.value = null
  }
}
/* ================= handle Clone Inactive and Delete ================= */
const handleClone = () => {
  if (!activeRow.value) return
  const ShiftId = activeRow.value.shiftId
  showRowPopup.value = false
  emit('clone', ShiftId)
}
const handleStartUsing = () => {
  emit('change-status', {
    id: activeRow.value.shiftId,
    inactive: true,
  })
  showRowPopup.value = false
}
const handleStopUsing = () => {
  emit('change-status', {
    id: activeRow.value.shiftId,
    inactive: false,
  })
  showRowPopup.value = false
}
const handleDelete = () => {
  if (!activeRow.value) return
  deleteTargetId.value = activeRow.value.shiftId
  PopupWaring.value = true

  showRowPopup.value = false
}

/* ================= Popup Waring ================= */
const PopupWaring = ref(false)
const deleteTargetId = ref(null)
const deleteShiftCode = computed(() => activeRow.value?.shiftCode ?? '')

const closePopupWaring = () => {
  PopupWaring.value = false
  deleteTargetId.value = null
}
const handleConfirmWaring = () => {
  if (!deleteTargetId.value) return
  emit('delete-row', {
    id: deleteTargetId.value,
  })
  PopupWaring.value = false
  deleteTargetId.value = null
}

/* ================= LIFECYCLE ================= */
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<template>
  <div>
    <!----------------------- Popup More ----------------------->
    <div
      v-if="showRowPopup"
      ref="rowPopupRef"
      class="row-popup"
      :style="{
        top: rowPopupPosition.y + 'px',
        left: rowPopupPosition.x + 'px',
      }"
    >
      <div class="item" @click="handleClone">
        <div class="icon-duplicate"></div>
        Nhân bản
      </div>

      <div v-if="activeRow && activeRow.inactive === true" class="item" @click="handleStopUsing">
        <div class="icon-unusing"></div>
        Ngưng sử dụng
      </div>
      <div v-else class="item" @click="handleStartUsing">
        <div class="icon-using"></div>
        Sử dụng
      </div>

      <div class="item" @click="handleDelete">
        <div class="icon-delete"></div>
        Xóa
      </div>
    </div>

    <!----------------------- Popup Waring ----------------------->
    <ToastWarningDelete
      v-if="PopupWaring && activeRow"
      :show="PopupWaring"
      :shiftCode="deleteShiftCode"
      @close="closePopupWaring"
      @confirm="handleConfirmWaring"
    />

    <!----------------------- BaseTable ----------------------->
    <BaseTable
      :columns="shiftColumn"
      :rows="props.shifts"
      :clearSelection="props.clearSelection"
      :filters="props.filters"
      @sort-change="handleSortChange"
      @selectedIds="handeSelect"
      @apply-filter="handleFilter"
    >
      <!-- ACTION -->
      <template #action="{ row }">
        <div class="row-action">
          <div class="icon-container">
            <div class="row-edit-icon" @click="handleEdit(row.shiftId)"></div>
          </div>
          <div class="icon-container">
            <div class="row-more-icon" @click.stop="openRowAction(row, $event)"></div>
          </div>
        </div>
      </template>
      <!-- Iactive -->
      <template #inactive="{ row }">
        <div :class="row.inactive ? 'status-inactive' : 'status-active'">
          {{ Status[row.inactive] }}
        </div>
      </template>
    </BaseTable>
  </div>
</template>

<style scoped>
.status-inactive {
  background-color: #ebfef6;
  color: #009b71;
  padding: 5px 8px;
  border-radius: 999px;
  width: 112px;
  text-align: center;
}

.status-active {
  background-color: #fee2e2; /* đỏ */
  color: #dc2626;
  padding: 5px 8px;
  border-radius: 999px;
  width: 112px;
  text-align: center;
}
.row-popup {
  position: fixed;
  width: 145px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  padding: 8px 0;
  z-index: 1000;
}
.row-popup .item {
  display: flex;
  flex-direction: row;
  align-content: center;
  padding: 8px 12px;
  gap: 8px;
  cursor: pointer;
}
.item .icon-using {
  background-color: #4b5563;
}
.item .icon-duplicate {
  background-color: #4b5563;
}
.item .icon-unusing {
  background-color: #4b5563;
}
.item:hover {
  background-color: rgb(243, 244, 246);
}
</style>
