<template>

    <div class="login-page form-wrapper form-wrapper__login-page">
          <!-- <v-overlay :value="$store.state.users.loading" v-if="$store.state.users.loading">
      <v-progress-circular
          indeterminate
          size="64"
        ></v-progress-circular>
    </v-overlay> -->
      <div class="login-form-box" v-if="!this.$auth.loggedIn">       
        <h1>Login!</h1>
        <a @click="signin">Sign in</a>
        
        <!-- <p v-show="$store.state.users.error !== ''">{{$store.state.users.error}}</p>
        <ValidationObserver v-slot="{ handleSubmit }">
          <form ref="loginForm" class="form" method="post" @submit.prevent="handleSubmit(signIn)">
            <div class="form__form-group--block">
              <ValidationProvider v-slot="{ errors }" name="Email" rules="required|email" class="form__input--input-group-simple">
                <label for="email" class="form__label">Email</label>
                <input type="email" class="form__input" name="email" id="email" v-model="email" />
                <span class="form__input--error">{{errors[0]}}</span>
              </ValidationProvider>
              <ValidationProvider v-slot="{ errors }" name="Password" rules="required" class="form__input--input-group-simple">
                <label for="pass" class="form__label">Password</label>
                <div class="form__input form__input--password-input">
                  <input :type="passwordVisibility ? 'text' : 'password'" class="form-input" id="pass" name="Password" v-model="password" />
                  <i :class="`form__input--icon mdi ${passwordVisibility ? 'mdi-eye-off' : 'mdi-eye'}`" @click="passwordVisibility = !passwordVisibility"></i>
                </div>
                <span class="form__input--error">{{errors[0]}}</span>
              </ValidationProvider>
            </div>
            <div class="submit-btn">
              <v-btn type="submit" class="button--normal" :dark="matchUrl ? true : false">{{ submitting ? 'Logging In' : 'Login' }}</v-btn>
            </div>
          </form>
        </ValidationObserver> -->
      </div>
      <div v-else>
        <!-- <p>
          Hi, {{jwt_decoded.given_name}}
        </p> -->
        <code><strong>user profile:</strong> {{ $auth.user }}</code>
        <a @click="$auth.logout()">Sign out</a>
      </div>
    </div>
</template>
<script>
import { computed, defineComponent, onMounted, ref, useContext, useMeta, useStore } from '@nuxtjs/composition-api';
export default defineComponent({
  head: {},
  setup(props, context) {
    const { route, $fire, $auth } = useContext()
    const { title } = useMeta()
    console.log(context)
    const router = context.root.$router
    const submitted = ref(false)
    const submitting = ref(false)
    const message = ref("")
    const idNumber = ref("")
    const password = ref("")
    const email = ref("")
    const passwordVisibility = ref(false)
    const errorMessage = ref("")
    const jwt_decoded = $auth.$storage.getUniversal('jwt_decoded')
    const store = useStore()
    const getUser = computed(() => store.getters["users/getUser"])
    const matchUrl = computed(() => {
      return route.value.path.match(/^(?:^|\W)reports(?:$|\W)(?:\/(?=$))?/gm)
    })
    const signin = () => {
      $auth.login().then(() => {
        console.log("logged in")
      })
    }
    const signIn = async () => {
      submitting.value = true
      store.dispatch('users/signin', { email, password }).then(() => {
        submitting.value = false
        //router.go()
      })
    }

    title.value = 'Login'

    return {
      password, email, errorMessage, passwordVisibility, submitting,
      matchUrl,
     // signIn,
     signin,
      jwt_decoded
    }
  }
})
</script>