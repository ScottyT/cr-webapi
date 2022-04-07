import Vue from 'vue';
import { IMaskDirective } from 'vue-imask';
import {IMaskComponent} from 'vue-imask';
import VueMask from 'v-mask';

Vue.directive('imask', IMaskDirective);
Vue.component('imask-input', IMaskComponent);

Vue.use(VueMask)