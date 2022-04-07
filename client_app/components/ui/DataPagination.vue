<template>
    <div class="data-list" :class="`data-list--${themeMode}`">
        <transition :name="direction" class="data-block" mode="out-in">
            <div :class="itemStyle" :key="`item-${cPage - 1}`" ref="report" @touchstart="touchStart" @touchend="touchEnd">
                <div :class="`data-list__data data-list__data--${themeMode}`" v-for="item in reportsPerPage[cPage - 1]" 
                    :key="`${item.formType}-${item.ReportType}-${item.JobId}-${item.Id}`">
                    <slot name="content" v-bind:report="item">{{item}}</slot>
                </div>
            </div>
        </transition>
        <UiBasePagination :pageCount="reportsPerPage.length" :currentPage="cPage" :visiblePagesCount="visiblePageNumbers" :theme="theme" :pages="!imageSlider"
            @nextPage="pageChangeHandle('next')" @previousPage="pageChangeHandle('previous')" @loadPage="pageChangeHandle" />
    </div>
</template>
<script>
import { defineComponent, toRefs, computed, ref, onMounted, watch } from '@nuxtjs/composition-api'

export default defineComponent({
    props: {
        items: Array,
        itemsPerPage: Number,
        itemStyle: String,
        visiblePageNumbers: Number,
        themeMode: String,
        curPage: {
            type: Number,
            default: 1
        },
        imageSlider: Boolean
    },
    setup(props, { emit, refs }) {
        const { items, itemsPerPage, curPage, imageSlider } = toRefs(props)
        const direction = ref("")
        const cPage = ref(curPage.value)
        const visiblePages = ref(0)
        const start = ref(0)
        
        const currentPageView = computed({
            get: () => cPage.value,
            set: val => { cPage.value = val }
        })
        const theme = computed(() => {
            if (imageSlider.value) {
                return 'base-pagination--image-slider'
            }
        })
        const reportsPerPage = computed(() => {
            if (items.value === null) return
            const res = []
            for (let i = 0; i < items.value.length; i += itemsPerPage.value) {
                const chunk = items.value.slice(i, i + itemsPerPage.value);
                res.push(chunk)
            }
            return res
        })

        const touchStart = (e) => {
            start.value = e.touches[0].clientX
        }
        const touchEnd = (e) => {
            var end = e.changedTouches[0].clientX
            if (start.value - 110 > end && currentPageView.value !== visiblePages.value) {
                direction.value = 'slide-fade-next'
                currentPageView.value += 1
            }
            if (start.value + 110 < end && currentPageView.value !== 1) {
                direction.value = 'slide-fade-previous'
                currentPageView.value -= 1
            }
        }
        function repPerPage() {
            if (items.value === null) return
            visiblePages.value = Math.ceil((items.value.length / 4))
        }
        function pageChangeHandle(value) {
            switch (true) {
                case value === "next":
                    direction.value = 'slide-fade-next'
                    cPage.value += 1
                    break;
                case value === "previous":
                    direction.value = 'slide-fade-previous'
                    cPage.value -= 1;
                    break;
                case typeof value === 'object':
                    if (value.currentpage > value.lastpage) {
                        direction.value = 'slide-fade-next'
                    }
                    if (value.currentpage < value.lastpage) {
                        direction.value = 'slide-fade-previous'
                    }
                    cPage.value = value.currentpage
                    break;
                default:
                    direction.value = 'data-list'
                    cPage.value = value
                    break;
            }
        }
        onMounted(repPerPage)
        watch(reportsPerPage, (val) => {
            if (val.length < cPage.value) {
                cPage.value = val.length
            }
        })
        watch(() => curPage.value, (newCurPage, oldCurPage) => {
            direction.value = 'data-list'
            currentPageView.value = newCurPage
        })
        return {
            reportsPerPage,
            currentPageView,
            cPage,
            visiblePages,
            direction,
            pageChangeHandle,
            touchEnd,
            touchStart,
            theme
        }
    },
})
</script>
<style lang="scss" scoped>
.data-list {
    display:flex;
    flex-direction: column;
    row-gap:40px;
    &--dark {
        color:$color-white;
    }
    &--light {
        color:$color-black;
    }
    &--image-slider {
        display:inline-flex;
        width:100%;
        height:59vh;
        align-items:center;
        position:relative;
        @include respond(tabletLarge) {
            height:600px;
        }
    }
    &--flex {
        display:flex;
        flex-wrap:wrap;
        row-gap:40px;
        column-gap:25px;
        height:100%;
        @include respond(mobileSmallPort) {
            column-gap:40px;
        }
    }
    &--grid {
        display:grid;
        grid-template-columns:repeat(auto-fill, minmax(114px, 1fr));
        grid-template-rows:repeat(auto-fill, minmax(101px, 1fr));
        gap:25px;
        @include respond(mobileSmallPort) {
            grid-template-columns:repeat(auto-fill, minmax(165px, 1fr));
            grid-template-rows:repeat(auto-fill, minmax(120px, 1fr));
            gap:40px;
        }
    }
    &__data {
        box-shadow: -1px 2px 12px -2px rgba($color-black, .8);
        display: flex;
        flex-direction: column;
        max-width:200px;
        width:100%;

        a {
            width: 100%;
            display: block;
            height: 100%;
        }

        &--dark {
            background-color: #333;
        }

        &--light {
            background-color: $color-white;
        }
        &--image-slider {
            display:flex;
            max-width:700px;
            height:100%;
            box-shadow: none;
            align-items:flex-end;
        }
    }
}
.slide-fade-next-enter-active, .slide-fade-next-leave-active {
  transition: all .4s cubic-bezier(1.0, 0.5, 0.8, 1.0);
}
.slide-fade-next-enter, .slide-fade-next-leave-to
/* .slide-fade-leave-active below version 2.1.8 */ {
  transform: translateX(-100px);
  opacity: 0;
}
.slide-fade-next-enter {
    transform:translateX(100px);
}
.slide-fade-previous {
  position:relative;
}
.slide-fade-previous-enter-active,  .slide-fade-previous-leave-active  {
    transition: all .4s cubic-bezier(1.0, 0.5, 0.8, 1.0);
}
.slide-fade-previous-enter, .slide-fade-previous-leave-to {
    opacity: 0;
    transform:translateX(100px);
    //position:absolute;
}
.slide-fade-previous-enter {
    transform:translateX(-100px);
}
</style>