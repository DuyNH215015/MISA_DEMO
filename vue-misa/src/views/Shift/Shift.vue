<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import {
  getShifts,
  addShift,
  updateShift,
  updateInactiveShifts,
  deleteManyShifts,
} from '@/api/shift-api' // import API của bạn
import { toTimeWithSeconds, normalizeShiftPayload } from '@/utils/format'
import { pageSizeOptions } from '@/data/form-select-options'
import { validateForm } from '@/utils/validate-shift'
import Select from '@/components/dropdown/MsDropDown.vue'
import ShiftTable from '@/views/Shift/ShiftTable.vue'
import PopupEdit from './ShiftPopupEdit.vue'
import PopupAdd from './ShiftPopupAdd.vue'
import Toast from '@/components/toast/Toast.vue'
import ToastWarningValidate from '@/components/toast/ToastWarningValidate.vue'
import ToastWarningDelete from '@/components/toast/ToastWarningDelete.vue'
// --- state ---
const shifts = ref([]) // dữ liệu table
const searchText = ref('') // text search
const pageSize = ref(10) // số record 1 trang
const currentPage = ref(1) // trang hiện tại
const totalRecords = ref(0) // tổng số record
const totalPages = computed(() => Math.ceil(totalRecords.value / pageSize.value)) // số trang được tính
const sortBy = ref('') // sort column
const sortDir = ref('') // sort asc || desc
const shiftId = ref() // lưu shiftId obj được chọn
const selectedIds = ref([]) // lưu mảng shiftId được chọn
// Popup
// const selectedShiftId = ref(null)

/**
 * Gán dữ liệu vào payload
 * Trả về mảng Payload
 * Create by: NHDuy (13/1/2026)
 */
const payload = computed(() => ({
  PageNumber: currentPage.value,
  PageSize: pageSize.value,
  Keyword: inactiveFromSearch.value.keyword || '',
  SortBy: sortBy.value || 'createdDate',
  SortDir: sortDir.value || 'desc',
  Inactive: inactiveFromSearch.value.inactive,
  Filters: filters.value.length ? filters.value : null,
}))

/**
 * Funtion Loadshifts
 * Nhận vào mảng payload
 * Trả về mảng shifts gồm các obj
 * Create by: NHDuy (13/1/2026)
 */
async function loadShifts() {
  try {
    const res = await getShifts(payload.value)
    shifts.value = res.data || []
    totalRecords.value = res.total || 0
  } catch (error) {
    console.error('API getShifts error:', error)
  }
}

/**
 * Hàm tính toán phân trang
 * Create by: NHDuy (13/1/2026)
 */
function prevPage() {
  if (currentPage.value > 1) currentPage.value--
}
function nextPage() {
  if (currentPage.value < totalPages.value) currentPage.value++
}

/**
 * Hàm tính toán phân trang
 * Create by: NHDuy (13/1/2026)
 */
const startRecord = computed(() =>
  totalRecords.value === 0 ? 0 : (currentPage.value - 1) * pageSize.value + 1,
)
const endRecord = computed(() => Math.min(currentPage.value * pageSize.value, totalRecords.value))

/**
 * Hàm onsortchange thay
 * Nhận vào 2 giá trị SortBy (columnName) || SortDir (ASC/DESC)
 * Trả về mảng payload mới và gọi Loadshift
 * Create by: NHDuy (13/1/2026)
 */
const onSortChange = ({ SortBy, SortDir }) => {
  payload.value.SortBy = SortBy
  payload.value.SortDir = SortDir
  currentPage.value = 1
  loadShifts()
}

/**
 * Hàm inactiveFromSearch
 * xử lí sự kiện tìm kiếm trạng thái của inacive theo keywork
 * Nhận vào giá trị searchText có 2 từ đang hoặc ngừng
 * Trả về mảng payload mới và gọi có giá trị Inactive tương ứng
 * Create by: NHDuy (13/1/2026)
 */
const inactiveFromSearch = computed(() => {
  const text = searchText.value?.toLowerCase() || ''
  if (text.includes('đang')) {
    return {
      inactive: true,
      keyword: null,
    }
  }
  if (text.includes('ngừng')) {
    return {
      inactive: false,
      keyword: null,
    }
  }
  return {
    inactive: null,
    keyword: searchText.value,
  }
})

/**  hàm onSelectedIds
 * Lưu mảng các giá trị ID được chọn được gửi lên từ MsBaseTable
 * Create by: NHDuy (13/1/2026)
 */
