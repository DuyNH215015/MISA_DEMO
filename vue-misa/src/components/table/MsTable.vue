<script setup>
import { ref, computed, watch, onBeforeUnmount, onMounted, reactive } from 'vue'
import { formatNumber, formatDate } from '@/utils/format'
import Checkbox from 'primevue/checkbox'
import Select from '@/components/dropdown/MsDropDown.vue'
import { selectOptions, filterConfig } from '@/data/form-select-options'
const emit = defineEmits(['selectedIds', 'more-option', 'sort-change', 'apply-filter'])
const props = defineProps({
  columns: {
    type: Array,
    required: true,
  },
  rows: {
    type: Array,
    required: true,
  },
  clearSelection: Boolean,
  filters: Array,
})
/**
 * selectedIds
 * mảng lưu các giá trị id được chọn
 * Create by: NHDuy (13/1/2026)
 */
const selectedIds = ref([])

// all checked trạng thái
const isAllChecked = computed(() => {
  return props.rows.length > 0 && selectedIds.value.length === props.rows.length
})
// checkbox ALL
function toggleCheckAll(checked) {
  selectedIds.value = checked ? props.rows.map((row) => row.shiftId) : []
}

/**
 * selectedIds
 * theo dõi giá trị selectedIds nếu có thay thì emit lại lên cha
 * đầu vào là mảng id mới
 * Create by: NHDuy (13/1/2026)
 */
watch(selectedIds, (val) => {
  emit('selectedIds', val)
})

// Xử lý resize cột
let startX = 0
let startWidth = 0
let currentColIndex = null
const localColumns = ref(props.columns.map((col) => ({ ...col })))

function startResize(e, index) {
  startX = e.clientX
  startWidth = localColumns.value[index].width
  currentColIndex = index

  document.addEventListener('mousemove', resizeColumn)
  document.addEventListener('mouseup', stopResize)
}

function resizeColumn(e) {
  if (currentColIndex === null) return

  const deltaX = e.clientX - startX
  const newWidth = startWidth + deltaX

  if (newWidth > 40) {
    localColumns.value[currentColIndex].width = newWidth
  }
}

function stopResize() {
  document.removeEventListener('mousemove', resizeColumn)
  document.removeEventListener('mouseup', stopResize)
  currentColIndex = null
}

// format lại dữ liệu để render lên UI
/**
 * đầu vào là giá trị và cột được lặp qua v-for
 * thực hiện format lại số sang dạng ,00 và ngày tháng
 * Create by: NHDuy (13/1/2026)
 */
function renderCell(value, col) {
  if (value === null || value === undefined || value === '') return '-'

  switch (col.type) {
    case 'number':
      return formatNumber(value, col.precision ?? 2)
    case 'date':
      return formatDate(value)
    case 'time':
      return value.slice(0, 5)
    default:
      return value
  }
}

// xử lí popup mở sort
/**
 * thực hiện mở popsup sort dựa trên col được chọn
 * Create by: NHDuy (13/1/2026)
 */
const showSortPopup = ref(false)
const sortColumn = ref(null)
const sortPosition = ref({ x: 0, y: 0 })
const sortPopupRef = ref(null)
function openSort(col, event) {
  sortColumn.value = col
  showSortPopup.value = true

  const rect = event.target.getBoundingClientRect() //lấy vị trí của col hiện tại
  sortPosition.value = {
    x: rect.left - 12,
    y: rect.bottom + 8,
  }
}
/**
 * hàm applySort
 * nhận giá trị đầu vào là direction và key của sortColumn
 * thực hiện gửi obj chứa sortBy và sortDir lên cha
 * Create by: NHDuy (13/1/2026)
 */
// xử lí sự kiện sort gửi lên cha
const applySort = (direction) => {
  if (!sortColumn.value) return
  emit('sort-change', {
    SortBy: sortColumn.value.key, // key của cột hiện tại
    SortDir: direction, // asc / desc
  })
  showSortPopup.value = false
}
/**
 * hàm clearSort
 * emit lại mảng rỗng để thực hiện sort theo mặc định
 * Create by: NHDuy (13/1/2026)
 */
const clearSort = () => {
  emit('sort-change', {})
  showSortPopup.value = false
}
// xử lí sự kiện open popupfilter

// biến lưu giá trị để emit lên gắn payload
// xử lí dữ liệu form filter
// chứa các field như key col, lọc dạng eq, startswith ,endswith...
const filterValue = reactive({
  field: null,
  operator: null,
  value: null,
})

