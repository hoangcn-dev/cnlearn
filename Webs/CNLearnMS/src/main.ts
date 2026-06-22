import './assets/styles/main.css'
import 'ant-design-vue/dist/reset.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import Antd, { message } from 'ant-design-vue'
import { createPinia } from 'pinia'

message.config({
  maxCount: 1,
});

const app = createApp(App)
app.use(createPinia())
app.use(router)
app.use(Antd)

app.mount('#app')
