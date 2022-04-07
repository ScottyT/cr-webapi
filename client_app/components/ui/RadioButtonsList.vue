<template>
    <div class="form__radio-list">
        <div class="form__radio-item" v-for="(radioBtn, i) in radioArr" :key="i">
            <input @input="picked(radioBtn.value)" type="radio" :id="`${radioGroup}-${i}`" :name="radioGroup" :value="radioBtn.value" />
            <label :for="`${radioGroup}-${i}`">{{radioBtn.label}}</label>
        </div>
    </div>
</template>
<script>
import { computed, ref, defineComponent, toRefs } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        radioArr: Array,
        radioGroup: String
    },
    setup(props, {emit}) {
        const { radioArr } = toRefs(props)
        function picked(value) {
            emit('input', value)
        }

        return {
            picked
        }
    },
})
</script>
<style lang="scss" scoped>
.form {
    &__radio-list {
        display:flex;
        flex-direction:column;
        &--row {
            flex-direction: row;
            .form__radio-item {
                &:not(:first-child) {
                    padding-left:10px;
                }
            }
        }
    }
}
</style>