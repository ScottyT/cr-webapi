<template>
    <v-dialog :ref="dialogId" v-model="dialogToggle" persistent
              :return-value.sync="date" transition="scale-transition" :max-width="maxWidth">
        <template v-slot:activator="{ on, attrs }">
            <input type="text" :id="dateId" v-model="formattedDate" class="form__input form__input--short" v-bind="attrs" readonly v-on="on" />
            <span class="button" @click="date = null">clear</span>
        </template>
        <v-date-picker class="date-picker" v-if="dialogToggle" v-model="date" scrollable>
            <v-spacer></v-spacer>
            <v-btn text color="#fff" @click="dialogToggle = false">Cancel</v-btn>
            <v-btn text color="#fff" @click="$refs[dialogId].save(date)">OK</v-btn>
        </v-date-picker>
    </v-dialog>
</template>
<script>
import { defineComponent, toRefs, ref, watch, onMounted, computed } from '@nuxtjs/composition-api'
import genericFuncs from '~/composable/utilityFunctions'
export default defineComponent({
    props: {
        d: String,
        minDate: String,
        maxDate: String,
        dateId: String,
        dialogId: String,
        maxWidth: {
            default: "320px",
            type: String
        }
    },
    setup(props, {refs, emit}) {
        const { d } = toRefs(props)
        const { formatDate } = genericFuncs()
        const dialogToggle = ref(false)
        const formattedDate = ref(formatDate(new Date().toISOString().substr(0, 10)))
        const date = ref(new Date().toISOString().substr(0, 10))
        watch(date, (val) => {
            if (val === null) {
                formattedDate.value = "N/A"
                emit("date", "N/A")
            } else {
                formattedDate.value = formatDate(val)
                emit("date", formattedDate.value)
            }
        })
        watch(d, (val) => {
            formattedDate.value = val
            emit("date", val)
        })
        return {
            dialogToggle,
            formattedDate,
            date
        }
    },
})
</script>
<style lang="scss">
.date-picker {
    [disabled] {
        background:transparent!important;
    }
    button {
        .v-btn__content {
            color:inherit;
        }
    }
}
</style>