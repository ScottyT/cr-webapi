<template>
    <div class="base-pagination" :class="theme">
        <button class="button button--round button--prev" :disabled="isPrevButtonDisabled" @click="previousPage">
            <v-icon aria-hidden="false" large dark>mdi-arrow-left-circle</v-icon>
        </button>
        <div class="d-flex ml-4 mr-4 align-center" v-if="pages">
            <UiBasePaginationTrigger v-for="(paginationTrigger, i) in paginationTriggers" :key="`item-${i}-page${paginationTrigger}`" :pageNumber="paginationTrigger" 
                @loadPage="onLoadPage" class="base-pagination__description" :class="`${parseInt(paginationTrigger) === currentPage ? 'base-pagination__description--current' : ''}`" />
        </div>
        <button class="button button--round button--next" :disabled="isNextButtonDisabled" @click="nextPage">
            <v-icon aria-hidden="false" large dark>mdi-arrow-right-circle</v-icon>
        </button>
    </div>
</template>
<script>
import { computed, defineComponent, toRefs } from '@nuxtjs/composition-api'

export default defineComponent({
    props:{
        currentPage: {
            type: Number,
            required: true
        },
        visiblePagesCount: {
            type: Number,
            default: 5,
            min: 1
        },
        pageCount: {
            type: Number,
            required: true
        },
        theme: String,
        pages: {
            type: Boolean,
            default: true
        }
    },
    setup(props, { emit }) {
        const { currentPage, visiblePagesCount, pageCount } = toRefs(props)
        const isPrevButtonDisabled = computed(() => { return currentPage.value === 1 })
        const isNextButtonDisabled = computed(() => { return currentPage.value === pageCount.value })
        const paginationTriggers = computed(() => {
            const visiblePagesThreshold = (visiblePagesCount.value - 1) / 2;
            const pageCountString = pageCount.value.toString() // This is only used for displaying the numbers on the front end
            var paginationTriggersArray = Array(visiblePagesCount.value).fill(0);
            if (pageCount.value <= visiblePagesCount.value) {
                paginationTriggersArray = Array(pageCount.value).fill(0)
                paginationTriggersArray[0] = 1
                const pTriggers = paginationTriggersArray.map((pTrigger, i) => {
                    return (paginationTriggersArray[0] + i).toString()
                })
                return pTriggers
            }
            if (currentPage.value <= visiblePagesThreshold + 1) {
                paginationTriggersArray[0] = 1
                const pTriggers = paginationTriggersArray.map((pTrigger, i) => {
                    return (paginationTriggersArray[0] + i).toString()
                })
                pTriggers[visiblePagesCount.value - 1] = pageCountString
                pTriggers[visiblePagesCount.value - 2] = '...'
                return pTriggers
            }
            if (currentPage.value >= pageCount.value - visiblePagesThreshold) {
                const pTriggers = paginationTriggersArray.map((pTrigger, i) => {
                    return (pageCount.value - i).toString()
                }).reverse()
                
                pTriggers[0] = '1'
                pTriggers[1] = '...'
                return pTriggers
            }

            paginationTriggersArray[0] = currentPage.value - (visiblePagesThreshold);
            const pTriggers = paginationTriggersArray.map((pTrigger, i) => {
                var newArr = paginationTriggersArray[0] + i
                return newArr.toString()
            })
            pTriggers[0] = '1'
            pTriggers[1] = '...'
            pTriggers[pTriggers.length - 1] = pageCountString
            pTriggers[visiblePagesCount.value - 2] = "..."
            return pTriggers
        })

        const onLoadPage = (value) => {
            emit("loadPage", {lastpage: currentPage.value, currentpage: value})
        }
        const previousPage = () => {
            emit("previousPage")
        }
        const nextPage = () => {
            emit("nextPage")
        }
        return {
            onLoadPage,
            paginationTriggers,
            isPrevButtonDisabled,
            isNextButtonDisabled,
            previousPage,
            nextPage
        }
    },
})
</script>
<style lang="scss" scoped>
.base-pagination {
    display:flex;
    justify-content: center;
    
    &__description {
        box-shadow:0 0 6px 2px rgba($color-black, .2);
        height:35px;
        width:35px;
        display:flex;
        //color:$color-black;
        align-items:center;
        justify-content: center;
        &:not(:first-child) {
            margin-left:10px;
        }

        &--current {
            background:$color-red;
            color:white;
        }
    }

    &__trigger {
        cursor:default;
        &--cursor {
            cursor:pointer;
        }
    }

    &--image-slider {
        width:100%;
        display:flex;
        position:absolute;
        align-items:center;
        height:100%;
        top:0;
        & > .button {
            position:absolute;
            background:transparent;
            border:none;
            height:100%;
            max-width:50%;
            display:flex;
            align-items:center;
            .v-icon {
                filter:drop-shadow(0px 0px 9px black);
            }
            &--prev {
                left:calc(10vw * .1);
                justify-content: flex-start;
            }
            &--next {
                right:calc(10vw * .1);
                justify-content: flex-end;
            }
        }
        [disabled] {
            display:none;
        }
    }
}
</style>