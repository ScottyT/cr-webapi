<template>
    <div class="form__range" :id="htmlId">
        <input :ref="slideRef" @input="getTrackPos" type="range" :min="minNum" :max="maxNum" :value="inputVal" />       
    </div>
</template>
<script>
import { defineComponent, toRefs, ref, watch } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        slideRef: String,
        htmlId: String,
        minNum: String,
        maxNum: String,
        parentInput: String
    },
    setup(props, { refs, emit }) {
        const { minNum, maxNum, slideRef, htmlId, parentInput } = toRefs(props)
        const inputVal = ref(minNum.value)
        const prefs = ref(['webkit-slider-runnable-track', 'moz-range-track', 'ms-track']);

        const getTrackPos = (el) => {
            var curVal = el.target.value,
                val = ((curVal - minNum.value) * 100) / (maxNum.value - minNum.value);
            /* for (var i = 0; i < prefs.value.length; i++) {
                styleInput += `#${htmlId.value} {background:linear-gradient(to right, #b10e0e 0%, #b10e0e ${val}%, #fff ${val}%, #fff 100%)}`;
                styleInput += `#${htmlId.value} input[type=range]::-${prefs.value[i]}{background: linear-gradient(to right, #b10e0e 0%, #b10e0e ${val}%, #ddd ${val}%, #ddd 100%)}`;
            } */
            inputVal.value = curVal
        }

        /* class Styles {
            constructor(sheet) {
                this.sheet = sheet               
                document.body.appendChild(this.sheet)
            }
            addStyles = () => {
                this.sheet.textContent = getTrackStyle(refs[slideRef.value])
            }
            removeStyles = () => {
                document.body.querySelectorAll('style').forEach(el => el.parentNode.removeChild(el));
            }
        } */
        
        watch(inputVal, (val) => {
            emit('sendInputVal', val)            
        })
        watch(parentInput, (val) => {
            inputVal.value = val
        })
        
        return {
            inputVal,
            getTrackPos
        }
    },
})
</script>