function onSelectedIds(ids) {
  selectedIds.value = ids
}

/**
 * hàm unSelectIds
 * xử lí sự kiện unslect
 * chuyển mảng selectedIds về null và props lại xuống Msbasetable
 * Create by: NHDuy (13/1/2026)
 */
const clearSelection = ref(false)
const unSelectIds = () => {
  selectedIds.value = []
  clearSelection.value = !clearSelection.value
}
/**
 * xử lí sự kiện chuyển toàn bộ cột được chọn sang inactive true || false
 * selectedShifts biến lưu các obj được chọn lấy
 * đầu vào là mảng shifts gồm các obj ,và selectedIds gồm các id được chọn
 * Create by: NHDuy (13/1/2026)
 */
const selectedShifts = computed(() => {
  return shifts.value.filter((s) => selectedIds.value.includes(s.shiftId))
})
/**
 * xử lí sự kiện chuyển toàn bộ cột được chọn sang inactive true || false
 * Đầu vào selectedShifts
 * kiểm tra giá trị inactive của các obj để hiển thị trên UI
 * Create by: NHDuy (13/1/2026)
 */
const allInactive = computed(
  () => selectedShifts.value.length > 0 && selectedShifts.value.every((s) => s.inactive === false),
)
const allActive = computed(
  () => selectedShifts.value.length > 0 && selectedShifts.value.every((s) => s.inactive === true),
)
const allUsing = computed(
  () => selectedShifts.value.length > 0 && selectedShifts.value.every((s) => s.inactive === true),
)
const allStopped = computed(
  () => selectedShifts.value.length > 0 && selectedShifts.value.every((s) => s.inactive === false),
)
const showUseButton = computed(() => {
  // tất cả đang ngừng sử dụng → cho phép Sử dụng
  if (allStopped.value) return true
  // trộn trạng thái → vẫn cho Sử dụng
  if (!allUsing.value && !allStopped.value) return true
  return false
})
const showStopUseButton = computed(() => {
  // tất cả đang sử dụng → cho phép Ngừng sử dụng
  if (allUsing.value) return true

  // trộn trạng thái → vẫn cho Ngừng sử dụng
  if (!allUsing.value && !allStopped.value) return true
  return false
})
/**
 * xử lí sự kiện chuyển toàn bộ cột được chọn sang inactive true
 * Đầu vào selectedIds mảng được chọn
 * inactive: true
 * Create by: NHDuy (13/1/2026)
 */
const handleBatchStartUsing = async () => {
  await updateInactiveShifts({
    ids: selectedIds.value,
    inactive: true,
  })
  loadShifts()
}
/**
 * xử lí sự kiện chuyển toàn bộ cột được chọn sang inactive false
 * Đầu vào selectedIds mảng được chọn
 * inactive: false
 * Create by: NHDuy (13/1/2026)
 */
const handleBatchStopUsing = async () => {
  await updateInactiveShifts({
    ids: selectedIds.value,
    inactive: false,
  })
  loadShifts()
}
/**
 * xử lí sự kiện xóa các cột được chọn
 * Đầu vào selectedIds mảng được chọn và gọi cảnh báo
 * Create by: NHDuy (13/1/2026)
 */
const handleDeleteMany = async () => {
  openPopupWaringDel()
}

/**
 * xử lí sự kiện lọc cột
 * Create by: NHDuy (13/1/2026)
 */
const filters = ref([]) // biến lưu giá trị filterValue từ MsTable gửi lên có field,operaor và value
const handleFilter = (payload) => {
  // 🧹 BỎ LỌC
  if (Array.isArray(payload)) {
    filters.value = payload
    currentPage.value = 1
    loadShifts()
    return
  }
  // ✅ ÁP DỤNG FILTER
  filters.value = filters.value.filter((f) => f.field !== payload.field)
  filters.value.push(payload)
  currentPage.value = 1
  loadShifts()
}

/**
 * xử lí sự kiện reload lại bảng bằng nút
 * gọi đến loadshift() truyền giá trị mặc định
 * Create by: NHDuy (13/1/2026)
 */
const reloadTable = () => {
  searchText.value = null
  loadShifts()
}

/**
 * xử lí sự kiện hiện thông báo nhanh
 * truyền vào msg để gọi đến Toast thông báo nhanh
 * Create by: NHDuy (13/1/2026)
 */
