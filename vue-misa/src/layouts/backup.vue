<script setup>
// kiểm tra trạng thái thu gọn của sidebar từ localStorage
import { ref, onMounted } from 'vue'

const STORAGE_KEY = 'sidebar-collapsed'
const isCollapsed = ref(false)

/* ================= đóng mở sibabar ================= */
const toggleSidebar = () => {
  isCollapsed.value = !isCollapsed.value
  localStorage.setItem(STORAGE_KEY, isCollapsed.value)
}
/* ================= Setclass Active khi click ================= */
const activeItem = ref('item-icon-menu')
const setActive = (key) => {
  activeItem.value = key
}

/* ================= LIFECYCLE ================= */
onMounted(() => {
  isCollapsed.value = localStorage.getItem(STORAGE_KEY) === 'true'
})

/* ================= Dropd ================= */
const isPlanOpen = ref(false)

const togglePlan = () => {
  isPlanOpen.value = !isPlanOpen.value
  setActive('calendar')
}
</script>

<template>
  <div class="sidebar" :class="{ collapsed: isCollapsed }">
    <div class="list-items-container">
      <div class="list-items display-flex align-items-center flex-direction-column">
        <div
          class="sidebar-item"
          :class="{ active: activeItem === 'recruitment' }"
          @click="setActive('recruitment')"
        >
          <div class="item-content">
            <div class="item-icon-dashboard"></div>
            <div class="item-text display-flex align-items-center">Tổng quan</div>
          </div>
        </div>
        <div
          class="sidebar-item"
          :class="{ active: activeItem === 'candidate' }"
          @click="setActive('candidate')"
        >
          <div class="item-content">
            <div class="item-icon-store"></div>
            <div class="item-text display-flex align-items-center">Đơn đặt hàng</div>
          </div>
        </div>
        <div
          class="sidebar-item"
          :class="{ active: activeItem === 'calendar' }"
          @click="togglePlan"
        >
          <div class="item-content">
            <div class="item-icon-plan"></div>
            <div class="item-text display-flex align-items-center">Kế hoạch sản xuất</div>
            <div class="icon-down-2"></div>
          </div>
        </div>

        <div class="sidebar-item" v-show="isPlanOpen">
          <div class="item-content">
            <div class="icon-cursor"></div>
            <div class="item-text display-flex align-items-center">Kế hoạch tổng thể</div>
          </div>
        </div>
        <div class="sidebar-item" v-show="isPlanOpen">
          <div class="item-content">
            <div class="icon-cursor"></div>
            <div class="item-text display-flex align-items-center">Kế hoạch chi tiết</div>
          </div>
        </div>
        <div class="sidebar-item" v-show="isPlanOpen">
          <div class="item-content">
            <div class="icon-cursor"></div>
            <div class="item-text display-flex align-items-center">Kế hoạch nguyên vật liệu</div>
          </div>
        </div>
        <div class="sidebar-item" v-show="isPlanOpen">
          <div class="item-content">
            <div class="icon-cursor"></div>
            <div class="item-text display-flex align-items-center">Yêu cầu mua NVL</div>
          </div>
        </div>
        <div
          class="sidebar-item"
          :class="{ active: activeItem === 'sb-talent' }"
          @click="setActive('sb-talent')"
        >
          <div class="item-content">
            <div class="item-icon-coordinate"></div>
            <div class="item-text display-flex align-items-center">Điều phối và thực thi</div>
            <div class="icon-down-2"></div>
          </div>
        </div>
        <div
          class="sidebar-item"
          :class="{ active: activeItem === 'item-icon-control' }"
          @click="setActive('item-icon-control')"
        >
          <div class="item-content">
            <div class="item-icon-control"></div>
            <div class="item-text display-flex align-items-center">Kiểm tra chất lượng</div>
            <div class="icon-down-2"></div>
          </div>
        </div>
        <div class="sidebar-item" :class="{ active: activeItem === '1' }" @click="setActive('1')">
          <div class="item-content">
            <div class="item-icon-repo"></div>
            <div class="item-text display-flex align-items-center">Kho vật tư</div>
            <div class="icon-down-2"></div>
          </div>
        </div>
        <div class="sidebar-item" :class="{ active: activeItem === '2' }" @click="setActive('2')">
          <div class="item-content">
            <div class="item-icon-job"></div>
            <div class="item-text display-flex align-items-center">Giao việc</div>
            <div class="icon-down-2"></div>
          </div>
        </div>
        <div class="sidebar-item" :class="{ active: activeItem === '3' }" @click="setActive('3')">
          <div class="item-content">
            <div class="item-icon-file-search"></div>
            <div class="item-text display-flex align-items-center">Truy xuất nguồn gốc</div>
          </div>
        </div>

        <div class="item-line"></div>
        <div class="sidebar-item" :class="{ active: activeItem === '4' }" @click="setActive('4')">
          <div class="item-content">
            <div class="item-icon-report"></div>
            <div class="item-text display-flex align-items-center">Báo cáo</div>
          </div>
        </div>
        <div class="item-line"></div>
        <div class="sidebar-item" :class="{ active: activeItem === '5' }" @click="setActive('5')">
          <div class="item-content">
            <div class="item-icon-product-material"></div>
            <div class="item-text display-flex align-items-center">Sản phẩm, NVL</div>
            <div class="icon-down-2"></div>
          </div>
        </div>
        <div class="sidebar-item" :class="{ active: activeItem === '6' }" @click="setActive('6')">
          <div class="item-content">
            <div class="item-icon-process"></div>
            <div class="item-text display-flex align-items-center">Quy trình sản xuất</div>
            <div class="icon-down-2"></div>
          </div>
        </div>
        <div class="sidebar-item" :class="{ active: activeItem === '7' }" @click="setActive('7')">
          <div class="item-content">
            <div class="item-icon-production-capacity"></div>
            <div class="item-text display-flex align-items-center">Năng lực sản xuất</div>
            <div class="icon-down-2"></div>
          </div>
        </div>
        <div
          class="sidebar-item"
          :class="{ active: activeItem === 'item-icon-menu' }"
          @click="setActive('item-icon-menu')"
        >
          <div class="item-content">
            <div class="item-icon-menu"></div>
            <div class="item-text display-flex align-items-center">Danh mục khác</div>
            <div class="icon-arrow-right"></div>
          </div>
        </div>
        <div class="sidebar-item" :class="{ active: activeItem === '8' }" @click="setActive('8')">
          <div class="item-content">
            <div class="item-icon-production-systom"></div>
            <div class="item-text display-flex align-items-center">Thiết lập</div>
            <div class="icon-arrow-right"></div>
          </div>
        </div>
      </div>
      <div class="bottom-area" @click="toggleSidebar">
        <div class="bottom-container display-flex align-items-center">
          <div class="icon" :class="{ collapsed: isCollapsed }"></div>
          <div class="text">{{ isCollapsed ? '' : 'Thu gọn' }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
