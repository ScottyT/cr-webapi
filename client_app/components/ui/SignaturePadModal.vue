<template>
  <ValidationProvider v-if="dialog" v-slot="{errors}" :rules="notrequired ? '' : 'required'" :name="name" class="form__input-group sig-pad-wrapper"
     :class="{modalOpen: sigDialog}">
    <label class="form__label">{{name}}</label>
    <input v-if="!notrequired" type="hidden" v-model="signage" />
    <v-dialog v-model="sigDialog" v-if="sigData.data.value === '' || sigType === 'customer'" :width="width" :height="height">
      <template v-slot:activator="{ on, attrs }">
        <div class="button--normal button" v-bind="attrs" v-on="on">{{!signed ? 'Click to sign' : 'Signed'}}</div>
      </template>
      <VueSignaturePad :width="width" :height="height" id="sigPad" :ref="sigRef" :options="{ onBegin, onEnd }" />
      <div class="modal__footer">
        <button type="button" class="button" @click="clear">Clear</button>
        <button type="button" @click="save" :class="`button`">{{ sigData.data !== '' ? 'Signed' : 'Sign' }}</button>
      </div>
    </v-dialog>
    <div id="populate" v-else class="button--normal button" @click="populateField">{{!signed ? 'Click to sign' : 'Signed'}}</div>
    <span class="form__input--error">{{ errors[0] }}</span>
    <div :style="`max-width:${width}; height:${height};width:100%;`" v-if="signed && typeof sigData.data === 'object'">
      <img style="object-fit:contain;" :src="sigData.data.value" />
    </div>
    <div :style="`max-width:${width}; height:${height};width:100%;`" v-else-if="signed">
      <img style="object-fit:contain;" :src="sigData.data" />
    </div>
  </ValidationProvider>
  <ValidationProvider v-else v-slot="{errors}" :vid="inputId" :rules="notrequired ? '' : 'required'" :ref="inputId" :name="name" class="form__input-group">
    <label class="form__label">{{name}}</label>
    <input v-if="!notrequired" type="hidden" v-model="signage" />
    <div class="sig-pad" :style="`max-width:${width};`" v-if="sigData.data === ''">
      <VueSignaturePad class="form__input" :width="width" :height="height" id="sigPad" :ref="sigRef" :options="{ onBegin, onEnd }" />
      <div class="sig-pad__footer" >
        <button type="button" class="button" @click="clear">Clear</button>
        <button type="button" :disabled="sigData.isEmpty" @click="save" :class="`button ${sigData.isEmpty ? 'button--disabled' : ''}`">{{ sigData.data !== '' ? 'Signed' : 'Sign' }}</button>
      </div>
    </div>
    <div id="populate" v-else class="button--normal button" @click="populateField">{{!signed ? 'Click to sign' : 'Signed'}}</div>
    <div :style="`max-width:${width}; height:${height};width:100%;`" v-if="signed && typeof sigData.data === 'object'">
      <img style="object-fit:contain;" :src="sigData.data.value" />
    </div>
    <div :style="`max-width:${width}; height:${height};width:100%;`" v-else-if="signed && typeof sigData.data === 'string'">
      <img style="object-fit:contain;" :src="sigData.data" />
    </div>
    <span class="form__input--error">{{ errors[0] }}</span>
  </ValidationProvider>
</template>
<script>
import { defineComponent, onMounted, watch, computed, ref, toRefs, useContext, useStore } from "@nuxtjs/composition-api";
import axios from 'axios';
export default defineComponent({
  props: {
    sigData: {
      type: Object,
      required: true
    },
    sigRef: {
      type: String,
      required: true
    },
    name: {
      type: String,
      default: "Signature"
    },
    inputId: {
      type: String
    },
    width: {
      type: String
    },
    height: {
      type: String
    },
    initial: {
      type: Boolean,
      default: false
    },
    sigType: String,
    dialog: Boolean,
    notrequired: Boolean
  },
  setup(props, {refs, emit}) {
    const { sigData, sigRef, sigType, inputId, initial } = toRefs(props)
    const { $fire, $fireModule } = useContext()
    const store = useStore()
    const sigDialog = ref(false)
    const errors = ref({})
    const signed = ref(false)
    const signage = ref("")
    const user = computed(() => store.getters['users/getUser'])
    const sigImage = computed(() => store.getters["users/getSignature"])
    const initialImage = computed(() => store.getters["users/getInitial"])

    const uploadFile = async (filename) => {
      const storageRef = $fire.storage.refFromURL(`gs://${process.env.userStorage}`).child(`${user.value.email}/${filename}.jpg`);
      const data = {
        "teamMember": user.value.email,
        "signature": sigData.value.data
      }
      
      storageRef.putString(data.signature, 'data_url').then((snapshot) => {
        store.dispatch('users/getSigOrInitialImage', snapshot.ref.name)
      })
    }
    const fetchSigImages = () => {
      if (initial.value && sigType.value === 'employee') {
        sigData.value.data = initialImage
      } 
      if (sigType.value === 'employee') {
        sigData.value.data = sigImage
      }
    }
    const populateField = () => {
      if (sigType.value == 'employee') signed.value = true
      if (sigType.value == 'customer') signed.value = !signed.value
      signage.value = sigData.value.data
      emit('input', sigData.value.data)
    }
    const clear = () => {
      refs[sigRef.value].clearSignature();
      //signed.value = false
      sigData.value.isEmpty = true
      sigData.value.data = ""
    }
    const save = () => {
      const { data, isEmpty } = refs[sigRef.value].saveSignature();
      if (sigData.value.isEmpty) return;     
      sigData.value.data = data;
      sigData.value.isEmpty = isEmpty
      sigDialog.value = false
      populateField()
      
      if (initial.value && sigType.value === 'employee') {
        uploadFile("initial")
      } else if (sigType.value === 'employee') {
        uploadFile("signature")
      }
    }
    const onEnd = () => {
      sigData.value.isEmpty = false
    }
    if (sigImage !== "") {
      onMounted(fetchSigImages)
    }
    // this might be breaking something but i don't know what it is
    watch(() => sigData.value.data, (val) => {
      if (sigType.value == 'customer') {
        signed.value = true
        signage.value = val
      }
    })
    return {
      user,
      sigDialog,
      sigImage,
      signage,
      signed,
      errors,
      save,
      clear,
      populateField,
      onEnd
    }
  },
  methods: {
    onBegin() {
      const { isEmpty } = this.$refs[this.sigRef].saveSignature();
      this.sigData.isEmpty = isEmpty
      this.$nextTick(() => {
        this.$refs[this.sigRef].resizeCanvas()
      })
    }
  }
})
</script>
<style lang="scss">
#sigPad {
   background-color:white;
   padding:0!important;
}
.sig-pad-wrapper {
  grid-column:1/3 span;
}
.form__input {
  &--initial {
    width:200px;
    height:70px;
  }
}
.sig-pad {
  &__footer {
    display:grid;
    grid-template-columns:1fr 1fr;
    column-gap:10px;
  }
}
</style>