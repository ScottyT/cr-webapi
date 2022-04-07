import { Scatter, mixins, Bar } from 'vue-chartjs'
import Vue from 'vue';

Vue.component("scatter-chart", {
    props: ['chartdata', 'options'],
    extends: Scatter,
    mounted() {
        this.renderScatterChart()
    },
    computed: {
        chartData() {
            return this.chartdata
        }
    },
    methods: {
        // each dataset will be for each day the chart is updated
        renderScatterChart: function() {
            /* this.renderChart({
                labels: [],
                datasets: [
                    { data: this.chartData[0], label: this.chartData[0].date, backgroundColor: '#000000'},
                    { data: this.chartData[1], label: this.chartData[0].date, backgroundColor: '#900C3F'},
                    { data: this.chartData[2], label: this.chartData[0].date, backgroundColor: '#FF5733'},
                ]
            }, this.options) */
            this.renderChart({
                labels: [],
                datasets: this.chartData
            }, this.options)
        }
    },
    watch: {
        chartdata() {
            this.$data._chart.destroy();
            this.renderScatterChart()
            this.$data._chart.update();
        }
    }
});

Vue.component("bar-chart", {
    props: ['bardata', 'baroptions'],
    extends: Bar,
    mounted() {
        this.renderBarChart()
    },
    computed: {
        barchartData() {
            return this.bardata
        }
    },
    methods: {
        renderBarChart: function() {
            this.renderChart(this.barchartData, this.baroptions)
        }
    },
    watch: {
        bardata() {
            this.$data._chart.destroy();
            this.renderBarChart()
            this.$data._chart.update();
        }
    }
})