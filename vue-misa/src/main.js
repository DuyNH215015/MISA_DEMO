import { createApp } from 'vue'
import App from './App.vue'
import './assets/css/main.css'
import router from './router'
import PrimeVue from 'primevue/config'
import Lara from '@primevue/themes/lara'

// icon
import 'primeicons/primeicons.css'
const app = createApp(App)
app.use(router)
app.use(PrimeVue, {
  theme: {
    preset: Lara,
  },
})

app.mount('#app')