const showFilterPopup = ref(false) //theo dõi sự mở popup V-if
const filterColumn = ref(null) // biến lưu col được truyền vào khi mở popp
const filterPosition = ref({ x: 0, y: 0 }) // tính khoảng cách dự trên cột được chọn
const filterPopupRef = ref(null) // theo dõi sự kiện click ngoài
// hàm gắn giá trị cho poup truyền vào obj col được chọn
function openFilter(col, event) {
  filterColumn.value = col // biến cục bộ lưu lại thông tin cột
  showFilterPopup.value = true
  filterValue.field = col.key
  const rect = event.target.getBoundingClientRect()
  filterPosition.value = {
    x: rect.left - 70,
    y: rect.bottom + 8,
  }
}
const applyFilter = () => {
  if (!filterColumn.value) return

  let payload = {
    field: filterColumn.value.key,
    operator: null,
    value: null,
  }
  // CASE BOOLEAN (inactive)
  if (filterColumn.value.key === 'inactive') {
    payload.operator = 'eq'
    payload.value = filterValue.operator // true / false
  }
  // CASE KHÁC
  else {
    payload.operator = filterValue.operator
    payload.value = filterValue.value
  }
  emit('apply-filter', payload)
  showFilterPopup.value = false
}
// click ngoài tắt popup
function handleClickOutside(e) {
  // SORT
  if (showSortPopup.value && sortPopupRef.value && !sortPopupRef.value.contains(e.target)) {
    showSortPopup.value = false
    sortColumn.value = null
  }

  // FILTER
  if (showFilterPopup.value && filterPopupRef.value && !filterPopupRef.value.contains(e.target)) {
    showFilterPopup.value = false
    filterColumn.value = null
  }
}
//xử lí tắt popupfilter bằng nút
const unselect = () => {
  showFilterPopup.value = false
  filterColumn.value = null
}
const unfilter = () => {
  if (!filterColumn.value) return
  const field = filterColumn.value.key
  // 🔥 tạo mảng mới = mảng cha gửi xuống TRỪ filter của cột đó
  const newFilters = props.filters.filter((f) => f.field !== field)
  console.log(newFilters)
  emit('apply-filter', newFilters)
  showFilterPopup.value = false
  filterColumn.value = null
}

// xử lí xác định col nào được filter để set cấu hình cho filter dạng text+dropdown hoặc chỉ dropdown...
// đầu vào key của obj đầu ra là obj tương ứng trong filterConfig
const currentFilterType = computed(() => {
  return filterConfig[filterColumn.value?.key]?.type ?? null
})

// bind lại dữ liệu của cột đang lọc
/**
 * Gắn dữ liệu lấy từ mảng filters từ cha gửi xuống
 * obj existedFilter chứa dữ liệu cột đang lọc
 * nếu có dữ liệu bind lại
 * Create by: NHDuy (13/1/2026)
 */
watch(
  () => showFilterPopup.value,
  (show) => {
    if (!show || !filterColumn.value) return

    const existedFilter = props.filters?.find((f) => f.field === filterColumn.value.key)

    if (existedFilter) {
      filterValue.field = existedFilter.field

      // CASE BOOLEAN (inactive)
      if (existedFilter.field === 'inactive') {
        filterValue.operator = existedFilter.value
        filterValue.value = null
      } else {
        filterValue.operator = existedFilter.operator
        filterValue.value = existedFilter.value
      }
    } else {
      filterValue.field = filterColumn.value.key
      filterValue.operator = null
      filterValue.value = null
    }
  },
)

// thay đổi trạng thái icon lọc
const isColumnFiltered = (col) => {
  return props.filters?.some(
    (f) => f.field === col.key && f.operator && f.value !== null && f.value !== '',
  )
}

watch(
  () => filterColumn.value?.key,
  () => {
    filterValue.operator = null
    filterValue.value = null
  },
)
watch(
  () => props.clearSelection,
  () => {
    selectedIds.value = []
  },
)

// bật và xóa sự kiện khi khởi tạo và xóa DOM
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})
onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
  stopResize()
})
</script>