const toastMessage = ref('')
const showToast = (msg) => {
  toastMessage.value = msg
}
/**
 * xử lí sự kiện hiện thông báo lỗi
 * truyền vào msg để gọi đến ToastVal nếu có lỗi
 * đầu vào là msg
 * Create by: NHDuy (13/1/2026)
 */
const PopupWaringVal = ref(false)
const toastMessageVal = ref('')
const deleteShiftCode = ref('')
const openDuplicateShiftWarning = () => {
  toastMessageVal.value = 'Mã ca làm việc đã tồn tại. Vui lòng kiểm tra lại mã ca.'
  PopupWaringVal.value = true
}
const closePopupWaringval = () => {
  PopupWaringVal.value = false
}
/**
 * xử lí sự kiện hiện thông báo xác nhận xóa nhiều dòng
 * Create by: NHDuy (13/1/2026)
 */
const PopupWaringDel = ref(false)
const closePopupWaringDel = () => {
  PopupWaringDel.value = false
}
const openPopupWaringDel = () => {
  if (!selectedIds.value.length) return
  PopupWaringDel.value = true
}
const handleConfirmWaringDel = async () => {
  PopupWaringDel.value = false
  try {
    await deleteManyShifts(selectedIds.value)
    // ✅ toast thành công
    showToast('Xóa ca làm việc thành công')
    loadShifts()
    selectedIds.value = []
  } catch (error) {
    showToast('Xóa ca làm việc thất bại', 'error')
    console.error(error)
  }
}

/**
 * xử lí sự kiênj mở popupadd
 * Create by: NHDuy (13/1/2026)
 */
const showAddPopup = ref(false)
const openAddPopup = () => {
  showAddPopup.value = true
}
const closeAddPopup = () => {
  showAddPopup.value = false
  cloningShift.value = null
}

/**
 * xử lí sự kiện thêm mới ca làm
 * dữ liệu đầu vào formData được emit từ popupedit
 * gọi đến addShift
 * đầu ra là null nếu succes và trả về Toast
 * Nếu lỗi do trùng tên ca gọi ToastVal truyền vào Mess (trùng tên ca)
 * Create by: NHDuy (13/1/2026)
 */
const handleAddSuccess = async (formData) => {
  const validateMessage = validateForm(formData)
  if (validateMessage) {
    toastMessageVal.value = validateMessage
    deleteShiftCode.value = ''
    PopupWaringVal.value = true
    return
  }
  try {
    const payload = normalizeShiftPayload(formData)
    deleteShiftCode.value = formData.ShiftCode
    const res = await addShift(payload)
    if (res.status === 201) {
      cloningShift.value = null
      closeAddPopup()
      showToast('Thêm ca làm việc thành công')
      loadShifts()
    }
  } catch (error) {
    const res = error.response
    if (res?.status === 400 && res.data?.message === 'ShiftCodeDuplicate') {
      openDuplicateShiftWarning()
      return // ❗ không đóng popup
    }
    showToast('Có lỗi xảy ra, vui lòng thử lại', 'error')
  }
}
const handleSaveAndAdd = async (formData) => {
  const validateMessage = validateForm(formData)
  if (validateMessage) {
    toastMessageVal.value = validateMessage
    deleteShiftCode.value = ''
    PopupWaringVal.value = true
    return
  }
  try {
    const payload = normalizeShiftPayload(formData)
    const res = await addShift(payload)
    if (res.status === 201) {
      cloningShift.value = null
      showToast('Thêm ca làm việc thành công')
      loadShifts()
    }
  } catch (error) {
    const res = error.response

    if (res?.status === 400 && res.data?.message === 'ShiftCodeDuplicate') {
      openDuplicateShiftWarning()
      return
    }
    showToast('Có lỗi xảy ra, vui lòng thử lại', 'error')
  }
}

/**
 * xử lí sự kiện sửa ca làm
 * dữ liệu đầu vào là id được gửi từ MsbaseTable của obj được chọn
 * lấy dữ liệu luuw vào editingShift và truyền xuống PopupEdit
 * Create by: NHDuy (13/1/2026)
 */
