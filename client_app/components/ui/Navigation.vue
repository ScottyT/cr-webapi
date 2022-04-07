<template>
    <div>
        <v-navigation-drawer v-model="drawer" :mini-variant="miniVariant" clipped open app :width="$vuetify.breakpoint.width < 991 ? 500 : 425" class="pl-3">
            <button v-if="$vuetify.breakpoint.width < 1200" type="button" class="button__icon" icon @click.stop="drawer = !drawer">
                <v-icon :size="$vuetify.breakpoint.width < 991 ? 55 : 36">mdi-chevron-left</v-icon>
            </button>
            <v-list class="nav-list">
                <span v-if="!isLoggedIn">
                    <v-skeleton-loader type="list-item" height="50px" width="200" v-for="(item, i) in items" :key="`item-${i}`"></v-skeleton-loader>
                </span>
                <nuxt-link class="nav-list-item" exact v-for="(item, i) in filteredItems" :key="i" :to="item.to" v-else>
                    <div class="nav-list-item__icon">
                        <v-icon :size="30">{{ item.icon }}</v-icon>
                    </div>

                    <div class="nav-list-item__content">
                        <p class="nav-list-item__title">{{item.title}}</p>
                    </div>
                </nuxt-link>
            </v-list>
        </v-navigation-drawer>
        <v-app-bar :dark="dark" :clipped-left="true" fixed app extension-height="60" height="80" class="header-navigation">
            <div class="d-flex align-center">
                <button type="button" aria-label="Toggle navigation" @click.stop="drawer = !drawer" class="button__icon button__icon--nav">
                    <span>
                        <i class="button__icon-content button__icon-content--top"></i>
                        <i class="button__icon-content button__icon-content--middle"></i>
                        <i class="button__icon-content button__icon-content--bottom"></i>
                    </span>
                </button>

                <nuxt-link class="v-toolbar__title ml-4" to="/">{{title}}</nuxt-link>
            </div>
            <span v-if="!$auth.loggedIn" class="d-flex flex-row">
                <a @click="$auth.loginWith('auth0')">Sign in</a>
            </span>
            
            <UiProfileDropdown v-else />
            <!-- <ul class="menu-items" v-if="!isMobile">
            </ul> -->
        </v-app-bar>
    </div>
</template>
<script>
import { defineComponent, toRefs, useStore, ref, computed, useContext } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        items: Array,
        filteredItems: Array,
        title: String,
        dark: Boolean
    },
    setup(props, context) {
        const store = useStore()
        const { $auth } = useContext();
        const { items } = toRefs(props)
        const clipped = ref(true); 
        const drawer = ref(false); 
        const fixed = ref(false); 
        const miniVariant = ref(false);
        const right = ref(true); 
        const rightDrawer = ref(false);
        const filteredNavItems = ref([])
        const getUser = computed(() => $auth.user)
        const isLoggedIn = computed(() => { return $auth.loggedIn })
        const filtered = (role) => items.value.filter((v) => {
            return v.access === role;
        });
        
        const itemsArr = () => {
            switch (getUser.value.role) {
                case "user":
                    filteredNavItems.value = filtered("user");
                    break;
                case "admin":
                    filteredNavItems.value = items.value;
            }
        };
        
        return {
            clipped, drawer, fixed, miniVariant, right, rightDrawer, filteredNavItems, isLoggedIn
        }
    },
})
</script>
