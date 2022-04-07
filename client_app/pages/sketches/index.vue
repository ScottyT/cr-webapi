<template>
  <div class="sketch-forms pa-6">
    <UiBreadcrumbs page="Sketches" />
    <h1>Sketch Forms</h1>
    <ul class="menu-items">
      <li class="menu-items__item" v-for="form in sketchForms" :key="form.value">
        <nuxt-link :to="`/sketches/${form.value}`">{{form.text}}</nuxt-link>
      </li>
    </ul>
  </div>
</template>
<script>
import {mapGetters} from 'vuex';
export default {
    layout: 'default',
    head() {
        return {
            title: "Sketch Forms"
        }
    },
    middleware: 'auth',
    data() {
        return {
            sketchForms: [
                { value: 'moisture-sketch', text: 'Moisture Mapping Location and Sketch' },
                { value: 'measurements-sketch', text:'Measurements and Sketch' },
                { value: 'equipment-location-sketch', text: 'Equipment Location and Sketch' },
                { value: 'soot-sketch', text: 'Soot Sketch' },
                { value: 'roof-sketch', text: 'Roof Sketch' },
                { value: 'exterior-sketch', text: 'Exterior Sketch' }
            ],
        }
    },
    computed: {
        ...mapGetters({
            isLoggedIn: 'isLoggedIn'
        })
    }
}
</script>