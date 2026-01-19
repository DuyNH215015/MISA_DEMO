<script setup>
import TheHeader from '@/layouts/TheHeader.vue'
import TheSidebar from '@/layouts/TheSidebar.vue'
</script>

<template>
  <div class="body">
    <div class="container display-flex flex-direction-column">
      <TheHeader></TheHeader>
      <div class="content display-flex flex1">
        <TheSidebar></TheSidebar>
        <router-view />
      </div>
    </div>
  </div>
</template>

<style></style>