<template>
  <div>
    <!-- popup sort cột  -->
    <div
      v-if="showSortPopup"
      ref="sortPopupRef"
      class="sort-popup"
      :style="{
        left: sortPosition.x + 'px',
        top: sortPosition.y + 'px',
      }"
    >
      <div @click="clearSort" class="item-sort">
        <div class="icon-empty"></div>
        <div>Không sắp xếp</div>
      </div>
      <div @click="applySort('ASC')" class="item-sort">
        <div class="icon-arrow-up"></div>
        <div>Tăng dần</div>
      </div>
      <div @click="applySort('DESC')" class="item-sort">
        <div class="icon-arrow-down"></div>
        <div>Giảm dần</div>
      </div>
      <span
        style="
          position: relative;
          display: flex;
          margin: 8px 12px;
          flex-direction: column;
          align-items: flex-start;
          align-self: stretch;
          height: 1px;
          background: #d1d5db;
        "
      ></span>
      <div class="item-sort">
        <div class="icon-pin"></div>
        <div>Ghim cột</div>
      </div>
      <div class="item-sort">
        <div class="icon-unpin"></div>
        <div>Bỏ ghim cột</div>
      </div>
    </div>

    <!--  popup lọc cột -->
    <div
      v-if="showFilterPopup"
      ref="filterPopupRef"
      class="filter-popup"
      :style="{
        top: filterPosition.y + 'px',
        left: filterPosition.x + 'px',
      }"
    >
      <div class="header-filter">
        <div class="text">Lọc {{ filterColumn.label }}</div>
        <div class="icon-container"><div class="icon-close" @click="unselect"></div></div>
      </div>
      <div class="main-filter">
        <Select
          v-model="filterValue.operator"
          :style="{ width: '100%' }"
          :options="selectOptions[filterColumn.key]"
          placeholder="Chọn điều kiện lọc"
        />
        <input
          v-if="currentFilterType === 'text'"
          v-model="filterValue.value"
          class="text"
          type="text"
          placeholder="Nhập giá trị lọc"
        />
      </div>
      <div class="footer-filter">
        <div class="btn-unfilter" @click="unfilter">Bỏ lọc</div>
        <div class="right-footer">
          <div class="btn-cancel" @click="unselect">Hủy</div>
          <div class="btn-acess" @click="applyFilter">Áp dụng</div>
        </div>
      </div>
    </div>

    <table class="data-table">
      <thead>
        <tr>
          <th
            v-for="(col, index) in localColumns"
            :key="col.key"
            :class="col.class"
            :style="{ width: col.width + 'px', maxWidth: col.maxWidth + 'px' }"
          >
            <!-- CHECKBOX ALL -->
            <div class="th-content">
              <template v-if="col.type === 'checkbox'">
                <Checkbox
                  :modelValue="isAllChecked"
                  binary
                  class="checkbox-mini"
                  @update:modelValue="toggleCheckAll"
                />
              </template>
              <!-- NORMAL HEADER -->
              <template v-else>
                <div class="th-inner">
                  <div @click.stop="openSort(col, $event)">{{ col.label }}</div>
                  <div
                    v-if="col.filterable"
                    :class="isColumnFiltered(col) ? 'filter-icon-select' : 'filter-icon'"
                    @click.stop="openFilter(col, $event)"
                  ></div>
                </div>
              </template>
            </div>
            <span class="col-resizer" @mousedown="startResize($event, index)"></span>
          </th>
        </tr>
      </thead>

      <tbody>
        <tr v-if="props.rows.length === 0">
          <td :colspan="props.columns.length" class="no-data">Không có dữ liệu phù hợp</td>
        </tr>

        <tr
          v-for="row in props.rows"
          :key="row.shiftId"
          :class="{ 'row-select': selectedIds.includes(row.shiftId) }"
        >
          <td v-for="col in localColumns" :key="col.key" :class="col.class">
            <!-- checkbox -->
            <template v-if="col.type === 'checkbox'">
              <Checkbox v-model="selectedIds" :value="row.shiftId" class="checkbox-mini" />
            </template>

            <!-- custom -->
            <template v-else-if="col.type === 'custom'">
              <slot :name="col.key" :row="row" :value="row[col.key]" />
            </template>

            <!-- default -->
            <template v-else>
              {{ renderCell(row[col.key], col) }}
            </template>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style>
.data-table {
  table-layout: fixed;
  border-collapse: collapse;
  width: max-content;
  min-width: 100%;
  font-size: 13px;
}
.data-table th {
  user-select: none;
  position: sticky;
  top: 0px;
  background-color: #f9fafb;
  font-weight: 600;
}
.data-table {
  table-layout: fixed;
}

