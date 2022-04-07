<template>
    <p v-if="$fetchState.pending">Fetching reports...</p>
    <div class="pa-6 reports-wrapper" v-else>
        <div class="block-group" v-for="(item, i) in groupedReports" :key="i">
            <h3 class="reports-wrapper__heading">{{item[0].heading}}</h3>
            <LazyLayoutReports :fieldJacket="false" :reports="item" theme="dark" />
        </div>
    </div>
</template>
<script>
import { computed, defineComponent, useStore } from '@nuxtjs/composition-api'
import useReports from '~/composable/reports'
export default defineComponent({
    middleware: 'auth',
    setup() {
        const { filterConditions, getReports, groupedReports } = useReports()
        const filterarr = ["aob", "coc", "contracting-agreement", "scope-of-work"]
        filterConditions(filterarr)
        getReports(false, true, true)
        
        return {
            groupedReports
        }
    },
})
</script>
