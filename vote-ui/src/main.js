import Vue from 'vue'
import './plugins/vuetify'
import router from './routes/index'
import App from './App.vue'
import VueRouter from 'vue-router';

Vue.config.productionTip = false
Vue.use(VueRouter);

new Vue({
  router,
  render: h => h(App),
}).$mount('#app')