.th-content {
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
.col-resizer {
  position: absolute;
  right: 0;
  top: 0;
  width: 1px;
  height: 100%;
  cursor: col-resize;
  z-index: 10;
}
th:hover .col-resizer {
  background-color: rgba(0, 0, 0, 0.1);
}
.col-resizer::before {
  content: '';
  position: absolute;
  top: 0;
  left: -1px;
  width: 5px; /* vùng bắt chuột */
  height: 100%;
}
th,
td {
  padding: 0px 16px;
  height: 32px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  border-bottom: 1px solid #ddd;
  box-sizing: border-box;
  vertical-align: middle;
  text-align: left;
  z-index: 1;
  cursor: pointer;
}
tr {
  font-size: 13px !important;
  font-weight: 400;
  position: relative;
  cursor: pointer;
  box-sizing: border-box;
  height: 32px;
}
.col-checkbox {
  width: 40px !important;
  text-align: center;
  position: sticky;
  left: 0;
  cursor: pointer;
  background: #fff;
  padding: 0;
}
th.col-checkbox {
  z-index: 5;
}
thead th {
  position: sticky;
  top: 0;
  background: #f3f4f6;
  z-index: 2;
}
th.col-action {
  cursor: default;
  background-color: #fff;
  z-index: 5;
}
.col-action {
  position: sticky;
  right: 0px;
  width: 100px;
  height: 100%;
  background-color: #fff;
  z-index: 1;
}
tbody tr:hover,
tbody tr:hover .col-checkbox,
tbody tr:hover .col-action {
  background-color: rgb(229, 231, 235);
}
tbody tr.row-select,
tbody tr.row-select .col-action,
tbody tr.row-select .col-checkbox {
  background-color: #a4f6d3;
}
tbody tr.row-select:hover,
tbody tr.row-select:hover .col-action,
tbody tr.row-select:hover .col-checkbox {
  background-color: rgb(229, 231, 235);
}
/* icon hiện khi edit */
.row-action {
  display: flex;
  justify-content: space-around;
  align-items: center;
  height: 100%;
  transition: opacity 0.15s ease;
}
.row-action .icon-container {
  height: 24px;
  width: 24px;
  border-radius: 50%;
  background-color: #fff;
  z-index: 10;
  display: flex;
  align-items: center;
  justify-content: center;
}
.th-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
th:hover .filter-icon {
  opacity: 1;
}
/* xử lí popup sort */
.sort-popup {
  position: fixed;
  background: #fff;
  border: 1px solid #ddd;
  box-shadow:
    0 0 8px #0000001a,
    0 8px 16px #0000001a;
  border-radius: 4px;
  z-index: 9999;
  min-width: 140px;
  box-sizing: border-box;
  height: auto;
  width: 145px;
  padding: 8px 0;
}

.item-sort {
  display: flex;
  flex-direction: row;
  width: 100%;
  padding: 8px 12px;
  height: 32px;
  font-size: 13;
  font-weight: 400;
  gap: 8px;
}

.sort-popup .item-sort:hover {
  background: #f3f4f6;
}
.checkbox-mini {
  transform: scale(0.82);
}
.filter-popup {
  display: flex;
  position: absolute;
  background: #fff;
  border: 1px solid #ddd;
  box-shadow:
    0 0 8px #0000001a,
    0 8px 16px #0000001a;
  border-radius: 4px;
  z-index: 9999;
  min-width: 350px;
  box-sizing: border-box;
  height: auto;
  /* width: 145px; */
  padding: 16px 16px;
  flex-direction: column;
  width: 350px;
  font-weight: 400;
  font-size: 13px;
  gap: 16px;
  overflow: visible !important;
}
.filter-popup .header-filter {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 20px;
}
.header-filter .text {
  font-size: 16px;
  font-weight: 600;
}
.header-filter .icon-container {
  padding: 6px 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  border-radius: 4px;
}
.header-filter .icon-container:hover {
  background-color: #f3f4f6;
}
.footer-filter {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
}
.btn-unfilter {
  padding: 6px 12px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f3f4f6;
  font-weight: 500;
  white-space: nowrap;
  cursor: pointer;
}
.right-footer {
  display: flex;
  flex-direction: row;
  gap: 8px;
}
.right-footer .btn-cancel {
  border: 1px solid #d1d5db;
  color: #111827;
  background-color: #fff;
  padding: 6px 12px;
  border-radius: 4px;
  font-weight: 500;
  white-space: nowrap;
  cursor: pointer;
}
.right-footer .btn-acess {
  color: #fff;
  background-color: rgb(0, 123, 93);
  padding: 6px 12px;
  border-radius: 4px;
  font-weight: 500;
  white-space: nowrap;
  cursor: pointer;
}
.main-filter {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.main-filter .text {
  border: 1px solid #d1d5db;
  border-radius: 6px;
  padding: 6px 12px;
  font-size: 13px;
  box-sizing: border-box;
}
.main-filter .text:focus-visible {
  outline: none;
  border-color: #009b71; /* màu viền bạn muốn */
}
.main-filter .p-select:not(.p-disabled):hover,
.main-filter .text:hover {
  border-color: #9ca3af;
}
.main-filter .p-select-label {
  padding: 5px 12px;
  font-size: 13px;
}
.p-checkbox-checked .p-checkbox-box {
  background-color: #009b71 !important;
}
</style>