const showEditPopup = ref(false)
const editingShift = ref(null) // biến lưu obj đang được edit
//đóng popupedit
const closeEditPopup = () => {
  showEditPopup.value = false
}
//lấy dữ liệu ra biến và mở popupedit
function openEditPopup(id) {
  const shift = shifts.value.find((s) => s.shiftId === id)
  if (!shift) return
  editingShift.value = { ...shift } // clone để tránh sửa trực tiếp list
  showEditPopup.value = true
}
const handleUpdateShift = async ({ id, formData }) => {
  const validateMessage = validateForm(formData)
  if (validateMessage) {
    toastMessageVal.value = validateMessage
    deleteShiftCode.value = ''
    PopupWaringVal.value = true
    return
  }
  try {
    const payload = normalizeShiftPayload(formData)
    const res = await updateShift(id, payload)
    if (res.status === 200) {
      closeEditPopup()
      showToast('Sửa ca làm việc thành công')
      loadShifts()
    }
  } catch (error) {
    const res = error.response
    // ❗ Trùng mã ca
    if (res?.status === 400 && res.data?.message === 'ShiftCodeDuplicate') {
      openDuplicateShiftWarning()
      return // ❗ không đóng popup
    }
    showToast('Có lỗi xảy ra, vui lòng thử lại', 'error')
    console.error(error)
  }
}
const handleSaveAndAddByPopupEdit = async ({ id, formData }) => {
  const validateMessage = validateForm(formData)
  if (validateMessage) {
    toastMessageVal.value = validateMessage
    deleteShiftCode.value = ''
    PopupWaringVal.value = true
    return
  }
  try {
    //  Chuẩn hóa payload giống update
    const payload = normalizeShiftPayload(formData)
    const res = await updateShift(id, payload)
    if (res.status === 200) {
      closeEditPopup()
      showToast('Sửa ca làm việc thành công')
      loadShifts()
      openAddPopup()
    }
  } catch (error) {
    const res = error.response
    if (res?.status === 400 && res.data?.message === 'ShiftCodeDuplicate') {
      openDuplicateShiftWarning()
      return // ❗ KHÔNG đóng popup edit
    }
    showToast('Có lỗi xảy ra, vui lòng thử lại', 'error')
    console.error(error)
  }
}

/**
 * xử lí sự kiện nhân bản ca làm việc
 * dữ liệu đầu vào là id được gửi từ MsbaseTable của obj được chọn
 * lấy dữ liệu luuw vào cloningShift và truyền xuống PopupAdd
 * Create by: NHDuy (13/1/2026)
 */
const cloningShift = ref(null)
const handleCloneShift = (id) => {
  const shift = shifts.value.find((s) => s.shiftId === id)
  if (!shift) return
  const clonedShift = {
    ...shift,
    ShiftCode: '', // ❌ không clone mã ca
    ShiftId: undefined,
  }
  cloningShift.value = clonedShift
  openAddPopup()
}
// xử lí chuyển đổi trạng thái ở nút more
const handleChangeInactive = async ({ id, inactive }) => {
  try {
    const payload = {
      ids: [id],
      inactive: inactive,
    }
    await updateInactiveShifts(payload)
    showToast(inactive ? 'Sử dụng ca làm việc' : 'Ngưng sử dụng ca làm việc', 'success')
    loadShifts()
  } catch (error) {
    const res = error.response
    showToast(res?.data?.message ?? 'Có lỗi xảy ra', 'error')
  }
}
const handleDeleteRow = async ({ id }) => {
  try {
    await deleteManyShifts([id])
    showToast('Xóa ca làm việc thành công')
    loadShifts()
  } catch (error) {
    console.error(error)
  }
}
// --- watchers ---
watch([searchText], () => {
  currentPage.value = 1
  loadShifts()
})
watch([currentPage], () => {
  loadShifts()
})
watch(pageSize, (newVal, oldVal) => {
  if (newVal !== oldVal) {
    currentPage.value = 1
    loadShifts()
  }
})
// --- mounted ---
onMounted(() => {
  loadShifts()
})
</script>

