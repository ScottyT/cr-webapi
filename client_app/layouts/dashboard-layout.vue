<template>
  <v-app :dark="appTheme">
    <UiNavigation dark :items="items" :filteredItems="items" :title="title" />
    <v-main>
      <!-- <span v-if="!isLoggedIn"><LazyFormsLogin /></span> -->
      <nuxt class="px-5 mx-auto mt-6 mb-6" />
    </v-main>
    <v-footer dark :fixed="fixed" app>
      <span>&copy; {{ new Date().getFullYear() }}</span>
    </v-footer>
  </v-app>
</template>
<script>
import {mapGetters, mapActions} from 'vuex'
export default {
  //middleware: ['auth'],
  data() {
    return {
      clipped: true,
      drawer: false,
      fixed: false,
      items: [
        {
          icon: 'mdi-apps',
          title: 'Dispatch Report',
          to: '/forms/dispatch-report',
          access: 'user'
        },
        {
          icon: 'mdi-chart-bubble',
          title: 'Rapid Response Report',
          to: '/forms/rapid-response',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'AOB & Mitigation Contract',
          to: '/forms/aob-contract-form',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Upholstery Cleaning Form',
          to: '/forms/upholstery-form',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Daily Containment Case File Report',
          to: '/forms/daily-containment-report',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Daily Technician Case File Report',
          to: '/forms/daily-technician-report',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Sketches',
          to: '/sketches',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Atmospheric Readings',
          to: '/forms/atmospheric-readings',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Moisture Readings',
          to: '/forms/moisture-readings',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Psychrometric Chart',
          to: '/forms/psychrometric-charting',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Unit Quantity and Equipment Inventory',
          to: '/forms/inventory-log',
          access: 'user'
        },
        {
          icon: "mdi-form-select",
          title: "Personal Property Inventory",
          to: "/forms/content-inventory",
          access: "user"
        },
        {
          icon: 'mdi-form-select',
          title: 'Certificate of Completion',
          to: '/forms/certificate-of-completion',
          access: 'user'
        },
        {
          icon: 'mdi-form-select',
          title: 'Quality Control Report',
          to: '/forms/quality-control-report',
          access: 'user'
        },
        {
          icon: 'mdi-clipboard',
          title: 'Field Jacket',
          to: '/field-jacket',
        },
        {
          icon: 'mdi-folder',
          title: 'Storage',
          to: '/storage',
        }
      ],
      miniVariant: false,
      right: true,
      rightDrawer: false,
      title: 'Code Red Claims',
      user: false,
      filteredNavItems: [],
      isMobile: false
    }
  },
  computed: {
    appTheme() {
      return this.$vuetify.theme.dark = false
    },
    matchUrl() {
      return this.$route.path.match(/^(?:^|\W)field-jacket(?:$|\W)(?:\/(?=$))?/gm)
    },
  },
  methods: {
    ...mapActions({
      fetchReports: 'reports/fetchReports',
      fetchLogs: 'reports/fetchLogs'
    }),
    /* itemsArr() {
      const filtered = (role) => this.items.filter((v) => {
        return v.access === role
      })
      switch (this.getUser.role) {
        case "user":
          this.filteredNavItems = filtered("user")
          break;
        case "admin":
          this.filteredNavItems = this.items
      }
    }, */
    onResize() {
      setTimeout(() => {
        this.isMobile = window.innerWidth < 1200
      }, 100)     
    },
  },
  beforeDestroy() {
    if (typeof window === 'undefined') return
    window.removeEventListener('resize', this.onResize, { passive: true })
  },
  mounted() { 
    this.onResize()
    window.addEventListener('resize', this.onResize, { passive: true })
    
    this.$nextTick(() => {
      //this.itemsArr()
    })
  }
}
</script>