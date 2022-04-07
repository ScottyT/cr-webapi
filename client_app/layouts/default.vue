<template>
  <v-app :dark="appTheme">
    <v-overlay :value="isLoading" v-show="isLoading">
      <v-progress-circular
          indeterminate
          size="64"
        ></v-progress-circular>
    </v-overlay>
    <LazyUiNavigation :items="items" :filteredItems="filteredNavItems" :title="title" />
    <v-main :class="matchUrl !== null ? 'reports-page' : ''">
      <!-- <span v-if="!isLoggedIn"><LazyFormsLogin /></span> -->
      <nuxt class="mx-auto"/>
      <v-footer :fixed="fixed" app>
        <span>&copy; {{ new Date().getFullYear() }}</span>
      </v-footer>
    </v-main>
  </v-app>
</template>

<script>
import { computed, defineComponent, reactive, ref, useStore, watch, onMounted, useFetch, useContext } from '@nuxtjs/composition-api'
import useReports from "@/composable/reports";
export default defineComponent({
    setup(props, context) {
        const { $axios, $userReports, $auth } = useContext();
        const store = useStore();
        const fetchReports = () => { store.dispatch("reports/fetchReports"); };
        const fetchCreditCards = (user) => { store.dispatch("reports/fetchCreditCards", user); };
        const fetchUser = () => { store.dispatch("users/fetchUser"); };
        const items = ref([
            {
                icon: "mdi-apps",
                title: "Dispatch Report",
                to: "/forms/dispatch-report",
                access: "user"
            },
            {
                icon: "mdi-chart-bubble",
                title: "Rapid Response Report",
                to: "/forms/rapid-response",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "AOB & Mitigation Contract",
                to: "/forms/aob-contract-form",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Upholstery Cleaning Form",
                to: "/forms/upholstery-form",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Daily Containment Case File Report",
                to: "/forms/daily-containment-report",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Daily Technician Case File Report",
                to: "/forms/daily-technician-report",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Sketches",
                to: "/sketches",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Atmospheric Readings",
                to: "/forms/atmospheric-readings",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Moisture Readings",
                to: "/forms/moisture-readings",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Psychrometric Chart",
                to: "/forms/psychrometric-charting",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Unit Quantity and Equipment Inventory",
                to: "/forms/inventory-log",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Personal Property Inventory",
                to: "/forms/content-inventory",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Certificate of Completion",
                to: "/forms/certificate-of-completion",
                access: "user"
            },
            {
                icon: "mdi-form-select",
                title: "Quality Control Report",
                to: "/forms/quality-control-report",
                access: "user"
            },
            {
                icon: "mdi-clipboard",
                title: "Field Jacket",
                to: "/field-jacket",
                access: "admin"
            },
            {
                icon: "mdi-folder",
                title: "Storage",
                to: "/storage",
                access: "admin"
            }
        ]);
        const clipped = ref(true);
        const drawer = ref(false);
        const fixed = ref(false);
        const miniVariant = ref(false);
        const right = ref(true);
        const rightDrawer = ref(false);
        const title = ref("Code Red Claims");
        const user = ref(false);
        const filteredNavItems = ref([]);
        const appTheme = computed(() => context.root.$vuetify.theme.dark = true);
        const matchUrl = computed(() => context.root.$route.path.match(/^(?:^|\W)field-jacket(?:$|\W)(?:\/(?=$))?/gm));
        const isOnline = computed(() => context.root.$nuxt.isOnline);
        const isLoading = computed(() => store.state.users.loading);
        const getUser = computed(() => store.state.users.user)
        const isLoggedIn = computed(() => store.getters["users/isLoggedIn"])
        const overlay = ref(false);
        const filtered = (role) => items.value.filter((v) => {
            return v.access === role;
        });
        const itemsArr = () => {
            if (!isLoggedIn.value) return;
            switch (getUser.value.role) {
                case "user":
                    filteredNavItems.value = filtered("user");
                    fetchReports();
                    break;
                case "admin":
                    filteredNavItems.value = items.value;
                    fetchReports();
            }
        };
        onMounted(itemsArr);
        fetchCreditCards($auth.user);
        watch(() => getUser.value, (val) => {
            if (Object.keys(val).length !== 0) {
                itemsArr();
                //$userReports.fetchReports(val.email)
            }
        });

        onMounted(fetchUser)
        return {
            items,
            clipped,
            drawer,
            fixed,
            miniVariant,
            right,
            rightDrawer,
            title,
            filteredNavItems,
            appTheme,
            matchUrl,
            isOnline,
            isLoading,
            overlay,
            isLoggedIn
        };
    }
})
</script>