<template>
  <div class="main-content">
    <!-- Toast -->
    <Toast :message="toastMessage" @close="toastMessage = ''" />
    <!-- ToastVal -->
    <ToastWarningValidate
      :show="PopupWaringVal"
      :shiftCode="deleteShiftCode"
      :message="toastMessageVal"
      @close="closePopupWaringval"
    />
    <!-- ToastWarningDeleteMany -->
    <ToastWarningDelete
      :show="PopupWaringDel"
      title="Xóa ca làm việc"
      message="Các Ca làm việc sau khi bị xóa sẽ không thể khôi phục. Bạn có muốn tiếp tục xóa không?"
      @close="closePopupWaringDel"
      @confirm="handleConfirmWaringDel"
    />
    <!-- Popup Add -->
    <PopupAdd
      v-if="showAddPopup"
      :cloneData="cloningShift"
      @close="closeAddPopup"
      @success="handleAddSuccess"
      @saveandadd="handleSaveAndAdd"
    />
    <!-- Popup edit -->
    <PopupEdit
      v-if="showEditPopup"
      :shiftEditvalue="editingShift"
      @close="closeEditPopup"
      @updateShift="handleUpdateShift"
      @saveandadd="handleSaveAndAddByPopupEdit"
    />

    <div class="list-candidate-center display-flex flex-direction-column">
      <div class="title-header">
        <div class="title-header-left">Ca làm việc</div>
        <div class="title-header-right">
          <div class="add-people" id="add-people" @click="openAddPopup">
            <div class="icon-add"></div>
            <div class="title-name-add display-flex align-items-center">Thêm</div>
          </div>
        </div>
      </div>
      <div class="candidate-wrapper">
        <div
          class="candidate-wrapper-content display-flex flex-direction-column flex1 justify-content-between"
        >
          <div class="toolbar-grid-container">
            <div class="toolbar-grid-default">
              <div class="toolbar-grid-left">
                <div class="search-grid">
                  <div class="icon-search-conainer">
                    <div class="icon-search"></div>
                  </div>
                  <input
                    type="text"
                    maxlength="255"
                    placeholder="Tìm kiếm"
                    class="search-grid-text"
                    id="search-grid"
                    v-model="searchText"
                  />
                </div>
                <div class="feature-batch" v-show="selectedIds.length">
                  <div class="select-count">
                    Đã chọn <span style="font-weight: 700">{{ selectedIds.length }}</span>
                  </div>
                  <div class="unselect" @click="unSelectIds">Bỏ chọn</div>
                  <div v-if="showUseButton" class="btn btn-using" @click="handleBatchStartUsing">
                    <div class="icon-using"></div>
                    Sử dụng
                  </div>
                  <div
                    v-if="showStopUseButton"
                    class="btn btn-unusing"
                    @click="handleBatchStopUsing"
                  >
                    <div class="icon-unusing"></div>
                    Ngừng sử dụng
                  </div>
                  <div class="btn btn-delete" @click="handleDeleteMany">
                    <div class="icon-delete"></div>
                    Xóa
                  </div>
                </div>
              </div>
              <div class="toolbar-grid-right">
                <div class="icon-container">
                  <div class="icon-reload" title="lấy lại dữ liệu" @click="reloadTable"></div>
                </div>
              </div>
            </div>
          </div>
          <span style="width: 100%; height: 1px; background-color: #ddd"></span>
          <!-- data grid content -->
          <div class="grid-content">
            <div class="data-grid-container display-flex flex-direction-column">
              <!--  dữ liệu phần bảng  -->
              <div class="table-content">
                <ShiftTable
                  :shifts="shifts"
                  :current-page="currentPage"
                  :page-size="pageSize"
                  :search-text="searchText"
                  :clear-selection="clearSelection"
                  :filters="filters"
                  @sort-change="onSortChange"
                  @selectedIds="onSelectedIds"
                  @edit-row="openEditPopup"
                  @apply-filter="handleFilter"
                  @clone="handleCloneShift"
                  @change-status="handleChangeInactive"
                  @delete-row="handleDeleteRow"
                />
              </div>
            </div>
          </div>

          <!-- paging -->
          <div class="paging display-flex flex-direction-row justify-content-between">
            <div class="text" style="font-size: 13px">
              Tổng số:
              <span class="total" style="font-weight: 700" id="total"> {{ totalRecords }}</span>
            </div>
            <div class="right-paging display-flex flex-direction-rows align-items-center">
              <div class="text-record">Số dòng/trang</div>
              <div class="drop-down-record">
                <Select
                  v-model="pageSize"
                  :options="pageSizeOptions"
                  placeholder="10"
                  style="width: 80px"
                />
              </div>
              <div class="text-paging">
                <div class="start" id="start">{{ startRecord }}</div>
                <span style="width: 5px; height: 1; border: 1px solid black"></span>
                <div class="end" id="end">{{ endRecord }}</div>
                <span> bản ghi </span>
              </div>
              <div class="icon-left-right">
                <div
                  class="icon-left"
                  style="margin-left: 10px"
                  id="icon-left"
                  @click="prevPage"
                ></div>
                <div class="icon-right" id="icon-right" @click="nextPage"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<style></style>
