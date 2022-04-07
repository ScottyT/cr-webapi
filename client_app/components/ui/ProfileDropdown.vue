<template>
    <div class="profile-menu" v-click-outside="onClickOutside">
        <div class="profile-menu__dropdown-trigger" @click="() => hidden = !hidden" >
            <span class="text text-right" v-show="$vuetify.breakpoint.width > 500">{{$auth.user.name}}</span>
            <span class="profile-menu__pfp">
                <!-- <v-icon size="45">mdi-account-circle</v-icon> -->
                <img :src="$auth.user.picture" />
            </span>
            <!-- <span class="profile-menu__pfp" v-else-if="Object.keys(avatarurl).length === 0">
                <img :src="avatarfromauth" />
            </span>
            <span class="profile-menu__pfp" v-else>
                <img :src="avatarurl" />
            </span> -->
            <i :class="`mdi-triangle mdi profile-menu__arrow ${hidden ? '' : 'open'}`"></i>
            <span class="text text--subtitle text-right" v-show="$vuetify.breakpoint.width > 600">{{$auth.user.email}}</span>
        </div>
        <div :class="`profile-menu__dropdown-menu ${hidden ? 'hide' : 'show'}`">
            <span class="text text--subtitle text-uppercase">My stuff</span>
            <nuxt-link class="profile-menu__dropdown-menu--item" :to="`/profile/user/${$auth.user.email}`">
                <v-icon>mdi-contacts</v-icon>
                <p>Profile</p>
            </nuxt-link>
            <nuxt-link class="profile-menu__dropdown-menu--item" :to="`/profile/create`" v-if="$store.state.users.user.role === 'admin'">
                <v-icon>mdi-account</v-icon>
                <p>Create Employee</p>
            </nuxt-link>
            <nuxt-link class="profile-menu__dropdown-menu--item" to="/forms">
                <v-icon>mdi-form-select</v-icon>
                <p>Forms</p>
            </nuxt-link>
            <nuxt-link class="profile-menu__dropdown-menu--item" to="/contracts">
                <v-icon>mdi-form-select</v-icon>
                <p>PDF viewable contracts</p>
            </nuxt-link>
            <a class="profile-menu__dropdown-menu--item">
                <v-icon>{{$auth.loggedIn ? 'mdi-logout-variant' : 'mdi-login-variant'}}</v-icon>
                <p @click="$auth.logout('auth0')">{{$auth.loggedIn ? "Logout" : "Login"}}</p>
            </a>
            <v-spacer class="white border mt-3 mb-3"></v-spacer>
            <v-expansion-panels accordion class="profile-menu__dropdown-section">
                <v-expansion-panel>
                    <v-expansion-panel-header class="text-uppercase profile-menu__dropdown-section--header">Guardian Resotration Contract Forms</v-expansion-panel-header>
                    <v-expansion-panel-content>
                        <nuxt-link class="profile-menu__dropdown-menu--item" to="/forms/guardian-aob-form">
                            <v-icon>mdi-form-select</v-icon>
                            <p>Guardian Resotration AOB</p>
                        </nuxt-link>
                        <nuxt-link class="profile-menu__dropdown-menu--item" to="/forms/guardian-contracting">
                            <v-icon>mdi-form-select</v-icon>
                            <p>Guardian Resotration Contracting Service Agreement</p>
                        </nuxt-link>
                        <nuxt-link class="profile-menu__dropdown-menu--item" to="/forms/guardian-scope-of-work">
                            <v-icon>mdi-form-select</v-icon>
                            <p>Guardian Resotration Contract Scope of Work</p>
                        </nuxt-link>
                    </v-expansion-panel-content>
                </v-expansion-panel>
            </v-expansion-panels>
        </div>
    </div>
</template>
<script>
import { computed, ref, onMounted, watch, defineComponent, useStore, useContext } from '@nuxtjs/composition-api'

export default defineComponent ({
    setup(props, context) {
        const { $fire } = useContext();
        const store = useStore()
        const router = context.root.$router
        
        const avatarfromauth = ref("")
        const avatarurl = ref("")
        /* const avatarurl = computed(() => store.getters['users/getAvatar'])
        const user = computed(() => store.getters['users/getUser']) */
    
        const hidden = ref(true)

        /* const fetchAvatar = () => {
            if (user.value.email !== null) {
                avatarfromauth.value = $fire.auth.currentUser.photoURL
            }
            
        } */
        const onClickOutside = () => {
            hidden.value = true
        }
        return {
            avatarfromauth,
            avatarurl,
            hidden,
            onClickOutside
        }
    },
})
</script>
<style lang="scss" scoped>
.profile-menu {
    display:flex;
    flex-direction:column;
    width:100%;
    
    @include respond(mobileSmallPort) {
        width:390px;
        position:relative;
    }

    &__dropdown-section {
        .v-expansion-panel {
           background:#333;
        }
        &--header {
            line-height:1.2;
        }
    }
    
    &__dropdown-trigger {
        cursor:pointer;
        display:grid;
        grid-template-rows:1fr 1fr;
        grid-template-columns:1fr 40px 20px;
        column-gap:15px;
        @include respond(mobileSmallPortMax) {
            display:flex;
            justify-content: flex-end;
        }
    }
    &__pfp {
        @include respond(mobileSmallPort) {
            
        }
        display:block;
        border-radius:50%;
        overflow:hidden;
        width:45px;
        height:45px;
        background-color:$dark-primary-1;
        grid-row:2 span;
    }
    &__arrow {
        grid-row:2 span;
        align-items:center;
        display:flex;
        font-size:18px;
        transform:rotate(180deg);
        transition: transform .3s ease-in-out;
        &.open {
            transform:rotate(0);
            transition: transform .3s ease-in-out;
        }
    }
    &__dropdown-menu {
        position:absolute;
        top:63px;
        background-color:#333;
        padding:5px 10px;
        width:100%;
        @include respond(mobileSmallPortMax) {
            right:0;
        }
        &.hide {
            display:none;
        }
        &.show {
            display:block;
        }
        &--item {
            font-size:1.1em;
            padding:10px 5px;
            display:flex;
            background-color:transparent;
            transition:background-color .3s ease-in-out;
            p {
                width:100%;
                margin-left:10px;
            }
            &:hover {
                background-color:$color-red;
                transition:background-color .3s ease-in-out;
            }
        }
    }
}
</style>