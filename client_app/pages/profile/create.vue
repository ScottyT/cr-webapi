<template>
    <div class="form-wrapper create-user">
        <div class="create-user__box">
            <h1>Create User</h1>
            <div v-for="(message, i) in submittedMessage" :key="i">
                <p class="font-weight-bold">{{message}}</p>
            </div>
            <p>{{successMessage}}</p>
            <p class="alert alert--error">{{errorMessage}}</p>
            <ValidationObserver ref="createUser" v-slot="{ errors, handleSubmit }">
                <v-dialog width="400px" v-model="errorDialog">
                    <div class="modal__error">
                        <div v-for="(error, i) in errors" :key="`error-${i}`">
                            <h3 class="form__input--error">{{ error[0] }}</h3>
                        </div>
                    </div>
                </v-dialog>
                <form class="form" @submit.prevent="handleSubmit(onSubmit)">
                    <ValidationProvider vid="fname" rules="required" name="First name" v-slot="{errors}" class="form__input--input-group-simple">
                        <input v-model="fname" type="text" placeholder="First name" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="lname" rules="required" name="Last name" v-slot="{errors}" class="form__input--input-group-simple">
                        <input v-model="lname" type="text" placeholder="Last name" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="username" rules="required" name="Username" v-slot="{errors}" class="form__input--input-group-simple">
                        <input v-model="username" type="text" placeholder="Username" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="email" rules="required" name="Email" v-slot="{errors}" class="form__input--input-group-simple">
                        <input v-model="email" type="text" placeholder="Email" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="id" name="ID" rules="required" v-slot="{errors}" class="form__input--input-group-simple">
                        <input v-model="id" type="number" placeholder="ID" class="form__input" />
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="password" rules="required" name="Password" v-slot="{errors}" class="form__input--input-group-simple">
                        <div class="form__input form__input--password-input">
                            <input v-model="password" :type="passwordVisibility ? 'text' : 'password'" placeholder="Password" class="form-input" />
                            <i :class="`form__input--icon mdi ${passwordVisibility ? 'mdi-eye-off' : 'mdi-eye'}`" @click="passwordVisibility = !passwordVisibility"></i>
                        </div>
                        <span class="form__input--error">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <ValidationProvider vid="role" name="Role" rules="required" v-slot="{errors, ariaMsg}" class="form__input-group form__input-group--long">
                        <input v-model="role" type="hidden" />
                        <label class="form__label">Role:</label>
                        <i class="form__select--icon icon--angle-down mdi" aria-label="icon"></i>
                        <select class="form__input" v-model="role">
                            <option disabled value="">Please select a role for this user</option>
                            <option v-for="(item, i) in roles" :key="`role-${i}`">{{item}}</option>
                        </select>
                        <span class="form__input--error" v-bind="ariaMsg">{{ errors[0] }}</span>
                    </ValidationProvider>
                    <button type="submit" class="button button--normal">{{ submitting ? 'Submitting' : 'Create' }}</button>
                </form>
            </ValidationObserver>
        </div>
    </div>
</template>
<script>
import { defineComponent, onMounted, ref, useContext, useMeta, useStore } from '@nuxtjs/composition-api';
export default defineComponent({
    head:{},
    middleware: 'auth',
    setup(props, {refs}) {
        useMeta({ title: 'Create Employee'})
        const store = useStore();
        const { $axios, $auth, $api } = useContext()
        const errorMessage = ref('')
        const fname = ref('')
        const lname = ref('')
        const email = ref('')
        const username = ref("")
        const id = ref('')
        const role = ref('')
        const submittedMessage = ref([])
        const errorDialog = ref(false)
        const password = ref('')
        const passwordVisibility = ref(false)
        const successMessage = ref('')
        const submitting = ref(false)
        const roles = [
            "user", "admin"
        ]
        const signupUser = async () => {
            return new Promise((resolve, reject) => {
                $api.$post("/api/employees/auth/create", {
                    fname: fname.value,
                    lname: lname.value,
                    email: email.value,
                    password: password.value,
                    username: username.value,
                    role: role.value,
                    team_id: id.value
                }).then((res) => {
                    errorMessage.value = ""
                    console.log(res)
                    resolve(`Successfully registered ${res.name}`)
                }).catch(err => {
                    reject(err)
                })
            })
        }
        const onSubmit = async () => {
            var post = {
                fname: fname.value,
                lname: lname.value,
                email: email.value,
                username: username.value,
                role: role.value,
                team_id: id.value
            }
            submitting.value = true
            await refs.createUser.validate().then(success => {
                if (!success) {
                    errorDialog.value = true
                    return
                }
                $api.$post("/api/employees/auth/create", {
                    fname: fname.value,
                    lname: lname.value,
                    email: email.value,
                    password: password.value,
                    username: username.value,
                    role: role.value,
                    team_id: id.value
                }).then((res) => {
                    errorMessage.value = ""
                    submittedMessage.value.push(res)
                    submitting.value = false
                    resolve(`Successfully registered ${res.name}`)
                }).catch(err => {
                    submitting.value = false
                    if (err.response) {
                        errorMessage.value = err.response.data.message
                    }
                })
                /* signupUser().then((result) => {
                    $api.$post("/api/employees/create", post).then((res) => {
                        submittedMessage.value.push(res, result)
                        setTimeout(() => {
                            window.location = "/"
                        }, 3000)
                    }).catch(err => {
                        console.error(err)
                    })
                }).catch(err => {
                    if (err.response) {
                        errorMessage.value = err.response.data.message
                    }
                }) */
            })
        }

        return {
            fname, lname, email, password, errorMessage, submittedMessage, successMessage, role, id, passwordVisibility, username,errorDialog, roles,
            onSubmit, submitting
        }
    }
})
</script>