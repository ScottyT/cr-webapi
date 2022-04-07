<template>
    <span class="base-pagination__trigger" :class="{'base-pagination__trigger--cursor':typeof newPageNumber === 'number'}" @click="onClick">
        {{ newPageNumber }}
    </span>
</template>
<script>
import { defineComponent, toRefs, computed } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        pageNumber: {
            type: String
        }
    },
    setup(props, { emit }) {
        const { pageNumber } = toRefs(props)
        const newPageNumber = computed(() => {
            var parsed = parseInt(pageNumber.value, 10)
            if (isNaN(parsed)) return pageNumber.value
            return parsed
        })
        const onClick = () => {
            if (typeof newPageNumber.value === 'string') return
            emit("loadPage", newPageNumber.value)
        }

        return {
            onClick,
            newPageNumber
        }
    },
})
</script>
